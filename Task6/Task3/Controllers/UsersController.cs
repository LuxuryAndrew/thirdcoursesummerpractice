using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Task3.Models;

namespace Task3.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private const string Administrator = "admin@admin.com";

        private Entities db = new Entities();
        private FabricContext db1 = new FabricContext();

        private ActionResult CheckAdmin()
        {
            if (!db.AspNetUsers.Find(User.Identity.GetUserId()).UserName.Equals(Administrator))
            {
                return RedirectToAction("Index", "Home");
            }

            return null;
        }

        // GET: Users
        public ActionResult Index()
        {
            var isAdmin = CheckAdmin();

            if (isAdmin != null)
            {
                return isAdmin;
            }

            return View(db.AspNetUsers.ToList());
        }

        // POST: Users
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult ProceedWithAccount(FormCollection form)
        {
            var isAdmin = CheckAdmin();

            if (isAdmin != null)
            {
                return isAdmin;
            }

            string id = Request["item.Id"];
            int actionId = Convert.ToInt32(Request["Action"]);
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            switch (actionId)
            {
            case 0:
                if (aspNetUser.LockoutEndDateUtc == null)
                {
                    aspNetUser.LockoutEndDateUtc = DateTime.MaxValue;
                    ViewBag.Message = "Locked user ";
                }
                else
                {
                    aspNetUser.LockoutEndDateUtc = null;
                    ViewBag.Message = "Unlocked user ";
                }
                db.Entry(aspNetUser).State = EntityState.Modified;
                break;
            case 1:
                db.AspNetUsers.Remove(aspNetUser);
                var fabrics = db1.Fabrics.Where(x => x.UserId == aspNetUser.Id);
                foreach (Fabric fabric in fabrics)
                {
                    db1.Fabrics.Remove(fabric);
                    bool exists = Directory.Exists(Server.MapPath("~/Content/fabrics/" + fabric.Id));
                    if (exists)
                        Directory.Delete(Server.MapPath("~/Content/fabrics/" + fabric.Id), true);
                    }
                db1.SaveChanges();
                ViewBag.Message = "Deleted user ";
                break;
            }
            ViewBag.Message += aspNetUser.Email;
            db.SaveChanges();
            return View(db.AspNetUsers.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
