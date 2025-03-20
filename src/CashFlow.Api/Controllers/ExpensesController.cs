using CashFlow.Application.UseCases.Expenses.Delete;
using CashFlow.Application.UseCases.Expenses.GetAll;
using CashFlow.Application.UseCases.Expenses.GetById;
using CashFlow.Application.UseCases.Expenses.Register;
using CashFlow.Application.UseCases.Expenses.Update;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredExpense), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RequestExpense request, [FromServices] IRegisterExpenseUseCase useCase)
        {
            var response =  await useCase.Execute(request);

            return Created(string.Empty, response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseExpenses), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllExpenses([FromServices] IGetAllExpensesUseCase useCase )
        {
            var response = await useCase.Execute();

            if (response.Expenses.Count != 0)
                return Ok(response);

            return NoContent();
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseExpense), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromServices] IGetExpenseByIdUseCase useCase, [FromRoute] long id)
        {
            var response = await useCase.Execute(id);

            return Ok(response);
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromServices] IDeleteExpenseUseCase useCase, [FromRoute] long id)
        {
            await useCase.Execute(id);

            return NoContent();
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromServices] IUpdateExpenseUseCase useCase, [FromRoute] long id, [FromBody] RequestExpense request)
        {
            await useCase.Execute(id, request);
            
            return NoContent();
        }
    }
}
