namespace PraTest
{
    public class QuizTest
    {
        private readonly DbServices dbServices;
        //private readonly IDbService _dbService;
        public QuizTest()
        {
            dbServices = RepositoryMock.GetQuizMock();
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
        private FullQuizDTO defaultQuiz2 = new FullQuizDTO()
        {
            QuizId = 2,
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
        public void Quiz_Delete_Success()
        {
            var addedQuiz = dbServices.AddFullQuiz(defaultQuiz1);
            if (addedQuiz != null)
            {
                var deletedQuiz = dbServices.DeleteQuiz(addedQuiz.QuizId);
                Assert.NotNull(deletedQuiz);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Fact]
        public void Quiz_Delete_Fail()
        {
            var deletedQuiz = dbServices.DeleteQuiz(10);
            Assert.Null(deletedQuiz);
            
        }

        [Fact]
        public void Quiz_Get_Success()
        {
            var addedQuiz = dbServices.AddFullQuiz(defaultQuiz1);
            if (addedQuiz != null)
                Assert.NotNull(dbServices.GetQuizById(addedQuiz.QuizId));
            else
                Assert.Fail(); 
        }

        [Fact]
        public void Quiz_Get_Fail()
        {
            Assert.Null(dbServices.GetQuizById(10));
        }

        [Fact]
        public void Quizzes_Get_Success()
        {
            var addedQuiz = dbServices.AddFullQuiz(defaultQuiz1);
            var addedQuiz2 = dbServices.AddFullQuiz(defaultQuiz1);
            if (addedQuiz != null && addedQuiz2 != null)
                Assert.Equal(2, dbServices.GetAllQuizzes().Count());
            else
                Assert.Fail();
        }

        [Fact]
        private void Question_Get_Success()
        {
            var addedQuestion = dbServices.AddFullQuiz(defaultQuiz1);
            var question = dbServices.GetQuizQuestion(addedQuestion.QuizId, 1);
            Assert.NotNull(question);
        }

        [Fact]
        private void Question_Get_Fail()
        {
            var addedQuestion = dbServices.AddFullQuiz(defaultQuiz1);
            var question = dbServices.GetQuizQuestion(10, 1);
            Assert.Null(question);
        }

        [Fact]
        public void Quiz_Add_Fail_NoQuestions()
        {
            var addedQuiz = dbServices.AddFullQuiz(new FullQuizDTO()
            {
                AuthorId = 1,
                Title = "TestNoQuestionFail",
                Description = "Test",
                Questions = new QuestionDTO[]
                            {}
            });
            Assert.Null(addedQuiz);
        }

        [Fact]
        public void Quiz_Update_Success()
        {
            dbServices.AddFullQuiz(defaultQuiz1);
            var differentQuiz = defaultQuiz1;
            differentQuiz.Questions.ElementAt(0).QuestionText = "New text";
            differentQuiz.Description = "DiferentDescription";
            dbServices.UpdateFullQuiz(differentQuiz);
            var quizzes = dbServices.GetAllQuizzes();
            var updatedQuiz = dbServices.GetQuizById(defaultQuiz1.QuizId);
            Assert.True(quizzes.Count() == 1 && updatedQuiz.Description == differentQuiz.Description);
        }

        [Fact]
        public void Quiz_Add_Success()
        {
            var addedQuiz = dbServices.AddFullQuiz(defaultQuiz1);
            Assert.NotNull(addedQuiz);
        }

        [Fact]
        public void Quiz_Add_Fail_Time()
        {
            var inccorectQuiz = defaultQuiz1;
            inccorectQuiz.Questions.ElementAt(0).QuestionTime = 0;
            var addedQuiz = dbServices.AddFullQuiz(inccorectQuiz);
            Assert.Null(addedQuiz);
        }


        [Fact]
        public void Quiz_GetByAuthorId_Success()
        {
            var quiz1 = defaultQuiz1;
            var quiz2 = defaultQuiz2;
            dbServices.AddFullQuiz(quiz1);
            dbServices.AddFullQuiz(quiz2);
            IEnumerable<QuizDTO> result = dbServices.GetQuizzesByAuthorId(quiz1.AuthorId);
            Assert.Equal(2, result.Count());
        }
    }
}
