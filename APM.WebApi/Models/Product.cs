using System;
using System.ComponentModel.DataAnnotations;

namespace APM.WebApi.Models
{
    public class Product
    {
        public string Description { get; set; }
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Product Code is required", AllowEmptyStrings = false)]
        [MinLength(6, ErrorMessage = "Product Code min length is 6 characters")]
        public string ProductCode { get; set; }

        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required", AllowEmptyStrings = false)]
        [MinLength(4, ErrorMessage = "Product Name min length is 4 characters")]
        [MaxLength(12, ErrorMessage = "Product Name max length is 12 characters")]
        public string ProductName { get; set; }
        public DateTime ReleaseDate { get; set; }

        //In the backend all values will be handled in USD. I decided to go this way due to cost x benefit of time. (Faster implementation)
        //We can also implement martin-fowler "Money Pattern" that will lead to a more mainteinable and more complex implementation 
        //(However IDK about performance issues of this pattern and this way would require some community research, benchmark and profiling testing)
        public string CurrencySymbol { get; set; } 
        public string CultureFormattedPrice { get; set; }

    }
}