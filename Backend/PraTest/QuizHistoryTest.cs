using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PraTest
{
    public class QuizHistoryTest
    {
        private readonly DbServices dbServicesHistory;
        //private readonly IDbService _dbService;
        public QuizHistoryTest()
        {
            dbServicesHistory = RepositoryMock.GetQuizHistoryMock();
        }

        private FullQuizDTO defaultQuiz1 = new FullQuizDTO()
        {
            QuizId = 1,
            AuthorId = 1,
            Title = "Test",
            Description = "Test",
            Questions = new QuestionDTO[]
                {
                   new QuestionDTO(){
                        QuestionMaxPoints = 10,
                        QuestionPosition = 1,
                        QuestionText = "Test",
                        QuestionTime = 30,
                        Answers = new AnswerDTO[]
                        {
                            new AnswerDTO(){
                                AnswerText = "Test1",
                                Correct = false
                            },
                            new AnswerDTO(){
                                AnswerText = "Test2",
                                Correct = false
                            },
                            new AnswerDTO(){
                                AnswerText = "Test3",
                                Correct = true
                            },
                            new AnswerDTO(){
                                AnswerText = "Test4",
                                Correct = false
                            },
                        }
                   }
                }
        };

        [Fact]
        private void QuizHistory_Delete_Success()
        {
            int startingCount = dbServicesHistory.GetAllQuizHistory().Count();
            var deletedQuizHistory = dbServicesHistory.DeleteQuizHistory(dbServicesHistory.GetAllQuizHistory().First().Id);
            int afterCount = dbServicesHistory.GetAllQuizHistory().Count();
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
            int startingCount = dbServicesHistory.GetAllQuizHistory().Count();
            dbServicesHistory.AddQuizHistory(new QuizHistoryDTO() { 
                PlayedAt = DateTime.Now,
                WinnerName = "Pero",
                QuizId = 1,
            });
            int afterCount = dbServicesHistory.GetAllQuizHistory().Count();
            Assert.Equal(startingCount + 1, afterCount);
        }

        [Fact]
        private void QuizHistory_GetByQuizId_Success()
        {
            dbServicesHistory.AddQuizHistory(new QuizHistoryDTO()
            {
                PlayedAt = DateTime.Now,
                WinnerName = "Pero",
                QuizId = 2,
            });
            Assert.NotEqual(dbServicesHistory.GetQuizHistoryByQuizId(2).Count(), dbServicesHistory.GetAllQuizHistory().Count());

        }
    }
}
