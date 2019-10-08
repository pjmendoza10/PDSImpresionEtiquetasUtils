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
using PDSImpresionEtiquetasUtils.Comun.DB;
using System.Collections;
using PDSImpresionEtiquetasUtils.Conectores;
using System.Diagnostics;
using System.Threading;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    public class panImpresionEtiquetaGen04_ViewModel : panPantallaBase_ViewModel
    {
        BackgroundWorker _bkgwk_GuardarEtiquetaEnBDD = new BackgroundWorker();
        BackgroundWorker _bkgwk_BuscarMaterialLote = new BackgroundWorker();
        public enum eSeleccionImpresion { Impresora = 0, Pantalla = 1 }
        public enum eSeleccionDescripcion { Articulo = 0, ArticuloCliente = 1 }

        private int _n_max_lineas_detalle_palet = 10;

        public panImpresionEtiquetaGen04_ViewModel()
        {
            VolverPantallaAnterior_Command = new RelayCommand(VolverPantallaAnterior_Command_Execute, VolverPantallaAnterior_Command_CanExecute);
            NuevaEtiqueta_Command = new RelayCommand(NuevaEtiqueta_Command_Execute, NuevaEtiqueta_Command_CanExecute);
            ModificarEtiquetaActual_Command = new RelayCommand(ModificarEtiquetaActual_Command_Execute, ModificarEtiquetaActual_Command_CanExecute);
            VerHistoricoEtiqueta_Command = new RelayCommand(VerHistoricoEtiqueta_Command_Execute, VerHistoricoEtiqueta_Command_CanExecute);
            ImprimirEtiqueta_Command = new RelayCommand(ImprimirEtiqueta_Command_Execute, ImprimirEtiqueta_Command_CanExecute);
            BuscarMaterialLote_Command = new RelayCommand(BuscarMaterialLote_Command_Execute, BuscarMaterialLote_Command_CanExecute);
            BuscarPorOF_Command = new RelayCommand(BuscarPorOF_Command_Execute, BuscarPorOF_Command_CanExecute);

            PART_Grid_ListaLineasGridEtiqueta_DoubleClick = new RelayCommand(PART_Grid_ListaLineasGridEtiqueta_DoubleClick_Execute);

            Entity.ListaLineasGridEtiqueta.CollectionChanged += ListaLineasGridEtiqueta_CollectionChanged;
        }
        #region Overrides

        internal override void Inicializa()
        {
            base.Inicializa();
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
           
            ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

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
                   /* string serializado = csEstadoPermanente.Serialize(Entity);
                    DataBaseLayer dbl = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_PDSImpresionEtiquetas);

                    // TODO Guardar En BDD
                    Entity.UIDEtiqueta = Guid.NewGuid().ToString();
                    dbl.DB_pds_progutils_PALETS_Insert(Guid.Parse(Entity.UIDEtiqueta), Entity.Sscc, dbl.DB_pds_progutils_PALETS_GetUIDEtiqueta("1"), serializado);
                    */
                    IsStoredInBD = true;
                    TextoBotonImpresion = "Reimprimir";
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
            ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

            return;
        }
        void _bkgwk_BuscarMaterialLote_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;

            try
            {
                if (e.Argument.ToString() == "ARTICULOLOTE")
                {
                    BuscarArticuloLote(_IN_CodArticulo, _IN_CodLote, true);
                }
                /*else if (e.Argument.ToString() == "ORDENFABRICACION")
                {
                    BuscarPorOF(_IN_CodLote);
                }*/
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

        public delegate void RellenaListaArticuloCliente_Callback(List<DB_pds_progutils_ETIQ01_PALETS_GEN01> db_list_items);
        public void RellenaListaArticuloCliente(List<DB_pds_progutils_ETIQ01_PALETS_GEN01> db_list_items)
        {
            panImpresionEtiquetaGen04 b_vista = (panImpresionEtiquetaGen04)View;

            if (!b_vista.Dispatcher.CheckAccess())
            {
                RellenaListaArticuloCliente_Callback d = new RellenaListaArticuloCliente_Callback(RellenaListaArticuloCliente);
                b_vista.Dispatcher.Invoke(d, db_list_items);
            }
            else
            {
                IDisposable d = null;

                try
                {
                    _ListaArticuloCliente.Clear();
                    _ListaArticuloCliente_SelectedItem = null;
                    PAN_ArticuloDescripcion = "";

                    if (db_list_items.Count == 0) TextoErrorBuscarMaterialLote = "Ningún artículo encontrado";
                    else TextoErrorBuscarMaterialLote = ""; 

                    foreach (DB_pds_progutils_ETIQ01_PALETS_GEN01 i_item in db_list_items)
                    {
                        DATO_CodigoArticulo = i_item.CodArticulo;
                        DATO_CodigoLote = IN_CodLote;
                        Entity.CodArticulo = DATO_CodigoArticulo;
                        Entity.Lote = DATO_CodigoLote;
                        Entity.Ean13 = i_item.EAN13;
                        Entity.Sscc = "(01)0" + i_item.EAN13 + "(10)" + DATO_CodigoLote;
                        PAN_ArticuloDescripcion = i_item.Descripcion;
                        Entity.Descripcion = PAN_ArticuloDescripcion;
                        csItem_ListaArticuloCliente b_lac = new csItem_ListaArticuloCliente();
                        b_lac.IDCustomerArticle = i_item.CodArticulo;
                        b_lac.DescripcionCustomer = i_item.NombreCliente;
                        b_lac.DescripcionArticuloCliente = i_item.DescripcionCliente;
                        b_lac.CodCustomer = i_item.IDCustomer;
                        b_lac.RefArticuloCliente = i_item.ReferenciaCliente;
                        if (i_item.DescripcionCliente.Trim() != "") b_lac.TieneDescripcionArticulo = true;
                        else b_lac.TieneDescripcionArticulo = false;

                        //_ListaArticuloCliente.Add(b_lac);
                    }

                    TextoBotonImpresion = "Imprimir y Guardar";
                    IsStoredInBD = false;
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

        public void ActualizarDatosTotales()
        {
            Entity.TotalCajas = 0; Entity.TotalKgAprox = 0; Entity.TotalUds = 0;
            foreach (csItem_ListaLineasGridEtiqueta item in Entity.ListaLineasGridEtiqueta)
            {
                Entity.TotalCajas += item.NumCajas; Entity.TotalKgAprox += item.KgsAprox; Entity.TotalUds += item.TotalUds;
            }
        }
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
            return true;
        }
        public void NuevaEtiqueta_Command_Execute()
        {
            try
            {
                ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                _IN_CodArticulo = "";
                _IN_CodLote = "";
                DATO_CodigoArticulo = "";
                DATO_CodigoLote = "";
                PAN_ArticuloDescripcion = "";
                PAN_CodigoLote_Comentario = "";
                TextoErrorBuscarMaterialLote = "";
                Entity.CodArticulo = "";
                Entity.Lote = "";
                Entity.TotalCajas = 0;
                Entity.TotalKgAprox = 0;
                Entity.TotalUds = 0;
                Entity.Descripcion = "";
                Entity.Fecha = "";
                Entity.Ean13 = "";
                Entity.ListaLineasGridEtiqueta.Clear();
                ListaArticuloCliente.Clear();

                TextoBotonImpresion = "Imprimir y Guardar";
                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public ICommand ModificarEtiquetaActual_Command { get; set; }
        public bool ModificarEtiquetaActual_Command_CanExecute()
        {
            return true;
        }
        public void ModificarEtiquetaActual_Command_Execute()
        {
            try
            {
                ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoBotonImpresion = "Imprimir y Guardar";
                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public ICommand VerHistoricoEtiqueta_Command { get; set; }
        public bool VerHistoricoEtiqueta_Command_CanExecute()
        {
            return true;
        }
        public void VerHistoricoEtiqueta_Command_Execute()
        {
            panHistorico b_pantalla = new Pantallas.panHistorico("4");

            if (Utilidades.UtilesCarga._pantallas_abiertas.Any(z => z.Key == b_pantalla.ToString()))
            {
                //if (ListaClientesEtiquetas_SelectedItem.Id == 1) b_pantalla = (Pantallas.panImpresionEtiquetaGen04)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value; 
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
           return true;
        }
        public void ImprimirEtiqueta_Command_Execute()
        {
            try
            {
                ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                if (!CamposCorrectosParaImpresion()) return;
                Entity.Lote = DATO_CodigoLote;
                csGeneraEtiqRepro_Comun imprimir = new csGeneraEtiqRepro_Comun();
                PrinterSettings printerSettings = new PrinterSettings();
                imprimir.ImprimeEtiquetaPaletIberbag(printerSettings, Entity, Convert.ToBoolean(SeleccionImpresion), true, ((panImpresionEtiquetaGen04)View).Dispatcher);

                /*if (!IsStoredInBD)
                {
                    if (_bkgwk_GuardarEtiquetaEnBDD.IsBusy != true)
                    {
                        _bkgwk_GuardarEtiquetaEnBDD.RunWorkerAsync("GUARDARENBDD");
                    }
                }*/

            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }
        public Boolean CamposCorrectosParaImpresion()
        {
            if (String.IsNullOrWhiteSpace(DATO_CodigoArticulo)) { MessageBox.Show("Falta el campo Cod Articulo", "Debe rellenar todos los campos obligatorios"); return false; }
            if (!Convert.ToBoolean(SeleccionDescripcion01)) {
                if (String.IsNullOrWhiteSpace(PAN_ArticuloDescripcion)) {
                    MessageBox.Show("Falta el campo Descripcion o está vacío", "Debe rellenar todos los campos obligatorios"); return false;
                }
            }
            else if ((ListaArticuloCliente_SelectedItem == null || String.IsNullOrWhiteSpace(ListaArticuloCliente_SelectedItem.DescripcionArticuloCliente)) && String.IsNullOrWhiteSpace(PAN_ArticuloDescripcion))
            {
                MessageBox.Show("Falta el campo Descripcion Artículo para el cliente o está vacío", "Debe rellenar todos los campos obligatorios"); return false;
            }
            if (String.IsNullOrWhiteSpace(DATO_CodigoLote)) { MessageBox.Show("Falta el campo Lote", "Debe rellenar todos los campos obligatorios"); return false; }
            if(UtilizaRefArticuloCliente && String.IsNullOrWhiteSpace(ListaArticuloCliente_SelectedItem.RefArticuloCliente)) {
                MessageBox.Show("La referencia del Artículo para el cliente no existe, se va a imprimir nuestro Codigo Artículo "+ DATO_CodigoArticulo, "Debe rellenar todos los campos obligatorios"); UtilizaRefArticuloCliente = false; return false; }
            return true;
        }
        public ICommand PART_Grid_ListaLineasGridEtiqueta_DoubleClick { get; set; }
        public void PART_Grid_ListaLineasGridEtiqueta_DoubleClick_Execute()
        {
            if (Entity.ListaLineasGridEtiqueta_SelectedItem == null) return;
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
                ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoErrorBuscarMaterialLote = "";

                if (string.IsNullOrWhiteSpace(_IN_CodArticulo.Trim()))
                {
                    TextoErrorBuscarMaterialLote = "Código artículo incorrecto";
                    ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public ICommand BuscarPorOF_Command { get; set; }
        public bool BuscarPorOF_Command_CanExecute()
        {
            //if (string.IsNullOrWhiteSpace(_IN_CodLote.Trim())) return false;
            if (_bkgwk_BuscarMaterialLote.IsBusy) return false;

            return false;
        }
        public void BuscarPorOF_Command_Execute()
        {
            try
            {
                ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoErrorBuscarMaterialLote = "";
                NuevaEtiqueta_Command_Execute();

                if (string.IsNullOrWhiteSpace(_IN_CodArticulo.Trim()))
                {
                    TextoErrorBuscarMaterialLote = "Código artículo incorrecto";
                    ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaGen04)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        void ListaLineasGridEtiqueta_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:

                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:

                    break;
            }

            RaisePropertyChanged("ListaLineasGridEtiqueta_CanAdd");

            // Recalculamos los numeros de lineas
            int b_numero_linea = 1;
            foreach (csItem_ListaLineasGridEtiqueta i_item in Entity.ListaLineasGridEtiqueta)
            {
                i_item.NumeroLinea = b_numero_linea;
                b_numero_linea++;
            }
        }

        public void BuscarArticuloLote(string p_codig_articulo, string p_codigo_lote, bool p_reset_datos_pantalla)
        {
            // DataBaseLayer b_base = new DataBaseLayer("Data Source=PDS-BBDD;Initial Catalog=RPS2013;Trusted_Connection=No;User=RPSUser;Password=rpsuser;MultipleActiveResultSets=True");/* FUNCIONA*/

            //DataBaseLayer b_base = new DataBaseLayer("Data Source=localhost;Initial Catalog=RPS2013;Trusted_Connection=No;User=RPSUserDev;Password=rpsuserdev;MultipleActiveResultSets=True");

            DataBaseLayer b_base = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_RPS2013);

            List<DB_pds_progutils_ETIQ01_PALETS_GEN01> db_SIRO_item = b_base.dB_Pds_Progutils_ETIQ01_PALETS_GEN01_GetItems(p_codig_articulo);
            
            RellenaListaArticuloCliente(db_SIRO_item);
            
            RaisePropertyChanged("ListaArticuloCliente_Existen");
            RaisePropertyChanged("ListaArticuloCliente");


        }

        #endregion



        #region Propiedades

        private csitem_EtiquetaGeneralPaletT1 _Entity = new csitem_EtiquetaGeneralPaletT1();
        public csitem_EtiquetaGeneralPaletT1 Entity { get => _Entity; set => _Entity = value; }

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

        private bool _Pantalla_GrupoGeneracionEnabled = true;
        public bool Pantalla_GrupoGeneracionEnabled
        {
            get { return _Pantalla_GrupoGeneracionEnabled; }
            set { _Pantalla_GrupoGeneracionEnabled = value; RaisePropertyChanged("Pantalla_GrupoGeneracionEnabled"); }
        }

        private string _TextoErrorBuscarMaterialLote = "";
        public string TextoErrorBuscarMaterialLote
        {
            get { return _TextoErrorBuscarMaterialLote; }
            set { _TextoErrorBuscarMaterialLote = value; RaisePropertyChanged("TextoErrorBuscarMaterialLote"); }
        }

        private string _IN_CodArticulo = "";
        public string IN_CodArticulo
        {
            get { return _IN_CodArticulo; }
            set { _IN_CodArticulo = value; DATO_CodigoArticulo = value; RaisePropertyChanged("IN_CodArticulo"); }
        }

        private string _IN_CodLote = "";
        public string IN_CodLote
        {
            get { return _IN_CodLote; }
            set { _IN_CodLote = value; DATO_CodigoLote = value; RaisePropertyChanged("IN_CodLote"); }
        }

        private string _DATO_CodigoArticulo = "";
        public string DATO_CodigoArticulo
        {
            get { return _DATO_CodigoArticulo; }
            set
            {
                _DATO_CodigoArticulo = value;
                RaisePropertyChanged("DATO_CodigoArticulo");
                RaisePropertyChanged("PAN_CodigoArticulo");
            }
        }

        public string PAN_CodigoArticulo
        {
            get { return _DATO_CodigoArticulo; }
        }

        private string _PAN_ArticuloDescripcion = "";
        public string PAN_ArticuloDescripcion
        {
            get { return _PAN_ArticuloDescripcion; }
            set { _PAN_ArticuloDescripcion = value; RaisePropertyChanged("PAN_ArticuloDescripcion"); }
        }

        private string _DATO_CodigoLote = "";
        public string DATO_CodigoLote
        {
            get { return _DATO_CodigoLote; }
            set
            {
                _DATO_CodigoLote = value;
                Entity.Sscc = "(01)0" + Entity.Ean13 + "(10)" + _DATO_CodigoLote;
                RaisePropertyChanged("DATO_CodigoLote");
                RaisePropertyChanged("PAN_CodigoLote");
            }
        }

        public string PAN_CodigoLote
        {
            get { return _DATO_CodigoLote; }
        }

        private string _PAN_CodigoLote_Comentario = "";
        public string PAN_CodigoLote_Comentario
        {
            get { return _PAN_CodigoLote_Comentario; }
            set { _PAN_CodigoLote_Comentario = value; RaisePropertyChanged("PAN_CodigoLote_Comentario"); }
        }

        private bool _imprimirZPL = false;
        public bool ImprimirZPL { get => _imprimirZPL; set { _imprimirZPL = value; RaisePropertyChanged("ImprimirZPL"); } }

        private bool _utilizaRefArticuloCliente = false;
        public bool UtilizaRefArticuloCliente { get => _utilizaRefArticuloCliente; set { _utilizaRefArticuloCliente = value; RaisePropertyChanged("UtilizaRefArticuloCliente"); } }

        private bool _utilizaAmbosCodigosArticulo = false;
        public bool UtilizaAmbosCodigosArticulo { get => _utilizaAmbosCodigosArticulo;
            set {
                _utilizaAmbosCodigosArticulo = value;
                if (ListaArticuloCliente_SelectedItem == null || string.IsNullOrWhiteSpace(ListaArticuloCliente_SelectedItem.RefArticuloCliente)) _utilizaAmbosCodigosArticulo = false;
                RaisePropertyChanged("UtilizaAmbosCodigosArticulo");
            }
        }

        private ObservableCollection<csItem_ListaArticuloCliente> _ListaArticuloCliente = new ObservableCollection<csItem_ListaArticuloCliente>();
        public ObservableCollection<csItem_ListaArticuloCliente> ListaArticuloCliente
        {
            get { return _ListaArticuloCliente; }
            set { _ListaArticuloCliente = value; RaisePropertyChanged("ListaArticuloCliente"); }
        }

        private csItem_ListaArticuloCliente _ListaArticuloCliente_SelectedItem = null;
        public csItem_ListaArticuloCliente ListaArticuloCliente_SelectedItem
        {
            get { return _ListaArticuloCliente_SelectedItem; }
            set
            {
                _ListaArticuloCliente_SelectedItem = value;
                if (ListaArticuloCliente_SelectedItem != null && !string.IsNullOrWhiteSpace(ListaArticuloCliente_SelectedItem.RefArticuloCliente)) UtilizaRefArticuloCliente = true;
                else { UtilizaRefArticuloCliente = false; UtilizaAmbosCodigosArticulo = false; }
                if (ListaArticuloCliente_SelectedItem != null && !string.IsNullOrWhiteSpace(ListaArticuloCliente_SelectedItem.DescripcionArticuloCliente)) SeleccionDescripcion01 = eSeleccionDescripcion.ArticuloCliente;
                else SeleccionDescripcion01 = eSeleccionDescripcion.Articulo;
                RaisePropertyChanged("ListaArticuloCliente_SelectedItem");
            }
        }

        public bool ListaArticuloCliente_Existen
        {
            get
            {
                if (_ListaArticuloCliente == null) return false;
                if (_ListaArticuloCliente.Count == 0) return false;
                return true;
            }
        }

        private eSeleccionDescripcion _SeleccionDescripcion01 = eSeleccionDescripcion.Articulo;
        public eSeleccionDescripcion SeleccionDescripcion01
        {
            get { return _SeleccionDescripcion01; }
            set
            {
                _SeleccionDescripcion01 = value;
                RaisePropertyChanged("SeleccionDescripcion01");
            }
        }
        public bool ListaLineasGridEtiqueta_CanAdd
        {
            get { return (Entity.ListaLineasGridEtiqueta.Count() < _n_max_lineas_detalle_palet); }
        }


        #endregion
    }

}
