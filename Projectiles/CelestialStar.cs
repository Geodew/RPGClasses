using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace RPG.Projectiles
{
    public class CelestialStar : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.height = 16;
            projectile.width = 16;
            projectile.friendly = true;
            projectile.aiStyle = 0;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.maxPenetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.alpha = 100;
            projectile.ignoreWater = true;
        }

        public override void AI()
        { 
            Player p = Main.player[projectile.owner];
            MPlayer mplayer = p.GetModPlayer<MPlayer>(mod);
            if (p.dead || mplayer.special3 == 0)
            {
                projectile.Kill();
            }
            else
            {
                projectile.timeLeft = 2;
            }

            // Adjust damage based on level and player stats

            float scalar = 1f + (float)Math.Pow(mplayer.specialProgressionCount, 1.6f) / 8f;
            float damage = 11f * Math.Max(p.meleeDamage, Math.Max(p.magicDamage, Math.Max(p.rangedDamage, p.thrownDamage))) * scalar * (1f+projectile.ai[1]);
            if(p.manaSick)
            {
                damage /= 2;
            }
            projectile.damage = (int)damage;

            // Orbit around player

            double deg = projectile.ai[0];
            double rad = deg * (Math.PI / 180);
            if (mplayer.special > 0)
            {
                projectile.ai[1] = 1f; //expanded orbit
                projectile.localAI[0] = Math.Min(projectile.localAI[0] + 1, 80 + mplayer.specialProgressionCount * 3);
            }
            if(mplayer.special == 0)
            {
                projectile.localAI[0] = Math.Max(0, projectile.localAI[0] - 1);
                projectile.ai[1] = 0f; //standard orbit
            }
            double dist = 80 + projectile.localAI[0] + mplayer.specialProgressionCount * 3;
            projectile.position.X = p.Center.X - (float)(Math.Cos(rad) * dist) - projectile.width / 4f;
            projectile.position.Y = p.Center.Y - (float)(Math.Sin(rad) * dist) - projectile.height / 4f;
            projectile.ai[0] += 1f + projectile.ai[1]/1.5f; //angle change in degrees
            if (projectile.ai[0] > 360.0f)
            {
                // Wrap angle
                projectile.ai[0] -= 360.0f;
            }

            // Visuals

            if (projectile.alpha < 170)
            {
                Vector2 adj = projectile.position - projectile.oldPosition;
                for (int num136 = 0; num136 < 3; num136++)  //zzz rename
                {
                    float x2 = projectile.position.X + 4f - adj.X / 10f * num136 * 4f;
                    float y2 = projectile.position.Y + 4f - adj.Y / 10f * num136 * 4f;
                    int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, mod.DustType<Dusts.CelestialDust>(), 0f, 0f, 0, default(Color), .7f);
                    Main.dust[num137].alpha = projectile.alpha;
                    Main.dust[num137].position.X = x2;
                    Main.dust[num137].position.Y = y2;
                    Main.dust[num137].velocity *= 0f;
                    Main.dust[num137].noGravity = true;
                }
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.localNPCImmunity[target.whoAmI] = 20 - (int)projectile.ai[1]*10;
            Main.npc[target.whoAmI].immune[projectile.owner] = 0;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if(target.catchItem > 0 || target.townNPC || target.friendly)
            {
                return false;
            }
            if(projectile.localNPCImmunity[target.whoAmI] <= 0)
            {
                return true;
            }

            return base.CanHitNPC(target);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player p = Main.player[projectile.owner];
            int critChance = Math.Max(p.meleeCrit, Math.Max(p.magicCrit, Math.Max(p.rangedCrit, p.thrownCrit)));
            if (Main.rand.Next(100) < critChance)
            {
                crit = true;
            }
        }
    }
}
