//AI
using System;
using UnityEngine;

namespace CoreKit.DynamicEnum
{
    /// <summary>
    /// DynamicEnum‚Ì’l‚ð•\‚·
    /// </summary>
    [Serializable]
    public struct DynamicEnumValue : IEquatable<DynamicEnumValue>
    {
        [SerializeField] private DynamicEnumDefinition _definition;
        [SerializeField] private string _guid;

        public string Guid => _guid;

        public DynamicEnumValue(string guid)
        {
            _definition = null;
            _guid = guid;
        }

        public bool Equals(DynamicEnumValue other)
        {
            return _guid == other._guid;
        }

        public override bool Equals(object obj)
        {
            return obj is DynamicEnumValue other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _guid?.GetHashCode() ?? 0;
        }

        public static bool operator ==(
            DynamicEnumValue left, DynamicEnumValue right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            DynamicEnumValue left, DynamicEnumValue right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            if (_definition == null)
                return "None";

            foreach (var entry in _definition.Entries)
            {
                if (entry.Guid == _guid)
                    return $"{_definition.ClassName}.{entry.Name}";
            }

            return $"{_definition.name}.None";
        }
    }
}