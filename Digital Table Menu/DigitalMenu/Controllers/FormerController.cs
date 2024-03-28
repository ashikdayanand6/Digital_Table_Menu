using DigitalMenu.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalMenu.Controllers
{
    public class FormerController : Controller
    {
        DigitalMenuEntities1 obj = new DigitalMenuEntities1();
        // GET: Former
        public ActionResult FormerHome()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        public ActionResult GetProdsList()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            string em = Session["User"].ToString();
            int id = (from s in obj.tblSellers where s.EmailId == em select s.SelId).SingleOrDefault();
            var res = (from p in obj.tblAdminProducts where p.SelId == id select p).ToList();
            ViewBag.Res = res;

            return View();
        }

        public ActionResult GetProID(int id)
        {
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$(document).ready(function() {$('#myModal').modal('show');});", true);
            Session["AdProID"] = id;

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActProductList()
        {
            int id = int.Parse(Session["AdProID"].ToString());
            string rep = Request.Form["rep"];
            tblAdminProduct ad = obj.tblAdminProducts.Where(x => x.AdPdId == id).SingleOrDefault();
            var dt = DateTime.Now.ToShortDateString();
            ad.Replay = rep;
            ad.RpDate = dt;
            obj.SaveChanges();

            return RedirectToAction(nameof(GetProdsList));
        }

        public ActionResult FormerProfile()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            string em = Session["User"].ToString();
            int uID = (from u in obj.tblSellers where u.EmailId == em && u.Type == "Former" select u.SelId).SingleOrDefault();
            tblSeller res = (from u in obj.tblSellers where u.SelId == uID select u).FirstOrDefault();

            return View(res);
        }

        [HttpPost]
        public ActionResult FormerProfile(tblSeller usr)
        {
            tblSeller us = obj.tblSellers.Where(x => x.SelId == usr.SelId).FirstOrDefault();
            us.SellerName = usr.SellerName;
            us.Address = usr.Address;
            us.City = usr.City;
            us.Mobile = usr.Mobile;
            obj.SaveChanges();
            TempData["UpdateProf"] = "Former Profile Update Sucessfully!";

            return RedirectToAction(nameof(FormerProfile));
        }

        public ActionResult FormerPassword()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        [HttpPost]
        public ActionResult FormerPassword(string st)
        {
            string old = Request.Form["oldPas"];
            string newP = Request.Form["newPas"];
            string em = Session["User"].ToString();
            int uID = (from u in obj.tblSellers where u.EmailId == em select u.SelId).SingleOrDefault();
            string pas = (from u in obj.tblSellers where u.SelId == uID select u.Password).SingleOrDefault();

            if (pas.Equals(old))
            {
                tblSeller usr = obj.tblSellers.Where(x => x.SelId == uID).FirstOrDefault();
                usr.Password = newP;
                obj.SaveChanges();
                TempData["PasSuc"] = "Password Updated Sucessfully!";
            }
            else
            {
                TempData["PasNtMt"] = "Incorrect Old Password!";
            }

            return RedirectToAction(nameof(FormerPassword));
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}