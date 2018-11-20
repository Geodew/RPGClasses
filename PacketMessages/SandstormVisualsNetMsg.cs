using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RPG.PacketMessages
{
    class SandstormVisualsNetMsg
    {
        private const PacketMessageTypeEnum mPacketMessageType = PacketMessageTypeEnum.SANDSTORM_VISUALS;
        private const int mSandstormDustType = 32;

        private int mPlayerId;


        private void Process(
                int whoAmI,
                Mod mod)
        {
            Player player = Main.player[mPlayerId];
            Vector2 spawnPos = new Vector2(player.position.X - 96.0f, player.position.Y + 32.0f);
            int dustIndex = Dust.NewDust(spawnPos, player.width + 192, player.height, mSandstormDustType);
            Vector2 moveToward = new Vector2(spawnPos.X + 96.0f + (float)Math.Cos(Main.rand.Next(6) % 6) * 16.0f, spawnPos.Y - 112.0f);  //zzz should be (specialTimer % 6)
            Main.dust[dustIndex].velocity = moveToward - spawnPos;
            Main.dust[dustIndex].velocity.Normalize();
            Main.dust[dustIndex].velocity *= 8.0f;
            Main.dust[dustIndex].velocity += player.velocity;
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
                int playerId)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket newPacket = mod.GetPacket();

                newPacket.Write(playerId);

                newPacket.Send();
            }
        }

        private void Deserialize(
                BinaryReader reader,
                int whoAmI)
        {
            mPlayerId = reader.ReadInt32();
        }

        private void ServerBroadcast(
                int whoAmI,
                Mod mod)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                SerializeAndSend(
                    mod,
                    mPlayerId);
            }
        }
    }
}
