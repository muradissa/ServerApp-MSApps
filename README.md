# ServerApp-MSApps


Assignment: Building a Server with Controllers, Models, and Batch Services
Objective:
The objective of this assignment is to create a server application using C# .NET framework,
incorporating controllers, models, and batch services, and integrating it with a MySQL database.
Requirements:
● Set up a MySQL database:
● Install MySQL Server on your machine if you haven't already.
● Create a new database named "MyDatabase" with a table named "Users".
● The "Users" table should have the following columns:
● ID (integer, primary key, auto-increment)
● Name (varchar)
● Email (varchar)
● Password (varchar)
● Create a new C# .NET project in Visual Studio or any other IDE:
● Target framework: .NET 5.0 or later.
● Name the project "ServerApp".
● Implement the model:
● Create a new class named "User" with properties corresponding to the columns in the
"Users" table.
● Use appropriate data types for each property.
● Implement the controller:
● Create a new class named "UserController".
● Implement methods to perform CRUD (Create, Read, Update, Delete) operations on
the "Users" table using the MySQL database.
● You can make use of the MySQL Connector/NET library to interact with the database.
● Include methods for:
● Adding a new user to the database.
● Retrieving a user by ID.
● Updating user information.
● Deleting a user.
● Implement batch services:
● Create a new class named "UserBatchService".
● Implement a method that retrieves a list of users from the database, performs batch
processing on the data (e.g., sending emails to users), and updates the user records
accordingly.
● The batch processing can be any relevant task you choose (e.g., sending a weekly
newsletter to users).
● Schedule the batch service to run at a specific time or interval (e.g., every Sunday at
8:00 PM).
● Set up the server:
● Create a new class named "Server".
● Implement a simple HTTP server using the HttpListener class in C#.
● Set up the server to listen for incoming requests on a specific port (e.g., 8080).
● Handle incoming requests by mapping them to appropriate methods in the
UserController.
● Test the server:
● Write a separate console application to test the server and the UserController
methods.
● Demonstrate the usage of each UserController method, including adding users,
retrieving users, updating user information, and deleting users.
● Print the results to the console to verify the functionality of the server.
● Test the batch service:
● Write a separate console application to test the batch service.
● Run the batch service manually and observe the batch processing task being
executed.
● Print relevant information to the console to demonstrate the successful execution of
the batch service.
