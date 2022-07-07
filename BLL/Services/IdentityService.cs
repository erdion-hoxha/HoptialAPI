using BLL.DTO;
using BLL.DTO.Requests;
using DAL.Models;
using HosptialAPI.BLL.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class IdentityService : IIdentity
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtConfig _jwtSettings;
        private DatabaseContext _context;


        public IdentityService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IOptionsMonitor<JwtConfig> optionMonitor,
            DatabaseContext context)
        {
            _userManager = userManager;
            _jwtSettings = optionMonitor.CurrentValue;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<AuthentificationResult> Register(UserRegistrationDto user)
        {

            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser != null)
            {
                return new AuthentificationResult
                {
                    Success = false,
                    ErrorMessage = new List<string>() {
                        "Emaili eshte i zene"
                    }
                };
            }
            else
            {
                var newUser = new ApplicationUser()
                {
                    Email = user.Email,
                    Birthday = user.Birthday,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.FirstName + user.LastName,
                    PhoneNumber = user.PhoneNumber
                };
                var result = await _userManager.CreateAsync(newUser, user.Password);
                if (result.Succeeded)
                {
                    var jwtToken = await GenerateJwtToken(newUser);
                    return new AuthentificationResult
                    {
                        Success = true,
                        Token = jwtToken
                    };
                }
                return new AuthentificationResult
                {
                    Success = false,
                    ErrorMessage = result.Errors.Select(x => x.Description).ToList()
                };
            }
        }

        public async Task<AuthentificationResult> Login(UserLoginDto user)
        {

            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser == null)
            {
                return new AuthentificationResult
                {
                    Success = false,
                    ErrorMessage = new List<string>() {
                        "Emaili ose passwordi eshte jo i sakte"
                    }
                };
            }
            var result = await _userManager.CheckPasswordAsync(existingUser, user.Password);
            if (result)
            {
                var jwtToken = await GenerateJwtToken(existingUser);
                return new AuthentificationResult
                {
                    Success = true,
                    Token = jwtToken
                };
            }
            return new AuthentificationResult
            {
                Success = false,
                ErrorMessage = new List<string>() {
                        "Invalid login request"
                    }
            };

        }
        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>() {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //var roles = await _userManager.GetRolesAsync(user);

            //if (roles != null)
            //{
            //    claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));
            //}
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Get the user role and add it to the claims
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new[] {
                //        new Claim("Id", user.Id),
                //        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                //        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                //    }),
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            //_context.re

            return jwtToken;

        }
        public async Task<ServiceResult> CreateRole(string name)
        {

            var roleExist = await _roleManager.RoleExistsAsync(name);

            if (!roleExist) // checks on the role exist status
            {
                var roleResult = await _roleManager.CreateAsync(new ApplicationRole(name));

                // We need to check if the role has been added successfully
                if (roleResult.Succeeded)
                {
                    //_logger.LogInformation($"The Role {name} has been added successfully");
                    return new ServiceResult()
                    {
                        Result = true
                    };
                }
                else
                {
                    return new ServiceResult()
                    {
                        Result = false,
                        ErrorMessage = new List<string>()
                        {
                            "Roli nuk u krijua me sukses"
                        }
                    };
                }
            }
            return new ServiceResult()
            {
                Result = false,
                ErrorMessage = new List<string>()
                        {
                            "Roli nuk ekziston"
                        }
            };
        }

        public async Task<List<ApplicationUser>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }

        public async Task<ServiceResult> AddUserToRole(string email, string roleName)
        {
            // Check if the user exist
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) // User does not exist
            {
                //_logger.LogInformation($"The user with the {email} does not exist");
                return new ServiceResult()
                {
                    Result = false,
                    ErrorMessage = new List<string>()
                        {
                            "Useri nuk ekziston"
                        }
                };
            }

            // Check if the role exist
            // Check if the role exist
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist) // checks on the role exist status
            {
                //_logger.LogInformation($"The role {email} does not exist");
                return new ServiceResult()
                {
                    Result = false,
                    ErrorMessage = new List<string>()
                        {
                            "Roli nuk ekziston"
                        }
                };
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            // Check if the user is assigned to the role successfully
            if (result.Succeeded)
            {
                return new ServiceResult()
                {
                    Result = true,
                };
            }
            else
            {
                //_logger.LogInformation($"The user was not abel to be added to the role");
                return new ServiceResult()
                {
                    Result = false,
                    ErrorMessage = new List<string>()
                        {
                            "Useri nuk u fut dot ne rolin e kerkuar"
                        }
                };
            }
        }
        public async Task<Tuple<bool, IList<string>>> GetUserRoles(string email)
        {
            // check if the email is valid
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) // User does not exist
            {
                //_logger.LogInformation($"The user with the {email} does not exist");
                return new Tuple<bool, IList<string>>(false, null); ;
            }

            // return the roles
            var roles = await _userManager.GetRolesAsync(user);

            return new Tuple<bool, IList<string>>(true, roles);
        }


        public async Task<ServiceResult> RemoveUserFromRole(string email, string roleName)
        {
            // Check if the user exist
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) // User does not exist
            {
                //_logger.LogInformation($"The user with the {email} does not exist");
                return new ServiceResult()
                {
                    Result = false,
                    ErrorMessage = new List<string>()
                        {
                            "Useri nuk ekzsiton"
                        }
                };
            }

            // Check if the role exist
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExist) // checks on the role exist status
            {
                //_logger.LogInformation($"The role {email} does not exist");
                return new ServiceResult()
                {
                    Result = false,
                    ErrorMessage = new List<string>()
                        {
                            "Roli nuk ekzsiton"
                        }
                };
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return new ServiceResult()
                {
                    Result = true,
                };
            }

            return new ServiceResult()
            {
                Result = false,
                ErrorMessage = new List<string>()
                        {
                            "Useri nuk u fshi dot nga roli"
                        }
            };
        }

        public async Task<ServiceResult> AddClaimsToUser(string email, string claimName, string claimValue)
        {
            // Check if the user exist
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) // User does not exist
            {
                //_logger.LogInformation($"The user with the {email} does not exist");
                return new ServiceResult()
                {
                    Result = false,
                    ErrorMessage = new List<string>()
                        {
                            "Useri nuk ekziston"
                        }
                };
            }

            var userClaim = new Claim(claimName, claimValue);

            var result = await _userManager.AddClaimAsync(user, userClaim);

            if (result.Succeeded)
            {
                return new ServiceResult()
                {
                    Result = true,
                };
            }

            return new ServiceResult()
            {
                Result = false,
                ErrorMessage = new List<string>()
                        {
                            "Claimi nuk u fut dot tek useri"
                        }
            };

        }
    }
}
