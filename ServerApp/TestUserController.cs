using ServerApp.Controller;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    class TestUserController
    {
        static async Task Main1()
        {
            string connectionString = "server=127.0.0.1;uid=root;password=murad123;port=3306;database=msappdb";
            UserController userController = new UserController(connectionString);

            try
            {
                // test adding a new user
                User newUser = new User
                {
                    Name = "John",
                    Email = "john@example.com",
                    Password = "12345",
                    lastBatch = DateTime.Now
                };

                newUser = await userController.AddUserAsync(newUser);

                Console.WriteLine("Added User:");
                Console.WriteLine($"ID: {newUser.ID}, Name: {newUser.Name}, Email: {newUser.Email}");

                // test retrieving users
                var users = await userController.GetAllUsers();
                Console.WriteLine("\nAll Users:");
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.ID}, Name: {user.Name}, Email: {user.Email}");
                }

                // test updating user information
                int userIdToUpdate = newUser.ID; // Use the ID of the previously added user
                User updatedUser = await userController.GetUserByIdAsync(userIdToUpdate);
                updatedUser.Name = "Updated Name";
                updatedUser.Email = "updated.email@example.com";
                await userController.UpdateUserAsync(updatedUser);
                Console.WriteLine("\nUpdated User:");
                Console.WriteLine($"ID: {updatedUser.ID}, Name: {updatedUser.Name}, Email: {updatedUser.Email}");

                // test deleting a user
                int userIdToDelete = newUser.ID; 
                await userController.DeleteUserAsync(userIdToDelete);
                Console.WriteLine($"\nUser with ID {userIdToDelete} deleted.");

                // test retrieving users again to see if the user was deleted
                users = await userController.GetAllUsers();
                Console.WriteLine("\nRemaining Users:");
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.ID}, Name: {user.Name}, Email: {user.Email}");
                }

                await Task.WhenAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }
        
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
