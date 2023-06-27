namespace Footballers.DataProcessor
{
    using Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {
            var coaches = context.Coaches
                .Where(c => c.Footballers.Any())
                .ToArray()
                .Select(c => new ExportCoachDto()
                {
                    CoachName = c.Name,
                    FootballersCount = c.Footballers.Count(),
                    Footballers = c.Footballers.Select(f => new FootballerDto()
                    {
                        Name = f.Name,
                        Position = f.PositionType.ToString()
                    })
                    .OrderBy(c => c.Name)
                    .ToArray()
                })
                .OrderByDescending(c => c.FootballersCount)
                .ThenBy(c => c.CoachName)
                .ToArray();

            return Serialize<ExportCoachDto[]>(coaches, "Coaches");
        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {
            var footballers = context.Teams
            .Where(t => t.TeamsFootballers.Any(f => f.Footballer.ContractStartDate >= date))
            .ToArray()
            .Select(t => new
            {
                Name = t.Name,
                Footballers = t.TeamsFootballers
                .Where(f => f.Footballer.ContractStartDate >= date)
                .OrderByDescending(f => f.Footballer.ContractEndDate)
                .ThenBy(f => f.Footballer.Name)
                .Select(f => new
                {
                    FootballerName = f.Footballer.Name,
                    ContractStartDate = f.Footballer.ContractStartDate.ToString("d", CultureInfo.InvariantCulture),
                    ContractEndDate = f.Footballer.ContractEndDate.ToString("d", CultureInfo.InvariantCulture),
                    BestSkillType = f.Footballer.BestSkillType.ToString(),
                    PositionType = f.Footballer.PositionType.ToString()
                })
               .ToArray()
            })
            .OrderByDescending(t => t.Footballers.Count())
            .ThenBy(t => t.Name)
            .Take(5)
            .ToArray();

            return JsonConvert.SerializeObject(footballers, Formatting.Indented);
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
