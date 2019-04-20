using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace GeminiLab.Autoproj {
    public class AutoprojEnvFunction : AutoprojEnvItem {
        protected readonly Func<string[], string> Func;
        public AutoprojEnvFunction(Func<string[], string> func) {
            Func = func;
        }

        public sealed override bool AcceptParameters => true;
        public override string Invoke(string[] param) => Func(param);
        public sealed override string Get() => throw new InvalidOperationException();

        // default values
        //   storage not completely disabled, maybe you need to storage some internal status
        public override bool UseStorage => false;
        public override void Load(string value) => throw new InvalidOperationException();
        public override string Save() => throw new InvalidOperationException();
        public sealed override bool CanAssign => false;
        public sealed override void Assign(string value) => throw new InvalidOperationException();
    }

    public class AutoprojEnvVar : AutoprojEnvNonFunctionItem {
        protected string val;

        public AutoprojEnvVar(string value) {
            val = value;
        }

        public override string Get() => val;

        public override bool CanAssign => true;
        public override void Assign(string value) => val = value;
    }

    public sealed class AutoprojEnvConst : AutoprojEnvVar {
        public AutoprojEnvConst(string value) : base(value) { }

        public static implicit operator AutoprojEnvConst(string value) => new AutoprojEnvConst(value);

        public override bool CanAssign => false;
        public override void Assign(string value) => throw new InvalidOperationException();
    }

    public sealed class AutoprojEnvStaticVar : AutoprojEnvVar {
        public AutoprojEnvStaticVar(string value) : base(value) { }
        public override bool UseStorage => true;
        public override void Load(string value) => val = value;
        public override string Save() => val;
    }

    public class AutoprojEnvCounter : AutoprojEnvNonFunctionItem {
        protected long counter;

        public AutoprojEnvCounter(): this(0) { }

        public AutoprojEnvCounter(long counter) {
            this.counter = counter;
        }

        public override string Get() => $"{counter++}";
    }

    public class AutoprojEnvStaticCounter : AutoprojEnvCounter {
        public AutoprojEnvStaticCounter() : base(0) { }
        public AutoprojEnvStaticCounter(long counter) : base(counter) { }

        public override bool UseStorage => true;
        public override void Load(string value) => counter = long.TryParse(value, out var result) ? result : 0;
        public override string Save() => $"{counter}";
    }
}
