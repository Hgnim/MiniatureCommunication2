using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MiniatureCommunication2.Database {
	public class ServerDbContext(DbContextOptions<ServerDbContext> opt) : IdentityDbContext<IdentityUser>(opt) {
		public DbSet<InviteCode> InviteCode => Set<InviteCode>();
		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);

			// 允许null
			builder.Entity<IdentityUser>()
				   .Property(u => u.Email)
				   .IsRequired(false)
				   .HasMaxLength(256);
			// 取消Email唯一索引
			builder.Entity<IdentityUser>()
				   .HasIndex(u => u.Email)
				   .IsUnique(false);
		}


		public DbSet<Userdata> Userdata => Set<Userdata>();
		public DbSet<ServerData> ServerData => Set<ServerData>();

		public DbSet<Conversation> Conversation => Set<Conversation>();
		public DbSet<ConversationMember> ConversationMember => Set<ConversationMember>();
		public DbSet<ConversationMessage> ConversationMessage => Set<ConversationMessage>();
	}
}
