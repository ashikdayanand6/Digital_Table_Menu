using DigitalMenu.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace DigitalMenu.Controllers
{
    public class ChefController : Controller
    {
        DigitalMenuEntities1 obj = new DigitalMenuEntities1();
        // GET: Chef
        public ActionResult ChefHome()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        public ActionResult ChefProducts()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            ViewBag.Res = (from c in obj.tblChefProducts select c).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChefProducts(tblChefProduct cp)
        {
            string em = Session["User"].ToString();
            int cId = (from c in obj.tblChefs where c.EmailId == em select c.ChId).SingleOrDefault();

            cp.ChId = cId;
            cp.PstDate = DateTime.Now.ToShortDateString();
            cp.Stataus = "Pending";
            //var dateAsString = DateTime.Now.ToString("yyyy-MM-dd");
            obj.tblChefProducts.Add(cp);
            obj.SaveChanges();
            TempData["InsertChPro"] = "Products List Inserted Sucessfully!";

            return RedirectToAction(nameof(ChefProducts));
        }

        public ActionResult ChefPrepFood()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            ViewBag.Res = obj.tblBooks.Where(d => d.Status == "Pending").Include(d => d.tblFood).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult ChefPrepFood(string id)
        {
            int ids = int.Parse(id);
            tblBook bk = obj.tblBooks.Where(x => x.BkId == ids).FirstOrDefault();
            bk.DisDate = DateTime.Now;
            bk.Status = "Finished";
            obj.SaveChanges();

            return RedirectToAction(nameof(ChefPrepFood));
        }

        public ActionResult CherResult(int id)
        {
            int ids = id;
            tblBook bk = obj.tblBooks.Where(x => x.BkId == ids).FirstOrDefault();
            bk.DisDate = DateTime.Now;
            bk.Status = "Finished";
            obj.SaveChanges();

            return RedirectToAction(nameof(ChefPrepFood));
        }

        public ActionResult ChefProfile()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            string em = Session["User"].ToString();
            int cId = (from c in obj.tblChefs where c.EmailId == em select c.ChId).SingleOrDefault();
            tblChef ch = (from c in obj.tblChefs where c.ChId == cId select c).FirstOrDefault();

            return View(ch);
        }

        [HttpPost]
        public ActionResult ChefProfile(tblChef ch)
        {
            obj.tblChefs.Add(ch);
            obj.SaveChanges();
            TempData["CheffPro"] = "Chef Profile Updated!";

            return RedirectToAction(nameof(ChefProfile));
        }

        public ActionResult ChefPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChefPassword(string st)
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            string old = Request.Form["oldPas"];
            string newP = Request.Form["newPas"];
            string em = Session["User"].ToString();
            int uID = (from u in obj.tblChefs where u.EmailId == em select u.ChId).SingleOrDefault();
            string pas = (from u in obj.tblChefs where u.ChId == uID select u.Password).SingleOrDefault();

            if (pas.Equals(old))
            {
                tblChef usr = obj.tblChefs.Where(x => x.ChId == uID).FirstOrDefault();
                usr.Password = newP;
                obj.SaveChanges();
                TempData["PasSuc"] = "Password Updated Sucessfully!";
            }
            else
            {
                TempData["PasNtMt"] = "Incorrect Old Password!";
            }

            return RedirectToAction(nameof(ChefPassword));
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}