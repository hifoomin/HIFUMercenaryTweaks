using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using HIFUMercenaryTweaks.Skills;
using R2API;
using HarmonyLib;

namespace HIFUMercenaryTweaks
{
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public const string PluginAuthor = "HIFU";
        public const string PluginName = "HIFUMercenaryTweaks";
        public const string PluginVersion = "1.1.1";

        public static ConfigFile HMTConfig;
        public static ConfigFile HMTBackupConfig;

        public static ConfigEntry<bool> enableAutoConfig { get; set; }
        public static ConfigEntry<string> latestVersion { get; set; }

        public static ManualLogSource HMTLogger;

        public static ConfigEntry<bool> scaleSomeSkillDamageWithAttackSpeed { get; set; }

        public static bool _preVersioning = false;

        public void Awake()
        {
            HMTLogger = Logger;
            HMTConfig = Config;

            HMTBackupConfig = new(Paths.ConfigPath + "\\" + PluginAuthor + "." + PluginName + ".Backup.cfg", true);
            HMTBackupConfig.Bind(": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :", ": DO NOT MODIFY THIS FILES CONTENTS :");

            enableAutoConfig = HMTConfig.Bind("Config", "Enable Auto Config Sync", true, "Disabling this would stop HIFUMercenaryTweaks from syncing config whenever a new version is found.");
            _preVersioning = !((Dictionary<ConfigDefinition, string>)AccessTools.DeclaredPropertyGetter(typeof(ConfigFile), "OrphanedEntries").Invoke(HMTConfig, null)).Keys.Any(x => x.Key == "Latest Version");
            latestVersion = HMTConfig.Bind("Config", "Latest Version", PluginVersion, "DO NOT CHANGE THIS");
            if (enableAutoConfig.Value && (_preVersioning || (latestVersion.Value != PluginVersion)))
            {
                latestVersion.Value = PluginVersion;
                ConfigManager.VersionChanged = true;
                HMTLogger.LogInfo("Config Autosync Enabled.");
            }

            scaleSomeSkillDamageWithAttackSpeed = Config.Bind("Non-Special Skills :: Scaling", "Scale Damage with Attack Speed?", true, "Vanilla is false");

            Keywords.Init();

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

            if (Eviscerate.allowMovement)
            {
                var merc = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercBody.prefab").WaitForCompletion();
                var esm = merc.AddComponent<EntityStateMachine>();

                esm.customName = "Evis";
                esm.initialStateType = new(typeof(EntityStates.Idle));
                esm.mainStateType = new(typeof(EntityStates.Idle));

                var nsm = merc.GetComponent<NetworkStateMachine>();

                Array.Resize(ref nsm.stateMachines, nsm.stateMachines.Length + 1);
                nsm.stateMachines[nsm.stateMachines.Length - 1] = esm;

                // ref is the worst piece of shit ever invented

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