using System;
using System.Collections.Generic;

namespace CurrencyConverter
{
    // PRESENTATION LAYER: the ONLY class that prints to the screen or reads the keyboard.
    public class ConsoleUI
    {
        private ConversionService service;   // does conversions and history
        private RateRepository rates;        // lists the currencies
        private InputValidator validator;    // checks what the user typed

        public ConsoleUI(ConversionService conversionService, RateRepository rateRepository, InputValidator inputValidator)
        {
            service = conversionService;
            rates = rateRepository;
            validator = inputValidator;
        }

        // Main loop: show menu, read choice, act, repeat until the user picks 0.
        public void Run()
        {
            bool running = true;
            while (running)
            {
                ShowMenu();
                string choice = ReadText("Choose an option: ");

                if (choice == "1")
                {
                    DoConversion();
                }
                else if (choice == "2")
                {
                    ShowRates();
                }
                else if (choice == "3")
                {
                    ShowHistory();
                }
                else if (choice == "4")
                {
                    ClearHistory();
                }
                else if (choice == "0")
                {
                    running = false;   // stops the loop
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter 1, 2, 3, 4 or 0.");
                }
            }
            Console.WriteLine("Goodbye!");
        }

        // Asks a question, waits for the user to type, returns the answer without extra spaces.
        private string ReadText(string question)
        {
            Console.Write(question);
            string? text = Console.ReadLine();   // "?" = could be empty (null)
            if (text == null)
            {
                return "";
            }
            return text.Trim();
        }

        private void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===== CURRENCY CONVERTER =====");
            Console.WriteLine("1. Convert currency");
            Console.WriteLine("2. View supported currencies and rates");
            Console.WriteLine("3. View conversion history");
            Console.WriteLine("4. Clear conversion history");
            Console.WriteLine("0. Exit");
        }

        private void ShowRates()
        {
            Console.WriteLine();
            Console.WriteLine("Supported currencies (units per 1 USD):");
            foreach (Currency c in rates.GetAllCurrencies())
            {
                Console.WriteLine($"  {c.Code}  {c.RateToUsd:N2}");
            }
        }

        // Keeps asking until the user gives a valid currency code (or types BACK).
        // "other" is the currency already chosen, so we can refuse the same one twice.
        private string AskCurrency(string question, string other)
        {
            while (true)   // repeat forever; the "return" lines below are the only ways out
            {
                string code = ReadText(question).ToUpper();

                if (code == "BACK")
                {
                    return "BACK";
                }

                if (!validator.IsSupportedCurrency(code))
                {
                    Console.WriteLine("Unsupported currency '" + code + "'. Choose option 2 on the menu to see valid codes.");
                }
                else if (validator.IsSameCurrency(code, other))
                {
                    Console.WriteLine("The two currencies must be different.");
                }
                else
                {
                    return code;   // valid, so give it back and leave the loop
                }
            }
        }

        // Keeps asking until the amount is a number between 0 (not included) and 1,000,000,000.
        // Returns false if the user typed BACK, true if "amount" now holds a good value.
        private bool AskAmount(out decimal amount)
        {
            while (true)
            {
                string text = ReadText("Amount: ");

                if (text.ToUpper() == "BACK")
                {
                    amount = 0;       // an "out" value must always be set before leaving
                    return false;
                }

                if (!validator.TryParseAmount(text, out amount))
                {
                    Console.WriteLine("Invalid amount. Please enter a number, for example 250.50");
                }
                else if (!validator.IsAmountInRange(amount))
                {
                    Console.WriteLine("Amount must be greater than 0 and not more than 1,000,000,000.");
                }
                else
                {
                    return true;      // good amount
                }
            }
        }

        private void DoConversion()
        {
            Console.WriteLine("(Type BACK at any question to return to the menu.)");

            string from = AskCurrency("From currency code (e.g. USD): ", "");
            if (from == "BACK") { return; }

            string to = AskCurrency("To currency code (e.g. EUR): ", from);
            if (to == "BACK") { return; }

            decimal amount;
            if (!AskAmount(out amount)) { return; }

            ConversionRecord r = service.ConvertAndRecord(amount, from, to);

            Console.WriteLine();
            Console.WriteLine($"{r.Amount:N2} {r.FromCode} = {r.Result:N2} {r.ToCode}");
            Console.WriteLine($"Rate used: 1 {r.FromCode} = {r.RateUsed:F4} {r.ToCode}");
        }

        private void ShowHistory()
        {
            List<ConversionRecord> list = service.GetHistory();   // newest first
            Console.WriteLine();
            if (list.Count == 0)
            {
                Console.WriteLine("No conversions yet.");
                return;
            }

            Console.WriteLine("Conversion history (newest first):");
            foreach (ConversionRecord r in list)
            {
                Console.WriteLine($"  {r.Timestamp}   {r.Amount:N2} {r.FromCode} = {r.Result:N2} {r.ToCode}   (rate {r.RateUsed:F4})");
            }
        }

        private void ClearHistory()
        {
            string answer = ReadText("Delete ALL history? Type y to confirm, anything else to cancel: ").ToLower();
            if (answer == "y")
            {
                service.ClearHistory();
                Console.WriteLine("History cleared.");
            }
            else
            {
                Console.WriteLine("Cancelled. History was not changed.");
            }
        }
    }
}
