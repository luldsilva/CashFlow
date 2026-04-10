using CashFlow.Application.UseCases.ExpenseCategories.Delete;
using CashFlow.Application.UseCases.ExpenseCategories.GetAll;
using CashFlow.Application.UseCases.ExpenseCategories.Register;
using CashFlow.Application.UseCases.ExpenseCategories.Update;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers
{
    [Route("api/expense-categories")]
    [ApiController]
    [Authorize]
    public class ExpenseCategoriesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ResponseExpenseCategories), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromServices] IGetAllExpenseCategoriesUseCase useCase)
        {
            var response = await useCase.Execute();
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseExpenseCategory), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(
            [FromBody] RequestManageExpenseCategory request,
            [FromServices] IRegisterExpenseCategoryUseCase useCase)
        {
            var response = await useCase.Execute(request);
            return Created(string.Empty, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseExpenseCategory), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(
            [FromRoute] long id,
            [FromBody] RequestManageExpenseCategory request,
            [FromServices] IUpdateExpenseCategoryUseCase useCase)
        {
            var response = await useCase.Execute(id, request);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(
            [FromRoute] long id,
            [FromServices] IDeleteExpenseCategoryUseCase useCase)
        {
            await useCase.Execute(id);
            return NoContent();
        }
    }
}
