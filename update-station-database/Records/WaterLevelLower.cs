//
//  WaterLevelLower.cs
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

namespace Krafta.Records
{
	/// <summary>
	/// Represents an input water level record for the lower station.
	/// </summary>
	public struct WaterLevelLower : ISqlRecord
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Krafta.Records.WaterLevelLower"/> struct.
		/// Takes a raw line from "OVY.SKV" as input.
		/// </summary>
		/// <param name="InRecord">In record.</param>
		public WaterLevelLower(string InRecord)
		{
			string cleanRecord = InRecord.Replace(" ", "");

			string[] recordParts = cleanRecord.Split(';');

			this.Date = recordParts[0];
			this.Time = recordParts[1];

			if (String.IsNullOrWhiteSpace(recordParts[2]))
			{
				this.WaterLevel = 0;
			}
			else
			{
				this.WaterLevel = double.Parse(Utilities.Math.CorrectNumericRecordValue(recordParts[2], 2));
			}

			if (String.IsNullOrWhiteSpace(recordParts[3]))
			{
				this.HatchBorvs = 0;
			}
			else
			{
				this.HatchBorvs = uint.Parse(recordParts[3]);
			}

			if (recordParts.Length > 4)
			{
				this.State = recordParts[4] != "OFF";
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

		/// <summary>
		/// Gets the time when the record was created.
		/// </summary>
		/// <value>The time.</value>
		public string Time
		{
			get;
			private set;
		}

		// ...

		/// <summary>
		/// Gets the water level.
		/// </summary>
		/// <value>The water level (in metric meters).</value>
		public double WaterLevel
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the hatch value. I'll level with you, I've no idea what this value is.
		/// </summary>
		/// <value>The hatch borvs.</value>
		public uint HatchBorvs
		{
			get;
			private set;
		}

		// ...

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
			return "CREATE TABLE water_level_lower (" +
			"id INTEGER PRIMARY KEY AUTO_INCREMENT," +
			"date TEXT," +
			"time TEXT," +
			"water_level DECIMAL(5, 2)," +
			"hatch_borvs INT," +
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
			string baseInsertCommand = "INSERT INTO water_level_lower (date, time, water_level, hatch_borvs, state) " +
			                           "VALUES(@date, @time, @water_level, @hatch_borvs, @state)";

			string baseSelectCommand = "SELECT date, time FROM water_level_lower WHERE date LIKE @date AND time LIKE @time";

			MySqlCommand selectCommand = new MySqlCommand(baseSelectCommand, connection);
			selectCommand.Parameters.AddWithValue("@date", this.Date);
			selectCommand.Parameters.AddWithValue("@time", this.Time);

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
			insertCommand.Parameters.AddWithValue("@time", this.Time);
			insertCommand.Parameters.AddWithValue("@water_level", this.WaterLevel);
			insertCommand.Parameters.AddWithValue("@hatch_borvs", this.HatchBorvs);
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

