using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
                return Ok(_dbService.GetAllQuizzes());
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
                return Ok(_dbService.GetQuizzesByAuthorId(AuthorId));
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
                var quiz = _dbService.GetQuizById(QuizId);
                if (quiz == null)
                {
                    return NotFound();
                }
                return Ok(quiz);
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
                if (quizDto.Questions.Count() > 20 || quizDto.Questions.Count() < 1)
                {
                    return BadRequest("Incorrect number of questions. A quiz has to have at least 1 and more that 20 questions.");
                }
                foreach (var question in quizDto.Questions)
                {
                    if (question.Answers.Count() != 4)
                    {
                        return BadRequest($"Every question has to have 4 answers. The question in position {question.QuestionPosition} does not.");
                    }
                    if (question.QuestionTime < 15 || question.QuestionTime>60)
                    {
                        return BadRequest($"Every question has to last from 15 to 60 seconds. The question in position {question.QuestionPosition} lasts {question.QuestionTime} seconds.");
                    }
                    if (question.Answers.Count(x => x.Correct) != 1)
                    {
                        return BadRequest($"Question {question.QuestionPosition} has to have 1 correct answer");
                    }
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
        [HttpPut]
        public ActionResult<FullQuizDTO> Put([FromBody] FullQuizDTO quizDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (quizDto.Questions.Count() > 20 || quizDto.Questions.Count() < 1)
                {
                    return BadRequest("Incorrect number of questions. A quiz has to have at least 1 and more that 20 questions.");
                }
                foreach (var question in quizDto.Questions)
                {
                    if (question.Answers.Count() != 4)
                    {
                        return BadRequest($"Every question has to have 4 answers. The question in position {question.QuestionPosition} does not.");
                    }
                    if (question.QuestionTime < 15 || question.QuestionTime > 60)
                    {
                        return BadRequest($"Every question has to last from 15 to 60 seconds. The question in position {question.QuestionPosition} lasts {question.QuestionTime} seconds.");
                    }
                    if (question.Answers.Count(x => x.Correct) != 1)
                    {
                        return BadRequest($"Question {question.QuestionPosition} has to have 1 correct answer");
                    }
                }

                var updatedQuiz = _dbService.UpdateFullQuiz(quizDto);
                if (updatedQuiz == null)
                {
                    return NotFound();
                }
                
                return Ok(updatedQuiz);
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
                var quizDto = _dbService.DeleteQuiz(id);
                if (quizDto == null)
                {
                    return NotFound();
                }
                return Ok(quizDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
