using EntityStates.Merc;
using UnityEngine.AddressableAssets;
using RoR2.Skills;

namespace HIFUMercenaryTweaks.Skills
{
    internal class Whirlwind : TweakBase
    {
        public static bool scaleDurationWithAttackSpeed;
        public override string Name => "Secondary : Whirlwind";

        public override string SkillToken => "secondary";

        public override string DescText => (Main.scaleSomeSkillDamageWithAttackSpeed.Value ? "<style=cIsDamage>Fleeting</style>. " : "") + "Quickly slice horizontally twice, dealing <style=cIsDamage>2x200% damage</style>. If airborne, slice vertically instead.";

        public override void Init()
        {
            scaleDurationWithAttackSpeed = ConfigOption(false, "Scale animation speed with Attack Speed?", "Vanilla is true");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Merc.WhirlwindBase.OnEnter += WhirlwindBase_OnEnter;
            On.EntityStates.Merc.WhirlwindAir.PlayAnim += WhirlwindAir_PlayAnim;
            On.EntityStates.Merc.WhirlwindGround.PlayAnim += WhirlwindGround_PlayAnim;
            Changes();
        }

        private void WhirlwindGround_PlayAnim(On.EntityStates.Merc.WhirlwindGround.orig_PlayAnim orig, WhirlwindGround self)
        {
            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = self.baseDuration;
            }
            orig(self);
        }

        private void WhirlwindAir_PlayAnim(On.EntityStates.Merc.WhirlwindAir.orig_PlayAnim orig, WhirlwindAir self)
        {
            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = self.baseDuration;
            }
            orig(self);
        }

        private void WhirlwindBase_OnEnter(On.EntityStates.Merc.WhirlwindBase.orig_OnEnter orig, WhirlwindBase self)
        {
            orig(self);

            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = self.baseDuration;
                self.hitPauseTimer = WhirlwindBase.hitPauseDuration;
            }

            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value && self.isAuthority)
            {
                var finalDamageCoefficient = self.overlapAttack.damage + (self.overlapAttack.damage * ((self.attackSpeedStat - 1) * (self.overlapAttack.damage / 100f)));
                self.overlapAttack.damage = finalDamageCoefficient;
            }
        }

        private void Changes()
        {
            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value)
            {
                string[] whirlwindKeywords = new string[] { "KEYWORD_FLEETING" };
                var whirlwind = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercBodyWhirlwind.asset").WaitForCompletion();
                whirlwind.keywordTokens = whirlwindKeywords;
            }
        }

        private void StartingTheCollapseWhirlwind()
        {
        }

        private void NothingCanSaveYouNooow()
        {
        }
    }
}