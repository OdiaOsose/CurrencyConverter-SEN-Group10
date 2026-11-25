using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CurrencyConverter
{
    // DATA LAYER: keeps the conversion history and saves it in history.txt,
    // so it is still there after you close and reopen the program.
    // File format, one conversion per line, parts separated by "|":
    //   2026-11-25 14:30:05|USD|EUR|100|0.92|92.00
    public class HistoryRepository
    {
        private string filePath;                                              // where history.txt lives
        private List<ConversionRecord> records = new List<ConversionRecord>(); // all records, oldest first

        // Runs once when the repository is created: find the file and load it.
        public HistoryRepository()
        {
            filePath = Path.Combine(AppContext.BaseDirectory, "history.txt");
            LoadHistory();
        }

        // Reads history.txt into the list. A missing file simply means "no history yet".
        private void LoadHistory()
        {
            records.Clear();
            if (!File.Exists(filePath))   // no file yet (first run): stay empty, no crash
            {
                return;                   // "return" leaves the method early
            }

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split('|');   // cut the line at each "|"
                if (parts.Length == 6)              // a good line has exactly 6 parts
                {
                    decimal amount;
                    decimal rate;
                    decimal result;
                    // All three numbers must be readable. "&&" means AND. A damaged line is just skipped.
                    if (decimal.TryParse(parts[3], NumberStyles.Number, CultureInfo.InvariantCulture, out amount)
                        && decimal.TryParse(parts[4], NumberStyles.Number, CultureInfo.InvariantCulture, out rate)
                        && decimal.TryParse(parts[5], NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                    {
                        records.Add(new ConversionRecord(parts[0], parts[1], parts[2], amount, rate, result));
                    }
                }
            }
        }

        // Writes the whole list back to history.txt (replacing the old file).
        private void SaveHistory()
        {
            List<string> lines = new List<string>();
            foreach (ConversionRecord r in records)
            {
                // Turn each record into one line. InvariantCulture makes sure decimals always use a dot.
                string line = r.Timestamp + "|" + r.FromCode + "|" + r.ToCode + "|"
                    + r.Amount.ToString(CultureInfo.InvariantCulture) + "|"
                    + r.RateUsed.ToString(CultureInfo.InvariantCulture) + "|"
                    + r.Result.ToString(CultureInfo.InvariantCulture);
                lines.Add(line);
            }
            File.WriteAllLines(filePath, lines);
        }

        // Adds a new conversion and saves straight away, so nothing is lost if the program is closed.
        public void Add(ConversionRecord record)
        {
            records.Add(record);
            SaveHistory();
        }

        // Returns the history with the NEWEST conversion first.
        public List<ConversionRecord> GetNewestFirst()
        {
            List<ConversionRecord> newestFirst = new List<ConversionRecord>();
            // Loop backwards: start at the last item and count down to 0.
            for (int i = records.Count - 1; i >= 0; i--)
            {
                newestFirst.Add(records[i]);   // records[i] = the item at position i
            }
            return newestFirst;
        }

        // Deletes the whole history (in memory and in the file).
        public void Clear()
        {
            records.Clear();
            SaveHistory();
        }
    }
}
