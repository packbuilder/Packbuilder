using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Packbuilder.Policys;

namespace Packbuilder.Tests.Factories;
internal class PackbuilderWebApplicationFactory(string testConnectionString) : WebApplicationFactory<Program> 
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {   
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddUserSecrets<Program>();
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["DB:TestConnectionString"] = testConnectionString
                });
        });

        // Customize the host builder here 
        builder.ConfigureServices(services =>{
            services.AddAuthorizationBuilder()
                .AddPolicy("VerifiedEmail", policy =>
                    policy.RequireAuthenticatedUser()
                        .AddRequirements(new EmailVerifiedRequirement()));
        })
        .UseDefaultServiceProvider(options => {
            options.ValidateScopes = false;
        });
    }
}