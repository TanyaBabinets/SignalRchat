using System.ComponentModel.DataAnnotations;

namespace SignalRchat.Model
{
	public class LoginModel
	{
		[Required(ErrorMessage = "Логин неверный, вы зарегистрированы?")]
		[Display(Name = "Введите логин: ")]
		public string? Login { get; set; }
		
	}
}
