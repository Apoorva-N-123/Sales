using System.Linq;
using System.Web.Mvc;
using Sales.Models;

namespace Sales.Controllers
{
    public class VarietyController : Controller
    {
        private readonly SalesDbContext _context;

        public VarietyController()
        {
            _context = new SalesDbContext();
        }

        // GET: Variety/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Variety/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Variety variety)
        {
            if (ModelState.IsValid)
            {
                if (_context.Varieties.Any(v => v.VarietyCode == variety.VarietyCode))
                {
                    TempData["PopupMessage"] = "Variety with this code already exists.";
                    TempData["PopupType"] = "warning";
                    return View(variety);
                }

                _context.Varieties.Add(variety);
                _context.SaveChanges();

                TempData["PopupMessage"] = "Variety created successfully.";
                TempData["PopupType"] = "success";
                return View(variety);
            }

            return View(variety);
        }



        // GET: Variety/Cancel
        public ActionResult Cancel()
        {
            TempData["PopupMessage"] = "Variety registration cancelled successfully.";
            TempData["PopupType"] = "info";

            // Return the Create view to stay on the same page and show the modal
            return RedirectToAction("Create");
        }

        public ActionResult ShowSuccessPage()
        {
            return View("~/Views/Account/Success.cshtml");
        }

        // GET: VarietyEditDelete
        public ActionResult VarietyEditDelete()
        {
            var varieties = _context.Varieties.ToList();
            return View(varieties);
        }

        // POST: Variety/Edit
        [HttpPost]
        public JsonResult EditVariety(Variety variety)
        {
            var existingVariety = _context.Varieties.Find(variety.VarietyId);
            if (existingVariety != null)
            {
                existingVariety.VarietyCode = variety.VarietyCode;
                existingVariety.VarietyDescription = variety.VarietyDescription;
                _context.SaveChanges();

                return Json(new { success = true, message = "Variety updated successfully." });
            }
            return Json(new { success = false, message = "Variety not found." });
        }

        // POST: Variety/Delete
        [HttpPost]
        public JsonResult DeleteVariety(int id)
        {
            var variety = _context.Varieties.Find(id);
            if (variety != null)
            {
                _context.Varieties.Remove(variety);
                _context.SaveChanges();

                return Json(new { success = true, message = "Variety deleted successfully." });
            }
            return Json(new { success = false, message = "Variety not found." });
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
