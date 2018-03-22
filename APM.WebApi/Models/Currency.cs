using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace APM.WebApi.Models
{
    public enum Currency
    {
        [Description("Default Currency (USD)")]
        _Default,
        [Description("United States Dollars")]
        USD,
        [Description("Euros")]
        EUR
    }
}