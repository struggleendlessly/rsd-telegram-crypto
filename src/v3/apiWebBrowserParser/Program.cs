using apiWebBrowserParser.models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<telegramMessagesDB>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/data",
    async (
        [FromBody] TelegramMessage message,
        HttpContext context, 
        telegramMessagesDB db) =>
{
    var ipAddress = context.Connection.RemoteIpAddress?.ToString();
    //add ip check
    var entity = new messagesEntity
    {
        Message = message.Message,
        ChatName = message.ChatName.Trim().ToLower(),
        isSent = false
    };

    db.messagesEntities.Add(entity);
    await db.SaveChangesAsync();

    // add telegram message sending

});


app.Run();

public class TelegramMessage
{
    public string Message { get; set; }
    public string ChatName { get; set; }
}
