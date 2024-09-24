using Microsoft.EntityFrameworkCore;

namespace SignalRchat.Model
{
	public class ChatContext : DbContext
	{

		public DbSet<User> user { get; set; }
		public DbSet<Message> message { get; set; }
		public ChatContext(DbContextOptions<ChatContext> options)
		 : base(options)

		{
			if (Database.EnsureCreated())
			{
				User u1 = new User { Login = "tanya", Email = "tanya@gmail.com" };
				User u2 = new User { Login = "misha", Email = "misha@gmail.com" };

				DateTime now = DateTime.Now;

				message?.Add(new Message
				{

					user = u1,
					Datetime = DateTime.Now,
					message = "Всем привет, это Таня",


				}); message?.Add(new Message
				{

					user = u2,
					Datetime = DateTime.Now,
					message = "Миша тут",


				});

				SaveChanges();
			}
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Message>().HasOne(m => m.user).WithMany(u => u.Messages).HasForeignKey(m => m.UserId);
		}
	}
}