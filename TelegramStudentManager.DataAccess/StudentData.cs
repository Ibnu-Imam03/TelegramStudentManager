using Npgsql;
using System;
using System.Data;

namespace TelegramStudentManager.DataAccess
{
    public class clsStudentData
    {
        public static DataTable GetAllStudents()
        {
            DataTable dt = new DataTable();

            NpgsqlConnection connection = new NpgsqlConnection(clsDataSetthing.connectionString);

            string query = @"SELECT ""Id"", ""FullName"", ""StudentNumber"", ""Department"", ""Year"", ""Phone"" FROM ""Students""";

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            try
            {
                connection.Open();

                NpgsqlDataReader result = command.ExecuteReader();

                if (result != null)
                {
                    dt.Load(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }

        public static bool GetStudentByID(int StudentID, ref string FullName, ref string StudentNumber, ref string Department, ref int Year, ref string Phone)
        {
            bool IsFound = false;

            NpgsqlConnection connection = new NpgsqlConnection(clsDataSetthing.connectionString);

            string query = @"SELECT ""Id"", ""FullName"", ""StudentNumber"", ""Department"", ""Year"", ""Phone"" 
                             FROM ""Students"" 
                             WHERE ""Id"" = @StudentID";

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@StudentID", StudentID);

            try
            {
                connection.Open();

                NpgsqlDataReader result = command.ExecuteReader();

                if (result.Read())
                {
                    IsFound = true;

                    FullName = result["FullName"].ToString();
                    StudentNumber = result["StudentNumber"].ToString();
                    Department = result["Department"].ToString();
                    Year = Convert.ToInt32(result["Year"]);

                    if (result["Phone"] == DBNull.Value)
                        Phone = "";
                    else
                        Phone = result["Phone"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return IsFound;
        }

        public static int AddStudent(string FullName, string StudentNumber, string Department, int Year, string Phone)
        {
            NpgsqlConnection connection = new NpgsqlConnection(clsDataSetthing.connectionString);

            int StudentID = -1;

            string query = @"INSERT INTO ""Students""
                            (""FullName"", ""StudentNumber"", ""Department"", ""Year"", ""Phone"")
                            VALUES
                            (@FullName, @StudentNumber, @Department, @Year, @Phone)
                            RETURNING ""Id"";";

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@FullName", FullName);
            command.Parameters.AddWithValue("@StudentNumber", StudentNumber);
            command.Parameters.AddWithValue("@Department", Department);
            command.Parameters.AddWithValue("@Year", Year);

            if (string.IsNullOrWhiteSpace(Phone))
                command.Parameters.AddWithValue("@Phone", DBNull.Value);
            else
                command.Parameters.AddWithValue("@Phone", Phone);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    StudentID = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return StudentID;
        }

        public static bool UpdateStudent(int StudentID, string FullName, string StudentNumber, string Department, int Year, string Phone)
        {
            NpgsqlConnection connection = new NpgsqlConnection(clsDataSetthing.connectionString);

            int RowsAffected = -1;

            string query = @"UPDATE ""Students""
                             SET ""FullName"" = @FullName,
                                 ""StudentNumber"" = @StudentNumber,
                                 ""Department"" = @Department,
                                 ""Year"" = @Year,
                                 ""Phone"" = @Phone
                             WHERE ""Id"" = @StudentID";

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@StudentID", StudentID);
            command.Parameters.AddWithValue("@FullName", FullName);
            command.Parameters.AddWithValue("@StudentNumber", StudentNumber);
            command.Parameters.AddWithValue("@Department", Department);
            command.Parameters.AddWithValue("@Year", Year);

            if (string.IsNullOrWhiteSpace(Phone))
                command.Parameters.AddWithValue("@Phone", DBNull.Value);
            else
                command.Parameters.AddWithValue("@Phone", Phone);

            try
            {
                connection.Open();

                RowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return RowsAffected > 0;
        }

        public static bool DeleteStudent(int StudentID)
        {
            NpgsqlConnection connection = new NpgsqlConnection(clsDataSetthing.connectionString);

            int RowsAffected = -1;

            string query = @"DELETE FROM ""Students""
                             WHERE ""Id"" = @StudentID";

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            command.Parameters.AddWithValue("@StudentID", StudentID);

            try
            {
                connection.Open();

                RowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return RowsAffected > 0;
        }
    }
}