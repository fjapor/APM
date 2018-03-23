using APM.WebApi.Models;
using APM.WebApi.Repositories;
using System;
using System.Globalization;
using System.Linq;

namespace APM.WebApi.Services
{
    /// <summary>
    /// Domain Service that handles Currency Conversion. It is based on the currency and .Net Framework countries information by 
    /// CultureInfo. (It can get the currency based on a specific location or a region)
    /// </summary>
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly CultureInfo DefaultCulture = new CultureInfo("en-US");
        private readonly ICurrencyRepository CurrencyRepository;
        public CurrencyConversionService(ICurrencyRepository currencyRepository)
        {
            CurrencyRepository = currencyRepository;
        }

        /// <summary>
        /// Get the decimal value formatted as a string using the CultureInfo format 
        /// (Some locations can have the currency symbol in the beginning, others in the end)
        /// </summary>
        /// <param name="currentValue">Value to be formatted by CultureInfo</param>
        /// <param name="destination">CultureInfo of the region of the value</param>
        /// <returns>A formatted string formatted with the Culture pattern</returns>
        public string GetConvertedText(decimal currentValue, CultureInfo destination)
        {
            return currentValue.ToString("c", destination.NumberFormat);
        }

        /// <summary>
        /// Convert a USD (en-US) currency to the destination culture currency
        /// </summary>
        /// <param name="currentValue">current value</param>
        /// <param name="destination">destination culture</param>
        /// <returns>Value converted to the currency, if we support the conversion. Otherwise it will return in default culture(en-US, USD)</returns>
        public decimal ConvertCurrency(decimal currentValue, CultureInfo destination)
        {
            if (destination == DefaultCulture)
                return currentValue;

            var conversionrate = GetConversionRate(DefaultCulture, destination);
            return Math.Round(conversionrate * currentValue, 3);
        }

        /// <summary>
        /// Returns the currency Symbol of the culture
        /// </summary>
        /// <param name="culture">Culture that will provide the currency symbol</param>
        /// <returns></returns>
        public string GetCurrencySymbol(CultureInfo culture)
        {
            if (culture.CultureTypes.HasFlag(CultureTypes.NeutralCultures))
                culture = CultureInfo.CreateSpecificCulture(culture.TwoLetterISOLanguageName);

            var ri = new RegionInfo(culture.LCID);
            return ri.CurrencySymbol;
        }

        /// <summary>
        /// Get the conversion rate in the repository 
        /// </summary>
        /// <param name="origin">Origin of the conversion</param>
        /// <param name="destination">Destination of the conversion</param>
        /// <returns></returns>
        private decimal GetConversionRate(CultureInfo origin, CultureInfo destination)
        {
            var originCurrencySymbol = new RegionInfo(origin.LCID).ISOCurrencySymbol;

            if (destination.CultureTypes.HasFlag(CultureTypes.NeutralCultures))
                destination = CultureInfo.CreateSpecificCulture(destination.TwoLetterISOLanguageName);

            var destinationCurrencySymbol = new RegionInfo(destination.LCID).ISOCurrencySymbol;

            if (CurrencyRepository.ExistsConversion(originCurrencySymbol, destinationCurrencySymbol))
                return CurrencyRepository.GetConversionRate(originCurrencySymbol, destinationCurrencySymbol);

            return 1;
        }

        /// <summary>
        /// Convert product price according to the conversion race (also shows correct currency)
        /// </summary>
        /// <param name="p">Product to have its price converted</param>
        /// <returns></returns>
        public void ConvertProductCurrency(Product p)
        {
            var destinationCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var originalPrice = p.Price;
            var convertedPrice = ConvertCurrency(p.Price, destinationCulture);

            if (originalPrice != convertedPrice ||
                originalPrice == 0)
            {
                p.Price = convertedPrice;
            }
            else
            {
                destinationCulture = DefaultCulture;
            }

            p.CurrencySymbol = GetCurrencySymbol(destinationCulture);
            p.CultureFormattedPrice = GetConvertedText(convertedPrice, destinationCulture);
        }

    }
}