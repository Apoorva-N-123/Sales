using System.Linq;
using System.Web.Mvc;
using Sales.Models;

namespace Sales.Controllers
{
    public class HSNController : Controller
    {
        private readonly SalesDbContext _context;

        public HSNController()
        {
            _context = new SalesDbContext();
        }

        // GET: HSN/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HSN/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HSN hsn)
        {
            if (ModelState.IsValid)
            {
                // Check if HSN Code already exists
                if (_context.HSNs.Any(h => h.HSNCode == hsn.HSNCode))
                {
                    TempData["PopupMessage"] = "HSN already existed.";
                    TempData["PopupType"] = "warning";
                    return View(hsn);
                }

                // Add new HSN
                _context.HSNs.Add(hsn);
                _context.SaveChanges();

                TempData["PopupMessage"] = "HSN created successfully.";
                TempData["PopupType"] = "success";

                // Stay on the Create page after saving
                return RedirectToAction("Create");
            }

            return View(hsn);
        }

        // GET: HSN/Cancel
        public ActionResult Cancel()
        {
            TempData["PopupMessage"] = "HSN registration cancelled successfully.";
            TempData["PopupType"] = "info";

            // Show the modal and stay on the Create page
            return View("Create");
        }

        // Action to handle the redirection after cancel
        public ActionResult RedirectToSuccessPage()
        {
            return View("~/Views/HSN/HSNEditDelete.cshtml");
        }
        // GET: HSN/HSNEditDelete
        public ActionResult HSNEditDelete()
        {
            var hsnList = _context.HSNs.ToList();
            return View(hsnList);
        }

        // POST: HSN/EditHSN
        [HttpPost]
        public JsonResult EditHSN(HSN hsn)
        {
            if (ModelState.IsValid)
            {
                var existingHSN = _context.HSNs.Find(hsn.HSNId);
                if (existingHSN != null)
                {
                    existingHSN.HSNCode = hsn.HSNCode;
                    existingHSN.HSNDescription = hsn.HSNDescription;
                    _context.SaveChanges();
                    return Json(new { success = true, message = "HSN updated successfully." });
                }
            }
            return Json(new { success = false, message = "Error updating HSN." });
        }

        // POST: HSN/DeleteHSN
        [HttpPost]
        public JsonResult DeleteHSN(int id)
        {
            var hsn = _context.HSNs.Find(id);
            if (hsn != null)
            {
                _context.HSNs.Remove(hsn);
                _context.SaveChanges();
                return Json(new { success = true, message = "HSN deleted successfully." });
            }
            return Json(new { success = false, message = "Error deleting HSN." });
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
