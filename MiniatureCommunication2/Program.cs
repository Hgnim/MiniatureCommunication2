using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniatureCommunication2.Database;
using MiniatureCommunication2.Models;
using Serilog;
using System;
using System.Threading.Tasks;
using IdentityUser = MiniatureCommunication2.Database.IdentityUser;

namespace MiniatureCommunication2
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
			var builder = WebApplication.CreateBuilder(args);

			var env = builder.Environment;

			{//初始化日志系统
				var loggerConfig = new LoggerConfiguration()
														.MinimumLevel.Information()
														.Enrich.FromLogContext()
														.WriteTo.Console()//同时输出至控制台
														.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day/*按天输出日志文件*/);
				loggerConfig.ReadFrom.Configuration(builder.Configuration.GetSection("SerilogConfig"));//读取json中的配置

				Log.Logger = loggerConfig.CreateLogger();

				builder.Host.UseSerilog(Log.Logger);//将默认日志系统替换为serilog
			}
			if (env.IsDevelopment())
				Log.Information("当前为开发模式");
			Log.Information(
@"----------
微型通信2
版本：正在开发中，无版本号
----------"
);
			Log.Debug("服务端开始加载");
			
			///加载配置日志输出函数
			void LoadConfigLogger(string configKey,bool isSuccess,string? configValue=null) {
				if(isSuccess)
					Log.Debug($"已加载\"{configKey}\"配置：{configValue}");
				else
					Log.Warning($"未成功加载\"{configKey}\"配置");
			}

			builder.Services.AddControllersWithViews();

			{
				const string key = "Config:Server:Url";
				string? url = builder.Configuration.GetSection(key).Get<string>();
				if (url != null) {
					builder.WebHost.UseUrls(url);
					LoadConfigLogger(key, true, url);
				}
				else
					LoadConfigLogger(key, false);
			}
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddSession();

			{
				const string key = "Config:Database:ConnectionString";
				string? dbcs = builder.Configuration.GetSection(key).Get<string>();
				if (dbcs != null) {
					builder.Services.AddDbContext<ServerDbContext>(opt =>
						opt.UseSqlite("Data Source=" + dbcs));
					LoadConfigLogger(key, true, dbcs);
				}
				else
					LoadConfigLogger(key, false);
			}

			#region Identity

			builder.Services.AddIdentity<IdentityUser, IdentityRole>()//不使用AddDefaultIdentity，使用本地页面与逻辑
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
				options.Cookie.HttpOnly = true;//只能通过HTTP访问，无法通过js访问。防止xxs攻击
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
				ServerDbContext db = scope.ServiceProvider.GetRequiredService<ServerDbContext>();
				//当前是否没有数据库文件。如果为true则说明是第一次运行，数据库文件是新建的
				bool isNewDatabase = db.Database.EnsureCreated();//如果不存在则自动创建数据库文件
				db.Database.Migrate(); // 自动应用迁移
				if (isNewDatabase)
					Log.Information("数据库文件不存在，已新建数据库");
				else
					Log.Debug("数据库文件存在");

				{
					//检查并创建角色
					RoleManager<IdentityRole> roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
					string[] roles = ["Owner", /*"Administrator", "Manager",*/ "User"];
					foreach (var r in roles) {
						if (!await roleMgr.RoleExistsAsync(r))
							await roleMgr.CreateAsync(new IdentityRole(r));
					}
				}
				if (isNewDatabase) {
					{
						//创建默认管理员用户
						string ownerUserName = "admin", ownerPassword = "Admin@0000", ownerRole = "Owner";
						UserManager<Database.IdentityUser> userMgr = scope.ServiceProvider.GetRequiredService<UserManager<Database.IdentityUser>>();
						RoleManager<IdentityRole> roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

						//确保管理员用户存在
						Database.IdentityUser? admin = await userMgr.FindByNameAsync(ownerUserName);
						if (admin == null) {
							admin = new Database.IdentityUser {
								UserName = ownerUserName,
								//Email为空，不使用邮箱
							};

							IdentityResult createResult = await userMgr.CreateAsync(admin, ownerPassword);
							if (createResult.Succeeded)
								Log.Information($"已创建默认管理员用户，用户名：{ownerUserName}，密码：{ownerPassword}");
							else
								Log.Warning("创建默认管理员失败：" + string.Join(", ", createResult.Errors));
						}
						else
							Log.Debug($"默认管理员用户已存在");

						//为管理员账户设置角色
						if (!await userMgr.IsInRoleAsync(admin, ownerRole)) {
							IdentityResult addRoleResult = await userMgr.AddToRoleAsync(admin, ownerRole);
							if (addRoleResult.Succeeded)
								Log.Information($"已将默认管理员用户的身份设置为：{ownerRole}");
							else
								Log.Warning("给默认管理员设置身份时失败：" + string.Join(", ", addRoleResult.Errors));
						}
						else
							Log.Debug($"默认管理员用户当前身份为 {ownerRole} ，无需设置");
					}
					{
						db.Conversation.Add(new Conversation {
							Type = ConversationType.Group,
							Group_Title = "公共群组",
							Group_DisShowUserList = true,
							Group_ForceUserJoinOnReg = true,
						});
						await db.SaveChangesAsync();
						Log.Information("已创建默认群组");
					}
				}
			}

			{
				const string key = "Config:Server:PathBase";
				//PathBase不能只包含单独的斜杠；开头必须是斜杠，结尾不能是斜杠；可以为空或为null
				string ? pathBase = builder.Configuration.GetSection(key).Get<string>();
				if (pathBase != null) {
					StaticData.Config.PathBase = pathBase;//保存至静态数据中，更方便其它地方使用
					app.UsePathBase(pathBase);
					LoadConfigLogger(key, true, pathBase);
				}
				else
					LoadConfigLogger(key, false);
			}
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
				pattern: "{controller=Home}/{action=Index}"); // /{id?}");
			app.Run();
		}
	}
}
