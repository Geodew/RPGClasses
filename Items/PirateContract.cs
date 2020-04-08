﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RPG.Items
{
    public class PirateContract : ModItem
    {
        public override void SetDefaults()
        {


            item.consumable = true;
            item.useStyle = 2;
        }

    public override void SetStaticDefaults()
    {
      DisplayName.SetDefault("Pirate Contract");
      Tooltip.SetDefault("Class focusing on melee and bullet damage; empowered in Ocean");
    }

        public override bool UseItem(Player player)
        {
            MPlayer mplayer = player.GetModPlayer<MPlayer>();
            if (mplayer.hasClass)
            {
                return true;
            }
            mplayer.hasClass = true;
            mplayer.pirate = true;
            if (player.whoAmI == Main.myPlayer)
            {
                player.QuickSpawnItem(ItemID.BuccaneerBandana);
            player.QuickSpawnItem(ItemID.BuccaneerShirt);
            player.QuickSpawnItem(ItemID.BuccaneerPants);
            player.QuickSpawnItem(ItemID.FlintlockPistol);
            player.QuickSpawnItem(ItemID.MusketBall, 24);
            }

            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType<BlankContract>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
