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
            CreateMap<RequestExpense,Expense>();
            CreateMap<RequestFinancialObligation, FinancialObligation>();
            CreateMap<RequestRegisterUser, User>().ForMember(dest => dest.Password, config => config.Ignore());
        }

        private void EntityToResponse()
        {
            CreateMap<ExpenseAttachment, ResponseExpenseAttachment>();
            CreateMap<Expense, ResponseRegisteredExpense>()
                .ForMember(dest => dest.CategoryId, config => config.MapFrom(source => source.ExpenseCategoryId))
                .ForMember(dest => dest.CategoryName, config => config.MapFrom(source => source.ExpenseCategory != null ? source.ExpenseCategory.Name : null));
            CreateMap<Expense, ResponseShortExpense>()
                .ForMember(dest => dest.CategoryId, config => config.MapFrom(source => source.ExpenseCategoryId))
                .ForMember(dest => dest.CategoryName, config => config.MapFrom(source => source.ExpenseCategory != null ? source.ExpenseCategory.Name : null));
            CreateMap<Expense, ResponseExpense>()
                .ForMember(dest => dest.CategoryId, config => config.MapFrom(source => source.ExpenseCategoryId))
                .ForMember(dest => dest.CategoryName, config => config.MapFrom(source => source.ExpenseCategory != null ? source.ExpenseCategory.Name : null));
            CreateMap<FinancialObligation, ResponseFinancialObligation>()
                .ForMember(dest => dest.CategoryId, config => config.MapFrom(source => source.ExpenseCategoryId))
                .ForMember(
                    dest => dest.RequiresReview,
                    config => config.MapFrom(source =>
                        source.RecurrenceType == Domain.Enums.FinancialObligationRecurrenceType.VariableMonthly
                        || string.IsNullOrWhiteSpace(source.BucketCode)));
        }
    }
}
