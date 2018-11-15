using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MyBot
{
    public class Program
    {
        string commandString = ";";
        Dictionary<string, string> callList = new Dictionary<string, string>();

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var client = new DiscordSocketClient();

            client.Log += Log;
            client.MessageReceived += MessageReceived;

            string token = System.IO.File.ReadAllText("token.txt"); 
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if(message.Content.StartsWith(commandString))
            {
                await ParseCommand(message);
            }
        }

        private async Task ParseCommand(SocketMessage message)
        {
            switch (message.Content.Split(' ')[0])
            {
                case ";ping":
                    await message.Channel.SendMessageAsync("Pong!");
                    break;
                case ";version":
                    await message.Channel.SendMessageAsync(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    break;
                case ";callme":
                    await CallMe(message);
                    break;
                case ";myname":
                    await GetName(message);
                    break;
                case ";isbot":
                    await message.Channel.SendMessageAsync("yes");
                    break;
                default:
                    await message.Channel.SendMessageAsync("Invalid command.");
                    break;
            }

        }

        private async Task CallMe(SocketMessage message)
        {
            if (callList.ContainsKey(message.Author.Username))
            {
                callList.Remove(message.Author.Username);
            }
                callList.Add(message.Author.Username, message.Content.Substring(message.Content.IndexOf(' ')));
            await message.Channel.SendMessageAsync(String.Format(@"{0}, I will call you {1}", 
                message.Author.Username, message.Content.Substring(message.Content.IndexOf(' '))));
            }

        private async Task GetName(SocketMessage message)
        {
            if(callList.ContainsKey(message.Author.Username))
            {
                await message.Channel.SendMessageAsync("Your name is " + callList[message.Author.Username] + ".");
            }
            else
            {
                await message.Channel.SendMessageAsync("I don't know your name.");
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}