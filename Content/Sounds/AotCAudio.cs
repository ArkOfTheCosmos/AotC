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

    static AotCAudio()
    {
        ChaosBlasterFire = new SoundStyle("AotC/Assets/Sounds/ChaosBlasterFire", (SoundType)0);
        Slash = new SoundStyle("AotC/Assets/Sounds/Slash", (SoundType)0);
        Bell = new SoundStyle("AotC/Assets/Sounds/Bell", (SoundType)0);
        
    }
}
