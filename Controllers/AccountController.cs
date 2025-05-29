using System.Linq;
using System.Web.Mvc;
using Sales.Models;

namespace Sales.Controllers
{
    public class AccountController : Controller
    {
        private SalesDbContext db = new SalesDbContext();

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public ActionResult Register(User user)
        {
            // Check if the username already exists in the database
            if (db.Users.Any(u => u.UserName == user.UserName))
            {
                TempData["Message"] = "User already exists, please choose a different username!";
                return RedirectToAction("Register");
            }

            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                TempData["Message"] = "Registration successful!";
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
            if (user != null)
            {
                
                return RedirectToAction("Success");
            }
            TempData["Message"] = "Invalid login credentials!";
            return View();
        }

        public ActionResult Success()
        {
            ViewBag.Message = "Login successfully";
            return View();
        }
    }
}
