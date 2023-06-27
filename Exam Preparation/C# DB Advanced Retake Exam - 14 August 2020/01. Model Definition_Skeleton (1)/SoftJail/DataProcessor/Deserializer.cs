namespace SoftJail.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Text.Json.Nodes;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";

        private const string SuccessfullyImportedDepartment = "Imported {0} with {1} cells";

        private const string SuccessfullyImportedPrisoner = "Imported {0} {1} years old";

        private const string SuccessfullyImportedOfficer = "Imported {0} ({1} prisoners)";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var depCellDtos = JsonConvert.DeserializeObject<ImportDepCellDto[]>(jsonString);

            ICollection<Department> departments = new HashSet<Department>();

            foreach (var depDto in depCellDtos)
            {
                if (!IsValid(depDto) || !depDto.Cells.Any())
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Department department = new Department()
                {
                    Name = depDto.Name
                };

                ICollection<Cell> cells = new HashSet<Cell>();
                foreach (var cellDto in depDto.Cells)
                {
                    if (!IsValid(cellDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        cells = null;
                        break;
                    }

                    Cell cell = new Cell()
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow
                    };

                    cells.Add(cell);
                }

                if (cells == null)
                {
                    continue;
                }

                department.Cells = cells;
                departments.Add(department);
                sb.AppendLine(string.Format(SuccessfullyImportedDepartment, department.Name, department.Cells.Count));
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var prisonerDtos = JsonConvert.DeserializeObject<ImportPrisMailDto[]>(jsonString);

            ICollection<Prisoner> prisoners = new HashSet<Prisoner>();

            foreach (var prisDto in prisonerDtos)
            {
                DateTime validIncarcerationDate;

                bool IsIncarValid = DateTime.TryParseExact(prisDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validIncarcerationDate);

                DateTime? validReleaseDate = null;
                bool isReleaseValid = true;

                if (prisDto.ReleaseDate != null)
                {
                    isReleaseValid = DateTime.TryParseExact(prisDto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);

                    validReleaseDate = parsedDate;
                }

                if (!IsValid(prisDto) || !IsIncarValid || !isReleaseValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Prisoner prisoner = new Prisoner()
                {
                    FullName = prisDto.FullName,
                    Nickname = prisDto.Nickname,
                    Age = prisDto.Age,
                    IncarcerationDate = validIncarcerationDate,
                    ReleaseDate = validReleaseDate,
                    Bail = prisDto.Bail,
                    CellId = prisDto.CellId
                };

                ICollection<Mail> mails = new HashSet<Mail>();

                foreach (var mailDto in prisDto.Mails)
                {
                    if (!IsValid(mailDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        mails = null;
                        break;
                    }

                    Mail mail = new Mail()
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address
                    };

                    mails.Add(mail);
                }

                if (mails == null)
                {
                    continue;
                }

                prisoner.Mails = mails;
                prisoners.Add(prisoner);
                sb.AppendLine(string.Format(SuccessfullyImportedPrisoner, prisoner.FullName, prisoner.Age));
            }

            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            ImportOfficerDto[] officerDtos = Deserialize<ImportOfficerDto[]>(xmlString, "Officers");
            StringBuilder sb = new StringBuilder();
            List<Officer> officers = new List<Officer>();

            foreach (var officerDto in officerDtos)
            {
                bool isSalaryValid = officerDto.Salary >= 0;
                bool isPositionValid = Enum.TryParse<Position>(officerDto.Position, out Position validPosition);
                bool isWeaponValid = Enum.TryParse<Weapon>(officerDto.Weapon, out Weapon validWeapon);

                if (!IsValid(officerDto)
                    || !isSalaryValid
                    || !isPositionValid
                    || !isWeaponValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Officer officer = new Officer()
                {
                    FullName = officerDto.FullName,
                    Salary = officerDto.Salary,
                    Position = validPosition,
                    Weapon = validWeapon,
                    DepartmentId = officerDto.DepartmentId
                };

                foreach (int prisonerId in officerDto.Prisoners.Select(p => p.Id))
                {
                    officer.OfficerPrisoners.Add(new OfficerPrisoner()
                    {
                        Officer = officer,
                        PrisonerId = prisonerId
                    });
                }
                officers.Add(officer);
                sb.AppendLine(String.Format(SuccessfullyImportedOfficer, officer.FullName, officer.OfficerPrisoners.Count));
            }
            context.Officers.AddRange(officers);
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

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}