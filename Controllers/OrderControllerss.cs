using System.Web.Mvc;
using Sales.Models;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Sales.Controllers
{
    public class OrderControllerss : Controller
    {
        private SalesDbContext db = new SalesDbContext();

        // GET: Order/Register
        public ActionResult Register()
        {
            var customerList = db.Customers.Select(c => new SelectListItem
            {
                Text = c.CustomerName, // Display this in the dropdown
                Value = c.CustomerName // Use CustomerName as the value (or use CustomerId)
            }).ToList();

            if (!customerList.Any())
            {
                throw new Exception("Customer list is empty!");
            }

            ViewBag.CustomerNames = customerList;

            // Fetch PaymentDescriptions from the Payments table
            var paymentDescriptions = db.Payments
                .Select(p => new SelectListItem
                {
                    Text = p.PaymentDescription,
                    Value = p.PaymentDescription
                })
                .ToList();

            // Pass the data to the ViewBag
            ViewBag.PaymentDescriptions = paymentDescriptions;

            return View();
        }

        // POST: Order/SubmitOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitOrder(Orderss order, List<OrderDetails> selectedProducts)
        {
            // Check if the OrderReferenceNumber already exists in the database
            bool orderExists = db.Orderss.Any(o => o.OrderReferenceNumber == order.OrderReferenceNumber);

            if (orderExists)
            {
                TempData["ErrorMessage"] = "Order Reference Number already exists!";
                return RedirectToAction("Register");
            }

            // Validate the OrderReferenceNumber length
            if (order.OrderReferenceNumber.Length != 25)
            {
                ModelState.AddModelError("OrderReferenceNumber", "Order Reference Number must be exactly 25 characters long.");
            }

            if (string.IsNullOrEmpty(order.PaymentTerms) || order.PaymentTerms == "Select Payment Terms")
            {
                ModelState.AddModelError("PaymentTerms", "Please select a valid payment term.");
            }

            // Validate OrderDate and OrderReferenceDate
            DateTime today = DateTime.Today;

            if (order.OrderDate > today)
            {
                ModelState.AddModelError("OrderDate", "Order Date cannot be a future date.");
            }

            if (order.OrderReferenceDate > today)
            {
                ModelState.AddModelError("OrderReferenceDate", "Order Reference Date cannot be a future date.");
            }

            // Check if products were selected
            if (selectedProducts == null || selectedProducts.Count == 0)
            {
                ModelState.AddModelError("", "Please add at least one product.");
                return View("Register");
            }

            // Inside SubmitOrder action:
            if (ModelState.IsValid)
            {
                try
                {
                    // Save the order to the Orderss table
                    db.Orderss.Add(order);
                    db.SaveChanges(); // Save order first to generate OrderId

                    int orderId = order.OrderId; // Get the generated OrderId

                    // Save the products to the OrderDetails table
                    foreach (var product in selectedProducts)
                    {
                        var orderProduct = new OrderDetails
                        {
                            OrderId = orderId,  // Foreign key to the Orderss table
                            ProductCode = product.ProductCode,
                            ProductDescription = product.ProductDescription,
                            OrderQuantity = product.OrderQuantity,
                            PackType = product.PackType,
                            Rate = product.Rate
                        };
                        db.OrderDetails.Add(orderProduct);
                    }

                    db.SaveChanges(); // Save all the product details to the database

                    TempData["SuccessMessage"] = "Order registered successfully!";
                    return RedirectToAction("Register");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while saving the order: {ex.Message}";
                    return RedirectToAction("Register");
                }
            }


            // If validation fails, reload the Register view with validation errors
            return View("Register");
        }

        // POST: Order/CancelOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder()
        {
            TempData["CancelMessage"] = "Order was cancelled!";
            return RedirectToAction("Register");  // This will ensure that the form is reset
        }

        // Method to get products for dropdown
        public JsonResult GetProducts()
        {
            var products = db.Products
                .Select(p => new { p.ProductCode, p.ProductDescription })
                .ToList();
            return Json(products, JsonRequestBehavior.AllowGet);
        }
    }
}
