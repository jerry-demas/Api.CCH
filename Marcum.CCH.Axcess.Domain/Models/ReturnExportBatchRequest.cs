namespace Marcum.CCH.Axcess.Domain.Models;

public record ReturnExportBatchRequest(List<string> ReturnId, string ConfigurationXml);
