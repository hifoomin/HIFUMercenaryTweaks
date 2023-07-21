using R2API;

namespace HIFUMercenaryTweaks
{
    public abstract class TweakBase
    {
        public abstract string Name { get; }
        public abstract string SkillToken { get; }
        public abstract string DescText { get; }
        public virtual bool isEnabled { get; } = true;
        public bool done = false;

        public T ConfigOption<T>(T value, string name, string description)
        {
            var config = Main.HMTConfig.Bind<T>(Name, name, value, description);
            ConfigManager.HandleConfig<T>(config, Main.HMTBackupConfig, name);
            if (!done)
            {
                ConfigManager.HandleConfig<T>(Main.scaleSomeSkillDamageWithAttackSpeed, Main.HMTBackupConfig, Main.scaleSomeSkillDamageWithAttackSpeed.Definition.Key);
                done = true;
            }
            return config.Value;
        }

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public virtual void Init()
        {
            Hooks();
            string descriptionToken = "MERC_" + SkillToken.ToUpper() + "_DESCRIPTION";
            LanguageAPI.Add(descriptionToken, DescText);
            Main.HMTLogger.LogInfo("Added " + Name);
        }
    }
}