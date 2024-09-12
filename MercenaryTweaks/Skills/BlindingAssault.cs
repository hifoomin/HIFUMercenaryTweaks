using EntityStates.Merc;
using UnityEngine.AddressableAssets;
using RoR2.Skills;

namespace HIFUMercenaryTweaks.Skills
{
    internal class BlindingAssault : TweakBase<BlindingAssault>
    {
        public static bool scaleDurationWithAttackSpeed;
        public override string Name => "Utility : Blinding Assault";

        public override string SkillToken => "utility";

        public override string DescText => (Main.scaleSomeSkillDamageWithAttackSpeed.Value ? "<style=cIsDamage>Fleeting</style>. " : "") + "<style=cIsDamage>Stunning</style>. Dash forward, dealing <style=cIsDamage>300% damage</style>. If you hit an enemy, <style=cIsDamage>you can dash again</style>, up to <style=cIsDamage>3</style> total.";

        public override void Init()
        {
            scaleDurationWithAttackSpeed = ConfigOption(false, "Scale animation speed with Attack Speed?", "Vanilla is true");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Merc.PrepAssaulter2.OnEnter += PrepAssaulter2_OnEnter;
            On.EntityStates.Merc.Assaulter2.OnEnter += Assaulter2_OnEnter;
            On.EntityStates.Merc.Assaulter2.AuthorityModifyOverlapAttack += Assaulter2_AuthorityModifyOverlapAttack;
            Changes();
        }

        private void Assaulter2_AuthorityModifyOverlapAttack(On.EntityStates.Merc.Assaulter2.orig_AuthorityModifyOverlapAttack orig, Assaulter2 self, RoR2.OverlapAttack overlapAttack)
        {
            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value && self.isAuthority)
            {
                var finalDamageCoefficient = 3f + (3f * ((self.attackSpeedStat - 1) * (1 / 3f)));
                self.damageCoefficient = finalDamageCoefficient;
            }
            orig(self, overlapAttack);
        }

        private void Assaulter2_OnEnter(On.EntityStates.Merc.Assaulter2.orig_OnEnter orig, Assaulter2 self)
        {
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

        private void Changes()
        {
            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value)
            {
                string[] blindingAssaultKeywords = new string[] { "KEYWORD_FLEETING", "KEYWORD_STUNNING" };
                var blindingAssault = Addressables.LoadAssetAsync<MercDashSkillDef>("RoR2/Base/Merc/MercBodyAssaulter.asset").WaitForCompletion();
                blindingAssault.keywordTokens = blindingAssaultKeywords;
            }
        }
    }
}