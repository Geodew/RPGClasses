﻿using System.IO;
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
                int senderPlayerId,
                Mod mod)
        {
            Player player = Main.player[mPlayerId];
            player.statLife += mHealAmount;
        }

        public void HandlePacket(
                BinaryReader reader,
                int senderPlayerId,
                Mod mod)
        {
            Deserialize(
                reader,
                senderPlayerId);
            ServerBroadcast(
                senderPlayerId,
                mod);
            Process(
                senderPlayerId,
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

                newPacket.Write((int)mPacketMessageType);

                newPacket.Write(playerId);
                newPacket.Write(healAmount);

                newPacket.Send();
            }
        }

        private void Deserialize(
                BinaryReader reader,
                int senderPlayerId)
        {
            mPlayerId = reader.ReadInt32();
            mHealAmount = reader.ReadInt32();
        }

        private void ServerBroadcast(
                int senderPlayerId,
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
