using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MDDbTool
{
    public class MDDbToolLibrary
    {

        public static string ConnectionStringHub;
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns>The connection string.</returns>
        public string ConnectionString()
        {
            return ConnectionStringHub;
        }
        
        /// <summary>
        /// Executes the given SQL query and returns the first column of the first row in the result set.
        /// </summary>
        /// <param name="Query">The SQL query to be executed.</param>
        /// <returns>The first column of the first row in the result set, or an empty string if no result is found.</returns>
        public string GetSingleValueFullQuery(string Query)
        {
            string data = "";
            string CS = ConnectionString();
            using (SqlConnection con = new SqlConnection(CS))
            {
                try
                {

                    string query = string.Format(Query);
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            data = rdr[0].ToString();
                        }
                    }

                    con.Close();
                }
                catch (SqlException ex)
                {

                }
            }
            return data;
        }
        /// <summary>
        /// Creates a SQL Server connection string based on the provided parameters.
        /// </summary>
        /// <param name="server">Server address.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="userName">User name for authentication.</param>
        /// <param name="password">Password for authentication.</param>
        /// <param name="IntegratedSecurity">True for Windows authentication, false for SQL Server authentication.</param>
        /// <param name="MultipleActiveResultSets">True to enable Multiple Active Result Sets (MARS), false otherwise.</param>
        /// <returns>The SQL Server connection string.</returns>
        private string CreateConnectionString(string server, string databaseName, string userName, string password, bool IntegratedSecurity, bool MultpleActiveResultSets)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server, // server address
                InitialCatalog = databaseName, // database name
                IntegratedSecurity = IntegratedSecurity, // server auth(false)/win auth(true)
                MultipleActiveResultSets = MultpleActiveResultSets, // activate/deactivate MARS
                PersistSecurityInfo = true, // hide login credentials
                UserID = userName, // user name
                Password = password // password
            };
            return builder.ConnectionString;
        }
        /// <summary>
        /// Retrieves values from a single column in a database table based on the provided parameters.
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <param name="fieldName">Name of the field to be retrieved.</param>
        /// <param name="condition">Optional condition to filter the data.</param>
        /// <param name="qty">Optional parameter to limit the number of rows returned.</param>
        /// <returns>An array containing values from the specified column.</returns>

        public string[] SingleValueArrayWithOutSize(string TableName, string fieldName, string condition = "", string qty = "")
        {
            try
            {
                List<string> resultData = new List<string>();
                string[] result;
                string query = "";
                String CS = ConnectionString();

                using (SqlConnection con = new SqlConnection(CS))
                {
                    query = string.Format("SELECT {3} {0} FROM {1} {2}; ", fieldName, TableName, condition, qty);


                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {

                        int counter = 0;

                        while (rdr.Read())
                        {

                            resultData.Add(rdr[0].ToString());
                            counter++;
                        }

                    }
                    rdr.Close();
                    con.Close();
                    cmd.Dispose();
                }
                if (resultData.Count != 0)
                {
                    result = new string[resultData.Count];
                    result = resultData.ToArray();
                }
                return resultData.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Array.Empty<string>();
        }
        /// <summary>
        /// Updates the specified columns in a database table based on the provided parameters and returns the number of affected rows.
        /// </summary>
        /// <param name="Tablename">Name of the table to be updated.</param>
        /// <param name="UpdateStatement">Update statement specifying the columns and their new values.</param>
        /// <param name="condition">Optional condition to filter the rows to be updated.</param>
        /// <returns>The number of rows affected by the update operation.</returns>

        public int UpdateColumnReturn(string Tablename, string UpdateStatement, string condition)
        {
            int returvalue = 0;
            bool done = false;
            string CS = ConnectionString();
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format("Update {0} SET {1} {2}", Tablename, UpdateStatement, condition);
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                returvalue = cmd.ExecuteNonQuery();
                con.Close();
                done = true;

            }
            return returvalue;
        }
        /// <summary>
        /// Inserts data into the specified table and returns a boolean indicating the success of the operation.
        /// </summary>
        /// <param name="TableName">Name of the table to insert data into.</param>
        /// <param name="values">Values to be inserted into the table.</param>
        /// <param name="idcolumnname">Name of the ID column (default is "id").</param>
        /// <param name="columns">Optional parameter specifying columns to insert data into.</param>
        /// <param name="customdbstring">Custom database connection string (optional).</param>
        /// <returns>True if the data was successfully inserted, false otherwise.</returns>
        public bool insertDataReturnBool(string TableName, string values, string idcolumnname = "id", string columns = "", string customdbstring = "")
        {
            try
            {

                string CS = "";
                if (customdbstring == "")
                {
                    CS = ConnectionString();
                }
                else
                {
                    CS = customdbstring;
                }
                string outid = "";

                using (SqlConnection con = new SqlConnection(CS))
                {
                    string query = string.Format("INSERT INTO {0} {1} {3}  VALUES {2};", TableName, columns, values, outid);

                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        /// <summary>
        /// Executes a non-query SQL operation and returns the number of rows affected.
        /// </summary>
        /// <param name="TableData">SQL operation to be executed.</param>
        /// <returns>The number of rows affected by the SQL operation.</returns> 
        public int ExecuteNonQueryOperations(string TableData)
        {
            int returnData = 0;
            string CS = "";

            CS = ConnectionString();

            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(TableData, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

            }
            return returnData;
        }
        /// <summary>
        /// Executes a SQL query with parameters and returns the results in a DataTable.
        /// </summary>
        /// <param name="query">The SQL query to be executed.</param>
        /// <param name="parameterNames">List of parameter names.</param>
        /// <param name="parameterValues">List of parameter values corresponding to the parameter names.</param>
        /// <returns>A DataTable containing the results of the query.</returns>
        public DataTable GetDataTableMultiParameters(string query, List<string> parameterNames, List<string> parameterValues)
        {
            string connectionString = ConnectionString();
            SqlConnection Conn;
            Conn = new SqlConnection(connectionString);
            SqlCommand command;

            command = new SqlCommand(query, Conn);
            Conn.Open();
            if (parameterNames != null && parameterNames.Count > 0)
            {
                for (int i = 0; i < parameterNames.Count; i++)
                {
                    command.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                }
            }

            DataTable dataTable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);
            Conn.Close();
            return dataTable;
        }

       
        /// <summary>
        /// Searches for a specific keyword in a specified column of a database table.
        /// </summary>
        /// <param name="columnname">Name of the column to search in.</param>
        /// <param name="tablename">Name of the table to search in.</param>
        /// <param name="keyword">Keyword to search for.</param>
        /// <returns>True if the keyword is found in the specified column, false otherwise.</returns>

        public bool SingleItemSearch(string columnname, string tablename, string keyword)
        {
            bool Result = false;
            String CS = ConnectionString();
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format("select {0} from {1} where {0} like '%{2}%'", columnname, tablename, keyword);
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    Result = true;
                }
                con.Close();
            }
            return Result;
        }
        /// <summary>
        /// Retrieves data from a specific table in the database based on the specified columns and condition.
        /// </summary>
        /// <param name="TableName">The name of the table to retrieve data from.</param>
        /// <param name="columnname">The columns to be retrieved (default is "*").</param>
        /// <param name="condition">The condition to filter the data (default is "").</param>
        /// <param name="need_from">A boolean indicating whether to include "FROM" in the SQL query (default is true).</param>
        /// <returns>A DataTable containing the retrieved data.</returns>
        public DataTable GetDataTable(string TableName, string columnname = "*", string condition = "")
        {

            DataTable dt = new DataTable();
            string CS = ConnectionString();
            SqlConnection con = new SqlConnection(CS);

            string query = string.Format("select {0}  {1} {2}", columnname, TableName, condition);
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                dt.Load(rdr);
            }
            con.Close();
            return dt;
        }
        /// <summary>
        /// Retrieves data from the database based on provided parameters for MonoCode.
        /// </summary>
        /// <param name="TableName">Name of the table to retrieve data from.</param>
        /// <param name="columnname">Columns to be retrieved from the table.</param>
        /// <param name="condition">Additional conditions for the SQL query.</param>
        /// <param name="Parameter">Parameter for the MonoCode.</param>
        /// <returns>DataTable containing the retrieved data.</returns>
        public DataTable GetMonoCodeDataTable(string TableName, string columnname = "*", string condition = "", string Parameter = "")
        {

            DataTable dt = new DataTable();
            string CS = ConnectionString();
            SqlConnection con = new SqlConnection(CS);

            string query = string.Format("select {0} {1} {2}", columnname, TableName, condition);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@MonoCode", Parameter);
            con.Open();

            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                dt.Load(rdr);
            }
            con.Close();
            return dt;
        }

        /// <summary>
        /// Updates rows in the specified table based on the provided update statement and condition.
        /// </summary>
        /// <param name="Tablename">Name of the table to update.</param>
        /// <param name="UpdateStatement">Update statement to set column values.</param>
        /// <param name="condition">Additional conditions for the SQL query.</param>
        /// <param name="customdbstring">Custom database connection string.</param>
        /// <returns>The number of rows affected by the update operation.</returns>

        public int UpdateColumnReturnRows(string Tablename, string UpdateStatement, string condition, string customdbstring = "")
        {
            int done = 0;
            string CS = "";
            if (customdbstring == "")
            {
                CS = ConnectionString();
            }
            else
            {
                CS = customdbstring;
            }
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format("Update {0} SET {1} {2}", Tablename, UpdateStatement, condition);
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                done = cmd.ExecuteNonQuery();
                con.Close();
            }
            return done;
        }
        /// <summary>
        /// Updates rows in the specified table based on the provided update statement, condition, and parameter.
        /// </summary>
        /// <param name="Tablename">Name of the table to update.</param>
        /// <param name="UpdateStatement">Update statement to set column values.</param>
        /// <param name="condition">Additional conditions for the SQL query.</param>
        /// <param name="customdbstring">Custom database connection string.</param>
        /// <param name="Parameter">Parameter for the MonoCode.</param>
        /// <returns>The number of rows affected by the update operation.</returns>
        public int UpdateTableRowsWithCondition(string Tablename, string UpdateStatement, string condition, string customdbstring = "", string Parameter = "")
        {
            int done = 0;
            string CS = "";
            if (customdbstring == "")
            {
                CS = ConnectionString();
            }
            else
            {
                CS = customdbstring;
            }
            using (SqlConnection con1 = new SqlConnection(CS))
            {
                string query = string.Format("Update {0} SET {1} {2}", Tablename, UpdateStatement, condition);
                SqlCommand cmd1 = new SqlCommand(query, con1);
                cmd1.Parameters.AddWithValue("@MonoCode", Parameter);
                con1.Open();
                done = cmd1.ExecuteNonQuery();
                con1.Close();
            }
            return done;
        }
        /// <summary>
        /// Updates rows in the specified table based on the provided update statement and condition.
        /// </summary>
        /// <param name="Tablename">Name of the table to update.</param>
        /// <param name="UpdateStatement">Update statement to set column values.</param>
        /// <param name="condition">Additional conditions for the SQL query.</param>
        /// <param name="customdbstring">Custom database connection string.</param>
        /// <returns>True if the update operation is successful, otherwise false.</returns>
        public bool UpdateColumn(string Tablename, string UpdateStatement, string condition, string customdbstring = "")
        {
            bool done = false;
            string CS = "";
            if (customdbstring == "")
            {
                CS = ConnectionString();
            }
            else
            {
                CS = customdbstring;
            }
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format("Update {0} SET {1} {2}", Tablename, UpdateStatement, condition);
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                done = true;

            }
            return done;
        }
        /// <summary>
        /// Inserts data into the specified table with return of the inserted ID.
        /// </summary>
        /// <param name="TableName">Name of the table to insert data into.</param>
        /// <param name="values">Values to be inserted into the table.</param>
        /// <param name="idcolumnname">Name of the ID column in the table.</param>
        /// <param name="returnid">Flag to indicate whether to return the inserted ID.</param>
        /// <param name="columns">Columns to be inserted into the table.</param>
        /// <param name="customdbstring">Custom database connection string.</param>
        /// <returns>If returnid is true, returns the ID of the inserted row. Otherwise, returns 0.</returns>
        public int InsertData(string TableName, string values, string idcolumnname = "id", bool returnid = false, string columns = "", string customdbstring = "")
        {
            int returnData = 0;
            string CS = "";
            if (customdbstring == "")
            {
                CS = ConnectionString();
            }
            else
            {
                CS = customdbstring;
            }
            string outid = "";
            if (returnid)
            {
                outid = string.Format("OUTPUT INSERTED.{0}", idcolumnname);

            }

            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format("INSERT INTO {0} {1} {3}  VALUES {2};", TableName, columns, values, outid);

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                if (returnid)
                {
                    returnData = (Int32)cmd.ExecuteScalar();
                }
                else
                {
                    cmd.ExecuteNonQuery();
                }
                con.Close();

            }
            return returnData;
        }

        /// <summary>
        /// Retrieves a single value from the specified column in the table based on the provided conditions.
        /// </summary>
        /// <param name="Tablename">Name of the table to retrieve data from.</param>
        /// <param name="ColumnName">Name of the column to retrieve data from.</param>
        /// <param name="condition">Additional conditions for the SQL query.</param>
        /// <param name="customdbstring">Custom database connection string.</param>
        /// <returns>The retrieved single value as a string.</returns>
        public string GetSingleData(string Tablename, string ColumnName, string condition = "", string customdbstring = "")
        {
            string data = "";
            string CS = "";
            if (customdbstring == "")
            {
                CS = ConnectionString();
            }
            else
            {
                CS = customdbstring;
            }
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format("select {0} from {1} {2}", ColumnName, Tablename, condition);

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        data = rdr[0].ToString();
                    }
                }
                con.Close();
            }
            return data;
        }
        /// <summary>
        /// Counts the number of rows in the specified table based on the provided column and conditions.
        /// </summary>
        /// <param name="columnname">Name of the column to be counted.</param>
        /// <param name="tablename">Name of the table to count rows from.</param>
        /// <param name="condition">Additional conditions for the SQL query.</param>
        /// <returns>The count of rows as an integer.</returns>
        public int CountSingleItem(string columnname, string tablename, string condition = "")
        {
            int Result = 0;
            String CS = ConnectionString();
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format("select count({0}) from {1} {2} ", columnname, tablename, condition);
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        Result = int.Parse(rdr[0].ToString());
                    }
                }
                con.Close();
            }
            return Result;
        }

        /// <summary>
        /// Retrieves a single value from the specified column in the table based on the provided conditions and parameter.
        /// </summary>
        /// <param name="Tablename">Name of the table to retrieve data from.</param>
        /// <param name="ColumnName">Name of the column to retrieve data from.</param>
        /// <param name="condition">Additional conditions for the SQL query.</param>
        /// <param name="Parameter">Parameter for the MonoCode.</param>
        /// <returns>The retrieved single value as a string.</returns>
        public string GetSingleValueFromTable(string Tablename, string ColumnName, string condition = "", string Parameter = "")
        {
            string data = "";
            string CS = "";

            CS = ConnectionString();

            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format("select {0} from {1} {2}", ColumnName, Tablename, condition);

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.Parameters.AddWithValue("@MonoCode", Parameter);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        data = rdr[0].ToString();
                    }
                }
                con.Close();
            }
            return data;
        }
        /// <summary>
        /// Retrieves data from the specified table based on the provided parameters and custom database connection string.
        /// </summary>
        /// <param name="TableName">Name of the table to retrieve data from.</param>
        /// <param name="columnname">Columns to be retrieved from the table.</param>
        /// <param name="condition">Additional conditions for the SQL query.</param>
        /// <param name="customdbstring">Custom database connection string.</param>
        /// <returns>DataTable containing the retrieved data.</returns>
        public DataTable GetDataTableWithConnection(string TableName, string columnname = "*", string condition = "", string customdbstring = "")
        {

            DataTable dt = new DataTable();
            string CS = "";
            if (customdbstring == "")
            {
                CS = ConnectionString();
            }
            else
            {
                CS = customdbstring;
            }
            SqlConnection con = new SqlConnection(CS);

            string query = string.Format(" {0}  {1} {2}", columnname, TableName, condition);
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                dt.Load(rdr);
            }
            con.Close();
            return dt;
        }
        /// <summary>
        /// Single Value Validity Check
        /// </summary>
        /// <param name="Tablename">Table Name</param>
        /// <param name="columnName">Column Name</param>
        /// <param name="condition">Condition</param>
        /// <param name="keyword">Condition for like operation</param>
        /// <returns></returns>
        public bool SingleValueValidityCheck(string Tablename, string columnName, string condition, string keyword)
        {
            bool result = false;
            string CS = ConnectionString();
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format("Select {0} from {1} {2} '{3}'", columnName, Tablename, condition, keyword);
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    result = true;
                }
                con.Close();
            }
            return result;
        }

       
        /// <summary>
        /// Checks the validity of a single value in the specified column of the table based on the provided conditions.
        /// </summary>
        /// <param name="Tablename">Name of the table to check for validity.</param>
        /// <param name="columnName">Name of the column to check.</param>
        /// <param name="condition">Additional conditions for the SQL query.</param>
        /// <returns>True if the value exists in the specified column, otherwise false.</returns>
        public bool SingleValueValidityCheckWithoutKeyword(string Tablename, string columnName, string condition)
        {
            bool result = false;
            string CS = ConnectionString();
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format("Select {0} from {1} {2}", columnName, Tablename, condition);
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    result = true;
                }
                con.Close();
            }
            return result;
        }

        /// <summary>
        /// Retrieves an array of strings containing values from the specified column of a table based on the provided conditions.
        /// </summary>
        /// <param name="arraysize">Size of the resulting string array.</param>
        /// <param name="TableName">Name of the table to retrieve data from.</param>
        /// <param name="fieldName">Name of the column to retrieve data from.</param>
        /// <param name="condition">Additional conditions for the SQL query.</param>
        /// <param name="qty">Quantity specifier for the SQL query.</param>
        /// <returns>Array of strings containing values from the specified column.</returns>
        public string[] SingleValueArray(int arraysize, string TableName, string fieldName, string condition = "", string qty = "")
        {
            string[] result;
            string query = "";

            String CS = ConnectionString();

            using (SqlConnection con = new SqlConnection(CS))
            {
                query = string.Format("SELECT {3} {0} FROM {1} {2}; ", fieldName, TableName, condition, qty);
                result = new string[arraysize];

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {

                    int counter = 0;

                    while (rdr.Read())
                    {

                        result[counter] = rdr[0].ToString();
                        counter++;
                    }
                }
                rdr.Close();
                con.Close();
                cmd.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Retrieves a single value from the database using a custom SQL query.
        /// </summary>
        /// <param name="SqlStr">Custom SQL query to execute.</param>
        /// <param name="customdbstring">Custom database connection string if applicable.</param>
        /// <returns>A string containing the retrieved value.</returns>
        public string GetSingleDataPassingFullQuery(string SqlStr, string customdbstring = "")
        {
            string data = "";
            string CS = "";
            if (customdbstring == "")
            {
                CS = ConnectionString();
            }
            else
            {
                CS = customdbstring;
            }
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format(SqlStr);

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        data = rdr[0].ToString();
                    }
                }
                con.Close();
            }
            return data;
        }

        /// <summary>
        /// Retrieves an array of strings containing values from a custom SQL query.
        /// </summary>
        /// <param name="arraysize">Size of the resulting string array.</param>
        /// <param name="SqlQry">Custom SQL query to execute.</param>
        /// <returns>Array of strings containing values from the custom SQL query.</returns>
        public string[] SingleValueArrayPassingFullQuery(int arraysize, string SqlQry)
        {
            string[] result;
            string query = "";

            String CS = ConnectionString();

            using (SqlConnection con = new SqlConnection(CS))
            {
                query = string.Format(SqlQry);
                result = new string[arraysize];

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {

                    int counter = 0;

                    while (rdr.Read())
                    {

                        result[counter] = rdr[0].ToString();
                        counter++;
                    }
                }
                rdr.Close();
                con.Close();
                cmd.Dispose();
            }

            return result;
        }
        /// <summary>
        /// Retrieves a DataTable from a custom SQL query.
        /// </summary>
        /// <param name="SqlQuery">Custom SQL query to execute.</param>
        /// <returns>DataTable containing the result of the custom SQL query.</returns>
        public DataTable GetDataTablePassingFullQuery(string SqlQuery)
        {

            DataTable dt = new DataTable();
            string CS = ConnectionString();
            SqlConnection con = new SqlConnection(CS);

            SqlCommand cmd = new SqlCommand(SqlQuery, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                dt.Load(rdr);
            }
            con.Close();
            return dt;
        }
        
        /// <summary>
        /// Deletes records from the specified table based on the provided conditions.
        /// </summary>
        /// <param name="Tablename">Name of the table from which to delete records.</param>
        /// <param name="condition">Additional conditions for the SQL query.</param>
        /// <param name="customdbstring">Custom database connection string if applicable.</param>
        /// <returns>True if deletion is successful, otherwise false.</returns>
        public bool DeleteColumn(string Tablename, string condition, string customdbstring = "")
        {
            bool done = false;
            string CS = "";
            if (customdbstring == "")
            {
                CS = ConnectionString();
            }
            else
            {
                CS = customdbstring;
            }
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = string.Format("DELETE FROM {0} {1} ", Tablename, condition);
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                done = true;

            }
            return done;
        }

        /// <summary>
        /// Gets data from a stored procedure and returns the result as a DataTable.
        /// </summary>
        /// /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameterNames">A list of parameter names for the stored procedure.</param>
        /// <param name="parameterValues">A list of parameter values corresponding to the parameter names.</param>
        /// <returns>A DataTable containing the result of the stored procedure execution.</returns>
        public DataTable SP_GetDataTable(string procedureName, List<string> parameterNames, List<string> parameterValues)
        {
            try
            {
                string connectionString = ConnectionString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(procedureName, connection))
                    {
                        connection.Open();
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters if provided
                        if (parameterNames != null && parameterNames.Count > 0)
                        {
                            for (int i = 0; i < parameterNames.Count; i++)
                            {
                                command.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                            }
                        }

                        DataTable dataTable = new DataTable();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);

                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions here (e.g., log the error)
                Console.WriteLine("Error: " + ex.Message);
                return null; // Return null or an empty DataTable in case of an error
            }
        }

        /// <summary>
        /// Executes a stored procedure and returns a boolean value indicating whether the result set contains any rows.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameterNames">A list of parameter names for the stored procedure.</param>
        /// <param name="parameterValues">A list of parameter values corresponding to the parameter names.</param>
        /// <returns>True if the stored procedure result set contains any rows; otherwise, false.</returns>

        public bool SP_GetSingleBoolValue(string procedureName, List<string> parameterNames, List<string> parameterValues)
        {
            bool result = false;
            string connectionString = ConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters if provided
                    if (parameterNames != null && parameterNames.Count > 0)
                    {
                        for (int i = 0; i < parameterNames.Count; i++)
                        {
                            command.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                        }
                    }
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Executes a stored procedure and returns a single string value from the first row of the result set.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameterNames">A list of parameter names for the stored procedure.</param>
        /// <param name="parameterValues">A list of parameter values corresponding to the parameter names.</param>
        /// <returns>A string containing the value from the first row of the stored procedure result set; an empty string if no rows are returned.</returns>
        public string SP_GetSingleStringValue(string procedureName, List<string> parameterNames, List<string> parameterValues)
        {
            string result = "";
            string connectionString = ConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters if provided
                    if (parameterNames != null && parameterNames.Count > 0)
                    {
                        for (int i = 0; i < parameterNames.Count; i++)
                        {
                            command.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                        }
                    }
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read(); // Move to the first row if available
                            result = reader[0].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions here
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Executes a stored procedure and returns a single integer value from the first row of the result set.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameterNames">A list of parameter names for the stored procedure.</param>
        /// <param name="parameterValues">A list of parameter values corresponding to the parameter names.</param>
        /// <returns>An integer containing the value from the first row of the stored procedure result set; 0 if no rows are returned.</returns>
        public int SP_GetSingleIntValue(string procedureName, List<string> parameterNames, List<string> parameterValues)
        {
            int result = 0;
            string connectionString = ConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters if provided
                    if (parameterNames != null && parameterNames.Count > 0)
                    {
                        for (int i = 0; i < parameterNames.Count; i++)
                        {
                            command.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                        }
                    }
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Executes a stored procedure with the given parameter names and values and returns a boolean indicating success.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameterNames">List of parameter names.</param>
        /// <param name="parameterValues">List of parameter values corresponding to the parameter names.</param>
        /// <returns>True if the stored procedure is executed successfully; false otherwise.</returns>
        public bool Sp_ExecuteNonQuery(string procedureName, List<string> parameterNames, List<string> parameterValues)
        {
            bool result = false;
            string connectionString = ConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters if provided
                    if (parameterNames != null && parameterNames.Count > 0)
                    {
                        for (int i = 0; i < parameterNames.Count; i++)
                        {
                            command.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                        }
                    }
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

    }
}
