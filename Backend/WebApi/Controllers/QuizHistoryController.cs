using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using WebApi.DTOs;
using WebApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizHistoryController : ControllerBase
    {
        private readonly PraContext _praContext;
        private readonly IMapper _mapper;
        public QuizHistoryController(PraContext praContext, IMapper mapper)
        {
            _praContext = praContext;
            _mapper = mapper;
        }

        // GET: api/<QuizHistoryController>
        [HttpGet("author/{AuthorId}")]
        public ActionResult<IEnumerable<QuizHistoryDTO>> GetHistoryByAuthorId(int AuthorId)
        {
            try
            {
                IEnumerable<QuizHistory> quizzes = _praContext.QuizHistories.Include("Quiz").Where(x => x.Quiz.AuthorId == AuthorId);
                if (quizzes.Count() == 0)
                {
                    return NotFound();
                }
                IEnumerable<QuizHistoryDTO> quizDtos = _mapper.Map<IEnumerable<QuizHistoryDTO>>(quizzes);
                return Ok(quizDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<QuizHistoryController>/5
        [HttpGet("{QuizId}")]
        public ActionResult<QuizHistoryDTO> GetHistoryByQuizId(int QuizId)
        {
            try
            {
                IEnumerable<QuizHistory> quizzes = _praContext.QuizHistories.Where(x => x.QuizId == QuizId);
                if (quizzes.Count() == 0)
                {
                    return NotFound();
                }
                IEnumerable<QuizHistoryDTO> quizDtos = _mapper.Map<IEnumerable<QuizHistoryDTO>>(quizzes);
                return Ok(quizDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<QuizHistoryController>
        [HttpPost]
        public ActionResult<QuizHistoryDTO> Post([FromBody] QuizHistoryDTO value)
        {
            try
            {
                QuizHistory quizHistory = _mapper.Map<QuizHistory>(value);
                quizHistory.PlayedAt = DateTime.Now;

                _praContext.QuizHistories.Add(quizHistory);

                _praContext.SaveChanges();

                value.Id = quizHistory.Id;
                return Ok(value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<QuizHistoryController>/5
        [HttpPut("{id}")]
        public ActionResult<QuizHistoryDTO> Put(int id, [FromBody] QuizHistoryDTO value)
        {
            try
            {
                /*QuizHistory quiz = _praContext.QuizHistories.FirstOrDefault(x => x.Id == id);
                if (quiz == null)
                {
                    return NotFound();
                }*/
                if (id != value.Id)
                {
                    return BadRequest();
                }
                _praContext.QuizHistories.Update(_mapper.Map<QuizHistory>(value));
                _praContext.SaveChanges();
                return Ok(value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<QuizHistoryController>/5
        [HttpDelete("{id}")]
        public ActionResult<QuizHistoryDTO> Delete(int id)
        {
            try
            {
                QuizHistory? quizHistory = _praContext.QuizHistories.FirstOrDefault(x => x.Id == id);
                if (quizHistory == null) {
                    return NotFound();
                }

                _praContext.QuizHistories.Remove(quizHistory);
                _praContext.SaveChanges();
                return Ok(_mapper.Map<QuizHistoryDTO>(quizHistory));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
