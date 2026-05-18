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
        //Main con correciones hechas por IA para que funcione en NET 7.3
        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient("8507961896:AAGiZyQat9gpbE-zvnd8vs15HPnuAH9MdGo");

            var cts = new CancellationTokenSource();

            var me = await botClient.GetMe(cts.Token);
            Console.Write("Hola, soy el bot de Telegram: ");
            Console.WriteLine(me.FirstName);

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            Console.WriteLine("Presiona cualquier tecla para salir");
            Console.ReadKey();

            cts.Cancel();
            cts.Dispose();
        }

        //Correciones hechas con IA para que funcione en NET 7.3, ahora cada caso tiene su propia respuesta y el eco genérico solo se activa si no se cumplen los casos anteriores.
        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message == null || update.Message.Text == null)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            Console.WriteLine($"Mensaje recibido: '{messageText}' en el chat {chatId}.");

            // Esta funcion me quito años de vida 
            if (messageText.ToLower().Contains("message"))
            {
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "¡Has mencionado la palabra 'Message'!",
                    cancellationToken: cancellationToken);
            }
            else if (messageText.ToLower().Contains("sticker"))
            {
                // Ahora esta separado por funcion, SendMessage ya no es universal para todo, cada caso tiene su propia respuesta.
                await botClient.SendSticker(
                    chatId: chatId,
                    sticker: InputFile.FromUri("https://tlgrm.eu/_/stickers/697/ba1/697ba160-9c77-3b1a-9d97-86a9ce75ff4d/192/35.webp"),
                    cancellationToken: cancellationToken);
            }
            else if (messageText.ToLower().Contains("contact"))
            {
                await botClient.SendContact(
                    chatId: chatId,
                    phoneNumber: "+1234567890",
                    firstName: "Johny",
                    lastName: "Silverhand",
                    cancellationToken: cancellationToken);
            }
            else
            {
                // No se que hace esto pero sin esto no corre 
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Echo: " + messageText,
                    cancellationToken: cancellationToken);
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error detectado: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}