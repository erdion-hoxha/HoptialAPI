using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HosptialAPI
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services) {

            //var builder = services.AddIdentityCore<UserAPI>(q => q.User.RequireUniqueEmail = true);

            //builder = new Microsoft.AspNetCore.Identity.IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            //builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
        
        }
    }
}
