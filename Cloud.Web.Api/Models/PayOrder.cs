using System.ComponentModel.DataAnnotations;

namespace Cloud.Web.Api.Models
{
    public class PayOrder
    {
        [Required]
        public decimal? Amount { get; set; }
    }
}
