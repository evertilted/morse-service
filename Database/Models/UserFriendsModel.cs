using System.ComponentModel.DataAnnotations.Schema;

namespace morse_service.Database.Models
{
    [Table("user_friends")]
    public class UserFriendsModel
    {
        [Column("relation_id")]
        int RelationId { get; set; }

        [Column("user_id_a")]
        int UserIDA { get; set; }

        [Column("user_id_b")]
        int UserIDB { get; set; }
    }
}
