using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content.Buffs;

public class Plague : ModBuff
{
	public static int DefenseReduction = 4;

	public override void SetStaticDefaults()
	{
        //DisplayName.SetDefault("Plague");
        //Description.SetDefault("Rotting from the inside");
		Main.debuff[Type] = true;
		Main.pvpBuff[Type] = true;
		Main.buffNoSave[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
    }

	public override void Update(Player player, ref int buffIndex)
	{
		player.Calamity().pFlames = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		if (npc.Calamity().pFlames < npc.buffTime[buffIndex])
		{
			npc.Calamity().pFlames = npc.buffTime[buffIndex];
		}
		npc.DelBuff(buffIndex);
		buffIndex--;
	}
}
