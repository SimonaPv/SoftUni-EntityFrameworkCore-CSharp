namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var despatcherDtos = Deserialize<ImportDespatchersDto[]>(xmlString, "Despatchers");

            ICollection<Despatcher> despatchers = new HashSet<Despatcher>();

            foreach (var despDto in despatcherDtos)
            {
                if (!IsValid(despDto) || string.IsNullOrEmpty(despDto.Position))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Despatcher despatcher = new Despatcher()
                {
                    Name = despDto.Name,
                    Position = despDto.Position
                };

                ICollection<Truck> trucks = new HashSet<Truck>();
                foreach (var truckDto in despDto.Trucks)
                {
                    if (!IsValid(truckDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Truck truck = new Truck()
                    {
                        RegistrationNumber = truckDto.RegistrationNumber,
                        VinNumber = truckDto.VinNumber,
                        TankCapacity = truckDto.TankCapacity,
                        CargoCapacity = truckDto.CargoCapacity,
                        CategoryType = (CategoryType)truckDto.CategoryType,
                        MakeType = (MakeType)truckDto.MakeType
                    };

                    trucks.Add(truck);
                }

                despatcher.Trucks = trucks;

                despatchers.Add(despatcher);
                sb.AppendLine(string.Format(SuccessfullyImportedDespatcher, despatcher.Name, trucks.Count()));
            }

            context.Despatchers.AddRange(despatchers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var clientDtos = JsonConvert.DeserializeObject<ImportClientDto[]>(jsonString);

            ICollection<Client> clients = new HashSet<Client>();

            foreach (var clientDto in clientDtos)
            {
                if (!IsValid(clientDto) || clientDto.Type == "usual")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Client client = new Client()
                {
                    Name = clientDto.Name,
                    Nationality = clientDto.Nationality,
                    Type = clientDto.Type
                };

                ICollection<Truck> clientsTrucks = new HashSet<Truck>();

                foreach (var truckId in clientDto.Trucks.Distinct())
                {
                    Truck truck = context.Trucks.FirstOrDefault(x => x.Id == truckId);

                    if (truck == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    clientsTrucks.Add(truck);

                    client.ClientsTrucks.Add(new ClientTruck()
                    {
                        Client = client,
                        Truck = truck
                    });


                }

                clients.Add(client);
                sb.AppendLine(string.Format(SuccessfullyImportedClient, client.Name, clientsTrucks.Count()));
            }

            context.Clients.AddRange(clients);
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