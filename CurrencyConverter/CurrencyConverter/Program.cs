using System;

namespace CurrencyConverter
{
    class Program
    {
        static void Main()
        {
            // Build the three layers and connect them. "new" creates an object from a class.
            RateRepository rates = new RateRepository();                    // data layer
            ConversionService service = new ConversionService(rates);       // business layer, given the data layer
            ConsoleUI ui = new ConsoleUI(service, rates);                   // presentation layer, given the other two

            ui.Run(); // start the menu
        }
    }
}
