using System;

namespace GeminiLab.Autoproj {
    public sealed class AutoprojEnvFunction : IAutoprojEnvParameterizedItem {
        private readonly Func<string[], string> _func;
        public AutoprojEnvFunction(Func<string[], string> func) {
            _func = func;
        }

        public string Invoke(string[] param) => _func(param);
    }

    public class AutoprojEnvVar : IAutoprojEnvAssignableItem, IAutoprojEnvNotParameterizedItem {
        protected string var;

        public AutoprojEnvVar(string value) {
            var = value;
        }

        public string Get() => var;

        public void Assign(string value) => var = value;
    }

    public sealed class AutoprojEnvConst : IAutoprojEnvNotParameterizedItem {
        private string _val;

        public AutoprojEnvConst(string value) {
            _val = value;
        }

        public static implicit operator AutoprojEnvConst(string value) => new AutoprojEnvConst(value);

        public string Get() => _val;
    }

    public sealed class AutoprojEnvStaticVar : AutoprojEnvVar, IAutoprojEnvPersistenceItem {
        public AutoprojEnvStaticVar(string value) : base(value) { }

        public void Load(string value) => var = value;
        public string Save() => var;
    }

    public class AutoprojEnvCounter : IAutoprojEnvNotParameterizedItem {
        protected long counter;

        public AutoprojEnvCounter(): this(0) { }

        public AutoprojEnvCounter(long counter) {
            this.counter = counter;
        }

        public string Get() => $"{counter++}";
    }

    public sealed class AutoprojEnvStaticCounter : AutoprojEnvCounter, IAutoprojEnvPersistenceItem {
        public AutoprojEnvStaticCounter() : base(0) { }
        public AutoprojEnvStaticCounter(long counter) : base(counter) { }


        public void Load(string value) => counter = long.TryParse(value, out var result) ? result : 0;
        public string Save() => $"{counter}";
    }
}
