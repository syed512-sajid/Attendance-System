using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using AttendanceSystem.Models;

namespace AttendanceSystem.Repository
{
    // Common ADO.NET helper.
    // Sab stored-procedure calls isi se guzarte hain.
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            string encryptedConnectionString =
                configuration.GetConnectionString("AttendanceDb");

            if (string.IsNullOrWhiteSpace(encryptedConnectionString))
            {
                throw new InvalidOperationException(
                    "AttendanceDb connection string not found in appsettings.json.");
            }

            _connectionString =
                Encryption.DecryptString(encryptedConnectionString);

            // TEMPORARY DEBUG
            System.Diagnostics.Debug.WriteLine(
                "Decrypted DB: " + _connectionString);
        }

        public DataTable ExecuteDataTable(
            string procName,
            SqlParameter[] parameters = null)
        {
            using var conn = new SqlConnection(_connectionString);

            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            using var da = new SqlDataAdapter(cmd);

            var dt = new DataTable();

            da.Fill(dt);

            return dt;
        }

        public int ExecuteNonQuery(
            string procName,
            SqlParameter[] parameters = null)
        {
            using var conn = new SqlConnection(_connectionString);

            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            conn.Open();

            return cmd.ExecuteNonQuery();
        }

        public object ExecuteScalar(
            string procName,
            SqlParameter[] parameters = null)
        {
            using var conn = new SqlConnection(_connectionString);

            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            conn.Open();

            return cmd.ExecuteScalar();
        }

        public SqlCommand PrepareCommand(
            SqlConnection conn,
            string procName,
            SqlParameter[] parameters = null)
        {
            var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            return cmd;
        }

        public string ConnectionString => _connectionString;
    }
}