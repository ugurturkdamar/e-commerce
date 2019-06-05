using e_Ticaret.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e_Ticaret.DataAccessLayer.EntityFramework
{
    public class DatabaseContext : DbContext
    {
        public DbSet<TicaretUser> TicaretUsers { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Comment> Yorumlar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Liked> Likes { get; set; }

        public DatabaseContext()
        {
            Database.SetInitializer(new MyInitializer());
        }
    }
}
