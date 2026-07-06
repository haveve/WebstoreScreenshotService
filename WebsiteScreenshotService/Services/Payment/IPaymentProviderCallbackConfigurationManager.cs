namespace WebsiteScreenshotService.Services.Payment;

public interface IPaymentProviderCallbackConfigurationManager
{
    public Configurations GetConfigurations();

    public SensitiveConfigurations GetSensitiveConfigurations();
}
