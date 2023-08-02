
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using MySqlConnector;
using ServerApp.Models;


namespace ServerApp.Controller
{
    public class UserController
    {
        private readonly string connectionString;

        public UserController(string connectionString)
        {
            this.connectionString = connectionString;
        }


        // method to get all users
        public async Task<Object> GetAllUsersAsync()
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


        public async Task<List<User>> GetAllUsers()
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


        // method to add a new user to the database
        public async Task<User> AddUserAsync(User user)
        {     
            if (user == null)
                return null;
            using MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            using MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Users (Name, Email, Password, lastBatch) VALUES (@Name, @Email, @Password, @lastBatch); SELECT LAST_INSERT_ID();";
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@lastBatch", user.lastBatch);
            await command.ExecuteNonQueryAsync();
            user.ID =  Convert.ToInt32(command.LastInsertedId);
            return user;
        }


        // method to retrieve a user by ID
        public async Task<User> GetUserByIdAsync(int userId)
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            using MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", userId);
            using MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync())
            {
                return new User
                {
                    ID = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Password = reader["Password"].ToString(),
                    lastBatch = DateTime.Parse(reader["lastBatch"].ToString()),
                };
            }
            return null; // user not found
        }


        // method to update user information
        public async Task UpdateUserAsync(User user)
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            using MySqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET Name = @Name, Email = @Email, Password = @Password, lastBatch = @lastBatch WHERE ID = @ID";
            command.Parameters.AddWithValue("@ID", user.ID);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@lastBatch", user.lastBatch);
            await command.ExecuteNonQueryAsync();
        }


        // Method to delete a user
        public async Task DeleteUserAsync(int userId)
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            using MySqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE ID = @ID";
            command.Parameters.AddWithValue("@ID", userId);
            await command.ExecuteNonQueryAsync();
        }
    }
}