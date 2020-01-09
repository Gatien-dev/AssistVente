using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;

namespace IdentitySample.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string name = "admin@example.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                var roleresult = roleManager.Create(role);
            }
            //Adding other roles
            List<string> allRoles = new List<string>()
            {
                "Produits",
                "Produits-edition",
                "Produits-suppression",
                "Ventes",
                "Ventes-edition",
                "Ventes-suppression",
                "Achats",
                "Achats-edition",
                "Achats-suppression",
                "Caisses",
                "Caisses-edition",
                "Caisses-suppression",
                "Abonnements",
                "Abonnements-edition",
                "Abonnements-suppression",
                "Locations",
                "Locations-edition",
                "Locations-suppression",
                "Stocks",
                "Stocks-edition",
                "Stocks-suppression",
                "Forfaits",
                "Forfaits-edition",
                "Forfaits-suppression",
                "Modifier les montants de location",
            };
            foreach (var newRole in allRoles)
            {
                if (roleManager.FindByName(newRole) == null)
                {
                    roleManager.Create(new IdentityRole(newRole));
                }
            }
            //var roleProduits = roleManager.FindByName("Produits");
            //if (roleProduits == null)
            //{
            //    roleProduits = new IdentityRole("Produits");
            //    var roleresult = roleManager.Create(roleProduits);
            //}

            //var roleVente = roleManager.FindByName("Ventes");
            //if (roleVente == null)
            //{
            //    roleVente = new IdentityRole("Ventes");
            //    var roleresult = roleManager.Create(roleVente);
            //}
            //var roleAchat = roleManager.FindByName("Achats");
            //if (roleAchat == null)
            //{
            //    roleAchat = new IdentityRole("Achats");
            //    var roleresult = roleManager.Create(roleAchat);
            //}

            //var roleCaisse = roleManager.FindByName("Caisses");
            //if (roleCaisse == null)
            //{
            //    roleCaisse = new IdentityRole("Caisses");
            //    var roleresult = roleManager.Create(roleCaisse);
            //}


            //var roleAbo = roleManager.FindByName("Abonnements");
            //if (roleAbo == null)
            //{
            //    roleAbo = new IdentityRole("Abonnements");
            //    var roleresult = roleManager.Create(roleAbo);
            //}

            //var roleLocations = roleManager.FindByName("Locations");
            //if (roleLocations == null)
            //{
            //    roleLocations = new IdentityRole("Locations");
            //    var roleresult = roleManager.Create(roleLocations);
            //}

            //var roleStock = roleManager.FindByName("Stocks");
            //if (roleStock == null)
            //{
            //    roleStock = new IdentityRole("Stocks");
            //    var roleresult = roleManager.Create(roleStock);
            //}
            //var roleForfaits = roleManager.FindByName("Forfaits");
            //if (roleForfaits == null)
            //{
            //    roleForfaits = new IdentityRole("Forfaits");
            //    var roleresult = roleManager.Create(roleForfaits);
            //}
            ////Modifier les montants de location
            //var roleModifMontantLocations = roleManager.FindByName("Modifier les montants de location");
            //if (roleModifMontantLocations == null)
            //{
            //    roleModifMontantLocations = new IdentityRole("Modifier les montants de location");
            //    var roleresult = roleManager.Create(roleModifMontantLocations);
            //}
            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = name };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager)
        { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}