﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RPG.Items
{
    public class WerewolfContract : ModItem
    {
        public override void SetDefaults()
        {


            item.consumable = true;
            item.useStyle = 2;
        }

    public override void SetStaticDefaults()
    {
      DisplayName.SetDefault("Werewolf Contract");
      Tooltip.SetDefault("Class granting melee bonuses at night");
    }

        public override bool UseItem(Player player)
        {
            MPlayer mplayer = player.GetModPlayer<MPlayer>(mod);
            if (mplayer.hasClass)
            {
                return true;
            }
            mplayer.hasClass = true;
            mplayer.werewolf = true;
            if (player.whoAmI == Main.myPlayer)
                player.QuickSpawnItem(ItemID.Shackle);
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
