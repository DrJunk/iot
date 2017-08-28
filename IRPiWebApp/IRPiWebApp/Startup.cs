using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IRPiWebApp.Startup))]
namespace IRPiWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
