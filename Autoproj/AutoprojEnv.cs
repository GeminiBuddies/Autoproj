using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

using GeminiLab.Core2;
using GeminiLab.Core2.Collections;
using GeminiLab.Core2.ML.Json;

namespace GeminiLab.Autoproj {
    internal enum AutoprojEnvItemType {
        Function,
        Const,
        Variable,
        Counter,
        StaticCounter
    }

    internal partial class AutoprojEnv {
        protected readonly Dictionary<string, AutoprojEnvItemType> _type;
        protected readonly Dictionary<string, ulong> _staticCounters;
        protected readonly Dictionary<string, ulong> _counters;
        protected readonly Dictionary<string, string> _vars;
        protected readonly Dictionary<string, string> _consts;
        protected readonly Dictionary<string, Func<string[], string>> _funcs;

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

            _type = new Dictionary<string, AutoprojEnvItemType>();
            _staticCounters = new Dictionary<string, ulong>();
            _counters = new Dictionary<string, ulong>();
            _vars = new Dictionary<string, string>();
            _consts = new Dictionary<string, string>();
            _funcs = new Dictionary<string, Func<string[], string>>();
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

        public bool TryGetVariable(string key, out string value) {
            value = null;
            if (_type.TryGetValue(key, out var type)) {
                if (type != AutoprojEnvItemType.Variable) return false;

                value = _vars[key];
                return true;
            }

            return Parent != null && Parent.TryGetVariable(key, out value);
        }

        public bool TryGetFunction(string key, out Func<string[], string> func) {
            func = null;

            if (_type.TryGetValue(key, out var type)) {
                if (type != AutoprojEnvItemType.Function) return false;

                func = _funcs[key];
                return true;
            }

            return Parent != null && Parent.TryGetFunction(key, out func);
        }

        public bool TryGetConst(string key, out string value) {
            value = null;
            if (_type.TryGetValue(key, out var type)) {
                if (type != AutoprojEnvItemType.Const) return false;

                value = _consts[key];
                return true;
            }

            return Parent != null && Parent.TryGetConst(key, out value);
        }

        public bool TryUseCounter(string key, out ulong value) {
            value = 0;
            if (_type.TryGetValue(key, out var type)) {
                if (type == AutoprojEnvItemType.Counter) {
                    value = _counters[key];
                    _counters[key] = value + 1;
                    return true;
                }

                if (type == AutoprojEnvItemType.StaticCounter) {
                    value = _staticCounters[key];
                    _staticCounters[key] = value + 1;
                    return true;
                }

                return false;
            }

            return Parent != null && Parent.TryUseCounter(key, out value);
        }

        public bool TryConvert(string key, string[] parameters, out string result) {
            result = null;

            if (_type.TryGetValue(key, out var type)) {
                if (type != AutoprojEnvItemType.Function && parameters.Length > 0) return false;

                if (type == AutoprojEnvItemType.Const) {
                    result = _consts[key];
                    return true;
                }

                if (type == AutoprojEnvItemType.Counter) {
                    result = _counters[key].ToString();
                    _counters[key] = _counters[key] + 1;
                    return true;
                }

                if (type == AutoprojEnvItemType.StaticCounter) {
                    result = _staticCounters[key].ToString();
                    _staticCounters[key] = _staticCounters[key] + 1;
                    return true;
                }

                if (type == AutoprojEnvItemType.Variable) {
                    result = _vars[key];
                    return true;
                }

                result = _funcs[key].Invoke(parameters);
                return true;
            }

            return Parent != null && Parent.TryConvert(key, parameters, out result);
        }

        public bool TryGetTypeLocal(string key, out AutoprojEnvItemType type) {
            return _type.TryGetValue(key, out type);
        }

        public bool TryAddCounter(string key, ulong initValue) {
            if (TryGetTypeLocal(key, out _)) return false;

            _type[key] = AutoprojEnvItemType.Counter;
            _counters[key] = initValue;
            return true;
        }

        public bool TryAddStaticCounter(string key, ulong initValue) {
            if (TryGetTypeLocal(key, out _)) return false;

            _type[key] = AutoprojEnvItemType.StaticCounter;
            _staticCounters[key] = initValue;
            return true;
        }

        public bool TrySetVariable(string key, string value) {
            if (TryGetTypeLocal(key, out var type)) {
                if (type != AutoprojEnvItemType.Variable) return false;
                _vars[key] = value;
                return true;
            }

            _type[key] = AutoprojEnvItemType.Variable;
            _vars[key] = value;
            return true;
        }

        public bool TryAddConst(string key, string value) {
            if (TryGetTypeLocal(key, out _)) return false;

            _type[key] = AutoprojEnvItemType.Const;
            _consts[key] = value;
            return true;
        }

        public bool TryAddFunction(string key, Func<string[], string> func) {
            if (TryGetTypeLocal(key, out _)) return false;

            _type[key] = AutoprojEnvItemType.Function;
            _funcs[key] = func;
            return true;
        }
    }
}
