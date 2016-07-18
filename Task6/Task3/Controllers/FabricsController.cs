using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Task3.Models;
using System.Web.Http.Cors;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Task3.Controllers
{
    [Authorize]
    public class FabricsController : Controller
    {
        private FabricContext db = new FabricContext();

        // GET: Fabrics
        public ActionResult Index()
        {
            string userId = IdentityExtensions.GetUserId(User.Identity);
            return View(db.Fabrics.Where(c => c.UserId == userId).ToList());
        }

        // POST: Fabrics
        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            int numericId = Convert.ToInt32(id);
            Fabric fabric = db.Fabrics.Find(numericId);
            db.Fabrics.Remove(fabric);
            db.SaveChanges();
            bool exists = Directory.Exists(Server.MapPath("~/Content/fabrics/" + id));
            if (exists)
                Directory.Delete(Server.MapPath("~/Content/fabrics/" + id), true);
            string userId = IdentityExtensions.GetUserId(User.Identity);
            ViewBag.Message = "Collage has been deleted!";
            return View(db.Fabrics.Where(c => c.UserId == userId).ToList());
        }

        // GET: Fabrics/Create
        public ActionResult Create()
        {
            string userId = IdentityExtensions.GetUserId(User.Identity);
            Fabric fabric = new Fabric() { UserId = userId };
            db.Fabrics.Add(fabric);
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = fabric.Id });
        }

        // GET: Fabrics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string userId = IdentityExtensions.GetUserId(User.Identity);
            Fabric fabric = db.Fabrics.Find(id);
            if (userId != fabric.UserId)
            {
                return RedirectToAction("Index");
            }
            if (fabric == null)
            {
                return HttpNotFound();
            }
            return View(fabric);
        }

        // POST: Fabrics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,JSONContent")] Fabric fabric, string thumbnail)
        {
            int id = fabric.Id;
            string folder = Server.MapPath("~/Content/fabrics/" + id);
            bool exists = Directory.Exists(folder);
            if (!exists)
                Directory.CreateDirectory(folder);
            var base64Data = Regex.Match(thumbnail, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);
            using (var stream = new MemoryStream(binData))
            {
                Bitmap thumbnailImage = new Bitmap(stream);
                thumbnailImage.Save(folder + '/' + "thumbnail.png");
            }

            fabric.UserId = IdentityExtensions.GetUserId(User.Identity);
            if (ModelState.IsValid)
            {
                db.Entry(fabric).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fabric);
        }

        public string UploadImage(string url, string path)
        {
            string id = path.Substring(path.LastIndexOf('/'));
            string folder = Server.MapPath("~/Content/fabrics" + id);
            bool exists = Directory.Exists(folder);
            if (!exists)
                Directory.CreateDirectory(folder);
            string filePath = folder + '/' + Path.GetFileName(url);
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(url, filePath);
            }
            return Request.Url.GetLeftPart(UriPartial.Authority) + "/Content/fabrics" + id + '/' + Path.GetFileName(url);
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
