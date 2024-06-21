using WebApi.Models;

namespace PraTest
{
    public class UserTest
    {
        private readonly DbServices dbServices;
        //private readonly IDbService _dbService;
        public UserTest()
        {
            dbServices = RepositoryMock.GetUserMock();
        }

        [Fact]
        public void User_ChangePassword_Success()
        {
            dbServices.UpdateUserPassword(new UserChangePasswordDTO()
            {
                Username = "Pero",
                OldPassword = "password",
                NewPassword = "password2"
            });
            Assert.True(dbServices.LoginUser(new UserLoginDTO()
            {
                Username = "Pero",
                Password = "password2"
            }));
        }

        [Fact]
        public void User_Delete_Success()
        {
            var addedUser = dbServices.RegisterUser(new UserRegisterDTO() 
            {
                FirstName = "Marko",
                LastName = "M",
                Email = "m@ko.hr",
                Username = "Marko",
                Password = "newPassword"
            });
            int startingCount = dbServices.GetUsers().Count();
            dbServices.DeleteUser(addedUser);
            int afterCount = dbServices.GetUsers().Count();
            Assert.Equal(startingCount - 1, afterCount);
        }

        [Fact]
        public void User_ChangePassword_Failure()
        {
            dbServices.UpdateUserPassword(new UserChangePasswordDTO()
            {
                Username = "Pero",
                OldPassword = "WrongPassword",
                NewPassword = "password2"
            });
            Assert.False(dbServices.LoginUser(new UserLoginDTO()
            {
                Username = "Pero",
                Password = "password2"
            }));
        }

        [Fact]
        public void User_Login_Success()
        {
            Assert.True(dbServices.LoginUser(new UserLoginDTO() { 
                Username = "Pero",
                Password = "password"
            }));
        }

        [Fact]
        public void User_Login_Failure()
        {
            Assert.False(dbServices.LoginUser(new UserLoginDTO()
            {
                Username = "Pero",
                Password = "WrongPassword"
            }));
        }

        [Fact]
        public void User_Register_Success()
        {
            dbServices.RegisterUser(new UserRegisterDTO()
            {
                Email = "mock@eample.hr",
                FirstName = "New",
                LastName = "User",
                Password = "password",
                Username = "newuser",
                Id = 0
            });
            Assert.Equal(2, dbServices.GetUsers().Count());
        }

        [Fact]
        public void User_UpdateSuccess()
        {
            var newUser = new UserRegisterDTO()
            {
                Id = 2,
                FirstName = "Marko",
                LastName = "M",
                Email = "m@ko.hr",
                Username = "Marko",
                Password = "newPassword"
            };
            var addedUser = dbServices.RegisterUser(newUser);
            if (dbServices.UpdateUserInfo(new UserDTO()
            {
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = "Marko",
                JoinDate = DateTime.Now,
                Username = "MarkoNewAndImporoved",
                Id = 2
            })){
                Assert.Equal("MarkoNewAndImporoved", dbServices.GetUser(2).Username);
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
