namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            var creators = context.Creators
                .ToArray()
                .Where(c => c.Boardgames.Any())
                .Select(c => new ExportCreatorDto()
                {
                    BoardgamesCount = c.Boardgames.Count,
                    CreatorName = $"{c.FirstName} {c.LastName}",
                    Boardgames = c.Boardgames
                                  .ToArray()
                                  .OrderBy(b => b.Name)
                                  .Select(b => new ExportBpardgameDto
                                  {
                                      BoardgameName = b.Name,
                                      BoardgameYearPublished = b.YearPublished
                                  })
                                  .ToArray(),
                })
                .OrderByDescending(c => c.BoardgamesCount)
                .ThenBy(c => c.CreatorName)
                .ToArray();

            return Serialize<ExportCreatorDto[]>(creators, "Creators");
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellers = context.Sellers
                .ToArray()
                .Where(s => s.BoardgamesSellers.Select(b => b.Boardgame.YearPublished >= year && b.Boardgame.Rating <= rating).Any())
                .Select(s => new
                {
                    Name = s.Name,
                    Website = s.Website,
                    Boardgames = s.BoardgamesSellers
                                  .ToArray()
                                  .Where(b => b.Boardgame.YearPublished >= year && b.Boardgame.Rating <= rating)
                                  .OrderByDescending(b => b.Boardgame.Rating)
                                  .ThenBy(b => b.Boardgame.Name)
                                  .Select(b => new
                                  {
                                      Name = b.Boardgame.Name,
                                      Rating = b.Boardgame.Rating,
                                      Mechanics = b.Boardgame.Mechanics,
                                      Category = b.Boardgame.CategoryType.ToString()
                                  })
                                  .ToArray()
                })
                .OrderByDescending(b => b.Boardgames.Count())
                .ThenBy(b => b.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(sellers, Formatting.Indented);
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