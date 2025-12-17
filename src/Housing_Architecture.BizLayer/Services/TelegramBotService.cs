using Housing_Architecture.DataAccess.Persistence;
using Housing_Architecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Housing_Architecture.BizLayer.Services;

public class TelegramBotService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotService> _logger;

    public TelegramBotService(
        ITelegramBotClient botClient,
        IServiceScopeFactory scopeFactory,
        ILogger<TelegramBotService> logger)
    {
        _botClient = botClient;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _botClient.DeleteWebhookAsync(cancellationToken: stoppingToken);

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = null
            };

            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                stoppingToken);

            await Task.Delay(-1, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Telegram bot service is stopping.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting Telegram bot");
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Type != UpdateType.Message)
            return;

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var message = update.Message!;
        var chatId = message.Chat.Id;

        if (message.Text == "/start")
        {
            var button = new KeyboardButton("Telefon raqamni yuborish") { RequestContact = true };
            var keyboard = new ReplyKeyboardMarkup(button)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true,
            };

            await bot.SendMessage(
                chatId,
                "Assalomu alaykum! Ro'yxatdan o'tish uchun telefon raqamingizni yuboring.",
                replyMarkup: keyboard,
                cancellationToken: ct);
        }
        else if (message.Contact != null)
        {
            var phone = message.Contact.PhoneNumber;
            if (!phone.StartsWith("+"))
                phone = "+" + phone;

            _logger.LogInformation("User sent phone number: {Phone}", phone);

            var numberExists = await dbContext.Users
                .AnyAsync(x => x.Phone == phone || x.Phone.Contains(phone) || phone.Contains(x.Phone), ct);

            if (numberExists)
            {
                await bot.SendMessage(chatId, "Bu telefon raqam allaqachon ro'yxatdan o'tgan!", cancellationToken: ct);
                return;
            }

            var otpCode = new Random().Next(10000, 99999).ToString();

            var tempUser = await dbContext.TempUsers.FirstOrDefaultAsync(x => x.PhoneNumber == phone, ct);
            if (tempUser == null)
            {
                tempUser = new TempUser
                {
                    PhoneNumber = phone,
                    OtpCode = otpCode,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    TelegramUserId = message.Contact.UserId?.ToString()
                };
                dbContext.TempUsers.Add(tempUser);
            }
            else
            {
                tempUser.OtpCode = otpCode;
                tempUser.ExpiresAt = DateTime.UtcNow.AddMinutes(5);
                tempUser.TelegramUserId = message.Contact.UserId?.ToString();
                dbContext.TempUsers.Update(tempUser);
            }

            await dbContext.SaveChangesAsync(ct);

            await bot.SendMessage(
                chatId,
                $"Sizning bir martalik kodingiz: <b>{otpCode}</b>\n\nKod 5 daqiqa davomida amal qiladi.",
                parseMode: ParseMode.Html,
                cancellationToken: ct);
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
    {
        _logger.LogError(exception, "Telegram bot error");
        return Task.CompletedTask;
    }
}
