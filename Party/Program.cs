using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Party.Data;
using Party.Models;

namespace Party
{
    public class Program
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true; // �n�D�b��T�{
//#if DEBUG
//                options.SignIn.RequireConfirmedAccount = false;
//                #else
//                   options.SignIn.RequireConfirmedAccount = true;
//                #endif
            })
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();


            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();// �[�J SignalR �A��

            // �K�[�h��y�t�ƪA��
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddControllersWithViews()
                .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            // �]�w�䴩���y���M�w�]�y��
            var supportedCultures = new[] { "en-US", "zh-TW", "fr-FR" };
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture(supportedCultures[0])
                       .AddSupportedCultures(supportedCultures)
                       .AddSupportedUICultures(supportedCultures);
            });
            // ��� ServiceProvider
            ServiceProvider = builder.Services.BuildServiceProvider();
            builder.Services.AddRazorPages();
            var app = builder.Build();
            // �ϥΥ��a��
            var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // �T�O�b UseAuthorization ���e�ϥΨ�������
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chatHub"); // �]�w SignalR Hub ������
            });

            app.MapRazorPages();

            app.Run();
        }
    }
}
