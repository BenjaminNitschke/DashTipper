using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DashTipper
{
	class Program
	{
		static void Main()
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			client = new DiscordSocketClient();
			client.Log += Log;
			client.MessageReceived += MessageReceived;
			string token = "-privatetoken-";
			await client.LoginAsync(TokenType.Bot, token);
			await client.StartAsync();
			// Block this task until the program is closed.
			await Task.Delay(-1);
		}
		private DiscordSocketClient client;

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private static async Task MessageReceived(SocketMessage message)
		{
			if (message.Content.StartsWith("!tip"))
				await HandleTip(message);
		}

		private static async Task HandleTip(SocketMessage message)
		{
			var tipUser = message.MentionedUsers.FirstOrDefault();
			if (tipUser == null || !CryptoCurrencyAmountExtractor.TryExtract(message.Content,
				out decimal amountInMdash))
				await message.Channel.SendMessageAsync(
					"Please specify a username and amount (DASH, mDASH, uDASH, Duff), e.g. " +
					"@TheDesertLynx !tip 1mDASH");
			else
				await message.Channel.SendMessageAsync("TODO: Successfully tipped @" + tipUser.Username +
					" " + amountInMdash + " mDASH");
		}
	}
}