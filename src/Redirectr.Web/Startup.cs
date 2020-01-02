using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Redirectr.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            var map = new ConcurrentDictionary<string, string>();

            var baseAddress = _configuration.GetBaseAddress();
            if (!baseAddress.EndsWith("/"))
            {
                baseAddress += "/";
            }

            var generator = new KeyGenerator();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/register/{to}", async context =>
                {
                    var to = (string) context.GetRouteValue("to");

                    if (map.TryGetValue(to, out var url))
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.MovedPermanently;
                        context.Response.Headers.Add("Location", url);
                    }
                    else
                    {
                        var key = generator.Generate();
                        map.TryAdd(key, to);
                        context.Response.Headers.TryAdd("Location", baseAddress + key);
                        context.Response.Headers.TryAdd("key", key);
                    }

                    await context.Response.CompleteAsync();
                });

                endpoints.MapGet("/{key}", async context =>
                {
                    var key = (string) context.GetRouteValue("key");
                    if (map.TryGetValue(key, out var url))
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.MovedPermanently;
                        context.Response.Headers.Add("Location", url);
                    }
                    else
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    }
                    await context.Response.CompleteAsync();
                });
            });
        }
    }
}