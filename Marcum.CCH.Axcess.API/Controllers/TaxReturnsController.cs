using System.Diagnostics;
using System.Text;
using System.Web;
using Marcum.CCH.Axcess.API.Helpers;
using Marcum.CCH.Axcess.API.Statics;
using Marcum.CCH.Axcess.Domain.Models;
using Marcum.CCH.Axcess.Infrastructure.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;



namespace Marcum.CCH.Axcess.API.Controllers;

[Route("")]
[ApiController]
public class TaxReturnsController : Controller
{ 
    private readonly ILogger<TaxReturnsController> _logger;
    private readonly CchSettingsOptions _cchSettingsOptions;
    private readonly ProgramOptions _programOptions;
    private readonly AuthorizationTokenOptions _authorizationTokenOptions;    
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TokenProvider _tokenProvider;
    
    public TaxReturnsController(IConfiguration config,
        ILogger<TaxReturnsController> logger,
        IOptions<CchSettingsOptions> cchSettingsOptions,
        IOptions<ProgramOptions> programOptions,
        IOptions<AuthorizationTokenOptions> authorizationTokenOptions,
        IHttpClientFactory httpClientFactory,
        TokenProvider tokenProvider
        )
    {
        _logger = logger;
        _cchSettingsOptions = cchSettingsOptions.Value;
        _programOptions = programOptions.Value;
        _authorizationTokenOptions = authorizationTokenOptions.Value;
        _httpClientFactory = httpClientFactory;
        _tokenProvider = tokenProvider;
        
    }
    [Authorize]
    [HttpGet(ApiEndPoints.TaxReturns.RequestNewAuthorizationTicket, Name = nameof(RequestNewAuthorizationTicket))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<string> RequestNewAuthorizationTicket(CancellationToken cancellationToken)
    {
        
        try
        {
                       
            var requestParams = string.Format("?response_type={0}&client_id={1}&redirect_uri={2}&scope={3}",
               "code",
                _cchSettingsOptions.ClientId,
                String.Format("{0}{1}", _programOptions.Domain, _cchSettingsOptions.RedirectUrl),
                _cchSettingsOptions.Scopes);
                                  
            return await Task.FromResult($"{_cchSettingsOptions.LoginUrl}{requestParams.ToString()}");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting login url");
            return await Task.FromResult($"Error getting login url Error:{ex.InnerException}");
        }
    }

    [Authorize]
    [HttpGet(ApiEndPoints.TaxReturns.RequestRefreshAuthorizationTicket, Name = nameof(RequestRefreshAuthorizationTicket))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<string> RequestRefreshAuthorizationTicket(CancellationToken cancellationToken)
    {    
        try
        {           
            OAuthTicket ticket = GetAuthTokenFromFile();

            if (ticket is null || string.IsNullOrEmpty(ticket.refresh_token))
                return await Task.FromResult($"Authorization ticket was not initialized.");

            using (var client = _httpClientFactory.CreateClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, string.Format("{0}", _cchSettingsOptions.RefreshUrl));
                request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_cchSettingsOptions.ClientId}:{_cchSettingsOptions.ClientSecret}"))}");

                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new("refresh_token", ticket.refresh_token));
                collection.Add(new("redirect_uri", _cchSettingsOptions.RefreshUrl));
                collection.Add(new("grant_type", "refresh_token"));

                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var refreshResponse = System.Text.Json.JsonSerializer.Deserialize<CchTokenRefreshResponse>(responseBody);

                    if(refreshResponse is null ||  (string.IsNullOrEmpty(refreshResponse.access_token) || string.IsNullOrEmpty(refreshResponse.refresh_token)))
                        return "response is null";

                    OAuthTicket newAuthTicket = new OAuthTicket(refreshResponse.access_token, refreshResponse.refresh_token, ticket.IssuedAt, DateTime.Now);
                                        
                    SaveAuthTokenToFile(newAuthTicket);
                   
                    return newAuthTicket.ToString();
                    
                }
                else
                    return $"Error: {response}";

            }
                           
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refresh url");
            return $"Error getting refresh url Error:{ex.InnerException}";
        }
    }
    
    [HttpGet(ApiEndPoints.TaxReturns.AuthCallbackRefresh, Name = nameof(AuthCallbackRefresh))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ContentResult> AuthCallbackRefresh([FromQuery] string code, CancellationToken cancellationToken)
    {
        return await Task.FromResult(base.Content("Done"));
    }

    [HttpGet(ApiEndPoints.TaxReturns.AuthCallback, Name = nameof(AuthCallback))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ContentResult> AuthCallback([FromQuery] string code, CancellationToken cancellationToken)
    {

        _logger.LogInformation("STEPS: In AuthCallback");
        
        StringContent content = new StringContent(string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type={4}",
            code,
            _cchSettingsOptions.ClientId,
            _cchSettingsOptions.ClientSecret,
             String.Format("{0}{1}", _programOptions.Domain, _cchSettingsOptions.RedirectUrl),
           "authorization_code"));
               
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                
        try
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                _logger.LogInformation("STEPS: In AuthCallback: using client");

                var result = client.PostAsync(_cchSettingsOptions.TokenUrl, content, cancellationToken).Result;
                string resultContent = result.Content.ReadAsStringAsync(cancellationToken).Result;
                var authenticationTicket = System.Text.Json.JsonSerializer.Deserialize<OAuthTicket>(resultContent);
                
                if (authenticationTicket != null && !string.IsNullOrEmpty(authenticationTicket.access_token))
                {
                    authenticationTicket.IssuedAt = DateTime.Now;
                    authenticationTicket.RefreshedAt = DateTime.Now;

                    _logger.LogInformation("STEPS: authenticationTicket is not null");
                    SaveAuthTokenToFile(authenticationTicket);

                }
                else
                {
                    _logger.LogInformation("STEPS: authenticationTicket is null");
                    return await Task.FromResult(base.Content($"Failed to get token: ${result}"));                 
                }              
                return await Task.FromResult(base.Content("<h1>Auth Success close this window.</h1>", "text/html"));

            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error CCAUthorization Error:{Message}", ex.Message);
            return await Task.FromResult(base.Content($"Error CCAUthorization Error:{ ex.InnerException}", "text /html"));
        }      
    }

    [Authorize]
    [HttpGet(ApiEndPoints.TaxReturns.GetAuthorizationToken, Name = nameof(GetAuthorizationToken))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<OAuthTicket> GetAuthorizationToken()      
        => await Task.FromResult(await EnsureValidAuthToken());

    [HttpGet(ApiEndPoints.TaxReturns.GetCCHAuthorizationToken, Name = nameof(GetCCHAuthorizationToken))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<OAuthTicket> GetCCHAuthorizationToken()
    {        
        return Task.FromResult(_tokenProvider.CreateToken());
    }

    [Authorize]
    [HttpGet(ApiEndPoints.TaxReturns.GetElfStatusHistoryByReturnId, Name = nameof(GetElfStatusHistoryByReturnId))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]    
    public async Task<IActionResult> GetElfStatusHistoryByReturnId(string returnId, CancellationToken cancellationToken)
    {               
        try
        {                        
            OAuthTicket tokens = await EnsureValidAuthToken(cancellationToken);
            if (!String.IsNullOrEmpty(tokens.access_token))
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    
                    var baseUrl = _cchSettingsOptions.ElfHistoryByReturnIdUrl;                
                    var uriBuilder = new UriBuilder(baseUrl);                
                    var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                    query["ReturnId"] = returnId;
                    uriBuilder.Query = query.ToString();
                    
                    var url = uriBuilder.Uri;
                    ApiHelper.AddStandardHeaders(client, _cchSettingsOptions.IntegrationKey, tokens.access_token);                    
                    _logger.LogInformation("Getting Status of ReturnId: {ReturnId}", returnId);

                    var result = await client.GetAsync(url, cancellationToken);                   
                    var response = await ApiHelper.HandleHttpResponseAsync<ElfStatusHistoryResponse>(
                        result,
                        url.ToString(),
                        cancellationToken);
                   
                    if (response is not OkObjectResult && response is ObjectResult objectResult)
                    {
                        _logger.LogInformation("Issue in GetElfStatusHistoryByReturnId for StatusCode={StatusCode} ReturnId={ReturnId} Value={Value}",
                            objectResult.StatusCode,
                            returnId,
                            objectResult.Value);
                    }

                    return response;                   
                }
            }
            else
            {
                _logger.LogError("{Message}", Constants.ErrorMessages.AuthNotInitialized);
                return Ok(Constants.ErrorMessages.AuthNotInitialized);
            }            
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost(ApiEndPoints.TaxReturns.GetReturnsExportBatchPDF, Name = nameof(GetReturnsExportBatchPDF))]   
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReturnsExportBatchPDF(string returnType, List<string> returnIds, CancellationToken cancellationToken)
    {
        //https://developers.cchaxcess.com/api-details#api=oip-tax-service&operation=post-batch-efile-extension
        try
        {
            OAuthTicket tokens = await EnsureValidAuthToken(cancellationToken);      
            if (!String.IsNullOrEmpty(tokens.access_token))
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                   
                    string url = _cchSettingsOptions.BatchEFileExtension;
                    var request = new ReturnExportBatchRequest(returnIds,
                        returnType.Equals(Constants.ReturnTypes.FedOnly) 
                                ? _cchSettingsOptions.ConfigurationXmlFederalOnlyTaxReturnBatchFileExtensionPdf
                                : _cchSettingsOptions.ConfigurationXmlFederalAndStateTaxReturnBatchFileExtensionPdf);

                    var json = System.Text.Json.JsonSerializer.Serialize(request);                  
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    ApiHelper.AddStandardHeaders(client, _cchSettingsOptions.IntegrationKey, tokens.access_token);
 
                    var result = await client.PostAsync(url, content, cancellationToken);
                    var response = await ApiHelper.HandleHttpResponseAsync<ReturnExportBatchResponse>(
                        result,
                        url,
                        cancellationToken);

                    if (response is not OkObjectResult && response is ObjectResult objectResult)
                    {                       
                            _logger.LogInformation("Issue in GetReturnsExportBatchPDF for StatusCode={StatusCode} Returns={ReturnIds} Value={Value}",
                                objectResult.StatusCode,
                                returnIds,
                                objectResult.Value);                        
                    }

                    return response;                  
                }
            }
            else
            {
                return Ok(Constants.ErrorMessages.AuthNotInitialized);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        //https://developers.cchaxcess.com/api-details#api=oip-tax-service&operation=post-returns-export-batch
    }

    [Authorize]
    [HttpGet(ApiEndPoints.TaxReturns.GetBatchStatus, Name = nameof(GetBatchStatus))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBatchStatus(string executionId, CancellationToken cancellationToken)
    {
        try
        {
            OAuthTicket tokens = await EnsureValidAuthToken(cancellationToken);
            if (!String.IsNullOrEmpty(tokens.access_token))
            {

                using (var client = _httpClientFactory.CreateClient())
                {
                    string url = string.Format(_cchSettingsOptions.BatchStatus + "?$filter=BatchGuid eq '{0}' and Expand eq 'Items'", executionId);                                                                                                    
                    StringContent content = new StringContent(url);                    
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    ApiHelper.AddStandardHeaders(client, _cchSettingsOptions.IntegrationKey, tokens.access_token); 
                    var result = await client.GetAsync(url, cancellationToken);                   
                    var response = await ApiHelper.HandleHttpResponseAsync<BatchStatusResponse>(
                        result,
                        url,                                       
                        cancellationToken);

                    if (response is not OkObjectResult && response is ObjectResult objectResult)
                    {
                        _logger.LogInformation("Issue in GetBatchStatus for ExecutionId={ExecutionId} StatusCode={StatusCode} StatusValue={StatusVode}",
                            executionId,
                            objectResult.StatusCode,
                            objectResult.Value);
                    }

                    return response;
                }
            }
            else {
                return Ok(Constants.ErrorMessages.AuthNotInitialized);
            }           
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        ///https://developers.cchaxcess.com/api-details#api=oip-tax-service&operation=get-batch-status
        ///
    }

    [Authorize]
    [HttpGet(ApiEndPoints.TaxReturns.GetBatchOutputFiles, Name = nameof(GetBatchOutputFiles))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]    
    public async Task<IActionResult> GetBatchOutputFiles(string executionId, CancellationToken cancellationToken)
    {
        try
        {

            OAuthTicket tokens = await EnsureValidAuthToken(cancellationToken);
            if (!String.IsNullOrEmpty(tokens.access_token))
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    string url = string.Format(_cchSettingsOptions.BatchOutputFiles + "?$filter=BatchGuid eq '{0}'", executionId);
                    StringContent content = new StringContent(url);                    
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    ApiHelper.AddStandardHeaders(client, _cchSettingsOptions.IntegrationKey, tokens.access_token);
                    var result = await client.GetAsync(url, cancellationToken);
                    var response = await ApiHelper.HandleHttpResponseAsync <List<BatchOutputFile>> (
                        result,
                        url,
                        cancellationToken);

                    if (response is not OkObjectResult && response is ObjectResult objectResult)
                    {
                        _logger.LogInformation("Issue in GetBatchOutputFiles for ExecutionId={ExecutionId} StatusCode={StatusCode} StatusValue={StatusVode}",
                            executionId,
                            objectResult.StatusCode,
                            objectResult.Value);
                    }

                    return response;
                }
            }
            else
            {
                return Ok(Constants.ErrorMessages.AuthNotInitialized);
            }

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        ///https://developers.cchaxcess.com/api-details#api=oip-tax-service&operation=get-batch-output-files
    }

    [Authorize]
    [HttpGet(ApiEndPoints.TaxReturns.DownloadBatchOutputFiles, Name = nameof(BatchOutputDownloadFile))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BatchOutputDownloadFile(string executionId, string batchItemGUID, string fileName, CancellationToken cancellationToken)
    {        
        try
        {
            //Retrieve token
            OAuthTicket tokens = await EnsureValidAuthToken(cancellationToken);
            if (!String.IsNullOrEmpty(tokens.access_token))
                using (var client = _httpClientFactory.CreateClient())
                {
                    string url = string.Format(_cchSettingsOptions.BatchOutputDownloadFile +
                        "?$filter=BatchGuid eq '{0}' and BatchItemGuid eq '{1}' and FileName eq '{2}'", executionId, batchItemGUID, fileName);

                    StringContent content = new StringContent(url);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    ApiHelper.AddStandardHeaders(client, _cchSettingsOptions.IntegrationKey, tokens.access_token);
                    var result = await client.GetAsync(url, cancellationToken);
                    var response = await ApiHelper.HandleHttpResponseAsync(
                        result,
                        url,
                        cancellationToken);

                    if (response is not OkObjectResult && response is ObjectResult objectResult)
                    {
                        _logger.LogInformation("Issue in BatchOutputDownloadFile for ExecutionId={ExecutionId} BatchItemGUIDS={BatchItemGUID} FileName={FileName} StatusCode={StatusCode} StatusValue={StatusVode}",
                            executionId,
                            batchItemGUID,
                            fileName,
                            objectResult.StatusCode,
                            objectResult.Value);

                        return BadRequest($"Could not downloadfile for ExecutionId={executionId} BatchItemGUIDS={batchItemGUID} FileName={fileName}");

                    }
                    
                    var baseDirectory = Path.Combine(AppContext.BaseDirectory, "Uploads");
                    Directory.CreateDirectory(baseDirectory);
                    var safeFileName = Path.GetFileName(fileName);
                    var fullPath = Path.Combine(baseDirectory, safeFileName);
                    var fullPathResolved = Path.GetFullPath(fullPath);
                    if (!fullPathResolved.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase)) throw new UnauthorizedAccessException("Invalid file path.");
                    using (var stream = await result.Content.ReadAsStreamAsync(cancellationToken))
                        using (var fileStream = new FileStream(fullPathResolved, FileMode.Create, FileAccess.Write, FileShare.None))
                            await stream.CopyToAsync(fileStream, cancellationToken);

                    var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPathResolved, cancellationToken);
                    return File(fileBytes, "application/pdf", fullPathResolved);                                      
                }
            else
            {
                return Ok(Constants.ErrorMessages.AuthNotInitialized);
            }

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        //https://developers.cchaxcess.com/api-details#api=oip-tax-service&operation=get-batch-output-download-files
    }
    
    private OAuthTicket GetAuthTokenFromFile()
    {
        OAuthTicket ticket = new OAuthTicket();

        if (!System.IO.File.Exists(TokenFileDirectory()))
            return ticket;
        
        var ticketString = System.IO.File.ReadAllText(TokenFileDirectory());

        if (string.IsNullOrWhiteSpace(ticketString))
            return ticket;
        else
             return System.Text.Json.JsonSerializer.Deserialize<OAuthTicket>(new MemoryStream(Encoding.UTF8.GetBytes(ticketString))) ?? ticket;
    
    }

    private async Task<OAuthTicket> EnsureValidAuthToken(CancellationToken cancellationToken = default)
    {
        OAuthTicket existingTicket = GetAuthTokenFromFile();
        bool needsRefreshTicket = false;
        bool needsNewTicket = false;

        needsRefreshTicket = existingTicket.RefreshedAt is null ||
                      Math.Abs((existingTicket.RefreshedAt.Value - DateTime.Now).TotalMinutes) >= _authorizationTokenOptions.TokenRefreshMinutes;

        needsNewTicket = existingTicket.IssuedAt is null ||
                     Math.Abs((existingTicket.IssuedAt.Value - DateTime.Now).TotalDays) >= _authorizationTokenOptions.TokenExpirationDays;

        if (needsNewTicket)
        {
            await CallToCreateNewTicket(cancellationToken);
        } else if (needsRefreshTicket)
        {
            await RequestRefreshAuthorizationTicket(cancellationToken);           
        }

        existingTicket = needsNewTicket || needsRefreshTicket ?
                GetAuthTokenFromFile() :
                existingTicket;

        return existingTicket;
    }

    private async Task<bool> CallToCreateNewTicket(CancellationToken cancellationToken)
    {              
        Process process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = await RequestNewAuthorizationTicket(cancellationToken),
            UseShellExecute = true // use the default browser
        };
        process.Start();
        System.Threading.Thread.Sleep(10000);
        try
        {
            if (!process.HasExited)
            {
                process.Kill();
            }
            return true;
        }
        catch (Exception ex)
        {          
            _logger.LogError(ex, "Error in CallToCreateNewTicket: {Message} ", ex.Message);
            return false;
        }           
    }

    private string TokenFileDirectory()
        => String.Format("{0}\\{1}", _authorizationTokenOptions.Directory, _authorizationTokenOptions.FileName);

    private void SaveAuthTokenToFile(OAuthTicket authenticationTicket)
    {
        string dir = string.Format("{0}", _authorizationTokenOptions.Directory);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        _logger.LogInformation("STEPS: Writing authenticationTicket to {Dir}", dir);
        System.IO.File.WriteAllText(TokenFileDirectory(), System.Text.Json.JsonSerializer.Serialize(authenticationTicket));
    }
        
}


//https://developers.cchaxcess.com/api-details#api=3fa06172a9344ca49c5e68b86a8322e4&operation=Gets-the-Elf-Return-History-Details
//https://api.cchaxcess.com/api/ElfStatusService/v1.0/ElfStatus/History?ReturnId={ReturnId}




