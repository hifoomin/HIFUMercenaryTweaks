using R2API;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2.Skills;
using HIFUAcridTweaks.Skills;

namespace HIFUAcridTweaks.Skilldefs
{
    public static class NewrotoxinSD
    {
        public static SkillDef sd;
        public static string nameToken = "HACT_CROCO_SECONDARY_NAME";

        public static void Create()
        {
            sd = ScriptableObject.CreateInstance<SkillDef>();
            sd.activationState = new(typeof(NewrotoxinState));
            sd.activationStateMachineName = "Weapon";
            sd.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;

            sd.baseRechargeInterval = 4f;
            sd.baseMaxStock = 1;
            sd.rechargeStock = 1;
            sd.requiredStock = 1;
            sd.stockToConsume = 1;

            sd.resetCooldownTimerOnUse = false;
            sd.fullRestockOnAssign = true;
            sd.dontAllowPastMaxStocks = false;
            sd.beginSkillCooldownOnSkillEnd = true;
            sd.cancelSprintingOnActivation = false;
            sd.canceledFromSprinting = false;
            sd.isCombatSkill = true;
            sd.mustKeyPress = false;

            sd.icon = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoSpit.asset").WaitForCompletion().icon;
            sd.skillNameToken = nameToken;
            sd.skillDescriptionToken = "HACT_CROCO_SECONDARY_DESCRIPTION";
            sd.keywordTokens = new string[] { "KEYWORD_AGILE" };

            LanguageAPI.Add("HACT_CROCO_SECONDARY_NAME", "Neurotoxin");
            LanguageAPI.Add("HACT_CROCO_SECONDARY_DESCRIPTION", "<style=cIsUtility>Agile</style>. Spit toxic bile in a cone for 280% damage.");
            ContentAddition.AddSkillDef(sd);
        }
    }
}