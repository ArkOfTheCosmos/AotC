using Terraria.Audio;

namespace AotC.Content.Sounds;

internal class AotCAudio
{
    public static readonly SoundStyle ChaosBusterLaser;
    public static readonly SoundStyle Slash;
    public static readonly SoundStyle Bell;
    public static readonly SoundStyle BloomingBlows;
    public static readonly SoundStyle BloomingBlowsCrit;
    public static readonly SoundStyle KillerWail;
    public static readonly SoundStyle MeatySlash;
    public static readonly SoundStyle Dash;
    public static readonly SoundStyle PlasmaShrimp;
    public static readonly SoundStyle PlimpRecharge;
    public static readonly SoundStyle ChaosBusterTarget;
    public static readonly SoundStyle ChaosBusterShoot;

    static AotCAudio()
    {
        ChaosBusterLaser = new SoundStyle("AotC/Assets/Sounds/ChaosBusterLaser", (SoundType)0);
        Slash = new SoundStyle("AotC/Assets/Sounds/Slash", (SoundType)0);
        Bell = new SoundStyle("AotC/Assets/Sounds/Bell", (SoundType)0);
        KillerWail = new SoundStyle("AotC/Assets/Sounds/KillerWail", (SoundType)0);
        MeatySlash = new SoundStyle("AotC/Assets/Sounds/MeatySlash", (SoundType)0);
        PlasmaShrimp = new SoundStyle("AotC/Assets/Sounds/PlasmaShrimp", (SoundType)0);
        PlimpRecharge = new SoundStyle("AotC/Assets/Sounds/PlimpRecharge", (SoundType)0);
        ChaosBusterTarget= new SoundStyle("AotC/Assets/Sounds/ChaosBusterTarget", (SoundType)0);

        Dash = new SoundStyle("AotC/Assets/Sounds/Dash", 2, (SoundType)0);
        ChaosBusterShoot = new SoundStyle("AotC/Assets/Sounds/ChaosBusterShoot", (SoundType)0)
        {
            Volume = 0.4f
        };
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
