using HMT;
using EntityStates.Merc.Weapon;
using UnityEngine.AddressableAssets;
using RoR2.Skills;

namespace HIFUMercenaryTweaks.Skills
{
    internal class LaserSword : TweakBase
    {
        public static bool scaleDurationWithAttackSpeed;
        public override string Name => "Primary : Laser Sword";

        public override string SkillToken => "primary";

        public override string DescText => (Main.scaleSomeSkillDamageWithAttackSpeed.Value ? "<style=cIsDamage>Fleeting</style>. " : "") + "<style=cIsUtility>Agile</style>. Slice in front for <style=cIsDamage>130% damage</style>. Every 3rd hit strikes in a greater area and <style=cIsUtility>Exposes</style> enemies.";

        public override void Init()
        {
            scaleDurationWithAttackSpeed = ConfigOption(false, "Scale animation speed with Attack Speed?", "Vanilla is true");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Merc.Weapon.GroundLight2.OnEnter += GroundLight2_OnEnter;
            Changes();
        }

        private void GroundLight2_OnEnter(On.EntityStates.Merc.Weapon.GroundLight2.orig_OnEnter orig, GroundLight2 self)
        {
            orig(self);

            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = self.baseDuration;
                self.durationBeforeInterruptable = self.isComboFinisher ? GroundLight2.comboFinisherBaseDurationBeforeInterruptable : GroundLight2.baseDurationBeforeInterruptable;
                self.ignoreAttackSpeed = true;
                self.scaleHitPauseDurationAndVelocityWithAttackSpeed = false;
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
                string[] laserSwordKeywords = new string[] { "KEYWORD_FLEETING", "KEYWORD_AGILE" };
                var laserSword = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercGroundLight2.asset").WaitForCompletion();
                laserSword.keywordTokens = laserSwordKeywords;
            }
        }
    }
}