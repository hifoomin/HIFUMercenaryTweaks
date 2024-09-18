using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2.Skills;
using System.Collections.Generic;

namespace HIFUMercenaryTweaks.Skills
{
    public class SlicingWinds : TweakBase<SlicingWinds>
    {
        public static float damage;
        public static bool agile;
        public static float sizeMultiplier;

        public override string Name => "Special :: Slicing Winds";

        public override string SkillToken => "special_alt1";

        public override string DescText => (agile ? "<style=cIsUtility>Agile</style>. " : "") + "Fire a wind of blades that attack up to <style=cIsDamage>3</style> enemies for <style=cIsDamage>8x" + d(damage) + " damage</style>. The last hit <style=cIsUtility>Exposes</style> enemies.";

        public override void Init()
        {
            damage = ConfigOption(1f, "Damage", "Decimal. Vanilla is 1");
            agile = ConfigOption(true, "Agile?", "Vanilla is false");
            sizeMultiplier = ConfigOption(2.5f, "Projectile Size Multiplier", "Makes the projectile easier to hit (as a result, it can get stuck on terrain easier). Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Merc.Weapon.ThrowEvisProjectile.OnEnter += ThrowEvisProjectile_OnEnter;
            Changes();
        }

        private void ThrowEvisProjectile_OnEnter(On.EntityStates.Merc.Weapon.ThrowEvisProjectile.orig_OnEnter orig, EntityStates.Merc.Weapon.ThrowEvisProjectile self)
        {
            self.damageCoefficient = damage;
            orig(self);
        }

        private void Changes()
        {
            var slicingWinds = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/EvisProjectile.prefab").WaitForCompletion();
            slicingWinds.transform.localScale = Vector3.one * sizeMultiplier;
            var swsd = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercBodyEvisProjectile.asset").WaitForCompletion();
            List<string> keywordTokens = new();

            if (agile)
            {
                swsd.cancelSprintingOnActivation = false;
                keywordTokens.Add("KEYWORD_AGILE");
            }

            keywordTokens.Add("KEYWORD_EXPOSE");
            swsd.keywordTokens = keywordTokens.ToArray();
        }
    }
}