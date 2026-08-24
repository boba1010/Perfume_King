using Microsoft.AspNetCore.Http;
using Perfume_King.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Perfume_King.Shared.Contracts.Requests
{
    public class CreatePerfumeRequest
    {

        [Required(ErrorMessage = "اسم العطر مطلوب")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.01, 10000, ErrorMessage = "يجب أن يكون السعر أكبر من صفر")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        public Category Category { get; set; }

        [Required(ErrorMessage = "الصورة مطلوبة")]
        public IFormFile Image { get; set; } = null!;
    }
}
