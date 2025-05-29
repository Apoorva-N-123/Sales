// Controllers/KCSController.cs
using System.Linq;
using System.Web.Mvc;
using Sales.Models;

namespace Sales.Controllers
{
    public class KCSController : Controller
    {
        private readonly SalesDbContext _context;

        public KCSController()
        {
            _context = new SalesDbContext();
        }

        // GET: KCS/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KCS/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KCS kcs)
        {
            if (ModelState.IsValid)
            {
                // Check if KCSCode already exists
                if (_context.KCSs.Any(k => k.KCSCode == kcs.KCSCode))
                {
                    TempData["PopupMessage"] = "KCS with this code already exists.";
                    TempData["PopupType"] = "warning";
                    return View(kcs);
                }

                // Add new KCS
                _context.KCSs.Add(kcs);
                _context.SaveChanges();

                TempData["PopupMessage"] = "KCS created successfully.";
                TempData["PopupType"] = "success";

                // Show success pop-up and stay on the Create page
                return View(kcs);
            }

            // If model state is invalid, return the same page with validation errors
            return View(kcs);
        }

        // GET: KCS/Cancel
        public ActionResult Cancel()
        {
            TempData["PopupMessage"] = "KCS registration cancelled successfully.";
            TempData["PopupType"] = "info";

            // Return to Create view with cancel message in pop-up
            return RedirectToAction("Create");
        }

        public ActionResult ShowSuccessPage()
        {
            return View("~/Views/Account/Success.cshtml");
        }

        // GET: KCS/EditDelete
        public ActionResult KCSEditDelete()
        {
            var kcsList = _context.KCSs.ToList();
            return View(kcsList);
        }

        // POST: KCS/Edit
        [HttpPost]
        public JsonResult Edit(KCS updatedKCS)
        {
            if (ModelState.IsValid)
            {
                var kcs = _context.KCSs.Find(updatedKCS.KCSId);
                if (kcs != null)
                {
                    kcs.KCSCode = updatedKCS.KCSCode;
                    kcs.KCSDescription = updatedKCS.KCSDescription;
                    _context.SaveChanges();
                    return Json(new { success = true, message = "KCS updated successfully!" });
                }
            }
            return Json(new { success = false, message = "Error updating KCS." });
        }

        // POST: KCS/Delete
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var kcs = _context.KCSs.Find(id);
            if (kcs != null)
            {
                _context.KCSs.Remove(kcs);
                _context.SaveChanges();
                return Json(new { success = true, message = "KCS deleted successfully!" });
            }
            return Json(new { success = false, message = "Error deleting KCS." });
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
