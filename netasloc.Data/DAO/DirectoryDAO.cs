using MySql.Data.MySqlClient;
using netasloc.Data.Entity;
using System;
using System.Collections.Generic;

namespace netasloc.Data.DAO
{
    public class DirectoryDAO : _IDirectoryDAO
    {
        private readonly MySqlDatabase _database;

        public DirectoryDAO(MySqlDatabase database)
        {
            _database = database;
        }

        public Directory GetByID(uint id)
        {
            var result = new List<Directory>();
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM directories WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new Directory()
                    {
                        id = reader.GetFieldValue<uint>(0),
                        created_at = reader.GetFieldValue<DateTime>(1),
                        updated_at = reader.GetFieldValue<DateTime>(2),
                        project_name = reader.GetFieldValue<string>(3),
                        full_path = reader.GetFieldValue<string>(4),
                        file_count = reader.GetFieldValue<uint>(5),
                        total_line_count = reader.GetFieldValue<uint>(6),
                        code_line_count = reader.GetFieldValue<uint>(7),
                        comment_line_count = reader.GetFieldValue<uint>(8),
                        empty_line_count = reader.GetFieldValue<uint>(9)
                    });
                }
            }
            if (result.Count > 0)
                return result[0];
            else
                return null;
        }

        public IEnumerable<Directory> GetAll()
        {
            var result = new List<Directory>();
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM directories ORDER BY `created_at` DESC";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new Directory()
                    {
                        id = reader.GetFieldValue<uint>(0),
                        created_at = reader.GetFieldValue<DateTime>(1),
                        updated_at = reader.GetFieldValue<DateTime>(2),
                        project_name = reader.GetFieldValue<string>(3),
                        full_path = reader.GetFieldValue<string>(4),
                        file_count = reader.GetFieldValue<uint>(5),
                        total_line_count = reader.GetFieldValue<uint>(6),
                        code_line_count = reader.GetFieldValue<uint>(7),
                        comment_line_count = reader.GetFieldValue<uint>(8),
                        empty_line_count = reader.GetFieldValue<uint>(9)
                    });
                }
            }
            return result;
        }

        public bool Create(Directory item)
        {
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = "INSERT INTO `directories` (`created_at`, `updated_at`, `project_name`, `full_path`, `file_count`, `total_line_count`, `code_line_count`, `comment_line_count`, `empty_line_count`) VALUES (@created_at, @updated_at, @project_name, @full_path, @file_count, @total_line_count, @code_line_count, @comment_line_count, @empty_line_count)";
            command.Parameters.AddWithValue("@created_at", item.created_at);
            command.Parameters.AddWithValue("@updated_at", item.updated_at);
            command.Parameters.AddWithValue("@project_name", item.project_name);
            command.Parameters.AddWithValue("@full_path", item.full_path);
            command.Parameters.AddWithValue("@file_count", item.file_count);
            command.Parameters.AddWithValue("@total_line_count", item.total_line_count);
            command.Parameters.AddWithValue("@code_line_count", item.code_line_count);
            command.Parameters.AddWithValue("@comment_line_count", item.comment_line_count);
            command.Parameters.AddWithValue("@empty_line_count", item.empty_line_count);
            int queryResult = command.ExecuteNonQuery();
            if (queryResult == 1)
                return true;
            else
                return false;
        }

        public bool Update(uint id, Directory item)
        {
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = "UPDATE `directories` SET created_at = @created_at, updated_at = @updated_at, project_name = @project_name, full_path = @full_path, file_count = @file_count, total_line_count = @total_line_count, code_line_count = @code_line_count, comment_line_count = @comment_line_count, empty_line_count = @empty_line_count WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@created_at", item.created_at);
            command.Parameters.AddWithValue("@updated_at", item.updated_at);
            command.Parameters.AddWithValue("@project_name", item.project_name);
            command.Parameters.AddWithValue("@full_path", item.full_path);
            command.Parameters.AddWithValue("@file_count", item.file_count);
            command.Parameters.AddWithValue("@total_line_count", item.total_line_count);
            command.Parameters.AddWithValue("@code_line_count", item.code_line_count);
            command.Parameters.AddWithValue("@comment_line_count", item.comment_line_count);
            command.Parameters.AddWithValue("@empty_line_count", item.empty_line_count);
            int queryResult = command.ExecuteNonQuery();
            if (queryResult == 1)
                return true;
            else
                return false;
        }

        public bool Delete(uint id)
        {
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = "DELETE FROM `directories` WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            int queryResult = command.ExecuteNonQuery();
            if (queryResult == 1)
                return true;
            else
                return false;
        }

        public IEnumerable<Directory> GetLastAnalyzedDirectories()
        {
            var result = new List<Directory>();
            MySqlCommand command = this._database.Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM `netasloc`.`directories` WHERE `created_at` IN (SELECT MAX(`created_at`) FROM `netasloc`.`directories` GROUP BY `project_name`);";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new Directory()
                    {
                        id = reader.GetFieldValue<uint>(0),
                        created_at = reader.GetFieldValue<DateTime>(1),
                        updated_at = reader.GetFieldValue<DateTime>(2),
                        project_name = reader.GetFieldValue<string>(3),
                        full_path = reader.GetFieldValue<string>(4),
                        file_count = reader.GetFieldValue<uint>(5),
                        total_line_count = reader.GetFieldValue<uint>(6),
                        code_line_count = reader.GetFieldValue<uint>(7),
                        comment_line_count = reader.GetFieldValue<uint>(8),
                        empty_line_count = reader.GetFieldValue<uint>(9)
                    });
                }
            }
            return result;
        }
    }
}
