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
using KeepAutomation.Barcode.Bean;
using KeepAutomation.Barcode;
using System.Drawing.Imaging;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    public class panImpresionEtiquetaGen02_ViewModel : panPantallaBase_ViewModel
    {
        BackgroundWorker _bkgwk_GuardarEtiquetaEnBDD = new BackgroundWorker();
        BackgroundWorker _bkgwk_BuscarMaterialLote = new BackgroundWorker();
        public enum eSeleccionImpresion { Impresora = 0, Pantalla = 1 }

        public panImpresionEtiquetaGen02_ViewModel()
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

            Entity.NumEle = " UDS";

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

            ((panImpresionEtiquetaGen02)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

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
                        dbl.DB_pds_progutils_PALETS_Insert(Guid.Parse(Entity.UIDEtiqueta), "0000123456" + Entity.CodArticulo, dbl.DB_pds_progutils_PALETS_GetUIDEtiqueta("5"), serializado);
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
            ((panImpresionEtiquetaGen02)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

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
            Entity.BarCode = null; 
            Entity.BarCode = Entity.CrearBarCode(p_codig_articulo);
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
                ((panImpresionEtiquetaGen02)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                //TextoErrorBuscarMaterialLote = "";

                if (string.IsNullOrWhiteSpace(Entity.CodArticulo))
                {
                    Entity.Descripcion = null;
                    //TextoErrorBuscarMaterialLote = "Código artículo incorrecto";
                    ((panImpresionEtiquetaGen02)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaGen02)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaGen02)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());
                Entity.CodArticulo = null;
                Entity.Descripcion = null;
                Entity.Lote = null;
                Entity.NumEle = null;
                Entity.BarCode = null;
                Entity.PDS = "PLASTICOS DEL SEGURA";
                Entity.Seccion = null;
                Entity.UIDEtiqueta = null;
                Entity.TotalEtiquetas = 1;
                TextoBotonImpresion = "Imprimir y Guardar";
                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen02)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaGen02)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoBotonImpresion = "Imprimir y Guardar";
                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen02)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaGen02)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                if (!CamposCorrectosParaImpresion()) return;
              
                if (ImprimirPorZPL)
                {                    
                    StreamReader objReader = new StreamReader(csEstadoPermanente.Configuracion.Datos.Ruta_informe_GER01_Caja02);
                    string sLine = "";
                    sLine = objReader.ReadToEnd();
                    objReader.Close();
                        
                    //DBConector_OLANET_BASE_2013 b_con_obase2013 = new DBConector_OLANET_BASE_2013(csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_2013);
                    if (!IsStoredInBD)
                    {
                        DBConector_OLANET_BASE_2013 b_con_obase2013 = new DBConector_OLANET_BASE_2013(csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_2013);
                        string b_error = "";
                        string b_sscc = b_con_obase2013.GetNewSSCCCode(ref b_error);
                        Entity.SSCC = b_sscc;
                    }
                    sLine = sLine.Replace("<<LOGO>>", Entity.PDS);
                    sLine = sLine.Replace("<<SECCION>>", Entity.Seccion);
                    sLine = sLine.Replace("<<DESCRIPCION>>", Entity.Descripcion);
                        
                    sLine = sLine.Replace("<<NUMELE>>", Entity.NumEle);
                      
                    sLine = sLine.Replace("<<CODIGOARTICULO>>", Entity.CodArticulo);
                    sLine = sLine.Replace("<<LOTE>>", Entity.Lote);
                        

                    csGeneraEtiqRepro_Comun imprime = new csGeneraEtiqRepro_Comun();
                    string filename = "";
                    //if (ImprimirZPL) filename = imprime.GenerarPDFdesdeZPL(sLine);
                    if (Convert.ToBoolean(SeleccionImpresion))
                    {
                        filename = imprime.GenerarPDFdesdeZPL(sLine);
                        Process.Start(Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + filename); Thread.Sleep(1000);
                        File.Delete(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + filename);
                           
                    }
                    else
                    {
                        //imprimir.ImprimeEtiquetaPalet(printerSettings, Entity, Convert.ToBoolean(SeleccionImpresion), true, ((panImpresionEtiquetaSIRO)View).Dispatcher);
                        //TODO IMPRIMIR POR IMPRESORA ZPL 
                        imprime.imprimirZPL(sLine, Entity.TotalEtiquetas);
                    }
                    

                    if (!IsStoredInBD)
                    {
                        if (_bkgwk_GuardarEtiquetaEnBDD.IsBusy != true)
                        {
                            _bkgwk_GuardarEtiquetaEnBDD.RunWorkerAsync("GUARDARENBDD");
                        }
                    }
                }
                /*csGeneraEtiqRepro_Comun imprimir = new csGeneraEtiqRepro_Comun();
                PrinterSettings printerSettings = new PrinterSettings(); 
                imprimir.ImprimeEtiquetaCaja(printerSettings, Entity, Convert.ToBoolean(SeleccionImpresion), false, ((panImpresionEtiquetaGen02)View).Dispatcher);*/

            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen02)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public Boolean CamposCorrectosParaImpresion() {
            if (String.IsNullOrWhiteSpace(Entity.CodArticulo)) { MessageBox.Show("Falta el campo Codigo Articulo", "Debe rellenar todos los campos obligatorios"); return false; }
            if (String.IsNullOrWhiteSpace(Entity.Descripcion.ToString())) { MessageBox.Show("Falta el campo Descripcion", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.Lote.ToString())) { MessageBox.Show("Falta el campo Lote", "Debe rellenar todos los campos obligatorios"); return false;}
            return true;
        }
        #endregion

        #region Properties
        private csItem_EtiquetaCaja _Entity = new csItem_EtiquetaCaja();
        public csItem_EtiquetaCaja Entity { get => _Entity; set { _Entity = value; RaisePropertyChanged("Entity"); } }

        public string TextoBotonImpresion { get => textoBotonImpresion; set { textoBotonImpresion = value; RaisePropertyChanged("TextoBotonImpresion"); } }

        private string textoBotonImpresion = "Imprimir y Guardar";

        public bool IsStoredInBD = false;
        private bool _ImprimirPorZPL = false;
        public bool ImprimirPorZPL
        {
            get { return _ImprimirPorZPL; }
            set { _ImprimirPorZPL = value; RaisePropertyChanged("ImprimirPorZPL"); }
        }
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
public class csItem_EtiquetaCaja : ObservableObject, IDataErrorInfo
{
    private string _codArticulo;
    private string _descripcion;
    private string _lote;
    private string _barCode;
    private string _numEle;
    private string _seccion;
    private string _PDS = "PLASTICOS DEL SEGURA";
    private string _uidEtiqueta;
    private string _sscc;
    private int _totalEtiquetas = 1;
    
    public string UIDEtiqueta { get => _uidEtiqueta; set { _uidEtiqueta = value; RaisePropertyChanged("UIDEtiqueta"); } }
    public string SSCC { get => _sscc; set { _sscc = value; RaisePropertyChanged("SSCC"); } }
    public string PDS { get => _PDS; set { _PDS = value; RaisePropertyChanged("PDS"); } }
    public string CodArticulo { get => _codArticulo; set { _codArticulo = value; RaisePropertyChanged("CodArticulo"); } }
    public string Descripcion { get => _descripcion; set { _descripcion = value; RaisePropertyChanged("Descripcion"); } }
    public string Lote { get => _lote; set { _lote = value; RaisePropertyChanged("Lote"); } }
    public string BarCode { get => _barCode; set { _barCode = value; RaisePropertyChanged("BarCode"); } }
    public string NumEle { get => _numEle; set { _numEle = value; RaisePropertyChanged("NumEle"); } }
    public string Seccion { get => _seccion; set { _seccion = value; RaisePropertyChanged("Seccion"); } }
    public int TotalEtiquetas { get => _totalEtiquetas; set { _totalEtiquetas = value; RaisePropertyChanged("TotalEtiquetas"); } }
    public string this[string columnName]
    {
        get { return ""; }
    }
    public string Error
    {
        get { return ""; }
    }
    public string CrearBarCode(string codArt)
    {
        BarCode ean128 = new BarCode();
        ean128.Symbology = KeepAutomation.Barcode.Symbology.Code128Auto;

        // Input valid EAN 128 encoding data: All ASCII characters, including 0-9, A-Z, a-z, and special characters.
        ean128.CodeToEncode = codArt;//"(01)225";

        //Apply checksum for EAN 128 barcode.
        ean128.ChecksumEnabled = true;
        // Display checksum in the EAN 128 barcode text
        ean128.DisplayChecksum = true;

        // EAN 128 unit of measure, Pixel, Cm and Inch supported.
        ean128.BarcodeUnit = BarcodeUnit.Pixel;
        // EAN 128 image resolution in DPI.
        ean128.DPI = 72;

        // EAN 128 image formats in Png, Jpeg/Jpg, Gif, Tiff, Bmp, etc.
        ean128.ImageFormat = ImageFormat.Png;

        // Set EAN 128 image size

        // EAN 128 bar module width (X dimention)
        ean128.X = 3;
        // EAN 128 bar module height (Y dimention)
        ean128.Y = 60;
        // Image left margin size, a 10X is automatically added to go with specifications.
        ean128.LeftMargin = 0;
        // Image right margin size, a 10X is automatically added to go with specifications.
        ean128.RightMargin = 0;
        // EAN 128 image top margin size
        ean128.TopMargin = 0;
        // EAN 128 image bottom margin size
        ean128.BottomMargin = 0;

        // Orientation, 90, 180, 270 degrees supported
        ean128.Orientation = KeepAutomation.Barcode.Orientation.Degree0;

        // Set EAN 128 human readable text style

        // Display human readable text
        ean128.DisplayText = true;
        ean128.TextFont = new Font("Arial", 10f, System.Drawing.FontStyle.Regular);
        // Space between barcode and text
        ean128.TextMargin = 6;

        if (!Directory.Exists("C://Temporales//")){
            Directory.CreateDirectory("C://Temporales//");
        }
        // Generate EAN 128 barcodes in BMP image format
        string ruta = "C://Temporales//barcode-ean128-csharp.bmp";
        ean128.generateBarcodeToImageFile(ruta);
        return ruta;
    }

}
