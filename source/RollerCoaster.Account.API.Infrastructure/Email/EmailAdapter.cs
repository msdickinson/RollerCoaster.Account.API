using DickinsonBros.Email.Abstractions;
using MimeKit;
using RollerCoaster.Account.API.UseCases.Exceptions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.Email
{
    public class EmailAdapter : IEmail
    {
        internal readonly IEmailService _emailService;
        internal const string FROM_EMAIL = "NoReply@RollerCoasterMaker.com";
        internal const string ACTIVATE_EMAIL_SUBJECT = "Roller Coaster Maker - Activate Email";
        internal const string PASSWORD_RESET_EMAIL_SUBJECT = "Roller Coaster Maker - Reset Password";

        public EmailAdapter
        (
            IEmailService emailService
        )
        {
            _emailService = emailService;
        }

        public async Task SendActivateEmailAsync(SendActivateEmailRequest sendActivateEmailRequest)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Roller Coaster Maker", FROM_EMAIL));
                message.To.Add(new MailboxAddress("", sendActivateEmailRequest.Email));
                message.Subject = ACTIVATE_EMAIL_SUBJECT;

                message.Body = new TextPart("plain")
                {
                    Text =
$@"ActivateEmail 
Username: {sendActivateEmailRequest.Username}
ActivateToken: {sendActivateEmailRequest.ActivateToken}
UpdateEmailSettingsToken: {sendActivateEmailRequest.EmailPreferenceToken}"
                };

                await _emailService.SendAsync(message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new SendActivateEmailException(ex);
            }

        }

        public async Task VaildateEmailAsync(string email)
        {
            if (!_emailService.IsValidEmailFormat(email))
            {
                throw new InvaildEmailFormatException();
            }

            if (!await _emailService.ValidateEmailDomainAsync(email.Split("@").Last()).ConfigureAwait(false))
            {
                throw new InvaildEmailDomainException();
            }
        }
    }
}
