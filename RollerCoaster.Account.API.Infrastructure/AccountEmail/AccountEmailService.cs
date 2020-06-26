using DickinsonBros.Email.Abstractions;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.AccountEmail
{
    public class AccountEmailService : IAccountEmailService
    {
        internal readonly IEmailService _emailService;
        internal const string FROM_EMAIL = "NoReply@RollerCoasterMaker.com";
        internal const string ACTIVATE_EMAIL_SUBJECT = "Roller Coaster Maker - Activate Email";
        internal const string PASSWORD_RESET_EMAIL_SUBJECT = "Roller Coaster Maker - Reset Password";

        public AccountEmailService
        (
            IEmailService emailService
        )
        {
            _emailService = emailService;
        }

        public async Task SendActivateEmailAsync(string email, string username, string activateToken, string emailPreferenceToken)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Roller Coaster Maker", FROM_EMAIL));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = ACTIVATE_EMAIL_SUBJECT;

            message.Body = new TextPart("plain")
            {
                Text = 
$@"ActivateEmail 
Username: {username}
ActivateToken: {activateToken}
UpdateEmailSettingsToken: {emailPreferenceToken}"
            };

            await _emailService.SendAsync(message).ConfigureAwait(false);
        }

        public async Task SendPasswordResetEmailAsync(string email, string passwordResetToken, string emailPreferenceToken)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Roller Coaster Maker", FROM_EMAIL));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = PASSWORD_RESET_EMAIL_SUBJECT;

            message.Body = new TextPart("plain")
            {
                Text = 
$@"Reset Password
PasswordResetToken: {passwordResetToken}
UpdateEmailSettingsToken: {emailPreferenceToken}"
            };

            await _emailService.SendAsync(message).ConfigureAwait(false);
        }
    }
}
