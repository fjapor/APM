using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.WebApi.Models
{
    public class CurrencyRatio
    {
        public string ISOCurrencySymbol_Origin { get; set; }
        public string ISOCurrencySymbol_Destination { get; set; }
        public decimal ConversionRate { get; set; }
    }
}