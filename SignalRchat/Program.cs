using SignalRchat;
using SignalRchat.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession();
builder.Services.AddDataProtection();
builder.Services.AddDistributedMemoryCache();


builder.Services.AddSignalR();
builder.Services.AddDbContext<ChatContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//var app = builder.Build();

//// Выполняем EnsureCreated один раз при запуске
//using (var scope = app.Services.CreateScope())
//{
//	var context = scope.ServiceProvider.GetRequiredService<ChatContext>();
//	context.Database.EnsureCreated();
//	Console.WriteLine("EnsureCreated is called from Program");
//}





string? connection = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<ChatContext>(options => options.UseSqlServer(connection));

var app = builder.Build();


app.UseHttpsRedirection();///????????

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();////????????
app.UseAuthorization();
app.UseSession();


app.MapHub<ChatHub>("/chat");   

app.Run();