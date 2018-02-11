using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace UniriojaREST
{
    public class TestMiddleware
    {
        public TestMiddleware(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                // Do work that doesn't write to the Response.
                await context.Response.WriteAsync("Hello from midlware 1");
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
                await context.Response.WriteAsync("Bye from middlware 1");
            });

            app.Map("/map1", HandleMapTest1);

            app.UseMyCustomMiddleware();

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from Run delegate.");
            });
        }

        private static void HandleMapTest1(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 1");
            });
        }
        
    }
    
    public static class MyCustomExtension
    {
        public static IApplicationBuilder UseMyCustomMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyCustomMiddleware>();
        }
    }

    public class MyCustomMiddleware
    {
        RequestDelegate _next;
        public MyCustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }
    
        public async Task Invoke(HttpContext ctx)
        {
            await ctx.Response.WriteAsync("Hello from My Custom Middleware!!!<br/>");
            await _next.Invoke(ctx);
            await ctx.Response.WriteAsync("Bye from My Custom Middleware!!!<br/>");
        }
    }
}
