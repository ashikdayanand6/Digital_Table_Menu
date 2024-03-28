using DigitalMenu.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity;

namespace DigitalMenu.Controllers
{
    public class UserController : Controller
    {
        DigitalMenuEntities1 obj = new DigitalMenuEntities1();
        // GET: User
        public ActionResult UserHome()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        public ActionResult Menu()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            ViewBag.FT = new SelectList(obj.tblFoodTypes, "FdId", "FoodType");

            if (Session["FoodTp"] != null)
            {
                try
                {
                    int id = int.Parse(Session["FoodTp"].ToString());
                    int tp = int.Parse(TempData["FD"].ToString());
                    ViewBag.Res = (from f in obj.tblFoods where f.FdId == id && f.Status == "Available" select f).ToList();
                }
                catch
                {
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult Menu(string st)
        {
            string tpId = Request.Form["type"];
            TempData["FD"] = tpId;
            Session["FoodTp"] = tpId;

            return RedirectToAction(nameof(Menu));
        }

        public ActionResult GetFdId(int id)
        {
            TempData["FdID"] = id;
            string em = Session["User"].ToString();
            if (id != 0)
            {
                tblCart crt = new tblCart();
                int uID = (from u in obj.tblUsers where u.EmailId == em select u.UsrId).SingleOrDefault();
                crt.UsrId = uID;
                crt.FId = id;
                crt.Status = "Cart";
                obj.tblCarts.Add(crt);
                obj.SaveChanges();
                TempData["AddCart"] = "Food Added to Cart!";
            }

            return RedirectToAction(nameof(Menu));
        }

        public ActionResult Cart()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            DataTable dt = new DataTable();
            dt.Columns.Add("FId");
            dt.Columns.Add("Name");
            dt.Columns.Add("Price");
            dt.Columns.Add("Image");

            string em = Session["User"].ToString();
            int uID = (from u in obj.tblUsers where u.EmailId == em select u.UsrId).SingleOrDefault();
            var fd = (from c in obj.tblCarts where c.UsrId == uID && c.Status == "Cart" select c.FId).ToList();
            for(int i = 0; i < fd.Count; i++)
            {
                int id = int.Parse(fd[i].ToString());
                var fds = (from f in obj.tblFoods where f.FId == id select f).ToList(); 
                foreach(var r in fds)
                {
                    dt.Rows.Add(r.FId, r.Name, r.Price, r.Image);
                }
            }
            int pr = 0;
            for(int i=0;i<dt.Rows.Count;i++)
            {
                pr = pr + int.Parse(dt.Rows[i]["Price"].ToString());
            }
            List<tblFood> FdList = new List<tblFood>();
            foreach (DataRow dr in dt.Rows)
            {
                FdList.Add(
                    new tblFood
                    {
                        FId = Convert.ToInt32(dr["FId"]),
                        Name = Convert.ToString(dr["Name"]),
                        Price = Convert.ToInt32(dr["Price"]),
                        Image = Convert.ToString(dr["Image"])
                    });
            }
            ViewBag.Res = FdList;
            TempData["total"] = pr;

            var FinRes = obj.tblCarts.Where(x => x.UsrId == uID && x.Status == "Cart").Include(x => x.tblFood).ToList();
            ViewBag.FinRes = FinRes;

            return View();
        }
        string conn = ConfigurationManager.ConnectionStrings["FoodObj"].ConnectionString;
        [HttpPost]
        public ActionResult Cart(string[] hdnFlag,string[] txtPric,string[] txtQt)
        {
            string em = Session["User"].ToString();
            int uID = (from u in obj.tblUsers where u.EmailId == em select u.UsrId).SingleOrDefault();

            for (int i = 0; i < hdnFlag.Length; i++)
            {
                int fd = int.Parse(hdnFlag[i].ToString());
                int pr = int.Parse(txtPric[i].ToString());
                int Qt = int.Parse(txtQt[i].ToString());
                string Tabl = Request.Form["tabls"];
                var dt = DateTime.Now.ToString();
                int tt = pr * Qt;

                //using (SqlConnection con = new SqlConnection(conn))
                //{
                //    string Qry = string.Format("insert into tblBook (FId,UsrId,Quantity,OrderDate,TotalPrice,TableNo,Status) values('{0}','{1}','{2}','{3}','{4}','{5}','Pending')", fd, uID, Qt, dt, tt, Tabl);
                //    SqlCommand cmd = new SqlCommand(Qry, con);
                //    con.Open();
                //    cmd.ExecuteNonQuery();
                //    con.Close();
                //}

                tblBook bk = new tblBook();
                bk.FId = fd;
                bk.UsrId = uID;
                bk.Quantity = Qt;
                bk.OrderDate = dt;
                bk.TableNo = int.Parse(Tabl);
                bk.TotalPrice = pr * Qt;
                bk.Status = "Pending";
                obj.tblBooks.Add(bk);
                obj.SaveChanges();

                tblCart ct = obj.tblCarts.Where(x => x.UsrId == uID && x.FId == fd && x.Status == "Cart").FirstOrDefault();
                ct.Status = "Booked";
                obj.SaveChanges();
            }

            return RedirectToAction(nameof(Cart));
        }

        public ActionResult CancelCart(int id, int Fid)
        {
            string em = Session["User"].ToString();
            int uID = (from u in obj.tblUsers where u.EmailId == em select u.UsrId).SingleOrDefault();

            tblCart ct = obj.tblCarts.Where(x => x.CtId==id && x.Status == "Cart").FirstOrDefault();
            ct.Status = "Cancelled";
            obj.SaveChanges();

            return RedirectToAction(nameof(Cart));
        }

        public ActionResult UserProfile()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            string em = Session["User"].ToString();
            int uID = (from u in obj.tblUsers where u.EmailId == em select u.UsrId).SingleOrDefault();
            tblUser res = (from u in obj.tblUsers where u.UsrId == uID select u).FirstOrDefault();

            return View(res);
        }

        [HttpPost]
        public ActionResult UserProfile(tblUser usr)
        {
            tblUser us = obj.tblUsers.Where(x => x.UsrId == usr.UsrId).FirstOrDefault();
            us.Name = usr.Name;
            us.Address = usr.Address;
            us.City = usr.City;
            us.Mobile = usr.Mobile;
            obj.SaveChanges();
            TempData["UpdateProf"] = "User Profile Update Sucessfully!";

            return RedirectToAction(nameof(UserProfile));
        }

        public ActionResult UserPassword()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        [HttpPost]
        public ActionResult UserPassword(string st)
        {
            string old = Request.Form["oldPas"];
            string newP = Request.Form["newPas"];
            string em = Session["User"].ToString();
            int uID = (from u in obj.tblUsers where u.EmailId == em select u.UsrId).SingleOrDefault();
            string pas = (from u in obj.tblUsers where u.UsrId == uID select u.Password).SingleOrDefault();

            if(pas.Equals(old))
            {
                tblUser usr = obj.tblUsers.Where(x => x.UsrId == uID).FirstOrDefault();
                usr.Password = newP;
                obj.SaveChanges();
                TempData["PasSuc"] = "Password Updated Sucessfully!";
            }
            else
            {
                TempData["PasNtMt"] = "Incorrect Old Password!";
            }

            return RedirectToAction(nameof(UserPassword));
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}