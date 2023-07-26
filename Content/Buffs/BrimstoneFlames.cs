using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content.Buffs;

public class BrimstoneFlames : ModBuff
{
	public override void SetStaticDefaults()
	{
        //DisplayName.SetDefault("Brimstone Flames");
        //Description.SetDefault("Rapid health loss");
		Main.debuff[Type] = true;
		Main.pvpBuff[Type] = true;
		Main.buffNoSave[Type] = true;
		BuffID.Sets.LongerExpertDebuff[Type] = false;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.Calamity().bFlames = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		if (npc.Calamity().bFlames < npc.buffTime[buffIndex])
		{
			npc.Calamity().bFlames = npc.buffTime[buffIndex];
		}
		npc.DelBuff(buffIndex);
		buffIndex--;
	}
}
