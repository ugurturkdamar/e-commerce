using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using e_Ticaret.Entities;

namespace e_Ticaret.DataAccessLayer.EntityFramework
{
    public class MyInitializer : CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            // adminKullanıcı ekle
            TicaretUser admin = new TicaretUser()
            {
                Name = "Ugur",
                Surname = "Turkdamar",
                Email = "ugurturkdamar@gmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                Username = "Turkishvein",
                Password = "123",
                ProfileImageFilename="user.jpg",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "Turkishvein"
            };

            // standartKullanıcı ekle
            TicaretUser standartUser = new TicaretUser()
            {
                Name = "Ali",
                Surname = "Tekol",
                Email = "alitekol65@gmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                Username = "alitekol",
                Password = "123",
                ProfileImageFilename = "user.jpg",
                CreatedOn = DateTime.Now.AddHours(1),
                ModifiedOn = DateTime.Now.AddMinutes(65),
                ModifiedUsername = "Turkishvein"
            };

            context.TicaretUsers.Add(admin);
            context.TicaretUsers.Add(standartUser);

            for (int i = 0; i < 8; i++)
            {
                TicaretUser user = new TicaretUser()
                {
                    Name = FakeData.NameData.GetFirstName(), 
                    Surname = FakeData.NameData.GetSurname(),
                    Email = FakeData.NetworkData.GetEmail(),
                    ProfileImageFilename = "user.jpg",
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = true,
                    IsAdmin = false,
                    Username = $"user{i}", 
                    Password = "123",
                    CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedUsername = $"user{i}"
                };
                context.TicaretUsers.Add(user);
            }

            context.SaveChanges();

            // User List 
            List<TicaretUser> userlist = context.TicaretUsers.ToList();

            // kategoriye fake data bas
            for (int i = 0; i < 10; i++)
            {
                Kategori cat = new Kategori()
                {
                    Title = FakeData.PlaceData.GetStreetName(),
                    Description = FakeData.PlaceData.GetAddress(),
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    ModifiedUsername = "Turkishvein"
                };

                context.Kategoriler.Add(cat);

                // Urunlere fake data bas
                for (int k = 0; k < FakeData.NumberData.GetNumber(5,9); k++)
                {
                    TicaretUser owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)];

                    Urun urun = new Urun()
                    {
                        Title=FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5,25)),
                        Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1,3)),
                        Kategori = cat,
                        IsDraft = false,
                        LikeCount = FakeData.NumberData.GetNumber(1,9),
                        Owner = owner,
                        CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1),DateTime.Now),
                        ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUsername = owner.Username
                    };

                    cat.Urunler.Add(urun);                

                // Yorumlara fake data bas 
                for (int j = 0; j < FakeData.NumberData.GetNumber(3,5) ; j++)
                {
                    TicaretUser comment_owner= userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)];

                        Comment comment = new Comment()
                    {
                        Text = FakeData.TextData.GetSentence(),
                        Urun=urun,
                        Owner = comment_owner,
                        CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUsername = comment_owner.Username
                        };

                        urun.Yorumlar.Add(comment);
                }

                    // Likelera fake data bas
                   

                    for (int m = 0; m < FakeData.NumberData.GetNumber(1,9); m++)
                    {
                        Liked liked = new Liked()
                        {
                            LikedUser = userlist[m]
                        };
                        urun.Likes.Add(liked);
                    }
                }
            }
            context.SaveChanges();
        }
    }
}
