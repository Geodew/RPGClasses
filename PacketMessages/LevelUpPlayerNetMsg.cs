using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RPG.PacketMessages
{
    // We don't want to just use 'NPCID' enum because (a) we want Brain of Cthulhu and Eater of Worlds to share an enum value, and
    //   (b) some bosses have multiple IDs for various segments, and this message should only be sent after all are defeated.
    // Order doesn't matter (because your level isn't dependent on the order you kill the bosses, just how many you've killed),
    //   but we'll put it in rough order of expected progression for organization's sake.
    enum BossGroupEnum
    {
        INVALID,
        OTHER,  // For future generic support for mods that add bosses? (optimistic lol)
        KING_SLIME,
        EYE_OF_CTHULHU,
        BRAIN_OF_CTHULHU_OR_EATER_OF_WORLDS,
        SKELETRON,
        QUEEN_BEE,
        WALL_OF_FLESH,
        DESTROYER,
        SKELETRON_PRIME,
        TWINS,
        PLANTERA,
        GOLEM,
        DUKE_FISHRON,
        LUNATIC_CULTIST,
        MOON_LORD
    }

    class LevelUpPlayerNetMsg
    {
        private const PacketMessageTypeEnum mPacketMessageType = PacketMessageTypeEnum.LEVEL_UP_PLAYER;

        private int mPlayerId;
        private BossGroupEnum mBossKilled;


        private void Process(
                int whoAmI,
                Mod mod)
        {
            Player player = Main.player[mPlayerId];
            MPlayer mplayer = (MPlayer)(player.GetModPlayer(mod, "MPlayer"));
            bool leveledUp = false;

            switch (mBossKilled)
            {
                case BossGroupEnum.KING_SLIME:
                    mplayer.killedSlime = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.EYE_OF_CTHULHU:
                    mplayer.killedEye = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.BRAIN_OF_CTHULHU_OR_EATER_OF_WORLDS:
                    mplayer.killedWormOrBrain = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.SKELETRON:
                    mplayer.killedSkelly = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.QUEEN_BEE:
                    mplayer.killedBee = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.WALL_OF_FLESH:
                    mplayer.killedWall = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.DESTROYER:
                    mplayer.killedDestroyer = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.SKELETRON_PRIME:
                    mplayer.killedPrime = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.TWINS:
                    mplayer.killedTwins = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.PLANTERA:
                    mplayer.killedPlant = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.GOLEM:
                    mplayer.killedGolem = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.DUKE_FISHRON:
                    mplayer.killedFish = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.LUNATIC_CULTIST:
                    mplayer.killedCultist = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.MOON_LORD:
                    mplayer.killedMoon = true;
                    leveledUp = true;
                    break;

                case BossGroupEnum.OTHER:
                    // Non-vanilla bosses (from other mods) not yet supported.
                    break;

                case BossGroupEnum.INVALID:
                default:
                    //zzz Note error?
                    break;
            }

            if (leveledUp)
            {
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 50, player.width, player.height), new Color(255, 255, 255, 255), "LEVEL UP!", true);
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
                BossGroupEnum bossDefeated)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket newPacket = mod.GetPacket();

                newPacket.Write(playerId);
                newPacket.Write((int)bossDefeated);

                newPacket.Send();
            }
        }

        private void Deserialize(
                BinaryReader reader,
                int whoAmI)
        {
            mPlayerId = reader.ReadInt32();

            try
            {
                mBossKilled = (BossGroupEnum)reader.ReadInt32();
            }
            catch
            {
                mBossKilled = BossGroupEnum.INVALID;
            }
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
                    mBossKilled);
            }
        }
    }
}
