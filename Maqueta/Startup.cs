using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Maqueta.Filtros;
using Maqueta.Middleware;
using Maqueta.Servicios;
using Maqueta.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Maqueta
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			
			services.AddControllers(options =>
			{
				options.Filters.Add(typeof(FiltroDeExcepcion));
				//options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
				//options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(new JsonSerializerOptions(JsonSerializerDefaults.Web)
				//{
				//	ReferenceHandler = ReferenceHandler.Preserve,
				//}));
			});
			services.AddControllers().AddNewtonsoftJson(x =>
 x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
			services.AddTransient<MiFiltroDeAccion>();
			services.AddResponseCaching();
			services.AddHostedService<EscribirEnArchivo>();
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(
				opciones => opciones.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
					ClockSkew = TimeSpan.Zero
				});
			var mappingConfig = new MapperConfiguration(mc =>
			{
				mc.AddProfile(new AutoMapperProfiles());
				
			});

			IMapper mapper = mappingConfig.CreateMapper();
			
			services.AddSingleton(mapper);
			services.AddIdentity<IdentityUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();
			services.AddMvc();
			services.AddDbContext<ApplicationDbContext>(options => { options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"));});
			//autorizacion basada en claims 
			services.AddAuthorization(opciones =>
			{
				opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("EsAdmin"));
			});
			// agregar politica de seguridad de cors para dominios web-.. basicamente yo le digo de que url permito hacer peticiones a mi apirest 
			// solo funciona para aplicacion web 
			services.AddCors(opcion =>
			{
				opcion.AddDefaultPolicy(builder =>
				{
					builder.WithOrigins("aquivalaurl").AllowAnyMethod().AllowAnyHeader(); //.WithExposedHeaders() //ppara exponer cabezeras que vas a devolver
				});

			});
			// acceso a los servicios de proteccion de datos
			services.AddDataProtection();

			// activo mi servicio de llaves 
			services.AddScoped<ServicioLlaves>();

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
			app.UseCors();// usaa cors
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
