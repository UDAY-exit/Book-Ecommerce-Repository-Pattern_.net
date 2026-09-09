using Ecommerce12Aug_Project.Data;
using Ecommerce12Aug_Project.DataAccess.Repository.IRepository;
using Ecommerce12Aug_Project.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce12Aug_Project.DataAccess.Repository
{
    public class CatagoryRepository : Repository<Catagory>,
        ICatagoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CatagoryRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}
