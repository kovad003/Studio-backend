using Domain;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context, 
            UserManager<User> userManager)
        {
            Console.WriteLine("Seeding DB...");
            if (!userManager.Users.Any())
            {
                Console.WriteLine("Seeding Users table...");

                var users = new List<User>
                {
                    new User
                    {
                        UserName = "admin",
                        FirstName = "Admin",
                        LastName = "Admin",
                        Bio = "About me...",
                        Company = "Studio",
                        Avatar = "Avatar comes here...",
                        Email = "admin@studio.com",
                        PhoneNumber = "+35857312752"
                    },
                    new User
                    {
                        UserName = "dave",
                        FirstName = "Dave",
                        LastName = "Vissers",
                        Bio = "About me...",
                        Company = "Studio",
                        Avatar = "Avatar comes here...",
                        Email = "dave@studio.com",
                        PhoneNumber = "+358587896754"
                    },
                    new User
                    {
                        UserName = "ricky",
                        FirstName = "Ricky",
                        LastName = "Spanish",
                        Bio = "About me...",
                        Company = "The Spashion Company",
                        Avatar = "Avatar comes here...",
                        Email = "ricky@spanish.com",
                        PhoneNumber = "+35805683751"
                    },
                    new User
                    {
                        UserName = "taina",
                        FirstName = "Taina",
                        LastName = "van Hauthem ",
                        Bio = "About me...",
                        Company = "Salandro",
                        Avatar = "Avatar comes here...",
                        Email = "taina@client.com",
                        PhoneNumber = "+35805899756"
                    }
                };

                foreach (var user in users)
                {
                    var result = await userManager.CreateAsync(user, "Pa$$w0rd");
                    Console.WriteLine("Inserting user resulted as:" + result);
                }
            }
            
            if (context.Projects.Any()) return;
            Console.WriteLine("Seeding Projects table...");
            var activities = new List<Project>
            {
                new Project
                {
                    Title = "Elastic dry fit T-shirt",
                    IsActive = false,
                    CreatedOn = DateTime.UtcNow.AddMonths(-2),
                    CompletedOn = DateTime.UtcNow.AddMonths(-1),
                    Description = "Will be sold for cross fitters. We have 3 different colors, " +
                                  "so we need 3 different models for the photos. We need a bunch of photos taken" +
                                  "outdoors. Also take some images inside (studio environment)",
                    Image = "Images will be uploaded here",
                    Attachment = "Attachments will be uploaded here",
                    Client = "Bruce Fashion Corporation",
                },
                new Project
                {
                    Title = "Running shoe for joggers",
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow.AddMonths(-1),
                    // CompletedOn = DateTime.UtcNow.AddMonths(-1),
                    Description = "Will be sold for marathon runners and joggers. We have 5 different colors. " +
                                  "We need 2 different models for the photos. Photos must be taken next to " +
                                  "a river bank.",
                    Image = "Images will be uploaded here",
                    Attachment = "Attachments will be uploaded here",
                    Client = "Rabbit Run",
                },
                new Project
                {
                    Title = "Soccer ball",
                    CreatedOn = DateTime.UtcNow.AddMonths(-3),
                    CompletedOn = DateTime.UtcNow.AddMonths(-1),
                    Description = "The soccer ball will be used to advertise the upcoming football league. " +
                                  "The atmosphere of the photos must be bright. The photo model should" +
                                  "wear red soccer jersey (it will be sent with the package).",
                    Image = "Images will be uploaded here",
                    Attachment = "Attachments will be uploaded here",
                    Client = "Voetbal-Bond",
                }
            };

            await context.Projects.AddRangeAsync(activities);
            await context.SaveChangesAsync();
        }
    }
}