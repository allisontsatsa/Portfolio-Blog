using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Portfolio_Blog.Startup))]
namespace Portfolio_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
