using HMT;
using EntityStates.Merc;

namespace HIFUMercenaryTweaks.Skills
{
    internal class Whirlwind : TweakBase
    {
        public static bool scaleDurationWithAttackSpeed;
        public override string Name => "Secondary : Whirlwind";

        public override string SkillToken => "secondary";

        public override string DescText => "Quickly slice horizontally twice, dealing <style=cIsDamage>2x200% damage</style>. If airborne, slice vertically instead." +
                                           (scaleDurationWithAttackSpeed ? "" : " <style=cIsUtility>Skill damage scales with attack speed</style>.");

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
        }

        private void WhirlwindGround_PlayAnim(On.EntityStates.Merc.WhirlwindGround.orig_PlayAnim orig, WhirlwindGround self)
        {
            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = self.baseDuration;
            }
            orig(self);
            /*
            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value)
            {
                var finalDamageCoefficient = self.overlapAttack.damage + (self.overlapAttack.damage * ((self.attackSpeedStat - 1) * 0.76923077f));
                self.overlapAttack.damage = finalDamageCoefficient;
            }
            */
        }

        private void WhirlwindAir_PlayAnim(On.EntityStates.Merc.WhirlwindAir.orig_PlayAnim orig, WhirlwindAir self)
        {
            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = self.baseDuration;
            }
            orig(self);
            /*
            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value)
            {
                var finalDamageCoefficient = self.overlapAttack.damage + (self.overlapAttack.damage * ((self.attackSpeedStat - 1) * 0.76923077f));
                self.overlapAttack.damage = finalDamageCoefficient;
            }
            */
        }

        private void WhirlwindBase_OnEnter(On.EntityStates.Merc.WhirlwindBase.orig_OnEnter orig, WhirlwindBase self)
        {
            orig(self);

            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = self.baseDuration;
                self.hitPauseTimer = WhirlwindBase.hitPauseDuration;
            }

            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value)
            {
                var finalDamageCoefficient = self.overlapAttack.damage + (self.overlapAttack.damage * ((self.attackSpeedStat - 1) * (self.overlapAttack.damage / 100f)));
                self.overlapAttack.damage = finalDamageCoefficient;
            }
        }
    }
}