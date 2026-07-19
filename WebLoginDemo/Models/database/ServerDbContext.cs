using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebLoginDemo.Models.database {
	public class ServerDbContext(DbContextOptions<ServerDbContext> opt) : IdentityDbContext<IdentityUserModel>(opt) {
		public DbSet<InviteCode> InviteCode => Set<InviteCode>();
		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);

			// 允许null
			builder.Entity<IdentityUserModel>()
				   .Property(u => u.Email)
				   .IsRequired(false)
				   .HasMaxLength(256);
			// 取消Email唯一索引
			builder.Entity<IdentityUserModel>()
				   .HasIndex(u => u.Email)
				   .IsUnique(false);
		}


		public DbSet<UserdataModel> Userdata => Set<UserdataModel>();
		public DbSet<ServerDataModel> ServerData => Set<ServerDataModel>();
	}
}
