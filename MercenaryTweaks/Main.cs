using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using RoR2;
using R2API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using HIFUMercenaryTweaks.Skills;

namespace HMT
{
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public const string PluginAuthor = "HIFU";
        public const string PluginName = "HIFUMercenaryTweaks";
        public const string PluginVersion = "1.0.2";

        public static ConfigFile HMTConfig;
        public static ManualLogSource HMTLogger;

        private string version = PluginVersion;

        public static ConfigEntry<bool> scaleSomeSkillDamageWithAttackSpeed { get; set; }

        public void Awake()
        {
            HMTLogger = Logger;
            HMTConfig = Config;

            scaleSomeSkillDamageWithAttackSpeed = Config.Bind("Non-Special Skills :: Scaling", "Scale Damage with Attack Speed?", true, "Vanilla is false");

            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(TweakBase))
                                           select type;

            HMTLogger.LogInfo("==+----------------==TWEAKS==----------------+==");

            foreach (Type type in enumerable)
            {
                TweakBase based = (TweakBase)Activator.CreateInstance(type);
                if (ValidateTweak(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable2 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(MiscBase))
                                            select type;

            HMTLogger.LogInfo("==+----------------==MISC==----------------+==");

            foreach (Type type in enumerable2)
            {
                MiscBase based = (MiscBase)Activator.CreateInstance(type);
                if (ValidateMisc(based))
                {
                    based.Init();
                }
            }

            if (Eviscerate.instance.improveEvis)
            {
                var merc = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercBody.prefab").WaitForCompletion();
                var esm = merc.AddComponent<EntityStateMachine>();
                esm.customName = "Evis";
                esm.initialStateType = new(typeof(EntityStates.Idle));
                esm.mainStateType = new(typeof(EntityStates.Idle));

                var evis = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Merc/MercBodyEvis.asset").WaitForCompletion();
                evis.activationStateMachineName = "Evis";
            }
        }

        public bool ValidateTweak(TweakBase tb)
        {
            if (tb.isEnabled)
            {
                bool enabledfr = Config.Bind(tb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ValidateMisc(MiscBase mb)
        {
            if (mb.isEnabled)
            {
                bool enabledfr = Config.Bind(mb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        private void PERIPHERYVSWEEP()
        {
        }
    }
}