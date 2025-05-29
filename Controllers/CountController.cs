using System;
using System.Linq;
using System.Web.Mvc;
using Sales.Models;

namespace Sales.Controllers
{
    public class CountController : Controller
    {
        private readonly SalesDbContext _context;

        public CountController()
        {
            _context = new SalesDbContext();
        }

        // GET: Count/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Count/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Count count)
        {
            if (ModelState.IsValid)
            {
                // Check if Count Code already exists
                if (_context.Counts.Any(c => c.CountCode == count.CountCode))
                {
                    TempData["PopupMessage"] = "Count with this code already exists.";
                    TempData["PopupType"] = "warning";
                    return View(count);
                }

                // Add new Count
                _context.Counts.Add(count);
                _context.SaveChanges();

                TempData["PopupMessage"] = "Count created successfully.";
                TempData["PopupType"] = "success";
                return View(count);
            }

            return View(count);
        }

        // GET: Count/Cancel
        // GET: Count/Cancel
        public ActionResult Cancel()
        {
            TempData["PopupMessage"] = "Count registration cancelled successfully.";
            TempData["PopupType"] = "info";
            return RedirectToAction("Create");
        }


        public ActionResult ShowSuccessPage()
        {
            return View("~/Views/Account/Success.cshtml");
        }

        // GET: Count/CountEditDelete
        public ActionResult CountEditDelete(int page = 1)
        {
            int pageSize = 5; // Number of records per page
            var counts = _context.Counts.OrderBy(c => c.CountId) // Adjust sorting as needed
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();

            int totalRecords = _context.Counts.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = page;

            return View(counts);
        }

        // POST: Count/Edit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(Count model)
        {
            if (ModelState.IsValid)
            {
                // Perform the update operation
                var existingCount = _context.Counts.Find(model.CountId);
                if (existingCount != null)
                {
                    string oldCode = existingCount.CountCode; // Save the old code for reference
                    existingCount.CountCode = model.CountCode;
                    existingCount.CountDescription = model.CountDescription;
                    _context.SaveChanges();

                    return Json(new { success = true, newData = model, oldCode });
                }

                return Json(new { success = false, message = "Count not found." });
            }

            return Json(new { success = false, message = "Invalid data submitted." });
        }

        // POST: Count/Delete
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var count = _context.Counts.Find(id);
            if (count != null)
            {
                _context.Counts.Remove(count);
                _context.SaveChanges();
                return Json(new { success = true, message = "Count deleted successfully." });
            }

            return Json(new { success = false, message = "Count not found." });
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            var totalItems = _context.Counts.Count(); // Get total item count from the database
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize); // Calculate total pages

            var paginatedCounts = _context.Counts
                                           .OrderBy(c => c.CountId)
                                           .Skip((page - 1) * pageSize) // Skip previous pages
                                           .Take(pageSize)             // Take items for the current page
                                           .ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;

            return View("CountEditDelete", paginatedCounts); // Ensure you pass the correct view
        }
    }
}
