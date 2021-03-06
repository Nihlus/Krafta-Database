﻿//
//  FlywheelG3.cs
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
	public struct FlywheelG3 : ISqlRecord
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Krafta.Records.FlywheelG3"/> struct.
		/// Takes a raw line from "LOPHJUL.SKV" as input.
		/// </summary>
		/// <param name="InRecord">In record.</param>
		public FlywheelG3(string InRecord)
		{
			string cleanRecord = InRecord.Replace(" ", "");

			string[] recordParts = cleanRecord.Split(';');

			this.Date = recordParts[0] + " " + recordParts[1];

			if (String.IsNullOrWhiteSpace(recordParts[3]))
			{
				this.Throttle = 0;
			}
			else
			{
				this.Throttle = double.Parse(Utilities.Math.CorrectNumericRecordValue(recordParts[3], 1), NumberStyles.Any, CultureInfo.InvariantCulture);
			}

			if (String.IsNullOrWhiteSpace(recordParts[4]))
			{
				this.HatchLevel = 0;
			}
			else
			{
				this.HatchLevel = double.Parse(Utilities.Math.CorrectNumericRecordValue(recordParts[4], 1), NumberStyles.Any, CultureInfo.InvariantCulture);
			}

			if (String.IsNullOrWhiteSpace(recordParts[5]))
			{
				this.OVY = 0;
			}
			else
			{
				this.OVY = uint.Parse(recordParts[5], NumberStyles.Any, CultureInfo.InvariantCulture);
			}

			if (recordParts.Length > 6)
			{
				this.State = recordParts[6] != "OFF";
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
		/// Gets the throttle.
		/// </summary>
		/// <value>The throttle.</value>
		public double Throttle
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the hatch level.
		/// </summary>
		/// <value>The hatch level.</value>
		public double HatchLevel
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the OVY value. I've got no idea what this value actually is.
		/// </summary>
		/// <value>The OVY.</value>
		public uint OVY
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
			return "CREATE TABLE flywheel_G3 (" +
			"id INTEGER PRIMARY KEY AUTO_INCREMENT," +
			"date DATETIME," +
			"throttle DECIMAL(5, 2)," +
			"hatch_level DECIMAL(5, 2)," +
			"OVY INT," +
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
			string baseInsertCommand = "INSERT INTO flywheel_G3 (date, throttle, hatch_level, OVY, state) VALUES(@date, @throttle, @hatch_level, @OVY, @state)";
			string baseSelectCommand = "SELECT date FROM flywheel_G3 WHERE date LIKE @date";

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
			insertCommand.Parameters.AddWithValue("@throttle", this.Throttle);
			insertCommand.Parameters.AddWithValue("@hatch_level", this.HatchLevel);
			insertCommand.Parameters.AddWithValue("@OVY", this.OVY);
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

