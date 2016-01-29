//
//  Config.cs
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
using System.IO;
using System.Collections.Generic;

namespace Krafta.Configuration
{
	/// <summary>
	/// This class handles loading, creation of and parsing for the configuration file.
	/// The default configuration file is named krafta.cfg, and is stored next to the executable.
	/// </summary>
	public class ConfigurationHandler
	{
		/// <summary>
		/// Singleton instance of the configuration handler.
		/// </summary>
		public static ConfigurationHandler Instance = new ConfigurationHandler();

		/// <summary>
		/// The comment signature. Lines starting with this string are not parsed.
		/// </summary>
		private const string CommentSignature = "//";

		/// <summary>
		/// The assignment signature. Whenever this character is encountered, the rest of the line is considered a value.
		/// </summary>
		private const char AssignmentSignature = '=';

		/// <summary>
		/// Initializes a new instance of the <see cref="Krafta.Configuration.ConfigurationHandler"/> class.
		/// </summary>
		private ConfigurationHandler()
		{
			if (!HasConfigurationFile())
			{
				CreateDefaultConfigurationFile();
			}

			List<string> rawConfigLines = new List<string>(File.ReadAllLines("krafta.cfg"));

			int i = 0;
			foreach (string line in rawConfigLines)
			{
				if (!line.StartsWith(ConfigurationHandler.CommentSignature) && !String.IsNullOrWhiteSpace(line))
				{
					KeyValuePair<string, string> configurationKey = ParseLine(line, i);

					switch (configurationKey.Key)
					{
						case "DatabaseUsername":
							{
								this.DatabaseUsername = configurationKey.Value;
								break;
							}
						case "DatabasePassword":
							{
								this.DatabasePassword = configurationKey.Value;
								break;
							}
						case "DatabaseName":
							{
								this.DatabaseName = configurationKey.Value;
								break;
							}
						case "DatabaseHost":
							{
								this.DatabaseHost = configurationKey.Value;
								break;
							}
						case "DatabasePort":
							{
								this.DatabasePort = configurationKey.Value;
								break;
							}						
						default:
							{
								break;
							}
					}
				}

				++i;
			}
		}

		/// <summary>
		/// Determines if the program has a configuration file.
		/// </summary>
		/// <returns><c>true</c> if a configuration file exists; otherwise, <c>false</c>.</returns>
		public static bool HasConfigurationFile()
		{
			return File.Exists("krafta.cfg");
		}

		/// <summary>
		/// Creates the default configuration file.
		/// </summary>
		public static void CreateDefaultConfigurationFile()
		{
			string defaultConfigContents = "//\tKrafta configuration file.\n" +
			                               "//\n" +
			                               "//\tThe default configuration file looks like this: \n" +
			                               "//\n" +
			                               "//\tDatabaseUsername=kraftauser\n" +
			                               "//\tDatabasePassword=kraftapassword\n" +
			                               "//\tDatabaseName=krafta\n" +
			                               "//\tDatabaseHost=localhost\n" +
			                               "//\tDatabasePort=3306\n" +
			                               "//\n" +
			                               "//\tAll keys in the file are formatted according to this spec: \n" +
			                               "//\t<key>=<value>\n" +
			                               "//\tAnything preceding the first equality sign is regarded as a key, and anything after it as the value.\n" +
			                               "//\tValues are not verified, and are assumed to be valid.\n" +
			                               "\n" +
			                               "DatabaseUsername=kraftauser\n" +
			                               "DatabasePassword=kraftapassword\n" +
			                               "DatabaseHost=localhost\n" +
			                               "DatabasePort=3306";

			File.WriteAllText("krafta.cfg", defaultConfigContents);
		}

		/// <summary>
		/// Parses a line inside the configuration file. The line is assumed to not
		/// be a comment at this point.
		/// </summary>
		/// <returns>The key and value as a paired variable.</returns>
		/// <param name="line">The line to be parsed.</param>
		/// <param name="lineNumber">Line number. Used in the error message.</param>
		private static KeyValuePair<string, string> ParseLine(string line, int lineNumber)
		{
			if (line.Contains(ConfigurationHandler.AssignmentSignature.ToString()))
			{
				int assignmentIndex = line.IndexOf(ConfigurationHandler.AssignmentSignature);
				string key = line.Substring(0, assignmentIndex);
				string value = line.Substring(assignmentIndex + 1);

				return new KeyValuePair<string, string>(key, value);
			}
			else
			{
				throw new ArgumentException("Invalid assignment in krafta.cfg at line " + lineNumber.ToString());
			}
		}

		/// <summary>
		/// Gets the database username.
		/// </summary>
		/// <value>The database username.</value>
		public string DatabaseUsername
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the database password.
		/// </summary>
		/// <value>The database password.</value>
		public string DatabasePassword
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the name of the database.
		/// </summary>
		/// <value>The name of the database.</value>
		public string DatabaseName
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the database host.
		/// </summary>
		/// <value>The database host.</value>
		public string DatabaseHost
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the database port.
		/// </summary>
		/// <value>The database port.</value>
		public string DatabasePort
		{
			get;
			private set;
		}
	}
}

