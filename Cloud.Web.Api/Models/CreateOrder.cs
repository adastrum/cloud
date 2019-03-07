using System.ComponentModel.DataAnnotations;

namespace Cloud.Web.Api.Models
{
    public class CreateOrder
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public decimal? Amount { get; set; }
    }
}
