using WebApi.Models;

namespace PraTest
{
    public class RepositoryMock
    {
        public static DbServices GetUserMock()
        {
            List<User> lstUser = GenerateTestData();
            PraContext dbContextMock = DbContextMock.GetMock<User, PraContext>(lstUser, x => x.Users);
            return new DbServices(dbContextMock, MappingProfileProvider.InitializeAutoMapper().CreateMapper());
        }

        public static DbServices GetQuizMock()
        {
            List<Quiz> lstUser = GenerateQuizData();
            PraContext dbContextMock = DbContextMock.GetMock<Quiz, PraContext>(lstUser, x => x.Quizzes);
            return new DbServices(dbContextMock, MappingProfileProvider.InitializeAutoMapper().CreateMapper());
        }

        public static DbServices GetQuizHistoryMock()
        {
            List<QuizHistory> lstUser = GenerateQuizHistoryData();
            PraContext dbContextMock = DbContextMock.GetMock<QuizHistory, PraContext>(lstUser, x => x.QuizHistories);
            return new DbServices(dbContextMock, MappingProfileProvider.InitializeAutoMapper().CreateMapper());
        }

        private static List<QuizHistory> GenerateQuizHistoryData()
        {

            List<QuizHistory> quiz = new List<QuizHistory>();
            quiz.Add(new QuizHistory()
            {
                Id = 1,
                PlayedAt = DateTime.Now,
                QuizId = 1,
                WinnerName = "Name",
                WinnerScore = 500
            });
            return quiz;
        }

        private static List<Quiz> GenerateQuizData()
        {
            List<Quiz> quiz = new List<Quiz>();
            return quiz;
        }

        private static List<User> GenerateTestData()
        {
            List<User> lstUser = new()
            {
                new User
                {
                    Id = 1,
                    FirstName = "Pero",
                    LastName = "P",
                    Email = "p@pero.hr",
                    JoinDate = DateTime.Now,
                    Username = "Pero",
                    PasswordHash = "FtJ4cYtOjs/FvfV9RIU1OTYbYWAcATua19Uvr6A94bI=",
                    PasswordSalt = "l/XCejNsK9oITTxe8I2niQ=="
                }
            };
            return lstUser;
        }
    }
}
