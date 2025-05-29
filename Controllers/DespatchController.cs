using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Sales.Models;

namespace Sales.Controllers
{
    public class DespatchController : Controller
    {
        private readonly SalesDbContext db = new SalesDbContext();

        public ActionResult Register()
        {
            var customers = db.Customers.Select(c => new
            {
                c.CustomerName
            }).ToList();

            ViewBag.Customers = new SelectList(customers, "CustomerName", "CustomerName");

            return View(new Despatch { DespatchAdviceDate = DateTime.Now });
        }
        public JsonResult GetCustomerGSTIN(string customerName)
        {
            var customer = db.Customers.FirstOrDefault(c => c.CustomerName == customerName);

            if (customer != null)
            {
                return Json(new { success = true, gstin = customer.GSTIN }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Register(Despatch despatch)
        {
            if (string.IsNullOrEmpty(despatch.CustomerName))
            {
                return Json(new { success = false, message = "Customer Name is required." });
            }
            if (despatch == null || despatch.DespatchDetails == null || despatch.DespatchDetails.Count == 0)
            {
                return Json(new { success = false, message = "Invalid despatch data." });
            }

            // Validate DespatchQuantity
            foreach (var detail in despatch.DespatchDetails)
            {
                var product = db.ProductDetails.FirstOrDefault(p => p.ProductDetailId == detail.DespatchDetailId);
                if (product != null && detail.DespatchQuantity > product.OrderQuantity)
                {
                    return Json(new { success = false, message = $"Despatch Quantity for {product.ProductCode} must be less than or equal to {product.OrderQuantity}." });
                }
            }

            try
            {
                using (var db = new SalesDbContext())
                {
                    db.Despatch.Add(despatch);
                    db.SaveChanges();
                }

                return Json(new { success = true, message = "Despatch saved successfully!" });
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessage = string.Join(", ", ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => e.ErrorMessage));

                return Json(new { success = false, message = "Error: " + errorMessage });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return Json(new { success = false, message = "Error: " + errorMessage });
            }
        }


        public ActionResult Cancel()
        {
            return Json(new { success = true, message = "Despatch registration cancelled" });
        }

        public JsonResult GetOrders()
        {
            var orders = db.Orders
                .Select(o => new
                {
                    OrderId = o.OrderId,
                    OrderReferenceNumber = o.OrderReferenceNumber,
                    OrderDate = o.OrderDate
                })
                .ToList();

            return Json(orders, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductsByOrder(int orderId)
        {
            var products = db.ProductDetails
                .Where(p => p.OrderId == orderId)
                .Select(p => new
                {
                    ProductDetailId = p.ProductDetailId,
                    ProductCode = p.ProductCode,
                    ProductDescription = p.ProductDescription,
                    OrderQuantity = p.OrderQuantity,
                    PackType = p.PackType,
                    Rate = p.Rate
                })
                .ToList();

            return Json(products, JsonRequestBehavior.AllowGet);
        }

        // Show all Despatch records in grid format
        public ActionResult EditDeleteDespatch()
        {
            var despatchList = db.Despatch.ToList();
            return View(despatchList);
        }

        // Load Despatch details for editing
        public ActionResult EditDespatch(int id)
        {
            var despatch = db.Despatch.FirstOrDefault(d => d.DespatchAdviceNumber == id);
            if (despatch == null)
            {
                return HttpNotFound();
            }

            var despatchDetails = db.DespatchDetails.Where(d => d.DespatchAdviceNumber == id).ToList();
            ViewBag.DespatchDetails = despatchDetails;
            return View(despatch);
        }

        [HttpPost]
        public JsonResult EditDespatch(Despatch model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();

                // Log errors for debugging
                System.Diagnostics.Debug.WriteLine("ModelState Errors: " + string.Join(", ", errors));

                return Json(new { success = false, message = "Invalid data", errors });
            }

            if (model.DespatchDetails == null || !model.DespatchDetails.Any())
            {
                return Json(new { success = false, message = "At least one product must be added!" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDespatch = db.Despatch.Include("DespatchDetails")
                        .FirstOrDefault(d => d.DespatchAdviceNumber == model.DespatchAdviceNumber);

                    if (existingDespatch != null)
                    {
                        existingDespatch.DespatchAdviceDate = model.DespatchAdviceDate;
                        existingDespatch.CustomerName = model.CustomerName;
                        existingDespatch.GSTIN = model.GSTIN;
                        existingDespatch.DeliveryAddress = model.DeliveryAddress;
                        existingDespatch.VehicleNumber = model.VehicleNumber;

                        // Update existing records and add new ones
                        var existingDetails = existingDespatch.DespatchDetails.ToList();
                        foreach (var detail in model.DespatchDetails)
                        {
                            var existingDetail = existingDetails.FirstOrDefault(d => d.OrderReferenceNumber == detail.OrderReferenceNumber);

                            if (existingDetail != null)
                            {
                                // Update existing record
                                existingDetail.ProductCode = detail.ProductCode;
                                existingDetail.ProductDescription = detail.ProductDescription;
                                existingDetail.OrderQuantity = detail.OrderQuantity;
                                existingDetail.PackType = detail.PackType;
                                existingDetail.Rate = detail.Rate;
                                existingDetail.DespatchQuantity = detail.DespatchQuantity;
                            }
                            else
                            {
                                // Add new detail
                                existingDespatch.DespatchDetails.Add(new DespatchDetails
                                {
                                    DespatchAdviceNumber = existingDespatch.DespatchAdviceNumber, // Ensure foreign key is set
                                    OrderReferenceNumber = detail.OrderReferenceNumber,
                                    ProductCode = detail.ProductCode,
                                    ProductDescription = detail.ProductDescription,
                                    OrderQuantity = detail.OrderQuantity,
                                    PackType = detail.PackType,
                                    Rate = detail.Rate,
                                    DespatchQuantity = detail.DespatchQuantity
                                });
                            }
                        }

                        db.SaveChanges();
                        return Json(new { success = true, message = "Despatch details updated successfully!" });
                    }

                    return Json(new { success = false, message = "Despatch not found!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error: " + ex.Message });
                }
            }

            return Json(new { success = false, message = "Invalid data!" });
        }

        [HttpPost]
        public ActionResult DeleteDespatch(int id)
        {
            var despatch = db.Despatch.FirstOrDefault(d => d.DespatchAdviceNumber == id);
            if (despatch == null)
            {
                return Json(new { success = false, message = "Despatch record not found." });
            }

            var despatchDetails = db.DespatchDetails.Where(d => d.DespatchAdviceNumber == id).ToList();
            db.DespatchDetails.RemoveRange(despatchDetails);
            db.Despatch.Remove(despatch);
            db.SaveChanges();

            return Json(new { success = true, message = "Despatch deleted successfully!" });
        }

    }
}
