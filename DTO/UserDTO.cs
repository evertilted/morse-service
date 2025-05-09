using System.ComponentModel.DataAnnotations.Schema;

namespace morse_service.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string? DisplayName { get; set; }
    }
}
