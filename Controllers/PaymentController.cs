using System;
using System.Linq;
using System.Web.Mvc;
using Sales.Models;

namespace Sales.Controllers
{
    public class PaymentController : Controller
    {
        private SalesDbContext db = new SalesDbContext();

        // GET: Payment/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Payment payment)
        {
            // Check if PaymentCode already exists
            var existingPayment = db.Payments.FirstOrDefault(p => p.PaymentCode == payment.PaymentCode);
            if (existingPayment != null)
            {
                TempData["PopupType"] = "Error";
                TempData["PopupMessage"] = $"Payment Code '{payment.PaymentCode}' already exists. Please enter a unique Payment Code.";
                return View(payment);
            }

            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                db.SaveChanges();
                TempData["PopupType"] = "Success";
                TempData["PopupMessage"] = "Payment details saved successfully.";
                return RedirectToAction("Create");
            }

            // If ModelState is invalid, return the same view with validation errors
            TempData["PopupType"] = "Error";
            TempData["PopupMessage"] = "Please correct the errors in the form.";
            return View(payment);
        }

        // GET: KCS/Cancel
        public ActionResult Cancel()
        {
            TempData["PopupMessage"] = "Payment registration cancelled successfully.";
            TempData["PopupType"] = "info";

            // Return to Create view with cancel message in pop-up
            return RedirectToAction("Create");
        }


        // GET: Payment/EditDelete
        public ActionResult PaymentEditDelete(int page = 1)
        {
            int pageSize = 5;
            var payments = db.Payments.OrderBy(s => s.PaymentId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalStates = db.Payments.Count();
            int totalPages = (int)Math.Ceiling((double)totalStates / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(payments);
        }

        [HttpPost]
        public JsonResult Edit(int id, string PaymentCode, string PaymentDescription)
        {
            var payment = db.Payments.FirstOrDefault(p => p.PaymentId == id);
            if (payment == null)
            {
                return Json(new { success = false, message = "Payment not found." });
            }

            payment.PaymentCode = PaymentCode;
            payment.PaymentDescription = PaymentDescription;
            db.SaveChanges();

            return Json(new { success = true, message = "Payment updated successfully." });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var payment = db.Payments.FirstOrDefault(p => p.PaymentId == id);
            if (payment == null)
            {
                return Json(new { success = false, message = "Payment not found." });
            }

            db.Payments.Remove(payment);
            db.SaveChanges();

            return Json(new { success = true, message = "Payment deleted successfully." });
        }
    }
}
