using Sales.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sales.Controllers
{
    public class OrderController : Controller
    {
        private readonly SalesDbContext _context;

        public OrderController()
        {
            _context = new SalesDbContext();
        }

        // GET: Order/Register

        // GET: Order/Register
        public ActionResult Register()
        {
            var order = new Orders
            {
                ProductDetails = new List<ProductDetails>()  // Initialize the ProductDetails collection
            };

            // Fetch Customer Names from Customers table and store in ViewBag
            var customers = _context.Customers.Select(c => c.CustomerName).ToList();
            ViewBag.Customers = customers;

            // Fetch Payment Descriptions from Payments table and store in ViewBag
            var paymentTerms = _context.Payments.Select(p => p.PaymentDescription).ToList();
            ViewBag.PaymentTerms = paymentTerms;

            return View(order);
        }


        // POST: Order/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Orders order)
        {
            // Check if the OrderReferenceNumber already exists in the database
            bool orderExists = _context.Orders.Any(o => o.OrderReferenceNumber == order.OrderReferenceNumber);

            if (orderExists)
            {
                ModelState.AddModelError("OrderReferenceNumber", "Order Reference Number already exists!");
            }

            // Validate the OrderReferenceNumber length
            if (order.OrderReferenceNumber.Length != 25)
            {
                ModelState.AddModelError("OrderReferenceNumber", "Order Reference Number must be exactly 25 characters long.");
            }

            // Validate Payment Terms
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

            // Validate if any products are selected
            if (order.ProductDetails == null || !order.ProductDetails.Any() || order.ProductDetails.Any(pd => pd.OrderQuantity <= 0))
            {
                ModelState.AddModelError("ProductDetails", "Please select at least one product and specify the quantity.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure order object is properly initialized
                    order.ProductDetails = order.ProductDetails?.Distinct().ToList(); // Remove duplicates

                    _context.Orders.Add(order); // Only add the order once
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Order registered successfully!";
                    return RedirectToAction("Register");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error: " + ex.Message;
                }
            }
            // Reload dropdown data in case of errors
            ViewBag.Customers = _context.Customers.Select(c => c.CustomerName).ToList();
            ViewBag.PaymentTerms = _context.Payments.Select(p => p.PaymentDescription).ToList();

            return View(order);
        }


        // Controller code for fetching products
        public ActionResult GetProducts()
        {
            var products = _context.Products.ToList();  // Fetch all products
            return Json(products, JsonRequestBehavior.AllowGet); // Return products as JSON
        }

        public ActionResult OrderEditDelete()
        {
            var orders = _context.Orders.ToList();
            return View("OrderEditDelete", orders);
        }

        // GET: Order/Edit/{id}
        public ActionResult Edit(int id)
        {
            // Fetch order details
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            // Fetch related product details
            var productDetails = _context.ProductDetails.Where(pd => pd.OrderId == id).ToList();

            // Set the data for the view
            order.ProductDetails = productDetails ?? new List<ProductDetails>(); // Initialize if null

            return View("OrderEdit", order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Orders model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new SalesDbContext())
                {
                    var existingOrder = db.Orders
                        .Include(o => o.ProductDetails) // Include related product details
                        .FirstOrDefault(o => o.OrderId == model.OrderId);

                    if (existingOrder == null)
                    {
                        return HttpNotFound();
                    }

                    // Update order fields
                    existingOrder.OrderReferenceNumber = model.OrderReferenceNumber;
                    existingOrder.OrderDate = model.OrderDate;
                    existingOrder.OrderReferenceDate = model.OrderReferenceDate;
                    existingOrder.CustomerName = model.CustomerName;
                    existingOrder.DeliveryAddress = model.DeliveryAddress;
                    existingOrder.PayMode = model.PayMode;
                    existingOrder.PaymentTerms = model.PaymentTerms;

                    // Maintain a list of existing ProductDetailIds
                    var existingProductIds = existingOrder.ProductDetails.Select(p => p.ProductDetailId).ToList();

                    foreach (var updatedProduct in model.ProductDetails)
                    {
                        var existingProduct = existingOrder.ProductDetails
                            .FirstOrDefault(p => p.ProductDetailId == updatedProduct.ProductDetailId);

                        if (existingProduct != null)
                        {
                            // Update existing product details
                            existingProduct.ProductCode = updatedProduct.ProductCode;
                            existingProduct.ProductDescription = updatedProduct.ProductDescription;
                            existingProduct.OrderQuantity = updatedProduct.OrderQuantity;
                            existingProduct.PackType = updatedProduct.PackType;
                            existingProduct.Rate = updatedProduct.Rate;
                        }
                        else
                        {
                            // Add new product details
                            updatedProduct.OrderId = model.OrderId;
                            db.ProductDetails.Add(updatedProduct);
                        }

                        // Remove the updatedProductId from the existing list (to track deleted items)
                        existingProductIds.Remove(updatedProduct.ProductDetailId);
                    }

                    db.SaveChanges(); // Save changes

                    TempData["SuccessMessage"] = "Order updated successfully!";
                    return RedirectToAction("OrderEditDelete");
                }
            }

            return View("OrderEdit", model);
        }

        public ActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            // Delete related ProductDetails
            var productDetails = _context.ProductDetails.Where(pd => pd.OrderId == id).ToList();
            _context.ProductDetails.RemoveRange(productDetails);

            // Delete the order
            _context.Orders.Remove(order);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Order and related product details deleted successfully!";
            return RedirectToAction("OrderEditDelete");
        }

        public ActionResult OrderReport()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetOrderReport(DateTime fromDate, DateTime toDate)
        {
            var orderData = (from o in _context.Orders
                             join pd in _context.ProductDetails on o.OrderId equals pd.OrderId
                             where o.OrderDate >= fromDate && o.OrderDate <= toDate
                             select new
                             {
                                 o.OrderDate,
                                 o.OrderReferenceNumber,
                                 o.CustomerName,
                                 pd.ProductCode,
                                 pd.ProductDescription,
                                 pd.OrderQuantity
                             }).ToList();

            var orderReport = orderData.Select(o => new
            {
                o.OrderDate,
                o.OrderReferenceNumber,
                o.CustomerName,
                o.ProductCode,
                o.ProductDescription,
                o.OrderQuantity,
                OrderStatus = GetOrderStatus(o.CustomerName, o.ProductCode)
            }).ToList();

            return Json(orderReport, JsonRequestBehavior.AllowGet);
        }

        private string GetOrderStatus(string customerName, string productCode)
        {
            bool inOrders = _context.Orders.Any(o => o.CustomerName == customerName);
            bool inDespatch = _context.Despatch.Any(d => d.CustomerName == customerName);
            bool inInvoice = _context.Invoice.Any(i => i.CustomerName == customerName);

            bool productInProductDetails = _context.ProductDetails.Any(pd => pd.ProductCode == productCode);
            bool productInDespatchDetails = _context.DespatchDetails.Any(dd => dd.ProductCode == productCode);
            bool productInInvoiceDetails = _context.InvoiceDetails.Any(id => id.ProductCode == productCode);

            if (inOrders && !inDespatch)
                return "Processing";

            if (inOrders && inDespatch && !inInvoice)
            {
                if (productInProductDetails && !productInDespatchDetails)
                    return "Processing";
                if (productInProductDetails && productInDespatchDetails && !productInInvoiceDetails)
                    return "Shipped";
            }

            if (inOrders && inDespatch && inInvoice)
            {
                if (productInProductDetails && productInDespatchDetails && productInInvoiceDetails)
                    return "Delivered";
                return "Shipped";
            }

            return "Unknown";
        }
    }
}