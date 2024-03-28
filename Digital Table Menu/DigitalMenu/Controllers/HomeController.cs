using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using DigitalMenu.Models.Entity;

namespace DigitalMenu.Controllers
{
    public class HomeController : Controller
    {
        DigitalMenuEntities1 obj = new DigitalMenuEntities1();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string st)
        {
            string tp = Request.Form["typ"];
            string em = Request.Form["email"];
            string ps = Request.Form["password"];

            if (tp == "Admin")
            {
                Session["User"] = em;
                int cnt = (from a in obj.tblAdmins where a.AdminId == em && a.Password == ps select a).Count();
                if (cnt > 0)
                {
                    return RedirectToAction("AdminHome", "Admin");
                }
            }
            else if (tp == "Chef")
            {
                int cnt = (from a in obj.tblChefs where a.EmailId == em && a.Password == ps select a).Count();
                if (cnt > 0)
                {
                    Session["User"] = em;
                    return RedirectToAction("ChefHome", "Chef");
                }
            }
            else if (tp == "User")
            {
                int cnt = (from a in obj.tblUsers where a.EmailId == em && a.Password == ps select a).Count();
                if (cnt > 0)
                {
                    Session["User"] = em;
                    return RedirectToAction("UserHome", "User");
                }
            }
            else if (tp == "Retailer")
            {
                int cnt = (from a in obj.tblSellers where a.EmailId == em && a.Password == ps && a.Type == "Retailer" select a).Count();
                if (cnt > 0)
                {
                    Session["User"] = em;
                    return RedirectToAction("RetailerHome", "Seller");
                }
            }
            else if (tp == "Former")
            {
                int cnt = (from a in obj.tblSellers where a.EmailId == em && a.Password == ps && a.Type == "Former" select a).Count();
                if (cnt > 0)
                {
                    Session["User"] = em;
                    return RedirectToAction("FormerHome", "Former");
                }
            }
            else
            {
                Response.Write("<script>alert('Invalied Login!')</script>");
            }

            return View();
        }

        public ActionResult Log()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Log(string st)
        {
            string tp = Request.Form["typ"];
            string em = Request.Form["email"];
            string ps = Request.Form["password"];

            if (tp == "Admin")
            {
                int cnt = (from a in obj.tblAdmins where a.AdminId == em && a.Password == ps select a).Count();
                if (cnt > 0)
                {
                    Session["User"] = em;
                    return RedirectToAction("AdminHome", "Admin");
                }
            }
            else if (tp == "Chef")
            {
                int cnt = (from a in obj.tblChefs where a.EmailId == em && a.Password == ps select a).Count();
                if (cnt > 0)
                {
                    Session["User"] = em;
                    return RedirectToAction("ChefProducts", "Chef");
                }
            }
            else if (tp == "User")
            {
                int cnt = (from a in obj.tblUsers where a.EmailId == em && a.Password == ps select a).Count();
                if (cnt > 0)
                {
                    Session["User"] = em;
                    return RedirectToAction("UserHome", "User");
                }
            }
            else if (tp == "Retailer")
            {
                int cnt = (from a in obj.tblSellers where a.EmailId == em && a.Password == ps && a.Type == "Retailer" select a).Count();
                if (cnt > 0)
                {
                    Session["User"] = em;
                    return RedirectToAction("GetProdList", "Seller");
                }
            }
            else if (tp == "Former")
            {
                int cnt = (from a in obj.tblSellers where a.EmailId == em && a.Password == ps && a.Type == "Former" select a).Count();
                if (cnt > 0)
                {
                    Session["User"] = em;
                    return RedirectToAction("GetProdsList", "Former");
                }
            }
            else
            {
                Response.Write("<script>alert('Invalied Login!')</script>");
            }

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(HttpPostedFileBase file)
        {
            string nm = Request.Form["name"];
            string ad = Request.Form["address"];
            string ct = Request.Form["city"];
            string mb = Request.Form["mobile"];
            string em = Request.Form["email"];
            string ps = Request.Form["password"];
            string tp = Request.Form["typ"];
            //string nm = Request.Form[""];

            if (tp == "User")
            {
                string name = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                string path = "~/Content/User/" + name + DateTime.Now.ToString("yymmddssfff") + ext;
                file.SaveAs(Server.MapPath(path));

                Models.Entity.tblUser usr = new Models.Entity.tblUser();
                usr.Name = nm; usr.Address = ad; usr.City = ct; usr.Mobile = mb; usr.EmailId = em; usr.Password = ps; usr.Type = tp;
                usr.Image = path;
                obj.tblUsers.Add(usr);
                obj.SaveChanges();
                TempData["RegisterSc"] = "Register Sucessfully!";
            }
            else
            {
                tblSeller sl = new tblSeller();
                sl.SellerName = nm; sl.Address = ad; sl.City = ct; sl.Mobile = mb; sl.EmailId = em; sl.Password = ps; sl.Type = tp;
                obj.tblSellers.Add(sl);
                obj.SaveChanges();
                TempData["RegisterSc"] = "Register Sucessfully!";
            }

            return RedirectToAction(nameof(Register));
        }
    }
}