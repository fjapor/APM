using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APM.WebApi.Repositories
{
    public interface ICurrencyRepository
    {
        bool ExistsConversion(string ISOCurrencySymbol_Origin, string ISOCurrencySymbol_Destination);
        decimal GetConversionRate(string ISOCurrencySymbol_Origin, string ISOCurrencySymbol_Destination);
    }
}
