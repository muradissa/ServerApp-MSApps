/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Collections.Specialized;
using ServerApp.Controller;
*/
using MySqlConnector;
using ServerApp.Models;


public class UserBatchService 
{
   
    private readonly string connectionString;

    public UserBatchService( )
    {
        connectionString = "server=127.0.0.1;uid=root;password=murad123;port=3306;database=msappdb";
    }

    public void Start()
    {
        this.ProcessUsersBatch();
    }

    private async void ProcessUsersBatch()
    {
        string result1 = "";
        string result2 = "";
        List<User> users = await GetAllUsers();
        foreach (var user in users)
        {
            DateTime currentDate = DateTime.Now;
            // Send email to the user
            result1 = await SendEmailToUser(user);
            Console.WriteLine(result1);
            // Update the user lastBatch
            result2 = await UpdateLastBatchToUser(user, currentDate);
            Console.WriteLine(result2);
        }     
    }

    public async Task<string> SendEmailToUser(User user)
    {
        if (user.Email == null){
            return ($"Error can not send email to {user.Name}");
        }
        else{
            return ($"Sending email to {user.Email}");
        }
    }

    private async Task<List<User>> GetAllUsers()
    {
        List<User> users = new List<User>();
        using MySqlConnection connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        using MySqlCommand command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Users";
        using MySqlDataReader reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            User user = new User
            {
                ID = Convert.ToInt32(reader["ID"]),
                Name = reader["Name"].ToString(),
                Email = reader["Email"].ToString(),
                Password = reader["Password"].ToString(),
                lastBatch = DateTime.Parse(reader["lastBatch"].ToString()),
            };
            users.Add(user);
        }
        return users;
    }

    public async Task<string> UpdateLastBatchToUser(User user, DateTime dateNow)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            using MySqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET lastBatch = @lastBatch WHERE ID = @ID";
            command.Parameters.AddWithValue("@ID", user.ID);
            command.Parameters.AddWithValue("@lastBatch", dateNow);
            await command.ExecuteNonQueryAsync();
            return ($"{user.Name} updated successfully");
        }
        catch (Exception error)
        {
            return "Error can not update";
        }
    }
}






/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using ServerApp.Controller;
using ServerApp.Models;
using System.ComponentModel;
using System.Collections.Specialized;

public class UserBatchService : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly string connectionString;

    public UserBatchService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        connectionString = "server=127.0.0.1;uid=root;password=murad123;port=3306;database=msappdb";
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate(() => ProcessUsersBatch(), Cron.Weekly(DayOfWeek.Wednesday, 11, 16));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async void ProcessUsersBatch()
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var userController = scope.ServiceProvider.GetRequiredService<UserController>();

            List<User> users = await GetAllUsers();

            foreach (var user in users)
            {
                DateTime currentDate = DateTime.Now;
                string formattedDate = currentDate.ToString("yyyy-MM-dd");
                Console.WriteLine(SendEmailToUser(user));
                Console.WriteLine(UpdateLastBatchToUser(user , formattedDate));
            }
        }
    }

    private string SendEmailToUser(User user)
    {
        if (user.Email == null) {
            return ($"Error can not send email to {user.Name}");
        }
        else {
            return ($"Sending email to {user.Email}");
        }
    }

    private async Task<List<User>> GetAllUsers()
    {
        List<User> users = new List<User>();

        using MySqlConnection connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        using MySqlCommand command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Users";
        using MySqlDataReader reader = await command.ExecuteReaderAsync();

        while (reader.Read())
        {
            User user = new User
            {
                ID = Convert.ToInt32(reader["ID"]),
                Name = reader["Name"].ToString(),
                Email = reader["Email"].ToString(),
                Password = reader["Password"].ToString(),
                lastBatch = DateTime.Parse(reader["lastBatch"].ToString()),
            };
            users.Add(user);
        }

        return users;
    }

    private async Task<string> UpdateLastBatchToUser(User user , string dateNow)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            using MySqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET lastBatch = @lastBatch WHERE ID = @ID";
            command.Parameters.AddWithValue("@ID", user.ID);
            command.Parameters.AddWithValue("@lastBatch", dateNow);
            await command.ExecuteNonQueryAsync();
            return ($"{user.Name} updated successfully");
        }
        catch (Exception err)
        {
            return "Error can not update";
        }   
    }
}
*/