using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Resources;
using System.Text.Json.Serialization;

namespace SignalRchat.Model
{
	public class User

	{
		public int Id { get; set; }
		//[NotMapped]
		//public string ConnectionId { get; set; } = string.Empty;
		public string? Login { get; set; }
		public string? Email { get; set; }

		[JsonIgnore]
		public ICollection<Message>? Messages { get; set; }
	}
}

