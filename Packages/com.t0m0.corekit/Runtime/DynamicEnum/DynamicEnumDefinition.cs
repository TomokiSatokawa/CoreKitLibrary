using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreKit.DynamicEnum
{
    /// <summary>
    /// DynamicEnum‚Ì‘I‘ğˆ‚ğŠÇ—‚·‚é
    /// </summary>
    [CreateAssetMenu(menuName = "CoreKit/Dynamic Enum/Definition")]
    public class DynamicEnumDefinition : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string Name;
            public string Guid;
        }

        [SerializeField] private string _className = "NewDynamicEnum";
        [SerializeField] private List<Entry> _entries = new();

        [SerializeField, HideInInspector]
        private string _previousClassName;

        public string ClassName => _className;
        public IReadOnlyList<Entry> Entries => _entries;
        public string PreviousClassName => _previousClassName;

        public void SetPreviousClassName(string className)
        {
            _previousClassName = className;
        }

        private void OnValidate()
        {
            var guids = new HashSet<string>();

            foreach (var entry in _entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Guid) ||
                    !guids.Add(entry.Guid))
                {
                    entry.Guid = System.Guid.NewGuid().ToString("N");
                    guids.Add(entry.Guid);
                }
            }
        }
    }
}