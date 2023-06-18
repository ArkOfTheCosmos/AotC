using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;

namespace AotC.Content.Sounds;

internal class AotCAudio
{
    public static readonly SoundStyle ChaosBlasterFire;
    public static readonly SoundStyle Slash;
    public static readonly SoundStyle Bell;
    public static readonly SoundStyle BloomingBlows;

    static AotCAudio()
    {
        ChaosBlasterFire = new SoundStyle("AotC/Assets/Sounds/ChaosBlasterFire", (SoundType)0);
        Slash = new SoundStyle("AotC/Assets/Sounds/Slash", (SoundType)0);
        Bell = new SoundStyle("AotC/Assets/Sounds/Bell", (SoundType)0);
        BloomingBlows = new SoundStyle("AotC/Assets/Sounds/BloomingBlows", 3, (SoundType)0)
        {
            Volume = 0.4f
        };
        
    }
}
