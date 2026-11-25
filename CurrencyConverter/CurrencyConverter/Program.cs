using System;

namespace CurrencyConverter
{
    class Program
    {
        // The program starts here.
        static void Main()
        {
            // Build the layers and connect them. "new" creates an object from a class.
            RateRepository rates = new RateRepository();                              // data: rates
            HistoryRepository history = new HistoryRepository();                      // data: history
            InputValidator validator = new InputValidator(rates);                     // business: input rules
            ConversionService service = new ConversionService(rates, history);        // business: conversion
            ConsoleUI ui = new ConsoleUI(service, rates, validator);                  // presentation

            ui.Run();   // start the menu
        }
    }
}
