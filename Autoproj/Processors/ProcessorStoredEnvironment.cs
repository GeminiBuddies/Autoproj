using System.Collections.Generic;
using System.IO;
using System.Text;
using GeminiLab.Core2.ML.Json;

namespace GeminiLab.Autoproj.Processors {
    public class ProcessorStoredEnvironment : ProcessorEnvironment {
        private ProcessorStoredEnvironment(ProcessorEnvironment parent) : base(parent) { }

        private readonly string _filename;

        public ProcessorStoredEnvironment(ProcessorEnvironment parent, string filename) : this(parent) {
            _filename = filename;
        }

        public sealed override void Begin() {
            base.Begin();

            if (!File.Exists(_filename)) return;

            using (var fs = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (var sr = new StreamReader(fs, Encoding.UTF8)) {
                    var content = sr.ReadToEnd();

                    if (!(JsonParser.Parse(content) is JsonObject obj)) return;

                    foreach (var (category, jsonValue) in obj.Values) {
                        if (!(jsonValue is JsonObject dict)) continue;

                        if (!storage.ContainsKey(category)) storage[category] = new Dictionary<string, object>();
                        var categoryStorage = storage[category];

                        foreach (var (key, value) in dict.Values) {
                            switch (value) {
                            case JsonString str:
                                categoryStorage[key] = (string)str;
                                break;
                            case JsonBool boolean:
                                categoryStorage[key] = (bool)boolean;
                                break;
                            case JsonNumber number:
                                categoryStorage[key] = number.IsFloat ? number.ValueFloat : (object)number.ValueInt;
                                break;
                            case JsonNull _:
                                categoryStorage[key] = null;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public sealed override void End() {
            var root = new JsonObject();

            foreach (var (cat, dict) in storage) {
                var obj = new JsonObject();

                foreach (var (key, value) in dict) {
                    switch (value) {
                    case string str:
                        obj[key] = str;
                        break;
                    case int i:
                        obj[key] = i;
                        break;
                    case float f:
                        obj[key] = f;
                        break;
                    case bool b:
                        obj[key] = b;
                        break;
                    case null:
                        obj[key] = new JsonNull();
                        break;
                    }
                }

                if (obj.Count > 0) root[cat] = obj;
            }

            if (root.Count > 0) {
                using (var fs = new FileStream(_filename, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    using (var sw = new StreamWriter(fs, new UTF8Encoding(false))) {
                        sw.Write(root.ToStringPrettified());
                    }
                }
            }

            base.End();
        }
    }
}