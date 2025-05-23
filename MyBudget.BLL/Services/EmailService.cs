using FluentEmail.Core;
using MyBudget.BLL.Services.Interfaces;

namespace MyBudget.BLL.Services;

public class EmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;

    public EmailService(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }
    
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        await _fluentEmail
            .To(toEmail)
            .Subject(subject)
            .Body(body)
            .SendAsync();
    }
}