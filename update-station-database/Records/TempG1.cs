﻿//
//  EffectG1.cs
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
using MySql.Data.MySqlClient;
using System.Globalization;

namespace Krafta.Records
{
	/// <summary>
	/// Represents an input temperature record for the G1 turbine.
	/// </summary>
	public struct TempG1 : ISqlRecord
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Krafta.Records.TempG1"/> struct.
		/// Takes a raw line from "TEMP.SKV" as input.
		/// </summary>
		/// <param name="InRecord">In record.</param>
		public TempG1(string InRecord)
		{
			string cleanRecord = InRecord.Replace(" ", "");

			string[] recordParts = cleanRecord.Split(';');

			this.Date = recordParts[0] + " " + recordParts[1];

			if (String.IsNullOrWhiteSpace(recordParts[2]))
			{
				this.RearGeneratorBearingTemperature = 0;
			}
			else
			{
				this.RearGeneratorBearingTemperature = double.Parse(Utilities.Math.CorrectNumericRecordValue(recordParts[2], 1), NumberStyles.Any, CultureInfo.InvariantCulture);
			}

			if (String.IsNullOrWhiteSpace(recordParts[3]))
			{
				this.FrontGeneratorBearingTemperature = 0;
			}
			else
			{
				this.FrontGeneratorBearingTemperature = double.Parse(Utilities.Math.CorrectNumericRecordValue(recordParts[3], 1), NumberStyles.Any, CultureInfo.InvariantCulture);
			}

			if (String.IsNullOrWhiteSpace(recordParts[4]))
			{
				this.RearFlywheelBearingTemperature = 0;
			}
			else
			{
				this.RearFlywheelBearingTemperature = double.Parse(Utilities.Math.CorrectNumericRecordValue(recordParts[4], 1), NumberStyles.Any, CultureInfo.InvariantCulture);
			}

			if (String.IsNullOrWhiteSpace(recordParts[5]))
			{
				this.FrontFlywheelBearingTemperature = 0;
			}
			else
			{
				this.FrontFlywheelBearingTemperature = double.Parse(Utilities.Math.CorrectNumericRecordValue(recordParts[5], 1), NumberStyles.Any, CultureInfo.InvariantCulture);
			}

			if (String.IsNullOrWhiteSpace(recordParts[6]))
			{
				this.TurbineBearingTemperature = 0;
			}
			else
			{
				this.TurbineBearingTemperature = double.Parse(Utilities.Math.CorrectNumericRecordValue(recordParts[6], 1), NumberStyles.Any, CultureInfo.InvariantCulture);
			}

			if (recordParts.Length > 7)
			{
				this.State = recordParts[7] != "OFF";
			}
			else
			{
				this.State = true;
			}
		}

		/// <summary>
		/// Gets the date when the record was created.
		/// </summary>
		/// <value>The date.</value>
		public string Date
		{
			get;
			private set;
		}

		// ...

		/// <summary>
		/// Gets the rear generator bearing temperature.
		/// </summary>
		/// <value>The rear generator bearing temperature.</value>
		public double RearGeneratorBearingTemperature
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the front generator bearing temperature.
		/// </summary>
		/// <value>The front generator bearing temperature (in celcius).</value>
		public double FrontGeneratorBearingTemperature
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the rear flywheel bearing temperature.
		/// </summary>
		/// <value>The rear flywheel bearing temperature (in celcius).</value>
		public double RearFlywheelBearingTemperature
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the front flywheel bearing temperature.
		/// </summary>
		/// <value>The front flywheel bearing temperature (in celcius).</value>
		public double FrontFlywheelBearingTemperature
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the turbine bearing temperature.
		/// </summary>
		/// <value>The turbine bearing temperature (in celcius).</value>
		public double TurbineBearingTemperature
		{
			get;
			private set;
		}

		//...

		/// <summary>
		/// Gets the state of the turbine.
		/// </summary>
		/// <value><c>true</c> if the turbine was on at the time; otherwise, <c>false</c>.</value>
		public bool State
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the SQL table creation string.
		/// </summary>
		/// <returns>The table creation string.</returns>
		public static string GetTableCreationString()
		{
			return "CREATE TABLE temp_G1 (" +
			"id INTEGER PRIMARY KEY AUTO_INCREMENT," +
			"date DATETIME," +
			"rear_generator_bearing_temp DECIMAL(4, 1)," +
			"front_generator_bearing_temp DECIMAL(4, 1)," +
			"rear_generator_flywheel_temp DECIMAL(4, 1)," +
			"front_generator_flywheel_temp DECIMAL(4, 1)," +
			"turbine_bearing_temp DECIMAL(4, 1)," +
			"state BOOLEAN" +
			") ENGINE=MyISAM";
		}

		/// <summary>
		/// Inserts the record into the database if it is unique. Its uniqueness is determined by 
		/// the time and date of the record. If there is not a record in the database where these two match
		/// this record, it is unique.
		/// </summary>
		/// <returns><c>true</c>, if unique was inserted, <c>false</c> otherwise.</returns>
		public bool InsertUnique(MySqlConnection connection)
		{
			string baseInsertCommand = "INSERT INTO temp_G1 (date, rear_generator_bearing_temp, front_generator_bearing_temp," +
			                           "rear_generator_flywheel_temp, front_generator_flywheel_temp, turbine_bearing_temp, state) " +
			                           "VALUES(@date, @rear_generator_bearing_temp, @front_generator_bearing_temp, " +
			                           "@rear_generator_flywheel_temp, @front_generator_flywheel_temp, @turbine_bearing_temp, @state)";

			string baseSelectCommand = "SELECT date FROM temp_G1 WHERE date LIKE @date";

			MySqlCommand selectCommand = new MySqlCommand(baseSelectCommand, connection);
			selectCommand.Parameters.AddWithValue("@date", this.Date);

			try
			{
				using (var reader = selectCommand.ExecuteReader())
				{
					if (reader.HasRows)
					{
						return false;
					}
				}
			}
			catch (MySqlException mex)
			{
				Console.WriteLine(mex.Message);
			}

			MySqlCommand insertCommand = new MySqlCommand(baseInsertCommand, connection);
			insertCommand.Parameters.AddWithValue("@date", this.Date);
			insertCommand.Parameters.AddWithValue("@rear_generator_bearing_temp", this.RearGeneratorBearingTemperature);
			insertCommand.Parameters.AddWithValue("@front_generator_bearing_temp", this.FrontGeneratorBearingTemperature);
			insertCommand.Parameters.AddWithValue("@rear_generator_flywheel_temp", this.RearFlywheelBearingTemperature);
			insertCommand.Parameters.AddWithValue("@front_generator_flywheel_temp", this.FrontFlywheelBearingTemperature);
			insertCommand.Parameters.AddWithValue("@turbine_bearing_temp", this.TurbineBearingTemperature);
			insertCommand.Parameters.AddWithValue("@state", this.State);

			try
			{
				insertCommand.ExecuteNonQuery();
				return true;
			}
			catch (MySqlException mex)
			{
				Console.WriteLine(mex.Message);
				return false;
			}
		}
	}
}

