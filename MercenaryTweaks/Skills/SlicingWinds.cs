using UnityEngine.AddressableAssets;
using RoR2.Skills;
using UnityEngine;

namespace HIFUMercenaryTweaks.Skills
{
    public class SlicingWinds : TweakBase<SlicingWinds>
    {
        public override string Name => "Special :: Slicing Winds";

        public override string SkillToken => "special_alt1";

        public override string DescText => "Fire a wind of blades that attack up to <style=cIsDamage>3</style> enemies for <style=cIsDamage>8x100% damage</style>. The last hit <style=cIsUtility>Exposes</style> enemies.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var slicingWinds = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/EvisProjectile.prefab").WaitForCompletion();
            slicingWinds.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        }
    }
}