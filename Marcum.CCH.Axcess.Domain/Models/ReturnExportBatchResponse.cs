namespace Marcum.CCH.Axcess.Domain.Models;

public record ReturnExportBatchResponse(Guid ExecutionID, List<ExportBatchFileResult> FileResults);




