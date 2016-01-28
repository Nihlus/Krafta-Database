//
//  Program.cs
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
using System.Data.SqlClient;
using Krafta.Configuration;
using System.IO;
using System.Security;
using MySql.Data.MySqlClient;

namespace Krafta
{
	class MainClass
	{
		private const string SQL_SETUP = "";
		private const string SQL_INSERT_NEW_EFFECT_VALUE = "";
		private const string SQL_INSERT_NEW_ALARM_VALUE = "";
		private const string SQL_INSERT_NEW_TEMP_VALUE = "";
		private const string SQL_INSERT_NEW_FLYWHEEL_VALUE = "";

		public static int Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			// Execution path sketch

			// -- Initial startup checks --
			// Is there a config file? If so, load it; else, create a default one.
			ConfigurationHandler Config = ConfigurationHandler.Instance;

			// Do we have our default folders? If so, continue; else, create them.
			SetupInitialFolders();

			// -- Standard operation --
			// Establish a connection to the SQL database. If the connection could not be established, exit.
			MySqlConnectionStringBuilder connectionString = new MySqlConnectionStringBuilder();
			connectionString.Server = Config.DatabaseHost;
			connectionString.Port = uint.Parse(Config.DatabasePort);
			connectionString.UserID = Config.DatabaseUsername;
			connectionString.Password = Config.DatabasePassword;

			using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
			{
				try
				{
					connection.Open();

					// If previous data exists, load it.
					// If new data exists, load it. If not, exit.
				}
				catch (MySqlException mex)
				{
					Console.WriteLine(mex.Message);
					return 1;
				}
			}

			return 0;
		}

		private static void SetupInitialFolders()
		{

			if (!Directory.Exists(GetOldLowerStationDataDirectory()))
			{
				Directory.CreateDirectory(GetOldLowerStationDataDirectory());
			}

			if (!Directory.Exists(GetOldUpperStationDataDirectory()))
			{
				Directory.CreateDirectory(GetOldUpperStationDataDirectory());
			}

			if (!Directory.Exists(GetNewLowerStationDataDirectory()))
			{
				Directory.CreateDirectory(GetNewLowerStationDataDirectory());
			}

			if (!Directory.Exists(GetNewUpperStationDataDirectory()))
			{
				Directory.CreateDirectory(GetNewUpperStationDataDirectory());
			}
		}

		private static string GetOldLowerStationDataDirectory()
		{
			return "data" + Path.DirectorySeparatorChar + "old" + Path.DirectorySeparatorChar + "lower";
		}

		private static string GetOldUpperStationDataDirectory()
		{
			return "data" + Path.DirectorySeparatorChar + "old" + Path.DirectorySeparatorChar + "upper";
		}

		private static string GetNewLowerStationDataDirectory()
		{
			return "data" + Path.DirectorySeparatorChar + "new" + Path.DirectorySeparatorChar + "lower";
		}

		private static string GetNewUpperStationDataDirectory()
		{
			return "data" + Path.DirectorySeparatorChar + "new" + Path.DirectorySeparatorChar + "upper";
		}
	}

	/// <summary>
	/// Different error states that the program may be in when exiting.
	/// The value of each enum is the return code of the program.
	/// </summary>
	public enum ErrorTypes
	{
		/// <summary>
		/// A connection to the database in krafta.cfg could not be established.
		/// </summary>
		CouldNotEstablishDatabaseConnection = 1,

		/// <summary>
		/// No new data was provided to the program, and as such the database was not updated.
		/// </summary>
		NoNewDataProvided = 2,
	}
}
