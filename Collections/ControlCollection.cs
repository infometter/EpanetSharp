using System;
using System.Collections;
using System.Collections.Generic;
using EpanetSharp.Elements;

namespace EpanetSharp.Collections
{
    /// <summary>
    /// Coleção de controles da rede. Suporta modo gerenciado e native-backed.
    /// </summary>
    public sealed class ControlCollection : IReadOnlyCollection<Control>
    {
        private readonly List<Control> _list = new();
        private readonly Dictionary<string, Control> _map = new(StringComparer.Ordinal);

        private readonly Native.NativeContext? _nativeContext;
        private Control?[]? _nativeCache;
        private Dictionary<string, int>? _idToIndexCache;

        public ControlCollection()
        {
        }

        public ControlCollection(Native.NativeContext nativeContext)
        {
            _nativeContext = nativeContext ?? throw new ArgumentNullException(nameof(nativeContext));
        }

        public int Count
        {
            get
            {
                if (_nativeContext is null) return _list.Count;
                if (!_nativeContext.IsProjectCreated) return 0;
                return _nativeContext.GetCount(Native.NativeConstants.EN_CONTROLCOUNT);
            }
        }

        public Control? this[string id]
        {
            get
            {
                if (id is null) throw new ArgumentNullException(nameof(id));
                if (_nativeContext is null)
                {
                    _map.TryGetValue(id, out var c);
                    return c;
                }

                if (_idToIndexCache != null && _idToIndexCache.TryGetValue(id, out var idx)) return this[idx];

                try
                {
                    int nativeIndex = _nativeContext.GetControlIndex(id);
                    return this[nativeIndex - 1];
                }
                catch (EpanetSharp.Exceptions.EpanetException)
                {
                    return null;
                }
            }
        }

        public Control this[int index]
        {
            get
            {
                if (_nativeContext is null) return _list[index];
                int count = Count;
                if (index < 0 || index >= count) throw new ArgumentOutOfRangeException(nameof(index));
                if (_nativeCache is null || _nativeCache.Length != count) _nativeCache = new Control[count];

                var existing = _nativeCache[index];
                if (existing is not null) return existing;

                string id = _nativeContext.GetControlId(index + 1);
                var ctrl = new Control(id, index);
                // load definition
                try
                {
                    ctrl.Definition = _nativeContext.GetControlDefinition(index + 1);
                }
                catch { }
                _nativeCache[index] = ctrl;
                (_idToIndexCache ??= new Dictionary<string, int>(StringComparer.Ordinal))[id] = index;
                return ctrl;
            }
        }

        public void Add(Control control)
        {
            if (control is null) throw new ArgumentNullException(nameof(control));
            if (control.Id is null) throw new ArgumentException("Control.Id must be set.", nameof(control));
            if (_map.ContainsKey(control.Id)) throw new ArgumentException("A control with same id exists.", nameof(control));
            _list.Add(control);
            _map[control.Id] = control;
        }

        public bool Remove(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));
            if (_nativeContext is null)
            {
                if (!_map.TryGetValue(id, out var c)) return false;
                _map.Remove(id);
                return _list.Remove(c);
            }

            int nativeIndex = _nativeContext.GetControlIndex(id);
            _nativeContext.DeleteControl(nativeIndex);
            // Invalidate caches
            _nativeCache = null;
            _idToIndexCache?.Remove(id);
            return true;
        }

        public Control? Find(string id) => this[id];

        public IEnumerator<Control> GetEnumerator()
        {
            if (_nativeContext is null)
            {
                foreach (var c in _list) yield return c;
                yield break;
            }

            int count = Count;
            for (int i = 0; i < count; i++) yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
