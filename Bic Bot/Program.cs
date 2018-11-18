using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;

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
                case ";gocommit":
                    await Commit(message);
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
                callList.Add(message.Author.Username, message.Content.Substring(message.Content.IndexOf(' ') + 1));
            await message.Channel.SendMessageAsync(String.Format(@"{0}, I will call you {1}", 
                message.Author.Username, message.Content.Substring(message.Content.IndexOf(' ') + 1)));
            }

        private async Task GetName(SocketMessage message)
        {
            LogSeverity low = new LogSeverity();
            LogMessage namemsg = new LogMessage(low, MethodBase.GetCurrentMethod().Name, message.ToString());
            await Log(namemsg);
            if(callList.ContainsKey(message.Author.Username))
            {
                await message.Channel.SendMessageAsync("Your name is " + callList[message.Author.Username] + ".");
            }
            else
            {
                await message.Channel.SendMessageAsync("I don't know your name.");
            }
        }

        private async Task Commit(SocketMessage message)
        {
            LogSeverity low = new LogSeverity();
            LogMessage commitMsg = new LogMessage(low, MethodBase.GetCurrentMethod().Name, message.ToString());
            await Log(commitMsg);

            try
            {
                string[] lines = File.ReadAllLines("command text\\commit.txt");
                Random rnd = new Random();
                await message.Channel.SendMessageAsync(lines[rnd.Next(lines.Length)]);
            }
            finally { string[] lines = new string[0]; }
            

        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}