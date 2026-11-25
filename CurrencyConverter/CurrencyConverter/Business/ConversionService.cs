using System;

namespace CurrencyConverter
{
    // BUSINESS LAYER: It does the calculation and knows nothing about the screen or files.
    public class ConversionService
    {
        // The service needs rates to work, so it keeps a reference to the repository.
        private RateRepository rates;

        // CONSTRUCTOR: whoever creates the service must hand it a RateRepository.
        public ConversionService(RateRepository rateRepository)
        {
            rates = rateRepository; // store it so the other methods can use it
        }

        // How many "to" units equal 1 "from" unit? Example: 1 USD to EUR = 0.92.
        public decimal GetEffectiveRate(string from, string to)
        {
            return rates.GetRate(to) / rates.GetRate(from); // divide; decimals keep the fractions
        }

        // Converts an amount and returns the result.
        public decimal Convert(decimal amount, string from, string to)
        {
            decimal fromRate = rates.GetRate(from); // units of "from" per 1 USD
            decimal toRate = rates.GetRate(to);     // units of "to" per 1 USD

            // Formula: amount / fromRate gives the value in USD, then * toRate gives it in the target currency.
            // Example: 100 USD to EUR = 100 / 1 * 0.92 = 92
            decimal result = amount / fromRate * toRate;

            return Math.Round(result, 2); // Math.Round cuts to 2 decimal places, e.g. 92.0000 becomes 92.00
        }
    }
}
