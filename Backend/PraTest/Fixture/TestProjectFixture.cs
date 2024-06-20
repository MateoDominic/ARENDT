using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.AutoMapper;
using WebApi.Models;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace PraTest.Fixture
{
    public class TestProjectFixture : TestBedFixture
    {
        protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        {
            services.AddAutoMapper(typeof(MappingProfileProvider)).
                AddScoped<IDbService, DbServices>().
                AddScoped<IConnectionHandler, ConnectionHandler>()
                .AddDbContext<PraContext>(options =>
                    options.UseSqlServer("server=.;Database=PRA;User=sa;Password=neznam;TrustServerCertificate=True;MultipleActiveResultSets=true"),
                    ServiceLifetime.Transient);
        }

        protected override ValueTask DisposeAsyncCore()
        {
            return new();
        }

        protected override IEnumerable<TestAppSettings> GetTestAppSettings()
        {
            yield return new() { Filename = "appsettings.json", IsOptional = false };
        }
    }
}
