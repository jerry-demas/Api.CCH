
namespace Marcum.CCH.Axcess.Domain.Models;

public class ElfStatusHistoryResponse
{

    public required string BankInfo { get; set; }
    public required string CategoryOfReturn { get; set; } 
    public required string EPostMark { get; set; }  
    public required string FiscalYearBegin { get; set; }  
    public required string FiscalYearEnd { get; set; }  
    public required string Form8879DateReceived { get; set; }  
    public required string IRSCenter { get; set; }  
    public required string IRSMessage { get; set; }  
    public bool IsPasswordProtected { get; set; }
    public bool IsRefund { get; set; }
    public bool IsSSN { get; set; }
    public required string Name { get; set; }  
    public required string Notification { get; set; }  
    public required string PlanNumber { get; set; }  
    public required string PreparerName { get; set; }  
    public required string Product { get; set; }  
    public double RefundAmount { get; set; }
    public required string ReturnID { get; set; }  
    public required string SsnEin { get; set; }  
    public List<ElfStatusHistory> StatusHistoryList { get; set; } = new List<ElfStatusHistory>();
    public List<SubUnit> SubUnitList { get; set; } = new List<SubUnit>();
    public int TaxYear { get; set; }
    public required string TypeOfReturn { get; set; }  

}

