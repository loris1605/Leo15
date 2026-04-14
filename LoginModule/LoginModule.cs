using Core;
using DTO.Repository;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using View;
using ViewModel;

namespace LoginModule
{
    public class LoginModule : IAppModule
    {
        public void Register(IServiceCollection services)
        {
            // Sposta qui le registrazioni che erano nel Main
            services.AddTransient<ILoginRepository, LoginRepository>();
            services.AddTransient<LoginVM>();
            services.AddTransient<IViewFor<LoginVM>, LoginV>();
        }
    }
}
