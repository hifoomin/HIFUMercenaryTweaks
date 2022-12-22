using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;
using RoR2.Skills;
using HIFUAcridTweaks.Skilldefs;
using HACT;

namespace HIFUAcridTweaks
{
    public static class ReplaceSkill
    {
        public static void Create()
        {
            var acrid = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoBody.prefab").WaitForCompletion();

            var sl = acrid.GetComponent<SkillLocator>();

            var skillFamily = sl.secondary.skillFamily;
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = NewrotoxinSD.sd,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(NewrotoxinSD.nameToken, false, null)
            };
        }
    }
}