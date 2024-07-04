using HotelBookingWeb.Data;
using HotelBookingWeb.Models;
using HotelBookingWeb.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace HotelBookingWeb.DbInitializer
{
    public class DbInitializer : IDbinitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            ApplicationDbContext db,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            ILogger<DbInitializer> logger
           )
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public void Initializer()
        {
            // apply all migration 
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }

            // create role if they are not created
            if (!_roleManager.RoleExistsAsync(StaticDetail.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(StaticDetail.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetail.Role_Customer)).GetAwaiter().GetResult();

                // if roles are not created, then will create account admin user
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Dang Duy Thong",
                    PhoneNumber = "123456789",
                    Street = "25 Test",
                    Ward = "9",
                    District = "12",
                    City = "Hồ Chí Minh"
                }, password: _configuration.GetSection("AdminPassword:Password").Get<string>()).GetAwaiter().GetResult();

                // After Create User -> Asign Role to user
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@gmail.com");
                _userManager.AddToRoleAsync(applicationUser, StaticDetail.Role_Admin).GetAwaiter().GetResult();
            }
        }
    }
}
