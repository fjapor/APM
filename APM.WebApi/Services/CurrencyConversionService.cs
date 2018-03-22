using APM.WebApi.Models;
using APM.WebApi.Repositories;
using System;
using System.Globalization;
using System.Linq;

namespace APM.WebApi.Services
{
    /// <summary>
    /// Domain service to handle currency functionality
    /// </summary>
    /// <remarks>
    /// In response to #5 - probably we could also implement the Martin-Fowler "Money pattern", 
    /// so we avoid errors of mixing currencies arithmetics, and we could gain speed in terms of minimizing other developers errors,
    /// however idk the performance impact(never implemented it). I would need some research and profiling tests. 
    ///
    /// For reference: 
    /// https://martinfowler.com/eaaCatalog/money.html
    /// https://code.tutsplus.com/tutorials/money-pattern-the-right-way-to-represent-value-unit-pairs--net-35509
    ///
    /// Found a good C# work of Denis Shcuka in CodeProject, but it seems to fail SOLID principles 
    /// and doesn't implement Strategy pattern to have an easier extensibility
    /// https://www.codeproject.com/Articles/837791/Money-pattern
    ///
    ///Also a good work is shown below: but maybe this would increase a lot the complexity of the software
    ///https://github.com/zpbappi/money
    ///http://zpbappi.com/multi-currency-generic-money-in-csharp/
    /// 
    /// So, i decided to go with a CurrencyConversionService as a first approach considering all internal money manipulation are dollar-based and
    /// the currency conversions are made only to show data in frontend

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