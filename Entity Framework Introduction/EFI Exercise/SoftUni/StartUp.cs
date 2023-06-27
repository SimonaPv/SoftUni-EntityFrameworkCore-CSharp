using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            string result = GetEmployeesInPeriod(context);

            Console.WriteLine(result);
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var EmployeeInfo = context.Employees
              .Take(10)
              .Select(e => new
              {
                  e.FirstName,
                  e.LastName,
                  ManagerFirstName = e.Manager!.FirstName,
                  ManagerLastName = e.Manager.LastName,
                  Projects = e.EmployeesProjects.Where(ep => ep.Project.StartDate.Year >= 2001 & ep.Project.StartDate.Year <= 2003)
                      .Select(ep => new
                      {
                          ProjectName = ep.Project.Name,
                          StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                          EndDate = ep.Project.EndDate != null
                              ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                              : "not finished"
                      })
              })
              .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in EmployeeInfo)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

                if (e.Projects.Any())
                {
                    sb.AppendLine(String.Join(Environment.NewLine, e.Projects
                        .Select(p => $"--{p.ProjectName} - {p.StartDate} - {p.EndDate}")));
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            Employee? employee = context.Employees //i know it can be null
                .FirstOrDefault(e => e.LastName == "Nakov");

            employee!.Address = address; //im sure it's not null

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => new
                {
                    AddressT = e.Address!.AddressText
                }).ToList();

            foreach (var e in employees)
            {
                sb.AppendLine(e.AddressT);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Include(e => e.Department)
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToList();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.Department.Name} - ${e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Employee[] employees = context.Employees
                .OrderBy(e => e.EmployeeId)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}