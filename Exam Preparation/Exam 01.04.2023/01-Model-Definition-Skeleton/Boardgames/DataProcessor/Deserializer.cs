namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var creatorDtos = Deserialize<ImportCreatorDto[]>(xmlString, "Creators");

            ICollection<Creator> creators = new HashSet<Creator>();

            foreach (var creatDto in creatorDtos)
            {
                if (!IsValid(creatDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Creator creator = new Creator()
                {
                    FirstName = creatDto.FirstName,
                    LastName = creatDto.LastName
                };

                ICollection<Boardgame> boardgames = new HashSet<Boardgame>();

                foreach (var boardDto in creatDto.Boardgames)
                {
                    if (!IsValid(boardDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Boardgame boardgame = new Boardgame()
                    {
                        Name = boardDto.Name,
                        Rating = boardDto.Rating,
                        YearPublished = boardDto.YearPublished,
                        CategoryType = (CategoryType)boardDto.CategoryType,
                        Mechanics = boardDto.Mechanics
                    };

                    boardgames.Add(boardgame);
                }

                creator.Boardgames = boardgames;
                creators.Add(creator);
                sb.AppendLine(string.Format(SuccessfullyImportedCreator, creatDto.FirstName, creatDto.LastName, boardgames.Count));
            }

            context.Creators.AddRange(creators);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var sellerDtos = JsonConvert.DeserializeObject<ImportSellerDto[]>(jsonString);

            ICollection<Seller> sellers = new HashSet<Seller>();

            foreach (var sellDto in sellerDtos)
            {
                if (!IsValid(sellDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Seller seller = new Seller()
                {
                    Name = sellDto.Name,
                    Address = sellDto.Address,
                    Country = sellDto.Country,
                    Website = sellDto.Website
                };

                int count = 0;
                foreach (var boardId in sellDto.Boardgames.Distinct())
                {
                    Boardgame board = context.Boardgames.FirstOrDefault(x => x.Id == boardId);

                    if (board == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    seller.BoardgamesSellers.Add(new BoardgameSeller()
                    {
                        Seller = seller,
                        Boardgame = board
                    });
                    count++;
                }

                sellers.Add(seller);
                sb.AppendLine(string.Format(SuccessfullyImportedSeller, sellDto.Name, count));
            }

            context.Sellers.AddRange(sellers);
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
