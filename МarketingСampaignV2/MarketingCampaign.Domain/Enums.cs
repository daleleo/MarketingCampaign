namespace MarketingCampaign.Domain;

public enum CampaignStatus 
{ 
    Redacting,
    WaitingForApproval,
    PendingActivation,
    Active,
    Rejected,
    Completed
}

public enum CampaignType { Discount, Communication, Gift }

public enum DiscountType { Percent, AbsoluteAmount }

public enum ProductType 
{ 
    MotorCasco,
    MotorThirdPartyLiability,
    TradeCreditInsurance,
    GeneralLiabilityInsurance,
    PropertyLegalEntities 
}

public enum DynamicDiscountType
{
    BasedOnVehicleYear,
    BasedOnVehicleMakeAndYear,
    BasedOnBonusMalusRange,
    BasedOnDriverExperience,
    BasedOnDriverAge
}

public enum CustomerCategory { NewCustomers, RenewalCustomers, AllCustomers }
public enum ClaimsHistory { ClientsWithClaims, CustomersWithoutClaims, AllCustomers }
public enum PartyType { NaturalPerson, LegalEntity }

public enum SalesChannel
{
    MandatoryRenewal,
    VoluntaryRenewal,
    RenewalAgent,
    ExternalMandatory,
    ExternalVoluntary,
    Intranet,
    HesabAz,
    InsureAz,
    phone4717,
    Sms,
    OnlineStore,
    Website,
    SocialMedia,
    ManualEntry,
    OnlineMandatory
}
