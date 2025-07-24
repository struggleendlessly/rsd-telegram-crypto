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
using System.Numerics;
using System.Text.RegularExpressions;


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
    try
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
            MK = message.MK ?? 0.0,
            Address = message.Address,
            ChatTitle = message.ChatTitle,
            Network = message.Network,
            isSent = false
        };

        if (entity.Address.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
        {
            if (message.Network.Trim().Contains("base", StringComparison.InvariantCultureIgnoreCase))
            {
                entity.isBase = true;
            }
            else
            {
                entity.isETH = true;
            }
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

        return Results.Ok(entity);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in /data POST: {ex.Message}");

        return Results.Problem("Internal Server Error: " + ex.Message);
    }
});

app.Run();

public class TelegramMessage
{
    public string Name { get; set; } = string.Empty;
    public double? MK { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Network { get; set; } = string.Empty;
    public string ChatTitle { get; set; } = string.Empty;
}

public class TelegramApi
{
    Dictionary<string, string> icons = new();
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

        icons.Add("greenBook", "%F0%9F%93%97");
        icons.Add("redBook", "%F0%9F%93%95");
        icons.Add("lightning", "%E2%9A%A1");
        icons.Add("coin", "%F0%9F%AA%99");
        icons.Add("chart", "%F0%9F%92%B9");
        icons.Add("whiteCircle", "%E2%9A%AA");
        icons.Add("yellowCircle", "%F0%9F%9F%A1");
        icons.Add("orangeCircle", "%F0%9F%9F%A0");
        icons.Add("redCircle", "%F0%9F%94%B4");
        icons.Add("star", "%E2%9C%A8");
        icons.Add("snowflake", "%E2%9C%B3");
        icons.Add("poops", "%F0%9F%92%A9");
        icons.Add("rocket", "%F0%9F%9A%80");
        icons.Add("flagRed", "%F0%9F%9A%A9");
        icons.Add("buy", "%F0%9F%93%88");
        icons.Add("sell", "%F0%9F%93%89");
        icons.Add("squareYellow", "%F0%9F%9F%A8");
        icons.Add("squareOrange", "%F0%9F%9F%A7");
        icons.Add("squareRed", "%F0%9F%9F%A5");
        icons.Add("fire", "%F0%9F%94%A5");
        icons.Add("boom", "%F0%9F%92%A5");
        icons.Add("clockSend", "%E2%8F%B3");
        icons.Add("calendar", "%F0%9F%97%93");
        icons.Add("antenna", "%F0%9F%93%B6");
        icons.Add("SCROLL", "%F0%9F%93%9C");
    }
    public async Task<long> SendSequest(

    messagesEntity telegramMessage)
    {
        var res = 0L;

        var thread_id = telegramMessage switch
        {
            { MK: <= 100_000, isSolana: true } => optionsTelegram.message_thread_id_solana_less100k,
            { MK: <= 100_000, isETH: true } => optionsTelegram.message_thread_id_eth_less100k,
            { MK: <= 100_000, isBase: true } => optionsTelegram.message_thread_id_base_less100k,

            { MK: (> 100_000) and (<= 300_000), isSolana: true } => optionsTelegram.message_thread_id_solana_more100k_less300k,
            { MK: (> 100_000) and (<= 300_000), isETH: true } => optionsTelegram.message_thread_id_eth_more100k_less300k,
            { MK: (> 100_000) and (<= 300_000), isBase: true } => optionsTelegram.message_thread_id_base_more100k_less300k,

            { MK: (> 300_000) and (<= 1_000_000), isSolana: true } => optionsTelegram.message_thread_id_solana_more300k_less1m,
            { MK: (> 300_000) and (<= 1_000_000), isETH: true } => optionsTelegram.message_thread_id_eth_more300k_less1m,
            { MK: (> 300_000) and (<= 1_000_000), isBase: true } => optionsTelegram.message_thread_id_base_more300k_less1m,

            { MK: (> 1_000_000) and (<= 10_000_000), isSolana: true } => optionsTelegram.message_thread_id_solana_more1m_less10m,
            { MK: (> 1_000_000) and (<= 10_000_000), isETH: true } => optionsTelegram.message_thread_id_eth_more1m_less10m,
            { MK: (> 1_000_000) and (<= 10_000_000), isBase: true } => optionsTelegram.message_thread_id_base_more1m_less10m,

            { MK: (> 10_000_000), isSolana: true } => optionsTelegram.message_thread_id_solana_more10m,
            { MK: (> 10_000_000), isETH: true } => optionsTelegram.message_thread_id_eth_more10m,
            { MK: (> 10_000_000), isBase: true } => optionsTelegram.message_thread_id_base_more10m,

            _ => 0.ToString(),
        };

        var bot_hashIndex = rnd.Next(0, optionsTelegram.bot_hash.Count - 1);
        var mk = Regex.Replace(BigInteger.Parse(telegramMessage.MK.ToString()).ToString(), @"(?<=\d)(?=(\d{3})+$)", ".");

        var text =
            $"{icons["lightning"]} {telegramMessage.Name}\n" +
            $"{icons["antenna"]} {mk}\n"+
            $"`{telegramMessage.Address}`\n";

        if (telegramMessage.isSolana)
        {
            text +=
                $"{icons["chart"]} [dextools]({optionsTelegram.dextoolsUrl}app/en/solana/pair-explorer/{telegramMessage.Address}) ";
        }
        else if (telegramMessage.isETH)
        {
            text +=
                $"{icons["chart"]} [dextools]({optionsTelegram.dextoolsUrl}app/en/ether/pair-explorer/{telegramMessage.Address}) ";
        }
        else
        {
            text +=
                $"{icons["chart"]} [dextools]({optionsTelegram.dextoolsUrl}app/en/base/pair-explorer/{telegramMessage.Address}) ";
        }

        string urlString = $"bot{optionsTelegram.bot_hash[bot_hashIndex]}/" +
            $"sendMessage?" +
            $"message_thread_id={thread_id}&" +
            $"chat_id={optionsTelegram.chat_id_coins}&" +
            $"text={text}&" +
            $"parse_mode=MarkDown&" +
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
