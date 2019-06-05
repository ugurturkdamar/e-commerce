using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using e_Ticaret.Entities;
using e_Ticaret.BusinessLayer;
using e_Ticaret.BusinessLayer.Results;
using e_Ticaret.Filters;

namespace e_Ticaret.Controllers
{
    [Auth]
    [AuthAdmin]
    [Exc]
    public class UserController : Controller
    {
        private UserManager userManager = new UserManager();


        public ActionResult Index()
        {
            return View(userManager.List());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicaretUser ticaretUser = userManager.Find(x => x.ID == id.Value);

            if (ticaretUser == null)
            {
                return HttpNotFound();
            }

            return View(ticaretUser);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TicaretUser ticaretUser)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<TicaretUser> res = userManager.Insert(ticaretUser);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(ticaretUser);
                }

                return RedirectToAction("Index");
            }

            return View(ticaretUser);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicaretUser ticaretUser = userManager.Find(x => x.ID == id.Value);

            if (ticaretUser == null)
            {
                return HttpNotFound();
            }

            return View(ticaretUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TicaretUser ticaretUser)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<TicaretUser> res = userManager.Update(ticaretUser);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(ticaretUser);
                }

                return RedirectToAction("Index");
            }
            return View(ticaretUser);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TicaretUser evernoteUser = userManager.Find(x => x.ID == id.Value);

            if (evernoteUser == null)
            {
                return HttpNotFound();
            }

            return View(evernoteUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicaretUser ticaretUser = userManager.Find(x => x.ID == id);
            userManager.Delete(ticaretUser);

            return RedirectToAction("Index");
        }
    }
}