using System.ComponentModel.DataAnnotations.Schema;

namespace morse_service.Database.Models
{
    [Table("users")]
    public class UserModel
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("login")]
        public string Login {  get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("display_name")]
        public string? DisplayName { get; set; }
    }
}
