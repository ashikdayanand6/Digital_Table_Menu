using DigitalMenu.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalMenu.Controllers
{
    public class AdminController : Controller
    {
        DigitalMenuEntities1 obj = new DigitalMenuEntities1();
        // GET: Admin
        public ActionResult AdminHome()
        {
            return View();
        }

        public ActionResult F_Type()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "Home");
            string em = Session["User"].ToString();
            ViewBag.Typ = obj.tblFoodTypes.ToList();
            if (TempData["FtDt"] != null)
            {
                tblFoodType ft = (tblFoodType)TempData["FtDt"];
                return View(ft);
            }

            return View();
        }

        [HttpPost]
        public ActionResult F_Type(tblFoodType ft)
        {
            if (ft.FdId == 0)
            {
                obj.tblFoodTypes.Add(ft);
                obj.SaveChanges();
                TempData["TypeInSucess"] = "Food Type Inserted Sucessfully!";
            }
            else
            {
                obj.Entry(ft).State = EntityState.Modified;
                obj.SaveChanges();
                TempData["TypeUpSucess"] = "Food Type Updated Sucessfully!";
            }

            return RedirectToAction("F_Type");
        }

        public ActionResult EditType(int id)
        {
            tblFoodType ft = obj.tblFoodTypes.Where(x => x.FdId == id).FirstOrDefault();
            TempData["FtDt"] = ft;
            TempData["FtID"] = id;

            return RedirectToAction("F_Type");
        }

        public ActionResult DeleteType(int id)
        {
            try
            {
                tblFoodType ft = obj.tblFoodTypes.SingleOrDefault(x => x.FdId == id);
                obj.tblFoodTypes.Remove(ft);
                obj.SaveChanges();
            }
            catch
            {
                Response.Write("<script>alert('Foreign key constraint!')</script>");
            }

            return RedirectToAction("F_Type");
        }

        public ActionResult Food()
        {
            ViewBag.Food = obj.tblFoods.Include(d => d.tblFoodType).ToList();
            ViewBag.FdTp = new SelectList(obj.tblFoodTypes, "FdId", "FoodType");

            return View();
        }

        [HttpPost]
        public ActionResult Food(tblFood fd, HttpPostedFileBase file)
        {
            if (fd.FId == 0)
            {
                var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };
                var fileName = Path.GetFileName(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                if (allowedExtensions.Contains(ext))
                {
                    string name = Path.GetFileNameWithoutExtension(fileName);
                    string path = "~/Content/Image/" + name + DateTime.Now.ToString("yymmddssfff") + ext;
                    file.SaveAs(Server.MapPath(path));
                    fd.Image = path;
                    fd.Status = "Available";
                    obj.tblFoods.Add(fd);
                    obj.SaveChanges();
                    TempData["InsertFood"] = "Food Inserted Sucessfully!";
                    ModelState.Clear();
                }
            }
            else
            {
                obj.Entry(fd).State = EntityState.Modified;
                obj.SaveChanges();
                TempData["UpdateFood"] = "Food Updated Sucessfully!";
                ModelState.Clear();
            }

            return RedirectToAction(nameof(Food));
        }

        public ActionResult DeleteFood(int id)
        {
            try
            {
                tblFood fd = obj.tblFoods.SingleOrDefault(x => x.FId == id);
                obj.tblFoods.Remove(fd);
                obj.SaveChanges();
                TempData["DeleteFood"] = "Food Deleted Sucessfully!";
            }
            catch
            {
                Response.Write("<script>alert('Foreign key constraint!')</script>");
            }

            return RedirectToAction(nameof(Food));
        }

        public ActionResult AddCheff()
        {
            ViewBag.Res = (from c in obj.tblChefs select c).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult AddCheff(tblChef ch)
        {
            obj.tblChefs.Add(ch);
            obj.SaveChanges();
            TempData["CheffAdd"] = "Cheff Added Sucessfully!";

            return RedirectToAction(nameof(AddCheff));
        }

        public ActionResult DeleteCheff(int id)
        {
            try
            {
                tblChef ch = obj.tblChefs.SingleOrDefault(x => x.ChId == id);
                obj.tblChefs.Remove(ch);
                obj.SaveChanges();
                TempData["CheffRemove"] = "Cheff Removed Sucessfully!";
            }
            catch
            {
                Response.Write("<script>alert('Foreign key constraint!')</script>");
            }

            return RedirectToAction(nameof(AddCheff));
        }

        public ActionResult Chefproducts()
        {
            var res = obj.tblChefProducts.Where(c => c.Stataus == "Pending").Include(c => c.tblChef).ToList();
            ViewBag.Res = res;

            return View();
        }

        public ActionResult FinishProd(int id)
        {
            tblChefProduct ch = obj.tblChefProducts.Where(x => x.ChPdId == id).FirstOrDefault();
            ch.Stataus = "Finished";
            obj.SaveChanges();

            return RedirectToAction(nameof(Chefproducts));
        }

        public ActionResult AdminProducts()
        {
            List<SelectListItem> itemsDt = new List<SelectListItem>();
            itemsDt.Add(new SelectListItem() { Text = "Retailer", Value = "Retailer" });
            itemsDt.Add(new SelectListItem() { Text = "Former", Value = "Former" });
            ViewBag.Seller = itemsDt;

            var res = obj.tblAdminProducts.Include(x => x.tblSeller).ToList();
            ViewBag.Res = res;

            return View();
        }

        public JsonResult GetSeller(string id)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "--Select--", Value = "0" });
            var res = (from s in obj.tblSellers where s.Type == id select s).ToList();
            foreach (var item in res)
            {
                list.Add(new SelectListItem { Text = item.SellerName, Value = item.SelId.ToString() });
            }
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));

            //List<tblSeller> ar = new List<tblSeller>();
            //ar = (from a in obj.tblSellers where a.Type == id select a).ToList();
            //return Json(new SelectList(ar, "SelId", "SellerName", JsonRequestBehavior.AllowGet));
        }

        [HttpPost]
        public ActionResult AdminProducts(tblAdminProduct ad)
        {
            try
            {
                var dt = DateTime.Now.ToString();
                ad.PstDate = dt;
                obj.tblAdminProducts.Add(ad);
                obj.SaveChanges();
                TempData["AdminProdIns"] = "Products Posted!";
            }
            catch
            {

            }

            return RedirectToAction(nameof(AdminProducts));
        }

        public ActionResult ViewProdOrderList()
        {
            var res = (from p in obj.tblAdminProducts select p).ToList();
            ViewBag.Res = res;

            return View();
        }

        public ActionResult AvailabaleFood()
        {
            ViewBag.Res = obj.tblFoods.Where(x => x.Status == "Available").Include(x => x.tblFoodType).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AvailabaleFood(int id)
        {
            tblFood fd = obj.tblFoods.Where(x => x.FId == id).FirstOrDefault();
            fd.Status = "Not Available";
            obj.SaveChanges();
            TempData["FoodNt"] = "Food Not Available!";

            return RedirectToAction(nameof(AvailabaleFood));
        }
        
        public ActionResult AvailableFoodAction(int id)
        {
            tblFood fd = obj.tblFoods.Where(x => x.FId == id).FirstOrDefault();
            fd.Status = "Not Available";
            obj.SaveChanges();
            TempData["FoodNt"] = "Food Not Available!";

            return RedirectToAction(nameof(AvailabaleFood));
        }

        public ActionResult NotAvailFood()
        {
            ViewBag.Res = obj.tblFoods.Where(x => x.Status == "Not Available").Include(x => x.tblFoodType).ToList();

            return View();
        }

        public ActionResult NotAvFood(int id)
        {
            tblFood fd = obj.tblFoods.Where(x => x.FId == id).FirstOrDefault();
            fd.Status = "Available";
            obj.SaveChanges();
            TempData["FoodAv"] = "Food Available!";

            return RedirectToAction(nameof(NotAvailFood));
        }

        public ActionResult Report()
        {
            var res = (from b in obj.tblBooks where b.Status == "Finished" select b.TotalPrice).Sum();
            ViewBag.Res = obj.tblBooks.Where(x => x.Status == "Finished").Include(x => x.tblFood).ToList();
            ViewBag.Tot = res;

            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}