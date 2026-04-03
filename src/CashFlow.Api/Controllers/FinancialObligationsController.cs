using CashFlow.Application.UseCases.FinancialObligations.Delete;
using CashFlow.Application.UseCases.FinancialObligations.GetAll;
using CashFlow.Application.UseCases.FinancialObligations.GetById;
using CashFlow.Application.UseCases.FinancialObligations.Register;
using CashFlow.Application.UseCases.FinancialObligations.Update;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers
{
    [Route("api/financial-obligations")]
    [ApiController]
    [Authorize]
    public class FinancialObligationsController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseFinancialObligation), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Register(
            [FromBody] RequestFinancialObligation request,
            [FromServices] IRegisterFinancialObligationUseCase useCase)
        {
            var response = await useCase.Execute(request);

            return Created(string.Empty, response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseFinancialObligations), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime month,
            [FromServices] IGetAllFinancialObligationsUseCase useCase)
        {
            var response = await useCase.Execute(month);

            if (response.Obligations.Count != 0)
            {
                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseFinancialObligation), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            [FromRoute] long id,
            [FromServices] IGetFinancialObligationByIdUseCase useCase)
        {
            var response = await useCase.Execute(id);

            return Ok(response);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            [FromRoute] long id,
            [FromBody] RequestFinancialObligation request,
            [FromServices] IUpdateFinancialObligationUseCase useCase)
        {
            await useCase.Execute(id, request);

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            [FromRoute] long id,
            [FromServices] IDeleteFinancialObligationUseCase useCase)
        {
            await useCase.Execute(id);

            return NoContent();
        }
    }
}
