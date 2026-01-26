
namespace Marcum.CCH.Axcess.Domain.Models;

public record BatchStatusItem(
    Guid ItemGuid,
    string ItemStatusCode,
    string ItemStatusDescription,
    string ResponseCode,
    string ResponseDescription,
    BatchStatusReturnInfo ReturnInfo,
    List<BatchStatusAdditionalInfo> AdditionalInfo
);
