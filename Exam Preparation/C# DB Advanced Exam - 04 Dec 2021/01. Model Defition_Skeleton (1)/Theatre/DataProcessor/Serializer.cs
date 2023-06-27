namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var teatres = context.Theatres
                .ToArray()
                .Where(h => h.NumberOfHalls >= numbersOfHalls && h.Tickets.Count() >= 20)
                .Select(h => new
                {
                    Name = h.Name,
                    Halls = h.NumberOfHalls,
                    TotalIncome = h.Tickets
                                   .Where(t => t.RowNumber >= 1 && t.RowNumber <= 5)
                                   .Sum(t => t.Price),
                    Tickets = h.Tickets
                               .Where(t => t.RowNumber >= 1 && t.RowNumber <= 5)
                               .Select(t => new
                               {
                                   Price = decimal.Parse($"{t.Price:f2}"),
                                   RowNumber = t.RowNumber
                               })
                               .OrderByDescending(t => t.Price)
                               .ToArray()
                })
                .OrderByDescending(h => h.Halls)
                .ThenBy(h => h.Name)
                .ToArray();

            return JsonConvert.SerializeObject(teatres, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double raiting)
        {
            var plays = context.Plays
                .ToArray()
                .Where(p => p.Rating <= raiting)
                .Select(p => new ExportPlayDto()
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString("c", CultureInfo.InvariantCulture),
                    Rating = (p.Rating == 0 ? "Premier" : p.Rating.ToString()),
                    Genre = p.Genre.ToString(),
                    Actors = p.Casts
                              .Where(a => a.IsMainCharacter)
                              .Select(a => new ActorDto()
                              {
                                  FullName = a.FullName,
                                  MainCharacter = $"Plays main character in '{p.Title}'."
                              })
                              .OrderByDescending(a => a.FullName)
                              .ToArray()
                })
                .OrderBy(p => p.Title)
                .ThenByDescending(p => p.Genre)
                .ToArray();

            return Serialize<ExportPlayDto[]>(plays, "Plays");
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
