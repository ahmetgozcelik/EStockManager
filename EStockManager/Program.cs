using Microsoft.EntityFrameworkCore;
using EStockManager.Infrastructure;
using EStockManager.Repositories.Interfaces;
using EStockManager.Services.Interfaces;
using EStockManager.Services.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EStockManager.MiddleWares;
using Microsoft.OpenApi.Models;
using EStockManager.Repositories.Concrete;

namespace EStockManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                // baðlantý dizesi okuma
                var connectionsString = builder.Configuration.GetConnectionString("PostgreSqlConnection");
                // DbContext yapýlandýrma
                options.UseNpgsql(connectionsString);
            });
            // repository pattern i DI container a kaydetme (Scoped yaþam döngüsü genellikle idealdir)
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            // jwt authentication þemasý
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // tokenýn nasýl doðrulanacaðý
                    options.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidateAudience = true, // kim için üretildi
                        ValidateIssuer = true, // kim tarafýndan üretildi
                        ValidateLifetime = true, // geçerlilik süresi
                        ValidateIssuerSigningKey = true, // secret key doðrulamasý

                        // appsetting.json'dan okunan deðerleri ayarlama
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),

                        // tokený kullanmadan önce gecikme süresi varsa
                        ClockSkew = TimeSpan.Zero
                    };
                });
            builder.Services.AddAuthentication();

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(c =>
            {
                // Swagger belgesinin temel bilgilerini tanýmlama
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EStockManager API", Version = "v1" });

                // JWT Authorization desteði ekleme
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Lütfen JWT Token'ýnýzý `Bearer ` kelimesinden sonra giriniz. (Örn: 'Bearer eyJhb...')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // JWT doðrulamasý gerektiren tüm uç noktalara (Authorize niteliði olanlara) kilit simgesi ekleme
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseCustomExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EStockManager API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
