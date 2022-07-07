using DAL.Interfaces;
using DAL.Models;
using HosptialAPI.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    class AuthentificationRepository : IAuthentificationRepository
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly DatabaseContext _context;

        public AuthentificationRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,DatabaseContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
  

        //public async Task<string> Login(UserDto user)
        //{
        //}

        public async Task<string> Register(UserDto User)
        {

            try
            {
                var user = new ApplicationUser()
                {
                    Birthday = User.Birthday,
                    Email = User.Email,
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    CreatedOn = DateTime.Now,
                    UserName = User.FirstName + " " + User.LastName
                };
                var result = await _userManager.CreateAsync(user, User.Password);
                if (result.Succeeded)
                {

                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    if (roleResult.Succeeded)
                    {
                        return "Me sukes";
                    }

                }
                else
                {
                    return "Error";
                }
            
            }
            catch (Exception)
            {
                return "Error";
            }
            return "Error";
        }

    }
}
