using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using e_Ticaret.BusinessLayer;
using e_Ticaret.Entities;
using e_Ticaret.Models;
using e_Ticaret.Filters;

namespace e_Ticaret.Controllers
{
    [Auth]
    [AuthAdmin]
    [Exc]
    public class CategoryController : Controller
    {
        private CategoryManager categoryManager = new CategoryManager();

        public ActionResult Index()
        {
            return View(categoryManager.List());
        }
      
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Kategori kategori = categoryManager.Find(x=>x.ID==id.Value);

            if (kategori == null)
            {
                return HttpNotFound();
            }

            return View(kategori);
        }
       
        public ActionResult Create()
        {
            return View();
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Kategori kategori)
        {
            ModelState.Remove("ModifiedUsername");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");

            if (ModelState.IsValid)
            {
                categoryManager.Insert(kategori);
                CacheHelper.RemoveCategoriesFromCache();

                return RedirectToAction("Index");
            }

            return View(kategori);
        }
      
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Kategori kategori = categoryManager.Find(x => x.ID == id.Value);

            if (kategori == null)
            {
                return HttpNotFound();
            }
            return View(kategori);
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Kategori kategori)
        {
            ModelState.Remove("ModifiedUsername");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");

            if (ModelState.IsValid)
            {
                Kategori cat = categoryManager.Find(x => x.ID == kategori.ID);
                cat.Title = kategori.Title;
                cat.Description = kategori.Description;

                categoryManager.Update(cat);
                CacheHelper.RemoveCategoriesFromCache();

                return RedirectToAction("Index");
            }
            return View(kategori);
        }
       
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Kategori kategori = categoryManager.Find(x => x.ID == id.Value);

            if (kategori == null)
            {
                return HttpNotFound();
            }

            return View(kategori);
        }
      
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kategori kategori = categoryManager.Find(x => x.ID == id);
            categoryManager.Delete(kategori);

            CacheHelper.RemoveCategoriesFromCache();

            return RedirectToAction("Index");
        }

    }
}
