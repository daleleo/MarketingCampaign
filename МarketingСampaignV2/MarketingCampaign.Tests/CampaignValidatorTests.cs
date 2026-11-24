using Microsoft.Extensions.Localization;
using MarketingCampaign.Application;
using MarketingCampaign.Domain;
using Xunit;

namespace MarketingCampaign.Tests;

public class CampaignValidatorTests
{
    private static CampaignValidator CreateValidator()
    {
        var localizer = new NullStringLocalizer<CampaignValidator>();
        return new CampaignValidator(localizer);
    }


    [Fact]
    public void Validate_EmptyName_ReturnsError()
    {
        var campaign = new Campaign { Name = "", StartDate = DateTime.Today };
        var result = CreateValidator().Validate(campaign);
        Assert.Contains(result.Errors, e => e.Contains("CampaignNameRequired"));
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
        var result = CreateValidator().Validate(campaign);
        Assert.Contains(result.Errors, e => e.Contains("YearRangeRequired"));
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
        var result = CreateValidator().Validate(campaign);
        Assert.Contains(result.Errors, e => e.Contains("DuplicateConditions"));
    }

    internal class NullStringLocalizer<T> : IStringLocalizer<T>
    {
        public LocalizedString this[string name] => new(name, name);

        public LocalizedString this[string name, params object[] arguments] =>
            new(name, string.Format(name, arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            Enumerable.Empty<LocalizedString>();
    }

}