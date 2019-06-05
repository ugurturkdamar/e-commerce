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
    [Table("Urun")]
    public class Urun : MyEntityBase
    {
        [DisplayName("Urun Başlığı"), Required, StringLength(60)]
        public string Title { get; set; }

        [DisplayName("Urun Metni"), Required, StringLength(2000)]
        public string Text { get; set; }

        [DisplayName("Taslak")]
        public bool IsDraft { get; set; }

        [DisplayName("Beğenilme")]
        public int LikeCount { get; set; }

        [DisplayName("Kategori")]
        public int CategoryId { get; set; }

        public virtual TicaretUser Owner { get; set; }
        public virtual Kategori Kategori { get; set; }
        public virtual List<Comment> Yorumlar { get; set; }
        public virtual List<Liked> Likes { get; set; }

        public Urun()
        {
            Yorumlar = new List<Comment>();
            Likes = new List<Liked>();
        }
    }
}
