using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SoftwareEngineering1Project.Startup))]
namespace SoftwareEngineering1Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app); 
        }
    }
}
