using Ecommerce12Aug_Project.DataAccess.Repository.IRepository;
using Ecommerce12Aug_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce12Aug_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Catagory catagory = new Catagory();
            if (id == null) return View(catagory);

            catagory = _unitOfWork.Catagory.Get(id.GetValueOrDefault());
            if (catagory == null) return NotFound();
            return View(catagory);
        }
        [HttpPost]
        public IActionResult Upsert(Catagory catagory)
        {
            if (catagory == null) return BadRequest();
            if (!ModelState.IsValid) return View(catagory);
            if (catagory.Id == 0)
                _unitOfWork.Catagory.Add(catagory);
            else
                _unitOfWork.Catagory.Update(catagory);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

       
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var categoryList = _unitOfWork.Catagory.GetAll();
            return Json(new { data = categoryList });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryInDb = _unitOfWork.Catagory.Get(id);
            if (categoryInDb == null)
                return Json(new { success = false, message = "Unable to delete data !!!" });
            _unitOfWork.Catagory.Remove(categoryInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data deleted succesfully !!!" });
        }
        #endregion
    }       
}
