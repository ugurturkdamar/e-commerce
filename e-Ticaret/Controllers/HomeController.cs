using e_Ticaret.BusinessLayer;
using e_Ticaret.BusinessLayer.Results;
using e_Ticaret.Entities;
using e_Ticaret.Entities.Messages;
using e_Ticaret.Entities.ValueObjects;
using e_Ticaret.Filters;
using e_Ticaret.Models;
using e_Ticaret.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace e_Ticaret.Controllers
{
    [Exc]
    public class HomeController : Controller
    {
        
        private UrunManager urunManager = new UrunManager();
        private CategoryManager categoryManager = new CategoryManager();
        private UserManager userManager = new UserManager();

        // GET: Home
        public ActionResult Index()
        {
            return View(urunManager.ListQueryable().Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<Urun> urunler = urunManager.ListQueryable().Where(
               x => x.IsDraft == false && x.CategoryId == id).OrderByDescending(
               x => x.ModifiedOn).ToList();

            return View("Index", urunler);
        }

        public ActionResult MostLiked()
        {            
            return View("Index",urunManager.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        [Auth]
        public ActionResult ShowProfile()
        {
            TicaretUser currUser = Session["login"] as TicaretUser;
            
            BusinessLayerResult <TicaretUser> res = userManager.GetUserById(currUser.ID);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Errors
                };

                return View("Error", errorNotifyObj);
            }

            return View(res.Result);
        }

        [Auth]
        public ActionResult EditProfile()
        {
            TicaretUser currUser = Session["login"] as TicaretUser;
            BusinessLayerResult<TicaretUser> res = userManager.GetUserById(currUser.ID);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Errors
                };

                return View("Error", errorNotifyObj);
            }

            return View(res.Result);
        }

        [Auth]
        [HttpPost]
        public ActionResult EditProfile(TicaretUser model,HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                if (ProfileImage != null && 
                    (ProfileImage.ContentType == "image/jpeg" || 
                    ProfileImage.ContentType == "image/jpg" || 
                    ProfileImage.ContentType == "image/png"))
                {
                    string filename = $"user_{model.ID}.{ProfileImage.ContentType.Split('/')[1]}";

                    ProfileImage.SaveAs(Server.MapPath($"~/Images/{filename}"));
                    model.ProfileImageFilename = filename;
                }

                BusinessLayerResult<TicaretUser> res = userManager.UpdateProfile(model);

                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    {
                        Title = "Profil Güncellenemedi",
                        Items = res.Errors,
                        RedirectingUrl = "/Home/EditProfile"
                    };

                    return View("Error", errorNotifyObj);
                }

                Session["login"] = res.Result;

                return RedirectToAction("ShowProfile");
            }
            return View(model);
        }

        [Auth]
        public ActionResult DeleteProfile()
        {
            TicaretUser currUser = Session["login"] as TicaretUser;

            BusinessLayerResult<TicaretUser> res = userManager.RemoveUserById(currUser.ID);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Profil Silinemedi.",
                    Items = res.Errors,
                    RedirectingUrl = "/Home/ShowProfile"
                };

                return View("Error", errorNotifyObj);
            }

            Session.Clear();

            return RedirectToAction("Index");
        }
     
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<TicaretUser> res = userManager.LoginUser(model);

                if (res.Errors.Count > 0)
                {                  
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    return View(model);
                }

                CurrentSession.Set<TicaretUser>("login", res.Result);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {         
            if (ModelState.IsValid)
            {             
                BusinessLayerResult<TicaretUser> res = userManager.RegisterUser(model);

                if (res.Errors.Count > 0)
                {                
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    return View(model);
                }
              
                OkViewModel notifyObj = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl="/Home/Login",                 
                };

                notifyObj.Items.Add(" Lütfen e-posta'nıza gönderdiğimiz aktivasyon link'ine tıklayarak hesabınızı aktive ediniz.Hesabınızı aktive etmeden ürün ekleyemez ve ürünleri beğenemezsiniz.");

                 return View("OK",notifyObj);
            }

                return View(model);
        }

        public ActionResult UserActivate(Guid id)
        {
            BusinessLayerResult<TicaretUser> res = userManager.ActivateUser(id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Errors
                };
                
                return View("Error",errorNotifyObj);
            }

            OkViewModel okNotifyObj = new OkViewModel()
            {
                Title="Hesap Aktifleştirildi",
                RedirectingUrl="/Home/Login"
            };

            okNotifyObj.Items.Add("Hesabınız aktifleştirildi. Artık ürün paylaşıp beğenebilirsiniz.");

            return View("Ok",okNotifyObj);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }
    }
}