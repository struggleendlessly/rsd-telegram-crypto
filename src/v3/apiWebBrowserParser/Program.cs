using apiWebBrowserParser.models;
using apiWebBrowserParser.options;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Polly;

using Shared;
using Shared.ConfigurationOptions;
using Shared.Telegram.Models;

using System.Net;


var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
                          policy =>
                          {
                              policy.WithOrigins("https://web.telegram.org")
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod();
                          });
});
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
builder.Services.Configure<optionsIPs>(builder.Configuration.GetSection(optionsIPs.SectionName));

builder.Services.AddTransient<TelegramApi>();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/", () => "Hello World!");
app.MapGet("/myip", (HttpContext context) => context.Connection.RemoteIpAddress?.ToString());
app.MapPost("/data",
async (
        [FromBody] TelegramMessage message,
        HttpContext context,
        telegramMessagesDB db,
        TelegramApi telegramApi,
        IOptions<optionsIPs> IoptionsIPs) =>
{
    var allowedIPs = IoptionsIPs.Value.ips;
    var ipAddress = context.Connection.RemoteIpAddress?.ToString();
   
    if (!allowedIPs.Contains(ipAddress))
    {
        return Results.Unauthorized();
    }

    var entity = new messagesEntity
    {
        Name = message.Name.Trim().ToLower(),
        MK = message.MK,
        Address = message.Address,

        isSent = false
    };

    if (entity.Address.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
    {
        entity.isETH = true;
    }
    else
    {
        entity.isSolana = true;
    }

    db.messagesEntities.Add(entity);
    await db.SaveChangesAsync();

    var msg = await telegramApi.SendSequest(entity);

    entity.isSent = true;
    await db.SaveChangesAsync();

    return Results.Ok();
});


app.Run();

public class TelegramMessage
{
    public string Name { get; set; } = string.Empty;
    public double MK { get; set; }
    public string Address { get; set; } = string.Empty;
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

    messagesEntity telegramMessage)
    {
        var res = 0L;

        var thread_id = telegramMessage switch
        {
            { MK: <= 100_000, isSolana: true } => optionsTelegram.message_thread_id_solana_less100k,
            { MK: <= 100_000, isETH: true } => optionsTelegram.message_thread_id_eth_less100k,

            { MK: (> 100_000) and (<= 300_000), isSolana: true } => optionsTelegram.message_thread_id_solana_more100k_less300k,
            { MK: (> 100_000) and (<= 300_000), isETH: true } => optionsTelegram.message_thread_id_eth_more100k_less300k,

            { MK: (> 300_000) and (<= 1_000_000), isSolana: true } => optionsTelegram.message_thread_id_solana_more300k_less1m,
            { MK: (> 300_000) and (<= 1_000_000), isETH: true } => optionsTelegram.message_thread_id_eth_more300k_less1m,

            { MK: (> 1_000_000) and (<= 10_000_000), isSolana: true } => optionsTelegram.message_thread_id_solana_more1m_less10m,
            { MK: (> 1_000_000) and (<= 10_000_000), isETH: true } => optionsTelegram.message_thread_id_eth_more1m_less10m,

            _ => 0.ToString(),
        };

        var bot_hashIndex = rnd.Next(0, optionsTelegram.bot_hash.Count - 1);

        var text =
            $"Name: {telegramMessage.Name}\n" +
            $"MK: {telegramMessage.MK}\n" +
            $"Address: {telegramMessage.Address}";

        string urlString = $"bot{optionsTelegram.bot_hash[bot_hashIndex]}/" +
            $"sendMessage?" +
            $"message_thread_id={thread_id}&" +
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
