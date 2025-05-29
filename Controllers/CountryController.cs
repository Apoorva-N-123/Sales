using System;
using System.Linq;
using System.Web.Mvc;
using Sales.Models;

namespace Sales.Controllers
{
    public class CountryController : Controller
    {
        private SalesDbContext db = new SalesDbContext();

        public ActionResult Country()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Country(Country country)
        {
            if (ModelState.IsValid)
            {
                bool isCountryExists = db.Countries.Any(c => c.CountryName == country.CountryName);

                if (isCountryExists)
                {
                    return Json(new { success = false, message = "Country already exists." });
                }

                db.Countries.Add(country);
                db.SaveChanges();

                return Json(new { success = true, message = "Country added successfully!" });
            }

            return Json(new { success = false, message = "Please fill in all required fields." });
        }

        public JsonResult Cancel()
        {
            return Json(new { message = "Country registration cancelled.", redirectUrl = Url.Action("Success", "Account") }, JsonRequestBehavior.AllowGet);
        }

        // GET: CountryEditDelete
        public ActionResult CountryEditDelete(int page = 1)
        {
            int pageSize = 5; // Number of cities per page
            int totalCountries = db.Countries.Count(); // Total number of cities in the database
            int totalPages = (int)Math.Ceiling((double)totalCountries / pageSize);

            var country = db.Countries
                           .OrderBy(c => c.CountryId)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(country);
        }

        [HttpGet]
        public JsonResult GetCountry(int id)
        {
            var country = db.Countries.FirstOrDefault(c => c.CountryId == id);
            if (country == null)
                return Json(new { success = false, message = "Country not found!" }, JsonRequestBehavior.AllowGet);

            return Json(new { success = true, country }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditCountry(Country country)
        {
            if (ModelState.IsValid)
            {
                var existingCountry = db.Countries.FirstOrDefault(c => c.CountryId == country.CountryId);
                if (existingCountry != null)
                {
                    existingCountry.CountryName = country.CountryName;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Country updated successfully!" });
                }
            }
            return Json(new { success = false, message = "Error updating country !" });
        }

        [HttpPost]
        public JsonResult DeleteCountry(int id)
        {
            var country = db.Countries.FirstOrDefault(c => c.CountryId == id);
            if (country != null)
            {
                db.Countries.Remove(country);
                db.SaveChanges();
                return Json(new { success = true, message = "Country deleted successfully!" });
            }
            return Json(new { success = false, message = "Country not found!" });
        }
    }
}
