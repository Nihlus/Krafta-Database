//
//  ISqlRecord.cs
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
	/// Interface for SQL records, providing basic property fields and an insert function.
	/// </summary>
	public interface ISqlRecord
	{
		/// <summary>
		/// Gets the date.
		/// </summary>
		/// <value>The date.</value>
		string Date
		{
			get;
		}

		/// <summary>
		/// Gets the time.
		/// </summary>
		/// <value>The time.</value>
		string Time
		{
			get;
		}

		// ...

		/// <summary>
		/// Gets whether or not the turbine was on when this record was created.
		/// </summary>
		/// <value><c>true</c> if the turbine was on; otherwise, <c>false</c>.</value>
		bool State
		{
			get;
		}

		/// <summary>
		/// Inserts the record into the database if it is unique. Its uniqueness is determined by 
		/// the time and date of the record. If there is not a record in the database where these two match
		/// this record, it is unique.
		/// </summary>
		/// <returns><c>true</c>, if the record was inserted, <c>false</c> otherwise.</returns>
		bool InsertUnique(MySqlConnection connection);
	}
}

