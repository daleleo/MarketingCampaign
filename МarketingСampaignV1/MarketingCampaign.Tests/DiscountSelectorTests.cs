using MarketingCampaign.Application;
using MarketingCampaign.Domain;
using Xunit;

namespace MarketingCampaign.Tests;

public class DiscountSelectorTests
{
    [Fact]
    public void Select_NoActiveCampaigns_ReturnsNoDiscount()
    {
        var policy = new MotorCascoPolicy { IssueDate = DateTime.Today };
        var campaigns = new List<Campaign>
        {
            new() { Status = CampaignStatus.Redacting, StartDate = DateTime.Today }
        };
        var result = new DiscountSelector().Select(policy, campaigns);
        Assert.True(result.NoDiscount);
    }

    [Fact]
    public void Select_DynamicVehicleYearConditionMatches_ReturnsDiscount()
    {
        var policy = new MotorCascoPolicy
        {
            IssueDate = DateTime.Today,
            VehicleYear = 2022,
            Premium = 50000,
            Product = "MotorCasco",
            SalesChannel = "Online"
        };

        var campaign = new Campaign
        {
            Status = CampaignStatus.Active,
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(1),
            Name = "Летняя",
            DynamicDiscountType = DynamicDiscountType.BasedOnVehicleYear,
            DynamicConditions = new List<DynamicCondition>
            {
                new()
                {
                    Type = DynamicDiscountType.BasedOnVehicleYear,
                    YearFrom = 2020, YearTo = 2024,
                    DiscountValue = 15, DiscountType = DiscountType.Percent
                }
            }
        };

        var result = new DiscountSelector().Select(policy, new[] { campaign });
        Assert.False(result.NoDiscount);
        Assert.Equal(15, result.Amount);
        Assert.Equal(campaign.Id, result.CampaignId);
    }

    [Fact]
    public void Select_GeneralConditionFails_ReturnsNoDiscount()
    {
        var policy = new MotorCascoPolicy
        {
            IssueDate = DateTime.Today,
            Premium = 10000,
            Product = "KASKO"
        };

        var campaign = new Campaign
        {
            Status = CampaignStatus.Active,
            StartDate = DateTime.Today.AddDays(-1),
            Name = "Дорогая",
            MinPremium = 50000,
            DiscountType = DiscountType.Percent,
            DiscountValue = 10
        };

        var result = new DiscountSelector().Select(policy, new[] { campaign });
        Assert.True(result.NoDiscount);
    }
}