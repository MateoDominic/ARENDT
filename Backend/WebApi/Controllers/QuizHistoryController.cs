using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Models;
using WebApi.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizHistoryController : ControllerBase
    {
        private readonly IDbService _dbService;
        private readonly IMapper _mapper;
        public QuizHistoryController(IMapper mapper, IDbService dbService)
        {
            _dbService = dbService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<QuizHistoryDTO>> GetHistory()
        {
            try
            {
                return Ok(_dbService.GetAllQuizHistory());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/<QuizHistoryController>
        [HttpGet("author/{AuthorId}")]
        public ActionResult<IEnumerable<QuizHistoryDTO>> GetHistoryByAuthorId(int AuthorId)
        {
            try
            {
                return Ok(_dbService.GetQuizHistoryByAuthorId(AuthorId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<QuizHistoryController>/5
        [HttpGet("{QuizId}")]
        public ActionResult<IEnumerable<QuizHistoryDTO>> GetHistoryByQuizId(int QuizId)
        {
            try
            {
                return Ok(_dbService.GetQuizHistoryByQuizId(QuizId));
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
                _dbService.AddQuizHistory(value);
                return value;
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
                QuizHistoryDTO? quizHistory = _dbService.DeleteQuizHistory(id);
                if (quizHistory == null) {
                    return NotFound();
                }

                return Ok(quizHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
