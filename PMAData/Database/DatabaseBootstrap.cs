using PMAData.Database.Abstract;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Linq;
using Dapper;

namespace PMAData.Database
{
    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DatabaseConfig databaseConfig;
        private Dictionary<string, string> tableConfiguration;

        public DatabaseBootstrap(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
            this.tableConfiguration = new Dictionary<string, string>() {
                { "Movie", "Create Table Movie (ID INT NOT NULL, Title VARCHAR(1000) NOT NULL, Year VARCHAR(4) NOT NULL, LastPlayed DATETIME NULL, Added DATETIME NULL, FileSize BIGINT NOT NULL, IsCurrent BIT NOT NULL, IsArchived BIT NOT NULL);" },
                { "TVShow", "Create Table TVShow (ID INT NOT NULL, Title VARCHAR(1000) NOT NULL, Year VARCHAR(4) NOT NULL, LastPlayed DATETIME NULL, Added DATETIME NULL, FileSize BIGINT NOT NULL, IsCurrent BIT NOT NULL, IsArchived BIT NOT NULL);" },
                { "User", "Create Table User (ID INT NOT NULL, UserName VARCHAR(100) NOT NULL, LastLogin DATETIME NULL, LastActivity DATETIME NULL);" },
                { "GenericData", "Create Table GenericData (ID UNIQUEIDENTIFIER NOT NULL, MediaID INT NOT NULL, MediaType VARCHAR(255) NOT NULL, DataKey VARCHAR(255) NOT NULL, DataValue VARCHAR(255) NULL);" }
            };
        }

        public void Setup()
        {
            if (databaseConfig.HasConnection)
            {
                foreach (var tableDefinition in this.tableConfiguration.Keys)
                {
                    var table = databaseConfig.Connection.Query<string>($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableDefinition}';");
                    var tableName = table.FirstOrDefault();

                    if (!string.IsNullOrEmpty(tableName) && tableName == tableDefinition)
                    {
                        continue;
                    }

                    databaseConfig.Connection.Execute(this.tableConfiguration[tableDefinition]);
                }

                databaseConfig.Dispose();
            }
        }
    }
}
