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
    public class panTipoEtiqueta_ViewModel : panPantallaBase_ViewModel
    {     

        public panTipoEtiqueta_ViewModel()
        {
            VolverPantallaAnterior_Command = new RelayCommand(VolverPantallaAnterior_Command_Execute, VolverPantallaAnterior_Command_CanExecute);
            VerPantallaEtiqueta_Command = new RelayCommand(VerPantallaEtiqueta_Command_Execute, VerPantallaEtiqueta_Command_CanExecute);
            PART_Grid_ListaClientesEtiquetas_DoubleClick = new RelayCommand(PART_Grid_ListaClientesEtiquetas_DoubleClick_Execute);
        }
        #region Overrides

        internal override void Inicializa()
        {
            base.Inicializa();

            csItem_ClienteEtiqueta Gen = new csItem_ClienteEtiqueta();
            csItem_ClienteEtiqueta Estiu = new csItem_ClienteEtiqueta();
            csItem_ClienteEtiqueta Siro = new csItem_ClienteEtiqueta();
            csItem_ClienteEtiqueta Iberbag = new csItem_ClienteEtiqueta();
            Gen.NombreCliente = "Cliente Mercadona (General)";
            Gen.NombrePantalla = "panImpresionEtiquetaGen01";
            Gen.Id = 1;
            Siro.NombreCliente = "Cliente Grupo SIRO";
            Siro.NombrePantalla = "panImpresionEtiquetaSIRO";
            Siro.Id = 2;
            Estiu.NombreCliente = "Cliente Helados ESTIU";
            Estiu.NombrePantalla = "panImpresionEtiquetaESTIU";
            Estiu.Id = 3;
            Iberbag.NombreCliente = "Cliente Iberbag";
            Iberbag.NombrePantalla = "panImpresionEtiquetaGen04";
            Iberbag.Id = 4;
            /*ListaClientesEtiquetas.Add(Gen);
            ListaClientesEtiquetas.Add(Siro);
            ListaClientesEtiquetas.Add(Estiu);*/
            ListaClientesEtiquetas.Add(Iberbag);
        }

        /*internal override void OnRendered()
        {
            base.OnRendered();
        }
        */
        #endregion

        #region BackgroundWorker

        #endregion

        #region Commands  
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

        public ICommand VerPantallaEtiqueta_Command { get; set; }

        public bool VerPantallaEtiqueta_Command_CanExecute()
        {
            return true;
        }
        public void VerPantallaEtiqueta_Command_Execute()
        {
            /*Pantallas.panImpresionEtiquetaSIRO b_pantalla = new Pantallas.panImpresionEtiquetaSIRO();

            if (Utilidades.UtilesCarga._pantallas_abiertas.Any(z => z.Key == b_pantalla.ToString()))
            {
                b_pantalla = (Pantallas.panImpresionEtiquetaSIRO)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value;
            }*/
            IPantallasContenedor b_pantalla;
            if (ListaClientesEtiquetas_SelectedItem != null)
            {
                //if (ListaClientesEtiquetas_SelectedItem.Id == 1) b_pantalla = new Pantallas.panImpresionEtiquetaGen01();
                if (ListaClientesEtiquetas_SelectedItem.Id == 2) b_pantalla = new Pantallas.panImpresionEtiquetaSIRO();
                else if (ListaClientesEtiquetas_SelectedItem.Id == 3) b_pantalla = new Pantallas.panImpresionEtiquetaESTIU();
                else if (ListaClientesEtiquetas_SelectedItem.Id == 4) b_pantalla = new Pantallas.panImpresionEtiquetaGen04();
                else b_pantalla = new Pantallas.panImpresionEtiquetaGen01();
            }
            else return;


            if (Utilidades.UtilesCarga._pantallas_abiertas.Any(z => z.Key == b_pantalla.ToString()))
            {
                //if (ListaClientesEtiquetas_SelectedItem.Id == 1) b_pantalla = (Pantallas.panImpresionEtiquetaGen01)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value; 
                if (ListaClientesEtiquetas_SelectedItem.Id == 2) b_pantalla = (Pantallas.panImpresionEtiquetaSIRO)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value;
                else if (ListaClientesEtiquetas_SelectedItem.Id == 3) b_pantalla = (Pantallas.panImpresionEtiquetaESTIU)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value;
                else if (ListaClientesEtiquetas_SelectedItem.Id == 4) b_pantalla = (Pantallas.panImpresionEtiquetaGen04)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value;
                else b_pantalla = (Pantallas.panImpresionEtiquetaGen01)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value;
            }
            else
            {
                Utilidades.UtilesCarga._pantallas_abiertas.Add(b_pantalla.ToString(), b_pantalla);
            }
            b_pantalla.PantallaAnterior = PantallaPrincipal.PantallaActual;

            PantallaPrincipal.BotonMenuPrincipalPulsado_Animaciones();

            PantallaPrincipal.CambiarPantalla(b_pantalla);

            return;
        }
        public ICommand PART_Grid_ListaClientesEtiquetas_DoubleClick { get; set; }
        public void PART_Grid_ListaClientesEtiquetas_DoubleClick_Execute()
        {
            if (ListaClientesEtiquetas_SelectedItem == null) return;
            else VerPantallaEtiqueta_Command_Execute();
        }
        #endregion

        #region Propiedades
        private List<csItem_ClienteEtiqueta> _listaClientesEtiquetas = new List<csItem_ClienteEtiqueta>();

        public List<csItem_ClienteEtiqueta> ListaClientesEtiquetas { get => _listaClientesEtiquetas; set { _listaClientesEtiquetas = value; RaisePropertyChanged("ListaClientesEtiquetas"); } }

        private csItem_ClienteEtiqueta _listaClientesEtiquetas_SelectedItem = null;

        public csItem_ClienteEtiqueta ListaClientesEtiquetas_SelectedItem { get => _listaClientesEtiquetas_SelectedItem; set { _listaClientesEtiquetas_SelectedItem = value; RaisePropertyChanged("ListaClientesEtiquetas_SelectedItem"); } }

        #endregion
    }
    public class csItem_ClienteEtiqueta
    {
        private int _id;
        private string _nombreCliente;
        private string _nombrePantalla;
        private string _tipoEtiqueta;

        public string NombreCliente { get => _nombreCliente; set => _nombreCliente = value; }
        public string NombrePantalla { get => _nombrePantalla; set => _nombrePantalla = value; }
        public string TipoEtiqueta { get => _tipoEtiqueta; set => _tipoEtiqueta = value; }
        public int Id { get => _id; set => _id = value; }
    }
}
