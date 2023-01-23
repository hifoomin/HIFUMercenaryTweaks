using HMT;
using EntityStates.Merc;

namespace HIFUMercenaryTweaks.Skills
{
    internal class BlindingAssault : TweakBase
    {
        public static bool scaleDurationWithAttackSpeed;
        public override string Name => "Utility : Blinding Assault";

        public override string SkillToken => "utility";

        public override string DescText => "<style=cIsDamage>Stunning</style>. Dash forward, dealing <style=cIsDamage>300% damage</style>. If you hit an enemy, <style=cIsDamage>you can dash again</style>, up to <style=cIsDamage>3</style> total." +
                                           (scaleDurationWithAttackSpeed ? "" : " <style=cIsUtility>Skill damage scales with attack speed</style>.");

        public override void Init()
        {
            scaleDurationWithAttackSpeed = ConfigOption(false, "Scale animation speed with Attack Speed?", "Vanilla is true");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Merc.PrepAssaulter2.OnEnter += PrepAssaulter2_OnEnter;
            On.EntityStates.Merc.Assaulter2.OnEnter += Assaulter2_OnEnter;
        }

        private void Assaulter2_OnEnter(On.EntityStates.Merc.Assaulter2.orig_OnEnter orig, Assaulter2 self)
        {
            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value)
            {
                var finalDamageCoefficient = self.damageCoefficient + (self.damageCoefficient * ((self.attackSpeedStat - 1) * (1 / self.damageCoefficient)));
                self.damageCoefficient = finalDamageCoefficient;
            }

            orig(self);

            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = self.baseDuration;
            }
        }

        private void PrepAssaulter2_OnEnter(On.EntityStates.Merc.PrepAssaulter2.orig_OnEnter orig, PrepAssaulter2 self)
        {
            orig(self);

            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = PrepAssaulter2.baseDuration;
            }
        }
    }
}