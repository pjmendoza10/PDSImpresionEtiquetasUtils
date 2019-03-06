using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDSImpresionEtiquetasUtils.Comun
{
    public interface IPantallasContenedor
    {
        object Window { get; set; }
        Guid IDUnico { get; }
        string Identificador { get; }
        object View { get; }

        csConfiguracion Configuracion { get; set; }

        void SetCursor(string p_cursor_name);

        IPantallasContenedor PantallaAnterior { get; set; }
    }
}
