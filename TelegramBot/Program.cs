using System;
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
            if (update.Message == null || update.Message.Text == null) return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text.ToLower();

            // 1. Comando de Bienvenida /start
            if (messageText == "/start")
            {
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Bienvenido al bot de la UISEK. Presiona 1 para más información o prueba nuestras otras funciones: *sticker*, *message* y *contact*",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: cancellationToken);
            }
            // 2. Opción 1 (Información)
            else if (messageText == "1")
            {
                await botClient.SendMessage(chatId, "Información institucional: Somos la UISEK, líderes en formación superior.", cancellationToken: cancellationToken);
            }
            // 3. Otras funciones
            else if (messageText.Contains("message"))
            {
                await botClient.SendMessage(chatId, "¡Has mencionado la palabra 'Message'!", cancellationToken: cancellationToken);
            }
            else if (messageText.Contains("sticker"))
            {
                await botClient.SendSticker(chatId, InputFile.FromUri("https://tlgrm.eu/_/stickers/697/ba1/697ba160-9c77-3b1a-9d97-86a9ce75ff4d/192/35.webp"), cancellationToken: cancellationToken);
            }
            else if (messageText.Contains("contact"))
            {
                await botClient.SendContact(chatId, "+593999999999", "Secretaria", "UISEK", cancellationToken: cancellationToken);
            }
            // 4. Echo genérico
            else
            {
                await botClient.SendMessage(chatId, "No entendí eso. Escribe /start para ver las opciones.", cancellationToken: cancellationToken);
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}