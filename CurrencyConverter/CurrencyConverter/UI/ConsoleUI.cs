using System;

namespace CurrencyConverter
{
    // PRESENTATION LAYER: the ONLY class that prints to the screen or reads the keyboard.
    // It asks the service to do the work and just shows the answers.
    public class ConsoleUI
    {
        private ConversionService service; // used to do conversions
        private RateRepository rates;      // used to list the currencies

        // CONSTRUCTOR: receives the two objects it needs.
        public ConsoleUI(ConversionService conversionService, RateRepository rateRepository)
        {
            service = conversionService;
            rates = rateRepository;
        }

        // The main loop: show menu, read choice, act, repeat until the user picks 0.
        public void Run()
        {
            bool running = true;              // bool = true/false. Controls the loop.

            while (running)                   // keep repeating while running is true
            {
                ShowMenu();                                   // print the menu
                string choice = ReadText("Choose an option: "); // read what the user typed

                if (choice == "1")            // == compares (slide 22)
                {
                    DoConversion();
                }
                else if (choice == "2")       // "else if" = otherwise, if this is true...
                {
                    ShowRates();
                }
                else if (choice == "0")
                {
                    running = false;          // one = stores a value. The loop will now stop.
                }
                else                          // anything else
                {
                    Console.WriteLine("Invalid choice. Please enter 1, 2 or 0.");
                }
            }

            Console.WriteLine("Goodbye!");
        }

        // Prints a question, waits for the user to type, and returns the cleaned-up answer.
        // Written once here so we don't repeat the same lines everywhere.
        private string ReadText(string question)
        {
            Console.Write(question);          // Write stays on the same line
            string? text = Console.ReadLine(); // ReadLine gets what the user typed. The ? just means "could be empty (null)".
            if (text == null)                 // if nothing came back...
            {
                return "";                    // ...give back empty text instead of crashing
            }
            return text.Trim();               // Trim removes spaces at the start and end
        }

        // Prints the menu.
        private void ShowMenu()
        {
            Console.WriteLine();              // blank line for spacing
            Console.WriteLine("===== CURRENCY CONVERTER =====");
            Console.WriteLine("1. Convert currency");
            Console.WriteLine("2. View supported currencies and rates");
            Console.WriteLine("0. Exit");
        }

        // Prints every currency with its rate.
        private void ShowRates()
        {
            Console.WriteLine();
            Console.WriteLine("Supported currencies (units per 1 USD):");
            foreach (Currency c in rates.GetAllCurrencies()) // loop through every Currency in the list
            {
                Console.WriteLine($"  {c.Code}  {c.RateToUsd:N2}"); // $ lets us put variables in { }. N2 = 2 decimals with commas
            }
        }

        // Asks for two currencies and an amount, converts, and shows the result.
        private void DoConversion()
        {
            string from = ReadText("From currency code (e.g. USD): ").ToUpper(); // ToUpper makes "usd" become "USD"
            if (!rates.HasCurrency(from))                                        // if the code is NOT supported...
            {
                Console.WriteLine("Unsupported currency: " + from);              // + joins text
                return;                                                          // leave this method and go back to the menu
            }

            string to = ReadText("To currency code (e.g. EUR): ").ToUpper();
            if (!rates.HasCurrency(to))
            {
                Console.WriteLine("Unsupported currency: " + to);
                return;
            }

            string amountText = ReadText("Amount: "); // what the user typed is TEXT, not yet a number
            decimal amount;                           // empty box for the number
            // TryParse = safe version of Parse. It puts the number in "amount" and answers true/false.
            if (!decimal.TryParse(amountText, out amount))
            {
                Console.WriteLine("Invalid amount. Please enter a number.");
                return;
            }

            decimal result = service.Convert(amount, from, to);  // ask the business layer to calculate
            decimal rate = service.GetEffectiveRate(from, to);   // and for the rate it used

            Console.WriteLine();
            Console.WriteLine($"{amount:N2} {from} = {result:N2} {to}");        // e.g. 100.00 USD = 92.00 EUR
            Console.WriteLine($"Rate used: 1 {from} = {rate:F4} {to}");          // F4 = 4 decimal places
        }
    }
}
