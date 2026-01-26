namespace Marcum.CCH.Axcess.Infrastructure.Options;


public record CchSettingsOptions
{
    public required string LoginUrl { get; set; }
     public required string RefreshUrl { get; set; }
     public required string TokenUrl { get; set; }
     public required string AxcessLoginAccountNumber { get; set; }
     public required string RedirectUrl { get; set; }
     public required string RedirectUrlRefresh { get; set; }
     public required string ElfHistoryByReturnIdUrl { get; set; }
     public required string ClientYearTaxReturns { get; set; }
     public required string BatchEFileExtension { get; set; }
     public required string BatchStatus { get; set; }
     public required string BatchOutputFiles { get; set; }
     public required string BatchOutputDownloadFile { get; set; }
     public required string ClientId { get; set; }
     public required string ClientSecret { get; set; }
     public required string IntegrationKey { get; set; }
     public required string Scopes { get; set; }
     public required string ConfigurationXmlTaxReturnXML { get; set; }
     public required string ConfigurationXmlFederalOnlyTaxReturnBatchFileExtensionPdf { get; set; }
     public required string ConfigurationXmlFederalAndStateTaxReturnBatchFileExtensionPdf { get; set; }
     public required string ConfigurationXmlTaxReturnOrganizer { get; set; }
}