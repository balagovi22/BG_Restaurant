using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Restaurant.Services.Identity.Database;
using Restaurant.Services.Identity.Models;
using System.Security.Claims;

namespace Restaurant.Services.Identity.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer()
        {
        }
        public DbInitializer(UserManager<ApplicationUser> userManager, 
                    RoleManager<IdentityRole> roleManager,
                    ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public void Initialize()
        {
            if (_roleManager.FindByIdAsync(SD.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
            }
            else { return; }

            ApplicationUser adminUser = new ApplicationUser()
            {
                UserName = "admin1@gmail.com",
                Email = "admin1@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "111111111",
                FirstName = "Ben",
                LastName = "Conger"
            };

            _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

            var temp1= _userManager.AddClaimsAsync(adminUser, new Claim[]
            {
              new Claim(JwtClaimTypes.Name, adminUser.FirstName+" " +adminUser.LastName),
              new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
              new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
              new Claim(JwtClaimTypes.Role, SD.Admin)
            }).Result;

            //User 2
            ApplicationUser customerUser = new ApplicationUser()
            {
                UserName = "admin2@gmail.com",
                Email = "admin2@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "111111111",
                FirstName = "Martin",
                LastName = "Kalde"
            };

            _userManager.CreateAsync(customerUser, "Cust123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customerUser, SD.Customer).GetAwaiter().GetResult();

            var temp2 = _userManager.AddClaimsAsync(customerUser, new Claim[]
            {
              new Claim(JwtClaimTypes.Name, customerUser.FirstName+" " + customerUser.LastName),
              new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
              new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
              new Claim(JwtClaimTypes.Role, SD.Customer)
            }).Result;
        }

    }
}
