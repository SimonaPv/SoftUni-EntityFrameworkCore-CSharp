namespace Footballers.DataProcessor
{
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var coachDtos = Deserialize<ImportCoachDto[]>(xmlString, "Coaches");

            ICollection<Coach> coaches = new HashSet<Coach>();

            foreach (var coachDto in coachDtos)
            {
                if (!IsValid(coachDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Coach coach = new Coach()
                {
                    Name = coachDto.Name,
                    Nationality = coachDto.Nationality
                };

                ICollection<Footballer> footballers = new HashSet<Footballer>();

                foreach (var footDto in coachDto.Footballers)
                {
                    DateTime validStart;
                    bool isStartValid = DateTime.TryParseExact(footDto.ContractStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validStart);

                    DateTime validEnd;
                    bool isEndValid = DateTime.TryParseExact(footDto.ContractEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validEnd);

                    if (!IsValid(footDto) || !isStartValid || !isEndValid || validStart > validEnd)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Footballer footballer = new Footballer()
                    {
                        Name = footDto.Name,
                        ContractStartDate = validStart,
                        ContractEndDate = validEnd,
                        BestSkillType = (BestSkillType)footDto.BestSkillType,
                        PositionType = (PositionType)footDto.PositionType
                    };

                    footballers.Add(footballer);
                }

                coach.Footballers = footballers;
                coaches.Add(coach);
                sb.AppendLine(string.Format(SuccessfullyImportedCoach, coach.Name, footballers.Count));
            }

            context.Coaches.AddRange(coaches);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var teamDtos = JsonConvert.DeserializeObject<ImportTeamDto[]>(jsonString);

            ICollection<Team> teams = new HashSet<Team>();

            foreach (var teamDto in teamDtos)
            {
                if (!IsValid(teamDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Team team = new Team()
                {
                    Name = teamDto.Name,
                    Nationality = teamDto.Nationality,
                    Trophies = teamDto.Trophies,
                };

                int count = 0;

                foreach (var footId in teamDto.Footballers.Distinct())
                {
                    Footballer footballer = context.Footballers.FirstOrDefault(f => f.Id == footId);

                    if (footballer == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    team.TeamsFootballers.Add(new TeamFootballer()
                    {
                        Team = team,    
                        Footballer = footballer
                    });

                    count++;
                }

                teams.Add(team);
                sb.AppendLine(string.Format(SuccessfullyImportedTeam, team.Name, count));
            }

            context.Teams.AddRange(teams);
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
