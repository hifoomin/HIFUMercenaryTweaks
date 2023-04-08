using EntityStates.Merc;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine.AddressableAssets;
using RoR2.Skills;

namespace HIFUMercenaryTweaks.Skills
{
    public class Eviscerate : TweakBase<Eviscerate>
    {
        public float damageCoefficient;
        public float cooldown;
        public bool improveEvis;
        public bool removeCameraChanges;
        public override string Name => "Special : Eviscerate";

        public override string SkillToken => "special";

        public override string DescText => "Target the nearest enemy, attacking them for <style=cIsDamage>" + d(damageCoefficient) + " damage</style> repeatedly. <style=cIsUtility>You cannot be hit for the duration</style>.";

        public override void Init()
        {
            damageCoefficient = ConfigOption(1.3f, "Damage", "Decimal. Vanilla is 1.1");
            improveEvis = ConfigOption(true, "Improve targetting and enable movement?", "Vanilla is false");
            removeCameraChanges = ConfigOption(true, "Remove camera changes?", "Vanilla is false");
            cooldown = ConfigOption(7f, "Cooldown", "Vanilla is 6");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Merc.Evis.OnEnter += Evis_OnEnter;
            if (removeCameraChanges)
            {
                IL.EntityStates.Merc.Evis.OnEnter += Evis_OnEnter1;
                IL.EntityStates.Merc.EvisDash.OnEnter += EvisDash_OnEnter;
            }

            On.EntityStates.Merc.Evis.SearchForTarget += Evis_SearchForTarget;
            On.EntityStates.Merc.Evis.FixedUpdate += Evis_FixedUpdate;

            On.EntityStates.Merc.EvisDash.FixedUpdate += EvisDash_FixedUpdate;
            Changes();
        }

        private void EvisDash_FixedUpdate(On.EntityStates.Merc.EvisDash.orig_FixedUpdate orig, EvisDash self)
        {
            if (improveEvis)
            {
                self.stopwatch += Time.fixedDeltaTime;
                if (self.stopwatch > EvisDash.dashPrepDuration && !self.isDashing)
                {
                    self.isDashing = true;
                    self.dashVector = self.inputBank.aimDirection;
                    self.CreateBlinkEffect(Util.GetCorePosition(self.gameObject));
                    self.PlayCrossfade("FullBody, Override", "EvisLoop", 0.1f);
                    if (self.modelTransform)
                    {
                        TemporaryOverlay temporaryOverlay = self.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                        temporaryOverlay.duration = 0.6f;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                        temporaryOverlay.AddToCharacerModel(self.modelTransform.GetComponent<CharacterModel>());
                        TemporaryOverlay temporaryOverlay2 = self.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                        temporaryOverlay2.duration = 0.7f;
                        temporaryOverlay2.animateShaderAlpha = true;
                        temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                        temporaryOverlay2.destroyComponentOnEnd = true;
                        temporaryOverlay2.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                        temporaryOverlay2.AddToCharacerModel(self.modelTransform.GetComponent<CharacterModel>());
                    }
                }
                bool flag = self.stopwatch >= EvisDash.dashDuration + EvisDash.dashPrepDuration;
                if (self.isDashing)
                {
                    if (self.characterMotor && self.characterDirection)
                    {
                        self.characterMotor.rootMotion += self.dashVector * (self.moveSpeedStat * EvisDash.speedCoefficient * Time.fixedDeltaTime);
                    }
                    if (self.isAuthority)
                    {
                        Collider[] array = Physics.OverlapSphere(self.transform.position, self.characterBody.radius + EvisDash.overlapSphereRadius * (flag ? EvisDash.lollypopFactor : 1f), LayerIndex.entityPrecise.mask);
                        for (int i = 0; i < array.Length; i++)
                        {
                            HurtBox component = array[i].GetComponent<HurtBox>();
                            if (component && component.teamIndex != self.teamComponent.teamIndex)
                            {
                                Evis evis = new();
                                self.outer.SetNextState(evis);
                                return;
                            }
                        }
                    }
                }
                if (flag && self.isAuthority)
                {
                    self.outer.SetNextStateToMain();
                }
            }
            else
            {
                orig(self);
            }
        }

        private void EvisDash_OnEnter(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(2)))
            {
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, 3);
            }
            else
            {
                Main.HMTLogger.LogError("Failed to apply Eviscerate Camera hook");
            }
        }

        private void Evis_OnEnter1(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(2)))
            {
                c.Remove();
                c.Emit(OpCodes.Ldc_I4, 3);
            }
            else
            {
                Main.HMTLogger.LogError("Failed to apply Eviscerate Camera hook");
            }
        }

        private void Evis_FixedUpdate(On.EntityStates.Merc.Evis.orig_FixedUpdate orig, Evis self)
        {
            if (improveEvis)
            {
                // self.FixedUpdate();
                self.stopwatch += Time.fixedDeltaTime;
                self.attackStopwatch += Time.fixedDeltaTime;
                float num = 1f / Evis.damageFrequency / self.attackSpeedStat;
                if (self.attackStopwatch >= num)
                {
                    self.attackStopwatch -= num;
                    HurtBox hurtBox = self.SearchForTarget();
                    if (hurtBox)
                    {
                        Util.PlayAttackSpeedSound(Evis.slashSoundString, self.gameObject, Evis.slashPitch);
                        Util.PlaySound(Evis.dashSoundString, self.gameObject);
                        Util.PlaySound(Evis.impactSoundString, self.gameObject);
                        HurtBoxGroup hurtBoxGroup = hurtBox.hurtBoxGroup;
                        HurtBox hurtBox2 = hurtBoxGroup.hurtBoxes[0];
                        if (hurtBox2)
                        {
                            Vector3 position = hurtBox2.transform.position;
                            Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
                            Vector3 vector = new Vector3(normalized.x, 0f, normalized.y);
                            EffectManager.SimpleImpactEffect(Evis.hitEffectPrefab, position, vector, false);
                            Transform transform = hurtBox.hurtBoxGroup.transform;
                            TemporaryOverlay temporaryOverlay = transform.gameObject.AddComponent<TemporaryOverlay>();
                            temporaryOverlay.duration = num;
                            temporaryOverlay.animateShaderAlpha = true;
                            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                            temporaryOverlay.destroyComponentOnEnd = true;
                            temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matMercEvisTarget");
                            temporaryOverlay.AddToCharacerModel(transform.GetComponent<CharacterModel>());
                            if (NetworkServer.active)
                            {
                                DamageInfo damageInfo = new();
                                damageInfo.damage = Evis.damageCoefficient * self.damageStat;
                                damageInfo.attacker = self.gameObject;
                                damageInfo.procCoefficient = Evis.procCoefficient;
                                damageInfo.position = hurtBox2.transform.position;
                                damageInfo.crit = self.crit;
                                hurtBox2.healthComponent.TakeDamage(damageInfo);
                                GlobalEventManager.instance.OnHitEnemy(damageInfo, hurtBox2.healthComponent.gameObject);
                                GlobalEventManager.instance.OnHitAll(damageInfo, hurtBox2.healthComponent.gameObject);
                            }
                        }
                    }
                    else if (self.isAuthority && self.stopwatch > Evis.minimumDuration)
                    {
                        self.outer.SetNextStateToMain();
                    }
                }
                if (self.stopwatch >= Evis.duration && self.isAuthority)
                {
                    self.outer.SetNextStateToMain();
                }
            }
            else
            {
                orig(self);
            }
        }

        private HurtBox Evis_SearchForTarget(On.EntityStates.Merc.Evis.orig_SearchForTarget orig, Evis self)
        {
            if (improveEvis)
            {
                BullseyeSearch bullseyeSearch = new()
                {
                    searchOrigin = self.transform.position,
                    searchDirection = self.inputBank.aimDirection,
                    maxDistanceFilter = Evis.maxRadius,
                    // teamMaskFilter = TeamMask.GetUnprotectedTeams(self.GetTeam()),
                    teamMaskFilter = TeamMask.AllExcept(self.GetTeam()),
                    sortMode = BullseyeSearch.SortMode.Angle
                };
                // bullseyeSearch.teamMaskFilter.RemoveTeam(self.characterBody.teamComponent.teamIndex);
                bullseyeSearch.RefreshCandidates();
                bullseyeSearch.FilterOutGameObject(self.gameObject);
                return bullseyeSearch.GetResults().FirstOrDefault();
            }
            else
            {
                return orig(self);
            }
        }

        private void Evis_OnEnter(On.EntityStates.Merc.Evis.orig_OnEnter orig, Evis self)
        {
            Evis.damageCoefficient = damageCoefficient;
            orig(self);
        }

        private void Changes()
        {
            var eviscerate = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercBodyEvis.asset").WaitForCompletion();
            eviscerate.baseRechargeInterval = cooldown;
        }
    }
}