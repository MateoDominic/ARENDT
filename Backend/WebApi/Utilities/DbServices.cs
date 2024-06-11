using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Utilities
{
    public interface IDbService {
        public FullQuizDTO AddFullQuiz(FullQuizDTO fullQuizDTO);
        public FullQuizDTO? UpdateFullQuiz(FullQuizDTO fullQuizDTO);
        public QuestionDTO? GetQuizQuestion(int quizId, int questionNumber);
        public IEnumerable<QuizDTO> GetAllQuizzes();
        public IEnumerable<QuizDTO> GetQuizzesByAuthorId(int AuthorId);
        public FullQuizDTO? GetQuizById(int QuizId);
        public QuizDTO? DeleteQuiz(int QuizId);
    }

    public class DbServices : IDbService
    {
        PraContext _praContext;
        IMapper _mapper;
        public DbServices(PraContext praContext, IMapper mapper) { 
            _praContext = praContext;
            _mapper = mapper;
        }

        public FullQuizDTO AddFullQuiz(FullQuizDTO fullQuizDTO)
        {
            var fullQuiz = _mapper.Map<Quiz>(fullQuizDTO);
            _praContext.Quizzes.Add(fullQuiz);
            _praContext.SaveChanges();

            fullQuizDTO.QuizId = fullQuiz.Id;
            return fullQuizDTO;
        }

        public FullQuizDTO? UpdateFullQuiz(FullQuizDTO fullQuizDTO)
        {
            if (_praContext.Quizzes.FirstOrDefault(x => x.Id == fullQuizDTO.QuizId) == null)
            {
                return null;
            }

            _praContext.Quizzes.Update(_mapper.Map<Quiz>(fullQuizDTO));
            _praContext.SaveChanges();
            return fullQuizDTO;
        }

        public QuestionDTO? GetQuizQuestion(int quizId, int questionNumber) {
            Quiz? quiz = _praContext.Quizzes.Include("Questions").Include("Questions.Answers").FirstOrDefault(x => x.Id == quizId);
            if (quiz == null)
            {
                return null;
            }
            List<Question> questions = quiz.Questions.OrderBy(x => x.QuestionPosition).ToList();
            return _mapper.Map<QuestionDTO>(questions.ElementAt(questionNumber-1));
        }

        public IEnumerable<QuizDTO> GetAllQuizzes() {
            IEnumerable<Quiz> quizzes = _praContext.Quizzes;
            List<QuizDTO> quizDTOs = new List<QuizDTO>();
            foreach (var quiz in quizzes)
            {
                quizDTOs.Add(_mapper.Map<QuizDTO>(quiz));
            }
            return quizDTOs;
        }

        public IEnumerable<QuizDTO> GetQuizzesByAuthorId(int AuthorId)
        {
            IEnumerable<Quiz> quizzes = _praContext.Quizzes.Where(x => x.AuthorId == AuthorId);
            IEnumerable<QuizDTO> quizDTOs = _mapper.Map<IEnumerable<QuizDTO>>(quizzes);
            return quizDTOs;
        }

        public FullQuizDTO? GetQuizById(int QuizId) {
            Quiz? quiz = _praContext.Quizzes.Include("Questions").Include("Questions.Answers").FirstOrDefault(x => x.Id == QuizId);

            if (quiz == null)
            {
                return null;
            }

            return _mapper.Map<FullQuizDTO>(quiz);
            
        }

        public QuizDTO? DeleteQuiz(int QuizId) {
            var existingQuiz = _praContext.Quizzes.FirstOrDefault(x => x.Id == QuizId);

            if (existingQuiz == null)
            {
                return null;
            }
            IEnumerable<QuizHistory> quizHistories = _praContext.QuizHistories.Where(x => x.QuizId == QuizId);
            foreach (var quizHistory in quizHistories)
            {
                _praContext.QuizHistories.Remove(quizHistory);
            }
            var deletedQuizDto = _mapper.Map<QuizDTO>(existingQuiz);
            _praContext.Quizzes.Remove(existingQuiz);
            _praContext.SaveChanges();
            return deletedQuizDto;
        }
    }
}
