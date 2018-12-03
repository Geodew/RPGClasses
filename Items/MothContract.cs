﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RPG.Items
{
    public class MothContract : ModItem
    {
        public override void SetDefaults()
        {
            item.consumable = true;
            item.useStyle = 2;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moth Contract");
            Tooltip.SetDefault("Class focusing on DoT effects, grants wings");
        }

        public override bool UseItem(Player player)
        {
            MPlayer mplayer = player.GetModPlayer<MPlayer>(mod);
            if (mplayer.hasClass)
            {
                return true;
            }

            mplayer.hasClass = true;
            mplayer.moth = true;
            if (player.whoAmI == Main.myPlayer)
            {
                player.QuickSpawnItem(ItemID.FlaskofPoison);
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
