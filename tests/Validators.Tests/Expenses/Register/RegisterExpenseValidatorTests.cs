using CashFlow.Application.UseCases.Expenses.Register;
using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;
using FluentAssertions;

namespace Validators.Tests.Expenses.Register
{
    public class RegisterExpenseValidatorTests
    {
        [Fact]

        public void Success()
        {
            //Arrange configuracao, instanciacao das propriedades para executar o teste
            var validator = new RegisterExpenseValidator();
            var request = RequestRegisterExpenseBuilder.Build(); //usando bpgus para mockar as requisicoes

            //Act executa a chamada ao metodo que vai ser testado
            var result = validator.Validate(request);

            //Assert compara o resultado para gerar o retorno final, se o teste passou ou falhou
            result.IsValid.Should().BeTrue(); //usando fluentassertions para fazer as comparacoes
        }
    }
}
