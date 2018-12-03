using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RPG.PacketMessages;

namespace RPG
{
    public class RPG : Mod
    {
        private const string ActiveAbilityHotkeyName = "Active Ability";

        public RPG()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadBackgrounds = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        public override void Load()
        {
            RegisterHotKey(ActiveAbilityHotkeyName, "Q");
        }

        public override void HotKeyPressed(string name)
        {
            Player player = Main.player[Main.myPlayer];
            MPlayer mplayer = player.GetModPlayer<MPlayer>(this);
            if (name.Equals(ActiveAbilityHotkeyName) && (player.FindBuffIndex(BuffType<Buffs.ActiveCooldown>()) == -1))
            {
                if (mplayer.knight)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 600);
                    int damage = 6;
                    int count = mplayer.specialProgressionCount;
                    int type;
                    if (count <= 5)
                    {
                        damage += 2 * count;
                        type = ProjectileType<Projectiles.Shield1>();
                    }
                    else if (count <= 9)
                    {
                        damage += 3 * count;
                        type = ProjectileType<Projectiles.Shield2>();
                    }
                    else if (count <= 12)
                    {
                        damage += 5 * count;
                        type = ProjectileType<Projectiles.Shield3>();
                    }
                    else
                    {
                        damage += 7 * count;
                        type = ProjectileType<Projectiles.Shield3>();
                    }
                    float num11 = damage * player.meleeDamage + player.statDefense;
                    int p = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0, 0, type, (int)num11, 10, player.whoAmI);
                }
                else if (mplayer.fortress)
                {
                    player.AddBuff(BuffID.Stoned, 240);
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1800);
                }
                else if (mplayer.warmage)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1800);
                    mplayer.specialTimer = 600;
                }
                else if (mplayer.harpy)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 240);
                    Main.PlaySound(2, player.position, 32);
                    int damage = 11;
                    int count = mplayer.specialProgressionCount;
                    float scalar = 1f + (float)Math.Pow(count, 1.75) / 7;
                    damage = (int)(damage * Math.Max(player.thrownDamage, player.rangedDamage) * scalar);
                    Vector2 vel = Main.MouseWorld - player.position;
                    vel.Normalize();
                    vel *= 11 + count/2;
                    for(int i=0; i<3+count/3; i++)
                    {
                        int p = Projectile.NewProjectile(player.position.X, player.position.Y, vel.X + Main.rand.Next(-20, 21)/6, vel.Y + Main.rand.Next(-20, 21) / 6, ProjectileID.HarpyFeather, damage, 3, player.whoAmI);
                        Main.projectile[p].friendly = true;
                        Main.projectile[p].hostile = false;
                        Main.projectile[p].ranged = true;
                    }
                    Main.PlaySound(2, player.position, 32);
                }
                else if (mplayer.spiritMage)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1800);
                    int minionsKilled = 0;
                    for(int i=0; i<1000; i++)
                    {
                        if(Main.projectile[i].minion && Main.projectile[i].owner == Main.myPlayer)
                        {
                            for (int j = 0; j < 20; j++)
                            {
                                int dust = Dust.NewDust(Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height, 15);
                                Main.dust[dust].velocity.Normalize();
                                Main.dust[dust].velocity *= 3.0f;

                                SpawnDustNetMsg.SerializeAndSendPlayer(
                                    this,
                                    player.whoAmI,
                                    15,
                                    true,
                                    true,
                                    3.0f);
                            }
                            Main.projectile[i].Kill();
                            minionsKilled++;
                        }
                    }
                    mplayer.specialTimer = 480;
                    mplayer.special2 = minionsKilled;
                }
                else if (mplayer.demon)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 20);
                    for(int i=0; i<1000; i++)
                    {
                        if(Main.projectile[i].type == ProjectileType<Projectiles.Demon>() && Main.projectile[i].owner == Main.myPlayer && !Main.projectile[i].tileCollide && Main.projectile[i].active)
                        {
                            Vector2 vel = Main.MouseWorld - Main.projectile[i].position;
                            vel.Normalize();
                            vel *= 10 + mplayer.specialProgressionCount / 2;
                            Main.projectile[i].velocity = vel;
                            Main.projectile[i].alpha = 150;
                            Main.projectile[i].friendly = true;
                            Main.PlaySound(2, player.position, 8);
                            mplayer.special--;
                            break;
                        }
                    }
                }
                else if (mplayer.werewolf)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 10800);
                    Main.PlaySound(3, player.position, 6);
                    mplayer.specialTimer = 3600;
                }
                else if (mplayer.sage)
                {
                    if(player.statManaMax <= 0)
                    {
                        return;
                    }
                    mplayer.specialTimer = 3;
                    if(mplayer.special == 119)
                    {
                        Main.PlaySound(25, -1, -1, 1);
                        for (int i = 0; i < 5; i++)
                        {
                            int num3 = Dust.NewDust(player.position, player.width, player.height, 45, 0f, 0f, 255, default(Color), (float)Main.rand.Next(20, 26) * 0.1f);
                            Main.dust[num3].noLight = true;
                            Main.dust[num3].noGravity = true;
                            Main.dust[num3].velocity *= 0.5f;
                        }
                    }
                }
                else if (mplayer.dragoon)
                {
                    mplayer.specialTimer = 3;
                    if (mplayer.special == 119)
                    {
                        Main.PlaySound(25, -1, -1, 1);
                        for (int i = 0; i < 5; i++)
                        {
                            int num3 = Dust.NewDust(player.position, player.width, player.height, 45, 0f, 0f, 255, default(Color), (float)Main.rand.Next(20, 26) * 0.1f);
                            Main.dust[num3].noLight = true;
                            Main.dust[num3].noGravity = true;
                            Main.dust[num3].velocity *= 0.5f;
                        }
                    }
                }
                else if (mplayer.soulbound)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1500);
                    mplayer.specialTimer = 600;
                    if (Main.expertMode)
                    {
                        player.AddBuff(BuffID.Cursed, 300);
                    }
                    else
                    {
                        player.AddBuff(BuffID.Cursed, 600);
                    }
                }
                else if (mplayer.ninja)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 900);
                    Vector2 vel = Main.MouseWorld - player.Center;
                    vel.Normalize();
                    vel *= 11;
                    int p = Projectile.NewProjectile(player.Center.X, player.Center.Y, vel.X, vel.Y, ProjectileType<Projectiles.DeathMark>(), 1, 0, player.whoAmI);
                    Main.PlaySound(2, player.position, 19);
                }
                else if (mplayer.merman)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 3600*8);
                    Main.raining = true;
                    Main.cloudBGActive = 1;
                    Main.cloudBGAlpha = 255;
                    Main.maxRaining = 1;
                    Main.rainTime = 3600*4;
                }
                else if (mplayer.cavalry)
                {
                    mplayer.specialTimer = 60;
                    player.velocity.X += (14 + mplayer.specialProgressionCount) * player.direction;
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 600);
                }
                else if (mplayer.pirate)
                {
                    bool flag = (player.position.X < 850 * 16 || player.position.X > (Main.maxTilesX - 850) * 16) && player.position.Y < Main.worldSurface * 16;
                    if (flag)
                    {
                        player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1800);
                        int damage = 30;
                        int count = mplayer.specialProgressionCount;
                        float scalar = 1f + (float)Math.Pow(count, 1.65) / 6;
                        damage = (int)(player.rangedDamage * damage * scalar);
                        int direction = (player.position.X < 850 * 16) ? 1 : -1;
                        Vector2 soundPos = new Vector2(player.position.X - 1000 * direction);
                        Main.PlaySound(2, soundPos, 38);
                        float posX = Main.MouseWorld.X - 1220 * direction;
                        float posY = Main.MouseWorld.Y - 3000;
                        for(int i=0; i<3+count/2; i++)
                        {
                            int p = Projectile.NewProjectile(posX + Main.rand.Next(-150, 151), posY + Main.rand.Next(-150, 151), 12 * direction, 0, 162, damage, 8, player.whoAmI);
                        }
                    }
                    else
                    {
                        player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 18000);
                        player.AddBuff(BuffType<Buffs.RumDrunk>(), 7200);
                        Main.PlaySound(2, player.position, 3);
                    }
                }
                else if (mplayer.pharaoh)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 14400);
                    mplayer.specialTimer = 1800;
                    if (player.ZoneDesert || player.ZoneUndergroundDesert)
                    {
                        mplayer.special2 = 1;  // Get bigger bonuses
                    }
                }
                else if (mplayer.arcaneSniper)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1800);
                    mplayer.specialTimer = 600;
                    mplayer.special2 = 1 + mplayer.specialProgressionCount / 4;  // Number of empowered shots
                }
                else if (mplayer.hallowMage)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1800);
                    mplayer.specialTimer = 25 + mplayer.specialProgressionCount * 8;
                }
                else if (mplayer.explorer)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1800);
                    player.TeleportationPotion();
                }
                else if (mplayer.taintedElf)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1800);
                    mplayer.specialTimer = 600;
                }
                else if (mplayer.bloodKnight)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 3600);
                    WorldGen.Convert((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 4, 30);
                    int damage = 25;
                    int count = mplayer.specialProgressionCount;
                    float scalar = 1f + (float)Math.Pow(count, 1.6) / 6;
                    damage = (int)(player.meleeDamage * damage * scalar);
                    Main.PlaySound(41, player.position);
                    DamageArea(player.Center, 150, damage, 10);//set damage progression
                    for (int j = 0; j < 50; j++)
                    {
                        int dust = Dust.NewDust(player.position, player.width, player.height, 90);
                        Main.dust[dust].velocity.Normalize();
                        Main.dust[dust].velocity *= 16.0f;
                        Main.dust[dust].noGravity = true;

                        SpawnDustNetMsg.SerializeAndSendPlayer(
                            this,
                            player.whoAmI,
                            90,
                            true,
                            true,
                            16.0f);
                    }
                }
                else if (mplayer.wanderer)
                {
                    if (mplayer.special3 > 0)
                    {
                        player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 60);
                        int p = Projectile.NewProjectile(Main.MouseWorld.X, Main.MouseWorld.Y, 0, 0, ProjectileType<Projectiles.WandererPortal>(), 0, 0, Main.myPlayer);
                        mplayer.special++;
                        for (int i = 0; i < 1000; i++)
                        {
                            if (Main.projectile[i].type == ProjectileType<Projectiles.WandererCharge>() && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].ai[1] == mplayer.special3)
                            {
                                Main.projectile[i].Kill();
                                break;
                            }
                        }
                    }
                }
                else if (mplayer.angel)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 7200);
                    mplayer.specialTimer = 600;
                }
                else if (mplayer.marksman)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 5400);
                    mplayer.specialTimer = 600;
                    player.AddBuff(BuffID.NightOwl, 600);
                }
                else if (mplayer.ranger)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 3600);
                    mplayer.specialTimer = 300;
                }
                else if (mplayer.dwarf)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 3600 * 6);
                    player.AddBuff(BuffType<Buffs.DwarvenStout>(), 7200);
                    Main.PlaySound(2, player.position, 3);
                }
                else if (mplayer.savage)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 7200);
                    mplayer.specialTimer = 900;
                    if (Main.expertMode)
                    {
                        player.AddBuff(BuffID.Bleeding, 450);
                    }
                    else
                    {
                        player.AddBuff(BuffID.Bleeding, 900);
                    }
                    player.Hurt(new Terraria.DataStructures.PlayerDeathReason(), 1, 0);
                }
                else if (mplayer.berserker)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 7200);
                    player.AddBuff(BuffID.Battle, 1200);
                    mplayer.specialTimer = 1200;
                }
                else if (mplayer.chronomancer)
                {
                    if (mplayer.specialTimer <= 0)
                    {
                        player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 120);
                        mplayer.specialTimer = 600;
                        int p = Projectile.NewProjectile(player.position.X, player.position.Y, 0, 0, ProjectileType<Projectiles.ChronoGhost>(), 0, 0, player.whoAmI);
                        mplayer.special2 = p;
                        mplayer.special = player.statLife;
                    }
                    else
                    {
                        player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 3600);
                        Projectile proj = Main.projectile[mplayer.special2];
                        player.Teleport(proj.position);
                        float scalar = 1f + (float)Math.Pow(mplayer.specialProgressionCount, 1.7) / 4;
                        float damage = 48 * scalar * player.magicDamage * (1+player.magicCrit/100.0f);
                        DamageArea(proj.Center, 256, (int)damage, 15);
                        for(int i=1; i<5; i++)
                        {
                            float velocityScalar = 2.5f * i;

                            for(int j = 0; j < (15 * i); j++)
                            {
                                //zzz This loop doesn't depend on j. What's the point of generating multiple Dust objects on top of each other?
                                //    Was velocityScalar supposed to scale with j instead of i, perhaps?
                                int d = Dust.NewDust(proj.Center, proj.width, proj.height, 15);
                                Main.dust[d].velocity.Normalize();
                                Main.dust[d].velocity *= velocityScalar;

                                SpawnDustNetMsg.SerializeAndSendProjectile(
                                    this,
                                    proj.whoAmI,
                                    15,
                                    true,
                                    true,
                                    velocityScalar);
                            }
                        }
                        if(player.statLife < mplayer.special)
                        {
                            int healAmount = mplayer.special - player.statLife;
                            player.HealEffect(healAmount);
                            player.statLife = mplayer.special;

                            HealPlayerNetMsg.SerializeAndSend(
                                this,
                                player.whoAmI,
                                healAmount);
                        }
                        proj.Kill();
                    }
                }
                else if (mplayer.contractedSword)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1800);
                    float scalar = 1f + (float)Math.Pow(mplayer.specialProgressionCount, 1.6) / 7;
                    float damage = 18 * scalar * (player.meleeDamage + player.minionDamage -1.5f);
                    Projectile.NewProjectile(player.position.X, player.position.Y, 8 * player.direction, 0, ProjectileType<Projectiles.SpiritKnight>(), (int)damage, 10, player.whoAmI);
                }
                else if (mplayer.angler)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1800);
                    Vector2 vel = Main.MouseWorld - player.Center;
                    vel.Normalize();
                    vel *= 9;
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, vel.X, vel.Y, ProjectileType<Projectiles.AnglerChum>(), 1, 0, player.whoAmI);
                }
                else if (mplayer.conjuror)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 600);
                    float scalar = 1f + (float)Math.Pow(mplayer.specialProgressionCount, 1.6) / 6;
                    float damage = 3 * scalar * (player.minionDamage);
                    Projectile.NewProjectile(Main.MouseWorld.X, Main.MouseWorld.Y, 0, 0, ProjectileType<Projectiles.ConjurorCrystal>(), (int)damage, 0, player.whoAmI);
                }
                else if (mplayer.celestial)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 180);
                    if (mplayer.special == 0)
                    {
                        mplayer.special = 1;
                    }
                    else
                    {
                        mplayer.special = 0;
                    }
                }
                else if (mplayer.voidwalker)
                {
                    int cost = 15 + 20 * mplayer.special2;
                    if (player.statMana < cost) { return; }
                    Vector2 vector15;
                    vector15.X = (float)Main.mouseX + Main.screenPosition.X;
                    if (player.gravDir == 1f)
                    {
                        vector15.Y = (float)Main.mouseY + Main.screenPosition.Y - (float)player.height;
                    }
                    else
                    {
                        vector15.Y = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY;
                    }

                    // Reduce to max teleport distance
                    float dist = Vector2.Distance(vector15, player.Center);
                    float maxDist = 200 + 50 * mplayer.specialProgressionCount;
                    if (dist > maxDist)
                    {
                        Vector2 toMouse = Main.MouseWorld - player.Center;
                        toMouse.Normalize();
                        toMouse *= (dist - maxDist);
                        vector15 -= toMouse;
                    }
                    vector15.X += player.width / 2f;
                    vector15.Y += player.height / 2f;
                    if (vector15.X > 50f && vector15.X < (float)(Main.maxTilesX * 16 - 50) && vector15.Y > 50f && vector15.Y < (float)(Main.maxTilesY * 16 - 50))
                    {
                        int num260 = (int)(vector15.X / 16f);
                        int num261 = (int)(vector15.Y / 16f);
                        if (((Main.tile[num260, num261].wall != WallID.LihzahrdBrickUnsafe) || ((double)num261 <= Main.worldSurface) || NPC.downedPlantBoss) &&  // Don't allow teleport inside the Jungle Temple unless the player should have a Temple Key by now (Plantera defeated)
                            !Collision.SolidCollision(vector15, player.width, player.height))
                        {
                            player.Teleport(vector15, 1, 0);
                            NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, (float)player.whoAmI, vector15.X, vector15.Y, 1, 0, 0);

                            player.statMana -= cost;
                            player.manaRegenDelay = 90;
                            mplayer.special2++;
                            player.AddBuff(BuffType<Buffs.ActiveCooldown>(), Math.Max(480 - mplayer.specialProgressionCount * 30, 60));
                            player.AddBuff(BuffID.ChaosState, 600);
                            mplayer.specialTimer = 600;
                            if (mplayer.special2 > 3)
                            {
                                int selfDam = Math.Min(player.statLifeMax / 20 * (mplayer.special2 - 3), player.statLifeMax / 5);
                                player.statLife -= selfDam;
                                CombatText.NewText(player.getRect(), Color.OrangeRed, selfDam.ToString());
                                player.lifeRegenTime = 0;
                                player.lifeRegenCount = 0;
                                if (player.statLife <= 0)
                                {
                                    player.KillMe(new Terraria.DataStructures.PlayerDeathReason(), 1.0, 0);
                                }
                            }

                            // Deal area damage
                            float scalar = 1f + (float)Math.Pow(mplayer.specialProgressionCount, 1.6) / 6;
                            float damage = 14 * scalar * Math.Max(player.magicDamage, player.meleeDamage) * Math.Max(1 + player.magicCrit / 100f, 1 + player.meleeCrit / 100f) * (1 + mplayer.special2 / 3f);
                            DamageArea(vector15, 144, (int)damage, 3);

                            // Visuals
                            for (int i = 0; i < 30; i++)
                            {
                                int d = Dust.NewDust(vector15, 1, 1, 27);
                                Main.dust[d].velocity *= 4.0f;

                                SpawnDustNetMsg.SerializeAndSendPosition(
                                    this,
                                    vector15,
                                    27,
                                    true,
                                    false,
                                    4.0f);
                            }
                        }
                    }
                }
                else if (mplayer.moth)
                {
                    if (mplayer.special == 0)
                    {
                        mplayer.special = 1;
                    }
                    else
                    {
                        mplayer.special = 0;
                    }
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 180);
                }
                else if (mplayer.monk)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 600);
                    float scalar = 1f + (float)Math.Pow(mplayer.specialProgressionCount, 1.7) / 6;
                    float dam = 18 * scalar * player.meleeDamage;
                    Vector2 toMouse = Main.MouseWorld - player.Center;
                    toMouse.Normalize();
                    toMouse *= 4;
                    //toMouse += player.velocity;
                    int p = Projectile.NewProjectile(player.Center.X, player.Center.Y, toMouse.X, toMouse.Y, ProjectileType<Projectiles.MonkPalm>(), (int)dam, 0, player.whoAmI);
                }
                else if (mplayer.warpKnight)
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 900 - mplayer.specialProgressionCount*30);
                    float scalar = 1f + (float)Math.Pow(mplayer.specialProgressionCount, 1.6) / 6;
                    float dam = 18 * scalar * player.meleeDamage;
                    Vector2 toMouse = Main.MouseWorld - player.Center;
                    toMouse.Normalize();
                    toMouse *= 11;
                    int p = Projectile.NewProjectile(player.Center.X, player.Center.Y, toMouse.X, toMouse.Y, ProjectileType<Projectiles.WarpBolt>(), (int)dam, 0, player.whoAmI);
                }
                else if (mplayer.heritor)
                {
                    if(mplayer.special == 0)
                    {
                        //reset between uses
                        if(mplayer.special3 == -1)
                        {
                            mplayer.special3 = 0;
                        }
                        if(mplayer.special3 == 0)
                        {
                            player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 60);
                            //begin cycle, exclude special2
                            mplayer.special3 = 540;
                        }
                        else
                        {
                            mplayer.special = mplayer.special4;
                            mplayer.special3 = -1;
                            mplayer.specialTimer = 1800;
                            player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 1800);
                        }
                    }
                }
                else
                {
                    player.AddBuff(BuffType<Buffs.ActiveCooldown>(), 3600);
                    player.statLife += 30 + mplayer.specialProgressionCount * 5;
                    player.HealEffect(30 + mplayer.specialProgressionCount * 5);
                }
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketMessageTypeEnum messageType = (PacketMessageTypeEnum)reader.ReadInt32();
            switch(messageType)
            {
                case PacketMessageTypeEnum.SPAWN_DUST:
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SpawnDustNetMsg spawnDustNetMsg = new SpawnDustNetMsg();
                        spawnDustNetMsg.HandlePacket(
                            reader,
                            whoAmI,
                            this);
                    }
                    break;

                case PacketMessageTypeEnum.VELOCITY_CHANGE_NPC:
                    VelocityChangeNpcNetMsg velocityChangeNpcNetMsg = new VelocityChangeNpcNetMsg();
                    velocityChangeNpcNetMsg.HandlePacket(
                        reader,
                        whoAmI,
                        this);
                    break;

                case PacketMessageTypeEnum.SANDSTORM_VISUALS:
                    SandstormVisualsNetMsg sandstormVisualsNetMsg = new SandstormVisualsNetMsg();
                    sandstormVisualsNetMsg.HandlePacket(
                        reader,
                        whoAmI,
                        this);
                    break;

                case PacketMessageTypeEnum.HEAL_PLAYER:
                    HealPlayerNetMsg healPlayerNetMsg = new HealPlayerNetMsg();
                    healPlayerNetMsg.HandlePacket(
                        reader,
                        whoAmI,
                        this);
                    break;

                case PacketMessageTypeEnum.LEVEL_UP_PLAYER:
                    LevelUpPlayerNetMsg levelUpPlayerNetMsg = new LevelUpPlayerNetMsg();
                    levelUpPlayerNetMsg.HandlePacket(
                        reader,
                        whoAmI,
                        this);
                    break;

                case PacketMessageTypeEnum.PLAYER_CLASS_LEVEL_INFO:
                    PlayerClassLevelInfoNetMsg playerClassLevelInfoNetMsg = new PlayerClassLevelInfoNetMsg();
                    playerClassLevelInfoNetMsg.HandlePacket(
                        reader,
                        whoAmI,
                        this);
                    break;

                default:
                    //zzz log unhandled message
                    break;
            }
        }

        public static void DamageArea(Vector2 p, int width, int damage, int knockback)  // Hostile npcs, no crit, no immunity
        {
            damage = (int)(damage * Main.rand.Next(90, 111) / 100.0);
            Microsoft.Xna.Framework.Rectangle hurtbox = new Microsoft.Xna.Framework.Rectangle((int)p.X - width, (int)p.Y - width, width * 2, width * 2);
            for (int i = 0; i < 200; i++)
            {
                bool flag2 = hurtbox.Intersects(Main.npc[i].getRect());
                if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && flag2 && !Main.npc[i].townNPC)
                {
                    int direction = (Main.npc[i].position.X > p.X ? 1:-1);
                    int d = (int)Main.npc[i].StrikeNPC(damage, knockback, direction, false, false, false);
                    NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, i, (float)damage, knockback, (float)direction, 0, 0, 0);
                }
            }
        }
    }
}
