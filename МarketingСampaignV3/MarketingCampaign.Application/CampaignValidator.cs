using MarketingCampaign.Domain;

namespace MarketingCampaign.Application;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = new();
}

public class CampaignValidator
{
    public ValidationResult Validate(Campaign campaign)
    {
        var result = new ValidationResult();

        // 1. Общие проверки
        if (string.IsNullOrWhiteSpace(campaign.Name))
            result.Errors.Add("Название кампании обязательно.");

        if (!campaign.StartDate.HasValue)
            result.Errors.Add("Дата начала обязательна.");

        if (campaign.EndDate.HasValue && campaign.EndDate < campaign.StartDate)
            result.Errors.Add("Дата окончания не может быть раньше даты начала.");

        if (campaign.Type == CampaignType.Discount && campaign.ProductTypes.Count == 0)
            result.Errors.Add("Для скидочной компании тип продукта обязателен");

        // 2. Валидация динамических условий
        if (campaign.DynamicDiscountType.HasValue)
        {
            ValidateDynamicConditions(campaign, result);
        }

        // 3. Согласованность: если есть динамическая скидка, статические поля скидки не обязательны
        // Но если динамической нет — проверяем статические
        if (!campaign.DynamicDiscountType.HasValue)
        {
            if (campaign.DiscountType == null)
                result.Errors.Add("Тип скидки обязателен, если не выбрана динамическая скидка.");

            if (campaign.DiscountValue == null || campaign.DiscountValue <= 0)
                result.Errors.Add("Размер скидки должен быть больше нуля.");

            if (campaign.DiscountType == DiscountType.Percent && campaign.DiscountValue > 100)
                result.Errors.Add("Процентная скидка не может превышать 100%.");
        }

        return result;
    }

    private void ValidateDynamicConditions(Campaign campaign, ValidationResult result)
    {
        if (campaign.DynamicConditions.Count == 0)
        {
            result.Errors.Add("Для динамической скидки необходимо заполнить хотя бы одно условие.");
            return;
        }

        foreach (var condition in campaign.DynamicConditions)
        {
            if (condition.DiscountValue <= 0)
                result.Errors.Add("Размер скидки в динамическом условии должен быть больше нуля.");

            if (condition.DiscountType == DiscountType.Percent && condition.DiscountValue > 100)
                result.Errors.Add("Процентная динамическая скидка не может превышать 100%.");

            // Проверки диапазонов в зависимости от типа
            if (condition.Type is DynamicDiscountType.BasedOnVehicleYear
                or DynamicDiscountType.BasedOnVehicleMakeAndYear)
            {
                if (condition.YearFrom == null || condition.YearTo == null)
                    result.Errors.Add($"Для типа {condition.Type} обязательны годы 'от' и 'до'.");

                if (condition.YearFrom > condition.YearTo)
                    result.Errors.Add("Год 'от' не может быть больше года 'до'.");

                if (condition.Type == DynamicDiscountType.BasedOnVehicleMakeAndYear
                    && (condition.VehicleMakes == null || condition.VehicleMakes.Count == 0))
                    result.Errors.Add("Для скидки по марке и году необходимо указать хотя бы одну марку.");
            }

            if (condition.Type == DynamicDiscountType.BasedOnBonusMalusRange)
            {
                if (condition.BonusMalusFrom == null || condition.BonusMalusTo == null)
                    result.Errors.Add("Для скидки по бонус-малус обязательны значения 'от' и 'до'.");

                if (condition.BonusMalusFrom > condition.BonusMalusTo)
                    result.Errors.Add("Коэффициент бонус-малус 'от' не может быть больше 'до'.");
            }

            if (condition.Type is DynamicDiscountType.BasedOnDriverExperience
                or DynamicDiscountType.BasedOnDriverAge)
            {
                if (condition.ValueFrom == null || condition.ValueTo == null)
                    result.Errors.Add($"Для типа {condition.Type} обязательны значения 'от' и 'до'.");

                if (condition.ValueFrom > condition.ValueTo)
                    result.Errors.Add("Значение 'от' не может быть больше значения 'до'.");
            }
        }

        // Проверка уникальности: нельзя иметь два условия одного типа с одинаковыми диапазонами
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
            result.Errors.Add("Обнаружены дублирующиеся динамические условия с одинаковыми диапазонами.");
    }
}