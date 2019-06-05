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
    public class MyEntityBase
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)    ]
        public int ID { get; set; }

        [DisplayName("Oluşturma Tarihi"), ScaffoldColumn(false), Required]
        public DateTime CreatedOn { get; set; }

        [DisplayName("Güncelleme Tarihi"), ScaffoldColumn(false), Required]
        public DateTime ModifiedOn { get; set; }

        [DisplayName("Güncelleyen"), ScaffoldColumn(false), Required,StringLength(30)]
        public string ModifiedUsername { get; set; }
    }
}
