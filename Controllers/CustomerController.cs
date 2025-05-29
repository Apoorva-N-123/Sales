using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Sales.Models;

namespace Sales.Controllers
{
    public class CustomerController : Controller
    {
        private SalesDbContext db = new SalesDbContext();

        // GET: Customer/Customer
        public ActionResult Customer()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        // POST: Customer/Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Customer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var existingCustomer = db.Customers
                    .FirstOrDefault(c => c.Email == customer.Email || c.GSTIN == customer.GSTIN);

                if (existingCustomer != null)
                {
                    TempData["Message"] = "Customer already exists with the same email or GSTIN!";
                    TempData["MessageType"] = "error";
                    return RedirectToAction("Customer");
                }

                db.Customers.Add(customer);
                db.SaveChanges();

                TempData["Message"] = "Customer added successfully!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Customer");
            }

            return View(customer);
        }

        // GET: Customer/GetCities - AJAX action for city lookup
        public JsonResult GetCities(string searchTerm)
        {
            var cities = db.Cities
                .Where(c => c.CityName.ToLower().Contains(searchTerm.ToLower()))
                .Select(c => new { c.CityName })
                .ToList();

            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        // GET: Customer/GetCountries - AJAX action for country lookup
        public JsonResult GetCountries(string searchTerm)
        {
            var countries = db.Countries
                .Where(c => c.CountryName.ToLower().Contains(searchTerm.ToLower()))
                .Select(c => new { c.CountryName })
                .ToList();

            return Json(countries, JsonRequestBehavior.AllowGet);
        }

        // GET: Customer/GetStates - AJAX action for state lookup
        public JsonResult GetStates(string searchTerm)
        {
            var states = db.States
                .Where(s => s.StateName.ToLower().Contains(searchTerm.ToLower()))
                .Select(s => new { s.StateName, s.GSTStateCode })
                .ToList();

            return Json(states, JsonRequestBehavior.AllowGet);
        }

        // GET: Customer/Cancel
        public ActionResult Cancel()
        {
            TempData["Message"] = "Customer registration cancelled successfully!";
            TempData["MessageType"] = "info";
            return RedirectToAction("Success", "Account");
        }

        // GET: Customer/CustomerEditDelete
        public ActionResult CustomerEditDelete(int page = 1)
        {
            int pageSize = 5; // Number of cities per page
            int totalCustomers = db.Customers.Count(); // Total number of cities in the database
            int totalPages = (int)Math.Ceiling((double)totalCustomers / pageSize);

            var customers = db.Customers
            .OrderBy(c => c.CustomerId)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(customers);
        }

        // GET: Customer/Edit/5
        public ActionResult CustomerEdit(int id)
        {
            var customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerEdit1(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var existingCustomer = db.Customers.Find(customer.CustomerId);

                if (existingCustomer != null)
                {
                    // Update fields
                    existingCustomer.CustomerName = customer.CustomerName;
                    existingCustomer.AddressLine1 = customer.AddressLine1;
                    existingCustomer.AddressLine2 = customer.AddressLine2;
                    existingCustomer.AddressLine3 = customer.AddressLine3;
                    existingCustomer.City = customer.City;
                    existingCustomer.State = customer.State;
                    existingCustomer.Country = customer.Country;
                    existingCustomer.Pincode = customer.Pincode;
                    existingCustomer.GSTIN = customer.GSTIN;
                    existingCustomer.PhoneNumber = customer.PhoneNumber;
                    existingCustomer.Email = customer.Email;
                    existingCustomer.WebsiteLink = customer.WebsiteLink;
                    existingCustomer.Remarks = customer.Remarks;

                    db.SaveChanges();

                    TempData["Message"] = "Customer updated successfully!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("CustomerEditDelete");
                }

                TempData["Message"] = "Customer not found!";
                TempData["MessageType"] = "error";
            }

            return View(customer);
        }


        // POST: Customer/Delete/5
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var customer = db.Customers.Find(id);
            if (customer != null)
            {
                db.Customers.Remove(customer);
                db.SaveChanges();
                return Json(new { success = true, message = "Customer deleted successfully!" });
            }
            return Json(new { success = false, message = "Customer not found!" });
        }
    }
}