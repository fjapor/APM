using APM.WebApi.Models;
using APM.WebApi.Repositories;
using System;
using System.Globalization;
using System.Linq;

namespace APM.WebApi.Services
{
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly CultureInfo DefaultCulture = new CultureInfo("en-US");
        private readonly ICurrencyRepository CurrencyRepository;
        public CurrencyConversionService(ICurrencyRepository currencyRepository)
        {
            CurrencyRepository = currencyRepository;
        }

        public string GetConvertedText(decimal currentValue, CultureInfo destination)
        {
            return currentValue.ToString("c", destination.NumberFormat);
        }

        public decimal ConvertCurrency(decimal currentValue, CultureInfo destination)
        {
            if (destination == DefaultCulture)
                return currentValue;

            var conversionrate = GetConversionRate(DefaultCulture, destination);
            return Math.Round(conversionrate * currentValue, 3);
        }

        public string GetCurrencySymbol(CultureInfo culture)
        {
            if (culture.CultureTypes.HasFlag(CultureTypes.NeutralCultures))
                culture = CultureInfo.CreateSpecificCulture(culture.TwoLetterISOLanguageName);

            var ri = new RegionInfo(culture.LCID);
            return ri.CurrencySymbol;
        }

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