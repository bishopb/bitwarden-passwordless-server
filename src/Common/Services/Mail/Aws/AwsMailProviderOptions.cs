namespace Passwordless.Common.Services.Mail.Aws;

public class AwsMailProviderOptions : BaseMailProviderOptions
{
    public const string Provider = "aws";

    public AwsMailProviderOptions()
    {
        Name = Provider;
    }

    public string AccessKey { get; set; }

    public string SecretKey { get; set; }

    public string Region { get; set; }
}