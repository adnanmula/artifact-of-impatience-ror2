using System;
using BepInEx;
using R2API.Utils;
using RoR2;

namespace NoShieldElitesMod
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.nan.noshieldelites", "No shield elites", "0.0.1")]

    public class NoShieldElitesPlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.CombatDirector.PrepareNewMonsterWave += OnPrepareNewMonsterWave;
        }

        private static void OnPrepareNewMonsterWave(On.RoR2.CombatDirector.orig_PrepareNewMonsterWave orig, CombatDirector self, DirectorCard monsterCard)
        {
            CombatDirector.EliteTierDef[] eliteTiers = self.GetFieldValue<CombatDirector.EliteTierDef[]>("eliteTiers");

            foreach (CombatDirector.EliteTierDef tier in eliteTiers)
            {
                tier.SetFieldValue(
                    "eliteTypes",
                    Array.FindAll(tier.eliteTypes, (EliteIndex index) => index != EliteIndex.Lightning)
                );
            }

            self.SetFieldValue<CombatDirector.EliteTierDef[]>("eliteTiers", eliteTiers);

            orig(self, monsterCard);
        }
    }
}
