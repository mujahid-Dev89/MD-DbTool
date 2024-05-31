

# MD-DbToolLibrary
The MD-DbToolLibrary is a powerful .NET database utility library that simplifies common database operations and provides a robust set of features to streamline your data-driven application development.

# Features
- Connection Management: Easily manage database connections with the ConnectionString() and ConnectionStringHive() methods, which allow you to retrieve and customize connection strings.
- Data Retrieval: Retrieve data from the database using various methods like GetDataTable(), GetMonoCodeDataTable(), and ----GetDataTableHive(), which return data in a DataTable format.
- Data Manipulation: Perform database operations such as inserting, updating, and deleting data using methods like InsertData(), UpdateColumnReturnRows(), and UpdateColumn().
- Single Value Retrieval: Retrieve single values from the database using the GetSingleData() and GetSingleValueFromTable() methods.
- Data Validation: Check for the existence of data in the database using the SingleItemSearch() and SingleValueValidityCheck() methods.
- Asynchronous Support: Leverage the power of asynchronous programming with methods like GetDataTableMultiParameters().
- Robust Error Handling: The library provides comprehensive error handling to ensure your application remains stable and reliable.
# Getting Started
1. Install the library from the NuGet package manager or by adding the following package to your project:
```
Install-Package MD-DbToolLibrary
```
2. Import the necessary namespace in your C# file:

```
using MDDbTool;
```

3. Create an instance of the MDDbToolLibrary class and utilize the available methods to interact with your database.

```
var dbTool = new MDDbToolLibrary(@"Server=DESKTOP-MDDBTOOL\SQLEXPRESS,90999;Database=NorthWind;User Id=sa;Password=1234; MultipleActiveResultSets=True;Connect Timeout=30");
string connectionString = dbTool.ConnectionString();
DataTable data = dbTool.GetDataTable("MyTable", "id, name", "WHERE id = 1");
```
# Documentation
Detailed documentation for the MD-DbToolLibrary, including method descriptions, parameters, and examples, can be found in the repository's wiki.

# Contributing
Contributions to the MD-DbToolLibrary are welcome! If you encounter any issues, have feature requests, or would like to contribute code, please feel free to open an issue or submit a pull request on the GitHub repository.

#License
The MD-DbToolLibrary is licensed under the MIT License.
