using CashFlow.Application.UseCases.CreditCards.GetAll;
using CashFlow.Application.UseCases.CreditCards.Register;
using CashFlow.Application.UseCases.CreditCardStatements.GetByMonth;
using CashFlow.Application.UseCases.CreditCardStatements.Register;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers
{
    [Route("api/credit-cards")]
    [ApiController]
    [Authorize]
    public class CreditCardsController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseCreditCard), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Register(
            [FromBody] RequestCreditCard request,
            [FromServices] IRegisterCreditCardUseCase useCase)
        {
            var response = await useCase.Execute(request);
            return Created(string.Empty, response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseCreditCards), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromServices] IGetAllCreditCardsUseCase useCase)
        {
            var response = await useCase.Execute();
            return Ok(response);
        }

        [HttpPost("statements")]
        [ProducesResponseType(typeof(ResponseCreditCardStatement), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegisterStatement(
            [FromBody] RequestCreditCardStatement request,
            [FromServices] IRegisterCreditCardStatementUseCase useCase)
        {
            var response = await useCase.Execute(request);
            return Created(string.Empty, response);
        }

        [HttpGet("statements")]
        [ProducesResponseType(typeof(ResponseCreditCardStatements), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStatements(
            [FromQuery] DateTime month,
            [FromServices] IGetCreditCardStatementsByMonthUseCase useCase)
        {
            var response = await useCase.Execute(month);
            if (response.Statements.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }
    }
}
