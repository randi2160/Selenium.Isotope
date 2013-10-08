// FitNesse.NET
// Copyright © 2012 StemSoft Software based on Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System;
using System.Text.RegularExpressions;
using fit;

namespace fitnesse.handlers
{
	public class SeleniumRecallHandler : AbstractSymbolHandler
	{
        private static Regex matchExpression = new Regex(@"\$\{(\w*)\}");
        public override bool Match(string searchString, System.Type type)
		{
			return matchExpression.IsMatch(searchString);
		}

		public override void HandleInput(Fixture fixture, Parse cell, Accessor accessor)
		{
			string cellText = cell.Text;
			string value = ResolveSymbols(cellText);
            accessor.Set(fixture, value);
            cell.SetBody((value == null ? "null" : value.ToString()) + Fixture.Gray("&lt;&lt;" + cellText));
		}

        private string ResolveSymbols(String cellText)
        {
            string value = cellText;
            while (matchExpression.IsMatch(value))
            {
                string symbol = ExtractNextSymbol(value);
                object recalledObject = Fixture.Recall(symbol);
                string symbolValue = recalledObject == null ? "null" : recalledObject.ToString();
                symbolValue = (symbolValue == null ? "null" : symbolValue);
                value = value.Replace("${" + symbol + "}", symbolValue);
            }
            return value;
        }
        
        private string ExtractNextSymbol(string cell)
		{
            Match match = matchExpression.Match(cell);
            string symbol = match.ToString();
			symbol = symbol.Substring(2,symbol.Length-3);
			return symbol;
		}
        public override bool HandleEvaluate(Fixture fixture, Parse cell, Accessor accessor)
        {
            string actual = GetActual(accessor, fixture).ToString();
            string expected = ResolveSymbols(cell.Text);
            return actual.Equals(expected,StringComparison.OrdinalIgnoreCase);
        }

        public override void HandleCheck(Fixture fixture, Parse cell, Accessor accessor)
        {
            bool evaluate = HandleEvaluate(fixture, cell, accessor);
            HandleInput(fixture, cell, accessor);
            if (evaluate)
            {
                fixture.Right(cell);
            }
            else
            {
                if (accessor.Get(fixture) == null)
                {
                    fixture.Wrong(cell, "null");
                }
                else
                {
                    fixture.Wrong(cell, accessor.Get(fixture).ToString());
                }
            }
        }

    }
}