using MarketingCampaign.Application;
using MarketingCampaign.Domain;
using Xunit;

namespace MarketingCampaign.Tests;

public class CampaignValidatorTests
{
    [Fact]
    public void Validate_EmptyName_ReturnsError()
    {
        var campaign = new Campaign { Name = "", StartDate = DateTime.Today };
        var result = new CampaignValidator().Validate(campaign);
        Assert.Contains(result.Errors, e => e.Contains("Название"));
    }

    [Fact]
    public void Validate_DynamicConditionWithoutYearRange_ReturnsError()
    {
        var campaign = new Campaign
        {
            Name = "Test",
            StartDate = DateTime.Today,
            DynamicDiscountType = DynamicDiscountType.BasedOnVehicleYear,
            DynamicConditions = new List<DynamicCondition>
            {
                new() { Type = DynamicDiscountType.BasedOnVehicleYear, DiscountValue = 10, DiscountType = DiscountType.Percent }
            }
        };
        var result = new CampaignValidator().Validate(campaign);
        Assert.Contains(result.Errors, e => e.Contains("годы 'от' и 'до'"));
    }

    [Fact]
    public void Validate_DuplicateConditions_ReturnsError()
    {
        var condition = new DynamicCondition
        {
            Type = DynamicDiscountType.BasedOnDriverAge,
            ValueFrom = 25,
            ValueTo = 60,
            DiscountValue = 5,
            DiscountType = DiscountType.Percent
        };
        var campaign = new Campaign
        {
            Name = "Test",
            StartDate = DateTime.Today,
            DynamicDiscountType = DynamicDiscountType.BasedOnDriverAge,
            DynamicConditions = new List<DynamicCondition> { condition, condition }
        };
        var result = new CampaignValidator().Validate(campaign);
        Assert.Contains(result.Errors, e => e.Contains("дублирующиеся"));
    }
}