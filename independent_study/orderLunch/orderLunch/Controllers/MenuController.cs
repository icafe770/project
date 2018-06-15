using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using orderLunch.Models;

namespace orderLunch.Controllers
{
    public class MenuController : Controller
    {
        OrderLunchEntities db = new OrderLunchEntities();
        // GET: Menu
        public ActionResult menuList()
        {
            if ((int)Session["memberId"] == 0) {
                Session["urlString"] = "~/Menu/menuList";
                return Redirect("~/Member/login");
            }
            int memberId = (int)Session["memberId"];
            //決定哪位成員
            var q = db.Members.Find(memberId);
            //透過該成員的menuinfores找東西
            var query = from selectedMenu in q.MenuInfoes
                        select new MenuListView { nameOfVendor = selectedMenu.nameOfVendor, phoneOfVendor = selectedMenu.phoneOfVendor,menuId= selectedMenu.menuId };

            List <MenuListView> menu = query.ToList();
            
            return View(menu);
        }

        public ActionResult addMenu() {

            return View();
        }

        [HttpPost]
        public ActionResult addMenu(string nameOfVendor, string phoneOfVendor) {
            //先新增該筆菜單
            MenuInfo newMenu = new MenuInfo { nameOfVendor = nameOfVendor, phoneOfVendor = phoneOfVendor };
            //找出擁有者
            var MemberUser = db.Members.Find((int)Session["memberId"]);
            //將該菜單加入該擁有者
            MemberUser.MenuInfoes.Add(newMenu);
            db.SaveChanges();

            return Redirect("~/Menu/menuList");
        }

        public ActionResult deleteMenu(int id) {
            MenuInfo selectedMenu = db.MenuInfoes.Find(id);
            Member MeberUser = db.Members.Find((int)Session["memberId"]);
            var prods = from prod in db.productDetails
                       where prod.menuId == id
                       select prod;
            foreach(var prod in prods.ToList()) {
                db.productDetails.Remove(prod);
            }

            MeberUser.MenuInfoes.Remove(selectedMenu);
            db.MenuInfoes.Remove(selectedMenu);
             
            db.SaveChanges();
            return Redirect("~/Menu/menuList");
        }

        public ActionResult productList(int id) {//id為menuId
            MenuInfo menu = db.MenuInfoes.Find(id);
            var query = from prod in menu.productDetails
                        select prod;

            List<productDetail> ProdList = query.ToList();
            ViewBag.menuId = id;
            

            return View(ProdList);
        }
        [HttpPost]
        public ActionResult productList(string newProd,string Price,int id) {
            productDetail addProd = new productDetail { menuId=id,nameOfProduct= newProd, Price=Price };
            db.productDetails.Add(addProd);
            db.SaveChanges();
            return Redirect("~/Menu/productList/"+id);
        }
    }
}