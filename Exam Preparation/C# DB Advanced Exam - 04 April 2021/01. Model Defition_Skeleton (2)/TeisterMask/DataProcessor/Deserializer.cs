// ReSharper disable InconsistentNaming

namespace TeisterMask.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Xml.Serialization;
    using System.Text;
    using TeisterMask.DataProcessor.ImportDto;
    using TeisterMask.Data.Models;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;
    using System.Linq;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var projectDtos = Deserialize<ImportProjectDto[]>(xmlString, "Projects");

            ICollection<Project> projects = new HashSet<Project>();

            foreach (var projDto in projectDtos)
            {
                bool isOpenValid = DateTime.TryParseExact(projDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validOpenDateProj);

                bool isDueValid = true;
                DateTime? validDueDateProj = null;

                if (!string.IsNullOrEmpty(projDto.DueDate))
                {
                    isDueValid = DateTime.TryParseExact(projDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);

                    validDueDateProj = parsedDate;
                }

                if (!IsValid(projDto) ||
                    !isDueValid ||
                    !isOpenValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Project project = new Project()
                {
                    Name = projDto.Name,
                    OpenDate = validOpenDateProj,
                    DueDate = validDueDateProj
                };

                ICollection<Task> tasks = new HashSet<Task>();

                foreach (var taskDto in projDto.Tasks)
                {
                    bool isOpenValidTask = DateTime.TryParseExact(taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validOpenDateTask);
                    bool isDueValidTask = DateTime.TryParseExact(taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validDueDateTask);

                    if (validDueDateProj != null)
                    {
                        if (!IsValid(taskDto) ||
                        !isDueValidTask ||
                        !isOpenValidTask ||
                        validOpenDateTask < validOpenDateProj ||
                        validDueDateTask > validDueDateProj)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                    }

                    else
                    {
                        if (!IsValid(taskDto) ||
                       !isDueValidTask ||
                       !isOpenValidTask ||
                       validOpenDateTask < validOpenDateProj)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                    }


                    Task task = new Task()
                    {
                        Name = taskDto.Name,
                        OpenDate = validOpenDateTask,
                        DueDate = validDueDateTask,
                        ExecutionType = (ExecutionType)taskDto.ExecutionType,
                        LabelType = (LabelType)taskDto.LabelType
                    };

                    tasks.Add(task);
                }

                project.Tasks = tasks;
                projects.Add(project);
                sb.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, tasks.Count));
            }

            context.Projects.AddRange(projects);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var employeeDtos = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);

            ICollection<Employee> employees = new HashSet<Employee>();

            foreach (var emplDto in employeeDtos)
            {
                if (!IsValid(emplDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Employee employee = new Employee()
                {
                    Username = emplDto.Username,
                    Email = emplDto.Email,
                    Phone = emplDto.Phone
                };

                ICollection<EmployeeTask> tasks = new HashSet<EmployeeTask>();

                foreach (var taskId in emplDto.Tasks.Distinct())
                {
                    Task task = context.Tasks.FirstOrDefault(t => t.Id == taskId);

                    if (task == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    EmployeeTask employeeTask = new EmployeeTask()
                    {
                        Employee = employee,
                        Task = task
                    };

                    tasks.Add(employeeTask);
                }

                employee.EmployeesTasks = tasks;
                employees.Add(employee);
                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, emplDto.Username, tasks.Count));
            }

            context.Employees.AddRange(employees);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), root);

            using StringReader reader = new StringReader(inputXml);

            T dtos = (T)serializer.Deserialize(reader);
            return dtos;
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}