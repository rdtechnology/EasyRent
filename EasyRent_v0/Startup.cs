using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EasyRent_v0.Startup))]
namespace EasyRent_v0
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
