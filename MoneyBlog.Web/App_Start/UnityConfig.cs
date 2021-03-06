using MoneyBlog.Web.Controllers;
using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.Mvc5;

namespace MoneyBlog.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<MoneyBlog.DataLayer.IRepositories.IAdminRepository, MoneyBlog.DataLayer.Repositories.AdminRepository>();
            container.RegisterType<MoneyBlog.Services.IService.IAdminService, MoneyBlog.Services.Service.AdminService>();

            container.RegisterType<MoneyBlog.DataLayer.IRepositories.IArticleRepository, MoneyBlog.DataLayer.Repositories.ArticleRepository>();
            container.RegisterType<MoneyBlog.Services.IService.IArticleService, MoneyBlog.Services.Service.ArticleService>();

            container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<ManageController>(new InjectionConstructor());

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}