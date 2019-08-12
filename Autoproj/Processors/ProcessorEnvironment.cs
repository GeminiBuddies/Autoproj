using System.Collections.Generic;
using GeminiLab.Autoproj.Evaluators;
using Storage = System.Collections.Generic.IDictionary<string, System.Collections.Generic.IDictionary<string, System.Tuple<object, bool>>>;
using StorageIns = System.Collections.Generic.Dictionary<string, System.Collections.Generic.IDictionary<string, System.Tuple<object, bool>>>;

namespace GeminiLab.Autoproj.Processors {
    public class ProcessorEnvironmentStorageItem {
        public object Value;
        public bool Persistent;
    }

    public class ProcessorEnvironment {
        protected ProcessorEnvironment parent;

        public ProcessorEnvironment(ProcessorEnvironment parent) {
            this.parent = parent;
        }


        // evaluator management
        protected IList<IExpressionEvaluator> evaluators = new List<IExpressionEvaluator>();

        public virtual void AddEvaluator(IExpressionEvaluator evaluator) {
            evaluators.Add(evaluator);
        }

        public virtual bool TryEvaluate(out string result, ProcessorEnvironment env, string command, params string[] param) {
            // later added, earlier checked
            for (int i = evaluators.Count - 1; i >= 0; --i) {
                var evaluator = evaluators[i];
                if (evaluator.TryEvaluate(out result, env, command, param)) return true;
            }

            result = null;
            return parent != null && parent.TryEvaluate(out result, env, command, param);
        }

        public virtual bool TryEvaluate(out string result, string command, params string[] param) =>
            TryEvaluate(out result, this, command, param);

        // storage management
        protected IDictionary<string, IDictionary<string, ProcessorEnvironmentStorageItem>> storage
            = new Dictionary<string, IDictionary<string, ProcessorEnvironmentStorageItem>>();

        public virtual bool StorageTryFind<T>(string category, string key, out ProcessorEnvironmentStorageEntry<T> entry) {
            entry = null;
            if (!storage.TryGetValue(category, out var categoryStorage)) return false;
            if (!categoryStorage.TryGetValue(key, out var item)) return false;

            entry = new ProcessorEnvironmentStorageEntry<T>(this, item);
            return true;
        }

        public virtual bool StorageTryFindRecursively<T>(string category, string key, out ProcessorEnvironmentStorageEntry<T> entry) {
            if (StorageTryFind(category, key, out entry)) return true;
            if (parent != null) return parent.StorageTryFindRecursively<T>(category, key, out entry);

            entry = null;
            return false;
        }

        public virtual ProcessorEnvironmentStorageEntry<T> StorageOpenOrCreate<T>(string category, string key) =>
            StorageOpenOrCreate<T>(category, key, default, false);

        public virtual ProcessorEnvironmentStorageEntry<T> StorageOpenOrCreate<T>(string category, string key, T defaultValue) =>
            StorageOpenOrCreate<T>(category, key, defaultValue, false);

        public virtual ProcessorEnvironmentStorageEntry<T> StorageOpenOrCreate<T>(string category, string key, T defaultValue, bool persistent) {
            if (!storage.TryGetValue(category, out var categoryStorage)) {
                storage[category] = categoryStorage = new Dictionary<string, ProcessorEnvironmentStorageItem>();
            }

            if (!categoryStorage.TryGetValue(key, out var item)) {
                categoryStorage[key] = item = new ProcessorEnvironmentStorageItem { Value = defaultValue, Persistent = persistent };
            }

            return new ProcessorEnvironmentStorageEntry<T>(this, item);
        }

        public virtual void Begin() { }

        public virtual void End() { }
    }
}
