using e_Ticaret.BusinessLayer.Results;
using e_Ticaret.BusinessLayer.Abstract;
using e_Ticaret.Common.Helpers;
using e_Ticaret.DataAccessLayer.EntityFramework;
using e_Ticaret.Entities;
using e_Ticaret.Entities.Messages;
using e_Ticaret.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e_Ticaret.BusinessLayer
{
    public class UserManager : ManagerBase<TicaretUser>
    {
        public BusinessLayerResult<TicaretUser> RegisterUser(RegisterViewModel data)
        {
            // Kullanıcı username kontrolü..
            // Kullanıcı e-posta kontrolü..
            // Kayıt işlemi..
            // Aktivasyon e-postası gönderimi.
            TicaretUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<TicaretUser> res = new BusinessLayerResult<TicaretUser>();

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }

                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExist, "E-posta adresi kayıtlı.");
                }
            }
            else
            {
                int dbResult = base.Insert(new TicaretUser()
                {
                    Username = data.Username,
                    Email = data.Email,
                    ProfileImageFilename = "user_boy.png",
                    Password = data.Password,
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false
                });

                if (dbResult > 0)
                {
                    res.Result = Find(x => x.Email == data.Email && x.Username == data.Username);

                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                    string body = $"Merhaba {res.Result.Username};<br><br>Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank'>tıklayınız</a>.";

                    MailHelper.SendMail(body, res.Result.Email, "e-Ticaret Hesap Aktifleştirme");
                }
            }

            return res;
        }

        public BusinessLayerResult<TicaretUser> GetUserById(int id)
        {
            BusinessLayerResult<TicaretUser> res = new BusinessLayerResult<TicaretUser>();
            res.Result = Find(x => x.ID == id);

            if (res.Result == null)
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı bulunamadı.");
            }

            return res;
        }

        public BusinessLayerResult<TicaretUser> LoginUser(LoginViewModel data)
        {
            // Giriş kontrolü
            // Hesap aktive edilmiş mi?
            BusinessLayerResult<TicaretUser> res = new BusinessLayerResult<TicaretUser>();
            res.Result = Find(x => x.Username == data.Username && x.Password == data.Password);

            if (res.Result != null)
            {
                if (!res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    res.AddError(ErrorMessageCode.CheckYourEmail, "Lütfen e-posta adresinizi kontrol ediniz.");
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UsernameOrPassWrong, "Kullanıcı adı yada şifre uyuşmuyor.");
            }

            return res;
        }

        public BusinessLayerResult<TicaretUser> UpdateProfile(TicaretUser data)
        {
            TicaretUser db_user = Find(x => x.ID != data.ID && (x.Username == data.Username || x.Email == data.Email));
            BusinessLayerResult<TicaretUser> res = new BusinessLayerResult<TicaretUser>();

            if (db_user != null && db_user.ID != data.ID)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExist, "E-posta adresi kayıtlı.");
                }

                return res;
            }

            res.Result = Find(x => x.ID == data.ID);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;

            if (string.IsNullOrEmpty(data.ProfileImageFilename) == false)
            {
                res.Result.ProfileImageFilename = data.ProfileImageFilename;
            }

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.ProfileCouldNotUpdated, "Profil güncellenemedi.");
            }

            return res;
        }

        public BusinessLayerResult<TicaretUser> RemoveUserById(int id)
        {
            BusinessLayerResult<TicaretUser> res = new BusinessLayerResult<TicaretUser>();
            TicaretUser user = Find(x => x.ID == id);

            if (user != null)
            {
                if (Delete(user) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı silinemedi.");
                    return res;
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UserCouldNotFind, "Kullanıcı bulunamadı.");
            }

            return res;
        }

        public BusinessLayerResult<TicaretUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<TicaretUser> res = new BusinessLayerResult<TicaretUser>();
            res.Result = Find(x => x.ActivateGuid == activateId);

            if (res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyActive, "Kullanıcı zaten aktif edilmiştir.");
                    return res;
                }

                res.Result.IsActive = true;
                Update(res.Result);
            }
            else
            {
                res.AddError(ErrorMessageCode.ActivateIdDoesNotExist, "Aktifleştirilecek kullanıcı bulunamadı.");
            }

            return res;
        }


        // Method hiding..
        public new BusinessLayerResult<TicaretUser> Insert(TicaretUser data)
        {
            TicaretUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<TicaretUser> res = new BusinessLayerResult<TicaretUser>();

            res.Result = data;

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }

                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExist, "E-posta adresi kayıtlı.");
                }
            }
            else
            {
                res.Result.ProfileImageFilename = "user_boy.png";
                res.Result.ActivateGuid = Guid.NewGuid();

                if (base.Insert(res.Result) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı eklenemedi.");
                }
            }

            return res;
        }

        public new BusinessLayerResult<TicaretUser> Update(TicaretUser data)
        {
            TicaretUser db_user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<TicaretUser> res = new BusinessLayerResult<TicaretUser>();
            res.Result = data;

            if (db_user != null && db_user.ID != data.ID)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExist, "E-posta adresi kayıtlı.");
                }

                return res;
            }

            res.Result = Find(x => x.ID == data.ID);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdated, "Kullanıcı güncellenemedi.");
            }

            return res;
        }
    }
}