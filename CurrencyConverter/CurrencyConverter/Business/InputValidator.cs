using System;

namespace CurrencyConverter
{
    // BUSINESS LAYER: all the input rules live here, in one place.
    // It only answers true/false. It never prints anything (the UI does that).
    public class InputValidator
    {
        private RateRepository rates;   // needed to check which currencies exist

        // The biggest amount we accept (requirement FR6). "m" marks a decimal number.
        private const decimal MaxAmount = 1000000000m;

        public InputValidator(RateRepository rateRepository)
        {
            rates = rateRepository;
        }

        // Is this currency code one we support? (not case-sensitive)
        public bool IsSupportedCurrency(string code)
        {
            return rates.HasCurrency(code);
        }

        // Are the two codes the same currency? (e.g. USD to usd)
        public bool IsSameCurrency(string from, string to)
        {
            return from.Trim().ToUpper() == to.Trim().ToUpper();
        }

        // Can the text be read as a number? If yes, the number is put into "amount".
        public bool TryParseAmount(string text, out decimal amount)
        {
            return decimal.TryParse(text, out amount);
        }

        // Is the amount more than 0 and not above the maximum?
        public bool IsAmountInRange(decimal amount)
        {
            return amount > 0 && amount <= MaxAmount;   // && means AND
        }
    }
}
