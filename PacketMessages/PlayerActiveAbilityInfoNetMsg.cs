using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RPG.PacketMessages
{
    class PlayerActiveAbilityInfoNetMsg
    {
        private const PacketMessageTypeEnum mPacketMessageType = PacketMessageTypeEnum.PLAYER_ACTIVE_ABILITY_INFO;

        private int mPlayerId;
        private bool mOverrideSpecialVariable;
        private int mPlayerModSpecialVariable;


        private void Process(
                int whoAmI,
                Mod mod)
        {
            // Don't allow a bug or malicious modder to activate your own Active Ability
            if (mPlayerId != whoAmI)
            {
                RPG typedMod = mod as RPG;
                Player player = Main.player[mPlayerId];
                MPlayer mplayer = player.GetModPlayer<MPlayer>(mod);

                mplayer.special = mPlayerModSpecialVariable;
            }
            else
            {
                //zzz Log error
            }
        }

        public void HandlePacket(
                BinaryReader reader,
                int whoAmI,
                Mod mod)
        {
            Deserialize(
                reader,
                whoAmI);
            ServerBroadcast(
                whoAmI,
                mod);
            Process(
                whoAmI,
                mod);
        }

        public static void SerializeAndSend(
                Mod mod,
                int playerId,
                bool overrideSpecialVariable,
                int playerModSpecialVariable)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket newPacket = mod.GetPacket();

                newPacket.Write((int)mPacketMessageType);

                newPacket.Write(playerId);
                newPacket.Write(overrideSpecialVariable);
                newPacket.Write(playerModSpecialVariable);

                newPacket.Send();
            }
        }

        private void Deserialize(
                BinaryReader reader,
                int whoAmI)
        {
            mPlayerId = reader.ReadInt32();
            mOverrideSpecialVariable = reader.ReadBoolean();
            mPlayerModSpecialVariable = reader.ReadInt32();
        }

        private void ServerBroadcast(
                int whoAmI,
                Mod mod)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                SerializeAndSend(
                    mod,
                    mPlayerId,
                    mOverrideSpecialVariable,
                    mPlayerModSpecialVariable);
            }
        }
    }
}
