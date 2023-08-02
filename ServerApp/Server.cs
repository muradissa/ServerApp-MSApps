using Microsoft.AspNetCore.Http;
using ServerApp.Controller;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ServerApp
{
    public class Server
    {
        private readonly string serverUrl;
        private readonly UserController userController;

        public Server(string serverUrl, UserController userController)
        {
            this.serverUrl = serverUrl;
            this.userController = userController;
        }

        public Server(UserController userController)
        {
            this.serverUrl =  "http://localhost:8080/";
            this.userController = userController;
        }
       
        public void Start()
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(serverUrl);
                listener.Start();
                Console.WriteLine($"Server listening at {serverUrl}...");
                while (true)
                {
                    var context = listener.GetContext();
                    Task.Run(() => HandleRequest(context));
                }
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/users")
                {
                    // retrieve a list of users
                    //var users2 = await userController.GetAllUsersAsync();
                    var users = await userController.GetAllUsers();

                    await SendJsonResponse(response, users);
                }
                else if (request.HttpMethod == "GET" && request.Url.AbsolutePath.StartsWith("/user/"))
                {
                    // retrieve a user by ID 
                    var userId = int.Parse(request.Url.Segments[2]);
                    var user = await userController.GetUserByIdAsync(userId);
                    await SendJsonResponse(response, user);
                }
                else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/users")
                {
                    // add a new user 
                    var userJson = await ReadRequestBody(request);
                    var user = JsonSerializer.Deserialize<User>(userJson);
                    var newUser = userController.AddUserAsync(user);
                    var strNewUser = newUser.ToString();
                    await SendJsonResponse(response, "User added successfully.");
                }
                else if (request.HttpMethod == "PUT" && request.Url.AbsolutePath.StartsWith("/user/"))
                {
                    // update a user by ID 
                    var userId = int.Parse(request.Url.Segments[2]);
                    var userJson = await ReadRequestBody(request);
                    var user = JsonSerializer.Deserialize<User>(userJson);
                    user.ID = userId; 
                    userController.UpdateUserAsync(user);
                    await SendJsonResponse(response, "User updated successfully.");
                } 
                else if (request.HttpMethod == "DELETE" && request.Url.AbsolutePath.StartsWith("/user/"))
                {
                    // delete a user by ID 
                    var userId = int.Parse(request.Url.Segments[2]);
                    userController.DeleteUserAsync(userId);
                    await SendJsonResponse(response, "User deleted successfully.");
                }
                else
                {
                    response.StatusCode = 404;
                    await SendJsonResponse(response, "404 - Not found");
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await SendJsonResponse(context.Response, $"Error: {ex.Message}");
            }
            finally
            {
                context.Response.Close();
            }
        }

        private async Task SendJsonResponse(HttpListenerResponse response, object data)
        {
            response.ContentType = "application/json";
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task<T> ReadJsonRequestBody22<T>(HttpRequest request)
        {
            using (var reader = new System.IO.StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(body);
            }
        }

        private async Task<string> ReadRequestBody(HttpListenerRequest request)
        {
            using var reader = new System.IO.StreamReader(request.InputStream);
            return await reader.ReadToEndAsync();
        }
    }
}
