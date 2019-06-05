using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using e_Ticaret.Entities;
using e_Ticaret.Models;
using e_Ticaret.BusinessLayer;
using e_Ticaret.Filters;

namespace e_Ticaret.Controllers
{
    [Exc]
    public class UrunController : Controller
    {
        private UrunManager urunManager = new UrunManager();
        private CategoryManager categoryManager = new CategoryManager();
        private LikedManager likedManager = new LikedManager();

        [Auth]
        public ActionResult Index()
        {
            var uruns = urunManager.ListQueryable().Include("Kategori").Include("Owner").Where(
                x => x.Owner.ID == CurrentSession.User.ID).OrderByDescending(
                x => x.ModifiedOn);

            return View(uruns.ToList());
        }

        [Auth]
        public ActionResult MyLikedUruns()
        {
            var uruns = likedManager.ListQueryable().Include("LikedUser").Include("Urun").Where(
                x => x.LikedUser.ID == CurrentSession.User.ID).Select(
                x => x.Urun).Include("Kategori").Include("Owner").OrderByDescending(
                x => x.ModifiedOn);

                return View("Index", uruns.ToList());
        }

        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urun urun = urunManager.Find(x => x.ID == id);
            if (urun == null)
            {
                return HttpNotFound();
            }
            return View(urun);
        }

        [Auth]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "ID", "Title");
            return View();
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Urun urun)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                urun.Owner = CurrentSession.User;
                urunManager.Insert(urun);
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "ID", "Title", urun.CategoryId);
            return View(urun);
        }

        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urun urun = urunManager.Find(x => x.ID == id);
            if (urun == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "ID", "Title", urun.CategoryId);
            return View(urun);
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Urun urun)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                Urun db_urun = urunManager.Find(x => x.ID == urun.ID);
                db_urun.IsDraft = urun.IsDraft;
                db_urun.CategoryId = urun.CategoryId;
                db_urun.Text = urun.Text;
                db_urun.Title = urun.Title;

                urunManager.Update(db_urun);

                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "ID", "Title", urun.CategoryId);
            return View(urun);
        }

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Urun urun = urunManager.Find(x => x.ID == id);
            if (urun == null)
            {
                return HttpNotFound();
            }
            return View(urun);
        }

        [Auth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Urun urun = urunManager.Find(x => x.ID == id);
            urunManager.Delete(urun);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            if (CurrentSession.User != null)
            {
                List<int> likedUrunIds = likedManager.List(
                    x => x.LikedUser.ID == CurrentSession.User.ID && ids.Contains(x.Urun.ID)).Select(
                    x => x.Urun.ID).ToList();

                return Json(new { result = likedUrunIds });
            }
            else
            {
                return Json(new { result = new List<int>() });
            }
        }

        [HttpPost]
        public ActionResult SetLikeState(int urunid, bool liked)
        {
            int res = 0;

            if (CurrentSession.User == null)
                return Json(new { hasError = true, errorMessage = "Beğenme işlemi için giriş yapmalısınız.", result = 0 });

            Liked like =
                likedManager.Find(x => x.Urun.ID == urunid && x.LikedUser.ID == CurrentSession.User.ID);

            Urun urun = urunManager.Find(x => x.ID == urunid);

            if (like != null && liked == false)
            {
                res = likedManager.Delete(like);
            }
            else if (like == null && liked == true)
            {
                res = likedManager.Insert(new Liked()
                {
                    LikedUser = CurrentSession.User,
                    Urun = urun
                });
            }

            if (res > 0)
            {
                if (liked)
                {
                    urun.LikeCount++;
                }
                else
                {
                    urun.LikeCount--;
                }

                res = urunManager.Update(urun);

                return Json(new { hasError = false, errorMessage = string.Empty, result = urun.LikeCount });
            }

            return Json(new { hasError = true, errorMessage = "Beğenme işlemi gerçekleştirilemedi.", result = urun.LikeCount });
        }

        public ActionResult GetNoteText(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Urun urun = urunManager.Find(x => x.ID == id);

            if (urun == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialNoteText", urun);
        }
    }
}