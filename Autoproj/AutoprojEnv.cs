using System;
using System.Collections.Generic;

namespace GeminiLab.Autoproj {
    public partial class AutoprojEnv {
        protected readonly Dictionary<string, AutoprojEnvItem> items;
        protected readonly Dictionary<string, string> initValues;

        public AutoprojEnv Parent { get; }
        public AutoprojEnv Root { get; }

        public AutoprojEnv() : this(null) { }

        public AutoprojEnv(AutoprojEnv parent) {
            if (parent == null) {
                Parent = null;
                Root = this;
            } else {
                Parent = parent;
                Root = parent.Root;
            }

            items = new Dictionary<string, AutoprojEnvItem>();
            initValues = new Dictionary<string, string>();
        }


        public void Begin() {
            DoBegin();
        }

        public void End() {
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
                    if (item.AcceptParameters) {
                        var funcResult = item.Invoke(parameters);
                        result = funcResult;
                        return true;
                    } else {
                        if (parameters.Length > 0) return false;
                        result = item.Get();
                        return true;
                    }
                } catch (Exception ex) {
                    result = ex.Message;
                    return false;
                }
            }

            return Parent != null && Parent.TryConvert(key, parameters, out result);
        }

        public bool TryAdd(string key, AutoprojEnvItem item) {
            if (!items.ContainsKey(key)) {
                if (item.UseStorage && initValues.TryGetValue(key, out var storage)) {
                    item.Load(storage);
                }

                items.Add(key, item);
                return true;
            } else {
                return false;
            }
        }

        public void TryAddConst(string key, AutoprojEnvConst val) {
            TryAdd(key, val);
        }

        public void TryAddFunction(string key, AutoprojEnvFunction func) {
            TryAdd(key, func);
        }

        public void TryAddFunction(string key, Func<string[], string> func) {
            TryAdd(key, new AutoprojEnvFunction(func));
        }

        public void TryAddCounter(string key, long initv = 0) {
            TryAdd(key, new AutoprojEnvCounter(initv));
        }

        public void TryAddStaticCounter(string key, long initv = 0) {
            TryAdd(key, new AutoprojEnvStaticCounter(initv));
        }

        public void TryAddVariable(string key, string value) {
            TryAdd(key, new AutoprojEnvVar(value));
        }

        public bool TryAssign(string key, string value) {
            if (items.TryGetValue(key, out var item)) {
                if (!item.CanAssign) return false;

                item.Assign(value);
            } else {
                TryAddVariable(key, value);
            }

            return false;
        }
    }
}
