using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Npgsql;

namespace Sprint_3
{
    public class TimeSeriesPoint
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Measurement { get; set; }
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, object> Fields { get; set; } = new Dictionary<string, object>();
    }

    public static class TigerDataService
    {
        private static string _connectionString = "Host=xu5311rdqa.pk2csa5t31.tsdb.cloud.timescale.com;Username=tsdbadmin;Password=evckixxwkhb9nuar;Database=tsdb;SSL Mode=Require;Trust Server Certificate=true;Port=35470";

        private static string _dbPath = "tiger_timeseries_data.json";
        private static List<TimeSeriesPoint> _buffer = new List<TimeSeriesPoint>();

        public static void IngestMove(int row, int col, Cell moveType, Player player, int boardSize)
        {
            var point = new TimeSeriesPoint
            {
                Timestamp = DateTimeOffset.UtcNow,
                Measurement = "player_move",
                Tags = new Dictionary<string, string>
                {
                    { "player_id", player.ToString() },
                    { "game_mode", boardSize > 0 ? "Standard" : "Custom" }
                },
                Fields = new Dictionary<string, object>
                {
                    { "row_index", row },
                    { "col_index", col },
                    { "move_type", moveType.ToString() }
                }
            };

            _buffer.Add(point);
            FlushToDisk();
        }

        public static void IngestGameResult(string winner, int durationSeconds)
        {
            var point = new TimeSeriesPoint
            {
                Timestamp = DateTimeOffset.UtcNow,
                Measurement = "game_summary",
                Tags = new Dictionary<string, string>
                {
                    { "winner_id", winner },
                    { "status", "completed" }
                },
                Fields = new Dictionary<string, object>
                {
                    { "duration_sec", durationSeconds },
                    { "total_moves", _buffer.Count }
                }
            };

            _buffer.Add(point);
            FlushToDisk();
        }

        public static string GetTimeSeriesAnalytics()
        {
            if (!File.Exists(_dbPath)) return "No Data";

            try
            {
                var totalPoints = _buffer.Count;

                return $"Tiger Data Metrics:\n" +
                       $"Total Points: {totalPoints}\n" +
                       $"Last Ingest: {DateTime.Now:HH:mm:ss}";
            }
            catch { return "Analytics Unavailable"; }
        }

        public static string UploadToTigerCloud()
        {
            if (_buffer.Count == 0) return "No data to upload.";

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string createTableSql = @"
                        CREATE TABLE IF NOT EXISTS game_events (
                            time TIMESTAMPTZ NOT NULL,
                            measurement TEXT,
                            player_id TEXT,
                            game_mode TEXT,
                            details JSONB
                        );
                        SELECT create_hypertable('game_events', 'time', if_not_exists => TRUE);";

                    using (var cmd = new NpgsqlCommand(createTableSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    foreach (var point in _buffer)
                    {
                        string insertSql = @"
                            INSERT INTO game_events (time, measurement, player_id, game_mode, details)
                            VALUES (@t, @m, @p, @g, @d::jsonb)";

                        using (var cmd = new NpgsqlCommand(insertSql, conn))
                        {
                            cmd.Parameters.AddWithValue("t", point.Timestamp);
                            cmd.Parameters.AddWithValue("m", point.Measurement);
                            cmd.Parameters.AddWithValue("p", point.Tags.ContainsKey("player_id") ? point.Tags["player_id"] : "System");
                            cmd.Parameters.AddWithValue("g", point.Tags.ContainsKey("game_mode") ? point.Tags["game_mode"] : "N/A");

                            string jsonDetails = JsonSerializer.Serialize(point.Fields);
                            cmd.Parameters.AddWithValue("d", jsonDetails);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return "Successfully uploaded to Tiger Data Cloud!";
            }
            catch (Exception ex)
            {
                return $"Cloud Upload Failed: {ex.Message}";
            }
        }

        private static void FlushToDisk()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_buffer, options);
            File.WriteAllText(_dbPath, json);
        }
    }
}