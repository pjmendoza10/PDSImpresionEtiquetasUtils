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
    public class panImpresionEtiquetaGen01_ViewModel : panPantallaBase_ViewModel
    {
        BackgroundWorker _bkgwk_GuardarEtiquetaEnBDD = new BackgroundWorker();
        BackgroundWorker _bkgwk_BuscarMaterialLote = new BackgroundWorker();
        public enum eSeleccionImpresion { Impresora = 0, Pantalla = 1 }
        public enum eSeleccionDescripcion { Articulo = 0, ArticuloCliente = 1 }

        private int _n_max_lineas_detalle_palet = 10;

        public panImpresionEtiquetaGen01_ViewModel()
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
           
            ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

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
                    string serializado = csEstadoPermanente.Serialize(Entity);
                    DataBaseLayer dbl = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_PDSImpresionEtiquetas);

                    // TODO Guardar En BDD
                    Entity.UIDEtiqueta = Guid.NewGuid().ToString();
                    dbl.DB_pds_progutils_PALETS_Insert(Guid.Parse(Entity.UIDEtiqueta), Entity.Sscc, dbl.DB_pds_progutils_PALETS_GetUIDEtiqueta("1"), serializado);

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
            ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

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
            panImpresionEtiquetaGen01 b_vista = (panImpresionEtiquetaGen01)View;

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

                    Entity.Lote = IN_CodLote;
                    if (db_list_items.Count == 0) TextoErrorBuscarMaterialLote = "Ningún artículo encontrado";
                    else TextoErrorBuscarMaterialLote = ""; 

                    foreach (DB_pds_progutils_ETIQ01_PALETS_GEN01 i_item in db_list_items)
                    {
                        DATO_CodigoArticulo = i_item.CodArticulo;
                        DATO_CodigoLote = IN_CodLote;
                        Entity.Ean13 = i_item.EAN13;
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
                ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

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
                ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoBotonImpresion = "Imprimir y Guardar";
                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public ICommand VerHistoricoEtiqueta_Command { get; set; }
        public bool VerHistoricoEtiqueta_Command_CanExecute()
        {
            return true;
        }
        public void VerHistoricoEtiqueta_Command_Execute()
        {
            panHistorico b_pantalla = new Pantallas.panHistorico("1");

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
           return true;
        }
        public void ImprimirEtiqueta_Command_Execute()
        {
            try
            {
                ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                if (!CamposCorrectosParaImpresion()) return;
                //StreamReader objReader = new StreamReader(@"\\Olanet\\etiquetas\\ZEBRA\\paletsG.zpl");
                //StreamReader objReader = new StreamReader("C:\\Temporales\\paletsG_T1.zpl");
                StreamReader objReader = new StreamReader(csEstadoPermanente.Configuracion.Datos.Ruta_informe_GER01_Palet01);
                string sLine = "";
                sLine = objReader.ReadToEnd();
                objReader.Close();
                sLine = sLine.Replace("^PQ1,0,1,Y^XZ", "");
                if(Entity.ListaLineasGridEtiqueta.Count > 0)
                {
                    int posY = 440; int aux = 0;
                    sLine += "^FT40,947^A0N,39,38^FH\\^FDTotal Unidades^FS\r\n";
                    sLine += "^FT40,900^A0N,39,38^FH\\^FDTotal Cajas^FS\r\n";
                    sLine += "^FT338,360^A0N,39,38^FH\\^FDUds por Caja^FS\r\n^FT43,359^A0N,39,38^FH\\^FDN\\A7 de Cajas^FS\r\n^FT944,362^A0N,39,38^FH\\^FDTotal Unidades^FS\r\n^FT726,361^A0N,39,38^FH\\^FDKgs Aprox.^FS\r\n";
                    foreach (csItem_ListaLineasGridEtiqueta item in Entity.ListaLineasGridEtiqueta)
                    {
                        sLine += "^FO83," + (posY + aux * 40) + "^A0N,39,38^FH\\^FB140,1,0,R^FD" + item.NumCajas.ToString().PadLeft(8) + "^FS\r\n";
                        sLine += "^FO450," + (posY + aux * 40) + "^A0N,39,38^FH\\^FB140,1,0,R^FD" + item.UdsPorCaja.ToString().PadLeft(8) + "^FS\r\n";
                        sLine += "^FO750," + (posY + aux * 40) + "^A0N,39,38^FH\\^FB140,1,0,R^FD" + item.KgsAprox.ToString().PadLeft(8) + "^FS\r\n";
                        sLine += "^FO1025," + (posY + aux * 40) + "^A0N,39,38^FH\\^FB140,1,0,R^FD" + item.TotalUds.ToString().PadLeft(8) + "^FS\r\n";
                        aux++;
                    }
                }

                //DBConector_OLANET_BASE_2013 b_con_obase2013 = new DBConector_OLANET_BASE_2013(csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_2013);
                if (!IsStoredInBD) { 
                    DBConector_OLANET_BASE_2013 b_con_obase2013 = new DBConector_OLANET_BASE_2013(csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_2013);
                    string b_error = "";
                    string b_sscc = b_con_obase2013.GetNewSSCCCode(ref b_error);
                    Entity.Sscc = b_sscc;
                }
                    sLine = sLine.Replace("<<SSCC2>>", "00" + Entity.Sscc);
                    sLine = sLine.Replace("<<SSCC>>", "(00)" + Entity.Sscc);

                if (string.IsNullOrWhiteSpace(IN_CodLote))
                {
                    sLine += "^FT0,1373^A0N,32,31^FH\\^FB1184,1,0,C^FD(01)" + Entity.Ean13.PadLeft(14, '0') + "^FS\r\n" +
                    "^BY5,3,255^FT255,1330^BCN,,N,N\r\n" +
                    "^FD>;>801" + Entity.Ean13.PadLeft(14, '0') + "^FS";
                }
                else
                {
                    sLine += "^FT0,1365^A0N,32,31^FH\\^FB1184,1,0,C^FD(01)" + Entity.Ean13.PadLeft(14, '0') + "(10)" + DATO_CodigoLote + "^FS\r\n" +
                    "^BY5,3,255^FT" + (255-((220/8)*((DATO_CodigoLote.Length / 2)+1))) + ",1330^BCN,,N,N\r\n" +
                    "^FD>;>801" + Entity.Ean13.PadLeft(14, '0') + "10" + DATO_CodigoLote + "^FS";
                }

                if (!Convert.ToBoolean(SeleccionDescripcion01))
                {
                    if (UtilizaAmbosCodigosArticulo) sLine = sLine.Replace("<<DESCRIPCIONART>>", PAN_ArticuloDescripcion + " (" + DATO_CodigoArticulo + ")");
                    else sLine = sLine.Replace("<<DESCRIPCIONART>>", PAN_ArticuloDescripcion);
                } else
                {
                    if (UtilizaAmbosCodigosArticulo)
                    {
                        if (ListaArticuloCliente_SelectedItem != null && ListaArticuloCliente_SelectedItem.TieneDescripcionArticulo) sLine = sLine.Replace("<<DESCRIPCIONART>>", ListaArticuloCliente_SelectedItem.DescripcionArticuloCliente + " (" + DATO_CodigoArticulo + ")");
                        else sLine = sLine.Replace("<<DESCRIPCIONART>>", PAN_ArticuloDescripcion + " (" + DATO_CodigoArticulo + ")");
                    } else { 
                       if (ListaArticuloCliente_SelectedItem != null && ListaArticuloCliente_SelectedItem.TieneDescripcionArticulo) sLine = sLine.Replace("<<DESCRIPCIONART>>", ListaArticuloCliente_SelectedItem.DescripcionArticuloCliente);
                       else sLine = sLine.Replace("<<DESCRIPCIONART>>", PAN_ArticuloDescripcion);
                    }
                }
                sLine = sLine.Replace("<<NUMORDEN>>", DATO_CodigoLote);
                if (UtilizaRefArticuloCliente && ListaArticuloCliente_SelectedItem != null && !String.IsNullOrWhiteSpace(ListaArticuloCliente_SelectedItem.RefArticuloCliente))
                    sLine = sLine.Replace("<<CODIGOARTICULO>>", ListaArticuloCliente_SelectedItem.RefArticuloCliente);
                else sLine = sLine.Replace("<<CODIGOARTICULO>>", DATO_CodigoArticulo);
                sLine = sLine.Replace("<<TOTALKGSAPROX>>", Entity.TotalKgAprox.ToString().PadLeft(8));
                sLine = sLine.Replace("<<TOTALUNIDADES>>", Entity.TotalUds.ToString().PadLeft(8));
                sLine = sLine.Replace("<<TOTALBULTOS>>", Entity.TotalCajas.ToString().PadLeft(8));
                sLine = sLine.Replace("<<FECHAIMPRESION>>", DateTime.Now.ToString());

                sLine +="^PQ1,0,1,Y^XZ";

                csGeneraEtiqRepro_Comun imprime = new csGeneraEtiqRepro_Comun();
                string filename = "";
                if(ImprimirZPL) filename = imprime.GenerarPDFdesdeZPL(sLine);
                if (Convert.ToBoolean(SeleccionImpresion))
                {
                    if (!ImprimirZPL)
                    {
                        filename = imprime.GenerarPDFdesdeZPL(sLine);
                        Process.Start(Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + filename); Thread.Sleep(1000);
                        File.Delete(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + filename);
                    }
                } else
                {
                    //imprimir.ImprimeEtiquetaPalet(printerSettings, Entity, Convert.ToBoolean(SeleccionImpresion), true, ((panImpresionEtiquetaSIRO)View).Dispatcher);
                    //TODO IMPRIMIR POR IMPRESORA ZPL 
                    imprime.imprimirZPL(sLine);
                }

                if (!IsStoredInBD)
                {
                    if (_bkgwk_GuardarEtiquetaEnBDD.IsBusy != true)
                    {
                        _bkgwk_GuardarEtiquetaEnBDD.RunWorkerAsync("GUARDARENBDD");
                    }
                }

            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoErrorBuscarMaterialLote = "";

                if (string.IsNullOrWhiteSpace(_IN_CodArticulo.Trim()))
                {
                    TextoErrorBuscarMaterialLote = "Código artículo incorrecto";
                    ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoErrorBuscarMaterialLote = "";
                NuevaEtiqueta_Command_Execute();

                if (string.IsNullOrWhiteSpace(_IN_CodArticulo.Trim()))
                {
                    TextoErrorBuscarMaterialLote = "Código artículo incorrecto";
                    ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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

    public class csItem_ListaLineasGridEtiqueta : ObservableObject, IDataErrorInfo
    {
        private int _NumCajas;
        private int _UdsPorCaja;
        private double _KgsAprox;
        private int _TotalUds;
        private int _NumeroLinea = 1;
        public string this[string columnName]
        {
            get { return ""; }
        }
        public string Error
        {
            get { return ""; }
        }
        public int NumeroLinea
        {
            get { return _NumeroLinea; }
            set { _NumeroLinea = value; RaisePropertyChanged("NumeroLinea"); }
        }
        public int NumCajas { get => _NumCajas; set { _NumCajas = value; RaisePropertyChanged("NumCajas"); if (_NumCajas > 0 && _UdsPorCaja > 0) TotalUds = NumCajas * UdsPorCaja; } }
        public int UdsPorCaja { get => _UdsPorCaja; set { _UdsPorCaja = value; RaisePropertyChanged("UdsPorCaja"); if (_NumCajas > 0 && _UdsPorCaja > 0) TotalUds = NumCajas * UdsPorCaja; } }
        public double KgsAprox { get => _KgsAprox; set { _KgsAprox = value; RaisePropertyChanged("KgsAprox"); } }
        public int TotalUds { get => _TotalUds; set { _TotalUds = value; RaisePropertyChanged("TotalUds"); } }
    }

    public class csItem_ListaArticuloCliente
    {
        public string IDCustomerArticle { get; set; }
        public string IDCustomer { get; set; }
        public string CodCustomer { get; set; }
        public string DescripcionCustomer { get; set; }
        public string DescripcionArticuloCliente { get; set; }
        public string IDUnitQuantity { get; set; }
        public string CodUnitQuantity { get; set; }
        public string RefArticuloCliente { get; set; }

        public bool TieneDescripcionArticulo { get; set; }
    }

    public class csitem_EtiquetaGeneralPaletT1 : ObservableObject, IDataErrorInfo
    {
        private string _codArticulo;

        private string _lote;


        private double _totalCajas;

        private double _totalUds;

        private double _totalKgAprox;

        private string _fecha;

        private string _descripcion;

        private string _ean13;
        private string _sscc;
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

        private string _uidEtiqueta = null;
        public string CodArticulo { get => _codArticulo; set { _codArticulo = value; RaisePropertyChanged("CodArticulo"); } }
        public string Lote { get => _lote; set { _lote = value; RaisePropertyChanged("Lote"); }
        }
        public double TotalCajas { get => _totalCajas; set => _totalCajas = value; }
        public double TotalUds { get => _totalUds; set => _totalUds = value; }
        public double TotalKgAprox { get => _totalKgAprox; set => _totalKgAprox = value; }
        public string Fecha { get => _fecha; set => _fecha = value; }
        public string Descripcion { get => _descripcion; set { _descripcion = value; RaisePropertyChanged("Descripcion"); } }
        public string Ean13 { get => _ean13; set { _ean13 = value; RaisePropertyChanged("Ean13"); } }
        public string Sscc { get => _sscc; set { _sscc = value; RaisePropertyChanged("Sscc"); } }

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
