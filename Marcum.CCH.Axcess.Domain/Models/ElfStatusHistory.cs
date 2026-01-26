namespace Marcum.CCH.Axcess.Domain.Models;

public class ElfStatusHistory
{
    public required string AckDate { get; set; }
    public required string CategoryOfReturn { get; set; }
    public int DisplayCode { get; set; }
    public required string Form8879DateReceived { get; set; }
    public required string Form8879DateReceived_UpldLevel { get; set; }
    public int intCOR { get; set; }
    public int IsActive { get; set; }
    public bool IsFBAR { get; set; }
    public bool IsParticipatingInEsign { get; set; }
    public required string MISBTID { get; set; }
    public required string ModifiedByName { get; set; }
    public bool ParentIsPassowordProtected { get; set; }
    public bool ParentWasPasswordProtected { get; set; }
    public required string StateCode { get; set; }
    public double StateDue { get; set; }
    public required string StatusDate { get; set; }
    public int StatusID { get; set; }
    public required string StatusText { get; set; }
    public required string SubmissionId { get; set; }
    public required string UnitName { get; set; }
    public required string UnmaskedMISBTID { get; set; }
    public required string UnmaskedStateCode { get; set; }

}

