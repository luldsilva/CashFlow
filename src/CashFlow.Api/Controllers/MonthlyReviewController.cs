using CashFlow.Application.UseCases.MonthlyReview.Close;
using CashFlow.Application.UseCases.MonthlyReview.Get;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers
{
    [Route("api/monthly-review")]
    [ApiController]
    [Authorize]
    public class MonthlyReviewController : ControllerBase
    {
        [HttpPost("close")]
        [ProducesResponseType(typeof(ResponseMonthlyClosure), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Close(
            [FromBody] RequestCloseMonth request,
            [FromServices] ICloseMonthUseCase useCase)
        {
            var response = await useCase.Execute(request);
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseMonthlyReview), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(
            [FromQuery] DateTime month,
            [FromServices] IGetMonthlyReviewUseCase useCase)
        {
            var response = await useCase.Execute(month);
            return Ok(response);
        }
    }
}
