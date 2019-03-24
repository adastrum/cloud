using System.ComponentModel.DataAnnotations;

namespace Cloud.Functions
{
    public class CreateOrder
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public decimal? Amount { get; set; }
    }
}
