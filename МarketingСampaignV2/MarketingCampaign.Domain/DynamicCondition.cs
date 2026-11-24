namespace MarketingCampaign.Domain;

public class DynamicCondition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DynamicDiscountType Type { get; set; }

    // Для VehicleYear / VehicleMakeAndYear
    public List<string>? VehicleMakes { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }

    // Для BonusMalusRange
    public decimal? BonusMalusFrom { get; set; }
    public decimal? BonusMalusTo { get; set; }

    // Для DriverExperience / DriverAge
    public int? ValueFrom { get; set; }
    public int? ValueTo { get; set; }

    public decimal DiscountValue { get; set; }
    public DiscountType DiscountType { get; set; } = DiscountType.Percent;
}