using BLL.DTO;
using BLL.DTO.Requests;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IIdentity
    {
        public Task<AuthentificationResult> Register(UserRegistrationDto user);
        public Task<AuthentificationResult> Login(UserLoginDto user);
        public Task<string> GenerateJwtToken(ApplicationUser user);
        public Task<ServiceResult> CreateRole(string name);
        public Task<List<ApplicationUser>> GetAllUsers();
        public Task<ServiceResult> AddUserToRole(string email, string roleName);
        public Task<ServiceResult> AddClaimsToUser(string email, string claimName, string claimValue);
    }
}
