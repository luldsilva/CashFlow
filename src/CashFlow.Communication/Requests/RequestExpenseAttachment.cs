using Microsoft.AspNetCore.Http;

namespace CashFlow.Communication.Requests
{
    public class RequestExpenseAttachment
    {
        public IFormFile File { get; set; } = default!;
    }
}
