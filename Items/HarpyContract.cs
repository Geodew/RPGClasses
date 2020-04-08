﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RPG.Items
{
    public class HarpyContract : ModItem
    {
        public override void SetDefaults()
        {


            item.consumable = true;
            item.useStyle = 2;
        }

    public override void SetStaticDefaults()
    {
      DisplayName.SetDefault("Harpy Contract");
      Tooltip.SetDefault("Class granting ranged and throwing bonuses when flying - and wings");
    }

        public override bool UseItem(Player player)
        {
            MPlayer mplayer = player.GetModPlayer<MPlayer>();
            if (mplayer.hasClass)
            {
                return true;
            }
            mplayer.hasClass = true;
            mplayer.harpy = true;
            if (player.whoAmI == Main.myPlayer)
                player.QuickSpawnItem(ItemID.RottenEgg, 50);
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
