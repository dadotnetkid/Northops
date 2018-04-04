using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NorthOps.Api.Startup))]
namespace NorthOps.Api {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}