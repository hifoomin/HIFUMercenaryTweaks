using UnityEngine.AddressableAssets;
using UnityEngine;

namespace HIFUMercenaryTweaks.Skills
{
    public class SlicingWinds : TweakBase<SlicingWinds>
    {
        public static float damage;
        public override string Name => "Special :: Slicing Winds";

        public override string SkillToken => "special_alt1";

        public override string DescText => "Fire a wind of blades that attack up to <style=cIsDamage>3</style> enemies for <style=cIsDamage>8x" + d(damage) + " damage</style>. The last hit <style=cIsUtility>Exposes</style> enemies.";

        public override void Init()
        {
            damage = ConfigOption(1f, "Damage", "Decimal. Vanilla is 1");
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
            slicingWinds.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        }
    }
}