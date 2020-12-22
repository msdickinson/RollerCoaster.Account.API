using DickinsonBros.Email.Abstractions;
using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeKit;
using Moq;
using RollerCoaster.Account.API.Infrastructure.Email;
using RollerCoaster.Account.API.Infrastructure.Email.Extensions;
using RollerCoaster.Account.API.UseCases.Exceptions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.Tests.Email
{
    [TestClass]
    public class EmailAdapterTests : BaseTest
    {
        #region SendActivateEmailAsync

        [TestMethod]
        public async Task SendActivateEmailAsync_Runs_EmailServiceSendAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var sendActivateEmailRequest = new SendActivateEmailRequest
                    {
                        Email = "SampleEmail@EmailService.com",
                        Username = "SampleUsername",
                        ActivateToken = "SampleActivateToken",
                        EmailPreferenceToken = "SampleEmailPreferenceToken"
                    };

                    var observedMimeMessage = (MimeMessage)null;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.SendAsync
                            (
                                It.IsAny<MimeMessage>()
                            )
                        )
                        .Callback((MimeMessage mimeMessage) =>
                        {
                            observedMimeMessage = mimeMessage;
                        });

                    var uut = serviceProvider.GetRequiredService<IEmail>();
                    var uutConcrete = (EmailAdapter)uut;

                    //Act
                    await uutConcrete.SendActivateEmailAsync(sendActivateEmailRequest);

                    //Assert
                    emailServiceMock
                       .Verify(
                            emailService => emailService.SendAsync
                            (
                                It.IsAny<MimeMessage>()
                            ),
                           Times.Once
                       );

                    Assert.AreEqual(sendActivateEmailRequest.Email, observedMimeMessage.To.Mailboxes.First().Address);
                    Assert.AreEqual(EmailAdapter.ACTIVATE_EMAIL_SUBJECT, observedMimeMessage.Subject);
                    Assert.AreEqual(EmailAdapter.FROM_EMAIL, observedMimeMessage.From.Mailboxes.First().Address);
                    Assert.IsTrue(observedMimeMessage.Body.ToString().Contains(sendActivateEmailRequest.Username));
                    Assert.IsTrue(observedMimeMessage.Body.ToString().Contains(sendActivateEmailRequest.ActivateToken));
                    Assert.IsTrue(observedMimeMessage.Body.ToString().Contains(sendActivateEmailRequest.EmailPreferenceToken));
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task SendActivateEmailAsync_EmailIsNull_ThrowsArgumentNullException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var sendActivateEmailRequest = new SendActivateEmailRequest
                    {
                        Email = null,
                        Username = "SampleUsername",
                        ActivateToken = "SampleActivateToken",
                        EmailPreferenceToken = "SampleEmailPreferenceToken"
                    };

                    var observedMimeMessage = (MimeMessage)null;

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                        .Setup
                        (
                            emailService => emailService.SendAsync
                            (
                                It.IsAny<MimeMessage>()
                            )
                        )
                        .Callback((MimeMessage mimeMessage) =>
                        {
                            observedMimeMessage = mimeMessage;
                        });

                    var uut = serviceProvider.GetRequiredService<IEmail>();
                    var uutConcrete = (EmailAdapter)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<SendActivateEmailException>(async () => await uut.SendActivateEmailAsync(sendActivateEmailRequest));

                    //Assert

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region VaildateEmailAsync

        [TestMethod]
        public async Task VaildateEmailAsync_InvaildEmailFormat_ThrowsInvaildEmailFormatException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var email = "BadEmailFormat";

                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                    .Setup
                    (
                        emailService => emailService.IsValidEmailFormat
                        (
                            It.IsAny<string>()
                        )
                    )
                    .Returns(false);

                    var uut = serviceProvider.GetRequiredService<IEmail>();
                    var uutConcrete = (EmailAdapter)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<InvaildEmailFormatException>(async () => await uut.VaildateEmailAsync(email).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert
                  
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task VaildateEmailAsync_InvaildEmailDomainException_ThrowsInvaildEmailDomainException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var email = "SampleEmail@Email.com";

                    //--IEmailService
                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                    .Setup
                    (
                        emailService => emailService.IsValidEmailFormat
                        (
                            It.IsAny<string>()
                        )
                    )
                    .Returns(true);

                    emailServiceMock
                    .Setup
                    (
                        emailService => emailService.ValidateEmailDomainAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync(false);

                    var uut = serviceProvider.GetRequiredService<IEmail>();
                    var uutConcrete = (EmailAdapter)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<InvaildEmailDomainException>(async () => await uut.VaildateEmailAsync(email).ConfigureAwait(false)).ConfigureAwait(false);

                    //Assert

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task VaildateEmailAsync_VaildEmailAndDomain_DoesNotThrow()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var email = "SampleEmail@Email.com";

                    //--IEmailService
                    var emailServiceMock = serviceProvider.GetMock<IEmailService>();
                    emailServiceMock
                    .Setup
                    (
                        emailService => emailService.IsValidEmailFormat
                        (
                            It.IsAny<string>()
                        )
                    )
                    .Returns(true);

                    emailServiceMock
                    .Setup
                    (
                        emailService => emailService.ValidateEmailDomainAsync
                        (
                            It.IsAny<string>()
                        )
                    )
                    .ReturnsAsync(true);

                    var uut = serviceProvider.GetRequiredService<IEmail>();
                    var uutConcrete = (EmailAdapter)uut;

                    //Act
                    await  uut.VaildateEmailAsync(email).ConfigureAwait(false);

                    //Assert

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region Helpers
        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddEmailAdapter();
            serviceCollection.AddSingleton(Mock.Of<IEmailService>());

            return serviceCollection;
        }

        #endregion
    }
}
