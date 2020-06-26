using DickinsonBros.Email.Abstractions;
using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeKit;
using Moq;
using RollerCoaster.Account.API.Infrastructure.AccountEmail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Logic.Tests
{
    [TestClass]
    public class AccountEmailServiceTests : BaseTest
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
                    var email                        = "SampleEmail@EmailService.com";
                    var username                     = "SampleUsername";
                    var activateToken                = "SampleActivateToken";
                    var emailPreferenceToken         = "SampleEmailPreferenceToken";

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

                    var uut = serviceProvider.GetRequiredService<IAccountEmailService>();
                    var uutConcrete = (AccountEmailService)uut;

                    //Act
                    await uut.SendActivateEmailAsync(email, username, activateToken, emailPreferenceToken);

                    //Assert
                    emailServiceMock
                       .Verify(
                            emailService => emailService.SendAsync
                            (
                                It.IsAny<MimeMessage>()
                            ),
                           Times.Once
                       );

                    Assert.AreEqual(email, observedMimeMessage.To.Mailboxes.First().Address);
                    Assert.AreEqual(AccountEmailService.ACTIVATE_EMAIL_SUBJECT, observedMimeMessage.Subject); 
                    Assert.AreEqual(AccountEmailService.FROM_EMAIL, observedMimeMessage.From.Mailboxes.First().Address);
                    Assert.IsTrue(observedMimeMessage.Body.ToString().Contains(username));
                    Assert.IsTrue(observedMimeMessage.Body.ToString().Contains(activateToken));
                    Assert.IsTrue(observedMimeMessage.Body.ToString().Contains(emailPreferenceToken));
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
                    var email                       = (string)null;
                    var username                    = "SampleUsername";
                    var activateToken               = "SampleActivateToken";
                    var emailPreferenceToken        = "SampleUpdateEmailSettingsToken";

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

                    var uut = serviceProvider.GetRequiredService<IAccountEmailService>();
                    var uutConcrete = (AccountEmailService)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await uut.SendActivateEmailAsync(email, username, activateToken, emailPreferenceToken)); 

                    //Assert

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region SendPasswordResetEmailAsync

        [TestMethod]
        public async Task SendPasswordResetEmailAsync_Runs_EmailServiceSendAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var email                   = "SampleEmail@EmailService.com";
                    var username                = "SampleUsername";
                    var emailPreferenceToken    = "SampleEmailPreferenceToken";

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

                    var uut = serviceProvider.GetRequiredService<IAccountEmailService>();
                    var uutConcrete = (AccountEmailService)uut;

                    //Act
                    await uut.SendPasswordResetEmailAsync(email, username, emailPreferenceToken);

                    //Assert
                    emailServiceMock
                       .Verify(
                            emailService => emailService.SendAsync
                            (
                                It.IsAny<MimeMessage>()
                            ),
                           Times.Once
                       );

                    Assert.AreEqual(email, observedMimeMessage.To.Mailboxes.First().Address);
                    Assert.AreEqual(AccountEmailService.PASSWORD_RESET_EMAIL_SUBJECT, observedMimeMessage.Subject);
                    Assert.AreEqual(AccountEmailService.FROM_EMAIL, observedMimeMessage.From.Mailboxes.First().Address);
                    Assert.IsTrue(observedMimeMessage.Body.ToString().Contains(username));
                    Assert.IsTrue(observedMimeMessage.Body.ToString().Contains(emailPreferenceToken));
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task SendPasswordResetEmailAsync_EmailIsNull_ThrowsArgumentNullException()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var email = (string)null;
                    var username = "SampleUsername";
                    var emailPreferenceToken = "SampleEmailPreferenceToken";

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

                    var uut = serviceProvider.GetRequiredService<IAccountEmailService>();
                    var uutConcrete = (AccountEmailService)uut;

                    //Act
                    await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await uut.SendPasswordResetEmailAsync(email, username, emailPreferenceToken));

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAccountEmailService, AccountEmailService>();
            serviceCollection.AddSingleton(Mock.Of<IEmailService>());

            return serviceCollection;
        }
        #endregion
    }
}
