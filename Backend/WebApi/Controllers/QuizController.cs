using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebApi.DTOs;
using WebApi.Models;
using WebApi.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly PraContext _praContext;
        private readonly IMapper _mapper;
        private readonly IDbService _dbService;
        public QuizController(PraContext praContext, IMapper mapper, IDbService dbService)
        {
            _praContext = praContext;
            _mapper = mapper;
            _dbService = dbService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<FullQuizDTO>> GetAllQuizzes()
        {
            try
            {
                IEnumerable<Quiz> quizzes = _praContext.Quizzes;
                List<QuizDTO> quizDTOs = new List<QuizDTO>();
                foreach (var quiz in quizzes)
                {
                    quizDTOs.Add(_mapper.Map<QuizDTO>(quiz));
                }
                return Ok(quizDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/<QuizController>
        [HttpGet("details/{AuthorId}")]
        public ActionResult<IEnumerable<QuizDTO>> GetQuizzesByAuthorID(int AuthorId)
        {
            try
            {
                IEnumerable<Quiz> quizzes = _praContext.Quizzes.Where(x => x.AuthorId == AuthorId);
                IEnumerable<QuizDTO> quizDtos= _mapper.Map<IEnumerable<QuizDTO>>(quizzes);
                return Ok(quizDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{QuizId}")]
        public ActionResult<FullQuizDTO> GetQuizByQuizID(int QuizId)
        {
            try
            {
                Quiz quiz = _praContext.Quizzes.Include("Questions").Include("Questions.Answers").FirstOrDefault(x => x.Id == QuizId);
                
                if (quiz == null)
                {
                    return NotFound();
                }
                
                FullQuizDTO quizDto = _mapper.Map<FullQuizDTO>(quiz);
                return Ok(quizDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<QuizController>
        [HttpPost]
        public ActionResult<FullQuizDTO> Post([FromBody] FullQuizDTO quizDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var postedQuiz = _dbService.AddFullQuiz(quizDto);

                return Ok(postedQuiz);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<QuizController>/5
        [HttpPut("{id}")]
        public ActionResult<FullQuizDTO> Put(int id, [FromBody] FullQuizDTO quizDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                
                return Ok(_dbService.UpdateFullQuiz(quizDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<QuizController>/5
        [HttpDelete("{id}")]
        public ActionResult<QuizDTO> Delete(int id)
        {
            try
            {
                var existingQuiz = _praContext.Quizzes.FirstOrDefault(x => x.Id == id);
                
                if (existingQuiz == null)
                {
                    return NotFound();
                }
                IEnumerable<QuizHistory> quizHistories = _praContext.QuizHistories.Where(x => x.QuizId == id);
                foreach (var quizHistory in quizHistories)
                {
                    _praContext.QuizHistories.Remove(quizHistory);
                }
                var deletedQuizDto = _mapper.Map<QuizDTO>(existingQuiz);
                _praContext.Quizzes.Remove(existingQuiz);
                _praContext.SaveChanges();

                return Ok(deletedQuizDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
