using HACT;
using MonoMod.Cil;
using RoR2;
using System;

namespace HIFUAcridTweaks.Skills
{
    public class Poison : TweakBase
    {
        public static float duration;
        public override string Name => "Passive : Poison";

        public override string SkillToken => "passive";

        public override string DescText => "<style=cIsHealing>Poisonous</style> attacks apply a powerful damage-over-time.";

        public override void Init()
        {
            duration = ConfigOption(6f, "Duration", "Vanilla is 10");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(4),
                x => x.MatchLdcR4(10f)))
            {
                c.Index += 1;
                c.Next.Operand = duration;
            }
            else
            {
                Main.HACTLogger.LogError("Failed to IL Hook Poison Duration");
            }
        }
    }
}