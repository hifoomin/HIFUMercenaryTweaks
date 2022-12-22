using EntityStates;
using EntityStates.VoidSurvivor;
using HACT;
using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace HIFUAcridTweaks.Skills
{
    public class NewrotoxinState : BaseState
    {
        public static CrocoDamageTypeController crocoDamageTypeController;
        public float baseDuration = 1f;
        public float duration;
        public string attackSound = "Play_acrid_m2_shoot";
        public float bloom = 0.3f;
        public float force = 500f;

        [SerializeField]
        public GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/MuzzleflashCroco.prefab").WaitForCompletion();

        [SerializeField]
        public GameObject effectPrefab2 = VFX.NewrotoxinVFX.ghost;

        public override void OnEnter()
        {
            base.OnEnter();

            crocoDamageTypeController = GetComponent<CrocoDamageTypeController>();
            duration = baseDuration / attackSpeedStat;
            var aimRay = GetAimRay();
            StartAimMode(duration + 2f, false);
            PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", duration);
            Util.PlaySound(attackSound, gameObject);

            AddRecoil(-2f, -2.5f, -1.25f, 1.25f);
            characterBody.AddSpreadBloom(bloom);

            var muzz = "MouthMuzzle";

            if (effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(effectPrefab, gameObject, muzz, true);
            }
            if (isAuthority)
            {
                var damageType = crocoDamageTypeController ? crocoDamageTypeController.GetDamageType() : DamageType.Generic;

                new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = 0f,
                    damage = damageStat * Main.newrotoxinDamage.Value,
                    force = force,
                    muzzleName = muzz,
                    hitEffectPrefab = effectPrefab,
                    isCrit = Util.CheckRoll(critStat, characterBody.master),
                    radius = Main.newrotoxinRadius.Value,
                    falloffModel = BulletAttack.FalloffModel.None,
                    stopperMask = LayerIndex.world.mask,
                    procCoefficient = Main.newrotoxinProcCoeff.Value,
                    maxDistance = Main.newrotoxinRange.Value,
                    smartCollision = false,
                    damageType = damageType,
                }.Fire();
                EffectManager.SpawnEffect(effectPrefab2, new EffectData { origin = aimRay.origin, scale = 1f }, true);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}