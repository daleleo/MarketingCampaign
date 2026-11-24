namespace MarketingCampaign.Domain;

public class MotorCascoPolicy
{
    public DateTime IssueDate { get; set; }
    public string Product { get; set; } = string.Empty;
    public ProductType InsuranceProductType { get; set; }
    public string SalesChannel { get; set; } = string.Empty;
    public PartyType PartyType { get; set; }
    public CustomerCategory CustomerCategory { get; set; }
    public ClaimsHistory ClaimsHistory { get; set; }
    public decimal Premium { get; set; }
    public string VehicleMake { get; set; } = string.Empty;
    public int VehicleYear { get; set; }
    public decimal BonusMalusCoefficient { get; set; }
    public int DriverAge { get; set; }
    public int DriverExperience { get; set; }
}