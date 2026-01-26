
namespace Marcum.CCH.Axcess.Domain.Models;

public record ReturnBatchOutputFileResponse(BatchOutputFile[] BatchOutputFiles);

public record BatchOutputFile(
    Guid BatchItemGuid,
    string FileName,
    int Length);

