using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using GeminiLab.Core2.ML.Json;

namespace GeminiLab.Autoproj {
    class AutoprojEnvStored : AutoprojEnv {
        private readonly string _storageFile;

        public AutoprojEnvStored(AutoprojEnv parent, string storagePath) : base(parent) {
            var dbFile = new FileInfo(_storageFile = storagePath);
            if (dbFile.Exists) readFromFile(dbFile);
        }

        // TODO: make these functions working for big files
        private void readFromFile(FileInfo dbFile) {
            var sr = new StreamReader(dbFile.OpenRead(), Encoding.UTF8);
            var cont = sr.ReadToEnd();
            sr.Close();

            var db = JsonParser.Parse(cont);

            if (!(db is JsonObject obj)) return;
            if (!obj.TryGetValue("values", out var v) || !(v is JsonObject values)) return;
            foreach (var pair in values.Values) {
                var name = pair.Key;
                string value = "";
                bool good = false;

                if (pair.Value is JsonNumber num && !num.IsFloat) {
                    value = $"{num.ValueInt}";
                    good = true;
                } else if (pair.Value is JsonString valueStr) {
                    value = valueStr;
                    good = true;
                }

                if (good) initValues[name] = value;
            }
        }

        private static void saveItemsToWriter(Dictionary<string, IAutoprojEnvPersistenceItem> savingItems, TextWriter writer) {
            var scs = new JsonObject();
            foreach (var (key, item) in savingItems) scs.Append(key, new JsonString(item.Save()));

            var output = new JsonObject();
            output.Append("values", scs);

            writer.Write(output.ToStringPrettified());
        }

        protected override void DoEnd() {
            var itemsToSave = new Dictionary<string, IAutoprojEnvPersistenceItem>();

            foreach (var (key, item) in items) {
                if (item is IAutoprojEnvPersistenceItem persistenceItem) {
                    itemsToSave[key] = persistenceItem;
                }
            }

            if (!itemsToSave.Any()) return;

            var sw = new StreamWriter(new FileStream(_storageFile, FileMode.Create));
            saveItemsToWriter(itemsToSave, sw);
            sw.Close();
        }
    }
}
