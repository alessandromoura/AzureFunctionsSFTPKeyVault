using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AMoura.Blogs.SFTPFunction.Startup))]
namespace AMoura.Blogs.SFTPFunction
{
    public class Startup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register services classes
            builder.Services.AddTransient<ISFTPService, SFTPService>();

            // Register repository classes
            builder.Services.AddScoped<IKeyVaultRepository, KeyVaultRepository>();
            builder.Services.AddScoped<ISFTPRepository, SFTPRepository>();
        }
    }
}