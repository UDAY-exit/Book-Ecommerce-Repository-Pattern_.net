using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce12Aug_Project.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICatagoryRepository Catagory { get; }
        ICovertypeRepository CoverType { get; }
        IProductRepository Product { get; }
        ICompanyRepository  Company { get; }
        IApplicationUserRepository ApplicationUser { get; }
        void Save();
    }
}
