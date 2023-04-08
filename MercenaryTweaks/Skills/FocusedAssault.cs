using EntityStates.Merc;
using UnityEngine.AddressableAssets;
using RoR2.Skills;

namespace HIFUMercenaryTweaks.Skills
{
    internal class FocusedAssault : TweakBase
    {
        public static bool scaleDurationWithAttackSpeed;
        public override string Name => "Utility :: Focused Assault";

        public override string SkillToken => "utility_alt1";

        public override string DescText => (Main.scaleSomeSkillDamageWithAttackSpeed.Value ? "<style=cIsDamage>Fleeting</style>. " : "") + "<style=cIsDamage>Stunning</style>. Dash forward, dealing <style=cIsDamage>700% damage</style> and <style=cIsUtility>Exposing</style> enemies after <style=cIsUtility>1 second</style>.";

        public override void Init()
        {
            scaleDurationWithAttackSpeed = ConfigOption(false, "Scale animation speed with Attack Speed?", "Vanilla is true");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Merc.FocusedAssaultPrep.OnEnter += FocusedAssaultPrep_OnEnter;
            On.EntityStates.Merc.FocusedAssaultDash.AuthorityModifyOverlapAttack += FocusedAssaultDash_AuthorityModifyOverlapAttack;
            On.EntityStates.Merc.FocusedAssaultDash.OnEnter += FocusedAssaultDash_OnEnter;
            Changes();
        }

        private void FocusedAssaultDash_AuthorityModifyOverlapAttack(On.EntityStates.Merc.FocusedAssaultDash.orig_AuthorityModifyOverlapAttack orig, FocusedAssaultDash self, RoR2.OverlapAttack overlapAttack)
        {
            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value && self.isAuthority)
            {
                var finalDamageCoefficient = 7f + (7f * ((self.attackSpeedStat - 1) * 0.4285714f));
                self.delayedDamageCoefficient = finalDamageCoefficient;
            }

            orig(self, overlapAttack);
        }

        private void FocusedAssaultDash_OnEnter(On.EntityStates.Merc.FocusedAssaultDash.orig_OnEnter orig, FocusedAssaultDash self)
        {
            orig(self);

            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = self.baseDuration;
            }
        }

        private void FocusedAssaultPrep_OnEnter(On.EntityStates.Merc.FocusedAssaultPrep.orig_OnEnter orig, FocusedAssaultPrep self)
        {
            orig(self);

            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = self.baseDuration;
            }
        }

        private void Changes()
        {
            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value)
            {
                string[] focusedAssaultKeywords = new string[] { "KEYWORD_FLEETING", "KEYWORD_STUNNING" };
                var focusedAssault = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercBodyFocusedAssault.asset").WaitForCompletion();
                focusedAssault.keywordTokens = focusedAssaultKeywords;
            }
        }
    }
}