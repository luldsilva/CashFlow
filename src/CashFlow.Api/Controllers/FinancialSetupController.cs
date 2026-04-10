using CashFlow.Application.UseCases.FinancialSetup.Delete;
using CashFlow.Application.UseCases.FinancialSetup.Get;
using CashFlow.Application.UseCases.FinancialSetup.Register;
using CashFlow.Application.UseCases.FinancialSetup.Update;
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

        [HttpPost]
        [ProducesResponseType(typeof(ResponseFinancialSetup), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(
            [FromBody] RequestUpsertFinancialSetup request,
            [FromServices] IRegisterFinancialSetupUseCase useCase)
        {
            var response = await useCase.Execute(request);

            return Created(string.Empty, response);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResponseFinancialSetup), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            [FromBody] RequestUpsertFinancialSetup request,
            [FromServices] IUpdateFinancialSetupUseCase useCase)
        {
            var response = await useCase.Execute(request);

            return Ok(response);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromServices] IDeleteFinancialSetupUseCase useCase)
        {
            await useCase.Execute();

            return NoContent();
        }
    }
}
