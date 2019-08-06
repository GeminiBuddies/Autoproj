using System.Collections.Generic;

namespace GeminiLab.Autoproj.Processor {
    public class ProcessorEnvironmentStorageItem<T> {
        public ProcessorEnvironment Environment { get; }
        private readonly IDictionary<string, object> _dictionary;
        private readonly string _key;

        public ProcessorEnvironmentStorageItem(ProcessorEnvironment environment, IDictionary<string, object> dictionary, string key) {
            Environment = environment;
            _dictionary = dictionary;
            _key = key;
        }

        public T Value {
            get => (T)_dictionary[_key];
            set => _dictionary[_key] = value;
        }

        public bool Exists => _dictionary.ContainsKey(_key);
    }
}
