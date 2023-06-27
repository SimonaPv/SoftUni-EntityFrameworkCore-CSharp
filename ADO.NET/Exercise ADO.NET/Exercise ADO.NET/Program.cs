using __2;
using Microsoft.Data.SqlClient;
using System.Text;

await using SqlConnection connection = new SqlConnection(Config.ConnString);
await connection.OpenAsync();

string result = await GetAllVillians(connection);

Console.WriteLine("Connected to the DB...");
Console.WriteLine(result);



static async Task<string> GetAllVillians(SqlConnection connection)
{
    StringBuilder sb = new StringBuilder();

    SqlCommand command = new SqlCommand(SqlQueries.Select, connection);

    //One row with many columns
    //First the reader hasn't loaded any data. We must call Read() first!
    SqlDataReader reader = await command.ExecuteReaderAsync();
    while(reader.Read())
    {
        string villianName = (string)reader["Name"];
        int minionsCount = (int)reader["MinionsCount"];

        sb.AppendLine($"{villianName} - {minionsCount}");
    }

    return sb.ToString().TrimEnd();
}