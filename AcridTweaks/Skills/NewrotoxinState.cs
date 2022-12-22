using EntityStates;
using EntityStates.VoidSurvivor;
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
        public float radius = 6f;
        public float range = 11f;
        public float procCoeff = 0.7f;
        public float damage = 2f;

        [SerializeField]
        public GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/MuzzleflashCroco.prefab").WaitForCompletion();

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
                EffectManager.SimpleMuzzleFlash(effectPrefab, gameObject, muzz, false);
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
                    damage = damageStat * damage,
                    force = force,
                    muzzleName = muzz,
                    hitEffectPrefab = effectPrefab,
                    isCrit = Util.CheckRoll(critStat, characterBody.master),
                    radius = radius,
                    falloffModel = BulletAttack.FalloffModel.None,
                    stopperMask = LayerIndex.world.mask,
                    procCoefficient = procCoeff,
                    maxDistance = range,
                    smartCollision = false,
                    damageType = damageType,
                }.Fire();
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