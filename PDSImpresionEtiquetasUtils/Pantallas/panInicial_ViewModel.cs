using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDSIEUCo = PDSImpresionEtiquetasUtils.Comun;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    class panInicial_ViewModel : panPantallaBase_ViewModel
    {
        public panInicial_ViewModel()
            : base()
        {

        }

        #region Overrides

        internal override void Inicializa()
        {
            base.Inicializa();

            //RaisePropertyChanged("NombreUsuarioActual");
        }

        #endregion

        #region Properties

        //public string NombreUsuarioActual
        //{
        //    get
        //    {
        //        if (Configuracion != null) return Configuracion.NombreUsuarioActual;
        //        else return "--";
        //    }
        //}

        #endregion

    }
}

