using HMT;
using EntityStates.Merc;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HIFUMercenaryTweaks.Skills
{
    public class Eviscerate : TweakBase<Eviscerate>
    {
        public float damageCoefficient;
        public bool improveEvis;
        public override string Name => "Special : Eviscerate";

        public override string SkillToken => "special";

        public override string DescText => "Target the nearest enemy, attacking them for <style=cIsDamage>" + d(damageCoefficient) + " damage</style> repeatedly. <style=cIsUtility>You cannot be hit for the duration</style>.";

        public override void Init()
        {
            damageCoefficient = ConfigOption(1.1f, "Damage", "Decimal. Vanilla is 1.1");
            improveEvis = ConfigOption(true, "Improve targetting and enable movement?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Merc.Evis.OnEnter += Evis_OnEnter;
            On.EntityStates.Merc.Evis.SearchForTarget += Evis_SearchForTarget;
            On.EntityStates.Merc.Evis.FixedUpdate += Evis_FixedUpdate;
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
                    teamMaskFilter = TeamMask.GetUnprotectedTeams(self.GetTeam()),
                    sortMode = BullseyeSearch.SortMode.Angle
                };
                bullseyeSearch.teamMaskFilter.RemoveTeam(self.characterBody.teamComponent.teamIndex);
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
    }
}