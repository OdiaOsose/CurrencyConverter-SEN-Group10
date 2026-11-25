using System;                    
using System.Collections.Generic; // gives us List<>, a list that grows as we add items
using System.Globalization;       // lets us read decimals the same way on every computer
using System.IO;                  // gives us File and Path, to read and write files

namespace CurrencyConverter
{
    // DATA LAYER: the only class that touches the rates file.
    // Nobody else knows or cares that rates live in a text file.
    public class RateRepository
    {
        // "private" = only this class can use it. This stores where the file is.
        private string filePath;

        // A List<Currency> is a list that holds Currency objects. "new List<Currency>()" makes it empty.
        private List<Currency> currencies = new List<Currency>();

        // CONSTRUCTOR: runs once when the program creates the repository.
        public RateRepository()
        {
            // AppContext.BaseDirectory = the folder where the program runs.
            // Path.Combine joins the folder and file name into a full path, so the file always sits next to the program.
            filePath = Path.Combine(AppContext.BaseDirectory, "rates.txt");
            LoadRates(); // load the rates straight away
        }

        // Makes sure we always end up with usable rates, even if the file is missing or damaged.
        private void LoadRates()
        {
            if (!File.Exists(filePath))   // "!" means NOT. If the file does NOT exist...
            {
                CreateDefaultFile();      // ...create it with default rates.
            }

            ReadFile();                   // read the file into our list

            if (currencies.Count == 0)    // Count = how many items are in the list. 0 means the file was empty or damaged.
            {
                CreateDefaultFile();      // overwrite it with good defaults
                ReadFile();               // and read again
            }
        }

        // Writes the default rates to rates.txt. One currency per line, written as CODE,RATE.
        private void CreateDefaultFile()
        {
            // An array of text lines. The rate is "how many of this currency equal 1 USD".
            string[] defaultLines =
            {
                "USD,1", "EUR,0.92", "GBP,0.79", "NGN,1500",
                "JPY,150", "CAD,1.36", "GHS,15", "ZAR,18.5"
            };
            File.WriteAllLines(filePath, defaultLines); // writes every line to the file (replaces what was there)
        }

        // Reads rates.txt and fills the currencies list.
        private void ReadFile()
        {
            currencies.Clear();                          // empty the list first so we never get duplicates
            string[] lines = File.ReadAllLines(filePath); // reads the whole file; each line becomes one item

            foreach (string line in lines)               // "for each line in the file, do this:"
            {
                string[] parts = line.Split(',');        // cut "EUR,0.92" at the comma into ["EUR", "0.92"]

                if (parts.Length == 2)                   // a good line has exactly 2 parts
                {
                    decimal rate;                        // empty box to hold the number
                    // TryParse tries to turn text into a number without crashing.
                    // It returns true/false (bool) and puts the number into "rate" ("out").
                    // InvariantCulture means "0.92" is always read with a dot, on any computer.
                    bool ok = decimal.TryParse(parts[1].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out rate);

                    if (ok && rate > 0)                  // && means AND: it worked AND the rate is positive
                    {
                        // Trim removes spaces, ToUpper makes capitals. Then we create a Currency and add it to the list.
                        currencies.Add(new Currency(parts[0].Trim().ToUpper(), rate));
                    }
                }
            }
        }

        // Answers: "do we support this currency code?" Returns true or false.
        public bool HasCurrency(string code)
        {
            string wanted = code.Trim().ToUpper();       // clean up what the user typed ("eur " becomes "EUR")
            foreach (Currency c in currencies)           // look at each currency in the list
            {
                if (c.Code == wanted)                    // == compares values
                {
                    return true;                         // found it, stop and answer true
                }
            }
            return false;                                // went through the whole list and found nothing
        }

        // Returns the rate for a code. Returns 0 if the code doesn't exist (callers should check HasCurrency first).
        public decimal GetRate(string code)
        {
            string wanted = code.Trim().ToUpper();
            foreach (Currency c in currencies)
            {
                if (c.Code == wanted)
                {
                    return c.RateToUsd;                  // found it, give back its rate
                }
            }
            return 0m;                                   // "m" marks a decimal number
        }

        // Gives the whole list so the UI can display it.
        public List<Currency> GetAllCurrencies()
        {
            return currencies;
        }
    }
}
