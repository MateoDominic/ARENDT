using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Utilities
{
    public interface IDbService {
        public FullQuizDTO AddFullQuiz(FullQuizDTO fullQuizDTO);
        
        public FullQuizDTO UpdateFullQuiz(FullQuizDTO fullQuizDTO);
        public QuestionDTO GetQuizQuestion(int quizId, int questionNumber);
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
            _praContext.Quizzes.Add(_mapper.Map<Quiz>(fullQuizDTO));
            _praContext.SaveChanges();
            
            return fullQuizDTO;
        }

        public FullQuizDTO UpdateFullQuiz(FullQuizDTO fullQuizDTO)
        {
            _praContext.Quizzes.Update(_mapper.Map<Quiz>(fullQuizDTO));
            _praContext.SaveChanges();
            return fullQuizDTO;
        }

        public QuestionDTO GetQuizQuestion(int quizId, int questionNumber) {
            Console.WriteLine("NES PLZ2");
            Quiz? quiz = _praContext.Quizzes.Include("Questions").Include("Questions.Answers").FirstOrDefault(x => x.Id == quizId);
            if (quiz == null)
            {
                Console.WriteLine("OVO JE NULL");
            }
            List<Question> questions = quiz.Questions.OrderBy(x => x.QuestionPosition).ToList();
            return _mapper.Map<QuestionDTO>(questions.ElementAt(questionNumber-1));
        }
    }
}
