namespace GeminiLab.Autoproj {
    public interface IAutoprojEnvItem {}

    public interface IAutoprojEnvNotParameterizedItem : IAutoprojEnvItem {
        string Get();
    }
    public interface IAutoprojEnvParameterizedItem : IAutoprojEnvItem {
        string Invoke(string[] param);
    }

    public interface IAutoprojEnvPersistenceItem : IAutoprojEnvItem {
        void Load(string value);
        string Save();
    }

    public interface IAutoprojEnvAssignableItem : IAutoprojEnvItem {
        void Assign(string value);
    }
}
