// Controllers/WHController.cs
using System.Linq;
using System.Web.Mvc;
using Sales.Models;

namespace Sales.Controllers
{
    public class WHController : Controller
    {
        private readonly SalesDbContext _context;

        public WHController()
        {
            _context = new SalesDbContext();
        }

        // GET: WH/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WH/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WH wh)
        {
            if (ModelState.IsValid)
            {
                // Check if WHCode already exists
                if (_context.WHs.Any(w => w.WHCode == wh.WHCode))
                {
                    TempData["PopupMessage"] = "Warehouse with this code already exists.";
                    TempData["PopupType"] = "warning";
                    return View(wh);
                }

                // Add new Warehouse
                _context.WHs.Add(wh);
                _context.SaveChanges();

                // Set success message for the pop-up and stay on the same page
                TempData["PopupMessage"] = "Warehouse created successfully.";
                TempData["PopupType"] = "success";
                return View(wh);
            }

            return View(wh);
        }

        // GET: WH/Cancel
        public ActionResult Cancel()
        {
            // Set the popup message for cancellation confirmation
            TempData["PopupMessage"] = "Are you sure you want to cancel the registration?";
            TempData["PopupType"] = "info";
            return View("Create");
        }

        public ActionResult ConfirmCancel()
        {
            // This will be redirected to after clicking "OK" on the popup
            return View("~/Views/Account/Success.cshtml");
        }

        // GET: WH/WHEditDelete
        public ActionResult WHEditDelete()
        {
            var whList = _context.WHs.ToList();
            return View(whList);
        }

        // POST: WH/EditWH
        [HttpPost]
        public JsonResult EditWH(WH wh)
        {
            if (ModelState.IsValid)
            {
                var existingWH = _context.WHs.Find(wh.WHId);
                if (existingWH != null)
                {
                    existingWH.WHCode = wh.WHCode;
                    existingWH.WHDescription = wh.WHDescription;
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Warehouse updated successfully." });
                }
            }
            return Json(new { success = false, message = "Error updating warehouse." });
        }

        // POST: WH/DeleteWH
        [HttpPost]
        public JsonResult DeleteWH(int id)
        {
            var wh = _context.WHs.Find(id);
            if (wh != null)
            {
                _context.WHs.Remove(wh);
                _context.SaveChanges();
                return Json(new { success = true, message = "Warehouse deleted successfully." });
            }
            return Json(new { success = false, message = "Error deleting warehouse." });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
