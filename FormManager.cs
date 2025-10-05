using System;

namespace DragnetControl
{
    public static class FormManager
    {
        private static MainControl? _mainControl;
        private static Func<MainControl> _factory = () => new MainControl();

        public static MainControl MainControl
        {
            get
            {
                if (_mainControl == null || _mainControl.IsDisposed)
                {
                    _mainControl = _factory();
                }

                return _mainControl;
            }
        }

        public static void Configure(Func<MainControl> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            if (_mainControl != null && !_mainControl.IsDisposed)
            {
                _mainControl.Dispose();
            }

            _mainControl = null;
        }
    }
}
