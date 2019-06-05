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
    [Table("Kategori")]
    public class Kategori : MyEntityBase
    {
        [DisplayName("Başlık"),
            Required(ErrorMessage ="{0} alanı gereklidir."),
            StringLength(50,ErrorMessage ="{0} alanı max {1} karakter içermeli.")]
        public string Title { get; set; }

        [DisplayName("Açıklama"),
            StringLength(150, ErrorMessage = "{0} alanı max {1} karakter içermeli.")]
        public string Description { get; set; }

        public virtual List<Urun> Urunler { get; set; }

        public Kategori()
        {
            Urunler = new List<Urun>();
        }
    }
}
