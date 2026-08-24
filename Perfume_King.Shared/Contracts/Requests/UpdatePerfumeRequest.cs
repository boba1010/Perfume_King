using Microsoft.AspNetCore.Http;
using Perfume_King.Shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Perfume_King.Shared.Contracts.Requests
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class UpdatePerfumeRequest
    {
        [Required(ErrorMessage = "Perfume Id is required")]
        public int Id { get; set; }

        public string? Name { get; set; }

        [Range(0.01, 10000, ErrorMessage = "يجب أن يكون السعر أكبر من صفر")]
        public decimal? Price { get; set; }

        public bool? IsAvailable { get; set; }

        public Category? Category { get; set; }

        public IFormFile? Image { get; set; }
    }

}
