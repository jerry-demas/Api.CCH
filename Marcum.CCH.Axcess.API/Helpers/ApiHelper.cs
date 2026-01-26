using System.Net;
using Marcum.CCH.Axcess.API.Statics;
using Marcum.CCH.Axcess.Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Marcum.CCH.Axcess.API.Helpers;

public static class ApiHelper
{

    public static async Task<IActionResult> HandleHttpResponseAsync<T>(
        HttpResponseMessage result,
        string url,
        CancellationToken cancellationToken)
    {
        switch (result.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                return new UnauthorizedObjectResult(
                    string.Format(Constants.Formats.HttpResponse, url, Constants.ErrorMessages.UnauthorizedRefreshToken));

            case HttpStatusCode.BadRequest:
                var badRequestContent = await result.Content.ReadAsStringAsync(cancellationToken);
                return new BadRequestObjectResult(
                    string.Format(Constants.Formats.HttpResponse, url, badRequestContent));

            case HttpStatusCode.NotFound:
                var notFoundContent = await result.Content.ReadAsStringAsync(cancellationToken);
                return new NotFoundObjectResult(
                    string.Format(Constants.Formats.HttpResponse, url, notFoundContent));

            case HttpStatusCode.OK:
                var mediaType = result.Content.Headers.ContentType?.MediaType;
                var stream = await result.Content.ReadAsStreamAsync(cancellationToken);
                if (mediaType != null && mediaType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
                {
                    return new FileStreamResult(stream, mediaType)
                    {
                        FileDownloadName = "downloaded_file.pdf"
                    };
                }
                var deserialized = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
                return new OkObjectResult(deserialized);

            default:
                var resultContent = await result.Content.ReadAsStringAsync(cancellationToken);
                JObject resultObject = JObject.Parse(resultContent);
                return new OkObjectResult(
                    string.Format(Constants.Formats.HttpResponse, url, resultObject));
        }
    }



    public static async Task<IActionResult> HandleHttpResponseAsync(
        HttpResponseMessage result,
        string url,
        CancellationToken cancellationToken)
        => await HandleHttpResponseAsync<object>(result, url, cancellationToken);

    
    public static void AddStandardHeaders(
        HttpClient client,
        string integrationKey,
        string accessToken)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(integrationKey);
        ArgumentNullException.ThrowIfNull(accessToken);

        if (!client.DefaultRequestHeaders.Contains(Constants.HttpRequestHeader.IntegratorKeyHeaderName))
            client.DefaultRequestHeaders.Add(Constants.HttpRequestHeader.IntegratorKeyHeaderName, integrationKey);

        if (!client.DefaultRequestHeaders.Contains(Constants.HttpRequestHeader.AuthorizationHeaderName))
        {
            string authorizationValue = Constants.HttpRequestHeader.AuthorizationHeaderValue
                .Replace(Constants.HttpRequestHeader.TokenPlaceholder, accessToken);

            client.DefaultRequestHeaders.Add(Constants.HttpRequestHeader.AuthorizationHeaderName, authorizationValue);
        }

    }
}
