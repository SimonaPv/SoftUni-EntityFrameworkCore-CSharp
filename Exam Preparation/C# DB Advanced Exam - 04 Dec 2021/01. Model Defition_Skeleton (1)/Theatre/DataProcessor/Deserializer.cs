namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var playDtos = Deserialize<ImportPlayDto[]>(xmlString, "Plays");

            ICollection<Play> plays = new HashSet<Play>();

            foreach (var playDto in playDtos)
            {
                bool validEnum = Enum.TryParse<Genre>(playDto.Genre, out Genre validGenre);

                bool validSpan = TimeSpan.TryParseExact(playDto.Duration, "c", CultureInfo.InvariantCulture, TimeSpanStyles.None, out TimeSpan validDuration);
                TimeSpan interval = new TimeSpan(1, 0, 0);

                if (!IsValid(playDto) || !validEnum || !validSpan || validDuration < interval)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Play play = new Play()
                {
                    Title = playDto.Title,
                    Duration = validDuration,
                    Rating = playDto.Rating,
                    Genre = validGenre,
                    Description = playDto.Description,
                    Screenwriter = playDto.Screenwriter
                };

                plays.Add(play);
                sb.AppendLine(string.Format(SuccessfulImportPlay, play.Title, play.Genre, play.Rating));
            }

            context.Plays.AddRange(plays);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var castDtos = Deserialize<ImportCastDto[]>(xmlString, "Casts");

            ICollection<Cast> casts = new HashSet<Cast>();

            foreach (var castDto in castDtos)
            {
                if (!IsValid(castDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Cast cast = new Cast()
                {
                    FullName = castDto.FullName,
                    IsMainCharacter = castDto.IsMainCharacter,
                    PhoneNumber = castDto.PhoneNumber,
                    PlayId = castDto.PlayId
                };

                casts.Add(cast);
                sb.AppendLine(string.Format(SuccessfulImportActor, castDto.FullName, (castDto.IsMainCharacter == true) ? "main" : "lesser"));
            }

            context.Casts.AddRange(casts);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var theatreDtos = JsonConvert.DeserializeObject<ImportTheatDto[]>(jsonString);

            ICollection<Theatre> theatres = new HashSet<Theatre>();

            foreach (var theaDto in theatreDtos)
            {
                if (!IsValid(theaDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Theatre theatre = new Theatre()
                {
                    Name = theaDto.Name,    
                    NumberOfHalls = theaDto.NumberOfHalls,
                    Director = theaDto.Director
                };

                ICollection<Ticket> tickets = new HashSet<Ticket>();

                foreach (var thickDto in theaDto.Tickets)
                {
                    if (!IsValid(thickDto) || thickDto.Price < 1.0m || thickDto.Price > 100.0m)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Ticket ticket = new Ticket()
                    {
                        Price = thickDto.Price,
                        RowNumber = thickDto.RowNumber,
                        PlayId = thickDto.PlayId
                    };

                    tickets.Add(ticket);
                }

                theatre.Tickets = tickets;
                theatres.Add(theatre);
                sb.AppendLine(string.Format(SuccessfulImportTheatre, theaDto.Name, tickets.Count));
            }

            context.Theatres.AddRange(theatres);
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
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
