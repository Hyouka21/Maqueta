using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Maqueta.Filtros;
using Maqueta.Middleware;
using Maqueta.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Maqueta
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			
			services.AddControllers(options =>
			{
				options.Filters.Add(typeof(FiltroDeExcepcion));
				options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
				options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(new JsonSerializerOptions(JsonSerializerDefaults.Web)
				{
					ReferenceHandler = ReferenceHandler.Preserve,
				}));
			});
			services.AddTransient<MiFiltroDeAccion>();
			services.AddResponseCaching();
			services.AddHostedService<EscribirEnArchivo>();
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,ILogger<Startup> logger ,IWebHostEnvironment env)
		{
			//otra forma de crear una clase que haga de middleware sin especificar la clase llamando a un metodo estatico para usar con el app
			app.useLoguearRespuestaHttp();
			//app.UseMiddleware<LoguarRespuestaHttpMiddleware>();
			app.Map("/ruta1", app =>//middlerware
				 {
					 app.Run(async contexto =>
					 {
						 await contexto.Response.WriteAsync("Estoy interceptando la tuberia");
					 });
				 });
			
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();
			app.UseResponseCaching();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
