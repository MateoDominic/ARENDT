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
    }
}
