using HACT;
using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HIFUAcridTweaks.Skills
{
    internal class FrenziedLeap : TweakBase
    {
        public static float damage;
        public static float cdr;
        public static float cooldown;
        public override string Name => "Utility :: Frenzied Leap";

        public override string SkillToken => "utility_alt1";

        public override string DescText => "<style=cIsDamage>Stunning</style>. Leap in the air, dealing <style=cIsDamage>" + d(damage) + " damage</style>. <style=cIsUtility>Reduce</style> the cooldown by <style=cIsUtility>" + cdr + "s</style> for every enemy hit.";

        public override void Init()
        {
            damage = ConfigOption(5f, "Damage", "Decimal. Vanilla is 5.5");
            cooldown = ConfigOption(8f, "Cooldown", "Vanilla is 10");
            cdr = ConfigOption(1.5f, "Cooldown Reduction Per Hit", "Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Croco.BaseLeap.OnEnter += BaseLeap_OnEnter;
            On.EntityStates.Croco.BaseLeap.OnExit += BaseLeap_OnExit;
            On.EntityStates.Croco.ChainableLeap.DoImpactAuthority += ChainableLeap_DoImpactAuthority;
            Changes();
        }

        private void BaseLeap_OnExit(On.EntityStates.Croco.BaseLeap.orig_OnExit orig, EntityStates.Croco.BaseLeap self)
        {
            orig(self);
            self.characterBody.isSprinting = true;
        }

        private void ChainableLeap_DoImpactAuthority(On.EntityStates.Croco.ChainableLeap.orig_DoImpactAuthority orig, EntityStates.Croco.ChainableLeap self)
        {
            EntityStates.Croco.ChainableLeap.refundPerHit = cdr;
            orig(self);
        }

        private void BaseLeap_OnEnter(On.EntityStates.Croco.BaseLeap.orig_OnEnter orig, EntityStates.Croco.BaseLeap self)
        {
            if (self is EntityStates.Croco.ChainableLeap)
            {
                self.blastDamageCoefficient = damage;
            }
            orig(self);
        }

        private void Changes()
        {
            var fleap = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoChainableLeap.asset").WaitForCompletion();
            fleap.baseRechargeInterval = cooldown;
        }
    }
}