using System;
using System.Linq;

#region Custom Usings
using DataAccess.Abstracts.Interfaces.RepositoryEntities;

using DataAccess.Abstracts.Interfaces.RepositoryBase;

using Models.DatabaseModels.DatabaseContext;

using DataAccess.Concretes.Classes.UnitOfWork;
using DataAccess.Abstracts.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;
#endregion Custom Usings

namespace Test.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IUnitOfWork unitOfWork = new UnitOfWork(dbContext: new yeniDbContext());

            var asdf = unitOfWork.RepositoryOfCategory.GetAllItems().ToList();


            Console.WriteLine("Hello World!");
        }
    }

    class yeniDbContext : DbContext
    {

    }
}
