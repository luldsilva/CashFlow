using System.Net;

namespace CashFlow.Exception.ExceptionsBase
{
    public class ConflictException : CashFlowException
    {
        public ConflictException(string message) : base(message)
        {
        }

        public override int StatusCode => (int)HttpStatusCode.Conflict;

        public override List<string> GetErrors()
        {
            return [Message];
        }
    }
}
