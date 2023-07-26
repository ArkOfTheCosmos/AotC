using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content.Buffs;

public class TemporalSadness : ModBuff
{
	public override void SetStaticDefaults()
	{
		//DisplayName.SetDefault("Temporal Sadness");
		//Description.SetDefault("You are crying");
		Main.debuff[Type] = true;
		Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
		BuffID.Sets.LongerExpertDebuff[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		if (npc.Calamity().tSad < npc.buffTime[buffIndex])
		{
			npc.Calamity().tSad = npc.buffTime[buffIndex];
		}
		npc.DelBuff(buffIndex);
		buffIndex--;
	}
}
