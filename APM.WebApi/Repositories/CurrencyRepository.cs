using APM.WebApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace APM.WebApi.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private const string ERR_UNSUPORTED_CONVERSION = "Unsuported currency conversion";

        //In-memory conversion mechanism, it can be ported to a database or whatever persistence needed,
        //as soon as the use of more currencies starts, this could be placed in a database
        //as conversion rates changes daily, i've decided to provide a json with the currencies 
        //I had some security problems that was taking too much time running the connectionString with attached file,
        //So i decided to manually attach the mdf to my sql server and updated the connectionstring in web.config file.


        private Dictionary<KeyValuePair<string, string>, decimal> _conversions = null;
        private Dictionary<KeyValuePair<string, string>, decimal> Conversions
        {
            get
            {
                if (_conversions == null)
                    _conversions = SetupDictionary(ReadCurrencyRatioFile());

                return _conversions;
            }
        }

        public CurrencyRepository()
        {
        }

        private List<CurrencyRatio> ReadCurrencyRatioFile()
        {
            var filePath = HostingEnvironment.MapPath(@"~/App_Data/currencyConversion.json");
            var json = File.ReadAllText(filePath);
            var currencyRatios = JsonConvert.DeserializeObject<List<CurrencyRatio>>(json);

            return currencyRatios;
        }

        private Dictionary<KeyValuePair<string, string>, decimal> SetupDictionary(List<CurrencyRatio> currencyRatios)
        {
            var inMemoryDictionary = new Dictionary<KeyValuePair<string, string>, decimal>();

            foreach (var currencyRatio in currencyRatios)
            {
                inMemoryDictionary.Add(
                    new KeyValuePair<string, string>(
                        currencyRatio.ISOCurrencySymbol_Origin,
                        currencyRatio.ISOCurrencySymbol_Destination),
                    currencyRatio.ConversionRate);
            }

            return inMemoryDictionary;
        }

        public bool ExistsConversion(string ISOCurrencySymbol_Origin, string ISOCurrencySymbol_Destination)
        {
            return Conversions.ContainsKey(new KeyValuePair<string, string>(ISOCurrencySymbol_Origin, ISOCurrencySymbol_Destination));
        }

        public decimal GetConversionRate(string ISOCurrencySymbol_Origin, string ISOCurrencySymbol_Destination)
        {
            decimal value = 0;

            if (Conversions.TryGetValue(new KeyValuePair<string, string>(ISOCurrencySymbol_Origin, ISOCurrencySymbol_Destination), out value))
                return value;

            //If the conversion is not in the list, but there is the reverse convertion there
            if (Conversions.TryGetValue(new KeyValuePair<string, string>(ISOCurrencySymbol_Destination, ISOCurrencySymbol_Origin), out value))
                return 1 / value;

            throw new InvalidCastException(ERR_UNSUPORTED_CONVERSION);
        }
    }
}