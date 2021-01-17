using System;
using BepInEx;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace ArtifactOfImpatience
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.adnanmula.artifactofimpatience", "Artifact of Impatience", "0.0.1")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    public class ArtifactOfImpatiencePlugin : BaseUnityPlugin
    {
        public static ArtifactDef Artifact;

        public void Awake()
        {
            Artifact = ScriptableObject.CreateInstance<ArtifactDef>();
            Artifact.nameToken = "Artifact of Impatience";
            Artifact.descriptionToken = "Removes lighting elites.";
            Artifact.smallIconSelectedSprite = LoadSprite(Properties.Resources.artifact_enabled);
            Artifact.smallIconDeselectedSprite = LoadSprite(Properties.Resources.artifact_disabled);

            ArtifactCatalog.getAdditionalEntries += (list) => {
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

        private static Sprite LoadSprite(Byte[] resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            Texture2D texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
            texture.LoadImage(resource, false);

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)); ;
        }
    }
}
