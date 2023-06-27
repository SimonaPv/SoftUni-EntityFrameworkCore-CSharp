namespace TeisterMask.DataProcessor
{
    using Data;
    using Microsoft.VisualBasic;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context.Projects
                .ToArray()
                .Where(p => p.Tasks.Any())
                .Select(p => new ExportProjectDto()
                {
                    TasksCount = p.Tasks.Count,
                    ProjectName = p.Name,
                    HasEndDate = (p.DueDate == null ? "No" : "Yes"),
                    Tasks = p.Tasks
                             .ToArray()
                             .Select(t => new ExportTaskDto()
                             {
                                 Name = t.Name,
                                 Label = t.LabelType.ToString()
                             })
                             .OrderBy(t => t.Name)
                             .ToArray()
                })
                .OrderByDescending(p => p.TasksCount)
                .ThenBy(p => p.ProjectName)
                .ToArray();

            return Serialize<ExportProjectDto[]>(projects, "Projects");
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
                .ToArray()
                .Where(e => e.EmployeesTasks.Select(t => t.Task).Any(t => t.OpenDate >= date))
                .Select(e => new
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                             .ToArray()
                             .Select(t => t.Task)
                             .Where(t => t.OpenDate >= date)
                             .OrderByDescending(t => t.DueDate)
                             .ThenBy(t => t.Name)
                             .Select(t => new
                             {
                                 TaskName = t.Name,
                                 OpenDate = t.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                                 DueDate = t.DueDate.ToString("d", CultureInfo.InvariantCulture),
                                 LabelType = t.LabelType.ToString(),
                                 ExecutionType = t.ExecutionType.ToString()
                             })
                             .ToArray()
                })
                .OrderByDescending(e => e.Tasks.Count())
                .ThenBy(e => e.Username)
                .Take(10)
                .ToArray();

            return JsonConvert.SerializeObject(employees, Formatting.Indented);
        }

        private static string Serialize<T>(T dataTransferObjects, string rootName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));

            StringBuilder sb = new StringBuilder();
            using var writer = new StringWriter(sb);

            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, dataTransferObjects, xmlNamespaces);

            return sb.ToString();
        }
    }
}