using MarketingCampaign.Domain;

namespace MarketingCampaign.Application;

public class DiscountResult
{
    public decimal Amount { get; set; }
    public DiscountType Type { get; set; }
    public Guid? CampaignId { get; set; }
    public string? AppliedCondition { get; set; }
    public bool NoDiscount { get; set; }
    public string? Reason { get; set; }
}

public class DiscountSelector
{
    public DiscountResult Select(MotorCascoPolicy policy, IEnumerable<Campaign> campaigns)
    {
        var applicableCampaigns = campaigns
            .Where(c => c.Status == CampaignStatus.Active)
            .Where(c => c.StartDate <= policy.IssueDate)
            .Where(c => !c.EndDate.HasValue || c.EndDate >= policy.IssueDate)
            .Where(c => MatchesGeneralConditions(policy, c))
            .ToList();

        if (!applicableCampaigns.Any())
        {
            return new DiscountResult
            {
                NoDiscount = true,
                Reason = "Нет активных кампаний, подходящих под общие условия договора."
            };
        }

        // Приоритет: сначала ищем динамические скидки (они специфичнее), потом статические
        foreach (var campaign in applicableCampaigns)
        {
            if (campaign.DynamicDiscountType.HasValue)
            {
                var dynamicResult = TryApplyDynamicDiscount(policy, campaign);
                if (dynamicResult != null)
                    return dynamicResult;
            }
            else if (campaign.DiscountType.HasValue && campaign.DiscountValue.HasValue)
            {
                return new DiscountResult
                {
                    Amount = campaign.DiscountValue.Value,
                    Type = campaign.DiscountType.Value,
                    CampaignId = campaign.Id,
                    AppliedCondition = $"Статическая скидка кампании «{campaign.Name}»"
                };
            }
        }

        return new DiscountResult
        {
            NoDiscount = true,
            Reason = "Кампании найдены, но ни одно динамическое условие не подошло."
        };
    }

    private bool MatchesGeneralConditions(MotorCascoPolicy policy, Campaign campaign)
    {
        if (campaign.Products.Any() && !campaign.Products.Contains(policy.Product))
            return false;

        if (campaign.SalesChannels.Any() && !campaign.SalesChannels.Contains(policy.SalesChannel))
            return false;

        if (campaign.PartyTypes.Any() && !campaign.PartyTypes.Contains(policy.PartyType))
            return false;

        if (campaign.CustomerCategories.Any() && !campaign.CustomerCategories.Contains(policy.CustomerCategory))
            return false;

        if (campaign.ClaimsHistories.Any() && !campaign.ClaimsHistories.Contains(policy.ClaimsHistory))
            return false;

        if (campaign.MinPremium.HasValue && policy.Premium < campaign.MinPremium)
            return false;

        if (campaign.MaxPremium.HasValue && policy.Premium > campaign.MaxPremium)
            return false;

        return true;
    }

    private DiscountResult? TryApplyDynamicDiscount(MotorCascoPolicy policy, Campaign campaign)
    {
        foreach (var condition in campaign.DynamicConditions)
        {
            if (MatchesDynamicCondition(policy, condition))
            {
                return new DiscountResult
                {
                    Amount = condition.DiscountValue,
                    Type = condition.DiscountType,
                    CampaignId = campaign.Id,
                    AppliedCondition = DescribeCondition(condition)
                };
            }
        }
        return null;
    }

    private bool MatchesDynamicCondition(MotorCascoPolicy policy, DynamicCondition condition)
    {
        return condition.Type switch
        {
            DynamicDiscountType.BasedOnVehicleYear =>
                policy.VehicleYear >= condition.YearFrom && policy.VehicleYear <= condition.YearTo,

            DynamicDiscountType.BasedOnVehicleMakeAndYear =>
                condition.VehicleMakes != null
                && condition.VehicleMakes.Contains(policy.VehicleMake, StringComparer.OrdinalIgnoreCase)
                && policy.VehicleYear >= condition.YearFrom && policy.VehicleYear <= condition.YearTo,

            DynamicDiscountType.BasedOnBonusMalusRange =>
                policy.BonusMalusCoefficient >= condition.BonusMalusFrom
                && policy.BonusMalusCoefficient <= condition.BonusMalusTo,

            DynamicDiscountType.BasedOnDriverExperience =>
                policy.DriverExperience >= condition.ValueFrom && policy.DriverExperience <= condition.ValueTo,

            DynamicDiscountType.BasedOnDriverAge =>
                policy.DriverAge >= condition.ValueFrom && policy.DriverAge <= condition.ValueTo,

            _ => false
        };
    }

    private string DescribeCondition(DynamicCondition condition) => condition.Type switch
    {
        DynamicDiscountType.BasedOnVehicleYear =>
            $"Год выпуска ТС от {condition.YearFrom} до {condition.YearTo}",
        DynamicDiscountType.BasedOnVehicleMakeAndYear =>
            $"Марка {string.Join(", ", condition.VehicleMakes ?? new())}, год от {condition.YearFrom} до {condition.YearTo}",
        DynamicDiscountType.BasedOnBonusMalusRange =>
            $"Бонус-малус от {condition.BonusMalusFrom} до {condition.BonusMalusTo}",
        DynamicDiscountType.BasedOnDriverExperience =>
            $"Стаж водителя от {condition.ValueFrom} до {condition.ValueTo}",
        DynamicDiscountType.BasedOnDriverAge =>
            $"Возраст водителя от {condition.ValueFrom} до {condition.ValueTo}",
        _ => "Неизвестное условие"
    };
}