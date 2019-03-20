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
using System.Drawing.Printing;
using System.Windows.Threading;
using System.Data.SqlClient;
using System.Data;
using PDSImpresionEtiquetasUtils.Comun.DB;
using System.Xml.Serialization;
using PDSImpresionEtiquetasUtils.Conectores;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    public class panImpresionEtiquetaGen03_ViewModel : panPantallaBase_ViewModel
    {
        BackgroundWorker _bkgwk_GuardarEtiquetaEnBDD = new BackgroundWorker();
        BackgroundWorker _bkgwk_BuscarMaterialLote = new BackgroundWorker();
        public enum eSeleccionImpresion { Impresora = 0, Pantalla = 1 }

        private int _n_max_lineas_detalle_palet = 10;

        public panImpresionEtiquetaGen03_ViewModel()
        {
            VolverPantallaAnterior_Command = new RelayCommand(VolverPantallaAnterior_Command_Execute, VolverPantallaAnterior_Command_CanExecute);
            NuevaEtiqueta_Command = new RelayCommand(NuevaEtiqueta_Command_Execute, NuevaEtiqueta_Command_CanExecute);
            ModificarEtiquetaActual_Command = new RelayCommand(ModificarEtiquetaActual_Command_Execute, ModificarEtiquetaActual_Command_CanExecute);
            VerHistoricoEtiqueta_Command = new RelayCommand(VerHistoricoEtiqueta_Command_Execute, VerHistoricoEtiqueta_Command_CanExecute);
            ImprimirEtiqueta_Command = new RelayCommand(ImprimirEtiqueta_Command_Execute, ImprimirEtiqueta_Command_CanExecute);
            BuscarMaterialLote_Command = new RelayCommand(BuscarMaterialLote_Command_Execute, BuscarMaterialLote_Command_CanExecute);
        }
        #region Overrides

        internal override void Inicializa()
        {
            base.Inicializa();

            Entity.NumDesde = 0;
            Entity.NumHasta = 0;
            Entity.NumeroInicial = 1;

            _bkgwk_GuardarEtiquetaEnBDD.DoWork += _bkgwk_GuardarEtiquetaEnBDD_DoWork;
            _bkgwk_GuardarEtiquetaEnBDD.RunWorkerCompleted += _bkgwk_GuardarEtiquetaEnBDD_RunWorkerCompleted;

            _bkgwk_BuscarMaterialLote.WorkerReportsProgress = false;
            _bkgwk_BuscarMaterialLote.WorkerSupportsCancellation = false;
            _bkgwk_BuscarMaterialLote.DoWork += _bkgwk_BuscarMaterialLote_DoWork;
            _bkgwk_BuscarMaterialLote.RunWorkerCompleted += _bkgwk_BuscarMaterialLote_RunWorkerCompleted;
        }

        /*internal override void OnRendered()
        {
            base.OnRendered();
        }
        */
        #endregion

        #region BackgroundWorker
        void _bkgwk_GuardarEtiquetaEnBDD_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();

            _bkgwk_GuardarEtiquetaEnBDD.Dispose();

            ((panImpresionEtiquetaGen03)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

            return;
        }

        void _bkgwk_GuardarEtiquetaEnBDD_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;
            try
            {
                if (e.Argument.ToString() == "GUARDARENBDD")
                {
                    // TODO Serializar Entity
                    DataBaseLayer dbl = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_PDSImpresionEtiquetas);

                    if (String.IsNullOrWhiteSpace(Entity.UIDEtiqueta))
                    {
                        Entity.UIDEtiqueta = Guid.NewGuid().ToString();
                        string serializado = csEstadoPermanente.Serialize(Entity);
                        // TODO Guardar En BDD
                        dbl.DB_pds_progutils_PALETS_Insert(Guid.Parse(Entity.UIDEtiqueta), Entity.SSCC, dbl.DB_pds_progutils_PALETS_GetUIDEtiqueta("5"), serializado);
                    }

                    TextoBotonImpresion = "Reimprimir";
                    IsStoredInBD = true;
                }
                else
                {
                    e.Result = false;
                }
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError("_bkgwk_GuardarEtiquetaEnBDD_DoWork", ex);
            }
        }

        void _bkgwk_BuscarMaterialLote_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();


            _bkgwk_BuscarMaterialLote.Dispose();

            //_HayOFSegunLote = true;
            ((panImpresionEtiquetaGen03)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

            return;
        }

        void _bkgwk_BuscarMaterialLote_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;

            try
            {
                if (e.Argument.ToString() == "ARTICULOLOTE")
                {
                    BuscarArticuloLote(Entity.CodArticulo, true);
                }
                else
                {
                    e.Result = false;
                }
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError("_bkgwk_BuscarMaterialLote_DoWork", ex);
            }
        }
        #endregion

        #region Commands
        public void BuscarArticuloLote(string p_codig_articulo, bool p_reset_datos_pantalla)
        {
            // DataBaseLayer b_base = new DataBaseLayer("Data Source=PDS-BBDD;Initial Catalog=RPS2013;Trusted_Connection=No;User=RPSUser;Password=rpsuser;MultipleActiveResultSets=True");/* FUNCIONA*/

            //DataBaseLayer b_base = new DataBaseLayer("Data Source=localhost;Initial Catalog=RPS2013;Trusted_Connection=No;User=RPSUserDev;Password=rpsuserdev;MultipleActiveResultSets=True");

            DataBaseLayer b_base = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_RPS2013);

            List<DB_pds_progutils_ETIQ01_PALETS_GEN01> db_Gen03_item = b_base.dB_Pds_Progutils_ETIQ01_PALETS_GEN01_GetItems(p_codig_articulo);

            //RellenaListaArticuloCliente(db_Gen03_item);
            foreach (DB_pds_progutils_ETIQ01_PALETS_GEN01 item in db_Gen03_item) {
                Entity.Descripcion = item.Descripcion;
            }

        }

        public ICommand BuscarMaterialLote_Command { get; set; }
        public bool BuscarMaterialLote_Command_CanExecute()
        {
            if (_bkgwk_BuscarMaterialLote.IsBusy) return false;

            return true;
        }
        public void BuscarMaterialLote_Command_Execute()
        {
            try
            {
                ((panImpresionEtiquetaGen03)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                //TextoErrorBuscarMaterialLote = "";

                if (string.IsNullOrWhiteSpace(Entity.CodArticulo))
                {
                    Entity.Descripcion = null;
                    //TextoErrorBuscarMaterialLote = "Código artículo incorrecto";
                    ((panImpresionEtiquetaGen03)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
                    return;
                }

                if (_bkgwk_BuscarMaterialLote.IsBusy != true)
                {
                    //ResetDatosPantalla();
                    _bkgwk_BuscarMaterialLote.RunWorkerAsync("ARTICULOLOTE");
                }

            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen03)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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

        public ICommand NuevaEtiqueta_Command { get; set; }
        public bool NuevaEtiqueta_Command_CanExecute()
        {
            //if (_bkgwk_FiltrarFicheroArticuloLote.IsBusy) return false;

            return true;
        }
        public void NuevaEtiqueta_Command_Execute()
        {
            try
            {
                ((panImpresionEtiquetaGen03)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());
                Entity.CodArticulo = null;
                Entity.Descripcion = null;
                Entity.ListaLineasGridEtiqueta = null;
                Entity.Lote = null;
                Entity.SSCC = null;
                Entity.NumDesde = 0;
                Entity.NumeroInicial = 1;
                Entity.NumHasta = 0;
                Entity.UIDEtiqueta = null;

                TextoBotonImpresion = "Imprimir y Guardar";
                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen03)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public ICommand ModificarEtiquetaActual_Command { get; set; }
        public bool ModificarEtiquetaActual_Command_CanExecute()
        {
            //if (_bkgwk_FiltrarFicheroArticuloLote.IsBusy) return false;

            return true;
        }
        public void ModificarEtiquetaActual_Command_Execute()
        {
            try
            {
                ((panImpresionEtiquetaGen03)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoBotonImpresion = "Imprimir y Guardar";
                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen03)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public ICommand VerHistoricoEtiqueta_Command { get; set; }
        public bool VerHistoricoEtiqueta_Command_CanExecute()
        {
            //if (_bkgwk_FiltrarFicheroArticuloLote.IsBusy) return false;

            return true;
        }
        public void VerHistoricoEtiqueta_Command_Execute()
        {
            panHistorico b_pantalla = new Pantallas.panHistorico("5");

            if (Utilidades.UtilesCarga._pantallas_abiertas.Any(z => z.Key == b_pantalla.ToString()))
            {
                //if (ListaClientesEtiquetas_SelectedItem.Id == 1) b_pantalla = (Pantallas.panImpresionEtiquetaGen01)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value; 
               Utilidades.UtilesCarga._pantallas_abiertas.Remove(b_pantalla.ToString());
            }
            Utilidades.UtilesCarga._pantallas_abiertas.Add(b_pantalla.ToString(), b_pantalla);
            
            //b_pantalla.CargarDatosHistorico();

            b_pantalla.PantallaAnterior = PantallaPrincipal.PantallaActual;

            PantallaPrincipal.BotonMenuPrincipalPulsado_Animaciones();

            PantallaPrincipal.CambiarPantalla(b_pantalla);

            return;
        }
        
        public ICommand ImprimirEtiqueta_Command { get; set; }
        public bool ImprimirEtiqueta_Command_CanExecute()
        {
            //if (_bkgwk_FiltrarFicheroArticuloLote.IsBusy) return false;

            return true;
        }
        public void ImprimirEtiqueta_Command_Execute()
        {
            try
            {
                ((panImpresionEtiquetaGen03)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                if (!CamposCorrectosParaImpresion()) return;
                if (!IsStoredInBD) {
                if (_bkgwk_GuardarEtiquetaEnBDD.IsBusy != true)
                    {
                        _bkgwk_GuardarEtiquetaEnBDD.RunWorkerAsync("GUARDARENBDD");
                    }
                }

                csGeneraEtiqRepro_Comun imprimir = new csGeneraEtiqRepro_Comun();
                PrinterSettings printerSettings = new PrinterSettings();
                imprimir.ImprimeEtiquetaPalet(printerSettings, Entity, Convert.ToBoolean(SeleccionImpresion), true, ((panImpresionEtiquetaGen03)View).Dispatcher);

            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen03)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public Boolean CamposCorrectosParaImpresion() {
            if (String.IsNullOrWhiteSpace(Entity.CodArticulo)) { MessageBox.Show("Falta el campo Codigo Articulo", "Debe rellenar todos los campos obligatorios"); return false; }
            if (String.IsNullOrWhiteSpace(Entity.NumDesde.ToString())) { MessageBox.Show("Falta el campo Nº Desde", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.NumHasta.ToString())) { MessageBox.Show("Falta el campo Nº Hasta", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.NumeroInicial.ToString())) { MessageBox.Show("Falta el campo Numero Inicial", "Debe rellenar todos los campos obligatorios"); return false;}
            return true;
        }
        #endregion

        #region Properties
        private csItem_EtiquetaChinos _Entity = new csItem_EtiquetaChinos();
        public csItem_EtiquetaChinos Entity { get => _Entity; set => _Entity = value; }

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
        public bool ListaLineasGridEtiqueta_CanAdd
        {
            get { return (Entity.ListaLineasGridEtiqueta.Count() < _n_max_lineas_detalle_palet); }
        }
        #endregion
    }

    public class csItem_EtiquetaChinos : ObservableObject, IDataErrorInfo
    {
        private string _codArticulo;
        private string _descripcion;
        private string _lote;
        private int _numDesde;
        private int _numHasta;
        private int _numeroInicial;
        private string _sscc;
        private string _uidEtiqueta;
        private ObservableCollection<csItem_ListaLineasGridEtiqueta> _ListaLineasGridEtiqueta = new ObservableCollection<csItem_ListaLineasGridEtiqueta>();
        public ObservableCollection<csItem_ListaLineasGridEtiqueta> ListaLineasGridEtiqueta
        {
            get { return _ListaLineasGridEtiqueta; }
            set
            {
                _ListaLineasGridEtiqueta = value;
                RaisePropertyChanged("ListaLineasGridEtiqueta");
            }
        }

        private csItem_ListaLineasGridEtiqueta _ListaLineasGridEtiqueta_SelectedItem = null;
        public csItem_ListaLineasGridEtiqueta ListaLineasGridEtiqueta_SelectedItem
        {
            get { return _ListaLineasGridEtiqueta_SelectedItem; }
            set { _ListaLineasGridEtiqueta_SelectedItem = value; RaisePropertyChanged("ListaLineasGridEtiqueta_SelectedItem"); }
        }
        public string UIDEtiqueta { get => _uidEtiqueta; set { _uidEtiqueta = value; RaisePropertyChanged("UIDEtiqueta"); } }        
        public string CodArticulo { get => _codArticulo; set { _codArticulo = value; RaisePropertyChanged("CodArticulo"); } }
        public string Descripcion { get => _descripcion; set { _descripcion = value; RaisePropertyChanged("Descripcion"); } }
        public string Lote { get => _lote; set { _lote = value; RaisePropertyChanged("Lote"); } }
        public int NumDesde { get => _numDesde; set { _numDesde = value; RaisePropertyChanged("NumDesde"); } }
        public int NumHasta { get => _numHasta; set { _numHasta = value; RaisePropertyChanged("NumHasta"); } }
        public int NumeroInicial { get => _numeroInicial; set { _numeroInicial = value; RaisePropertyChanged("NumeroInicial");
                long num1 = long.Parse(DateTime.Today.ToString("yyMMdd")) * 1000L + (long)System.Convert.ToInt32(value); SSCC = num1.ToString(); } }
        public string SSCC { get => _sscc; set { _sscc = value; RaisePropertyChanged("SSCC"); } }

        public string this[string columnName]
        {
            get { return ""; }
        }
        public string Error
        {
            get { return ""; }
        }
    }
}
