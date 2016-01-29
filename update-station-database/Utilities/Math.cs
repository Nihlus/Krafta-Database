//
//  Math.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2016 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;

namespace Krafta.Utilities
{
	/// <summary>
	/// A bunch of helper functions for doing obscure or missing math.
	/// </summary>
	public static class Math
	{
		/// <summary>
		/// Corrects the temperature value by adding a decimal point to the raw string.
		/// </summary>
		/// <returns>The corrected temperature value.</returns>
		/// <param name="incorrectTemperatureValue">Incorrect temperature value.</param>
		/// <param name="decimalPlaces">The number of decimal places in the input value.</param>
		public static string CorrectNumericRecordValue(string incorrectTemperatureValue, int decimalPlaces)
		{
			int decimalPointIndex = incorrectTemperatureValue.Length - decimalPlaces;
			decimalPointIndex = Clamp(decimalPointIndex, 0, int.MaxValue);

			string correctTemperatureValue = incorrectTemperatureValue.Insert(decimalPointIndex, ".");
			return correctTemperatureValue;
		}

		/// <summary>
		/// Clamp the specified value, min and max.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public static int Clamp(int value, int min, int max)
		{  
			return (value < min) ? min : (value > max) ? max : value;  
		}
	}
}

