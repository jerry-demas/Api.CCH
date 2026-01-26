
namespace Marcum.CCH.Axcess.Domain.Models;

public record BatchStatusResponse(
    string BatchStatus, 
    string BatchStatusDescription, 
    List<BatchStatusItem> Items);


