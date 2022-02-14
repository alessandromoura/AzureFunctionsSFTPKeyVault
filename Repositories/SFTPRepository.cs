using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace AMoura.Blogs.SFTPFunction
{
    public interface ISFTPRepository
    {
        Task UploadFileAsync(Stream fileToCopy, Stream keyFile, string fileName);
    }

    public class SFTPRepository : ISFTPRepository
    {
        private readonly ILogger<ISFTPRepository> _logger;

        public SFTPRepository(ILogger<ISFTPRepository> logger)
        {
            _logger = logger;
        }

        public async Task UploadFileAsync(Stream fileToCopy, Stream keyFile, string fileName)
        {
            _logger.LogInformation($"SFTPRepository -> Start UploadFileAsync (Filename={fileName}");

            string sftpUsername = Environment.GetEnvironmentVariable("SFTPUsername");
            string sftpAddress = Environment.GetEnvironmentVariable("SFTPAddress");
            string sftpFolderName = Environment.GetEnvironmentVariable("SFTPFolderName");

            try {
                // Create authentication method as SSH
                PrivateKeyFile pkf = new PrivateKeyFile(keyFile);
                PrivateKeyAuthenticationMethod pkam = new PrivateKeyAuthenticationMethod(sftpUsername, pkf);
                AuthenticationMethod[] am = {pkam};

                // Connect to the SFTP server
                ConnectionInfo ci = new ConnectionInfo(sftpAddress, sftpUsername, am);
                SftpClient sftpClient = new SftpClient(ci);
                sftpClient.Connect();

                // Upload file to SFTP server
                await Task.Factory.FromAsync(
                    sftpClient.BeginUploadFile(fileToCopy, $"{sftpFolderName}/{fileName}"),
                    sftpClient.EndUploadFile);

                _logger.LogInformation($"SFTPRepository -> End UploadFileAsync (Filename={fileName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"SFTPRepository -> Error UploadFileAsync (Details: {ex.Message})");
                throw;
            }
        }
    }
}