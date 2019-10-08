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
    public class panImpresionEtiquetaCOliveRueda_ViewModel : panPantallaBase_ViewModel
    {
        BackgroundWorker _bkgwk_GuardarEtiquetaEnBDD = new BackgroundWorker();
        BackgroundWorker _bkgwk_BuscarMaterialCliente = new BackgroundWorker();
        public enum eSeleccionImpresion { Impresora = 0, Pantalla = 1 }
        public enum eSeleccionDescripcion { Articulo = 0, ArticuloCliente = 1 }

        public panImpresionEtiquetaCOliveRueda_ViewModel()
        {
            VolverPantallaAnterior_Command = new RelayCommand(VolverPantallaAnterior_Command_Execute, VolverPantallaAnterior_Command_CanExecute);
            NuevaEtiqueta_Command = new RelayCommand(NuevaEtiqueta_Command_Execute, NuevaEtiqueta_Command_CanExecute);
            ModificarEtiquetaActual_Command = new RelayCommand(ModificarEtiquetaActual_Command_Execute, ModificarEtiquetaActual_Command_CanExecute);
            VerHistoricoEtiqueta_Command = new RelayCommand(VerHistoricoEtiqueta_Command_Execute, VerHistoricoEtiqueta_Command_CanExecute);
            ImprimirEtiqueta_Command = new RelayCommand(ImprimirEtiqueta_Command_Execute, ImprimirEtiqueta_Command_CanExecute);
            BuscarMaterialCliente_Command = new RelayCommand(BuscarMaterialCliente_Command_Execute, BuscarMaterialCliente_Command_CanExecute);
            BuscarPorCliente_Command = new RelayCommand(BuscarPorCliente_Command_Execute, BuscarPorCliente_Command_CanExecute);

        }
        #region Overrides

        internal override void Inicializa()
        {
            base.Inicializa();
            _bkgwk_GuardarEtiquetaEnBDD.DoWork += _bkgwk_GuardarEtiquetaEnBDD_DoWork;
            _bkgwk_GuardarEtiquetaEnBDD.RunWorkerCompleted += _bkgwk_GuardarEtiquetaEnBDD_RunWorkerCompleted;

            _bkgwk_BuscarMaterialCliente.WorkerReportsProgress = false;
            _bkgwk_BuscarMaterialCliente.WorkerSupportsCancellation = false;
            _bkgwk_BuscarMaterialCliente.DoWork += _bkgwk_BuscarMaterialCliente_DoWork;
            _bkgwk_BuscarMaterialCliente.RunWorkerCompleted += _bkgwk_BuscarMaterialCliente_RunWorkerCompleted;

            IN_CodCliente = "102305";
            BuscarPorCliente_Command_Execute();
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
           
            ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

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
                    Entity.UIDEtiqueta = Guid.NewGuid().ToString();
                    string serializado = csEstadoPermanente.Serialize(Entity);
                    DataBaseLayer dbl = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_PDSImpresionEtiquetas);

                    // TODO Guardar En BDD
                    dbl.DB_pds_progutils_PALETS_Insert(Guid.Parse(Entity.UIDEtiqueta), Entity.Sscc.Serialize(), dbl.DB_pds_progutils_PALETS_GetUIDEtiqueta("7"), serializado);

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

        void _bkgwk_BuscarMaterialCliente_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();


            _bkgwk_BuscarMaterialCliente.Dispose();

            //_HayOFSegunLote = true;
            ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

            return;
        }
        void _bkgwk_BuscarMaterialCliente_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;

            try
            {
                if (e.Argument.ToString() == "ARTICULO")
                {
                    BuscarArticuloLote(_IN_CodArticulo, _IN_CodCliente, true);
                }
                else if (e.Argument.ToString() == "CLIENTE")
                {
                    BuscarPorCliente(_IN_CodCliente);
                }
                else
                {
                    e.Result = false;
                }
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError("_bkgwk_BuscarMaterialCliente_DoWork", ex);
            }
        }

        public delegate void RellenaListaArticuloCliente_Callback(List<DB_pds_progutils_ETIQ01_PALETS_GEN01> db_list_items);
        public void RellenaListaArticuloCliente(List<DB_pds_progutils_ETIQ01_PALETS_GEN01> db_list_items)
        {
            panImpresionEtiquetaCOliveRueda b_vista = (panImpresionEtiquetaCOliveRueda)View;

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
                    
                    if (db_list_items.Count == 0) TextoErrorBuscarMaterialCliente = "Ningún artículo encontrado";
                    else TextoErrorBuscarMaterialCliente = ""; 

                    foreach (DB_pds_progutils_ETIQ01_PALETS_GEN01 i_item in db_list_items)
                    {
                        DATO_CodigoArticulo = i_item.CodArticulo;
                        DATO_CodigoCliente = IN_CodCliente;
                        PAN_ArticuloDescripcion = i_item.Descripcion;
                        csItem_ListaArticuloCliente b_lac = new csItem_ListaArticuloCliente();
                        b_lac.IDCustomerArticle = i_item.CodArticulo;
                        b_lac.DescripcionCustomer = i_item.NombreCliente;
                        b_lac.DescripcionArticuloCliente = i_item.DescripcionCliente;
                        b_lac.CodCustomer = i_item.IDCustomer;
                        b_lac.RefArticuloCliente = i_item.ReferenciaCliente;
                        if (i_item.DescripcionCliente.Trim() != "") b_lac.TieneDescripcionArticulo = true;
                        else b_lac.TieneDescripcionArticulo = false;

                        _ListaArticuloCliente.Add(b_lac);
                        if (!String.IsNullOrWhiteSpace(DATO_CodigoCliente) && b_lac.CodCustomer == DATO_CodigoCliente)
                        {
                            SeleccionDescripcion01 = eSeleccionDescripcion.Articulo;
                            ListaArticuloCliente_SelectedItem = ListaArticuloCliente[ListaArticuloCliente.Count() - 1];
                        }
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
                ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                _IN_CodArticulo = "";
                _IN_CodCliente = "";
                DATO_CodigoArticulo = "";
                DATO_CodigoCliente = "";
                PAN_ArticuloDescripcion = "";
                PAN_Cliente_Comentario = "";
                TextoErrorBuscarMaterialCliente = "";
                Entity.CodArticulo = "";
                Entity.CodCliente = "";
                Entity.Lote = "";
                Entity.UIDEtiqueta = "";
                Entity.Sscc = null;
                Entity.TotalUds = 0;
                Entity.TotalEtiquetas = 0;
                Entity.Descripcion = "";
               // Entity.Fecha = "";
                //Entity.Ean13 = "";
                //Entity.ListaLineasGridEtiqueta.Clear();
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
                ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoBotonImpresion = "Imprimir y Guardar";
                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public ICommand VerHistoricoEtiqueta_Command { get; set; }
        public bool VerHistoricoEtiqueta_Command_CanExecute()
        {
            return true;
        }
        public void VerHistoricoEtiqueta_Command_Execute()
        {
            panHistorico b_pantalla = new Pantallas.panHistorico("6");

            if (Utilidades.UtilesCarga._pantallas_abiertas.Any(z => z.Key == b_pantalla.ToString()))
            {
                //if (ListaClientesEtiquetas_SelectedItem.Id == 1) b_pantalla = (Pantallas.panImpresionEtiquetaCOliveRueda)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value; 
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
                ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                if (!CamposCorrectosParaImpresion()) return;
                if (!IsStoredInBD)
                {
                    
                    DBConector_PDSImpresionEtiquetas b_con_obase2013 = new DBConector_PDSImpresionEtiquetas(csEstadoPermanente.Configuracion.Datos.connectionString_PDSImpresionEtiquetas);
                    Entity.Sscc = b_con_obase2013.DameNuevosCodigosSSCC((int)Entity.TotalEtiquetas * 12);
                   
                    if (_bkgwk_GuardarEtiquetaEnBDD.IsBusy != true)
                    {
                        _bkgwk_GuardarEtiquetaEnBDD.RunWorkerAsync("GUARDARENBDD");
                    }
                }

                csGeneraEtiqRepro_Comun imprimir = new csGeneraEtiqRepro_Comun();
                PrinterSettings printerSettings = new PrinterSettings();
                imprimir.ImprimeEtiquetaPalet(printerSettings, Entity, Convert.ToBoolean(SeleccionImpresion), true, ((panImpresionEtiquetaCOliveRueda)View).Dispatcher);

            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
            if (String.IsNullOrWhiteSpace(DATO_CodigoCliente)) { MessageBox.Show("Falta el campo Lote", "Debe rellenar todos los campos obligatorios"); return false; }
            if(UtilizaRefArticuloCliente && String.IsNullOrWhiteSpace(ListaArticuloCliente_SelectedItem.RefArticuloCliente)) {
                MessageBox.Show("La referencia del Artículo para el cliente no existe, se va a imprimir nuestro Codigo Artículo "+ DATO_CodigoArticulo, "Debe rellenar todos los campos obligatorios"); UtilizaRefArticuloCliente = false; return false; }
            if (Entity.TotalUds == 0) { MessageBox.Show("Falta el campo Unidades", "Debe rellenar todos los campos obligatorios"); return false; }
            if (Entity.TotalEtiquetas <= 0) { MessageBox.Show("Falta el campo Total Paginas a imprimir", "Debe rellenar todos los campos obligatorios"); return false; }
            if (String.IsNullOrWhiteSpace(Entity.Lote)) { MessageBox.Show("Falta el campo Lote", "Debe rellenar todos los campos obligatorios"); return false; }
            return true;
        }

        public ICommand BuscarMaterialCliente_Command { get; set; }
        public bool BuscarMaterialCliente_Command_CanExecute()
        {
            if (_bkgwk_BuscarMaterialCliente.IsBusy) return false;
            if (String.IsNullOrWhiteSpace(_IN_CodArticulo)) return false;
            return true;
        }
        public void BuscarMaterialCliente_Command_Execute()
        {
            try
            {
                ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoErrorBuscarMaterialCliente = "";

                if (string.IsNullOrWhiteSpace(_IN_CodArticulo.Trim()))
                {
                    TextoErrorBuscarMaterialCliente = "Código artículo incorrecto";
                    ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
                    return;
                }
                
                if (_bkgwk_BuscarMaterialCliente.IsBusy != true)
                {
                    //ResetDatosPantalla();
                    _bkgwk_BuscarMaterialCliente.RunWorkerAsync("ARTICULO");
                }

            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public ICommand BuscarPorCliente_Command { get; set; }
        public bool BuscarPorCliente_Command_CanExecute()
        {
            if (string.IsNullOrWhiteSpace(_IN_CodCliente.Trim())) return false;
            if (_bkgwk_BuscarMaterialCliente.IsBusy) return false;

            return true;
        }
        public void BuscarPorCliente_Command_Execute()
        {
            try
            {
                ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoErrorBuscarMaterialCliente = "";

                if (string.IsNullOrWhiteSpace(_IN_CodCliente.Trim()))
                {
                    TextoErrorBuscarMaterialCliente = "Código cliente incorrecto";
                    ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
                    return;
                }

                if (_bkgwk_BuscarMaterialCliente.IsBusy != true)
                {
                    //ResetDatosPantalla();
                    _bkgwk_BuscarMaterialCliente.RunWorkerAsync("CLIENTE");
                }

            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaCOliveRueda)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
        public void BuscarPorCliente(string p_codig_cliente)
        {

            DataBaseLayer b_base = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_RPS2013);

            List<DB_pds_progutils_ETIQ01_PALETS_GEN01> db_item = b_base.dB_Pds_Progutils_ETIQ_BOBINA_GEN01_GetItem(p_codig_cliente);

            if (db_item.Count == 1 && !String.IsNullOrWhiteSpace(db_item[0].IDCustomer) && !String.IsNullOrWhiteSpace(db_item[0].NombreCliente))
            {
                DATO_CodigoCliente = db_item[0].IDCustomer;
                IN_CodCliente = db_item[0].IDCustomer;
                PAN_Cliente_Comentario = db_item[0].NombreCliente;
            } else if (db_item.Count > 1){
                TextoErrorBuscarMaterialCliente = "Varios clientes encontrados, afine su búsqueda";
            }
            else
            {
                TextoErrorBuscarMaterialCliente = "Ningún cliente encontrado";
            }

            RaisePropertyChanged("ListaArticuloCliente_Existen");
            RaisePropertyChanged("ListaArticuloCliente");


        }
        #endregion



        #region Propiedades

        private csitem_EtiquetaGeneralCajaBobinaT1 _Entity = new csitem_EtiquetaGeneralCajaBobinaT1();
        public csitem_EtiquetaGeneralCajaBobinaT1 Entity { get => _Entity; set => _Entity = value; }

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

        private string _TextoErrorBuscarMaterialCliente = "";
        public string TextoErrorBuscarMaterialCliente
        {
            get { return _TextoErrorBuscarMaterialCliente; }
            set { _TextoErrorBuscarMaterialCliente = value; RaisePropertyChanged("TextoErrorBuscarMaterialCliente"); }
        }

        private string _IN_CodArticulo = "";
        public string IN_CodArticulo
        {
            get { return _IN_CodArticulo; }
            set { _IN_CodArticulo = value; DATO_CodigoArticulo = value; Entity.CodArticulo = value; RaisePropertyChanged("IN_CodArticulo"); }
        }

        private string _IN_CodCliente = "";
        public string IN_CodCliente
        {
            get { return _IN_CodCliente; }
            set { _IN_CodCliente = value; DATO_CodigoCliente = value; Entity.CodCliente= value; RaisePropertyChanged("IN_CodCliente"); }
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

        private string _DATO_CodigoCliente = "";
        public string DATO_CodigoCliente
        {
            get { return _DATO_CodigoCliente; }
            set
            {
                _DATO_CodigoCliente = value;
                RaisePropertyChanged("DATO_CodigoCliente");
                RaisePropertyChanged("PAN_CodigoCliente");
            }
        }

        public string PAN_CodigoCliente
        {
            get { return _DATO_CodigoCliente; }
        }

        private string _PAN_Cliente_Comentario = "";
        public string PAN_Cliente_Comentario
        {
            get { return _PAN_Cliente_Comentario; }
            set { _PAN_Cliente_Comentario = value; RaisePropertyChanged("PAN_Cliente_Comentario"); }
        }

        private bool _imprimirZPL = false;
        public bool ImprimirZPL { get => _imprimirZPL; set { _imprimirZPL = value; RaisePropertyChanged("ImprimirZPL"); } }

        private bool _utilizaRefArticuloCliente = true;
        public bool UtilizaRefArticuloCliente { get => _utilizaRefArticuloCliente;
            set {  _utilizaRefArticuloCliente = value; RaisePropertyChanged("UtilizaRefArticuloCliente");
                if (value == false) Entity.CodArticulo = DATO_CodigoArticulo;
                else if (!String.IsNullOrWhiteSpace(ListaArticuloCliente_SelectedItem.RefArticuloCliente)) Entity.CodArticulo = ListaArticuloCliente_SelectedItem.RefArticuloCliente; } }

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
                if (ListaArticuloCliente_SelectedItem != null && !string.IsNullOrWhiteSpace(ListaArticuloCliente_SelectedItem.RefArticuloCliente))
                {
                    UtilizaRefArticuloCliente = true;
                    Entity.CodArticulo = ListaArticuloCliente_SelectedItem.RefArticuloCliente;
                }
                else { UtilizaRefArticuloCliente = false; UtilizaAmbosCodigosArticulo = false; Entity.CodArticulo = DATO_CodigoArticulo; }
                if (ListaArticuloCliente_SelectedItem != null && !string.IsNullOrWhiteSpace(ListaArticuloCliente_SelectedItem.DescripcionArticuloCliente))
                {
                    SeleccionDescripcion01 = eSeleccionDescripcion.ArticuloCliente;
                    Entity.Descripcion = ListaArticuloCliente_SelectedItem.DescripcionArticuloCliente;
                }
                else { SeleccionDescripcion01 = eSeleccionDescripcion.Articulo; Entity.Descripcion = PAN_ArticuloDescripcion; }
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
                if (value == eSeleccionDescripcion.Articulo) Entity.Descripcion = PAN_ArticuloDescripcion;
                else {
                    if (!String.IsNullOrWhiteSpace(ListaArticuloCliente_SelectedItem.DescripcionArticuloCliente)) Entity.Descripcion = ListaArticuloCliente_SelectedItem.DescripcionArticuloCliente;
                    else { Entity.Descripcion = PAN_ArticuloDescripcion; _SeleccionDescripcion01 = eSeleccionDescripcion.Articulo; }
                }
                RaisePropertyChanged("SeleccionDescripcion01");
            }
        }
        #endregion
        public class csitem_EtiquetaGeneralCajaBobinaT1 : ObservableObject, IDataErrorInfo
        {
            private string _codArticulo;

            private string _codCliente;

            private double _totalEtiquetas = 1;

            private double _totalUds = 1;

            private string _lote;

            private string _descripcion;

            private string _numUnidades;

            private List<string> _sscc;

            private string _uidEtiqueta = null;
            
            public string UIDEtiqueta { get => _uidEtiqueta; set { _uidEtiqueta = value; RaisePropertyChanged("UIDEtiqueta"); } }
            public string CodArticulo { get => _codArticulo; set { _codArticulo = value; RaisePropertyChanged("CodArticulo"); } }
            public string CodCliente { get => _codCliente; set { _codCliente = value; RaisePropertyChanged("CodCliente"); } }
            public double TotalEtiquetas { get => _totalEtiquetas; set { if (value > 0) _totalEtiquetas = value; RaisePropertyChanged("TotalEtiquetas"); } }
            public double TotalUds { get => _totalUds; set { _totalUds = value; RaisePropertyChanged("TotalEtiquetas"); } }
            public string Lote { get => _lote; set { _lote = value; RaisePropertyChanged("Lote"); } }
            public string Descripcion { get => _descripcion; set { _descripcion = value; RaisePropertyChanged("Descripcion"); } }
            public string NumUnidades { get => _numUnidades; set { _numUnidades = value; RaisePropertyChanged("NumUnidades"); } }
            public List<string> Sscc { get => _sscc; set { _sscc = value; RaisePropertyChanged("Sscc"); } }

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
}
