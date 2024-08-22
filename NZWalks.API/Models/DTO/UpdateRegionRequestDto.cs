using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class UpdateRegionRequestDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Code should consists of 3 characters only.")]
        [MaxLength(3, ErrorMessage = "Code should consists of 3 characters only.")]
        public string Code { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Name should not exceed 100 characters.")]
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; }
    }
}
