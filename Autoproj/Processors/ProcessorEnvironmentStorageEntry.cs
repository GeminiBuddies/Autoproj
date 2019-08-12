using System;
using System.Collections.Generic;

namespace GeminiLab.Autoproj.Processors {
    public class ProcessorEnvironmentStorageEntry<T> {
        public ProcessorEnvironment Environment { get; }

        private readonly ProcessorEnvironmentStorageItem _item;

        public ProcessorEnvironmentStorageEntry(ProcessorEnvironment environment, ProcessorEnvironmentStorageItem item) {
            Environment = environment;
            _item = item;
        }

        public T Value {
            get => (T)_item.Value;
            set => _item.Value = value;
        }

        public bool Persistent {
            get => _item.Persistent;
            set => _item.Persistent = value;
        }
    }
}
