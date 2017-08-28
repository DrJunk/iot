using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IRPiWeb.Startup))]
namespace IRPiWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
