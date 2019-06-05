using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e_Ticaret.Entities
{
    [Table("User")]
    public class TicaretUser : MyEntityBase
    {
        [DisplayName("İsim"),
            StringLength(25, ErrorMessage ="{0} alanı max. {1} karakter olmalıdır")]
        public string Name { get; set; }

        [DisplayName("Soyad"),
            StringLength(25)]
        public string Surname { get; set; }

        [DisplayName("Kullanıcı adı"),
            Required(ErrorMessage = "{0} alanı gereklidir.") ,
            StringLength(25, ErrorMessage ="{0} alanı max. {1} karakter olmalıdır")]
        public string Username { get; set; }

        [DisplayName("Eposta"),
            Required(ErrorMessage = "{0} alanı gereklidir."),
            StringLength(70, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır")]
        public string Email { get; set; }

        [DisplayName("Şifre"),
            Required(ErrorMessage = "{0} alanı gereklidir."),
            StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır")]
        public string Password { get; set; }

        [StringLength(30), ScaffoldColumn(false)]
        public string ProfileImageFilename { get; set; }

        [DisplayName("Aktif")]
        public bool IsActive { get; set; }

        [DisplayName("Yönetici")]
        public bool IsAdmin { get; set; }

        [Required, ScaffoldColumn(false)]
        public Guid ActivateGuid { get; set; }

        public virtual List<Urun> Urunler { get; set; }
        public virtual List<Comment> Yorumlar { get; set; }
        public virtual List<Liked> Likes { get; set; }
    }
}
