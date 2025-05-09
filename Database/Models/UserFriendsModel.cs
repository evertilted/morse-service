using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace morse_service.Database.Models
{
    [Table("user_friends")]
    public class UserFriendsModel
    {
        [Key]
        [Column("relation_id")]
        public int RelationId { get; set; }

        [Column("user_id_a")]
        public int UserIDA { get; set; }

        [Column("user_id_b")]
        public int UserIDB { get; set; }
    }
}
