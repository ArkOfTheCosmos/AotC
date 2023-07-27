using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content.Buffs;

public class HolyFlames : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.pvpBuff[Type] = true;
		Main.buffNoSave[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
    }

	public override void Update(Player player, ref int buffIndex)
	{
		player.Calamity().hFlames = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		if (npc.Calamity().hFlames < npc.buffTime[buffIndex])
		{
			npc.Calamity().hFlames = npc.buffTime[buffIndex];
		}
		npc.DelBuff(buffIndex);
		buffIndex--;
	}
}
