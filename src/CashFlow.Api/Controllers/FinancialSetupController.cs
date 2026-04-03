using CashFlow.Application.UseCases.FinancialSetup.Get;
using CashFlow.Application.UseCases.FinancialSetup.Upsert;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers
{
    [Route("api/financial-setup")]
    [ApiController]
    [Authorize]
    public class FinancialSetupController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ResponseFinancialSetup), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] IGetFinancialSetupUseCase useCase)
        {
            var response = await useCase.Execute();

            return Ok(response);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResponseFinancialSetup), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upsert(
            [FromBody] RequestUpsertFinancialSetup request,
            [FromServices] IUpsertFinancialSetupUseCase useCase)
        {
            var response = await useCase.Execute(request);

            return Ok(response);
        }
    }
}
