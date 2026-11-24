using MarketingCampaign.Domain;
using Microsoft.Extensions.Localization;

namespace MarketingCampaign.Application;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = new();
}

public class CampaignValidator
{
    private readonly IStringLocalizer<CampaignValidator> _localizer;

    public CampaignValidator(IStringLocalizer<CampaignValidator> localizer)
    {
        _localizer = localizer;
    }

    public ValidationResult Validate(Campaign campaign)
    {
        var result = new ValidationResult();

        // 1. Общие проверки
        if (string.IsNullOrWhiteSpace(campaign.Name))
            result.Errors.Add(_localizer["CampaignNameRequired"]);

        if (campaign.StartDate == default)
            result.Errors.Add(_localizer["StartDateRequired"]);

        if (campaign.EndDate.HasValue && campaign.EndDate < campaign.StartDate)
            result.Errors.Add(_localizer["EndDateBeforeStart"]);

        // 2. Тип продукта обязателен для скидочной кампании
        if (campaign.Type == CampaignType.Discount && campaign.ProductTypes.Count == 0)
            result.Errors.Add(_localizer["ProductTypeRequired"]);

        // 3. Валидация динамических условий
        if (campaign.DynamicDiscountType.HasValue)
        {
            ValidateDynamicConditions(campaign, result);
        }
        else
        {
            // 4. Статическая скидка
            if (campaign.DiscountType == null)
                result.Errors.Add(_localizer["DiscountTypeRequired"]);

            if (campaign.DiscountValue == null || campaign.DiscountValue <= 0)
                result.Errors.Add(_localizer["DiscountValuePositive"]);

            if (campaign.DiscountType == DiscountType.Percent && campaign.DiscountValue > 100)
                result.Errors.Add(_localizer["PercentDiscountMax"]);
        }

        return result;
    }

    private void ValidateDynamicConditions(Campaign campaign, ValidationResult result)
    {
        if (campaign.DynamicConditions.Count == 0)
        {
            result.Errors.Add(_localizer["DynamicConditionRequired"]);
            return;
        }

        foreach (var condition in campaign.DynamicConditions)
        {
            if (condition.DiscountValue <= 0)
                result.Errors.Add(_localizer["DynamicDiscountPositive"]);

            if (condition.DiscountType == DiscountType.Percent && condition.DiscountValue > 100)
                result.Errors.Add(_localizer["DynamicPercentMax"]);

            if (condition.Type is DynamicDiscountType.BasedOnVehicleYear
                or DynamicDiscountType.BasedOnVehicleMakeAndYear)
            {
                if (condition.YearFrom == null || condition.YearTo == null)
                    result.Errors.Add(_localizer["YearRangeRequired", condition.Type]);

                if (condition.YearFrom > condition.YearTo)
                    result.Errors.Add(_localizer["YearFromGreaterThanTo"]);

                if (condition.Type == DynamicDiscountType.BasedOnVehicleMakeAndYear
                    && (condition.VehicleMakes == null || condition.VehicleMakes.Count == 0))
                    result.Errors.Add(_localizer["VehicleMakesRequired"]);
            }

            if (condition.Type == DynamicDiscountType.BasedOnBonusMalusRange)
            {
                if (condition.BonusMalusFrom == null || condition.BonusMalusTo == null)
                    result.Errors.Add(_localizer["BonusMalusRangeRequired"]);

                if (condition.BonusMalusFrom > condition.BonusMalusTo)
                    result.Errors.Add(_localizer["BonusMalusFromGreaterThanTo"]);
            }

            if (condition.Type is DynamicDiscountType.BasedOnDriverExperience
                or DynamicDiscountType.BasedOnDriverAge)
            {
                if (condition.ValueFrom == null || condition.ValueTo == null)
                    result.Errors.Add(_localizer["ValueRangeRequired", condition.Type]);

                if (condition.ValueFrom > condition.ValueTo)
                    result.Errors.Add(_localizer["ValueFromGreaterThanTo"]);
            }
        }

        var duplicates = campaign.DynamicConditions
            .GroupBy(c => new
            {
                c.Type,
                c.YearFrom,
                c.YearTo,
                c.BonusMalusFrom,
                c.BonusMalusTo,
                c.ValueFrom,
                c.ValueTo
            })
            .Where(g => g.Count() > 1);

        if (duplicates.Any())
            result.Errors.Add(_localizer["DuplicateConditions"]);
    }
}