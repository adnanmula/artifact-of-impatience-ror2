using System;
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace ArtifactOfImpatience
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.nan.artifactofimpatience", "Artifact of Impatience", "0.0.1")]
    [R2APISubmoduleDependency(nameof(LoadoutAPI))]

    public class ArtifactOfImpatiencePlugin : BaseUnityPlugin
    {
        public static ArtifactDef Artifact;

        public void Awake()
        {
            Artifact = ScriptableObject.CreateInstance<ArtifactDef>();
            Artifact.nameToken = "Artifact of Impatience";
            Artifact.descriptionToken = "Removes lighting elites.";
            Artifact.smallIconDeselectedSprite = LoadoutAPI.CreateSkinIcon(Color.white, Color.white, Color.white, Color.white);
            Artifact.smallIconSelectedSprite = LoadoutAPI.CreateSkinIcon(Color.black, Color.blue, Color.black, Color.blue);
            
            ArtifactCatalog.getAdditionalEntries += (list) =>
            {
                list.Add(Artifact);
            };

            On.RoR2.CombatDirector.PrepareNewMonsterWave += OnPrepareNewMonsterWave;
        }

        private static void OnPrepareNewMonsterWave(On.RoR2.CombatDirector.orig_PrepareNewMonsterWave orig, CombatDirector self, DirectorCard monsterCard)
        {
            if (false == RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfImpatiencePlugin.Artifact))
            {
                orig(self, monsterCard);

                return;
            }

            CombatDirector.EliteTierDef[] eliteTiers = self.GetFieldValue<CombatDirector.EliteTierDef[]>("eliteTiers");

            foreach (CombatDirector.EliteTierDef tier in eliteTiers)
            {
                tier.SetFieldValue(
                    "eliteTypes",
                    Array.FindAll(tier.eliteTypes, (EliteIndex index) => index != EliteIndex.Lightning)
                );
            }

            self.SetFieldValue("eliteTiers", eliteTiers);

            orig(self, monsterCard);
        }
    }
}
