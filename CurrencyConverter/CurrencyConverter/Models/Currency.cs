using System;

namespace CurrencyConverter // A namespace is just a family name for our code. Every file uses the same one so they can see each other.
{
    // MODEL: a class is a blueprint.
    // A Currency object is one row of our rates table, e.g. Code = "EUR", RateToUsd = 0.92.
    // "public" means other files are allowed to use it.
    public class Currency
    {
        // A property is a variable that belongs to the object.
        // string = text. { get; set; } means "you can read it and change it".
        public string Code { get; set; }

        // decimal = high-precision decimal number. Best type for money.
        // Meaning: how many units of this currency you get for 1 US dollar.
        public decimal RateToUsd { get; set; }

        // A constructor is a special method that runs when we create an object with "new".
        // It has the same name as the class and fills in the starting values.
        public Currency(string code, decimal rateToUsd)
        {
            Code = code;            // copy the code we were given into the property
            RateToUsd = rateToUsd;  // copy the rate we were given into the property
        }
    }
}
