using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    class UnitOfWork : IUnitOfWork
    {
        //public IAuthentificationRepository _authentificationRepository;
        private DatabaseContext Context;


        public UnitOfWork()
        {
            Context = new DatabaseContext();
        }
        public UnitOfWork(DatabaseContext context)
        {
            Context = context;
        }

        
        public void SaveChanges()
        {
        }
    }
}
