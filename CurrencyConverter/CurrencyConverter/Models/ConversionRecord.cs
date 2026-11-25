using System;

namespace CurrencyConverter
{
    // MODEL: one finished conversion, kept in the history.
    // Like Currency, it only holds data (a row in the history table).
    public class ConversionRecord
    {
        public string Timestamp { get; set; }  // when it happened, e.g. "2026-11-25 14:30:05" (kept as text to keep things simple)
        public string FromCode { get; set; }   // e.g. "USD"
        public string ToCode { get; set; }     // e.g. "EUR"
        public decimal Amount { get; set; }    // the amount the user typed
        public decimal RateUsed { get; set; }  // 1 FromCode = RateUsed ToCode
        public decimal Result { get; set; }    // the converted amount

        // Constructor: fills all six values when we create a record with "new".
        public ConversionRecord(string timestamp, string fromCode, string toCode, decimal amount, decimal rateUsed, decimal result)
        {
            Timestamp = timestamp;
            FromCode = fromCode;
            ToCode = toCode;
            Amount = amount;
            RateUsed = rateUsed;
            Result = result;
        }
    }
}
