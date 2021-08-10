using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Serilog;

namespace DofusMarket.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/item-prices", WriteItemPrices);
            });
        }

        private async Task WriteItemPrices(HttpContext context)
        {
            if (!GetBasicAuthCredentials(context, out var creds)
                || creds.username != _configuration["DofusMarket:Authentication:Username"]
                || creds.password != _configuration["DofusMarket:Authentication:Password"])
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var itemPrices = (await JsonSerializer.DeserializeAsync<ItemPrice[]>(context.Request.Body))!;
            StringBuilder queryBuilder = new("INSERT INTO item_prices(time, server_id, item_id, item_type_id, stack_size, price) VALUES ");
            List<NpgsqlParameter> parameters = new(itemPrices.Length * 5);
            for (int i = 0; i < itemPrices.Length; i += 1)
            {
                int parameterIdx = i * 5;
                queryBuilder.AppendFormat("(NOW(), @{0}, @{1}, @{2}, @{3}, @{4}), ", parameterIdx, parameterIdx + 1,
                    parameterIdx + 2, parameterIdx + 3, parameterIdx + 4);
                parameters.Add(new NpgsqlParameter(parameterIdx.ToString(), itemPrices[i].ServerId));
                parameters.Add(new NpgsqlParameter((parameterIdx + 1).ToString(), itemPrices[i].ItemId));
                parameters.Add(new NpgsqlParameter((parameterIdx + 2).ToString(), itemPrices[i].ItemTypeId));
                parameters.Add(new NpgsqlParameter((parameterIdx + 3).ToString(), itemPrices[i].StackSize));
                parameters.Add(new NpgsqlParameter((parameterIdx + 4).ToString(), itemPrices[i].Price));
            }

            queryBuilder.Length -= ", ".Length;

            await using NpgsqlConnection conn = new(_configuration["ConnectionStrings:DofusMarket"]);
            await conn.OpenAsync();

            await using NpgsqlCommand cmd = new(queryBuilder.ToString(), conn);
            foreach (var parameter in parameters)
            {
                cmd.Parameters.Add(parameter);

            }

            await cmd.ExecuteNonQueryAsync();
            context.Response.StatusCode = (int)HttpStatusCode.Created;
        }

        public static bool GetBasicAuthCredentials(HttpContext context, out (string? username, string? password) creds)
        {
            string authorizeHeader = context.Request.Headers["Authorization"];
            if (authorizeHeader == null || !authorizeHeader.StartsWith("Basic ", StringComparison.Ordinal))
            {
                creds = (null, null);
                return false;
            }

            string base64Creds = authorizeHeader["Basic ".Length..];
            string[] parts = Encoding.UTF8.GetString(Convert.FromBase64String(base64Creds)).Split(':');
            creds = (parts[0], parts[1]);
            return true;
        }

        private record ItemPrice(int ServerId, int ItemId, int ItemTypeId, int StackSize, int Price);
    }
}
