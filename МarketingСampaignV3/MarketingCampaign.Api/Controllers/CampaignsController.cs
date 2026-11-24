using MarketingCampaign.Application;
using MarketingCampaign.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketingCampaign.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    //private readonly CampaignValidator _validator = new();
    //private readonly DiscountSelector _selector = new();

    private readonly CampaignValidator _validator;
    private readonly DiscountSelector _selector;

    // улучшил вариант за счёт DI (внедрения зависимостей) через конструктор
    public CampaignsController(CampaignValidator validator, DiscountSelector selector)
    {
        _validator = validator;
        _selector = selector;
    }


    // Валидация: доступна администратору и утверждающему
    [HttpPost("validate")]
    [Authorize(Roles = "AdministratorMarketingCampaign,ApproverMarketingCampaign")]
    public IActionResult Validate([FromBody] Campaign campaign)
    {
        var result = _validator.Validate(campaign);
        return result.IsValid ? Ok(new { valid = true }) : BadRequest(result.Errors);
    }

    // Подбор скидки: доступен всем авторизованным ролям
    [HttpPost("select-discount")]
    [Authorize(Roles = "ViewerMarketingCampaign,AdministratorMarketingCampaign,ApproverMarketingCampaign")]
    public IActionResult SelectDiscount([FromBody] SelectDiscountRequest request)
    {
        var result = _selector.Select(request.Policy, request.Campaigns);
        return Ok(result);
    }
}

public class SelectDiscountRequest
{
    public MotorCascoPolicy Policy { get; set; } = new();
    public List<Campaign> Campaigns { get; set; } = new();
}