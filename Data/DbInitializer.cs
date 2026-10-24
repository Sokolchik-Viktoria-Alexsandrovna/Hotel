using Hotel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.Migrate();

            // Роли
            string[] roles = { "Admin", "Guest" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Администратор
            if (await userManager.FindByEmailAsync("admin@hotel.com") == null)
            {
                var admin = new User
                {
                    UserName = "admin@hotel.com",
                    Email = "admin@hotel.com",
                    PassportNumber = "ADMIN0001",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Номера
            if (!context.Rooms.Any())
            {
                context.Rooms.AddRange(
                    new Room
                    {
                        RoomNumber = "101",
                        Type = RoomType.Standard,
                        Description = "Уютный стандартный номер с видом на сад. Оснащён двуспальной кроватью, телевизором и бесплатным Wi-Fi.",
                        PricePerNight = 79,
                        Capacity = 2,
                        ImageUrl = "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=600",
                        IsAvailable = true
                    },
                    new Room
                    {
                        RoomNumber = "202",
                        Type = RoomType.Deluxe,
                        Description = "Просторный делюкс номер с панорамным видом на город. Мини-бар, джакузи, премиальные удобства.",
                        PricePerNight = 149,
                        Capacity = 2,
                        ImageUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=600",
                        IsAvailable = true
                    },
                    new Room
                    {
                        RoomNumber = "301",
                        Type = RoomType.Suite,
                        Description = "Роскошный люкс с отдельной гостиной, спальней и террасой. Включает завтрак и персонального дворецкого.",
                        PricePerNight = 299,
                        Capacity = 2,
                        ImageUrl = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=600",
                        IsAvailable = true
                    },
                    new Room
                    {
                        RoomNumber = "105",
                        Type = RoomType.Family,
                        Description = "Семейный номер с двумя комнатами, идеально подходящий для семей с детьми. Детская кроватка предоставляется бесплатно.",
                        PricePerNight = 189,
                        Capacity = 4,
                        ImageUrl = "https://images.unsplash.com/photo-1596394516093-501ba68a0ba6?w=600",
                        IsAvailable = true
                    },
                    new Room
                    {
                        RoomNumber = "110",
                        Type = RoomType.Standard,
                        Description = "Стандартный номер с двумя раздельными кроватями, идеален для коллег или друзей.",
                        PricePerNight = 89,
                        Capacity = 2,
                        ImageUrl = "https://images.unsplash.com/photo-1618773928121-c32242e63f39?w=600",
                        IsAvailable = true
                    },
                    new Room
                    {
                        RoomNumber = "405",
                        Type = RoomType.Suite,
                        Description = "Президентский люкс на верхнем этаже с личным баром, бильярдом и видом на всю панораму города.",
                        PricePerNight = 499,
                        Capacity = 3,
                        ImageUrl = "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=600",
                        IsAvailable = true
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
