using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Mvc.Razor;
using ModularAsp5_Beta8.Infrastructure;
using Microsoft.AspNet.Mvc.Infrastructure;
using System.Reflection;
using System.IO;
using ModularAsp5_Beta8.Extensions;

namespace ModularAsp5_Beta8
{
    public class Startup
    {

        private IFileProvider _modulesFileProvider;
        private readonly string ApplicationBasePath;
        private readonly IAssemblyLoadContextAccessor _assemblyLoadContextAccessor;
        private readonly IAssemblyLoaderContainer _assemblyLoaderContainer;

        public Startup(IHostingEnvironment hostingEnvironment,
            IApplicationEnvironment applicationEnvironment,
            IAssemblyLoaderContainer assemblyLoaderContainer,
            IAssemblyLoadContextAccessor assemblyLoadContextAccessor)
        {
            _assemblyLoadContextAccessor = assemblyLoadContextAccessor;
            _assemblyLoaderContainer = assemblyLoaderContainer;
            ApplicationBasePath = applicationEnvironment.ApplicationBasePath;


            // Setup configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(applicationEnvironment.ApplicationBasePath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime.
        public void ConfigureServices(IServiceCollection services)
        {

            var moduleAssemblies = LoadAssembliesFrom( ApplicationBasePath.Substring(0, ApplicationBasePath.LastIndexOf("src")) + "artifacts\\bin\\ModulesDLLs", _assemblyLoaderContainer, _assemblyLoadContextAccessor);

            string ParentPath = ApplicationBasePath.Substring(0, ApplicationBasePath.LastIndexOf("\\") + 1);

            List<string> BasePaths = new List<string>();

            BasePaths.Add(ApplicationBasePath);

            foreach (Assembly asmbly in moduleAssemblies)
            {
                BasePaths.Add(ParentPath + asmbly.GetName().Name);
            }


            

            _modulesFileProvider = GetModulesFileProvider(BasePaths.ToArray(), moduleAssemblies);


            services.AddInstance(Configuration);

            // Add MVC services to the services container.
            services.AddMvc();

            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();


            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.FileProvider = _modulesFileProvider;
            });
            

            services.AddInstance(new ModuleAssemblyLocator(moduleAssemblies));
            services.AddTransient<DefaultAssemblyProvider>();
            services.AddTransient<IAssemblyProvider, ModuleAwareAssemblyProvider>();


        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            // Configure the HTTP request pipeline.

            // Add the following to the request pipeline only in development environment.
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // send the request to the following path or controller action.
                app.UseExceptionHandler("/Home/Error");
            }

            // Add the platform handler to the request pipeline.
            app.UseIISPlatformHandler();

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                // Uncomment the following line to add a route for porting Web API 2 controllers.
                // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
            });
        }


        private List<Assembly> LoadAssembliesFrom(string modulesDirectory,
    IAssemblyLoaderContainer assemblyLoaderContainer,
    IAssemblyLoadContextAccessor loadContextAccessor)
        {
            var assemblies = new List<Assembly>();
            var loadContext = _assemblyLoadContextAccessor.GetLoadContext(typeof(Startup).GetTypeInfo().Assembly);
            using (assemblyLoaderContainer.AddLoader(new DirectoryLoader(modulesDirectory, loadContext)))
            {
                foreach (var modulePath in Directory.EnumerateFiles(modulesDirectory, "*.dll"))
                {
                    var name = Path.GetFileNameWithoutExtension(modulePath);
                    assemblies.Add(loadContext.Load(name));
                }
            }
            return assemblies;
        }
        private IFileProvider GetModulesFileProvider(string[] basePaths, List<Assembly> moduleAssemblies)
        {
            // TODO - probably want to set this to be debug only as it allows serving content outside the root directory
            var redirectedFileProviders = basePaths
                .Select(path => Path.IsPathRooted(path) ? path : Path.Combine(ApplicationBasePath, path))
                .Select(root => new PhysicalFileProvider(root));

            var resourceFileProviders = moduleAssemblies.Select(a => new SafeEmbeddedFileProvider(a));

            IFileProvider rootProvider = new PhysicalFileProvider(ApplicationBasePath);

            return new CompositeFileProvider(
                    rootProvider
                        .Concat(redirectedFileProviders)
                        .Concat(resourceFileProviders)
                        );


            
        }
    }
}
