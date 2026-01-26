namespace Marcum.CCH.Axcess.Domain.Models;

public record ExportBatchFileResult(bool IsError, string[] Messages, string[] SubItemExecutionIDs, int FIleGroupID);
