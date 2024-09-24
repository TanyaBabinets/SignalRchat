using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRchat.Model;

namespace SignalRchat
{
	public class ChatHub : Hub
	{
		private readonly ChatContext _context;
		private static List<ChatModel> Users = new List<ChatModel>();
		public ChatHub(ChatContext context)
		{
			_context = context;
		}

		// Отправка сообщений
		public async Task Send(string username, string message, string date)
		{
			try
			{
				Console.WriteLine($"Received username: '{username}'");
				var user = await _context.user.FirstOrDefaultAsync(u => u.Login == username);//находим юзера по имени
				if (user != null)
				{
					var newMessage = new Message
					{
						message = message,
						Datetime = DateTime.Now,
						date = DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
						UserId = user.Id,
						user = user
					};
					await _context.message.AddAsync(newMessage); //сохраняем сообщение в базу данных

					await _context.SaveChangesAsync();
					// Вызов метода AddMessage на всех клиентах
					await Clients.All.SendAsync("AddMessage", username, message, date);

				}
				else
				{
					Console.WriteLine($"User with username '{username}' not found.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
			}
		}

		public async Task Connect(string username)
		{
			Console.WriteLine($"Received username: '{username}'"); //..........................
			var connectionId = Context.ConnectionId;
			var user = await _context.user.SingleOrDefaultAsync(x => x.Login == username);
			if (user != null)
			{
				if (!Users.Any(u => u.ConnectionId == connectionId))
				{

					Users.Add(new ChatModel { ConnectionId = connectionId, Name = username });
					await Clients.Caller.SendAsync("Connected", connectionId, username, Users);
					await Clients.AllExcept(connectionId).SendAsync("NewUserConnected", connectionId, username);
					var allUsers = await _context.user.ToListAsync();

					await Clients.All.SendAsync("AddMessage", null, $"{username} вошел в чат", null);
					Console.WriteLine("вошел в чат");
					await LoadMessagesAsync();
				}
				else
				{
					Console.WriteLine("Пользователь не найден, отправляем сообщение на клиент.");//////////////////////////////
					await Clients.Caller.SendAsync("LoginFailed", "Пользователь не найден.");
					return;
				}
			}
		}
	
		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			var connection = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
			if (connection != null)
			{
				Users.Remove(connection);
				await Clients.All.SendAsync("UserDisconnected", connection.ConnectionId, connection.Name);
				await Clients.All.SendAsync("AddMessage", $"{connection.Name} вышел из чата", null);
			}
			await base.OnDisconnectedAsync(exception);
		}
		public async Task Exit()
		{
			var connection = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
			if (connection != null)
			{
				Users.Remove(connection);
				await Clients.All.SendAsync("UserDisconnected", connection.ConnectionId, connection.Name);
				await Clients.All.SendAsync("AddMessage", $"{connection.Name} вышел из чата", null);
			}
			
		}


		public async Task<bool> CheckLogin(string login)//check login
		{
			var user = await _context.user.FirstOrDefaultAsync(u => u.Login == login);
			if (user != null)
			{
				var ConnectionId = Context.ConnectionId;
				
				var message = await _context.message.Include(m => m.user)
					.OrderByDescending(m => m.Datetime).Take(10).ToListAsync();
				await Clients.Caller.SendAsync("LoginSuccessful", user.Id, login, _context.user.ToList(), message);
				return true;
			}
			else
			{
				await Clients.Caller.SendAsync("LoginFailed", "Пользователь с таким логином не найден.");
				return false;
			};
		}
		public async Task RegisterUser(string login, string email)
		{
			try
			{
				if (string.IsNullOrEmpty(login))
				{
					throw new ArgumentException("Заполните поле Логин");
				}


				if (!_context.user.Any(u => u.Login == login))

				{
					var newUser = new User//если нет то создаем и записываем в БД
					{
						Login = login,
						Email = email,

					};
					await _context.user.AddAsync(newUser);
					await _context.SaveChangesAsync();
					await Clients.Caller.SendAsync("RegistrationSuccessful", newUser.Id, login);
				}
				else
				{
					await Clients.Caller.SendAsync("RegistrationFailed", "Пользователь с таким логином уже существует.");

				}
			}
			catch (Exception ex)
			{				
				Console.WriteLine("Ошибка при регистрации:", ex.Message);
				throw; 
			}
		}
		public async Task LoadMessagesAsync()
		{
			var messages = await _context.message
				.Include(m => m.user)
				.OrderByDescending(m => m.Datetime)
				.Take(10)
				 .Select(m => new
				 {
					 login = m.user.Login,
					 mes = m.message,
					 date = m.Datetime
				 })
				.ToListAsync();
			Console.WriteLine("Show messages:", messages); // /////////////////
			await Clients.Caller.SendAsync("LoadMes", messages);
		}

	}
}



