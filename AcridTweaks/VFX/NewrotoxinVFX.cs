using HACT;
using IL.RoR2.ContentManagement;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUAcridTweaks.VFX
{
    public static class NewrotoxinVFX
    {
        public static GameObject ghost;

        public static void Create()
        {
            var mat = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matMageFlamethrower.mat").WaitForCompletion());
            var tex = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampAntler.png").WaitForCompletion();
            mat.SetTexture("_RemapTex", tex);
            ghost = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/FlamebreathEffect.prefab").WaitForCompletion(), "NewrotoxinVFX", false);
            Object.Destroy(ghost.GetComponent<ProjectileGhostController>());

            ghost.AddComponent<EffectComponent>();

            var d = ghost.AddComponent<DestroyOnTimer>();
            d.duration = 0.7f;

            var gt = ghost.transform;

            var flame = gt.GetChild(1);
            var flameps = flame.GetComponent<ParticleSystem>();

            var flameMain = flameps.main;
            flameMain.duration = 0.5f;
            var flameSS = flameMain.startSpeed;
            flameSS.mode = ParticleSystemCurveMode.Constant;
            flameSS.constant = Main.newrotoxinRange.Value * 2f;

            var flameShape = flameps.shape;
            flameShape.radius = Main.newrotoxinRadius.Value * 0.4f;

            var flamepsr = flame.GetComponent<ParticleSystemRenderer>();
            flamepsr.sharedMaterial = mat;

            var flameVOL = flameps.velocityOverLifetime;
            flameVOL.enabled = true;
            flameVOL.z = Main.newrotoxinRange.Value;
            flameVOL.speedModifier = 1.15f;

            var flameEmission = flameps.emission;
            flameEmission.rateOverDistance = 180;
            flameEmission.rateOverTime = 180;

            var flamed = gt.GetChild(2);
            var flamedps = flamed.GetComponent<ParticleSystem>();

            var flamedMain = flamedps.main;
            flamedMain.duration = 0.6f;
            var flamedSS = flamedMain.startSpeed;
            flamedSS.mode = ParticleSystemCurveMode.Constant;
            flamedSS.constant = Main.newrotoxinRange.Value * 2f;

            var flamedShape = flamedps.shape;
            flamedShape.radius = Main.newrotoxinRadius.Value * 0.89f;

            var flamedpsr = flamed.GetComponent<ParticleSystemRenderer>();
            flamedpsr.sharedMaterial = mat;

            var flamedVOL = flamedps.velocityOverLifetime;
            flamedVOL.enabled = true;
            flamedVOL.z = Main.newrotoxinRange.Value;
            flamedVOL.speedModifier = 1.15f;

            var flamedEmission = flamedps.emission;
            flamedEmission.rateOverDistance = 180;
            flamedEmission.rateOverTime = 180;

            var sparks = gt.GetChild(4);
            var sparksps = sparks.GetComponent<ParticleSystem>();

            var sparksMain = sparksps.main;
            sparksMain.duration = 0.65f;
            var sparksSS = sparksMain.startSpeed;
            sparksSS.mode = ParticleSystemCurveMode.Constant;
            sparksSS.constant = Main.newrotoxinRange.Value * 2f;

            var sparksSC = sparksMain.startColor;
            sparksSC.color = new Color32(134, 255, 0, 255);

            var sparksShape = sparksps.shape;
            sparksShape.radius = Main.newrotoxinRadius.Value * 2f;

            var sparksVOL = sparksps.velocityOverLifetime;
            sparksVOL.enabled = true;
            sparksVOL.z = Main.newrotoxinRange.Value;
            sparksVOL.speedModifier = 1.15f;

            var sparksEmission = sparksps.emission;
            sparksEmission.rateOverDistance = 180;
            sparksEmission.rateOverTime = 180;

            gt.GetChild(3).gameObject.SetActive(false);

            var l1 = gt.GetChild(0).GetComponent<Light>();
            l1.color = new Color32(134, 255, 0, 255);
            l1.intensity = 15f;
            l1.range = 10f;

            var l2 = gt.GetChild(5).GetComponent<Light>();
            l2.color = new Color32(186, 219, 75, 255);
            l2.range = 20f;
            l2.intensity = 10f;

            ContentAddition.AddEffect(ghost);
        }
    }
}