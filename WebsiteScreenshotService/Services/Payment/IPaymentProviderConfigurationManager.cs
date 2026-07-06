namespace WebsiteScreenshotService.Services.Payment;

public interface IPaymentProviderConfigurationManager
{
    public Configurations GetConfigurations();

    public SensitiveConfigurations GetSensitiveConfigurations();
}
