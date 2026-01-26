namespace Marcum.CCH.Axcess.Domain.Models;

public class SubUnit
{
    public required string AckBSAID { get; set; }
    public required string AckDate { get; set; }
    public required string CategoryOfReturn { get; set; }
    public required string CorText { get; set; }
    public bool IsFBAR { get; set; }
    public bool ParentIsPassowordProtected { get; set; }
    public bool ParentWasPasswordProtected { get; set; }
    public required string StateCode { get; set; }
    public required string StatusDate { get; set; }
    public int StatusID { get; set; }
    public required string StatusText { get; set; }
    public required string SubUnitID { get; set; }
    public required string UnmaskedSubUnitID { get; set; }
}