using MicroMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PDSIEUCo = PDSImpresionEtiquetasUtils.Comun;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    class panInicial_ViewModel : panPantallaBase_ViewModel
    {
        public panInicial_ViewModel()
            : base()
        {
            CambiarUsuario_Command = new RelayCommand(CambiarUsuario_Command_Execute, CambiarUsuario_Command_CanExecute);
        }
        #region Command

        public ICommand CambiarUsuario_Command { get; set; }
        public bool CambiarUsuario_Command_CanExecute()
        {
            return true;
        }

        public void CambiarUsuario_Command_Execute()
        {
            int b_res = this.PantallaPrincipal.ViewModel.EjecutaProcesoLogin(true);

            RaisePropertyChanged("CodUsuarioActual");

            if (b_res == 1)
            {

                this.PantallaPrincipal.ViewModel.CerrarTodasVentanasLibres();

                this.PantallaPrincipal.ViewModel.AplicarOpcionesUsuario(Configuracion.CodUsuarioActual);
            }
        }

        #endregion

        #region Overrides
        internal override void Inicializa()
        {
            base.Inicializa();

            RaisePropertyChanged("CodUsuarioActual");
        }

        #endregion

        #region Properties


        public string CodUsuarioActual
        {
            get
            {
                if (Configuracion != null) return Configuracion.CodUsuarioActual;
                else return "--";
            }
        }

        #endregion

    }
}

