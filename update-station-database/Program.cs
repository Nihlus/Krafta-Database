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
using System.Collections.Generic;
using Krafta.Records;
using System.Linq;

namespace Krafta
{
	class MainClass
	{
		public static int Main(string[] args)
		{
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
					connection.ChangeDatabase(Config.DatabaseName);

					// -- Database setup --
					if (!DoesTableExist("effect_G1", connection))
					{
						MySqlCommand createTable = new MySqlCommand(EffectG1.GetTableCreationString(), connection);
						createTable.ExecuteNonQuery();
					}

					if (!DoesTableExist("effect_G2", connection))
					{
						MySqlCommand createTable = new MySqlCommand(EffectG2.GetTableCreationString(), connection);
						createTable.ExecuteNonQuery();
					}

					if (!DoesTableExist("effect_G3", connection))
					{
						MySqlCommand createTable = new MySqlCommand(EffectG3.GetTableCreationString(), connection);
						createTable.ExecuteNonQuery();
					}

					if (!DoesTableExist("temp_G1", connection))
					{
						MySqlCommand createTable = new MySqlCommand(TempG1.GetTableCreationString(), connection);
						createTable.ExecuteNonQuery();
					}		

					if (!DoesTableExist("temp_G2", connection))
					{
						MySqlCommand createTable = new MySqlCommand(TempG2.GetTableCreationString(), connection);
						createTable.ExecuteNonQuery();
					}	

					if (!DoesTableExist("temp_G3", connection))
					{
						MySqlCommand createTable = new MySqlCommand(TempG3.GetTableCreationString(), connection);
						createTable.ExecuteNonQuery();
					}						

					if (!DoesTableExist("water_level_lower", connection))
					{
						MySqlCommand createTable = new MySqlCommand(WaterLevelLower.GetTableCreationString(), connection);
						createTable.ExecuteNonQuery();
					}	

					if (!DoesTableExist("water_level_upper", connection))
					{
						MySqlCommand createTable = new MySqlCommand(WaterLevelUpper.GetTableCreationString(), connection);
						createTable.ExecuteNonQuery();
					}	

					if (!DoesTableExist("flywheel_G1", connection))
					{
						MySqlCommand createTable = new MySqlCommand(FlywheelG1.GetTableCreationString(), connection);
						createTable.ExecuteNonQuery();
					}	

					if (!DoesTableExist("flywheel_G2", connection))
					{
						MySqlCommand createTable = new MySqlCommand(FlywheelG2.GetTableCreationString(), connection);
						createTable.ExecuteNonQuery();
					}

					if (!DoesTableExist("flywheel_G3", connection))
					{
						MySqlCommand createTable = new MySqlCommand(FlywheelG3.GetTableCreationString(), connection);
						createTable.ExecuteNonQuery();
					}

					// -- Database Update ---
					// Load effect data for G1
					string newTargetFile = GetNewUpperStationDataDirectory() + "EFFEKT.SKV";
					string oldTargetFile = GetOldUpperStationDataDirectory() + "EFFEKT.SKV";
					if (File.Exists(newTargetFile))
					{
						List<string> upperEffectRecords = new List<string>(File.ReadAllLines(newTargetFile));

						if (File.Exists(oldTargetFile))
						{
							List<string> oldRecords = new List<string>(File.ReadAllLines(oldTargetFile));

							// Diff the lists and just use the new records					
							upperEffectRecords = upperEffectRecords.Except(oldRecords).ToList();
						}

						List<EffectG1> G1EffectRecords = new List<EffectG1>();
						foreach (string record in upperEffectRecords)
						{
							if (record != "END")
							{
								G1EffectRecords.Add(new EffectG1(record));
							}
						}

						foreach (ISqlRecord sqlRecord in G1EffectRecords)
						{
							sqlRecord.InsertUnique(connection);
						}


						CacheRecords(newTargetFile, oldTargetFile);
					}

					newTargetFile = GetNewLowerStationDataDirectory() + "EFFEKT.SKV";
					oldTargetFile = GetOldLowerStationDataDirectory() + "EFFEKT.SKV";
					// Load effect data for G2 & G3
					if (File.Exists(newTargetFile))
					{
						List<string> lowerEffectRecords = new List<string>(File.ReadAllLines(newTargetFile));

						if (File.Exists(oldTargetFile))
						{
							List<string> oldRecords = new List<string>(File.ReadAllLines(oldTargetFile));

							// Diff the lists and just use the new records					
							lowerEffectRecords = lowerEffectRecords.Except(oldRecords).ToList();
						}

						List<EffectG2> G2EffectRecords = new List<EffectG2>();
						List<EffectG3> G3EffectRecords = new List<EffectG3>();
						foreach (string record in lowerEffectRecords)
						{
							if (record != "END")
							{
								G2EffectRecords.Add(new EffectG2(record));
								G3EffectRecords.Add(new EffectG3(record));
							}
						}

						foreach (ISqlRecord sqlRecord in G2EffectRecords)
						{
							sqlRecord.InsertUnique(connection);
						}

						foreach (ISqlRecord sqlRecord in G3EffectRecords)
						{
							sqlRecord.InsertUnique(connection);
						}

						CacheRecords(newTargetFile, oldTargetFile);
					}

					newTargetFile = GetNewUpperStationDataDirectory() + "TEMP.SKV";
					oldTargetFile = GetOldUpperStationDataDirectory() + "TEMP.SKV";
					// Load temperature data for G1
					if (File.Exists(newTargetFile))
					{
						List<string> upperTemperatureRecords = new List<string>(File.ReadAllLines(newTargetFile));

						if (File.Exists(oldTargetFile))
						{
							List<string> oldRecords = new List<string>(File.ReadAllLines(oldTargetFile));

							// Diff the lists and just use the new records					
							upperTemperatureRecords = upperTemperatureRecords.Except(oldRecords).ToList();
						}

						List<TempG1> G1TemperatureRecords = new List<TempG1>();
						foreach (string record in upperTemperatureRecords)
						{
							if (record != "END")
							{
								G1TemperatureRecords.Add(new TempG1(record));
							}
						}

						// Finally, insert the new records into the database
						foreach (ISqlRecord sqlRecord in G1TemperatureRecords)
						{
							sqlRecord.InsertUnique(connection);
						}

						CacheRecords(newTargetFile, oldTargetFile);
					}

					newTargetFile = GetNewLowerStationDataDirectory() + "TEMPG2.SKV";
					oldTargetFile = GetOldLowerStationDataDirectory() + "TEMPG2.SKV";
					// Load temperature data for G2 & G3
					if (File.Exists(newTargetFile))
					{
						List<string> lowerTemperatureRecords = new List<string>(File.ReadAllLines(newTargetFile));

						if (File.Exists(oldTargetFile))
						{
							List<string> oldRecords = new List<string>(File.ReadAllLines(oldTargetFile));

							// Diff the lists and just use the new records					
							lowerTemperatureRecords = lowerTemperatureRecords.Except(oldRecords).ToList();
						}

						List<TempG2> G2TemperatureRecords = new List<TempG2>();
						foreach (string record in lowerTemperatureRecords)
						{
							if (record != "END")
							{
								G2TemperatureRecords.Add(new TempG2(record));
							}
						}

						// Finally, insert the new records into the database
						foreach (ISqlRecord sqlRecord in G2TemperatureRecords)
						{
							sqlRecord.InsertUnique(connection);
						}

						CacheRecords(newTargetFile, oldTargetFile);
					}

					newTargetFile = GetNewLowerStationDataDirectory() + "TEMPG3.SKV";
					oldTargetFile = GetOldLowerStationDataDirectory() + "TEMPG3.SKV";
					if (File.Exists(newTargetFile))
					{
						List<string> lowerTemperatureRecords = new List<string>(File.ReadAllLines(newTargetFile));

						if (File.Exists(oldTargetFile))
						{
							List<string> oldRecords = new List<string>(File.ReadAllLines(oldTargetFile));

							// Diff the lists and just use the new records					
							lowerTemperatureRecords = lowerTemperatureRecords.Except(oldRecords).ToList();
						}

						List<TempG3> G3TemperatureRecords = new List<TempG3>();
						foreach (string record in lowerTemperatureRecords)
						{
							if (record != "END")
							{
								G3TemperatureRecords.Add(new TempG3(record));
							}
						}

						// Finally, insert the new records into the database
						foreach (ISqlRecord sqlRecord in G3TemperatureRecords)
						{
							sqlRecord.InsertUnique(connection);
						}

						CacheRecords(newTargetFile, oldTargetFile);
					}

					newTargetFile = GetNewUpperStationDataDirectory() + "OVY.SKV";
					oldTargetFile = GetOldUpperStationDataDirectory() + "OVY.SKV";
					// Load water data for upper station
					if (File.Exists(newTargetFile))
					{
						List<string> upperWaterRecords = new List<string>(File.ReadAllLines(newTargetFile));

						if (File.Exists(oldTargetFile))
						{
							List<string> oldRecords = new List<string>(File.ReadAllLines(oldTargetFile));

							// Diff the lists and just use the new records					
							upperWaterRecords = upperWaterRecords.Except(oldRecords).ToList();
						}

						List<WaterLevelUpper> G1WaterRecords = new List<WaterLevelUpper>();
						foreach (string record in upperWaterRecords)
						{
							if (record != "END")
							{
								G1WaterRecords.Add(new WaterLevelUpper(record));
							}
						}

						// Finally, insert the new records into the database
						foreach (ISqlRecord sqlRecord in G1WaterRecords)
						{
							sqlRecord.InsertUnique(connection);
						}

						CacheRecords(newTargetFile, oldTargetFile);
					}

					newTargetFile = GetNewLowerStationDataDirectory() + "OVY.SKV";
					oldTargetFile = GetOldLowerStationDataDirectory() + "OVY.SKV";
					// Load water data for lower station
					if (File.Exists(newTargetFile))
					{
						List<string> lowerWaterRecords = new List<string>(File.ReadAllLines(newTargetFile));

						if (File.Exists(oldTargetFile))
						{
							List<string> oldRecords = new List<string>(File.ReadAllLines(oldTargetFile));

							// Diff the lists and just use the new records					
							lowerWaterRecords = lowerWaterRecords.Except(oldRecords).ToList();
						}

						List<WaterLevelLower> G23WaterRecords = new List<WaterLevelLower>();
						foreach (string record in lowerWaterRecords)
						{
							if (record != "END")
							{
								G23WaterRecords.Add(new WaterLevelLower(record));
							}
						}

						// Finally, insert the new records into the database
						foreach (ISqlRecord sqlRecord in G23WaterRecords)
						{
							sqlRecord.InsertUnique(connection);
						}

						CacheRecords(newTargetFile, oldTargetFile);
					}

					newTargetFile = GetNewUpperStationDataDirectory() + "LOPHJUL.SKV";
					oldTargetFile = GetOldUpperStationDataDirectory() + "LOPHJUL.SKV";
					// Load flywheel data for G1
					if (File.Exists(newTargetFile))
					{
						List<string> upperFlywheelRecords = new List<string>(File.ReadAllLines(newTargetFile));

						if (File.Exists(oldTargetFile))
						{
							List<string> oldRecords = new List<string>(File.ReadAllLines(oldTargetFile));

							// Diff the lists and just use the new records					
							upperFlywheelRecords = upperFlywheelRecords.Except(oldRecords).ToList();
						}

						List<FlywheelG1> G1FlywheelRecords = new List<FlywheelG1>();
						foreach (string record in upperFlywheelRecords)
						{
							if (record != "END")
							{
								G1FlywheelRecords.Add(new FlywheelG1(record));
							}
						}

						// Finally, insert the new records into the database
						foreach (ISqlRecord sqlRecord in G1FlywheelRecords)
						{
							sqlRecord.InsertUnique(connection);
						}

						CacheRecords(newTargetFile, oldTargetFile);
					}

					newTargetFile = GetNewLowerStationDataDirectory() + "LOPHJUL.SKV";
					oldTargetFile = GetOldLowerStationDataDirectory() + "LOPHJUL.SKV";
					// Load flywheel data for G2 & G3
					if (File.Exists(newTargetFile))
					{
						List<string> lowerFlywheelRecords = new List<string>(File.ReadAllLines(newTargetFile));

						if (File.Exists(oldTargetFile))
						{
							List<string> oldRecords = new List<string>(File.ReadAllLines(oldTargetFile));

							// Diff the lists and just use the new records					
							lowerFlywheelRecords = lowerFlywheelRecords.Except(oldRecords).ToList();
						}

						List<FlywheelG2> G2FlywheelRecords = new List<FlywheelG2>();
						List<FlywheelG3> G3FlywheelRecords = new List<FlywheelG3>();
						foreach (string record in lowerFlywheelRecords)
						{
							if (record != "END")
							{
								G2FlywheelRecords.Add(new FlywheelG2(record));
								G3FlywheelRecords.Add(new FlywheelG3(record));
							}
						}

						// Finally, insert the new records into the database
						foreach (ISqlRecord sqlRecord in G2FlywheelRecords)
						{
							sqlRecord.InsertUnique(connection);
						}

						// Finally, insert the new records into the database
						foreach (ISqlRecord sqlRecord in G3FlywheelRecords)
						{
							sqlRecord.InsertUnique(connection);
						}

						CacheRecords(newTargetFile, oldTargetFile);
					}
				}
				catch (MySqlException mex)
				{
					Console.WriteLine(mex.Message);
					return 1;
				}
			}

			return 0;
		}

		/// <summary>
		/// Checks if the specified table exists in the database.
		/// </summary>
		/// <returns><c>true</c>, if the table exists, <c>false</c> otherwise.</returns>
		/// <param name="tableName">Table name to check.</param>
		/// <param name="connection">Live connection to the database.</param>
		private static bool DoesTableExist(string tableName, MySqlConnection connection)
		{			
			string baseCommand = "SHOW TABLES LIKE @tableName";
			MySqlCommand tableCheck = new MySqlCommand(baseCommand, connection);
			tableCheck.Parameters.AddWithValue("@tableName", tableName);

			using (var reader = tableCheck.ExecuteReader())
			{
				if (reader.HasRows)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Caches the specifed records, effectively moving them from /data/new to /data/old.
		/// </summary>
		/// <param name="newRecordsPath">New records path.</param>
		/// <param name="oldRecordsPath">Old records path.</param>
		private static void CacheRecords(string newRecordsPath, string oldRecordsPath)
		{
			if (File.Exists(oldRecordsPath))
			{
				File.Delete(oldRecordsPath);
			}

			File.Move(newRecordsPath, oldRecordsPath);
		}

		/// <summary>
		/// Creates the initial folders where data is stored.
		/// These folders are required for the program to run.
		/// </summary>
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

		/// <summary>
		/// Gets the old lower station data directory where cached records are stored.
		/// </summary>
		/// <returns>The old lower station data directory.</returns>
		private static string GetOldLowerStationDataDirectory()
		{
			return "data" + Path.DirectorySeparatorChar + "old" + Path.DirectorySeparatorChar + "lower" + Path.DirectorySeparatorChar;
		}

		/// <summary>
		/// Gets the old upper station data directory where cached records are stored.
		/// </summary>
		/// <returns>The old upper station data directory.</returns>
		private static string GetOldUpperStationDataDirectory()
		{
			return "data" + Path.DirectorySeparatorChar + "old" + Path.DirectorySeparatorChar + "upper" + Path.DirectorySeparatorChar;
		}

		/// <summary>
		/// Gets the new lower station data directory where new data is stored.
		/// </summary>
		/// <returns>The new lower station data directory.</returns>
		private static string GetNewLowerStationDataDirectory()
		{
			return "data" + Path.DirectorySeparatorChar + "new" + Path.DirectorySeparatorChar + "lower" + Path.DirectorySeparatorChar;
		}

		/// <summary>
		/// Gets the new upper station data directory where new data is stored.
		/// </summary>
		/// <returns>The new upper station data directory.</returns>
		private static string GetNewUpperStationDataDirectory()
		{
			return "data" + Path.DirectorySeparatorChar + "new" + Path.DirectorySeparatorChar + "upper" + Path.DirectorySeparatorChar;
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
