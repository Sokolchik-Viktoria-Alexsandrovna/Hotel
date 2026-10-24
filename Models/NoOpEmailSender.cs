using Microsoft.AspNetCore.Identity.UI.Services;

namespace Hotel.Models
{
    public class HotelEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
            => Task.CompletedTask;
    }
}
