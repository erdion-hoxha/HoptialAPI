using HosptialAPI.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    interface IAuthentificationRepository
    {
        //public Task<string> Login(UserDto user);
        public Task<string> Register(UserDto user);

    }
}
