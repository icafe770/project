using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using orderLunch.Models;

namespace orderLunch.Controllers
{
    public class orderController : Controller
    {
        OrderLunchEntities db = new OrderLunchEntities();
        // GET: order
        public ActionResult Index()
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
                        select new MenuListView { nameOfVendor = selectedMenu.nameOfVendor, phoneOfVendor = selectedMenu.phoneOfVendor, menuId = selectedMenu.menuId };

            List<MenuListView> menu = query.ToList();
            return View(menu);
        }

        public ActionResult orderDetail() {
            return View();
        }
    }
}