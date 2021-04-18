using System;
using System.Data;
using System.Data.SqlClient;

namespace Aqua.SQL.InsertStoredProcedureGenerator
{
    public class InsertStoredProcedureGenerator
    {
        /// <summary>
        /// Generates SQL Insert SQL Insert Stored Procedure
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="dataBase"></param>
        /// <param name="tableName"></param>
        /// <param name="generatedProcedureNamePrefix"></param>
        /// <returns></returns>
        public string GenerateProcedure(SqlConnection sqlConnection, string dataBase, string tableName, string generatedProcedureNamePrefix = "", string options = "")
        {
            try
            {
                if (sqlConnection == null)
                    throw new ArgumentNullException(nameof(sqlConnection));

                if (dataBase == null)
                    throw new ArgumentNullException(nameof(dataBase));

                if (tableName == null)
                    throw new ArgumentNullException(nameof(tableName));

                var schemaQuery = Helpers.BuildSchemaQuery(dataBase, tableName, options);

                var cmd = new SqlCommand(schemaQuery, sqlConnection);

                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                var dataTable = new DataTable();

                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dataTable);
                }

                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();

                var spParts = Helpers.BuildStoredProcedureParts(dataTable);

                return Helpers.BuildStoredProcedureCommand(dataBase, tableName, spParts, generatedProcedureNamePrefix);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
