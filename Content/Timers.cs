using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using AotC.Content.StolenCalamityCode;
using Terraria.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using AotC.Content.Items.Weapons;
using System.IO;
using Terraria.DataStructures;

namespace AotC.Content
{
    internal class Timers : ModPlayer
    {
        public float ArkThrowCooldown;

        public override void UpdateBadLifeRegen()
        {
            ArkThrowCooldown--;
            if (ArkThrowCooldown == 0)
            {
                SoundStyle style = SoundID.Item35;
                style.Volume = SoundID.Item35.Volume * 2f;
                SoundEngine.PlaySound(in Sounds.AotCAudio.Bell);
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            //this is to draw the cooldown

            Player owner = Main.player[Player.whoAmI];
            if (ArkThrowCooldown >= 0)
            {
                float Timer = 340 - ArkThrowCooldown;
                float ParryTime = 0;

                Texture2D value = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarBack", (AssetRequestMode)2).Value;
                Texture2D value2 = ModContent.Request<Texture2D>("AotC/Assets/Textures/GenericBarFront", (AssetRequestMode)2).Value;
                Vector2 val = owner.Center - Main.screenPosition + new Vector2(0f, 36f) - value.Size() / 2f;
                Rectangle value3 = new(0, 0, (int)((Timer - ParryTime) / (340f - ParryTime) * (float)value2.Width), value2.Height);
                float num = ((Timer <= ParryTime + 25f) ? ((Timer - ParryTime) / 25f) : ((340f - Timer <= 8f) ? (ArkThrowCooldown / 8f) : 1f));
                Color val2 = CalamityUtils.HsvToRgb(Main.GlobalTimeWrappedHourly, 1f, 1f);
                Main.spriteBatch.Draw(value, val, val2 * num);
                Main.spriteBatch.Draw(value2, val, (Rectangle?)value3, val2 * num * 0.8f);
            }



            /*Texture2D value = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GenericBarBack", (AssetRequestMode)2).get_Value();
            Texture2D value2 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GenericBarFront", (AssetRequestMode)2).get_Value();
            Vector2 val = Owner.Center - Main.screenPosition + new Vector2(0f, -36f) - value.Size() / 2f;
            Rectangle value3 = default(Rectangle);
            ((Rectangle)(ref value3))._002Ector(0, 0, (int)((Timer - ParryTime) / (340f - ParryTime) * (float)value2.get_Width()), value2.get_Height());
            float num = ((Timer <= ParryTime + 25f) ? ((Timer - ParryTime) / 25f) : ((340f - Timer <= 8f) ? ((float)base.Projectile.timeLeft / 8f) : 1f));
            Color val2 = Main.hslToRgb(Main.GlobalTimeWrappedHourly * 0.6f % 1f, 1f, 0.85f + (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * 3f)) * 0.1f);
            Main.spriteBatch.Draw(value, val, val2 * num);
            Main.spriteBatch.Draw(value2, val, (Rectangle?)value3, val2 * num * 0.8f);*/
        }
    }
}
