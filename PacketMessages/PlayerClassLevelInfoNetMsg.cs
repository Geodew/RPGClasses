using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RPG.PacketMessages
{
    class PlayerClassLevelInfoNetMsg
    {
        private const PacketMessageTypeEnum mPacketMessageType = PacketMessageTypeEnum.PLAYER_CLASS_LEVEL_INFO;

        private int mPlayerId;
        //zzz add version information so we know what classes to expect from the other person and in what order

        private bool mHasClass;

        #region Class flags
        // Omitting the standard `m` in front of these symbols for easy copy-paste from MPlayer.cs...
        //zzz Consider moving these bools to a struct for common use instead.
        private bool knight;
        private bool berserker;
        private bool fortress;
        private bool sage;
        private bool warmage;
        private bool conjuror;
        private bool spiritMage;
        private bool contractedSword;
        private bool wanderer;
        private bool marksman;
        private bool ranger;
        private bool arcaneSniper;
        private bool savage;
        private bool ninja;
        private bool rogue;
        private bool soulbound;
        private bool explorer;
        private bool cavalry;
        private bool merman;
        private bool werewolf;
        private bool harpy;
        private bool angel;
        private bool demon;
        private bool dwarf;
        private bool bloodKnight;
        private bool taintedElf;
        private bool hallowMage;
        private bool pharaoh;
        private bool pirate;
        private bool jungleShaman;
        private bool viking;
        private bool truffle;
        private bool dragoon;
        private bool chronomancer;
        private bool angler;
        private bool celestial;
        private bool voidwalker;
        private bool moth;
        private bool monk;
        private bool warpKnight;
        private bool heritor;
        #endregion
        #region Killed boss flags
        private bool killedSlime;
        private bool killedEye;
        private bool killedWormOrBrain;
        private bool killedSkelly;
        private bool killedBee;
        private bool killedWall;
        private bool killedDestroyer;
        private bool killedTwins;
        private bool killedPrime;
        private bool killedPlant;
        private bool killedGolem;
        private bool killedFish;
        private bool killedCultist;
        private bool killedMoon;
        #endregion


        private void Process(
                int senderPlayerId,
                Mod mod)
        {
            if (mPlayerId != Main.myPlayer)
            {
                Player player = Main.player[mPlayerId];
                MPlayer modPlayer = player.GetModPlayer<MPlayer>();

                modPlayer.hasClass = mHasClass;

                #region Class flags
                modPlayer.knight = knight;
                modPlayer.berserker = berserker;
                modPlayer.fortress = fortress;
                modPlayer.sage = sage;
                modPlayer.warmage = warmage;
                modPlayer.conjuror = conjuror;
                modPlayer.spiritMage = spiritMage;
                modPlayer.contractedSword = contractedSword;
                modPlayer.wanderer = wanderer;
                modPlayer.marksman = marksman;
                modPlayer.ranger = ranger;
                modPlayer.arcaneSniper = arcaneSniper;
                modPlayer.savage = savage;
                modPlayer.ninja = ninja;
                modPlayer.rogue = rogue;
                modPlayer.soulbound = soulbound;
                modPlayer.explorer = explorer;
                modPlayer.cavalry = cavalry;
                modPlayer.merman = merman;
                modPlayer.werewolf = werewolf;
                modPlayer.harpy = harpy;
                modPlayer.angel = angel;
                modPlayer.demon = demon;
                modPlayer.dwarf = dwarf;
                modPlayer.bloodKnight = bloodKnight;
                modPlayer.taintedElf = taintedElf;
                modPlayer.hallowMage = hallowMage;
                modPlayer.pharaoh = pharaoh;
                modPlayer.pirate = pirate;
                modPlayer.jungleShaman = jungleShaman;
                modPlayer.viking = viking;
                modPlayer.truffle = truffle;
                modPlayer.dragoon = dragoon;
                modPlayer.chronomancer = chronomancer;
                modPlayer.angler = angler;
                modPlayer.celestial = celestial;
                modPlayer.voidwalker = voidwalker;
                modPlayer.moth = moth;
                modPlayer.monk = monk;
                modPlayer.warpKnight = warpKnight;
                modPlayer.heritor = heritor;
                #endregion
                #region Killed boss flags
                modPlayer.killedSlime = killedSlime;
                modPlayer.killedEye = killedEye;
                modPlayer.killedWormOrBrain = killedWormOrBrain;
                modPlayer.killedSkelly = killedSkelly;
                modPlayer.killedBee = killedBee;
                modPlayer.killedWall = killedWall;
                modPlayer.killedDestroyer = killedDestroyer;
                modPlayer.killedTwins = killedTwins;
                modPlayer.killedPrime = killedPrime;
                modPlayer.killedPlant = killedPlant;
                modPlayer.killedGolem = killedGolem;
                modPlayer.killedFish = killedFish;
                modPlayer.killedCultist = killedCultist;
                modPlayer.killedMoon = killedMoon;
                #endregion
            }
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
                MPlayer modPlayer,
                int toWho,
                int fromWho)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket newPacket = mod.GetPacket();

                newPacket.Write((int)mPacketMessageType);

                newPacket.Write(modPlayer.player.whoAmI);

                newPacket.Write(modPlayer.hasClass);

                #region Class flags
                newPacket.Write(modPlayer.knight);
                newPacket.Write(modPlayer.berserker);
                newPacket.Write(modPlayer.fortress);
                newPacket.Write(modPlayer.sage);
                newPacket.Write(modPlayer.warmage);
                newPacket.Write(modPlayer.conjuror);
                newPacket.Write(modPlayer.spiritMage);
                newPacket.Write(modPlayer.contractedSword);
                newPacket.Write(modPlayer.wanderer);
                newPacket.Write(modPlayer.marksman);
                newPacket.Write(modPlayer.ranger);
                newPacket.Write(modPlayer.arcaneSniper);
                newPacket.Write(modPlayer.savage);
                newPacket.Write(modPlayer.ninja);
                newPacket.Write(modPlayer.rogue);
                newPacket.Write(modPlayer.soulbound);
                newPacket.Write(modPlayer.explorer);
                newPacket.Write(modPlayer.cavalry);
                newPacket.Write(modPlayer.merman);
                newPacket.Write(modPlayer.werewolf);
                newPacket.Write(modPlayer.harpy);
                newPacket.Write(modPlayer.angel);
                newPacket.Write(modPlayer.demon);
                newPacket.Write(modPlayer.dwarf);
                newPacket.Write(modPlayer.bloodKnight);
                newPacket.Write(modPlayer.taintedElf);
                newPacket.Write(modPlayer.hallowMage);
                newPacket.Write(modPlayer.pharaoh);
                newPacket.Write(modPlayer.pirate);
                newPacket.Write(modPlayer.jungleShaman);
                newPacket.Write(modPlayer.viking);
                newPacket.Write(modPlayer.truffle);
                newPacket.Write(modPlayer.dragoon);
                newPacket.Write(modPlayer.chronomancer);
                newPacket.Write(modPlayer.angler);
                newPacket.Write(modPlayer.celestial);
                newPacket.Write(modPlayer.voidwalker);
                newPacket.Write(modPlayer.moth);
                newPacket.Write(modPlayer.monk);
                newPacket.Write(modPlayer.warpKnight);
                newPacket.Write(modPlayer.heritor);
                #endregion
                #region Killed boss flags
                newPacket.Write(modPlayer.killedSlime);
                newPacket.Write(modPlayer.killedEye);
                newPacket.Write(modPlayer.killedWormOrBrain);
                newPacket.Write(modPlayer.killedSkelly);
                newPacket.Write(modPlayer.killedBee);
                newPacket.Write(modPlayer.killedWall);
                newPacket.Write(modPlayer.killedDestroyer);
                newPacket.Write(modPlayer.killedTwins);
                newPacket.Write(modPlayer.killedPrime);
                newPacket.Write(modPlayer.killedPlant);
                newPacket.Write(modPlayer.killedGolem);
                newPacket.Write(modPlayer.killedFish);
                newPacket.Write(modPlayer.killedCultist);
                newPacket.Write(modPlayer.killedMoon);
                #endregion

                newPacket.Send(toWho, fromWho);
            }
        }

        private void Deserialize(
                BinaryReader reader,
                int senderPlayerId)
        {
            mPlayerId = reader.ReadInt32();

            mHasClass = reader.ReadBoolean();

            #region Class flags
            knight = reader.ReadBoolean();
            berserker = reader.ReadBoolean();
            fortress = reader.ReadBoolean();
            sage = reader.ReadBoolean();
            warmage = reader.ReadBoolean();
            conjuror = reader.ReadBoolean();
            spiritMage = reader.ReadBoolean();
            contractedSword = reader.ReadBoolean();
            wanderer = reader.ReadBoolean();
            marksman = reader.ReadBoolean();
            ranger = reader.ReadBoolean();
            arcaneSniper = reader.ReadBoolean();
            savage = reader.ReadBoolean();
            ninja = reader.ReadBoolean();
            rogue = reader.ReadBoolean();
            soulbound = reader.ReadBoolean();
            explorer = reader.ReadBoolean();
            cavalry = reader.ReadBoolean();
            merman = reader.ReadBoolean();
            werewolf = reader.ReadBoolean();
            harpy = reader.ReadBoolean();
            angel = reader.ReadBoolean();
            demon = reader.ReadBoolean();
            dwarf = reader.ReadBoolean();
            bloodKnight = reader.ReadBoolean();
            taintedElf = reader.ReadBoolean();
            hallowMage = reader.ReadBoolean();
            pharaoh = reader.ReadBoolean();
            pirate = reader.ReadBoolean();
            jungleShaman = reader.ReadBoolean();
            viking = reader.ReadBoolean();
            truffle = reader.ReadBoolean();
            dragoon = reader.ReadBoolean();
            chronomancer = reader.ReadBoolean();
            angler = reader.ReadBoolean();
            celestial = reader.ReadBoolean();
            voidwalker = reader.ReadBoolean();
            moth = reader.ReadBoolean();
            monk = reader.ReadBoolean();
            warpKnight = reader.ReadBoolean();
            heritor = reader.ReadBoolean();
            #endregion
            #region Killed boss flags
            killedSlime = reader.ReadBoolean();
            killedEye = reader.ReadBoolean();
            killedWormOrBrain = reader.ReadBoolean();
            killedSkelly = reader.ReadBoolean();
            killedBee = reader.ReadBoolean();
            killedWall = reader.ReadBoolean();
            killedDestroyer = reader.ReadBoolean();
            killedTwins = reader.ReadBoolean();
            killedPrime = reader.ReadBoolean();
            killedPlant = reader.ReadBoolean();
            killedGolem = reader.ReadBoolean();
            killedFish = reader.ReadBoolean();
            killedCultist = reader.ReadBoolean();
            killedMoon = reader.ReadBoolean();
            #endregion
        }

        private void ServerBroadcast(
                int senderPlayerId,
                Mod mod)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                Player player = Main.player[mPlayerId];
                MPlayer modPlayer = player.GetModPlayer<MPlayer>();

                SerializeAndSend(
                    mod,
                    modPlayer,
                    -1,
                    mPlayerId);
            }
        }
    }
}
