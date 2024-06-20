using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Utilities;

namespace PraTest
{
    public class DbContextProvider
    {
        public static PraContext GetDbContext() {
            /*var options = new DbContextOptionsBuilder<PraContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var databaseContext = new PraContext(options);
            if (databaseContext.Users.Count() == 0)
            {
                databaseContext.Users.Add(new User { 
                    FirstName = "Pero",
                    LastName = "P",
                    Email = "p@pero.hr",
                    JoinDate = DateTime.Now,
                    Username = "Pero",
                    PasswordHash = "FtJ4cYtOjs/FvfV9RIU1OTYbYWAcATua19Uvr6A94bI=",
                    PasswordSalt = "l/XCejNsK9oITTxe8I2niQ=="
                });
                databaseContext.SaveChanges();
            }
            return databaseContext;*/
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(MappingProfileProvider)).
                AddScoped<IDbService, DbServices>()
                .AddDbContext<PraContext>(options =>
                    options.UseSqlServer("server=.;Database=PRA;User=sa;Password=neznam;TrustServerCertificate=True;MultipleActiveResultSets=true"),
                    ServiceLifetime.Transient);
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetService<PraContext>();

        }
    }
}
