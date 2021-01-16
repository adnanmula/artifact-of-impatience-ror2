using System;
using BepInEx;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace NoShieldElitesMod
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.nan.noshieldelites", "No shield elites", "0.0.1")]

    public class NoShiledElitesPlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.CombatDirector.PrepareNewMonsterWave += CombatDirectorOnPrepareNewMonsterWave;
        }

        private static void CombatDirectorOnPrepareNewMonsterWave(On.RoR2.CombatDirector.orig_PrepareNewMonsterWave orig, CombatDirector self, DirectorCard monsterCard)
        {
            float costMultiplier = self.GetFieldValue<float>("baseEliteCostMultiplier");
            float damageBoostCoefficient = self.GetFieldValue<float>("baseEliteDamageBoostCoefficient");
            float healthBoostCoefficient = self.GetFieldValue<float>("baseEliteHealthBoostCoefficient");

            CombatDirector.EliteTierDef[] elDef = {
                new CombatDirector.EliteTierDef
                {
                    costMultiplier = 1f,
                    damageBoostCoefficient = 1f,
                    healthBoostCoefficient = 1f,
                    eliteTypes = new EliteIndex[]
                    {
                        EliteIndex.None
                    },
                    isAvailable = new Func<bool>(NotEliteOnlyArtifactActive)
                },
                new CombatDirector.EliteTierDef
                {
                    costMultiplier = costMultiplier,
                    damageBoostCoefficient = damageBoostCoefficient,
                    healthBoostCoefficient = healthBoostCoefficient,
                    eliteTypes = new EliteIndex[]
                    {
                        EliteIndex.Fire,
                        //EliteIndex.Lightning,
                        EliteIndex.Ice
                    },
                    isAvailable = new Func<bool>(NotEliteOnlyArtifactActive)
                },
                new CombatDirector.EliteTierDef
                {
                    costMultiplier = Mathf.LerpUnclamped(1f, costMultiplier, 0.5f),
                    damageBoostCoefficient = Mathf.LerpUnclamped(1f, damageBoostCoefficient, 0.5f),
                    healthBoostCoefficient = Mathf.LerpUnclamped(1f, healthBoostCoefficient, 0.5f),
                    eliteTypes = new EliteIndex[]
                    {
                        EliteIndex.Fire,
                        EliteIndex.Lightning,
                        EliteIndex.Ice
                    },
                    isAvailable = new Func<bool>(NotEliteOnlyArtifactActive)
                },
                new CombatDirector.EliteTierDef
                {
                    costMultiplier = costMultiplier * 6f,
                    damageBoostCoefficient = damageBoostCoefficient * 3f,
                    healthBoostCoefficient = healthBoostCoefficient * 4.5f,
                    eliteTypes = new EliteIndex[]
                    {
                        EliteIndex.Poison,
                        EliteIndex.Haunted
                    },
                    isAvailable = (() => Run.instance.loopClearCount > 0)
                }
            };

            self.SetFieldValue<CombatDirector.EliteTierDef[]>("eliteTiers", elDef);

            orig(self, monsterCard);
        }

        private static bool NotEliteOnlyArtifactActive()
        {
            return false == RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef);
        }
    }
}

