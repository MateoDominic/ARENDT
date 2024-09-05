﻿using AutoMapper;
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
        private readonly IConfiguration _configuration;
        private readonly IDbService _dbService;
        public UserController(IConfiguration configuration, IDbService dbService)
        {
            _configuration = configuration;
            _dbService = dbService;
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<UserDTO> Get(int id)
        {
            try
            {
                UserDTO? user = _dbService.GetUser(id);
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
                int newId = _dbService.RegisterUser(value);
                value.Id = newId;
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
                if (!_dbService.LoginUser(value))
                {
                    return BadRequest("Bad username or password");
                }
                var user = _dbService.GetUserByUsername(value.Username);
                if (user == null)
                {
                    return StatusCode(500, "Something went wrong");
                }
                var secureKey = _configuration["JWT:SecureKey"];
                if (secureKey == null)
                {
                    return StatusCode(500);
                }
                var serializedToken =
                    JwtTokenProvider.CreateToken(
                        secureKey,
                        120,
                        user.Username,
                        user.Id);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<UserController>/5
        [HttpPut]
        [Authorize]
        public ActionResult<UserDTO> UpdateUserInfo([FromBody] UserDTO value)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!_dbService.UpdateUserInfo(value))
                {
                    return NotFound();
                }
                return Ok(value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("changePassword")]
        [Authorize]
        public ActionResult<UserChangePasswordDTO> UpdateUserPassword([FromBody] UserChangePasswordDTO value)
        {
            try
            {
                if (!_dbService.UpdateUserPassword(value))
                {
                    return BadRequest("Bad username or password");
                }
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
                var user = _dbService.DeleteUser(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
