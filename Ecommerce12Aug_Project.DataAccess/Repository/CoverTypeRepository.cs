using Ecommerce12Aug_Project.Data;
using Ecommerce12Aug_Project.DataAccess.Repository.IRepository;
using Ecommerce12Aug_Project.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce12Aug_Project.DataAccess.Repository
{
    public class CoverTypeRepository:Repository<CoverType>,ICovertypeRepository
    {
        private readonly ApplicationDbContext _context;
        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
