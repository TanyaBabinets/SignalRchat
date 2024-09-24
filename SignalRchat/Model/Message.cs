using SignalRchat.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SignalRchat.Model
{
	public class Message
	{
		public int Id { get; set; }

		public string? message { get; set; }
		
		public DateTime? Datetime { get; set; }
		[NotMapped]
		public string? date { get; set; }
		public int UserId { get; set; }
		[JsonIgnore]
		public User? user { get; set; }
	}
}

