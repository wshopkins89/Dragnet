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

        public static MainControl MainControl
        {
            get
            {
                if (_mainControl == null || _mainControl.IsDisposed)

                {
                    _mainControl = new MainControl();
                }

                return _mainControl;
            }
        }
    }
}
       
    

    
