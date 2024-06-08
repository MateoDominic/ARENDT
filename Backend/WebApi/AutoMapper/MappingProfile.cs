using AutoMapper;
using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Quiz, QuizDTO>().ForMember(dest => dest.QuizId, opt => opt.MapFrom(src => src.Id));
            CreateMap<QuizDTO, Quiz>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.QuizId));

            CreateMap<QuestionDTO, Question>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.QuestionId));
            CreateMap<Question, QuestionDTO>().ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Answer, AnswerDTO>().ForMember(dest => dest.AnswerId, opt => opt.MapFrom(src => src.Id));
            CreateMap<AnswerDTO, Answer>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AnswerId));

            CreateMap<Quiz, FullQuizDTO>().ForMember(dest => dest.QuizId, opt => opt.MapFrom(src => src.Id));
            CreateMap<FullQuizDTO, Quiz>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.QuizId));

            CreateMap<User, UserLoginDTO>();
            CreateMap<UserLoginDTO, User>();
            
            CreateMap<User, UserRegisterDTO>();
            CreateMap<UserRegisterDTO, User>();
            
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();

            CreateMap<QuizHistory, QuizHistoryDTO>();
            CreateMap<QuizHistoryDTO, QuizHistory>();
        }
    }
}
