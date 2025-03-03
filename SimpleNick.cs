using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using RemoteAdmin;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SimpleNick
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "Kaesekuchen";
        public override string Name => "SimpleNick";
        public override string Prefix => "Nick";
        public override Version Version => new Version(1, 2, 6);

        private static readonly string FilePath = Path.Combine(Exiled.API.Features.Paths.Configs, "blockednick.yml");
        private static Dictionary<string, string> blockedPlayers = new();

        public override void OnEnabled()
        {
            base.OnEnabled();
            LoadBlockedPlayers();
            Log.Info("SimpleNick has been enabled.");
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            SaveBlockedPlayers();
            Log.Info("SimpleNick has been disabled.");
        }

        private static void LoadBlockedPlayers()
        {
            if (!File.Exists(FilePath))
            {
                SaveBlockedPlayers();
                return;
            }

            try
            {
                string yaml = File.ReadAllText(FilePath);
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                blockedPlayers = deserializer.Deserialize<Dictionary<string, string>>(yaml) ?? new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                Log.Error($"Error loading blockednick.yml: {ex.Message}");
                blockedPlayers = new Dictionary<string, string>();
            }
        }

        private static void SaveBlockedPlayers()
        {
            try
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                string yaml = serializer.Serialize(blockedPlayers);
                File.WriteAllText(FilePath, yaml);
            }
            catch (Exception ex)
            {
                Log.Error($"Error saving blockednick.yml: {ex.Message}");
            }
        }

        public static bool IsPlayerBlocked(string userId) => blockedPlayers.ContainsKey(userId);

        public static void BlockPlayer(string userId, string lastNick)
        {
            if (IsPlayerBlocked(userId))
            {
                return;
            }

            blockedPlayers[userId] = lastNick ?? "Unknown";
            SaveBlockedPlayers();
        }

        public static void UnblockPlayer(string userId)
        {
            blockedPlayers.Remove(userId);
            SaveBlockedPlayers();
        }

        public static string GetLastNick(string userId) => blockedPlayers.TryGetValue(userId, out string lastNick) ? lastNick : "Unknown";

        public static string GetBlockedList()
        {
            if (blockedPlayers.Count == 0)
                return "No players are currently blocked.";

            return "Blocked Players:\n" + string.Join("\n", blockedPlayers.Select(kvp => $"{kvp.Value} ({kvp.Key})"));
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class NickCommand : ICommand
    {
        public string Command => "nick";
        public string[] Aliases => new string[] { "nickname" };
        public string Description => "Manage nicknames of players.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            return NickCommandHandler.ExecuteCommand(arguments, sender, out response);
        }
    }

    public static class NickCommandHandler
    {
        public static bool ExecuteCommand(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "[SN] Please enter a valid subcommand:\n" +
                           "  clist -> Show a list of all available commands\n" +
                           "  list -> Show blocked players\n" +
                           "  <name> -> Set your nickname\n" +
                           "  reset -> Reset your nickname\n" +
                           "  {playerid} <name/reset/exclude/include> -> Manage others\n";
                return false;
            }

            if (arguments.At(0).ToLower() == "list")
            {
                response = Plugin.GetBlockedList();
                return true;
            }

            if (sender is not PlayerCommandSender playerSender)
            {
                response = "Only players can use this command.";
                return false;
            }

            var player = Player.Get(playerSender);
            if (player == null)
            {
                response = "Player not found.";
                return false;
            }

            if (arguments.Count == 1)
            {
                string arg = arguments.At(0).ToLower();
                if (arg == "reset")
                {
                    if (Plugin.IsPlayerBlocked(player.UserId))
                    {
                        player.DisplayNickname = Plugin.GetLastNick(player.UserId);
                        response = $"Your nickname has been reset to: {player.DisplayNickname}";
                    }
                    else
                    {
                        player.DisplayNickname = null;
                        response = "Your nickname has been completely reset.";
                    }
                    return true;
                }

                if (Plugin.IsPlayerBlocked(player.UserId))
                {
                    response = $"You are blocked from changing your nickname. Last nickname: {Plugin.GetLastNick(player.UserId)}";
                    return false;
                }

                if (arg.Length > 32)
                {
                    response = "Nickname too long! Max 32 characters.";
                    return false;
                }

                player.DisplayNickname = arg;
                response = $"Your new nickname: {arg}";
                return true;
            }

            response = "Invalid command syntax.";
            return false;
        }
    }
}
