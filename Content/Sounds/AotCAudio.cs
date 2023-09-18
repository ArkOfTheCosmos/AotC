using Terraria.Audio;

namespace AotC.Content.Sounds;

internal class AotCAudio
{
    public static readonly SoundStyle ChaosBlasterFire;
    public static readonly SoundStyle Slash;
    public static readonly SoundStyle Bell;
    public static readonly SoundStyle BloomingBlows;
    public static readonly SoundStyle BloomingBlowsCrit;
    public static readonly SoundStyle KillerWail;
    public static readonly SoundStyle MeatySlash;
    public static readonly SoundStyle Dash;
    public static readonly SoundStyle PlasmaShrimp;

    static AotCAudio()
    {
        ChaosBlasterFire = new SoundStyle("AotC/Assets/Sounds/ChaosBlasterFire", (SoundType)0);
        Slash = new SoundStyle("AotC/Assets/Sounds/Slash", (SoundType)0);
        Bell = new SoundStyle("AotC/Assets/Sounds/Bell", (SoundType)0);
        KillerWail = new SoundStyle("AotC/Assets/Sounds/KillerWail", (SoundType)0);
        MeatySlash = new SoundStyle("AotC/Assets/Sounds/MeatySlash", (SoundType)0);
        PlasmaShrimp = new SoundStyle("AotC/Assets/Sounds/PlasmaShrimp", (SoundType)0);
        Dash = new SoundStyle("AotC/Assets/Sounds/Dash", 2, (SoundType)0);
        BloomingBlows = new SoundStyle("AotC/Assets/Sounds/BloomingBlows", 3, (SoundType)0)
        {
            Volume = 0.4f
        };
        BloomingBlowsCrit = new SoundStyle("AotC/Assets/Sounds/BloomingBlowsCrit", 3, (SoundType)0)
        {
            Volume = 0.4f
        };
    }
}
