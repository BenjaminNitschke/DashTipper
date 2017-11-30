using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DashTipper
{
	public class CryptoCurrencyAmountExtractor
	{
		public static bool TryExtract(string input, out decimal amountInMdash)
		{
			var amountWithSymbol = TryExtractCoinAmountWithSymbol(input);
			if (amountWithSymbol == null)
			{
				amountInMdash = default(decimal);
				return false;
			}
			amountInMdash = ConvertToMdash(amountWithSymbol);
			return true;
		}

		private static CoinAmountWithSymbol TryExtractCoinAmountWithSymbol(string input)
		{
			var matchCollection = CoinRegex.Matches(input);
			if (matchCollection.Count == 1)
				return TryExtractCoinAmountWithSymbol(matchCollection[0].Groups);
			return null;
		}

		private static readonly Regex CoinRegex = new Regex(CoinMatchPattern, CoinMatchOptions);
		private const string CoinMatchPattern =
			@"([-+]?[0-9]*\.?[0-9]+)\s?(dash|mdash|udash|duff)";
		private const RegexOptions CoinMatchOptions =
			RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

		private class CoinAmountWithSymbol
		{
			public CoinAmountWithSymbol(decimal amount, string symbol)
			{
				Amount = amount;
				Symbol = symbol;
			}

			public decimal Amount { get; }
			public string Symbol { get; }
		}

		private static CoinAmountWithSymbol TryExtractCoinAmountWithSymbol(
			GroupCollection matchedGroups)
		{
			string amountAsString = matchedGroups[1].Value;
			string symbol = matchedGroups[2].Value;
			decimal amount = decimal.Parse(amountAsString, CultureInfo.InvariantCulture);
			return new CoinAmountWithSymbol(amount, symbol);
		}

		private static decimal ConvertToMdash(CoinAmountWithSymbol amountWithSymbol)
		{
			decimal amount = amountWithSymbol.Amount;
			switch (amountWithSymbol.Symbol.ToLowerInvariant())
			{
			case "dash":
				return amount * 1000m;
			case "mdash":
				return amount;
			case "udash":
				return amount * 0.001m;
			case "duff":
				return amount * 0.00001m;
			default:
				throw new Exception($"Unsupported symbol: '{amountWithSymbol.Symbol}'");
			}
		}
	}
}