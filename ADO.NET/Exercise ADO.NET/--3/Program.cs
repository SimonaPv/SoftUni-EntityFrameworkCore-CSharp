using __3;
using Microsoft.Data.SqlClient;
using System.Text;

await using SqlConnection connection = new SqlConnection(Config.ConnString);
await connection.OpenAsync();

int id = int.Parse(Console.ReadLine());
string result = await GetById(connection, id);

Console.WriteLine(result);


static async Task<string> GetById(SqlConnection connection, int villianId)
{
    StringBuilder sb = new StringBuilder();

    SqlCommand getVillianNameCmd = new SqlCommand(SqlQueries.SelectVillianId, connection);
    getVillianNameCmd.Parameters.AddWithValue("@Id", villianId);

    object? villianNameObj = await getVillianNameCmd.ExecuteScalarAsync();
    if (villianNameObj is null)
    {
        return $"No villain with ID {villianId} exists in the database.";
    }

    string villianName = (string)villianNameObj;

    SqlCommand getMinions = new SqlCommand(SqlQueries.SelectMinionsByVillianId, connection);
    getMinions.Parameters.AddWithValue("@Id", villianId);

    sb.AppendLine($"Villain: {villianName}");

    SqlDataReader reader = await getMinions.ExecuteReaderAsync();
    if (!reader.HasRows)
    {
        sb.AppendLine("(no minions)");
    }
    else
    {
        while (reader.Read())
        {
            long rowNum = (long)reader["RowNum"];
            string minName = (string)reader["Name"];
            int age = (int)reader["Age"];

            sb.AppendLine($"{rowNum}. {minName} {age}");
        }
    }

    return sb.ToString().TrimEnd();
}