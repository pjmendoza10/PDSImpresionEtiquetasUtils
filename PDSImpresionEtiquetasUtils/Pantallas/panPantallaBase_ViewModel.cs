using MicroMvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PDSIEUCo = PDSImpresionEtiquetasUtils.Comun;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    public class panPantallaBase_ViewModel : ObservableObject
    {

        MainWindow _pantalla_Principal = null;
        object _view = null;

        //SROUCo.Utilidades.csLogUtils _LogUtil;


        //List<UserControl> _lista_controles_scada = new List<UserControl>();

        #region Propiedades

        PDSIEUCo.csConfiguracion _configuracion = null;
        public PDSIEUCo.csConfiguracion Configuracion
        {
            get { return _configuracion; }
            set { _configuracion = value; RaisePropertyChanged("Configuracion"); }
        }

        public MainWindow PantallaPrincipal
        {
            get { return _pantalla_Principal; }
            set
            {
                _pantalla_Principal = value;
            }
        }

        public object View
        {
            get { return _view; }
            set { _view = value; }
        }

        public bool IsDesingMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

        #endregion

        #region Funciones

        internal virtual void Inicializa()
        {
            return;
        }

        internal virtual void Inicializa(object p_parametros)
        {
            Inicializa();

            return;
        }

        #endregion
    }
}
