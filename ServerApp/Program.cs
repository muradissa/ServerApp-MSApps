using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Models;
using ServerApp.Controller;
using ServerApp;


class Program
{
    static async Task Main(string[] args)
    {
        Task serverThread = Task.Run(() => RunServerThread());
        ValueTask userBatchThread = ScheduleUserBatchThread();
        await Task.WhenAll(serverThread, userBatchThread.AsTask());


        //Task test1Thread = Task.Run(() => Test1());
        //Task test2Thread = Task.Run(() => Test2());
        //await Task.WhenAll(test1Thread,test2Thread);
    }

    static void RunServerThread()
    {
        var connectionString = "server=127.0.0.1;uid=root;password=murad123;port=3306;database=msappdb";
        var userController = new UserController(connectionString);
        var server = new Server("http://localhost:8080/", userController);
        server.Start();
    }

    static async ValueTask ScheduleUserBatchThread()
    {
        // calculate the delay until the next sunday at 9:00 
        TimeSpan delay = CalculateDelayUntilNextSunday();

        // schedule the second thread to run on the next sunday at 9:00 
        Timer secondThreadTimer = new Timer(RunUserBatchThread, null, delay, Timeout.InfiniteTimeSpan);
        /// --  OR  --
        //schedule the second thread to run on the next minute
        //Timer secondThreadTimer = new Timer(RunUserBatchThread, null, TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(5));

        await Task.Delay(-1);
    }

    static void RunUserBatchThread(object state)
    {
        var userBatchService = new UserBatchService();
        userBatchService.Start();  
        ScheduleUserBatchThread().ConfigureAwait(false);
    }

    static TimeSpan CalculateDelayUntilNextSunday()
    {
        DayOfWeek today = DateTime.Now.DayOfWeek;
        int daysUntilNextSunday = ((int)DayOfWeek.Sunday - (int)today + 7) % 7;
        TimeSpan delay = TimeSpan.FromDays(daysUntilNextSunday);
        DateTime nextSundayAtNine = DateTime.Today.AddDays(daysUntilNextSunday).AddHours(9);
        if (nextSundayAtNine < DateTime.Now)
        {
            // If today is sunday and it's already past 9:00 , schedule for next week
            delay += TimeSpan.FromDays(7);
        }
        return delay;
    }


    static async Task Test1()
    {
        string connectionString = "server=127.0.0.1;uid=root;password=murad123;port=3306;database=msappdb";
        UserController userController = new UserController(connectionString);

        try
        {
            // Test adding a new user
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

            // Test retrieving users
            var users = await userController.GetAllUsers();
            Console.WriteLine("\nAll Users:");
            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.ID}, Name: {user.Name}, Email: {user.Email}");
            }

            // Test updating user information
            int userIdToUpdate = newUser.ID; // Use the ID of the previously added user
            User updatedUser = await userController.GetUserByIdAsync(userIdToUpdate);
            updatedUser.Name = "Updated Name";
            updatedUser.Email = "updated.email@example.com";
            await userController.UpdateUserAsync(updatedUser);
            Console.WriteLine("\nUpdated User:");
            Console.WriteLine($"ID: {updatedUser.ID}, Name: {updatedUser.Name}, Email: {updatedUser.Email}");

            // Test deleting a user
            int userIdToDelete = newUser.ID; // Use the ID of the previously added user
            await userController.DeleteUserAsync(userIdToDelete);
            Console.WriteLine($"\nUser with ID {userIdToDelete} deleted.");

            // Test retrieving users again to see if the user was deleted
            users = await userController.GetAllUsers();
            Console.WriteLine("\nRemaining Users:");
            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.ID}, Name: {user.Name}, Email: {user.Email}");
            }

            // Wait for all asynchronous operations to complete before exiting the application
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


    static async Task Test2()
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





/*
using System;
using System.Threading;
using Hangfire;
using Hangfire.MemoryStorage;
using ServerApp;
using ServerApp.Controller;

public class Program
{
    public static void Main(string[] args)
    {    
        var serverUrl = "http://localhost:8080/";
        var connectionString = "server=127.0.0.1;uid=root;password=murad123;port=3306;database=msappdb";

        GlobalConfiguration.Configuration.UseMemoryStorage();

        
        var serverThread = new Thread(() =>
        {
            var server = new Server(serverUrl, new UserController(connectionString));
            server.Start();
        });
        serverThread.Start();

       
        RecurringJob.AddOrUpdate(() => ProcessUsersBatch(connectionString), Cron.Weekly(DayOfWeek.Sunday, 9, 0));
    }

    public static void ProcessUsersBatch(string connectionString)
    {
        Console.WriteLine(" ProcessUsersBatch ");
    }
}
*/
