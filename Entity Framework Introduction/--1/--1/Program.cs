using __1.Data;
using __1.Models;
using Microsoft.EntityFrameworkCore;

using (SoftUniContext context = new SoftUniContext()) //създаваме контекст, който е репрезентацията на базата данни
{
    DateTime date = new DateTime(2000, 1, 1);

    List<Employee> employees = await context.Employees //от контекста взимаме съответната таблица АСИНХРОННО
        .Where(e => e.HireDate < date)
        .ToListAsync();

    foreach (var e in employees)
    {
        Console.WriteLine($"{e.FirstName} {e.LastName}");
    }

    var person = await context.Employees.FindAsync(30);

    Console.WriteLine();
    Console.WriteLine($"{person.FirstName} {person.LastName}");

    var richKid = await context.Employees
        .AsNoTracking() //използваме, когато показваме нещо, без да го променяме ==> тракера не го следи и пестим ресурси
        .OrderByDescending(e => e.Salary)
        .Select(e => new
        {
            e.FirstName,
            e.LastName,
            e.Salary
        }).FirstOrDefaultAsync();

    Console.WriteLine();
    Console.WriteLine($"{richKid.FirstName} {richKid.LastName} - {richKid.Salary}");

    Console.WriteLine();

    await context.Projects
        .AddAsync(new Project()
        {
            Name = "Judge",
            StartDate = DateTime.Today
        });

    await context.SaveChangesAsync(); //внимаваме, защото запазва напълно всички промени в базата
}