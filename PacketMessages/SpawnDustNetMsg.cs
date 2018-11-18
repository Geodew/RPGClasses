using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace RPG.PacketMessages
{
    enum DustMessageSubtypeEnum
    {
        INVALID,
        POSITION,
        PROJECTILE_POSITION,
        PLAYER_POSITION
    }

    class SpawnDustNetMsg
    {
        private const PacketMessageTypeEnum mPacketMessageType = PacketMessageTypeEnum.SPAWN_DUST;

        private DustMessageSubtypeEnum mMessageSubtype;
        private int mDustTypeID;
        private int? mProjectileId = null;
        private float? mPositionX = null;
        private float? mPositionY = null;
        private int? mPlayerId = null;
        private bool mModifyVelocity;
        private bool mNormalizeVelocity;
        private float mVelocityMultiplier;


        private void Process(
                int whoAmI,
                Mod mod)
        {
            int dustIdIndex;

            switch (mMessageSubtype)
            {
                case DustMessageSubtypeEnum.POSITION:
                    Vector2 pos = new Vector2(mPositionX.Value, mPositionY.Value);
                    dustIdIndex = Dust.NewDust(pos, 1, 1, mDustTypeID);
                    break;

                case DustMessageSubtypeEnum.PROJECTILE_POSITION:
                    Projectile proj = Main.projectile[mProjectileId.Value];
                    dustIdIndex = Dust.NewDust(proj.position, proj.width, proj.height, mDustTypeID);
                    break;

                case DustMessageSubtypeEnum.PLAYER_POSITION:
                    Player player = Main.player[mPlayerId.Value];
                    dustIdIndex = Dust.NewDust(player.position, player.width, player.height, mDustTypeID);
                    break;

                case DustMessageSubtypeEnum.INVALID:
                default:
                    // Shouldn't happen: unimplemented
                    return;
                    //break;  // (Unreachable)
            }

            if (mModifyVelocity)
            {
                if (mNormalizeVelocity)
                {
                    Main.dust[dustIdIndex].velocity.Normalize();
                }

                Main.dust[dustIdIndex].velocity *= mVelocityMultiplier;
                if ((mDustTypeID == 91) || (mDustTypeID == 90) || (mDustTypeID == 27))
                {
                    Main.dust[dustIdIndex].noGravity = true;
                }
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
            Process(
                whoAmI,
                mod);
        }

        public static void SerializeAndSendPosition(
                Mod mod,
                Vector2 position,
                int dustTypeID,
                bool modifyVelocity,
                bool normalize,
                float velocityMultiplier)
        {
            SerializeAndSendPosition(
                mod,
                position.X,
                position.Y,
                dustTypeID,
                modifyVelocity,
                normalize,
                velocityMultiplier);
        }

        public static void SerializeAndSendPosition(
                Mod mod,
                float positionX,
                float positionY,
                int dustTypeID,
                bool modifyVelocity,
                bool normalize,
                float velocityMultiplier)
        {
            ModPacket newPacket = mod.GetPacket();

            newPacket.Write((int)mPacketMessageType);
            newPacket.Write((int)DustMessageSubtypeEnum.POSITION);
            newPacket.Write(dustTypeID);
            newPacket.Write(positionX);
            newPacket.Write(positionY);
            newPacket.Write(modifyVelocity);
            newPacket.Write(normalize);
            newPacket.Write(velocityMultiplier);

            newPacket.Send();
        }

        public static void SerializeAndSendProjectile(
                Mod mod,
                int projectileId,
                int dustTypeID,
                bool modifyVelocity,
                bool normalize,
                float velocityMultiplier)
        {
            ModPacket newPacket = mod.GetPacket();

            newPacket.Write((int)mPacketMessageType);
            newPacket.Write((int)DustMessageSubtypeEnum.PROJECTILE_POSITION);
            newPacket.Write(dustTypeID);
            newPacket.Write(projectileId);
            newPacket.Write(modifyVelocity);
            newPacket.Write(normalize);
            newPacket.Write(velocityMultiplier);

            newPacket.Send();
        }

        public static void SerializeAndSendPlayer(
                Mod mod,
                int playerId,
                int dustTypeID,
                bool modifyVelocity,
                bool normalize,
                float velocityMultiplier)
        {
            ModPacket newPacket = mod.GetPacket();

            newPacket.Write((int)mPacketMessageType);
            newPacket.Write((int)DustMessageSubtypeEnum.PLAYER_POSITION);
            newPacket.Write(dustTypeID);
            newPacket.Write(playerId);
            newPacket.Write(modifyVelocity);
            newPacket.Write(normalize);
            newPacket.Write(velocityMultiplier);

            newPacket.Send();
        }

        private void Deserialize(
                BinaryReader reader,
                int whoAmI)
        {
            try
            {
                mMessageSubtype = (DustMessageSubtypeEnum)reader.ReadInt32();
            }
            catch
            {
                mMessageSubtype = DustMessageSubtypeEnum.INVALID;
            }

            mDustTypeID = reader.ReadInt32();

            switch(mMessageSubtype)
            {
                case DustMessageSubtypeEnum.POSITION:
                    mPositionX = reader.ReadSingle();
                    mPositionY = reader.ReadSingle();
                    break;

                case DustMessageSubtypeEnum.PROJECTILE_POSITION:
                    mProjectileId = reader.ReadInt32();
                    break;

                case DustMessageSubtypeEnum.PLAYER_POSITION:
                    mPlayerId = reader.ReadInt32();
                    break;

                case DustMessageSubtypeEnum.INVALID:
                default:
                    // Shouldn't happen: unimplemented
                    int dummy = reader.ReadInt32(); // Skip an int because that's what's most common, in an effort to read the rest of the message for debugging (save it into temp variable for debugging too)
                    break;
            }

            mModifyVelocity = reader.ReadBoolean();
            mNormalizeVelocity = reader.ReadBoolean();
            mVelocityMultiplier = reader.ReadSingle();
        }
    }
}
