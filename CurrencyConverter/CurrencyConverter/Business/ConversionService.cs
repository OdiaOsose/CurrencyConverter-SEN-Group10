using System;
using System.Collections.Generic;
using System.Globalization;

namespace CurrencyConverter
{
    // BUSINESS LAYER: the "brain". It calculates and records conversions.
    // It never prints and never reads the keyboard.
    public class ConversionService
    {
        private RateRepository rates;       // where the rates come from
        private HistoryRepository history;  // where finished conversions are saved

        // Whoever creates the service must give it both repositories.
        public ConversionService(RateRepository rateRepository, HistoryRepository historyRepository)
        {
            rates = rateRepository;
            history = historyRepository;
        }

        // How many "to" units equal 1 "from" unit? Example: USD to EUR = 0.92.
        public decimal GetEffectiveRate(string from, string to)
        {
            return rates.GetRate(to) / rates.GetRate(from);
        }

        // Pure calculation: amount / fromRate gives USD, then * toRate gives the target currency.
        public decimal Convert(decimal amount, string from, string to)
        {
            decimal fromRate = rates.GetRate(from);
            decimal toRate = rates.GetRate(to);
            decimal result = amount / fromRate * toRate;
            return Math.Round(result, 2);   // 2 decimal places
        }

        // Converts, saves the conversion in the history, and returns the record so the UI can show it.
        public ConversionRecord ConvertAndRecord(decimal amount, string from, string to)
        {
            string cleanFrom = from.Trim().ToUpper();
            string cleanTo = to.Trim().ToUpper();

            decimal result = Convert(amount, cleanFrom, cleanTo);
            decimal rate = GetEffectiveRate(cleanFrom, cleanTo);
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); // current date and time as text

            ConversionRecord record = new ConversionRecord(now, cleanFrom, cleanTo, amount, rate, result);
            history.Add(record);   // the data layer saves it to the file
            return record;
        }

        // History, newest first.
        public List<ConversionRecord> GetHistory()
        {
            return history.GetNewestFirst();
        }

        // Deletes all history.
        public void ClearHistory()
        {
            history.Clear();
        }
    }
}
