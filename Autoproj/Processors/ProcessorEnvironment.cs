using System.Collections.Generic;
using GeminiLab.Autoproj.Evaluators;

namespace GeminiLab.Autoproj.Processors {
    public class ProcessorEnvironment {
        protected ProcessorEnvironment parent;

        public ProcessorEnvironment(ProcessorEnvironment parent) {
            this.parent = parent;
        }

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

        protected IDictionary<string, IDictionary<string, object>> storage = new Dictionary<string, IDictionary<string, object>>();

        public virtual bool TryFindStorage<T>(string category, string key, out ProcessorEnvironmentStorageItem<T> item) {
            bool rv = false;

            if (storage.TryGetValue(category, out var categoryStorage)) {
                if (categoryStorage.TryGetValue(key, out _)) rv = true;
            }

            item = new ProcessorEnvironmentStorageItem<T>(this, categoryStorage, key);
            return rv;
        }

        public virtual bool TryFindStorageRecursively<T>(string category, string key, out ProcessorEnvironmentStorageItem<T> item) {
            if (!storage.TryGetValue(category, out var categoryStorage) || !categoryStorage.TryGetValue(key, out _)) {
                if (parent != null) return parent.TryFindStorageRecursively(category, key, out item);

                item = new ProcessorEnvironmentStorageItem<T>(this, null, key);
                return false;

            }

            item = new ProcessorEnvironmentStorageItem<T>(this, categoryStorage, key);
            return true;
        }

        public virtual ProcessorEnvironmentStorageItem<T> OpenStorage<T>(string category, string key) {
            if (!storage.TryGetValue(category, out var categoryStorage)) {
                storage[category] = categoryStorage = new Dictionary<string, object>();
            }

            return new ProcessorEnvironmentStorageItem<T>(this, categoryStorage, key);
        }

        public virtual void Begin() { }

        public virtual void End() { }
    }
}
