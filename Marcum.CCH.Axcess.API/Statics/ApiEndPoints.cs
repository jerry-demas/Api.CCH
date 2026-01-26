
namespace Marcum.CCH.Axcess.API.Statics;

public static class ApiEndPoints
{

    private const string ApiBase = "api";
    
    public static class TaxReturns
    {        
        private const string Base = $"{ApiBase}/{nameof(TaxReturns)}";
        
        public const string GetAuthorizationToken = $"{Base}/GetAuthorizationToken";       
        public const string RequestNewAuthorizationTicket = $"{Base}/RequestNewAuthorizationTicket";
        public const string RequestRefreshAuthorizationTicket = $"{Base}/RequestRefreshAuthorizationTicket";
        public const string AuthCallback = $"{Base}/AuthCallback";
        public const string AuthCallbackRefresh = $"{Base}/AuthCallbackRefresh";
        public const string GetElfStatusHistoryByReturnId = $"{Base}/GetElfStatusHistoryByReturnId/{{returnId}}";
        public const string GetReturnsExportBatchPDF = $"{Base}/GetReturnsExportBatchPDF/{{returnType}}";
        public const string GetBatchStatus = $"{Base}/GetBatchStatus/{{executionId}}";
        public const string GetBatchOutputFiles = $"{Base}/GetBatchOutputFiles/{{executionId}}";
        public const string DownloadBatchOutputFiles = $"{Base}/BatchOutputDownloadFile/{{executionId}}/{{batchItemGUID}}/{{fileName}}";
        public const string GetCCHAuthorizationToken = $"{Base}/GetCCHAuthorizationToken";

    }
    
}


