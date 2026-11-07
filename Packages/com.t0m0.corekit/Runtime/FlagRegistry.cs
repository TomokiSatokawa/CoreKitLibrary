using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreKit
{
    /// <summary>
    /// •¡”‚Ì—v‹Œ³‚©‚ç‚ÌFlag“o˜^‚ğŠÇ—‚µA1‚Â‚Å‚à“o˜^‚ª‚ ‚ê‚ÎFlag‚ğtrue‚É‚·‚é
    /// </summary>
    public class FlagRegistry
    {
        private List<FlagToken> _tokens = new List<FlagToken>();

        /// <summary>
        /// “o˜^‚ª1‚ÂˆÈã‚ ‚ê‚Îtrue
        /// </summary>
        public bool Flag => _tokens.Count > 0;

        /// <summary>
        /// Flag‚ğTrue‚É‚·‚éB‰ğœ‚É‚Í‚±‚±‚Å•Ô‚µ‚½ƒg[ƒNƒ“‚ª•K—v
        /// </summary>
        public FlagToken RegisterFlag()
        {
            var token = FlagToken.Create();
            _tokens.Add(token);
            return token ;
        }

        /// <summary>
        /// “o˜^‚Ìƒg[ƒNƒ“‚ğ“n‚µ‚ÄFlag‚Ì“o˜^‚ğ‰ğœ‚·‚é
        /// </summary>
        public void UnregisterFlag(FlagToken token)
        {
            if (!_tokens.Remove(token))
            {
                Debug.LogError($"[FlagRegistry] –¢“o˜^‚Ìƒg[ƒNƒ“‚ğ‰ğœ‚µ‚æ‚¤‚Æ‚µ‚Ü‚µ‚½: {token}");
                return;
            }
        }
    }

    /// <summary>
    /// ƒtƒ‰ƒO“o˜^‚ğ¯•Ê‚·‚é‚½‚ß‚Ìƒg[ƒNƒ“
    /// </summary>
    public readonly struct FlagToken : IEquatable<FlagToken>
    {
        private static long _nextId;

        private readonly long _id;

        public bool IsValid => _id != 0;

        private FlagToken(long id)
        {
            _id = id;
        }

        public static FlagToken Create()
        {
            return new FlagToken(++_nextId);
        }

        public bool Equals(FlagToken other)
        {
            return _id == other._id;
        }

        public override bool Equals(object obj)
        {
            return obj is FlagToken other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return _id.ToString();
        }

        public static bool operator ==(FlagToken left, FlagToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FlagToken left, FlagToken right)
        {
            return !left.Equals(right);
        }
    }
}
