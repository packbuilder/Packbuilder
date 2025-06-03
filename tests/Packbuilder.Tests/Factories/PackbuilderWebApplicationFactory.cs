using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Packbuilder.Tests.Factories;
internal class PackbuilderWebApplicationFactory : WebApplicationFactory<Program> 
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Customize the host builder here 
        builder.ConfigureServices(services =>{})
        .UseDefaultServiceProvider(options => options.ValidateScopes = false);
    }
}