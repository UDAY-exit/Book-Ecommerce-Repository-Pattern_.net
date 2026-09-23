using Ecommerce12Aug_Project.Data;
using Ecommerce12Aug_Project.DataAccess.Repository.IRepository;
using Ecommerce12Aug_Project.Models;
using Ecommerce12Aug_Project.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce12Aug_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]

    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(ApplicationDbContext context,IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }



        #region APIs

        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.ToList();//aspnetusers
            var roleList = _context.Roles.ToList();//aspnetroles
            var userRoles = _context.UserRoles.ToList();//aspnetuserroles

            foreach(var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roleList.FirstOrDefault(r => r.Id == roleId).Name;
                if(user.CompanyId == null)
                {
                    user.Company = new Models.Company()
                    {
                        Name = ""
                    };
                }
                if(user.CompanyId != null)
                {
                    user.Company = new Company()
                    {
                        Name = _unitOfWork.Company.Get(Convert.ToInt32(user.CompanyId)).Name
                    };
                }
            }
            //Remove Admin ROle User

            var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            userList.Remove(adminUser);

            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string Id)
        {
            bool isLocked = false;
            var userInDb = _unitOfWork.ApplicationUser.FirstOrDefault(u => u.Id == Id);
            if(userInDb == null)
            {
                return Json(new { success = false, message = "Something went wrong When Lock And Unlock User" });
            }if(userInDb != null && userInDb.LockoutEnd > DateTime.Now)
            {
                userInDb.LockoutEnd = DateTime.Now;
                isLocked = false;
            }
            else
            {
                userInDb.LockoutEnd = DateTime.Now.AddYears(100);
                isLocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = isLocked == true ? "User Successfully Locked" : "User SuccessFully Unlocked" });
        }

        #endregion
    }
}
