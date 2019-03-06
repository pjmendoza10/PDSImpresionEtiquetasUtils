using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDSImpresionEtiquetasUtils.Comun.Utilidades
{
    public class VariosComunesWindows
    {
        #region Varios

        public static System.Windows.Input.Cursor GetCursorByName(string p_cursor_name)
        {
            switch (p_cursor_name)
            {
                case "Arrow":
                    return System.Windows.Input.Cursors.Arrow;
                    //break;
                case "Wait":
                    return System.Windows.Input.Cursors.Wait;
                    //break;
                default:
                    return System.Windows.Input.Cursors.Arrow;
                    //break;
            }
        }

        #endregion
    }
}
