using CashFlow.Application.UseCases.Expenses.Reports.Excel;
using CashFlow.Application.UseCases.Expenses.Reports.Pdf;
using CashFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CashFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.ADMIN)]
    public class ReportController : Controller
    {
        [HttpGet("excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetExcel([FromServices] IGenerateExpensesReportExcelUseCase useCase, [FromHeader] DateOnly month)
        {
            //usar o from header e dateOnly em um metodo get, faz sentido se poucos filtros
            //se forem muitos filtros faz sentido passar no body e mudar o metodo para post
            byte[] file = await useCase.Execute(month);

            if(file.Length > 0)
                return File(file, MediaTypeNames.Application.Octet, "report.xlsx");
            
            return NoContent();
        }

        [HttpGet("pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPdf([FromServices] IGenerateExpensesReportPdfuseCase useCase, [FromQuery] DateOnly month)
        {
            byte[] file = await useCase.Execute(month);

            if(file.Length > 0)
                return File(file, MediaTypeNames.Application.Pdf, "report.pdf");

            return NoContent();
        }
    }
}
