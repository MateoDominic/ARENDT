using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Models;
using WebApi.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PraContext _praContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IDbService _dbService;
        public UserController(PraContext praContext, IMapper mapper, IConfiguration configuration, IDbService dbService)
        {
            _praContext = praContext;
            _mapper = mapper;
            _configuration = configuration;
            _dbService = dbService;
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public ActionResult<UserDTO> Get(int id)
        {
            try
            {
                UserDTO user = _mapper.Map<UserDTO>(_praContext.Users.FirstOrDefault(x => x.Id == id));
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<UserController>
        [HttpPost("register")]
        public ActionResult<UserRegisterDTO> RegisterUser([FromBody] UserRegisterDTO value)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var newUser = _mapper.Map<User>(value);

                newUser.PasswordSalt = AuthUtilities.GetSalt();
                newUser.PasswordHash = AuthUtilities.GetStringSha256Hash(value.Password, newUser.PasswordSalt);
                newUser.JoinDate = DateTime.Now;

                _praContext.Users.Add(newUser);
                _praContext.SaveChanges();
                value.Id = newUser.Id;
                return Ok(value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] UserLoginDTO value)
        {
            try
            {
                var user = _praContext.Users.FirstOrDefault(x => x.Username == value.Username);

                string v = AuthUtilities.GetStringSha256Hash(value.Password, user.PasswordSalt);
                if (user == null || v != user.PasswordHash)
                {
                    return BadRequest("Bad username or password");
                }
                var secureKey = _configuration["JWT:SecureKey"];

                var serializedToken =
                    JwtTokenProvider.CreateToken(
                        secureKey,
                        120,
                        value.Username);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public ActionResult<UserDTO> Put(int id, [FromBody] UserDTO value)
        {
            try
            {
                
                var existingUser = _praContext.Users.FirstOrDefault(x => x.Username == value.Username);
                if (existingUser == null) {
                    return NotFound();
                }
                var updatedUser = _mapper.Map<User>(value);
                updatedUser.JoinDate = existingUser.JoinDate;
                updatedUser.PasswordHash = existingUser.PasswordHash;
                updatedUser.PasswordSalt = existingUser.PasswordSalt;
                _praContext.Entry(existingUser).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                _praContext.Users.Update(updatedUser);                
                _praContext.SaveChanges();
                return Ok(value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("changePassword")]
        public ActionResult<UserChangePasswordDTO> Put([FromBody] UserChangePasswordDTO value)
        {
            try
            {
                var existingUser = _praContext.Users.FirstOrDefault(x => x.Username == value.Username);
                if (existingUser == null || AuthUtilities.GetStringSha256Hash(value.OldPassword, existingUser.PasswordSalt) != existingUser.PasswordHash)
                {
                    return BadRequest();
                }

                existingUser.PasswordSalt = AuthUtilities.GetSalt();
                existingUser.PasswordHash = AuthUtilities.GetStringSha256Hash(value.NewPassword, existingUser.PasswordSalt);
                
                _praContext.Users.Update(existingUser);
                _praContext.SaveChanges();
                return Ok(value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public ActionResult<UserDTO> Delete(int id)
        {
            try
            {
                User user = _praContext.Users.FirstOrDefault(x => x.Id == id);

                if (user == null)
                {
                    return NotFound();
                }

                UserDTO userDto = _mapper.Map<UserDTO>(user);
                _praContext.Users.Remove(user);
                _praContext.SaveChanges();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
