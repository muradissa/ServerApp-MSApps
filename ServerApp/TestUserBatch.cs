using ServerApp.Controller;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    class TestUserBatch
    {
        static async Task Main2()
        {
            string connectionString = "server=127.0.0.1;uid=root;password=murad123;port=3306;database=msappdb";
            UserController userController = new UserController(connectionString);

            try
            {
                string res1 = "";
                string res2 = "";
                var userBatchService = new UserBatchService();
                User newUser = new User
                {
                    Name = "Smith",
                    Email = "smith@example.com",
                    Password = "12345",
                    lastBatch = new DateTime(2023, 8, 1)
                };
                // add user
                newUser = await userController.AddUserAsync(newUser);
                Console.WriteLine($"ID: {newUser.ID}, Name: {newUser.Name}, Email: {newUser.Email}");

                //send email
                userBatchService.SendEmailToUser(newUser);
                res1 = await userBatchService.SendEmailToUser(newUser);
                Console.WriteLine(res1);

                // Update the lastBatch
                res2 = await userBatchService.UpdateLastBatchToUser(newUser, DateTime.Now);

                User user2 = await userController.GetUserByIdAsync(newUser.ID);
                Console.WriteLine($"lastBatch before update  ${newUser.lastBatch}");
                Console.WriteLine($"lastBatch after -update  ${user2.lastBatch}");

                // Wait for all async
                await Task.WhenAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }

            // Wait for user input before exiting the console application
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
