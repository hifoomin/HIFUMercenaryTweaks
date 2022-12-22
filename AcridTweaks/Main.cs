using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HIFUAcridTweaks;
using HIFUAcridTweaks.Skilldefs;
using R2API;
using R2API.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HIFUAcridTweaks.VFX;

namespace HACT
{
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public const string PluginAuthor = "HIFU";
        public const string PluginName = "HIFUAcridTweaks";
        public const string PluginVersion = "1.0.0";

        public static ConfigFile HACTConfig;
        public static ManualLogSource HACTLogger;

        private string version = PluginVersion;

        public static ConfigEntry<float> newrotoxinDamage;
        public static ConfigEntry<float> newrotoxinRange;
        public static ConfigEntry<float> newrotoxinRadius;
        public static ConfigEntry<float> newrotoxinProcCoeff;

        public void Awake()
        {
            HACTLogger = Logger;
            HACTConfig = Config;

            newrotoxinDamage = Config.Bind("Secondary : Neurotoxin", "Damage", 2.5f, "Decimal. Default is 2.5");
            newrotoxinRange = Config.Bind("Secondary : Neurotoxin", "Range", 25f, "Default is 25");
            newrotoxinRadius = Config.Bind("Secondary : Neurotoxin", "Radius", 5.3f, "Default is 5.3");
            newrotoxinProcCoeff = Config.Bind("Secondary : Neurotoxin", "Proc Coefficient", 1f, "Default is 1");

            NewrotoxinVFX.Create();
            NewrotoxinSD.Create();
            ReplaceSkill.Create();

            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(TweakBase))
                                           select type;

            HACTLogger.LogInfo("==+----------------==TWEAKS==----------------+==");

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

            HACTLogger.LogInfo("==+----------------==MISC==----------------+==");

            foreach (Type type in enumerable2)
            {
                MiscBase based = (MiscBase)Activator.CreateInstance(type);
                if (ValidateMisc(based))
                {
                    based.Init();
                }
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

        private void WITHINDESTRUCTIONMYFUCKINGBELOVED()
        {
        }
    }
}