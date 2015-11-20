using System;

namespace Oxide.Plugins
{
    [Info("Spiritwalk", "Wulf/lukespragg", "0.2.0", ResourceId = 0)]
    [Description("Leave your body behind, and enter the spirit realm.")]

    class Spiritwalk : RustLegacyPlugin
    {
        // Do NOT edit this file, instead edit Spiritwalk.json in serverdata/oxide/config

        #region Configuration and Setup

        // Messages
        string ChatCommand => GetConfig("ChatCommand", "spirit");
        string NoPermission => GetConfig("NoPermission", "Sorry, you can't use 'spirit' right now");
        string SpiritFree => GetConfig("SpiritFree", "Your spirit has been set free");
        string SpiritReturned => GetConfig("SpiritReturned", "Your spirit has returned to your body");

        protected override void LoadDefaultConfig()
        {
            // Messages
            Config["ChatCommand"] = ChatCommand;
            Config["NoPermission"] = NoPermission;
            Config["SpiritFree"] = SpiritFree;
            Config["SpiritReturned"] = SpiritReturned;

            SaveConfig();
        }

        void Loaded()
        {
            LoadDefaultConfig();

            permission.RegisterPermission("spiritwalk.allowed", this);
            cmd.AddChatCommand(ChatCommand, this, "SpiritChatCmd");
        }

        #endregion

        #region Chat Command

        void SpiritChatCmd(NetUser netuser)
        {
            if (!HasPermission(netuser, "spiritwalk.allowed"))
            {
                rust.SendChatMessage(netuser, NoPermission);
                return;
            }

            var character = rust.GetCharacter(netuser);
            const int hp = -100;

            if (character.takeDamage.maxHealth < 0 || character.takeDamage.health < 0)
            {
                character.takeDamage.maxHealth = 100;
                character.takeDamage.health = 100;

                rust.Notice(netuser, SpiritReturned);

                return;
            }

            character.takeDamage.maxHealth = 100;
            character.takeDamage.health = hp;

            rust.Notice(netuser, SpiritFree);
        }

        #endregion

        #region Helper Methods

        T GetConfig<T>(string name, T defaultValue)
        {
            if (Config[name] == null) return defaultValue;
            return (T)Convert.ChangeType(Config[name], typeof(T));
        }

        bool HasPermission(NetUser netuser, string perm) => permission.UserHasPermission(netuser.displayName, perm);

        #endregion
    }
}