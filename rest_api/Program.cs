using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using rest_api;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

/*
 * Gets all records from tasks table
 */
app.MapGet("/getAll", async (IConfiguration config) =>
{
    var connStr = config.GetConnectionString("DefaultConnection");

    using var conn = new MySqlConnection(connStr);
    await conn.OpenAsync();

    var cmd = new MySqlCommand("SELECT * FROM Tasks", conn);
    using var reader = await cmd.ExecuteReaderAsync();

    var items = new List<rest_api.Task>();
    while (await reader.ReadAsync())
    {
        if(!reader.IsDBNull(3))
        {
            items.Add(new rest_api.Task { Id = reader.GetInt32(0), expiry = reader.GetDateTime(1), title = reader.GetString(2), description = reader.GetString(3), completePercentage = reader.GetInt32(4) });
        }
        else
        {
            items.Add(new rest_api.Task { Id = reader.GetInt32(0), expiry = reader.GetDateTime(1), title = reader.GetString(2), completePercentage = reader.GetInt32(4) });

        }
    }

    return Results.Ok(items);
});

/*
 * Gets single record that matches id or throws 404
 */
app.MapGet("/getById/{id}", async (int id, IConfiguration config) =>
{
    var connStr = config.GetConnectionString("DefaultConnection");
    using var conn = new MySqlConnection(connStr);
    await conn.OpenAsync();

    var cmd = new MySqlCommand("SELECT * FROM tasks WHERE ID = @id", conn);
    cmd.Parameters.AddWithValue("@id", id);

    using var reader = await cmd.ExecuteReaderAsync();
    var task = new rest_api.Task { };
    if (await reader.ReadAsync())
    {
        if (!reader.IsDBNull(3))
        {
            task = new rest_api.Task { Id = reader.GetInt32(0), expiry = reader.GetDateTime(1), title = reader.GetString(2), description = reader.GetString(3), completePercentage = reader.GetInt32(4) };
        }
        else
        {
            task = new rest_api.Task { Id = reader.GetInt32(0), expiry = reader.GetDateTime(1), title = reader.GetString(2), completePercentage = reader.GetInt32(4) };

        }

        return Results.Ok(task);
    }

    return Results.NotFound();
});
/*
 * Gets incoming tasks based on the option. Throws 400 if option is invalid
 * 
 * options:
    0 - today
    1 - next day
    2 - current week
*/
app.MapGet("/getIncoming/{option}", async (int option, IConfiguration config) =>
{
    

    if(option == 0 || option == 1 || option == 2)
    {
        var connStr = config.GetConnectionString("DefaultConnection");
        using var conn = new MySqlConnection(connStr);
        await conn.OpenAsync();
        var cmd = new MySqlCommand();

        if (option == 0)
        {
            var today = DateTime.Today;
            cmd = new MySqlCommand("SELECT * FROM Tasks WHERE expiry >= @start AND expiry < @end", conn);
            cmd.Parameters.AddWithValue("@start", today);
            cmd.Parameters.AddWithValue("@end", today.AddDays(1));
        }
        else if (option == 1)
        {
            var tomorrow = DateTime.Today.AddDays(1);
            var dayAfterTomorrow = tomorrow.AddDays(1);
            cmd = new MySqlCommand("SELECT * FROM Tasks WHERE expiry >= @tomorrow AND expiry < @dayAfterTomorrow", conn);
            cmd.Parameters.AddWithValue("@tomorrow", tomorrow);
            cmd.Parameters.AddWithValue("@dayAfterTomorrow", dayAfterTomorrow);
        }
        else if (option == 2)
        {
            var today = DateTime.Today;
            int difference = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var monday = today.AddDays(-1 * difference).Date;
            var sunday = monday.AddDays(7);
            cmd = new MySqlCommand("SELECT * FROM Tasks WHERE expiry >= @monday AND expiry < @sunday", conn);
            cmd.Parameters.AddWithValue("@monday", monday);
            cmd.Parameters.AddWithValue("@sunday", sunday);
        }

        using var reader = await cmd.ExecuteReaderAsync();

        var items = new List<rest_api.Task>();
        while (await reader.ReadAsync())
        {
            if (!reader.IsDBNull(3))
            {
                items.Add(new rest_api.Task { Id = reader.GetInt32(0), expiry = reader.GetDateTime(1), title = reader.GetString(2), description = reader.GetString(3), completePercentage = reader.GetInt32(4) });
            }
            else
            {
                items.Add(new rest_api.Task { Id = reader.GetInt32(0), expiry = reader.GetDateTime(1), title = reader.GetString(2), completePercentage = reader.GetInt32(4) });

            }
        }

        return Results.Ok(items);

    }
    return Results.StatusCode(400);



});

/*
 * Adds task to db by body (json). If data not valid throws 400.
 * Template of body:
 * {
    "expiry": "2025-04-24T12:23:47.603Z",
    "title": "string",
    "description": "string",
    "completePercentage": 0
    }
 */
app.MapPost("/addTask", async (rest_api.Task task, IConfiguration config) =>
{
    bool isDateTimeValid = false;
    bool isTaskDescValid = false;
    if (DateTime.TryParse(task.expiry.ToString(), out var result))
    {
        Results.Ok(isDateTimeValid = true);
    }
    if(task.description != null)
    {
        if(task.description.Length <= 100)
        {
            isTaskDescValid = true;
        }
    } else
    {
        isTaskDescValid= true;
    }
    if (isDateTimeValid && task.title != null && task.title.Length < 31 && isTaskDescValid && task.completePercentage >= 0 && task.completePercentage <= 100)
    {
        var connStr = config.GetConnectionString("DefaultConnection");
        using var conn = new MySqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = new MySqlCommand(@"INSERT INTO Tasks (expiry, title, description, completePercentage)
        VALUES (@expiry, @title, @description, @completePercentage)", conn);

        cmd.Parameters.AddWithValue("@expiry", task.expiry);
        cmd.Parameters.AddWithValue("@title", task.title);
        cmd.Parameters.AddWithValue("@description", task.description);
        cmd.Parameters.AddWithValue("@completePercentage", task.completePercentage);

        await cmd.ExecuteNonQueryAsync();

        // get and return created task's id

        var getId = new MySqlCommand("SELECT LAST_INSERT_ID()", conn);
        var getIdResult = await getId.ExecuteScalarAsync();
        task.Id = Convert.ToInt32(getIdResult);

        return Results.Created($"/task/{task.Id}", task);
    }

    return Results.StatusCode(400);

});

/*
 * Updates task with matching id. Same way as adding task but with additional id parameter
 */
app.MapPut("/updateTask/{id}", async (int id, rest_api.Task updatedTask, IConfiguration config) =>
{
    bool isDateTimeValid = false;
    bool isTaskDescValid = false;

    if (DateTime.TryParse(updatedTask.expiry.ToString(), out var result))
    {
        Results.Ok(isDateTimeValid = true);
    }
    if (updatedTask.description != null)
    {
        if (updatedTask.description.Length <= 100)
        {
            isTaskDescValid = true;
        }
    }
    if (isDateTimeValid && updatedTask.title != null && updatedTask.title.Length < 31 && isTaskDescValid && updatedTask.completePercentage >= 0 && updatedTask.completePercentage <= 100)
    {
        var connStr = config.GetConnectionString("DefaultConnection");
        using var conn = new MySqlConnection(connStr);
        await conn.OpenAsync();
        var cmd = new MySqlCommand(@"UPDATE Tasks SET expiry = @expiry, title = @title, description = @description, completePercentage = @completePercentage 
        WHERE Id = @id", conn);

        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@expiry", updatedTask.expiry);
        cmd.Parameters.AddWithValue("@title", updatedTask.title);
        cmd.Parameters.AddWithValue("@description", updatedTask.description);
        cmd.Parameters.AddWithValue("@completePercentage", updatedTask.completePercentage);

        int affectedRows = await cmd.ExecuteNonQueryAsync();

        if (affectedRows == 0)
            return Results.NotFound();

        return Results.Ok(updatedTask);
    }
    return Results.StatusCode(400);
});

/*
 * sets percentage complete for task with matching id. Throws 400 if out of bounds or invalid data
 */
app.MapPut("/setPercentageComlpete/{id}", async (int id, int percentageComplete, IConfiguration config) =>
{

    if (percentageComplete >= 0 && percentageComplete <= 100)
    {
        var connStr = config.GetConnectionString("DefaultConnection");
        using var conn = new MySqlConnection(connStr);
        await conn.OpenAsync();
        var cmd = new MySqlCommand(@"UPDATE Tasks SET completePercentage = @percentageComplete 
        WHERE Id = @id", conn);

        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@percentageComplete", percentageComplete);

        int affectedRows = await cmd.ExecuteNonQueryAsync();

        if (affectedRows == 0)
            return Results.NotFound();

        return Results.Ok(id);
    }
    return Results.StatusCode(400);
});

/*
 * Sets records percenage complete to 100 for task with matching id. 404 if not found id
 */
app.MapPut("/setAsDone/{id}", async (int id, IConfiguration config) =>
{
    var connStr = config.GetConnectionString("DefaultConnection");
    using var conn = new MySqlConnection(connStr);
    await conn.OpenAsync();
    var cmd = new MySqlCommand(@"UPDATE Tasks SET completePercentage = 100 WHERE Id = @id", conn);
    
    cmd.Parameters.AddWithValue("@id", id);

    int affectedRows = await cmd.ExecuteNonQueryAsync();

    if (affectedRows == 0)
        return Results.NotFound();

    return Results.Ok(id);
});

/*
 * Delets task with matching id. 404 if not found id
 */
app.MapDelete("/deleteTask/{id}", async (int id, IConfiguration config) =>
{
    var connStr = config.GetConnectionString("DefaultConnection");
    using var conn = new MySqlConnection(connStr);
    await conn.OpenAsync();
    var cmd = new MySqlCommand(@"DELETE FROM Tasks WHERE Id = @id", conn);

    cmd.Parameters.AddWithValue("@id", id);

    int affectedRows = await cmd.ExecuteNonQueryAsync();

    if (affectedRows == 0)
        return Results.NotFound();

    return Results.Ok(id);
});

app.Run();


/*
 * Making it public for tests
 */
public partial class Program { }