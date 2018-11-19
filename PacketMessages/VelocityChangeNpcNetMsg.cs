﻿using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace RPG.PacketMessages
{
    class VelocityChangeNpcNetMsg
    {
        private const PacketMessageTypeEnum mPacketMessageType = PacketMessageTypeEnum.VELOCITY_CHANGE_NPC;

        private float mNewVelocityX;
        private float mNewVelocityY;
        private int mNpcIndex;


        private void Process(
                int whoAmI,
                Mod mod)
        {
            Vector2 vel = new Vector2(mNewVelocityX, mNewVelocityY);
            NPC npc = Main.npc[mNpcIndex];
            npc.velocity = vel;
        }

        public void HandlePacket(
                BinaryReader reader,
                int whoAmI,
                Mod mod)
        {
            Deserialize(
                reader,
                whoAmI);
            Process(
                whoAmI,
                mod);
        }

        public static void SerializeAndSend(
                Mod mod,
                Vector2 newVelocity,
                int npcIndex)
        {
            SerializeAndSend(
                mod,
                newVelocity.X,
                newVelocity.Y,
                npcIndex);
        }

        public static void SerializeAndSend(
                Mod mod,
                float newVelocityX,
                float newVelocityY,
                int npcIndex)
        {
            if (Main.netMode != 0)  // Not single-player
            {
                ModPacket newPacket = mod.GetPacket();

                newPacket.Write((int)mPacketMessageType);
                newPacket.Write(newVelocityX);
                newPacket.Write(newVelocityY);
                newPacket.Write(npcIndex);

                newPacket.Send();
            }
        }

        private void Deserialize(
                BinaryReader reader,
                int whoAmI)
        {
            mNewVelocityX = reader.ReadSingle();
            mNewVelocityY = reader.ReadSingle();
            mNpcIndex = reader.ReadInt32();
        }
    }
}