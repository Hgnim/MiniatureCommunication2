using WebLoginDemo.Models;
using WebLoginDemo.Models.database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using static WebLoginDemo.Const.FilePath;
using static WebLoginDemo.DataCore;

namespace WebLoginDemo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
			Console.WriteLine(
@$"服务端启动"
				);
			try {
				static void saveData() => ConfigModel.SaveData(configFile, config);

				if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);

				if (!File.Exists(configFile))
					saveData();
				else
					config=ConfigModel.ReadData(configFile);

				if (config.UpdateConfig == true) {
					config.UpdateConfig = false;
					saveData();
					Console.WriteLine("配置文件已更新，已退出服务端");
					return;
				}
			} catch { Console.WriteLine("处理配置文件时出现错误!"); return; }


			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddControllersWithViews();

			builder.WebHost.UseUrls(config.Website.Url.Get());
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddSession();

			builder.Services.AddDbContext<ServerDbContext>(opt =>
				opt.UseSqlite("Data Source="+config.Website.Database.ConnectionString));

			#region Identity

			builder.Services.AddIdentity<IdentityUserModel, IdentityRole>()//不使用AddDefaultIdentity，使用本地页面与逻辑
				.AddEntityFrameworkStores<ServerDbContext>();
			    //.AddDefaultTokenProviders();//不需要添加对于令牌验证提供的程序

			builder.Services.Configure<IdentityOptions>(options => {
				// Password settings.
				options.Password.RequireDigit = false;//不要求数字
				options.Password.RequireLowercase = false;//不要求小写字母
				options.Password.RequireNonAlphanumeric = false;//不要求特殊字符
				options.Password.RequireUppercase = false;//不要求大写字母
				options.Password.RequiredLength = 6;//最小长度为6
				options.Password.RequiredUniqueChars = 1;//要求至少1个不同字符

				// Lockout settings.
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);//账户锁定时间为5分钟
				options.Lockout.MaxFailedAccessAttempts = 5;//5次错误后锁定账户
				options.Lockout.AllowedForNewUsers = true;//新用户允许账户锁定

				// User settings.
				options.User.AllowedUserNameCharacters =
				"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
				options.User.RequireUniqueEmail = false;//不检查邮箱唯一性

				// SignIn settings.
				options.SignIn.RequireConfirmedEmail = false;//不要求邮箱确认
				options.SignIn.RequireConfirmedAccount = false;//不要求账户确认
			});


			builder.Services.ConfigureApplicationCookie(options => {
				// Cookie settings
				options.Cookie.HttpOnly = true;//只能通过HTTP访问
				options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

				options.LoginPath = "/Account/Login";//登录页面
				options.AccessDeniedPath = "/Account/AccessDenied";
				options.SlidingExpiration = true;//允许滑动过期
			});

			//兼容Identity等页面，添加Razor Pages支持
			builder.Services.AddRazorPages();
			#endregion

			var app = builder.Build();

			//兼容Identity等页面，启用Razor Pages路由
			app.MapRazorPages();

			using (var scope = app.Services.CreateScope()) {
				//如果不存在则自动创建数据库文件
				scope.ServiceProvider.GetRequiredService<ServerDbContext>().Database.EnsureCreated();

				scope.ServiceProvider.GetRequiredService<ServerDbContext>().Database.Migrate(); // 自动应用迁移


				{
					//检查并创建角色
					RoleManager<IdentityRole> roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
					string[] roles = ["Owner", /*"Administrator", "Manager",*/ "User" ];
					foreach (var r in roles) {
						if (!await roleMgr.RoleExistsAsync(r))
							await roleMgr.CreateAsync(new IdentityRole(r));
					}
				}
				{
					//创建默认管理员用户
					string ownerUserName = "admin", ownerPassword = "Admin@0000",ownerRole = "Owner";
					UserManager<IdentityUserModel> userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUserModel>>();
					RoleManager<IdentityRole> roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

					//确保管理员用户存在
					IdentityUserModel? admin = await userMgr.FindByNameAsync(ownerUserName);
					if (admin == null) {
						admin = new IdentityUserModel {
							UserName = ownerUserName,
							//Email为空，不使用邮箱
						};

						IdentityResult createResult = await userMgr.CreateAsync(admin, ownerPassword);
						if (createResult.Succeeded)
							Console.WriteLine($"已创建默认管理员用户，用户名：{ownerUserName}，密码：{ownerPassword}");
						else
							throw new Exception("创建管理员失败：" + string.Join(", ", createResult.Errors));
					}

					//为管理员账户设置角色
					if (!await userMgr.IsInRoleAsync(admin, ownerRole)) {
						IdentityResult addRoleResult = await userMgr.AddToRoleAsync(admin, ownerRole);
						if (!addRoleResult.Succeeded)
							throw new Exception("给管理员加角色失败：" + string.Join(", ", addRoleResult.Errors));
					}
				}
			}

			app.UsePathBase(config.Website.Url.UrlRoot);
			app.UseSession();

			/*// Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }*/
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=StartPage}/{action=Index}"); // /{id?}");
			app.Run();
		}
	}
}
