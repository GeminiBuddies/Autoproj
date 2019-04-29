using System;
using System.Collections.Generic;

namespace GeminiLab.Autoproj {
    public partial class AutoprojEnv {
        protected readonly Dictionary<string, IAutoprojEnvItem> items;
        protected readonly Dictionary<string, string> initValues;

        protected static int counter = 0x400;
        protected static object counterLock = new object();

        protected string id;

        public AutoprojEnv Parent { get; }
        public AutoprojEnv Root { get; }

        public AutoprojEnv() : this(null) {}

        public AutoprojEnv(AutoprojEnv parent) {
            if (parent == null) {
                Parent = null;
                Root = this;
            } else {
                Parent = parent;
                Root = parent.Root;
            }

            items = new Dictionary<string, IAutoprojEnvItem>();
            initValues = new Dictionary<string, string>();

            lock (counterLock) {
                id = $"{counter++:X6}";
            }

            Program.Logger.Trace($"env {id} created.");
        }


        public void Begin() {
            Program.Logger.Trace($"env {id} begin.");
            DoBegin();
        }

        public void End() {
            Program.Logger.Trace($"env {id} end.");
            DoEnd();
        }

        protected virtual void DoBegin() {
            // nothing here
        }

        protected virtual void DoEnd() {
            // nothing here
        }

        // result spit out error message when failed
        public bool TryConvert(string key, string[] parameters, out string result) {
            result = null;

            if (items.TryGetValue(key, out var item)) {
                try {
                    switch (item) {
                    case IAutoprojEnvParameterizedItem parameterizedItem: 
                        result = parameterizedItem.Invoke(parameters);
                         return true;
                    case IAutoprojEnvNotParameterizedItem _ when parameters.Length > 0:
                        return false;
                    case IAutoprojEnvNotParameterizedItem notParameterizedItem:
                        result = notParameterizedItem.Get();
                        return true;
                    default:
                        return false;
                    }
                } catch (Exception ex) {
                    result = ex.Message;
                    return false;
                }
            }

            return Parent != null && Parent.TryConvert(key, parameters, out result);
        }

        public bool TryAdd(string key, IAutoprojEnvItem item) {
            if (!items.ContainsKey(key)) {
                if (item is IAutoprojEnvPersistenceItem persistenceItem && initValues.TryGetValue(key, out var storage)) {
                    persistenceItem.Load(storage);
                }

                items.Add(key, item);
                return true;
            }

            return false;
        }

        public bool TryAddConst(string key, AutoprojEnvConst val) => TryAdd(key, val);

        public bool TryAddFunction(string key, AutoprojEnvFunction func) => TryAdd(key, func);

        public bool TryAddFunction(string key, Func<string[], string> func) => TryAdd(key, new AutoprojEnvFunction(func));

        public bool TryAddCounter(string key, long initv = 0) => TryAdd(key, new AutoprojEnvCounter(initv));

        public bool TryAddStaticCounter(string key, long initv = 0) => TryAdd(key, new AutoprojEnvStaticCounter(initv));

        public bool TryAddVariable(string key, string value) => TryAdd(key, new AutoprojEnvVar(value));

        public bool TryAssign(string key, string value) {
            if (items.TryGetValue(key, out var item)) {
                if (!(item is IAutoprojEnvAssignableItem assignableItem)) return false;

                assignableItem.Assign(value);
                return true;
            }

            return TryAddVariable(key, value);
        }
    }
}
