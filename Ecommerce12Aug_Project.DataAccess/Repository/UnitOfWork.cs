using Ecommerce12Aug_Project.Data;
using Ecommerce12Aug_Project.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce12Aug_Project.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Catagory = new CatagoryRepository(context);
            CoverType = new CoverTypeRepository(context);
            Product = new ProductRepository(context);
            Company = new CompanyRepository(context);
            ApplicationUser = new ApplicationUserRepository(context);
        }
        public ICatagoryRepository Catagory { private set; get; }

        public ICovertypeRepository CoverType { private set; get; }
        public IProductRepository Product { private set; get; }
        public ICompanyRepository Company { private set; get; }

        public IApplicationUserRepository ApplicationUser {  private set; get; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
