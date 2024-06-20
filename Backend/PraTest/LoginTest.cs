using PraTest.Fixture;
using WebApi.Utilities;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace PraTest
{
    [CollectionDefinition("Dependency Injection")]
    public class LoginTest : TestBed<TestProjectFixture>
    {
        //private readonly IDbService _dbService;
        public LoginTest(ITestOutputHelper testOutputHelper, TestProjectFixture fixture) : base(testOutputHelper, fixture)
        {

            /*var mapper = MappingProfileProvider.InitializeAutoMapper().CreateMapper();
            var context = DbContextProvider.GetDbContext();
            _dbService = new DbServices(context, mapper);*/
        }

        [Fact]
        public void Login_Success()
        {
            var _dbService = _fixture.GetService<IDbService>(_testOutputHelper);
            Assert.True(_dbService.LoginUser(new UserLoginDTO() { 
                Username = "Pero",
                Password = "password"
            }));
        }

        [Fact]
        public void Login_Failure()
        {
            var _dbService = _fixture.GetService<IDbService>(_testOutputHelper);
            Assert.True(_dbService.LoginUser(new UserLoginDTO()
            {
                Username = "Pero",
                Password = "WrongPassword"
            }));
        }
    }
}
