using HACT;
using R2API;
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
            ghost = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoSpitGhost.prefab").WaitForCompletion(), "NewrotoxinVFX");
            Object.Destroy(ghost.GetComponent<ProjectileGhostController>());

            var gt = ghost.transform;

            var flashes = gt.GetChild(0).GetComponent<ParticleSystem>();
            var flashesMain = flashes.main;
            flashesMain.startSizeMultiplier = Main.newrotoxinRadius.Value / 2f;
            var flashesShape = flashes.shape;
            flashesShape.radius = Main.newrotoxinRadius.Value;

            var pointLight = gt.GetChild(1).GetComponent<Light>();
            pointLight.range = Main.newrotoxinRange.Value / 2f;

            var goows = gt.GetChild(2).GetComponent<ParticleSystem>();
            var goowsMain = goows.main;
            goowsMain.startSizeMultiplier = Main.newrotoxinRadius.Value / 1.5f;
            var goowsShape = goows.shape;
            goowsShape.radius = Main.newrotoxinRadius.Value / 10f;

            var goodrip = gt.GetChild(3).GetComponent<ParticleSystem>();
            var goodripMain = goodrip.main;
            goodripMain.startSizeMultiplier = Main.newrotoxinRadius.Value;
            var goodripShape = goodrip.shape;
            goodripShape.radius = Main.newrotoxinRadius.Value / 5f;

            var flames = gt.GetChild(4).GetComponent<ParticleSystem>();
            var flamesMain = flames.main;
            flamesMain.startSizeMultiplier = Main.newrotoxinRadius.Value * 1.3f;
            var flamesShape = flames.shape;
            flamesShape.radius = Main.newrotoxinRadius.Value / 100f;

            var spinner = gt.GetChild(6);
            var trail1 = spinner.GetChild(0).GetComponent<TrailRenderer>();
            trail1.widthMultiplier = Main.newrotoxinRadius.Value / 2f;
            var trail2 = spinner.GetChild(1).GetComponent<TrailRenderer>();
            trail2.widthMultiplier = Main.newrotoxinRadius.Value / 2f;
        }
    }
}