using System.ComponentModel.DataAnnotations;

namespace SignalRchat.Model
{
	public class RegModel
	{

		[Required(ErrorMessage = "Введите логин")]
		[Display(Name = "Логин: ")]
		public string? Login { get; set; }

		[Required(ErrorMessage = "Введите еmail")]
		[Display(Name = "Введите email: ")]
	
		public string? Email { get; set; }

		
	}
}
