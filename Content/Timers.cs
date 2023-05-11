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
    }
}
