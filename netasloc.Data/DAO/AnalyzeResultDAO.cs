using MySql.Data.MySqlClient;
using netasloc.Data.Entity;
using System;
using System.Collections.Generic;

namespace netasloc.Data.DAO
{
    public class AnalyzeResultDAO : _IAnalyzeResultDAO
    {
        private readonly MySqlDatabase _database;

        public AnalyzeResultDAO(MySqlDatabase database)
        {
            _database = database;
        }

        public AnalyzeResult GetByID(uint id)
        {
            var result = new List<AnalyzeResult>();
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM analyze_results WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new AnalyzeResult()
                    {
                        id = reader.GetFieldValue<uint>(0),
                        created_at = reader.GetFieldValue<DateTime>(1),
                        updated_at = reader.GetFieldValue<DateTime>(2),
                        directory_count = reader.GetFieldValue<uint>(3),
                        directory_id_list = reader.GetFieldValue<string>(4),
                        total_line_count = reader.GetFieldValue<uint>(5),
                        code_line_count = reader.GetFieldValue<uint>(6),
                        comment_line_count = reader.GetFieldValue<uint>(7),
                        empty_line_count = reader.GetFieldValue<uint>(8),
                        difference_sloc = reader.GetFieldValue<int>(9),
                        difference_loc = reader.GetFieldValue<int>(10)
                    });
                }
            }
            if (result.Count > 0)
                return result[0];
            else
                return null;
        }

        public IEnumerable<AnalyzeResult> GetAll()
        {
            var result = new List<AnalyzeResult>();
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM analyze_results ORDER BY `created_at` DESC";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new AnalyzeResult()
                    {
                        id = reader.GetFieldValue<uint>(0),
                        created_at = reader.GetFieldValue<DateTime>(1),
                        updated_at = reader.GetFieldValue<DateTime>(2),
                        directory_count = reader.GetFieldValue<uint>(3),
                        directory_id_list = reader.GetFieldValue<string>(4),
                        total_line_count = reader.GetFieldValue<uint>(5),
                        code_line_count = reader.GetFieldValue<uint>(6),
                        comment_line_count = reader.GetFieldValue<uint>(7),
                        empty_line_count = reader.GetFieldValue<uint>(8),
                        difference_sloc = reader.GetFieldValue<int>(9),
                        difference_loc = reader.GetFieldValue<int>(10)
                    });
                }
            }
            return result;
        }

        public bool Create(AnalyzeResult item)
        {
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = "INSERT INTO `analyze_results` (`created_at`, `updated_at`, `directory_count`, `directory_id_list`, `total_line_count`, `code_line_count`, `comment_line_count`, `empty_line_count`, `difference_sloc`, `difference_loc`) VALUES (@created_at, @updated_at, @directory_count, @directory_id_list, @total_line_count, @code_line_count, @comment_line_count, @empty_line_count, @difference_sloc, @difference_loc)";
            command.Parameters.AddWithValue("@created_at", item.created_at);
            command.Parameters.AddWithValue("@updated_at", item.updated_at);
            command.Parameters.AddWithValue("@directory_count", item.directory_count);
            command.Parameters.AddWithValue("@directory_id_list", item.directory_id_list);
            command.Parameters.AddWithValue("@total_line_count", item.total_line_count);
            command.Parameters.AddWithValue("@code_line_count", item.code_line_count);
            command.Parameters.AddWithValue("@comment_line_count", item.comment_line_count);
            command.Parameters.AddWithValue("@empty_line_count", item.empty_line_count);
            command.Parameters.AddWithValue("@difference_sloc", item.difference_sloc);
            command.Parameters.AddWithValue("@difference_loc", item.difference_loc);
            int queryResult = command.ExecuteNonQuery();
            if (queryResult == 1)
                return true;
            else
                return false;
        }

        public bool Update(uint id, AnalyzeResult item)
        {
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = "UPDATE `analyze_results` SET created_at = @created_at, updated_at = @updated_at, directory_count = @directory_count, directory_id_list = @directory_id_list, total_line_count = @total_line_count, code_line_count = @code_line_count, comment_line_count = @comment_line_count, empty_line_count = @empty_line_count, difference_sloc = @difference_sloc, difference_loc = @difference_loc WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@created_at", item.created_at);
            command.Parameters.AddWithValue("@updated_at", item.updated_at);
            command.Parameters.AddWithValue("@directory_count", item.directory_count);
            command.Parameters.AddWithValue("@directory_id_list", item.directory_id_list);
            command.Parameters.AddWithValue("@total_line_count", item.total_line_count);
            command.Parameters.AddWithValue("@code_line_count", item.code_line_count);
            command.Parameters.AddWithValue("@comment_line_count", item.comment_line_count);
            command.Parameters.AddWithValue("@empty_line_count", item.empty_line_count);
            command.Parameters.AddWithValue("@difference_sloc", item.difference_sloc);
            command.Parameters.AddWithValue("@difference_loc", item.difference_loc);
            int queryResult = command.ExecuteNonQuery();
            if (queryResult == 1)
                return true;
            else
                return false;
        }

        public bool Delete(uint id)
        {
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = "DELETE FROM `analyze_results` WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            int queryResult = command.ExecuteNonQuery();
            if (queryResult == 1)
                return true;
            else
                return false;
        }

        public IEnumerable<AnalyzeResult> GetAnalyzeResultsForRelease(DateTime release_start, DateTime release_end)
        {
            var result = new List<AnalyzeResult>();
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM analyze_results WHERE `created_at` >= @release_start AND `created_at` <= @release_end ORDER BY `created_at` DESC";
            command.Parameters.AddWithValue("@release_start", release_start);
            command.Parameters.AddWithValue("@release_end", release_end);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new AnalyzeResult()
                    {
                        id = reader.GetFieldValue<uint>(0),
                        created_at = reader.GetFieldValue<DateTime>(1),
                        updated_at = reader.GetFieldValue<DateTime>(2),
                        directory_count = reader.GetFieldValue<uint>(3),
                        directory_id_list = reader.GetFieldValue<string>(4),
                        total_line_count = reader.GetFieldValue<uint>(5),
                        code_line_count = reader.GetFieldValue<uint>(6),
                        comment_line_count = reader.GetFieldValue<uint>(7),
                        empty_line_count = reader.GetFieldValue<uint>(8),
                        difference_sloc = reader.GetFieldValue<int>(9),
                        difference_loc = reader.GetFieldValue<int>(10)
                    });
                }
            }
            return result;
        }
    }
}
