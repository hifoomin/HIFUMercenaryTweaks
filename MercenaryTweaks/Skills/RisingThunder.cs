﻿using EntityStates.Merc;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using System.Collections.Generic;

namespace HIFUMercenaryTweaks.Skills
{
    internal class RisingThunder : TweakBase<RisingThunder>
    {
        public static bool scaleDurationWithAttackSpeed;
        public static float upwardForce;
        public static float damageCoefficient;
        public static float duration;
        public static bool agile;

        public override string Name => "Secondary :: Rising Thunder";

        public override string SkillToken => "secondary_alt1";

        public override string DescText => (Main.scaleSomeSkillDamageWithAttackSpeed.Value ? "<style=cIsDamage>Fleeting</style>. " : "") + (agile ? "<style=cIsUtility>Agile</style>. " : "") + "Unleash a slicing uppercut, dealing <style=cIsDamage>" + d(damageCoefficient) + " damage</style> and sending you airborne.";

        public override void Init()
        {
            scaleDurationWithAttackSpeed = ConfigOption(false, "Scale animation speed with Attack Speed?", "Vanilla is true");
            upwardForce = ConfigOption(2000f, "Upward Boost Strength", "Vanilla is 3000");
            damageCoefficient = ConfigOption(6f, "Damage", "Decimal. Vanilla is 5.5");
            duration = ConfigOption(0.25f, "Base Duration", "Vanilla is 0.4");
            agile = ConfigOption(true, "Agile?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Merc.Uppercut.OnEnter += Uppercut_OnEnter;
            On.EntityStates.Merc.Uppercut.PlayAnim += Uppercut_PlayAnim;
            Changes();
        }

        private void Uppercut_PlayAnim(On.EntityStates.Merc.Uppercut.orig_PlayAnim orig, Uppercut self)
        {
            Uppercut.baseDuration = duration;
            if (scaleDurationWithAttackSpeed == false)
            {
                self.duration = Uppercut.baseDuration;
            }
            orig(self);
        }

        private void Uppercut_OnEnter(On.EntityStates.Merc.Uppercut.orig_OnEnter orig, Uppercut self)
        {
            Uppercut.upwardForceStrength = upwardForce;
            Uppercut.baseDamageCoefficient = damageCoefficient;

            orig(self);

            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value && self.isAuthority)
            {
                var finalDamageCoefficient = self.overlapAttack.damage + (self.overlapAttack.damage * ((self.attackSpeedStat - 1) * (self.overlapAttack.damage / 432f)));
                self.overlapAttack.damage = finalDamageCoefficient;
            }
        }

        private void Changes()
        {
            List<string> risingThunderKeywords = new();
            var risingThunder = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercBodyUppercut.asset").WaitForCompletion();
            if (Main.scaleSomeSkillDamageWithAttackSpeed.Value)
            {
                risingThunderKeywords.Add("KEYWORD_FLEETING");
            }
            if (agile)
            {
                risingThunderKeywords.Add("KEYWORD_AGILE");
                risingThunder.cancelSprintingOnActivation = false;
            }
            risingThunder.keywordTokens = risingThunderKeywords.ToArray();
        }
    }
}