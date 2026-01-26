namespace Marcum.CCH.Axcess.Domain.Models;

public record EFileStatusClientsRequest(
    List<string> ClientIDs,
    List<string> ClientGUIDs,
    string LastDateTimeCalled,
    int SummaryStatus
    );

