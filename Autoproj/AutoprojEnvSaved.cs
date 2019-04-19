using GeminiLab.Core2.ML.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeminiLab.Autoproj {
    class AutoprojEnvSaved : AutoprojEnv {
        private readonly string _dbFile = null;

        public AutoprojEnvSaved(AutoprojEnv parent, string dbPath) : base(parent) {
            var dbFile = new FileInfo(_dbFile = dbPath);
            if (dbFile.Exists) readFromFile(dbFile);
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
            foreach (var key in _staticCounters.Keys) scs.Append(key, new JsonString(_staticCounters[key].ToString()));

            JsonObject output = new JsonObject();
            output.Append("static_counters", scs);

            writer.Write(output.ToStringPrettified());
        }

        protected override void DoEnd() {
            if (_staticCounters.Count > 0) {
                var sw = new StreamWriter(new FileStream(_dbFile, FileMode.Create));

                saveToWriter(sw);
                sw.Close();
            }
        }
    }
}
