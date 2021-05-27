using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QuotesAPI.Models
{
    public class Quote
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(20)]
        public string Author { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required] // Måste fyllas i
        [StringLength(20)] // Bestämmer max antal tecken som är tillåtet
        public string Type { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }


        [JsonIgnore] // Bestämmer att detta ej ska visas i seponsen av apin
        public string UserId { get; set; }


        public Quote()
        {
        }
    }
}
