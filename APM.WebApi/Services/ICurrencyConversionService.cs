using APM.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APM.WebApi.Services
{
    public interface ICurrencyConversionService
    {
        string GetConvertedText(decimal currentValue, CultureInfo destination);
        decimal ConvertCurrency(decimal currentValue, CultureInfo destination);
        void ConvertProductCurrency(Product p);
    }

}