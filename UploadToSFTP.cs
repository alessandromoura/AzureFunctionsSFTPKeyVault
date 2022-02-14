using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AMoura.Blogs.SFTPFunction
{
    public class UploadToSFTP
    {
        private readonly ISFTPService _service;

        public UploadToSFTP(ISFTPService service)
        {
            _service = service;
        }

        [FunctionName("UploadToSFTP")]
        public async Task Run(
            [BlobTrigger("copytosftp/{filename}", Connection = "StorageConnection")]Stream fileToUpload, 
            string filename, 
            ILogger log)
        {
            log.LogInformation($"UploadToSFTP Function -> Start (FileName={filename})");

            try
            {
                await _service.UploadFileAsync(fileToUpload, filename);
                
                log.LogInformation($"UploadToSFTP Function -> End");
            }
            catch (Exception ex)
            {
                log.LogError($"UploadToSFTP Function -> Error (Details={ex.Message})");
                throw;
            }
        }
    }
}
