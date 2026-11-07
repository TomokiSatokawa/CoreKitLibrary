using UnityEngine.InputSystem;

namespace CoreKit.Inputs
{
    public static class InputSource
    {
        private static InputActionAsset _custom;

        /// <summary>Project-wide ActionsˆÈŠO‚ğg‚¤ê‡‚¾‚¯ŒÄ‚Ô</summary>
        public static void Use(InputActionAsset asset)
        {
            _custom = asset;
            asset.Enable();
        }

        internal static InputActionAsset Resolve()
        {
            var asset = _custom != null ? _custom : InputSystem.actions;
            if (asset == null)
                throw new System.InvalidOperationException(
                    "Project-wide Actions‚ª–¢İ’è‚Å‚·BProject Settings > Input System Package ‚Åİ’è‚µ‚Ä‚­‚¾‚³‚¢");
            return asset;
        }

        internal static void Reset() => _custom = null;
    }
}