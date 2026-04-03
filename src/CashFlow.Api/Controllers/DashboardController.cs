using CashFlow.Application.UseCases.Dashboard.GetMonthlySummary;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        [HttpGet]
        [Route("monthly-summary")]
        [ProducesResponseType(typeof(ResponseMonthlySummary), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMonthlySummary(
            [FromQuery] DateTime month,
            [FromServices] IGetMonthlySummaryUseCase useCase)
        {
            var response = await useCase.Execute(month);

            return Ok(response);
        }
    }
}
