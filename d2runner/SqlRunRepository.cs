using System;
using Microsoft.Data.Sqlite;

namespace d2runner
{
    public class SqlRunRepository : RunRepository, IDisposable
    {
        private readonly SqliteConnection connection; 

        public SqlRunRepository()
        {
            this.connection = new SqliteConnection("Data source=d2.db");
            this.connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS runs(
                    id INT PRIMARY KEY,
                    start STRING,
                    durationMs REAL
                );
            ";
            command.ExecuteNonQuery();
        }

        public override int NextId()
        {
            var c = this.connection.CreateCommand();
            c.CommandText = "SELECT MAX (id) from runs";
            var result = c.ExecuteScalar();
            if (result == DBNull.Value)
            {
                return 0;
            }
            return (int)((long)result) + 1;
        }

        public override void Add(Run run)
        {
            base.Add(run);
            var addCommand = this.connection.CreateCommand();
            addCommand.CommandText = $@"
                INSERT INTO runs (id, start, durationMs)
                VALUES ({run.Id}, '{run.Start}', '{(run.End - run.Start)?.TotalMilliseconds}')";
            addCommand.ExecuteNonQuery();
        }

        public void Dispose()
        {
            this.connection.Close();
            this.connection.Dispose();
        }
    }
}
