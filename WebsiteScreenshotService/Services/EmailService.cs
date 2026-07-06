using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using WebsiteScreenshotService.Configurations;

namespace WebsiteScreenshotService.Services;

public class AzureEmailService : IEmailService
{
    private readonly string _baseUrl;
    private readonly EmailClient _emailClient;
    private readonly EmailConfigurations _options;

    public AzureEmailService(
        IOptions<EmailConfigurations> options,
        IOptions<FrontendConfigurations> frontEndOptions)
    {
        var config = options.Value;
        _emailClient = new EmailClient(config.ConnectionString);
        _options = config;
        _baseUrl = frontEndOptions.Value!.BaseUrl;
    }

    public async Task SendAsync(EmailRequest request, CancellationToken cancellationToken = default)
    {
        var message = new EmailMessage(
            senderAddress: _options.SenderAddress,
            content: new EmailContent(request.Subject)
            {
                PlainText = request.PlainText,
                Html = request.Html
            },
            recipients: new EmailRecipients(
            [
                new EmailAddress(request.To)
            ])
        );

        try
        {
            var operation = await _emailClient.SendAsync(WaitUntil.Completed, message, cancellationToken);

            if (operation.Value.Status != EmailSendStatus.Succeeded)
                throw new Exception($"Email failed. Status: {operation.Value.Status}");
        }
        catch (RequestFailedException ex)
        {
            throw new ApplicationException("Failed to send email via Azure Communication Services", ex);
        }
    }

    public async Task SendResetPasswordEmailAsync(
        ResetPasswordEmail request,
        CancellationToken cancellationToken = default)
    {
        var resetUrl = $"{_baseUrl}/reset-password?token={Uri.EscapeDataString(request.Token)}";

        await SendAsync(
            new()
            {
                To = request.To,
                Subject = "Скидання пароля",
                Html = $$"""
                <h2>Скидання пароля</h2>
                <p>Ми отримали запит на скидання вашого пароля, скористайтесь захищеним посиланням нижче.</p>
                <p>{{resetUrl}}</p>
                <p>Якщо ви не надсилали цей запит, просто проігноруйте цей лист.</p>
                """,
                PlainText = $"Скиньте пароль за допомогою цього посилання: {resetUrl}"
            },
            cancellationToken);
    }

    public async Task SendRegisterFirstAdminEmailAsync(
        RegisterFirstAdminEmail request,
        CancellationToken cancellationToken = default)
    {
        var registrationUrl = $"{_baseUrl}/register-admin?token={Uri.EscapeDataString(request.Token)}";

        await SendAsync(new()
        {
            To = request.To,
            Subject = "Завершення створення першого адміністратора",
            Html = $$"""
                <h2>Створення першого адміністратора</h2>
                <p>Для платформи Screenshot Service було створено початковий запит на реєстрацію адміністратора</p>
                <p>Щоб завершити налаштування облікового запису адміністратора, скористайтесь захищеним посиланням нижче</p>
                <p>{{registrationUrl}}</p>
                <p>Це режстраційне посилання:</p>
                <p>може бути використане лише один раз</p>
                <p>дійсне протягом 30 хвилин</p>
                <p>не підлягає передаванню іншим особам</p>
                <br/>
                <p>Screenshot service</p>
                """,
            PlainText = $"Complete administrator account setup: {registrationUrl}",
        }, cancellationToken);
    }
}

public class EmailRequest
{
    public string To { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string? PlainText { get; set; }
    public string? Html { get; set; }
}