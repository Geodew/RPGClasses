using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RPG.PacketMessages
{
    class HealPlayerNetMsg
    {
        private const PacketMessageTypeEnum mPacketMessageType = PacketMessageTypeEnum.HEAL_PLAYER;

        private int mPlayerId;
        private int mHealAmount;


        private void Process(
                int whoAmI,
                Mod mod)
        {
            Player player = Main.player[mPlayerId];
            player.statLife += mHealAmount;
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
                int healAmount)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket newPacket = mod.GetPacket();

                newPacket.Write(playerId);
                newPacket.Write(healAmount);

                newPacket.Send();
            }
        }

        private void Deserialize(
                BinaryReader reader,
                int whoAmI)
        {
            mPlayerId = reader.ReadInt32();
            mHealAmount = reader.ReadInt32();
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
                    mHealAmount);
            }
        }
    }
}
