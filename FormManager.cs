using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragnetControl
{
    public class FormManager
    {
        private static MainControl _mainControl;
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
       
    

    
