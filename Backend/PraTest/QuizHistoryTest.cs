using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PraTest
{
    public class QuizHistoryTest
    {
        private readonly DbServices dbServices;
        //private readonly IDbService _dbService;
        public QuizHistoryTest()
        {
            dbServices = RepositoryMock.GetQuizHistoryMock();
        }

        [Fact]
        private void QuizHistory_Delete_Success()
        {
            int startingCount = dbServices.GetAllQuizHistory().Count();
            var deletedQuizHistory = dbServices.DeleteQuizHistory(dbServices.GetAllQuizHistory().First().Id);
            int afterCount = dbServices.GetAllQuizHistory().Count();
            if (deletedQuizHistory != null)
            {
                Assert.Equal(startingCount - 1, afterCount);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Fact]
        private void QuizHistory_Add_Success()
        {
            int startingCount = dbServices.GetAllQuizHistory().Count();
            dbServices.AddQuizHistory(new QuizHistoryDTO() { 
                PlayedAt = DateTime.Now,
                WinnerName = "Pero",
                QuizId = 1,
            });
            int afterCount = dbServices.GetAllQuizHistory().Count();
            Assert.Equal(startingCount + 1, afterCount);
        }

        [Fact]
        private void QuizHistory_GetByQuizId_Success()
        {
            dbServices.AddQuizHistory(new QuizHistoryDTO()
            {
                PlayedAt = DateTime.Now,
                WinnerName = "Pero",
                QuizId = 2,
            });
            Assert.NotEqual(dbServices.GetQuizHistoryByQuizId(2).Count(), dbServices.GetAllQuizHistory().Count());

        }
    }
}
