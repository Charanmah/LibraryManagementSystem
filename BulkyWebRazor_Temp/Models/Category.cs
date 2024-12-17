using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazor_Temp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public required string Name { get; set; }
        [Range(0, 300, ErrorMessage = "The DisplayOrder must be in bw 1 to 150")]
        [DisplayName("Display order")]
        public int DisplayOrder { get; set; }
    }
}
