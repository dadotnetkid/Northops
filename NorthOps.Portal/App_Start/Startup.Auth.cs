using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using NorthOps.Portal.Models;
using Owin;
using System;

namespace NorthOps.Portal {
    public partial class Startup {
        public void ConfigureAuth(IAppBuilder app) {
            app.CreatePerOwinContext(NorthOpsEntities.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/member/login"),
                Provider = new CookieAuthenticationProvider {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, User>(
                       validateInterval: TimeSpan.FromMinutes(30),
                       regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
        }
    }

}