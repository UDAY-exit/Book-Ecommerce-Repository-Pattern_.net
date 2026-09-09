using Ecommerce12Aug_Project.DataAccess.Repository;
using Ecommerce12Aug_Project.DataAccess.Repository.IRepository;
using Ecommerce12Aug_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce12Aug_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null) return View(coverType);

            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if (coverType == null) return NotFound();
            return View(coverType);
        }

        [HttpPost]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null) return NotFound();
            if(!ModelState.IsValid)return BadRequest();
            if (coverType.Id == 0)
                _unitOfWork.CoverType.Add(coverType);
            else
                _unitOfWork.CoverType.Update(coverType);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
     

        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var covertype = _unitOfWork.CoverType.GetAll();
            return Json(new { data =covertype });
        }
        [HttpDelete]

        public IActionResult Delete(int id)
        {
            var Deldata = _unitOfWork.CoverType.Get(id);
            if (Deldata == null)
                return Json(new { success = false, message = "Unable to delete data!!!" });
            _unitOfWork.CoverType.Remove(Deldata);
            _unitOfWork.Save();
            return Json(new { success = true, message = "data is deleated succesfully!!!" });
        }
        #endregion
    }
}
