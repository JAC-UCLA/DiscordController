using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordController {
    class Program {
        private static Config Config;

        private static Controller Controller;
        private static DiscordSocketClient DiscordClient;

        static async Task Main(string[] args) {
            InitConfig();
            Controller = new Controller(Config.Controller);
            
            await InitDiscord();
            await Task.Delay(-1);

            Controller.Dispose();
        }

        private static void InitConfig() {
            Config = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
            if (Config == null) {
                throw new Exception("Failed to load config.json.");
            }

            Console.WriteLine("Loaded config.");
        }

        private static async Task InitDiscord() {
            DiscordClient = new DiscordSocketClient();
            DiscordClient.MessageReceived += DiscordMessageReceived;
            
            await DiscordClient.LoginAsync(TokenType.Bot, Config.Discord.DiscordToken);
            await DiscordClient.StartAsync();
            
            Console.WriteLine("Connected to Discord.");
        }

        private static async Task DiscordMessageReceived(SocketMessage arg) {
            var msg = arg as SocketUserMessage;
            if (msg == null
                || msg.Author.Id == DiscordClient.CurrentUser.Id
                || msg.Author.IsBot) {
                return;
            }

            if (msg.Channel.Id != Config.Discord.ListenChannelId) {
                Console.WriteLine("Ignoring message from channel \"" + msg.Channel.Id + "\".");
                return;
            }

            Console.WriteLine("Received command: \"" + msg.Content + "\" from \"" + msg.Author.Id + "\".");
            try {
                ProcessCommand(msg);
            } catch (Exception e) {
                Console.WriteLine("Error processing message \"" + msg.Content + "\":");
                Console.WriteLine(e.ToString());
            }
        }

        private static void ProcessCommand(SocketUserMessage msg) {
            if (msg.Author.Id == Config.Discord.AdminUserId) {
                if (msg.Content.Equals("reload", StringComparison.OrdinalIgnoreCase)) {
                    InitConfig();
                    Controller = new Controller(Config.Controller);
                    return;
                } else if (msg.Content.StartsWith("profile ", StringComparison.OrdinalIgnoreCase)) {
                    Controller.SetProfile(msg.Content.Substring(msg.Content.IndexOf(' ') + 1));
                    return;
                }
            }

            Controller.QueueInput(msg.Content);
        }
    }
}
