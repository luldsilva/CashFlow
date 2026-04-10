using CashFlow.Application.AutoMapper;
using CashFlow.Application.UseCases.CreditCards.GetAll;
using CashFlow.Application.UseCases.CreditCards.Register;
using CashFlow.Application.UseCases.CreditCardStatements.GetByMonth;
using CashFlow.Application.UseCases.CreditCardStatements.Register;
using CashFlow.Application.UseCases.Expenses.Attachments;
using CashFlow.Application.UseCases.ExpenseCategories.Delete;
using CashFlow.Application.UseCases.ExpenseCategories.GetAll;
using CashFlow.Application.UseCases.ExpenseCategories.Register;
using CashFlow.Application.UseCases.ExpenseCategories.Update;
using CashFlow.Application.UseCases.Dashboard.GetMonthlySummary;
using CashFlow.Application.UseCases.Expenses.Delete;
using CashFlow.Application.UseCases.Expenses.GetAll;
using CashFlow.Application.UseCases.Expenses.GetById;
using CashFlow.Application.UseCases.Expenses.Register;
using CashFlow.Application.UseCases.Expenses.Reports.Excel;
using CashFlow.Application.UseCases.Expenses.Reports.Pdf;
using CashFlow.Application.UseCases.Expenses.Update;
using CashFlow.Application.UseCases.FinancialObligations.Delete;
using CashFlow.Application.UseCases.FinancialObligations.GetAll;
using CashFlow.Application.UseCases.FinancialObligations.GetById;
using CashFlow.Application.UseCases.FinancialObligations.Register;
using CashFlow.Application.UseCases.FinancialObligations.Update;
using CashFlow.Application.UseCases.FinancialSetup.Delete;
using CashFlow.Application.UseCases.FinancialSetup.Get;
using CashFlow.Application.UseCases.FinancialSetup.Register;
using CashFlow.Application.UseCases.FinancialSetup.Update;
using CashFlow.Application.UseCases.Login.DoLogin;
using CashFlow.Application.UseCases.MonthlyReview.Close;
using CashFlow.Application.UseCases.MonthlyReview.Get;
using CashFlow.Application.UseCases.MonthlyReview.Prepare;
using CashFlow.Application.UseCases.Users.ChangePassword;
using CashFlow.Application.UseCases.Users.ForgotPassword;
using CashFlow.Application.UseCases.Users.ResetPassword;
using CashFlow.Application.UseCases.Users.Register;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Application
{
    public static class DependencyInjectionExtension
    {
        public static void AddApplication(this IServiceCollection services)
        {
            AddAutoMapper(services);
            AddUsesCases(services);
        }

        private static void AddAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(_ => { }, typeof(AutoMapping).Assembly);
        }

        private static void AddUsesCases(IServiceCollection services)
        {
            services.AddScoped<IRegisterCreditCardUseCase, RegisterCreditCardUseCase>();
            services.AddScoped<IGetAllCreditCardsUseCase, GetAllCreditCardsUseCase>();
            services.AddScoped<IRegisterCreditCardStatementUseCase, RegisterCreditCardStatementUseCase>();
            services.AddScoped<IGetCreditCardStatementsByMonthUseCase, GetCreditCardStatementsByMonthUseCase>();
            services.AddScoped<IGetAllExpenseCategoriesUseCase, GetAllExpenseCategoriesUseCase>();
            services.AddScoped<IRegisterExpenseCategoryUseCase, RegisterExpenseCategoryUseCase>();
            services.AddScoped<IUpdateExpenseCategoryUseCase, UpdateExpenseCategoryUseCase>();
            services.AddScoped<IDeleteExpenseCategoryUseCase, DeleteExpenseCategoryUseCase>();
            services.AddScoped<IRegisterExpenseUseCase, RegisterExpenseUseCase>();
            services.AddScoped<IAddExpenseAttachmentUseCase, AddExpenseAttachmentUseCase>();
            services.AddScoped<IGetMonthlySummaryUseCase, GetMonthlySummaryUseCase>();
            services.AddScoped<ICloseMonthUseCase, CloseMonthUseCase>();
            services.AddScoped<IGetMonthlyReviewUseCase, GetMonthlyReviewUseCase>();
            services.AddScoped<IPrepareMonthUseCase, PrepareMonthUseCase>();
            services.AddScoped<IGetAllExpensesUseCase, GetAllExpenseUseCase>();
            services.AddScoped<IGetExpenseByIdUseCase, GetExpenseByIdUseCase>();
            services.AddScoped<IDeleteExpenseUseCase, DeleteExpenseUseCase>();
            services.AddScoped<IUpdateExpenseUseCase, UpdateExpenseUseCase>();
            services.AddScoped<IGenerateExpensesReportExcelUseCase, GenerateExpensesReportExcelUseCase>();
            services.AddScoped<IGenerateExpensesReportPdfuseCase, GenerateExpensesReportPdfUseCase>();
            services.AddScoped<IRegisterFinancialObligationUseCase, RegisterFinancialObligationUseCase>();
            services.AddScoped<IGetAllFinancialObligationsUseCase, GetAllFinancialObligationsUseCase>();
            services.AddScoped<IGetFinancialObligationByIdUseCase, GetFinancialObligationByIdUseCase>();
            services.AddScoped<IUpdateFinancialObligationUseCase, UpdateFinancialObligationUseCase>();
            services.AddScoped<IDeleteFinancialObligationUseCase, DeleteFinancialObligationUseCase>();
            services.AddScoped<IDeleteFinancialSetupUseCase, DeleteFinancialSetupUseCase>();
            services.AddScoped<IGetFinancialSetupUseCase, GetFinancialSetupUseCase>();
            services.AddScoped<IRegisterFinancialSetupUseCase, RegisterFinancialSetupUseCase>();
            services.AddScoped<IUpdateFinancialSetupUseCase, UpdateFinancialSetupUseCase>();
            services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
            services.AddScoped<IForgotPasswordUseCase, ForgotPasswordUseCase>();
            services.AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>();
            services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
            services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();
        }
    }
}
