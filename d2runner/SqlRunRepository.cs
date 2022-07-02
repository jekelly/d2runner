using System;
using System.Data;
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

            // read runs
            this.ReadRuns();


            this.LastRun = this.GetLastRun();
        }

        private void ReadRuns()
        {
            var c = this.connection.CreateCommand();
            c.CommandText = "SELECT * FROM runs";
            var reader = c.ExecuteReader();
            while (reader.Read())
            {
                base.Add(ReadRun(reader));
            }
        }

        private static Run ReadRun(IDataRecord data)
        {
            var start = DateTimeOffset.Parse((string)data["start"]);
            var duration = (double)data["durationMs"];
            var run = new Run((int)data.GetInt64(0))
            {
                Start = start,
                End = start + TimeSpan.FromMilliseconds(duration)
            };
            return run;
        }

        public override void Remove(Run run)
        {
            base.Remove(run);

            var c = this.connection.CreateCommand();
            c.CommandText = $"DELETE from runs where id = {run.Id}";
            c.ExecuteNonQuery();
        }

        private Run? GetLastRun()
        {
            var c = this.connection.CreateCommand();
            c.CommandText = $"SELECT * FROM runs ORDER BY id DESC LIMIT 1";
            var reader = c.ExecuteReader();
            reader.Read();
            return ReadRun(reader);
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
            if (this.Runs.Lookup(run.Id) != DynamicData.Kernel.Optional<Run>.None)
            {
                return;
            }
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
