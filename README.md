
# ExpensesTracker
A learning application to track shared expenses between roomies.

## Installing
This application requires ASP.NET 7 and a MySQL-compatible database. We used [MariaDB](https://mariadb.com/), but you are free to use whatever you prefer.

### Database Setup
The database setup is simple. Create a new database and a user and give that user full access to everything in that database. After that, run the table setup script located at `sql/Table Setup.sql`. After that, run all scripts in `sql/Stored Procs`. Once you've completed that, the database is setup.

### Application Setup
The application is just a standard ASP.NET application, so just a simple build and run command will suffice.
```bash
dotnet build
dotnet run
```

### Notes
As of right now, the web application doesn't have a way to add in expenses or payments, so it must all be done from the database. Stay tuned for this feature to be added to the web application.

## Purpose
The purpose of this application is to help @joshuawcampb learn more about databases and web application.

### Technologies
Because this is a learning exercise, we decided to create a simple application using only a few tools at a time.

#### ASP.NET 7
I decided to go with ASP.NET 7 because I personally use .NET 6 at my work right now and wanted to see if there were any noticeable differences between 6 and 7. Also, I was writing the web application and left Josh in charge of the database work, so I wanted to give him an example of not being able to choose the frameworks you're given when you're on the job. I think Node.js would have been a better choice since we're not using Microsoft SQL, but unfortunately, you don't always get to pick your tool set in the work environment.

#### MariaDB
To help Josh learn about database applications, we decided to go with MySQL / MariaDB to get started with databases. While Microsoft SQL would have been a better choice since we used ASP.NET for the web application, I have had to use MariaDB in the past with .NET and don't have an issue using whatever tools are provided.

I provided Josh with three example tables and one example view. All the rest of the tables and queries were written by him. This includes populating the database with data and all the queries written in the web application.
