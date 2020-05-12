using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace cXMLHandler
{
    public interface IEnvironmentSettings
    {
        string S3BucketName { get; }
        string BucketPrefix { get; }
        string ASPNETCORE_ENVIRONMENT { get; }
        string EnvironmentName { get; }
        string AWSAccessKey { get; }
        string AWSSecret { get; }

    }

    public interface ISettingsProvider
    {
        string GetSetting(string key);
        string GetSecret(string key, ILogger<EnvironmentSettings> logger);
    }

    public class LocalEnvironmentProvider : ISettingsProvider
    {
        IConfiguration _config;

        public LocalEnvironmentProvider(IConfiguration config)
        {
            _config = config;
        }

        public string GetSecret(string key, ILogger<EnvironmentSettings> logger) => GetSetting(key);

        public string GetSetting(string key)
        {
            return _config[key];
        }
    }

    class LambdaEnvironmentProvider : ISettingsProvider
    {

        public string GetSetting(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        public string GetSecret(string key, ILogger<EnvironmentSettings> logger)
        {
            string secret = "";
            var secretRegion = RegionEndpoint.USWest2;
            string secretName = $"cxmlhandler-{GetSetting("EnvironmentName").ToLower()}";
            logger?.LogInformation($"Reading secret. Name={secretName}");

            MemoryStream memoryStream = new MemoryStream();
            IAmazonSecretsManager client = new AmazonSecretsManagerClient(secretRegion);
            GetSecretValueRequest request = new GetSecretValueRequest();
            request.SecretId = secretName;
            GetSecretValueResponse response = null;

            try
            {
                response = client.GetSecretValueAsync(request).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // Decrypts secret using the associated KMS CMK.
            // Depending on whether the secret is a string or binary, one of these fields will be populated.
            if (response.SecretString != null)
            {
                secret = response.SecretString;
            }
            else
            {
                memoryStream = response.SecretBinary;
                StreamReader reader = new StreamReader(memoryStream);
                secret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            logger?.LogInformation($"Parsing secret. key={key}");
            return JObject.Parse(secret)[key].ToString();
        }

    }

    public abstract class EnvironmentSettings : IEnvironmentSettings
    {
        ISettingsProvider _provider;
        ILogger<EnvironmentSettings> _logger;
        public ILogger<EnvironmentSettings> Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        public EnvironmentSettings(ISettingsProvider provider)
        {
            _provider = provider;
        }



        public string AWSAccessKey => _provider.GetSecret("AWSAccessKey", Logger);

        public string AWSSecret => _provider.GetSecret("AWSSecret", Logger);

        public string S3BucketName => _provider.GetSecret("S3BucketName", Logger);

        public string BucketPrefix => _provider.GetSecret("BucketPrefix", Logger);

        public string ASPNETCORE_ENVIRONMENT => _provider.GetSecret("ASPNETCORE_ENVIRONMENT", Logger);

        public string EnvironmentName => _provider.GetSecret("EnvironmentName", Logger);
    }


    public class LambdaEnvironmentSettings : EnvironmentSettings, IEnvironmentSettings
    {
        public LambdaEnvironmentSettings() : base(new LambdaEnvironmentProvider()) { }
    }

    public class LocalWindowsEnvironmentSettings : EnvironmentSettings, IEnvironmentSettings
    {
        //see https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows for explanation of local secret storage
        IConfiguration _config;

        public LocalWindowsEnvironmentSettings(IConfiguration config) : base(new LocalEnvironmentProvider(config))
        {
            _config = config;
        }

    }

    internal static class ConfigHelpers
    {
        internal static bool ParseBoolWithDefault(string source, bool defaultValue)
        {
            bool value;
            var result = bool.TryParse(source, out value);
            return result ? value : defaultValue;
        }
    }

}