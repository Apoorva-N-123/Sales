using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Sales.Models;

namespace Sales.Controllers
{
    public class InvoiceController : Controller
    {
        private SalesDbContext db = new SalesDbContext();

        public ActionResult InvoiceRegister()
        {
            var despatchList = db.Despatch
                                 .Select(d => new
                                 {
                                     d.DespatchAdviceNumber
                                 })
                                 .ToList();

            ViewBag.DespatchList = new SelectList(despatchList, "DespatchAdviceNumber", "DespatchAdviceNumber");
            return View();
        }

        [HttpPost]
        public ActionResult SaveInvoice(Invoice invoice, List<InvoiceDetails> invoiceDetails)
        {
            if (invoice == null)
            {
                return Json(new { success = false, message = "Invoice data is missing!" });
            }

            // Debugging: Check what data is being received
            Console.WriteLine("Received Invoice: " + JsonConvert.SerializeObject(invoice));
            Console.WriteLine("Received InvoiceDetails: " + JsonConvert.SerializeObject(invoiceDetails));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Validation Errors", errors });
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Save Invoice
                    db.Invoice.Add(invoice);
                    db.SaveChanges();
                    int invoiceNumber = invoice.InvoiceNumber;

                    // Ensure InvoiceDetails are not null
                    if (invoiceDetails != null && invoiceDetails.Any())
                    {
                        foreach (var detail in invoiceDetails)
                        {
                            detail.InvoiceNumber = invoiceNumber; // Assign foreign key
                            db.InvoiceDetails.Add(detail);
                        }
                        db.SaveChanges();
                    }

                    transaction.Commit();
                    return Json(new { success = true, message = "Invoice Saved Successfully" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Error saving invoice!", error = ex.Message });
                }
            }
        }

        public JsonResult GetDespatchDetails(int despatchId)
        {
            var despatch = db.Despatch.Where(d => d.DespatchAdviceNumber == despatchId)
                                        .Select(d => new
                                        {
                                            d.CustomerName,
                                            d.GSTIN,
                                            d.DeliveryAddress,
                                            d.VehicleNumber
                                        })
                                        .FirstOrDefault();
            return Json(despatch, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerPaymentDetails(string customerName)
        {
            var orderDetails = db.Orders.Where(o => o.CustomerName == customerName)
                                        .Select(o => new
                                        {
                                            o.PayMode,
                                            o.PaymentTerms
                                        })
                                        .FirstOrDefault();
            return Json(orderDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDespatchProducts(int despatchId)
        {
            using (var db = new SalesDbContext()) // Ensure this matches your DbContext name
            {
                var products = db.DespatchDetails // Ensure this is the correct table
                    .Where(p => p.DespatchAdviceNumber == despatchId)
                    .Select(p => new
                    {
                        p.ProductCode,
                        p.ProductDescription,
                        p.OrderQuantity,
                        p.PackType,
                        p.DespatchQuantity,
                        p.Rate
                    }).ToList();

                return Json(products, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult InvoiceDelete()
        {
            var invoices = db.Invoice.ToList(); // Fetch all invoices from the database
            return View(invoices);
        }

        [HttpPost]
        public JsonResult DeleteInvoice(int invoiceNumber)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var invoice = db.Invoice.Find(invoiceNumber);
                    if (invoice == null)
                    {
                        return Json(new { success = false, message = "Invoice not found!" });
                    }

                    // Delete related invoice details
                    var invoiceDetails = db.InvoiceDetails.Where(d => d.InvoiceNumber == invoiceNumber);
                    db.InvoiceDetails.RemoveRange(invoiceDetails);
                    db.Invoice.Remove(invoice);
                    db.SaveChanges();

                    transaction.Commit();
                    return Json(new { success = true, message = "Invoice deleted successfully!" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Error deleting invoice!", error = ex.Message });
                }
            }
        }

        public JsonResult GetInvoiceDetails(int invoiceNumber)
        {
            var invoice = db.Invoice
                            .Where(i => i.InvoiceNumber == invoiceNumber)
                            .Select(i => new
                            {
                                i.InvoiceNumber,
                                i.InvoiceDate,
                                i.DespatchAdviceNumber,
                                i.CustomerName,
                                i.GSTIN,
                                i.DeliveryAddress,
                                i.VehicleNumber,
                                i.PayMode,
                                i.PaymentTerms,
                                i.BankName,
                                i.TotValue,
                                i.TotGST,
                                i.FreightAmount,
                                i.OtherAmount,
                                i.RoundOfAmount,
                                i.TotInvoiceAmount
                            }).FirstOrDefault();

            if (invoice == null)
            {
                return Json(new { success = false, message = "Invoice not found." }, JsonRequestBehavior.AllowGet);
            }

            var invoiceProducts = db.InvoiceDetails
                                    .Where(d => d.InvoiceNumber == invoiceNumber)
                                    .Select(d => new
                                    {
                                        d.ProductCode,
                                        d.ProductDescription,
                                        d.OrderQuantity,
                                        d.PackType,
                                        d.DespatchQuantity,
                                        d.InvoiceQuantity,
                                        d.Rate,
                                        d.Val,
                                        d.TaxPercentage,
                                        d.TaxAmount,
                                        d.InvoiceAmount
                                    }).ToList();

            return Json(new { success = true, invoice, invoiceProducts }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InvoiceReport()
        {
            return View();
        }

        public JsonResult GetInvoiceReport(DateTime fromDate, DateTime toDate)
        {
            var invoiceData = (from i in db.Invoice
                               join d in db.InvoiceDetails on i.InvoiceNumber equals d.InvoiceNumber
                               where i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate
                               select new
                               {
                                   i.CustomerName,
                                   i.GSTIN,
                                   d.ProductCode,
                                   d.ProductDescription,
                                   d.InvoiceQuantity,
                                   d.Rate,
                                   d.Val,
                                   d.TaxPercentage,
                                   d.TaxAmount,
                                   d.InvoiceAmount,
                                   i.InvoiceDate, // Do not format inside LINQ
                                   i.FreightAmount,
                                   i.OtherAmount,
                                   i.RoundOfAmount,
                                   i.TotInvoiceAmount
                               }).ToList() // Execute the query first
                               .Select(x => new
                               {
                                   x.CustomerName,
                                   x.GSTIN,
                                   x.ProductCode,
                                   x.ProductDescription,
                                   x.InvoiceQuantity,
                                   x.Rate,
                                   x.Val,
                                   x.TaxPercentage,
                                   x.TaxAmount,
                                   x.InvoiceAmount,
                                   InvoiceDate = x.InvoiceDate.ToString("yyyy-MM-dd"), // Now format the date
                                   x.FreightAmount,
                                   x.OtherAmount,
                                   x.RoundOfAmount,
                                   x.TotInvoiceAmount
                               }).ToList(); // Convert to list again after formatting

            return Json(new { invoiceData }, JsonRequestBehavior.AllowGet);
        }

    }
}
