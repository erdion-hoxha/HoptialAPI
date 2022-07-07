using AutoMapper;
using BLL.DTO.Requests;
using BLL.Services;
using DAL.Models;
using HosptialAPI.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HosptialAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IIdentity _identityService;

        private readonly ILogger<AccountController> _logger;

        //private readonly IMapper _mapper;

        public AccountController(
            ILogger<AccountController> logger,
            //IMapper mapper,
            IIdentity identity)
        {
            _logger = logger;
            //_mapper = mapper;
            _identityService = identity;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto User)
        {

            if (ModelState.IsValid)
            {
                var result = await _identityService.Register(User);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest("Ka gabime ne forme");
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto User)
        {

            if (ModelState.IsValid)
            {
                var result = await _identityService.Login(User);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest("Ka gabime ne forme");
            }
        }
        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {

            var result = await _identityService.GetAllUsers();
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return Ok("Nuk ka usera aktive");
            }
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto Role)
        {

            if (ModelState.IsValid)
            {
                var result = await _identityService.CreateRole(Role.Role);
                if (result.Result)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest("Ka gabime ne forme");
            }
        }

        //beje post me model me vone
        [HttpGet]
        [Route("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email,string role)
        {

            if (email != null && role != null)
            {
                var result = await _identityService.AddUserToRole(email,role);
                if (result.Result)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest("Ka gabime ne te dhenat");
            }
        }
        [HttpGet]
        [Route("AddClaimToUser")]
        public async Task<IActionResult> AddClaimToUser(string email, string claimName, string claimValue)
        {

            if (email != null && claimName != null && claimValue != null)
            {
                var result = await _identityService.AddClaimsToUser(email, claimName, claimValue);
                if (result.Result)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest("Ka gabime ne te dhenat");
            }
        }
    }
}
