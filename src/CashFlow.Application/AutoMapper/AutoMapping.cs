using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;

namespace CashFlow.Application.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            RequestToEntity();
            EntityToResponse();
        }
        private void RequestToEntity()
        {
            CreateMap<RequestRegisterExpense,Expense>();
        }

        private void EntityToResponse()
        {
            CreateMap<Expense, ResponseRegisteredExpense>();
        }
    }
}
