using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRKošarka.Domain.Common
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }
        public DateTime? DateDeleted { get; set; }

        [Column(TypeName = "varchar(450)")]
        public string? CreatedBy { get; set; } 
        [Column(TypeName = "varchar(450)")]
        public string? ModifiedBy { get; set; } 
        [Column(TypeName = "varchar(450)")]
        public string? DeletedBy { get; set; }
    }
}
