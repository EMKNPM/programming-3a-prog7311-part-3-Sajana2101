using System.ComponentModel.DataAnnotations;

namespace GLMS2.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Region { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();

        
            }
}