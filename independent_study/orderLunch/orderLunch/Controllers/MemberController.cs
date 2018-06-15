using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using orderLunch.Models;

namespace orderLunch.Controllers
{
    public class MemberController : Controller
    {
        OrderLunchEntities db = new OrderLunchEntities();

        // GET: Member
        public ActionResult Join()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Join(string ID, string password,string confirm) {
            ID = ID.ToLower();
            ViewBag.errorMessage = "";
            ViewBag.ID = ID;
            var query = from m in db.Members
                        where m.groupID == ID
                        select m;

            //確定帳號是否有人使用
            if (query.Count() > 0) {
                ViewBag.errorMessage +="帳號:"+ID+" 已經有人使用了唷。<br>";
                return View();
            }

            //確認密碼是否相符
            if (password != confirm) {
                ViewBag.errorMessage += "密碼結果不相符唷，請重新輸入。<br>";
                return View();
            }

            Member newMem = new Member { groupID = ID, passWord = password };
            db.Members.Add(newMem);
            db.SaveChanges();
            return Redirect("~/");
        }

        public ActionResult login() {
            return View();
        }

        [HttpPost]
        public ActionResult login(string ID, string password) {
            var query = from m in db.Members
                        where m.groupID == ID
                        select m;
            Member member;
            try { 
                     member = query.Single();
                if (password != member.passWord) {
                    ViewBag.ID = ID;
                    ViewBag.errorMessage = "密碼不符，請重新輸入。";
                    return View();
                }
                } catch {
                ViewBag.errorMessage = ID + "，該帳號不存在唷。";
                return View();
            }
            
            Session["identity"] = ID;
            Session["memberId"] = member.ID;
            return Redirect(Session["urlString"].ToString());
        }

        public ActionResult logout() {
            Session["identity"] = "Guest";
            Session["memberId"] = 0;
            Session["urlString"] = "~/";
            return Redirect("~/");
        }

        public ActionResult edit() {
            int memberId = Convert.ToInt32(Session["memberId"]);
            if (memberId >= 1) {
                ViewBag.ID = memberId;
                Member member = db.Members.Find(memberId);
                

                return View(member);

            }
            return Redirect("~/");
        }

        [HttpPost]
        public ActionResult edit(int ID) {
            Member member = db.Members.Find(ID);
            if (Request["orgPassword"] != member.passWord) {
                ViewBag.errorMessage = "原本的密碼不對唷。";
                return View(member);
            };
            if (Request["newPassword"] != Request["confirm"]) {
                ViewBag.errorMessage = "兩次輸入的新密碼不同唷。";
                return View(member);
            };

            member.ID = member.ID;
            member.groupID = member.groupID;
            member.passWord = Request["newPassword"];
            db.SaveChanges();

            return Redirect("~/");
        }

    }
}