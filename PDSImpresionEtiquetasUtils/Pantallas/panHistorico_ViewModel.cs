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
using PDSImpresionEtiquetasUtils.Comun.DB;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    public class panHistorico_ViewModel : panPantallaBase_ViewModel
    {
        BackgroundWorker _bkgwk_BuscarDatosEnBDD = new BackgroundWorker();
        public panHistorico_ViewModel()
        {
            VolverPantallaAnterior_Command = new RelayCommand(VolverPantallaAnterior_Command_Execute, VolverPantallaAnterior_Command_CanExecute);
            CargarPantallaEtiqueta_Command = new RelayCommand(CargarPantallaEtiqueta_Command_Execute, CargarPantallaEtiqueta_Command_CanExecute);
            PART_Grid_ListaEtiquetas_DoubleClick = new RelayCommand(PART_Grid_ListaEtiquetas_DoubleClick_Execute);
            BuscarDatos_Command = new RelayCommand(BuscarDatos_Command_Execute, BuscarDatos_Command_CanExecute);
        }
        #region Overrides

        internal override void Inicializa()
        {
            base.Inicializa();            
        }

        #endregion

        #region BackgroundWorker
        void _bkgwk_BuscarDatosEnBDD_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();

            _bkgwk_BuscarDatosEnBDD.Dispose();

            ((panImpresionEtiquetaESTIU)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

            return;
        }

        void _bkgwk_BuscarDatosEnBDD_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;
            try
            {
                if (e.Argument.ToString() == "BUSCARENBDD")
                {
                    DataBaseLayer b_base = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_PDSImpresionEtiquetas);
                    
                    List<DB_pds_progutils_ETIQ01_PALETS_HIST01> db_HIST_item = b_base.DB_pds_progutils_HIST_GetHistoricoEtiqueta(CodEtiquetaSeleccionado);
                    RellenaListaArticuloCliente(db_HIST_item);

                    RaisePropertyChanged("ListaEtiquetas");
                    RaisePropertyChanged("ListaEtiquetas_SelectedItem");
                }
                else
                {
                    e.Result = false;
                }
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(" _bkgwk_BuscarDatosEnBDD_DoWork", ex);
            }
        }
        #endregion

        #region Commands 
        public delegate void RellenaListaArticuloClienteRep_Callback(List<DB_pds_progutils_ETIQ01_PALETS_HIST01> db_list_items);
        public void RellenaListaArticuloCliente(List<DB_pds_progutils_ETIQ01_PALETS_HIST01> db_list_items)
        {
            panHistorico b_vista = (panHistorico)View;

            if (!b_vista.Dispatcher.CheckAccess())
            {
                RellenaListaArticuloClienteRep_Callback d = new RellenaListaArticuloClienteRep_Callback(RellenaListaArticuloCliente);
                b_vista.Dispatcher.Invoke(d, db_list_items);
            }
            else
            {
                IDisposable d = null;

                try
                {
                    ListaEtiquetas.Clear();
                    ListaEtiquetas_SelectedItem = null;
                    foreach (DB_pds_progutils_ETIQ01_PALETS_HIST01 item in db_list_items)
                    {
                        csItem_Historico newitem = new csItem_Historico();
                        newitem.FechaCreacion = item.FechaCreacion;
                        newitem.SSCC = item.SSCC;
                        newitem.UIDEtiqueta = item.UidEtiqueta;
                        if (CodEtiquetaSeleccionado == "2") newitem.Entity =  csEstadoPermanente.Deserialize<csitem_EtiquetaSiroT1>(item.Datos);
                        else if (CodEtiquetaSeleccionado == "3") newitem.Entity = csEstadoPermanente.Deserialize<csitem_EtiquetaEstiuT1>(item.Datos);
                        else newitem.Entity = csEstadoPermanente.Deserialize<csitem_EtiquetaGeneralPaletT1>(item.Datos);
                    }
                }
                catch (Exception ex1)
                {

                }
                finally
                {
                    if (d != null) d.Dispose();
                }
            }

        }

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

        public ICommand BuscarDatos_Command { get; set; }
        public bool BuscarDatos_Command_CanExecute()
        {
            if (_bkgwk_BuscarDatosEnBDD.IsBusy) return false;

            return true;
        }
        public void BuscarDatos_Command_Execute()
        {
            try
            {
                if (_bkgwk_BuscarDatosEnBDD.IsBusy != true)
                {
                    ((panHistorico)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());
                    _bkgwk_BuscarDatosEnBDD.RunWorkerAsync("BUSCARENBDD");
                }
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panHistorico)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public ICommand CargarPantallaEtiqueta_Command { get; set; }

        public bool CargarPantallaEtiqueta_Command_CanExecute()
        {
            return true;
        }
        public void CargarPantallaEtiqueta_Command_Execute()
        {
           
            IPantallasContenedor b_pantalla;
            if (ListaEtiquetas_SelectedItem != null)
            {
                //if (ListaClientesEtiquetas_SelectedItem.Id == 1) b_pantalla = new Pantallas.panImpresionEtiquetaGen01();
                if (ListaEtiquetas_SelectedItem.CodEtiqueta == "2") b_pantalla = new Pantallas.panImpresionEtiquetaSIRO();
                else if (ListaEtiquetas_SelectedItem.CodEtiqueta == "3") b_pantalla = new Pantallas.panImpresionEtiquetaESTIU();
                else b_pantalla = new Pantallas.panImpresionEtiquetaGen01();
            }
            else return;


            if (Utilidades.UtilesCarga._pantallas_abiertas.Any(z => z.Key == b_pantalla.ToString()))
            {
                //if (ListaClientesEtiquetas_SelectedItem.Id == 1) b_pantalla = (Pantallas.panImpresionEtiquetaGen01)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value; 
                if (ListaEtiquetas_SelectedItem.CodEtiqueta == "2") b_pantalla = (Pantallas.panImpresionEtiquetaSIRO)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value;
                else if (ListaEtiquetas_SelectedItem.CodEtiqueta == "3") b_pantalla = (Pantallas.panImpresionEtiquetaESTIU)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value;
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
        public ICommand PART_Grid_ListaEtiquetas_DoubleClick { get; set; }
        public void PART_Grid_ListaEtiquetas_DoubleClick_Execute()
        {
            if (ListaEtiquetas_SelectedItem == null) return;
            else CargarPantallaEtiqueta_Command_Execute();
        }
        #endregion

        #region Propiedades
        private string _codEtiquetaSeleccionado = null;
        public string CodEtiquetaSeleccionado { get => _codEtiquetaSeleccionado; set { _codEtiquetaSeleccionado = value; RaisePropertyChanged("CodEtiquetaSeleccionado"); } }

        private List<csItem_Historico> _listaEtiquetas = new List<csItem_Historico>();

        public List<csItem_Historico> ListaEtiquetas { get => _listaEtiquetas; set { _listaEtiquetas = value; RaisePropertyChanged("ListaEtiquetas"); } }

        private csItem_Historico _listaEtiquetas_SelectedItem = null;

        public csItem_Historico ListaEtiquetas_SelectedItem { get => _listaEtiquetas_SelectedItem; set { _listaEtiquetas_SelectedItem = value; RaisePropertyChanged("ListaEtiquetas_SelectedItem"); } }

        #endregion

        public class csItem_Historico : ObservableObject, IDataErrorInfo
        {
            private string _uidEtiqueta;
            private string _sscc;
            private string _codEtiqueta;
            private string _fechaCreacion;
            private Object _entity;

            public string UIDEtiqueta { get => _uidEtiqueta; set { _uidEtiqueta = value; RaisePropertyChanged("UIDEtiqueta"); } }
            public string SSCC { get => _sscc; set { _sscc = value; RaisePropertyChanged("SSCC"); } }
            public string CodEtiqueta { get => _codEtiqueta; set { _codEtiqueta = value; RaisePropertyChanged("CodEtiqueta"); } }
            public string FechaCreacion { get => _fechaCreacion; set { _fechaCreacion = value; RaisePropertyChanged("FechaCreacion"); } }
            public Object Entity { get => _entity; set { _entity = value; RaisePropertyChanged("Entity"); } }

            public string Error
            {
                get { return ""; }
            }

            public string this[string columnName]
            {
                get { return ""; }
            }
        }
    }
    
}
