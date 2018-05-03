using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

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

    internal class AutoprojEnv {
        private readonly Dictionary<string, AutoprojEnvItemType> _type;
        private readonly Dictionary<string, ulong> _staticCounters;
        private readonly Dictionary<string, ulong> _counters;
        private readonly Dictionary<string, string> _vars;
        private readonly Dictionary<string, string> _consts;
        private readonly Dictionary<string, Func<string[], string>> _funcs;

        private readonly string _dbFile = null;

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

        public AutoprojEnv(AutoprojEnv parent, DirectoryInfo dir) : this(parent) {
            var dbFile = new FileInfo(_dbFile = Path.Combine(dir.FullName, ".autoproj.jdb"));
            if (dbFile.Exists) readFromFile(dbFile);
        }

        public AutoprojEnv(AutoprojEnv parent, FileInfo file) : this(parent) {
            var dbFile = new FileInfo(_dbFile = file.FullName + ".jdb");
            if (dbFile.Exists) readFromFile(dbFile);

            TryAddConst("filename", file.Name);
            TryAddConst("filefullname", file.FullName);
            TryAddConst("outputname", file.Name.Substring(0, file.Name.Length - ".autoproj".Length));
            TryAddConst("outputfullname", file.FullName.Substring(0, file.FullName.Length - ".autoproj".Length));
        }

        private void readFromFile(FileInfo dbFile) {
            var sr = new StreamReader(dbFile.OpenRead(), Encoding.UTF8);
            var cont = sr.ReadToEnd();
            sr.Close();

            var db = JsonParser.Parse(cont);

            if (db is JsonObject obj) {
                if (obj.TryGetValue("static_counters", out var val) && val is JsonObject cnts) {
                    foreach (var cnt in cnts.Values) {
                        var name = cnt.Key;
                        ulong value = 0;
                        bool good = false;

                        if (cnt.Value is JsonNumber num && !num.IsFloat && num.ValueInt > 0) {
                            value = (ulong)num.ValueInt;
                            good = true;
                        } else if (cnt.Value is JsonString valueStr && ulong.TryParse(valueStr, out ulong res)) {
                            value = res;
                            good = true;
                        }

                        if (good) {
                            if (_staticCounters.ContainsKey(name)) {
                                // todo: log here, warning
                            } else {
                                _type[name] = AutoprojEnvItemType.StaticCounter;
                                _staticCounters[name] = value;
                            }
                        } else {
                            // todo: log here
                        }
                    }
                } else {
                    // todo: log here
                }
            } else {
                // todo: invalid file, log here
            }
        }

        private void saveToWriter(TextWriter writer) {
            JsonObject scs = new JsonObject();
            _staticCounters.Keys.ForEach(key => scs.Append(key, new JsonString(_staticCounters[key].ToString())));

            JsonObject output = new JsonObject();
            output.Append("static_counters", scs);

            writer.Write(output.ToStringPrettified());
        }

        public void EndUse() {
            if (_staticCounters.Count > 0) {
                var sw = new StreamWriter(new FileStream(_dbFile, FileMode.Create));

                saveToWriter(sw);
                sw.Close();
            }
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
