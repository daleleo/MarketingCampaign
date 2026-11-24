namespace MarketingCampaign.Domain;

public static class EnumTranslations
{
    private static readonly Dictionary<CampaignType, (string Ru, string En)> CampaignTypes = new()
    {
        [CampaignType.Discount] = ("Скидочная кампания", "Discount campaign"),
        [CampaignType.Communication] = ("Коммуникационная кампания", "Communication campaign"),
        [CampaignType.Gift] = ("Подарочная кампания", "Gift campaign"),
    };

    private static readonly Dictionary<DiscountType, (string Ru, string En)> DiscountTypes = new()
    {
        [DiscountType.Percent] = ("Процент", "Percent"),
        [DiscountType.AbsoluteAmount] = ("Фиксированная сумма", "Absolute amount"),
    };

    private static readonly Dictionary<DynamicDiscountType, (string Ru, string En)> DynamicDiscountTypes = new()
    {
        [DynamicDiscountType.BasedOnVehicleYear] =
            ("В зависимости от года выпуска транспортного средства", "Based on the vehicle's year of manufacture"),
        [DynamicDiscountType.BasedOnVehicleMakeAndYear] =
            ("В зависимости от марки и года выпуска транспортного средства", "Based on the vehicle's make and year of manufacture"),
        [DynamicDiscountType.BasedOnBonusMalusRange] =
            ("В зависимости от диапазона обязательных бонусных коэффициентов", "Based on the range of compulsory bonus coefficients"),
        [DynamicDiscountType.BasedOnDriverExperience] =
            ("В зависимости от опыта водителя", "Based on the driver's experience"),
        [DynamicDiscountType.BasedOnDriverAge] =
            ("В зависимости от возраста водителя", "Based on the driver's age"),
    };

    private static readonly Dictionary<CustomerCategory, (string Ru, string En)> CustomerCategories = new()
    {
        [CustomerCategory.NewCustomers] = ("Новые клиенты", "New customers"),
        [CustomerCategory.RenewalCustomers] = ("Клиенты на продление", "Renewal customers"),
        [CustomerCategory.AllCustomers] = ("Все клиенты", "All customers"),
    };

    private static readonly Dictionary<ClaimsHistory, (string Ru, string En)> ClaimsHistories = new()
    {
        [ClaimsHistory.ClientsWithClaims] = ("Клиенты с убытками", "Clients with claims"),
        [ClaimsHistory.CustomersWithoutClaims] = ("Клиенты без убытков", "Customers without claims"),
        [ClaimsHistory.AllCustomers] = ("Все клиенты", "All customers"),
    };

    private static readonly Dictionary<PartyType, (string Ru, string En)> PartyTypes = new()
    {
        [PartyType.NaturalPerson] = ("Физическое лицо", "Natural Person"),
        [PartyType.LegalEntity] = ("Юридическое лицо", "Legal Entity"),
    };

    private static readonly Dictionary<ProductType, (string Ru, string En)> ProductTypes = new()
    {
        [ProductType.MotorCasco] = ("КАСКО", "MotorCasco"),
    };

    private static readonly Dictionary<CampaignStatus, (string Ru, string En)> CampaignStatuses = new()
    {
        [CampaignStatus.Redacting] = ("Редактирование", "Redacting"),
        [CampaignStatus.WaitingForApproval] = ("Ожидает подтверждения", "Waiting for Approval"),
        [CampaignStatus.PendingActivation] = ("Ожидает активации", "Pending activation"),
        [CampaignStatus.Active] = ("Активен", "Active"),
        [CampaignStatus.Rejected] = ("Отклонен", "Rejected"),
        [CampaignStatus.Completed] = ("Завершен", "Completed"),
    };

    public static string Translate(CampaignType value, string culture) => Pick(CampaignTypes[value], culture);
    public static string Translate(DiscountType value, string culture) => Pick(DiscountTypes[value], culture);
    public static string Translate(DynamicDiscountType value, string culture) => Pick(DynamicDiscountTypes[value], culture);
    public static string Translate(CustomerCategory value, string culture) => Pick(CustomerCategories[value], culture);
    public static string Translate(ClaimsHistory value, string culture) => Pick(ClaimsHistories[value], culture);
    public static string Translate(PartyType value, string culture) => Pick(PartyTypes[value], culture);
    public static string Translate(ProductType value, string culture) => Pick(ProductTypes[value], culture);
    public static string Translate(CampaignStatus value, string culture) => Pick(CampaignStatuses[value], culture);

    private static string Pick((string Ru, string En) pair, string culture) =>
        culture.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? pair.En : pair.Ru;
}