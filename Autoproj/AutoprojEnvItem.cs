using System;
using System.Collections.Generic;
using System.Text;

namespace GeminiLab.Autoproj {
    public abstract class AutoprojEnvItem {
        // functions for get value, `Get` is used if AcceptParameters == false, otherwise `Invoke` is used
        public abstract bool AcceptParameters { get; }
        public abstract string Get();
        public abstract string Invoke(string[] param);

        // functions for storage, not used if UseStorage == false
        public abstract bool UseStorage { get; }
        public abstract void Load(string value);
        public abstract string Save();

        // functions for assign, not used if CanAssign == false
        public abstract bool CanAssign { get; }
        public abstract void Assign(string value);
    }

    public abstract class AutoprojEnvNonFunctionItem : AutoprojEnvItem {
        public sealed override bool AcceptParameters => false;
        public sealed override string Invoke(string[] param) => throw new InvalidOperationException();
        
        // default values
        public override bool UseStorage => false;
        public override void Load(string value) => throw new InvalidOperationException();
        public override string Save() => throw new InvalidOperationException();
        public override bool CanAssign => false;
        public override void Assign(string value) => throw new InvalidOperationException();
    }
}
