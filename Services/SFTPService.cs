using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AMoura.Blogs.SFTPFunction
{
    public interface ISFTPService
    {
        Task UploadFileAsync(Stream fileToUpload, string fileName);
    }

    public class SFTPService : ISFTPService
    {
        private readonly ILogger<ISFTPService> _logger;
        private readonly IKeyVaultRepository _keyVaultRepository;
        private readonly ISFTPRepository _sftpRepository;

        public SFTPService(ILogger<ISFTPService> logger,
            IKeyVaultRepository keyVaultRepository,
            ISFTPRepository sftpRepository)
        {
            _logger = logger;
            _keyVaultRepository = keyVaultRepository;
            _sftpRepository = sftpRepository;
        }

        public async Task UploadFileAsync(Stream fileToUpload, string fileName)
        {
            _logger.LogInformation($"SFTPService -> Start UploadFileAsync (Filename={fileName}");

            try
            {
                // Retrieve secret from KeyVault
                var secretValue = await _keyVaultRepository.GetSecretAsync("UserSftp1PrivateKey");

                // Save the secret in a Stream
                var secretBytes = Convert.FromBase64String(secretValue.Value);
                MemoryStream ms = new MemoryStream();
                ms.Write(secretBytes, 0, secretBytes.Length);
                ms.Position = 0;

                // Upload the file to the SFTP server
                await _sftpRepository.UploadFileAsync(fileToUpload, ms, fileName);

                _logger.LogInformation($"SFTPService -> End UploadFileAsync (Filename={fileName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"SFTPService -> Error UploadFileAsync (Details: {ex.Message})");
                throw;
            }

        }
    }
}