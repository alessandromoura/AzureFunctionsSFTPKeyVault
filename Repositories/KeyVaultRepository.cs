using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;

namespace AMoura.Blogs.SFTPFunction
{
    public interface IKeyVaultRepository
    {
        Task<KeyVaultSecret> GetSecretAsync(string secretName);
    }

    public class KeyVaultRepository : IKeyVaultRepository
    {
        private readonly ILogger<IKeyVaultRepository> _logger;

        public KeyVaultRepository(ILogger<IKeyVaultRepository> logger)
        {
            _logger = logger;
        }

        public async Task<KeyVaultSecret> GetSecretAsync(string secretName)
        {
            _logger.LogInformation($"KeyVaultRepository -> Start GetSecret (Filename={secretName}");

            string keyVaultUrl = Environment.GetEnvironmentVariable("KeyVaultUrl");

            try
            {
                var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
                var secretValue = await secretClient.GetSecretAsync(secretName);

                _logger.LogInformation($"KeyVaultRepository -> End GetSecret (Filename={secretName}");
                return secretValue;
            }
            catch (Exception ex)
            {
                _logger.LogError($"KeyVaultRepository -> Error GetSecret (Details: {ex.Message})");
                throw;
            }
        }
    }
}