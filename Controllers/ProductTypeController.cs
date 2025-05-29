using System.Linq;
using System.Web.Mvc;
using Sales.Models;

namespace Sales.Controllers
{
    public class ProductTypeController : Controller
    {
        private readonly SalesDbContext _context;

        public ProductTypeController()
        {
            _context = new SalesDbContext();
        }

        // GET: ProductType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductType/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductType productType)
        {
            if (ModelState.IsValid)
            {
                // Check if ProductType Code already exists
                if (_context.ProductTypes.Any(p => p.ProductTypeCode == productType.ProductTypeCode))
                {
                    TempData["PopupMessage"] = "Product Type with this code already exists.";
                    TempData["PopupType"] = "warning";
                    return View(productType);
                }

                // Add new ProductType
                _context.ProductTypes.Add(productType);
                _context.SaveChanges();

                TempData["PopupMessage"] = "Product Type created successfully.";
                TempData["PopupType"] = "success";

                // Return the same page with success message after Save
                return View(productType); // This will stay on the same page (Create.cshtml)
            }

            return View(productType);
        }

        // GET: ProductType/Cancel
        public ActionResult Cancel()
        {
            // Set the message for cancel action
            TempData["PopupMessage"] = "Product Type registration cancelled successfully.";
            TempData["PopupType"] = "info";
            // Redirect back to the Create view so the pop-up appears
            return RedirectToAction("Create");
        }
        public ActionResult ShowSuccessPage()
        {
            return View("~/Views/Account/Success.cshtml");
        }

       

        // GET: ProductType/EditDelete
        public ActionResult ProductTypeEditDelete()
        {
            var productTypes = _context.ProductTypes.ToList();
            return View(productTypes);
        }

        // POST: ProductType/Edit
        [HttpPost]
        public JsonResult EditProductType(int productTypeId, string productTypeCode, string productTypeDescription)
        {
            var productType = _context.ProductTypes.SingleOrDefault(p => p.ProductTypeId == productTypeId);
            if (productType != null)
            {
                productType.ProductTypeCode = productTypeCode;
                productType.ProductTypeDescription = productTypeDescription;

                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        // POST: ProductType/Delete
        [HttpPost]
        public JsonResult DeleteProductType(int productTypeId)
        {
            var productType = _context.ProductTypes.SingleOrDefault(p => p.ProductTypeId == productTypeId);
            if (productType != null)
            {
                _context.ProductTypes.Remove(productType);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
