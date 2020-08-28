using MySql.Data.MySqlClient;
using netasloc.Data.Entity;
using System;
using System.Collections.Generic;

namespace netasloc.Data.DAO
{
    public class ReleaseDAO : _IReleaseDAO
    {
        private readonly MySqlDatabase _database;

        public ReleaseDAO(MySqlDatabase database)
        {
            _database = database;
        }

        public Release GetByID(uint id)
        {
            var result = new List<Release>();
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM releases WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new Release()
                    {
                        id = reader.GetFieldValue<uint>(0),
                        created_at = reader.GetFieldValue<DateTime>(1),
                        updated_at = reader.GetFieldValue<DateTime>(2),
                        release_code = reader.GetFieldValue<string>(3),
                        release_start = reader.GetFieldValue<DateTime>(4),
                        release_end = reader.GetFieldValue<DateTime>(5),
                        total_line_count = reader.GetFieldValue<uint>(6),
                        code_line_count = reader.GetFieldValue<uint>(7),
                        comment_line_count = reader.GetFieldValue<uint>(8),
                        empty_line_count = reader.GetFieldValue<uint>(9),
                        difference_sloc = reader.GetFieldValue<int>(10),
                        difference_loc = reader.GetFieldValue<int>(11)
                    });
                }
            }
            if (result.Count > 0)
                return result[0];
            else
                return null;
        }

        public IEnumerable<Release> GetAll()
        {
            var result = new List<Release>();
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM releases ORDER BY `created_at` DESC";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new Release()
                    {
                        id = reader.GetFieldValue<uint>(0),
                        created_at = reader.GetFieldValue<DateTime>(1),
                        updated_at = reader.GetFieldValue<DateTime>(2),
                        release_code = reader.GetFieldValue<string>(3),
                        release_start = reader.GetFieldValue<DateTime>(4),
                        release_end = reader.GetFieldValue<DateTime>(5),
                        total_line_count = reader.GetFieldValue<uint>(6),
                        code_line_count = reader.GetFieldValue<uint>(7),
                        comment_line_count = reader.GetFieldValue<uint>(8),
                        empty_line_count = reader.GetFieldValue<uint>(9),
                        difference_sloc = reader.GetFieldValue<int>(10),
                        difference_loc = reader.GetFieldValue<int>(11)
                    });
                }
            }
            return result;
        }

        public bool Create(Release item)
        {
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = "INSERT INTO `releases` (`created_at`, `updated_at`, `release_code`, `release_start`, `release_end`, `total_line_count`, `code_line_count`, `comment_line_count`, `empty_line_count`, `difference_sloc`, `difference_loc`) VALUES (@created_at, @updated_at, @release_code, @release_start, @release_end, @total_line_count, @code_line_count, @comment_line_count, @empty_line_count, @difference_sloc, @difference_loc)";
            command.Parameters.AddWithValue("@created_at", item.created_at);
            command.Parameters.AddWithValue("@updated_at", item.updated_at);
            command.Parameters.AddWithValue("@release_code", item.release_code);
            command.Parameters.AddWithValue("@release_start", item.release_start);
            command.Parameters.AddWithValue("@release_end", item.release_end);
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

        public bool Update(uint id, Release item)
        {
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = "UPDATE `releases` SET created_at = @created_at, updated_at = @updated_at, release_code = @release_code, release_start = @release_start, release_end = @release_end, total_line_count = @total_line_count, code_line_count = @code_line_count, comment_line_count = @comment_line_count, empty_line_count = @empty_line_count, difference_sloc = @difference_sloc, difference_loc = @difference_loc WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@created_at", item.created_at);
            command.Parameters.AddWithValue("@updated_at", item.updated_at);
            command.Parameters.AddWithValue("@release_code", item.release_code);
            command.Parameters.AddWithValue("@release_start", item.release_start);
            command.Parameters.AddWithValue("@release_end", item.release_end);
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
            command.CommandText = "DELETE FROM `releases` WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            int queryResult = command.ExecuteNonQuery();
            if (queryResult == 1)
                return true;
            else
                return false;
        }
    }
}
