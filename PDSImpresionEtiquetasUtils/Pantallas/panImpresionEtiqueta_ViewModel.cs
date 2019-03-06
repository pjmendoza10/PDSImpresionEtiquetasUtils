using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMvvm;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using PDSImpresionEtiquetasUtils.Comun;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    public class panImpresionEtiqueta_ViewModel : panPantallaBase_ViewModel
    {
        public enum eSeleccionImpresion { Impresora = 0, Pantalla = 1 }
        public panImpresionEtiqueta_ViewModel()
            {
                VolverPantallaAnterior_Command = new RelayCommand(VolverPantallaAnterior_Command_Execute, VolverPantallaAnterior_Command_CanExecute);
            }

            #region Overrides

            internal override void Inicializa()
            {
                base.Inicializa();               
            }

            /*internal override void OnRendered()
            {
                base.OnRendered();
            }
            */
            #endregion

            #region BackgroundWorker
            
            #endregion

            
            public ICommand VolverPantallaAnterior_Command { get; set; }
            public bool VolverPantallaAnterior_Command_CanExecute()
            {
                return true;
            }
            public void VolverPantallaAnterior_Command_Execute()
            {
                try
                {
                    //if (PantallaPrincipal.PantallaActual.Identificador == Pantallas.panGeneraEtiqRepro01Historico.CIdentificador) return;

                    //Pantallas.panGeneraEtiqRepro01Historico b_pantalla = new Pantallas.panGeneraEtiqRepro01Historico();
                    IPantallasContenedor b_pantalla = ((IPantallasContenedor)this.View).PantallaAnterior;

                    b_pantalla.PantallaAnterior = PantallaPrincipal.PantallaActual;

                    PantallaPrincipal.BotonMenuPrincipalPulsado_Animaciones();

                    PantallaPrincipal.CambiarPantalla(b_pantalla, true);

                }
                catch (Exception ex)
                {
                    Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
                }
                finally
                {
                    //((panGeneraEtiqRepro01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
                }
            }
        #region Propiedades

        public string TextoBotonImpresion { get => textoBotonImpresion; set { textoBotonImpresion = value; RaisePropertyChanged("TextoBotonImpresion"); } }

        private string textoBotonImpresion = "Imprimir y Guardar";

        public bool IsStoredInBD = false;

        private eSeleccionImpresion _SeleccionImpresion = eSeleccionImpresion.Pantalla;
        public eSeleccionImpresion SeleccionImpresion
        {
            get { return _SeleccionImpresion; }
            set
            {
                _SeleccionImpresion = value;
                RaisePropertyChanged("SeleccionImpresion");
            }
        }
        #endregion
    }
}
