using System.Data.SqlClient;

string connString = @"Data Source=SIMONAS-LAPTOP\SQLEXPRESS;Initial Catalog=SoftUni;Integrated Security=True";
SqlConnection conn = new SqlConnection(connString);

conn.Open();

using (conn)
{
    SqlCommand cmd = new SqlCommand("SELECT * FROM Employees WHERE DepartmentId = 7", conn);
    SqlDataReader reader = await cmd.ExecuteReaderAsync();

    using (reader)
    {
        while (reader.Read())
        {
            string? firstName = (string?)reader["FirstName"]; 
            string? lastName = (string?)reader["LastName"]; 
            decimal? salary = (decimal?)reader["Salary"];

            Console.WriteLine($"{firstName} {lastName} - {salary:f2}");
        }
    }
}

void InsertProject(string name, string description, DateTime startDate)
{
    SqlCommand cmd = new SqlCommand(
            "INSERT INTO Projects " +
            "(Name, Description, StartDate, EndDate) VALUES " +
            "(@name, @desc, @start, @end)", conn);

    cmd.Parameters.AddWithValue("@name", name);
    cmd.Parameters.AddWithValue("@desc", description);
    cmd.Parameters.AddWithValue("@start", startDate);

    cmd.ExecuteNonQuery();
}