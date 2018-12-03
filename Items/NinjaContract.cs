﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RPG.Items
{
    public class NinjaContract : ModItem
    {
        public override void SetDefaults()
        {


            item.consumable = true;
            item.useStyle = 2;
        }

    public override void SetStaticDefaults()
    {
      DisplayName.SetDefault("Ninja Contract");
      Tooltip.SetDefault("Class focusing on melee and throwing damage");
    }

        public override bool UseItem(Player player)
        {
            MPlayer mplayer = player.GetModPlayer<MPlayer>(mod);
            if (mplayer.hasClass)
            {
                return true;
            }
            mplayer.hasClass = true;
            mplayer.ninja = true;
            if (player.whoAmI == Main.myPlayer)
                player.QuickSpawnItem(ItemID.Shuriken, 100);
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
