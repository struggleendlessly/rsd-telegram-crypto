using apiWebBrowserParser.models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Shared;
using Shared.ConfigurationOptions;
using Shared.Telegram.Models;

using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddHttpClient("Api", client =>
{
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var socketHandler = new SocketsHttpHandler
    {
        MaxConnectionsPerServer = int.MaxValue,
        PooledConnectionLifetime = TimeSpan.FromMinutes(15),
    };
    return socketHandler;
})
.SetHandlerLifetime(TimeSpan.FromMinutes(5))
.AddPolicyHandler(PolicyHandlers.GetRetryPolicy());

builder.Services.AddDbContext<telegramMessagesDB>(options => options.UseSqlServer(connectionString));

builder.Services.Configure<OptionsTelegram>(builder.Configuration.GetSection(OptionsTelegram.SectionName));

builder.Services.AddTransient<TelegramApi>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/data",
    async (
        [FromBody] TelegramMessage message,
        HttpContext context, 
        telegramMessagesDB db,
        TelegramApi telegramApi) =>
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
    var t = """
        <a href="https://www.defined.fi/sol/8cVqXcASuS61JhTLQkFdUz5PV5w7BE9RpcjDiGw5662t">SQUIDGAME(DALMATIAN)</a>
        """;
    var msg = await telegramApi.SendSequest(

        "t");

    entity.isSent = true;
    await db.SaveChangesAsync();

    return Results.Ok();
});


app.Run();

public class TelegramMessage
{
    public string Message { get; set; }
    public string ChatName { get; set; }
}

public class TelegramApi
{
    private readonly HttpClient httpClient;
    private readonly OptionsTelegram optionsTelegram;

    Random rnd = new Random();

    public TelegramApi(
        IHttpClientFactory httpClient,
        IOptions<OptionsTelegram> options)
    {
        this.optionsTelegram = options.Value;

        this.httpClient = httpClient.CreateClient("Api");
        this.httpClient.BaseAddress = new Uri(optionsTelegram.UrlBase);

    }
    public async Task<long> SendSequest(

    string text)
    {
        var res = 0L;

        var bot_hashIndex = rnd.Next(0, optionsTelegram.bot_hash.Count - 1);

        string urlString = $"bot{optionsTelegram.bot_hash[bot_hashIndex]}/" +
            $"sendMessage?" +
            $"message_thread_id={optionsTelegram.message_thread_id_webscrapper}&" +
            $"chat_id={optionsTelegram.chat_id_coins}&" +
            $"text={text}&" +
            $"parse_mode=HTML&" +
            $"disable_web_page_preview=true";

        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
        var ee = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, urlString));
        var nn = ee.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<MessageSend>(nn.Result);
        //var response = await httpClient.GetFromJsonAsync<MessageSend>(urlString);
        res = resp.result.message_id;

        await Task.Delay(optionsTelegram.api_delay_forech);

        return res;
    }
}
