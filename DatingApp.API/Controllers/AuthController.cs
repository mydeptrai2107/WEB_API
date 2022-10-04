using System.Security.Cryptography;
using System.Text;
using DatingApp.API.Data;
using DatingApp.API.Data.Entities;
using DatingApp.API.DTOs;
using DatingApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    public class AuthController : BaseController
    {      
        private readonly DataContext _context;

        private readonly ITokenService _tokenService;

        public AuthController(DataContext dataContext, ITokenService tokenService)
        {
            _context = dataContext;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthUserDto authUserDto)
        {
            authUserDto.Username = authUserDto.Username.ToLower();
            if(_context.AppUsers.Any(u => u.Username == authUserDto.Username)){
                return BadRequest("Usernam is already registered!");
            }

            using var hmac = new HMACSHA512();
            var passWordBytes  = Encoding.UTF8.GetBytes(authUserDto.PassWord);
            var newUser = new User{
                Username = authUserDto.Username,
                PasswordSalt = hmac.Key,
                PasswordHashed = hmac.ComputeHash(passWordBytes)
            };
            _context.AppUsers.Add(newUser);
            _context.SaveChanges();

            var token = _tokenService.CreateToken(newUser.Username);
            return Ok(token);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthUserDto authUserDto)
        {
            authUserDto.Username = authUserDto.Username.ToLower();
            var currentUser = _context.AppUsers.FirstOrDefault(u => u.Username == authUserDto.Username);

            if(currentUser == null){
                return Unauthorized("Username is invalid.");
            }
            
            using var hmac = new HMACSHA512(currentUser.PasswordSalt);
            var passWordBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(authUserDto.PassWord));
            for(int i = 0; i < currentUser.PasswordHashed.Length; i++){
                if(currentUser.PasswordHashed[i] != passWordBytes[i]){
                    return Unauthorized("Password is invalid");
                }
            }

            var token = _tokenService.CreateToken(currentUser.Username);
            return Ok(token);
        }
        
        [Authorize]
        [HttpGet]
        public IActionResult Get(){
            return Ok(_context.AppUsers.ToList());
        }
    }
}