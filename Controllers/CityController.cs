using System;
using System.Linq;
using System.Web.Mvc;
using Sales.Models;

namespace Sales.Controllers
{
    public class CityController : Controller
    {
        private SalesDbContext db = new SalesDbContext();

        // GET: City
        public ActionResult City()
        {
            return View();
        }

        // POST: City
        [HttpPost]
        public ActionResult City(City city)
        {
            if (ModelState.IsValid)
            {
                // Check if the city already exists in the database
                if (db.Cities.Any(c => c.CityName == city.CityName))
                {
                    TempData["Message"] = "City already exists!";
                    return RedirectToAction("City");
                }

                // Add the new city to the database
                db.Cities.Add(city);
                db.SaveChanges();

                TempData["Message"] = "City added successfully!";
                return RedirectToAction("City");
            }

            return View(city);
        }

       
        // Cancel action to handle Cancel button
        public ActionResult Cancel()
        {
            TempData["Message"] = "City registration cancelled."; // Set the cancellation message
            return RedirectToAction("City"); // Stay on the same page
        }



        public ActionResult CityEditDelete(int page = 1)
        {
            int pageSize = 5; // Number of cities per page
            int totalCities = db.Cities.Count(); // Total number of cities in the database
            int totalPages = (int)Math.Ceiling((double)totalCities / pageSize);

            var cities = db.Cities
                           .OrderBy(c => c.CityId)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(cities);
        }


        // Edit: Fetch city data for editing
        [HttpGet]
        public JsonResult GetCity(int id)
        {
            var city = db.Cities.FirstOrDefault(c => c.CityId == id);
            if (city == null)
                return Json(new { success = false, message = "City not found!" }, JsonRequestBehavior.AllowGet);

            return Json(new { success = true, city }, JsonRequestBehavior.AllowGet);
        }

        // Edit: Save updated city data
        [HttpPost]
        public JsonResult EditCity(City city)
        {
            if (ModelState.IsValid)
            {
                var existingCity = db.Cities.FirstOrDefault(c => c.CityId == city.CityId);
                if (existingCity != null)
                {
                    existingCity.CityName = city.CityName;
                    db.SaveChanges();
                    return Json(new { success = true, message = "City updated successfully!" });
                }
            }
            return Json(new { success = false, message = "Error updating city!" });
        }

        // Delete: Delete city by ID
        [HttpPost]
        public JsonResult DeleteCity(int id)
        {
            var city = db.Cities.FirstOrDefault(c => c.CityId == id);
            if (city != null)
            {
                db.Cities.Remove(city);
                db.SaveChanges();
                return Json(new { success = true, message = "City deleted successfully!" });
            }
            return Json(new { success = false, message = "City not found!" });
        }

        public JsonResult Cancels()
        {
            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("CityEditDelete", "City")
            }, JsonRequestBehavior.AllowGet);
        }

        
    }
}