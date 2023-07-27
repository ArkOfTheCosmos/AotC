using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content.Buffs;

public class ArmorCrunch : ModBuff
{
	public static int DefenseReduction = 15;

	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.pvpBuff[Type] = true;
		Main.buffNoSave[Type] = true;
		BuffID.Sets.LongerExpertDebuff[Type] = false;
		BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		if (npc.Calamity().aCrunch < npc.buffTime[buffIndex])
		{
			npc.Calamity().aCrunch = npc.buffTime[buffIndex];
		}
		npc.DelBuff(buffIndex);
		buffIndex--;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.Calamity().aCrunch = true;
	}
}
