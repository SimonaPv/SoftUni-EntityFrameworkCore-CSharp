using __1.Data;
using Microsoft.EntityFrameworkCore;

using (SoftUniContext context = new SoftUniContext())
{
    var employees1 = await context.Employees
        .Where(e => e.ManagerId == 185)
        .Include(e => e.Manager)
        .ToListAsync();

    foreach (var e in employees1)
    {
        Console.WriteLine($"{e.FirstName} {e.LastName} - {e.Manager.FirstName} {e.Manager.LastName}");
    }

    

    var employees2 = await context.Employees
        .Where(e => e.ManagerId == 185)
        .Include(e => e.Manager)
        .Select(e => new
        {
            FirstName = e.FirstName,
            LastName = e.LastName,
            Manager = e.Manager.FirstName + " " + e.Manager.LastName
        })
        .ToListAsync();

    foreach (var e in employees2)
    {
        Console.WriteLine($"{e.FirstName} {e.LastName} - {e.Manager}");
    }
}