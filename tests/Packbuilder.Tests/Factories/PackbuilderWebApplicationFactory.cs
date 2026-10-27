using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Packbuilder.Tests.Factories;
internal class PackbuilderWebApplicationFactory(string testConnectionString) : WebApplicationFactory<Program> 
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {   
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Db:ConnectionString"] = testConnectionString
                });
        });

        // Customize the host builder here 
        builder.ConfigureServices(services =>{}).UseDefaultServiceProvider(options => options.ValidateScopes = false);
    }
}