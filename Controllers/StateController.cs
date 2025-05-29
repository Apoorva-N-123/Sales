using System;
using System.Linq;
using System.Web.Mvc;
using Sales.Models;


public class StateController : Controller
{
    private readonly SalesDbContext _context;

    public StateController()
    {
        _context = new SalesDbContext();
    }

    // GET: State
    public ActionResult State()
    {
        return View();
    }

    // POST: State
    [HttpPost]
    [ValidateAntiForgeryToken]
    public JsonResult State(State state)
    {
        if (ModelState.IsValid)
        {
            // Check if the state already exists
            bool isStateExists = _context.States.Any(s => s.StateName == state.StateName);

            if (isStateExists)
            {
                return Json(new { success = false, message = "State already exists." });
            }

            // Add the new state to the database
            _context.States.Add(state);
            _context.SaveChanges();

            return Json(new { success = true, message = "State added successfully!" });
        }

        // If the model state is invalid
        return Json(new { success = false, message = "Error: Unable to add state. Please check your input." });
    }

    // GET: State/Cancel
    public JsonResult Cancel()
    {
        return Json(new { success = true, message = "State registration cancelled." }, JsonRequestBehavior.AllowGet);
    }

    // GET: State/StateEditDelete
    // GET: State/StateEditDelete
    public ActionResult StateEditDelete(int page = 1)
    {
        int pageSize = 5;
        var states = _context.States.OrderBy(s => s.StateId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        int totalStates = _context.States.Count();
        int totalPages = (int)Math.Ceiling((double)totalStates / pageSize);

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(states);
    }


    // POST: State/UpdateState
    [HttpPost]
    public JsonResult UpdateState(int StateId, string StateName, string StateFlag, decimal GSTStateCode)
    {
        var state = _context.States.FirstOrDefault(s => s.StateId == StateId);
        if (state == null)
        {
            return Json(new { success = false });
        }

        state.StateName = StateName;
        state.StateFlag = StateFlag;
        state.GSTStateCode = GSTStateCode;

        _context.SaveChanges();
        return Json(new { success = true });
    }

    // POST: State/DeleteState
    [HttpPost]
    public JsonResult DeleteState(int StateId)
    {
        var state = _context.States.FirstOrDefault(s => s.StateId == StateId);
        if (state == null)
        {
            return Json(new { success = false });
        }

        _context.States.Remove(state);
        _context.SaveChanges();
        return Json(new { success = true });
    }
}
