using Sales.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace Sales.Controllers
{
    public class ProductController : Controller
    {
        private SalesDbContext db = new SalesDbContext();

        // GET: Product/Create
        public ActionResult Create()
        {
            // Fetch distinct Count data for the dropdown
            var countList = db.Counts
                              .Select(c => new { c.CountCode, c.CountDescription })
                              .Distinct()
                              .ToList();
            ViewBag.CountList = new SelectList(countList, "CountCode", "CountDescription");

            // Fetch distinct HSN data for the dropdown
            var hsnList = db.HSNs
                            .Select(h => new { h.HSNCode, h.HSNDescription })
                            .Distinct()
                            .ToList();
            ViewBag.HSNList = new SelectList(hsnList, "HSNCode", "HSNDescription");

            // Fetch distinct ProductType data for the dropdown
            var productTypeList = db.ProductTypes
                                    .Select(pt => new { pt.ProductTypeCode, pt.ProductTypeDescription })
                                    .Distinct()
                                    .ToList();
            ViewBag.ProductTypeList = new SelectList(productTypeList, "ProductTypeCode", "ProductTypeDescription");

            // Fetch distinct Variety data for the dropdown
            var varietyList = db.Varieties
                                .Select(v => new { v.VarietyCode, v.VarietyDescription })
                                .Distinct()
                                .ToList();
            ViewBag.VarietyList = new SelectList(varietyList, "VarietyCode", "VarietyDescription");

            // Fetch distinct WH data for the dropdown
            var whList = db.WHs
                           .Select(w => new { w.WHCode, w.WHDescription })
                           .Distinct()
                           .ToList();
            ViewBag.WHList = new SelectList(whList, "WHCode", "WHDescription");

            // Fetch distinct KCS data for the dropdown
            var kcsList = db.KCSs
                            .Select(k => new { k.KCSCode, k.KCSDescription })
                            .Distinct()
                            .ToList();
            ViewBag.KCSList = new SelectList(kcsList, "KCSCode", "KCSDescription");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                // Check if the product code already exists
                var existingProduct = db.Products.FirstOrDefault(p => p.ProductCode == product.ProductCode);
                if (existingProduct != null)
                {
                    return Json(new { success = false, message = "Product code already exists!" });
                }

                // Ensure CountDescription is fetched correctly
                var count = db.Counts.FirstOrDefault(c => c.CountCode == product.CountCode);
                if (count != null)
                {
                    product.CountDescription = count.CountDescription;
                }

                // Ensure HSNDescription is fetched correctly
                var hsn = db.HSNs.FirstOrDefault(h => h.HSNCode == product.HSNCode);
                if (hsn != null)
                {
                    product.HSNDescription = hsn.HSNDescription;
                }

                // Ensure ProductTypeDescription is fetched correctly
                var productType = db.ProductTypes.FirstOrDefault(pt => pt.ProductTypeCode == product.ProductTypeCode);
                if (productType != null)
                {
                    product.ProductTypeDescription = productType.ProductTypeDescription;
                }

                // Ensure VarietyDescription is fetched correctly from the database
                var variety = db.Varieties.FirstOrDefault(v => v.VarietyCode == product.VarietyCode);
                if (variety != null)
                {
                    product.VarietyDescription = variety.VarietyDescription;
                }

                // Ensure WHDescription is fetched correctly from the database
                var wh = db.WHs.FirstOrDefault(w => w.WHCode == product.WHCode);
                if (wh != null)
                {
                    product.WHDescription = wh.WHDescription;
                }

                // Ensure KCSDescription is fetched correctly from the database
                var kcs = db.KCSs.FirstOrDefault(k => k.KCSCode == product.KCSCode);
                if (kcs != null)
                {
                    product.KCSDescription = kcs.KCSDescription;
                }

                // Save the product to the database
                db.Products.Add(product);
                db.SaveChanges();

                return Json(new { success = true, message = "Product registered successfully!" });
            }

            return Json(new { success = false, message = "Invalid product data!" });
        }


        public ActionResult GetHSNs()
        {
            var hsns = db.HSNs.Select(h => new { h.HSNCode, h.HSNDescription }).Distinct().ToList();
            return Json(hsns, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCounts()
        {
            var counts = db.Counts.Select(c => new { c.CountCode, c.CountDescription }).Distinct().ToList();
            return Json(counts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductTypes()
        {
            var productTypes = db.ProductTypes
                                 .Select(pt => new { pt.ProductTypeCode, pt.ProductTypeDescription })
                                 .Distinct()
                                 .ToList();
            return Json(productTypes, JsonRequestBehavior.AllowGet);
        }

        // Get Variety data
        public ActionResult GetVarieties()
        {
            var varieties = db.Varieties
                              .Select(v => new { v.VarietyCode, v.VarietyDescription })
                              .Distinct()
                              .ToList();
            return Json(varieties, JsonRequestBehavior.AllowGet);
        }

        // Get WH data
        public ActionResult GetWHs()
        {
            var whs = db.WHs
                        .Select(w => new { w.WHCode, w.WHDescription })
                        .Distinct()
                        .ToList();
            return Json(whs, JsonRequestBehavior.AllowGet);
        }

        // Get KCS data
        public ActionResult GetKCS()
        {
            var kcs = db.KCSs
                        .Select(k => new { k.KCSCode, k.KCSDescription })
                        .Distinct()
                        .ToList();
            return Json(kcs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductEditDelete(int page = 1)
        {

            int pageSize = 4; // Number of cities per page
            int totalProducts = db.Products.Count(); // Total number of cities in the database
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = db.Products
            .OrderBy(c => c.ProductId)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(products);
        }

        // Edit product (GET)
        public ActionResult ProductEdit(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // Edit product (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductEdit1(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                
            }
            return RedirectToAction("ProductEditDelete");

        }

        // Delete product
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
                return Json(new { success = true, message = "Product deleted successfully!" });
            }

            return Json(new { success = false, message = "Failed to delete product!" });
        }
    }
}
