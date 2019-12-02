using System;
using System.Linq;

#region Custom Usings
using DataAccess.Abstracts.Interfaces.RepositoryEntities;

using DataAccess.Abstracts.Interfaces.RepositoryBase;

using Models.DatabaseModels.DatabaseContext;

using DataAccess.Concretes.Classes.UnitOfWork;
using DataAccess.Abstracts.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Models.DatabaseModels.DatabaseEntities;
#endregion Custom Usings

namespace Test.Test
{
    class Program
    {

        static void Main(string[] args)
        {
            OrderDBContext currentDbContext = new OrderDBContext();

            UserTransactions userTransactions = new UserTransactions(dbContext: currentDbContext);

            userTransactions.CreateUser(new Users
            {
                CreationDate = DateTime.Now,
                Email = "hamsi.palamut@gmail.com",
                Id = Guid.NewGuid(),
                Name = "Hamsi",
                Password = "Password",
                Status = true,
                Surname = "Palamut"
            });

            Console.WriteLine("Hello World!");
        }

    }

    class UserTransactions
    {

        IUnitOfWork unitOfWorkForUsers;
        public UserTransactions(DbContext dbContext)
        {
            unitOfWorkForUsers = new UnitOfWork(dbContext: dbContext);
        }

        public Users CreateUser(Users newUserInformation)
        {
            Users createdUser = default(Users);
            try
            {
                using (var transactionScope = this.unitOfWorkForUsers.BeginTransaction())
                {
                    int numberOfRowsAffected = default(int);

                    createdUser = this.unitOfWorkForUsers.RepositoryOfUser
                                                            .InsertItem(insertItem: newUserInformation);

                    numberOfRowsAffected = this.unitOfWorkForUsers.SaveChanges();
                    if (numberOfRowsAffected > 0)
                    {
                        transactionScope.Complete();
                    }
                    else
                    {
                        transactionScope.Dispose();

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return createdUser;
        }
    }


}
