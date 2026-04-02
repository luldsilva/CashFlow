using CashFlow.Application.UseCases.Expenses.Reports.Excel;
using CashFlow.Application.UseCases.Expenses.Reports.Pdf;
using CashFlow.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CashFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : Controller
    {
        //Estamos validando o modelo? ex: tipo de dados inseridos etc
        [HttpGet("excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExcel([FromServices] IGenerateExpensesReportExcelUseCase useCase, [FromQuery] DateOnly month)
        {
            //usar o from header e dateOnly em um metodo get, faz sentido se poucos filtros
            //se forem muitos filtros faz sentido passar no body e mudar o metodo para post
            byte[] file = await useCase.Execute(month);
            return File(file, MediaTypeNames.Application.Octet, "report.xlsx");
        }

        [HttpGet("pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPdf([FromServices] IGenerateExpensesReportPdfuseCase useCase, [FromQuery] DateOnly month)
        {
            byte[] file = await useCase.Execute(month);
            return File(file, MediaTypeNames.Application.Pdf, "report.pdf");
        }
    }
}
