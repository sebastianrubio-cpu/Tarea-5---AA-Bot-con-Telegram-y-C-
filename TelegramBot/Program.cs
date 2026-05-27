using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient("8507961896:AAGiZyQat9gpbE-zvnd8vs15HPnuAH9MdGo");
            var cts = new CancellationTokenSource();

            var me = await botClient.GetMe(cts.Token);
            Console.WriteLine($"Bot iniciado: {me.FirstName}");

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                receiverOptions: new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
                cancellationToken: cts.Token
            );

            Console.WriteLine("Presiona cualquier tecla para salir");
            Console.ReadKey();
            cts.Cancel();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message?.Text == null) return;

            var chatId = update.Message.Chat.Id;
            var text = update.Message.Text.ToLower();

            // Mapa de comandos exactos
            var commands = new Dictionary<string, Func<Task>>
            {
                { "/start", () => botClient.SendMessage(chatId, "Bienvenido al bot de la UISEK. Presiona 1 para más información o prueba nuestras otras funciones: *sticker*, *help* y *contact*", parseMode: ParseMode.Markdown, cancellationToken: cancellationToken) },
                { "1",      () => botClient.SendMessage(chatId, "Información institucional: Somos la UISEK, líderes en formación superior.", cancellationToken: cancellationToken) }
            };

            if (commands.ContainsKey(text))
            {
                await commands[text]();
            }
            else
            {
                await ProcessKeywords(botClient, chatId, text, cancellationToken);
            }
        }

        private static async Task ProcessKeywords(ITelegramBotClient botClient, long chatId, string text, CancellationToken token)
        {
            if (text.Contains("help"))
                await botClient.SendMessage(chatId, "Si deseas asitencia porfavor llamar al +593 099999999, si necesita ayuda medica llamar al 911. ", cancellationToken: token);

            else if (text.Contains("sticker"))
                await botClient.SendSticker(chatId, InputFile.FromUri("https://tlgrm.eu/_/stickers/697/ba1/697ba160-9c77-3b1a-9d97-86a9ce75ff4d/192/35.webp"), cancellationToken: token);

            else if (text.Contains("contact"))
                await botClient.SendContact(chatId, "+1-800-5959", "Johny", "Silverhand", cancellationToken: token);

            else
                await botClient.SendMessage(chatId, "No entendí eso. Escribe /start para ver las opciones.", cancellationToken: token);
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}