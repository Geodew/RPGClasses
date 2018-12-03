﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace RPG.Buffs
{
    public class MonkPalm : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Knockback in progress");
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            GNPC info = npc.GetGlobalNPC<GNPC>();
            Player player = Main.player[info.monkPalmOwner];
            MPlayer mplayer = player.GetModPlayer<MPlayer>(mod);
            for(int i=0; i<200; i++)
            {
                NPC target = Main.npc[i];
                if (!target.Equals(npc))
                {
                    Rectangle targetRect = target.getRect();
                    if (targetRect.Intersects(npc.getRect()) && target.FindBuffIndex(mod.BuffType<Buffs.ActiveCooldown>()) == -1)
                    {
                        float scalar = 1.0f + (float)Math.Pow(mplayer.specialProgressionCount, 1.7) / 6.0f;
                        float damage = (20.0f * scalar * player.meleeDamage + npc.damage) * Main.rand.Next(90,111)/100f;
                        int dir = (target.position.X > npc.position.X) ? 1 : -1;
                        bool crit = Main.rand.Next(100) < player.meleeCrit;
                        target.StrikeNPC((int)damage, 12, dir, crit);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(28, -1, -1, null, target.whoAmI, damage, 12, dir, crit ? 1 : 0);
                        }
                        target.AddBuff(mod.BuffType<Buffs.ActiveCooldown>(), 60);
                    }
                }
            }
            //npc.velocity = npc.oldVelocity;
            if (npc.buffTime[buffIndex]<= 30)
            {
                npc.velocity *= .95f;
            }
            if(npc.buffTime[buffIndex] == 1 && info.killAfterKnockback)
            {
                npc.StrikeNPC(npc.life+npc.defense/2+1, 0, 0);
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(28, -1, -1, null, npc.whoAmI, npc.life + npc.defense / 2 + 1);
                }
            }
        }
    }
}
