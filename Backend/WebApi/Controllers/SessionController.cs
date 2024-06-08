using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly PraContext _praContext;
        public SessionController(PraContext praContext)
        {
            _praContext = praContext;
        }
        Random generator = new Random();

        [HttpGet("{quizId}")]
        public ActionResult<int> GetFreeSessionCode(int quizId)
        {
            try
            {
                IEnumerable<SessionCode> activeCodes = _praContext.SessionCodes;
                string newCode = generator.Next(0, 1000000).ToString("D6");
                while (activeCodes.Any(x => x.Code == newCode))
                {
                    newCode = generator.Next(0, 1000000).ToString("D6");
                }

                SessionCode code = new SessionCode() { 
                    Code = newCode,
                    QuizId = quizId
                };

                _praContext.SessionCodes.Add(code);

                return Ok(newCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{code}")]
        public ActionResult<int> DeleteSessionCode(int code) {
            try
            {
                var session = _praContext.SessionCodes.FirstOrDefault(x => x.Code == code.ToString());
            
                if (session == null) {
                    return NotFound();
                }

                _praContext.SessionCodes.Remove(session);
                _praContext.SaveChanges();

                return Ok(code);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
