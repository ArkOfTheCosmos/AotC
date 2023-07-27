using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content.Buffs;

public class GlacialState : ModBuff
{
	public static int DefenseReduction = 10;

	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.pvpBuff[Type] = true;
		Main.buffNoSave[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
    }

	public override void Update(Player player, ref int buffIndex)
	{
		player.Calamity().gState = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		if (npc.Calamity().gState < npc.buffTime[buffIndex])
		{
			npc.Calamity().gState = npc.buffTime[buffIndex];
		}
		npc.DelBuff(buffIndex);
		buffIndex--;
	}
}
