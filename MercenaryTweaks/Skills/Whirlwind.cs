using EntityStates.Merc;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using System.Collections.Generic;

namespace HIFUMercenaryTweaks.Skills
{
    internal class Whirlwind : TweakBase<Whirlwind>
    {
        public static bool scaleDurationWithAttackSpeed;
        public static bool agile;
        public static float groundedSpeedCoefficient;
        public static float airborneSpeedCoefficient;

        public override string Name => "Secondary : Whirlwind";

        public override string SkillToken => "secondary";

        public override string DescText => (Main.scaleSomeSkillDamageWithAttackSpeed.Value ? "<style=cIsDamage>Fleeting</style>. " : "") + (agile ? "<style=cIsUtility>Agile</style>. " : "") + "Quickly slice horizontally twice, dealing <style=cIsDamage>2x200% damage</style>. If airborne, slice vertically instead.";

        public override void Init()
        {
            scaleDurationWithAttackSpeed = ConfigOption(false, "Scale animation speed with Attack Speed?", "Vanilla is true");
            agile = ConfigOption(true, "Agile?", "Vanilla is false");
            groundedSpeedCoefficient = ConfigOption(6f, "Grounded Speed Coefficient", "Vanilla is 8. This is a compensation for it being Agile and going farther than usual with the default value.");
            airborneSpeedCoefficient = ConfigOption(3f, "Airborne Speed Coefficient", "Vanilla is 3.");
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
            self.moveSpeedBonusCoefficient = groundedSpeedCoefficient;
            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = self.baseDuration;
            }
            orig(self);
        }

        private void WhirlwindAir_PlayAnim(On.EntityStates.Merc.WhirlwindAir.orig_PlayAnim orig, WhirlwindAir self)
        {
            self.moveSpeedBonusCoefficient = airborneSpeedCoefficient;
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
            List<string> whirlwindKeywords = new();
            var whirlwind = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercBodyWhirlwind.asset").WaitForCompletion();
            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value)
            {
                whirlwindKeywords.Add("KEYWORD_FLEETING");
            }
            if (agile)
            {
                whirlwind.cancelSprintingOnActivation = false;
                whirlwindKeywords.Add("KEYWORD_AGILE");
            }

            whirlwind.keywordTokens = whirlwindKeywords.ToArray();
        }

        private void StartingTheCollapseWhirlwind()
        {
        }
    }
}