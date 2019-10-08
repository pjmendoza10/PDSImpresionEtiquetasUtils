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
using System.Data.SqlClient;
using System.Data;
using System.Runtime.CompilerServices;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    public class panReImpresionEtiquetaGen01_ViewModel : panPantallaBase_ViewModel
    {
        BackgroundWorker _bkgwk_GuardarEtiquetaEnBDD = new BackgroundWorker();
        BackgroundWorker _bkgwk_BuscarEtiquetas = new BackgroundWorker();
        public enum eSeleccionImpresion { Pantalla = 0, Impresora = 1 }
        public enum eSeleccionFormato { Pequeño = 0, Grande = 1 }
        public enum eSeleccionOF { Todas = 0, Seleccion = 1 }
        public enum eSeleccionFecha { Todas = 0, Seleccion = 1 }
        public enum eSeleccionMQ { Todas = 0, Seleccion = 1 }
        public enum eSeleccionBobina { Todas = 0, Consumidas = 1, NoConsumidas = 2, Final = 3 }
        public enum eSeleccionEstadoOF { Todas = 0, Abiertas = 1, Cerradas = 2 }
        public enum eSeleccionDescripcion { Articulo = 0, ArticuloCliente = 1 }
        
        public panReImpresionEtiquetaGen01_ViewModel()
        {
            VolverPantallaAnterior_Command = new RelayCommand(VolverPantallaAnterior_Command_Execute, VolverPantallaAnterior_Command_CanExecute);
            NuevaEtiqueta_Command = new RelayCommand(NuevaEtiqueta_Command_Execute, NuevaEtiqueta_Command_CanExecute);
            ModificarEtiquetaActual_Command = new RelayCommand(ModificarEtiquetaActual_Command_Execute, ModificarEtiquetaActual_Command_CanExecute);
            VerHistoricoEtiqueta_Command = new RelayCommand(VerHistoricoEtiqueta_Command_Execute, VerHistoricoEtiqueta_Command_CanExecute);
            ImprimirEtiqueta_Command = new RelayCommand(ImprimirEtiqueta_Command_Execute, ImprimirEtiqueta_Command_CanExecute);
            BuscarEtiquetas_Command = new RelayCommand(BuscarEtiquetas_Command_Execute, BuscarEtiquetas_Command_CanExecute);

            PART_Grid_ListaLineasGridBobina_DoubleClick = new RelayCommand(PART_Grid_ListaLineasGridBobina_DoubleClick_Execute);

            ListaLineasGridBobina.CollectionChanged += ListaLineasGridEtiqueta_CollectionChanged;
        }
        #region Overrides

        internal override void Inicializa()
        {
            base.Inicializa();
            _bkgwk_GuardarEtiquetaEnBDD.DoWork += _bkgwk_GuardarEtiquetaEnBDD_DoWork;
            _bkgwk_GuardarEtiquetaEnBDD.RunWorkerCompleted += _bkgwk_GuardarEtiquetaEnBDD_RunWorkerCompleted;

            _bkgwk_BuscarEtiquetas.WorkerReportsProgress = false;
            _bkgwk_BuscarEtiquetas.WorkerSupportsCancellation = false;
            _bkgwk_BuscarEtiquetas.DoWork += _bkgwk_BuscarEtiquetas_DoWork;
            _bkgwk_BuscarEtiquetas.RunWorkerCompleted += _bkgwk_BuscarEtiquetas_RunWorkerCompleted;

            ActualizarListaMQs();

            ListaLineasGridBobina_SelectedItems = new ObservableCollection<DB_pds_progutils_Listado_Etiquetas_GEN01>();
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
           
            ((panReImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

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
                    dbl.DB_pds_progutils_PALETS_Insert(Guid.Parse(Entity.UIDEtiqueta), Entity.Sscc, dbl.DB_pds_progutils_PALETS_GetUIDEtiqueta("8"), serializado);

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

        void _bkgwk_BuscarEtiquetas_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();


            _bkgwk_BuscarEtiquetas.Dispose();

            //_HayOFSegunLote = true;
            ((panReImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

            return;
        }
        void _bkgwk_BuscarEtiquetas_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;

            try
            {
                if (e.Argument.ToString() == "ETIQUETA")
                {
                    BuscarEtiqueta();
                }
                else
                {
                    e.Result = false;
                }
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError("_bkgwk_BuscarEtiquetas_DoWork", ex);
            }
        }

        public delegate void RellenaListaMaquinas_Callback(List<DB_pds_progutils_Lista_MQs_GEN01> db_list_items);
        public void RellenaListaMaquinas(List<DB_pds_progutils_Lista_MQs_GEN01> db_list_items)
        {
            panReImpresionEtiquetaGen01 b_vista = (panReImpresionEtiquetaGen01)View;

            if (!b_vista.Dispatcher.CheckAccess())
            {
                RellenaListaMaquinas_Callback d = new RellenaListaMaquinas_Callback(RellenaListaMaquinas);
                b_vista.Dispatcher.Invoke(d, db_list_items);
            }
            else
            {
                IDisposable d = null;

                try
                {
                    _ListaMQs.Clear();
                    _ListaMQs_SelectedItem = null;

                    if (db_list_items.Count == 0) TextoErrorBuscarMaterialLote = "La lista de MQs no se ha podido actualizar";
                    else TextoErrorBuscarMaterialLote = ""; 

                    foreach (DB_pds_progutils_Lista_MQs_GEN01 i_item in db_list_items)
                    {
                        DB_pds_progutils_Lista_MQs_GEN01 b_lac = new DB_pds_progutils_Lista_MQs_GEN01();
                        b_lac.IDMaquina = i_item.IDMaquina;
                        b_lac.NmeMaquina = i_item.NmeMaquina;

                        _ListaMQs.Add(b_lac);
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

        public delegate void RellenaListaBobinas_Callback(List<DB_pds_progutils_Listado_Etiquetas_GEN01> db_list_items);
        public void RellenaListaBobinas(List<DB_pds_progutils_Listado_Etiquetas_GEN01> db_list_items)
        {
            panReImpresionEtiquetaGen01 b_vista = (panReImpresionEtiquetaGen01)View;

            if (!b_vista.Dispatcher.CheckAccess())
            {
                RellenaListaBobinas_Callback d = new RellenaListaBobinas_Callback(RellenaListaBobinas);
                b_vista.Dispatcher.Invoke(d, db_list_items);
            }
            else
            {
                IDisposable d = null;

                try
                {
                    ListaLineasGridBobina.Clear();
                    ListaLineasGridBobina_SelectedItem = null;

                    if (db_list_items.Count == 0) TextoErrorBuscarMaterialLote = "No se han encontrado resultados";
                    else TextoErrorBuscarMaterialLote = "";

                    foreach (DB_pds_progutils_Listado_Etiquetas_GEN01 i_item in db_list_items)
                    {
                        DB_pds_progutils_Listado_Etiquetas_GEN01 b_lac = new DB_pds_progutils_Listado_Etiquetas_GEN01();
                        b_lac.FechaRegistro = i_item.FechaRegistro;
                        b_lac.Estado = i_item.Estado;
                        b_lac.OrdenID = i_item.OrdenID;
                        b_lac.Codart = i_item.Codart;
                        b_lac.Descripcion = i_item.Descripcion;
                        b_lac.Sit = i_item.Sit;
                        b_lac.OperarioID = i_item.OperarioID;
                        b_lac.NomOperario = i_item.NomOperario;
                        b_lac.CodMatEnt = i_item.CodMatEnt;
                        b_lac.CodMatEnt2 = i_item.CodMatEnt2;
                        b_lac.CodMatEnt3 = i_item.CodMatEnt3;
                        b_lac.CodMatSal = i_item.CodMatSal;
                        b_lac.MetrosBobina = i_item.MetrosBobina;
                        b_lac.MaquinaID = i_item.MaquinaID;
                        b_lac.SeccionID = i_item.SeccionID;
                        b_lac.KgsCalculados = i_item.KgsCalculados;
                        b_lac.OperacionID = i_item.OperacionID;
                        b_lac.DescripOperacionID = i_item.DescripOperacionID;
                        b_lac.BobinaOrigen = i_item.BobinaOrigen;
                        b_lac.BobinaOrigen2 = i_item.BobinaOrigen2;

                        ListaLineasGridBobina.Add(b_lac);
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
                ((panReImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                _IN_OFDesde = "";
                _IN_OFHasta = "";
                TextoErrorBuscarMaterialLote = "";
                Entity.CodArticulo = "";
                Entity.Lote = "";
                Entity.TotalCajas = 0;
                Entity.TotalKgAprox = 0;
                Entity.TotalUds = 0;
                Entity.Descripcion = "";
                Entity.Fecha = "";
                Entity.Ean13 = "";
                ListaLineasGridBobina.Clear();
                ListaMQs.Clear();

                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panReImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panReImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panReImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                //if (ListaClientesEtiquetas_SelectedItem.Id == 1) b_pantalla = (Pantallas.panReImpresionEtiquetaGen01)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value; 
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
            if (ListaLineasGridBobina_SelectedItems != null && ListaLineasGridBobina_SelectedItems.Count > 0) return true;
            //else 
            return false;
        }
        public void ImprimirEtiqueta_Command_Execute()
        {
            try
            {
                clImprimirEtiquetas imprimirEtiquetas = new clImprimirEtiquetas();

                string str1 = "";
                foreach (DB_pds_progutils_Listado_Etiquetas_GEN01 item in _ListaLineasGridBobina_SelectedItems)
                {
                    string str2 = @"UPDATE HISTORICO_TRAZABILIDAD SET Impreso=@impreso WHERE  datediff(second, FECHA_REGISTRO, @FECHA_REGISTRO) = 0 AND OPERARIO_ID=@OPERARIO_ID AND MAQUINA_ID=@MAQUINA_ID AND COD_MAT_SAL=@COD_MAT_SAL";
                    SqlConnection sqlConnection = new SqlConnection(csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_DATOS_2013);
                    sqlConnection.Open();
                    SqlCommand command = sqlConnection.CreateCommand();
                    command.CommandText = str2;
                    command.Parameters.Add("@FECHA_REGISTRO", SqlDbType.DateTime, 50);
                    command.Parameters.Add("@MAQUINA_ID", SqlDbType.VarChar, 15);
                    command.Parameters.Add("@OPERARIO_ID", SqlDbType.VarChar, 15);
                    command.Parameters.Add("@COD_MAT_SAL", SqlDbType.VarChar, 15);
                    command.Parameters.Add("@impreso", SqlDbType.VarChar, 2);
                    command.Prepare();
                    command.Parameters["@FECHA_REGISTRO"].Value = RuntimeHelpers.GetObjectValue(item.FechaRegistro);
                    command.Parameters["@MAQUINA_ID"].Value = RuntimeHelpers.GetObjectValue(item.MaquinaID);
                    command.Parameters["@OPERARIO_ID"].Value = RuntimeHelpers.GetObjectValue(item.OperarioID);
                    command.Parameters["@COD_MAT_SAL"].Value = RuntimeHelpers.GetObjectValue(item.CodMatSal);
                    command.Parameters["@impreso"].Value = RuntimeHelpers.GetObjectValue("No");
               
                    str1 = item.MaquinaID;
                    try
                    {
                        command.ExecuteNonQuery();
                        sqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        //ProjectData.SetProjectError(ex);
                        int num = (int)MessageBox.Show(ex.Message, "ERROR ACTUALIZANDO");
                        //ProjectData.ClearProjectError();
                    }                                           
                }

                SqlConnection connection = new SqlConnection(csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_DATOS_2013);
                string str3 = "";
                try
                {
                    string cmdText = "select terminal_id from OLANET_BASE_2013.dbo.LYC_MAQUINAS_TERMINAL where maquina_id='" + str1 + "'";
                    connection.Open();
                    SqlDataReader sqlDataReader = new SqlCommand(cmdText, connection).ExecuteReader();
                    if (sqlDataReader.Read())
                        str3 = sqlDataReader["terminal_id"].ToString();
                    sqlDataReader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    //ProjectData.SetProjectError(ex);
                    int num = (int)MessageBox.Show("Error " + ex.Message);
                    //ProjectData.ClearProjectError();
                }
                imprimirEtiquetas.PrinterLabel((object)str1, (object)str3, (object)"S", (object)ImprimirZPL);
                //int num1 = (int)MessageBox.Show("Sale por PCSOS " + str3 + " de la máquina " + str1);



                /*
                ((panReImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

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

                if (string.IsNullOrWhiteSpace(IN_OFHasta))
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
                    sLine = sLine.Replace("<<DESCRIPCIONART>>", PAN_ArticuloDescripcion);
                } else
                {
                    
                    if (ListaMQs_SelectedItem != null && ListaMQs_SelectedItem.TieneDescripcionArticulo) sLine = sLine.Replace("<<DESCRIPCIONART>>", ListaMQs_SelectedItem.DescripcionArticuloCliente);
                    else sLine = sLine.Replace("<<DESCRIPCIONART>>", PAN_ArticuloDescripcion);
                   
                }
                sLine = sLine.Replace("<<NUMORDEN>>", DATO_CodigoLote);
                if (ListaMQs_SelectedItem != null && !String.IsNullOrWhiteSpace(ListaMQs_SelectedItem.RefArticuloCliente))
                    sLine = sLine.Replace("<<CODIGOARTICULO>>", ListaMQs_SelectedItem.RefArticuloCliente);
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
                }*/

            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panReImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }
        
        public ICommand PART_Grid_ListaLineasGridBobina_DoubleClick { get; set; }
        public void PART_Grid_ListaLineasGridBobina_DoubleClick_Execute()
        {
            if (ListaLineasGridBobina_SelectedItem == null) return;
        }

        public ICommand BuscarEtiquetas_Command { get; set; }
        public bool BuscarEtiquetas_Command_CanExecute()
        {
            if (_bkgwk_BuscarEtiquetas.IsBusy) return false;

            return true;
        }
        public void BuscarEtiquetas_Command_Execute()
        {
            try
            {
                ((panReImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoErrorBuscarMaterialLote = "";
                
                if (_bkgwk_BuscarEtiquetas.IsBusy != true)
                {
                    if (CamposCorrectos()) _bkgwk_BuscarEtiquetas.RunWorkerAsync("ETIQUETA");
                }

            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panReImpresionEtiquetaGen01)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
            
            // Recalculamos los numeros de lineas
            int b_numero_linea = 1;
            foreach (DB_pds_progutils_Listado_Etiquetas_GEN01 i_item in ListaLineasGridBobina)
            {
                i_item.NumeroLinea = b_numero_linea;
                b_numero_linea++;
            }
        }
        public bool CamposCorrectos()
        {
            if (SeleccionMQ == eSeleccionMQ.Seleccion) { if (ListaMQs_SelectedItem == null) { TextoErrorBuscarMaterialLote = "Seleccione una máquina o Buscar Todas"; return false; } }

            if (SeleccionOF == eSeleccionOF.Seleccion) { if (String.IsNullOrWhiteSpace(IN_OFDesde) && String.IsNullOrWhiteSpace(IN_OFHasta)) { TextoErrorBuscarMaterialLote = "Seleccione una OF o Buscar Todas"; return false; } }

            if (SeleccionFecha == eSeleccionFecha.Seleccion) { if (String.IsNullOrWhiteSpace(IN_FechaDesde) && String.IsNullOrWhiteSpace(IN_FechaHasta)) { TextoErrorBuscarMaterialLote = "Seleccione al menos una Fecha o Buscar Todas"; return false; } }

            return true;
        }
        public void BuscarEtiqueta()
        {
            // DataBaseLayer b_base = new DataBaseLayer("Data Source=PDS-BBDD;Initial Catalog=RPS2013;Trusted_Connection=No;User=RPSUser;Password=rpsuser;MultipleActiveResultSets=True");// FUNCIONA

            //DataBaseLayer b_base = new DataBaseLayer("Data Source=localhost;Initial Catalog=RPS2013;Trusted_Connection=No;User=RPSUserDev;Password=rpsuserdev;MultipleActiveResultSets=True");

            DataBaseLayer b_base = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_DATOS_2013);
            int selBob = 0;
            if (SeleccionBobina == eSeleccionBobina.Consumidas) selBob = 1;
            if (SeleccionBobina == eSeleccionBobina.NoConsumidas) selBob = 2;
            if (SeleccionBobina == eSeleccionBobina.Final) selBob = 3;
            int selOF = 0;
            if (SeleccionEstadoOF == eSeleccionEstadoOF.Abiertas) selOF = 1;
            if (SeleccionEstadoOF == eSeleccionEstadoOF.Cerradas) selOF = 2;
            List<DB_pds_progutils_Listado_Etiquetas_GEN01> db_Bobinas_Item = b_base.DB_Pds_Progutils_Listado_Etiquetas_GetItems(SeleccionMQ == eSeleccionMQ.Todas ? "" : ListaMQs_SelectedItem.IDMaquina,
                                                                                                                                SeleccionFecha == eSeleccionFecha.Todas ? "" : IN_FechaDesde,
                                                                                                                                SeleccionFecha == eSeleccionFecha.Todas ? "" : IN_FechaHasta,
                                                                                                                                SeleccionOF == eSeleccionOF.Todas ? "" : IN_OFDesde,
                                                                                                                                SeleccionOF == eSeleccionOF.Todas ? "" : IN_OFHasta,                                                                                                                                 
                                                                                                                                selBob, selOF);
            
            RellenaListaBobinas(db_Bobinas_Item);
            
            //RaisePropertyChanged("ListaLineasGridBobina");

        }

        public void ActualizarListaMQs()
        {           
            DataBaseLayer b_base = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_DATOS_2013);

            List < DB_pds_progutils_Lista_MQs_GEN01 > db_List_MQs = b_base.DB_Pds_Progutils_Lista_Maquinas_GetItems();

            RellenaListaMaquinas(db_List_MQs);

            RaisePropertyChanged("ListaMQs");
        }
        #endregion



        #region Propiedades

        private csitem_ReimprimirEtiquetaBobina _Entity = new csitem_ReimprimirEtiquetaBobina();
        public csitem_ReimprimirEtiquetaBobina Entity { get => _Entity; set => _Entity = value; }

        public string TextoBotonImpresion { get => textoBotonImpresion; set { textoBotonImpresion = value; RaisePropertyChanged("TextoBotonImpresion"); } }

        private string textoBotonImpresion = "Reimprimir";

        public bool IsStoredInBD = false;

        private eSeleccionFormato _SeleccionFormato = eSeleccionFormato.Pequeño;
        public eSeleccionFormato SeleccionFormato
        {
            get { return _SeleccionFormato; }
            set
            {
                _SeleccionFormato = value;
                RaisePropertyChanged("SeleccionFormato");
            }
        }

        private eSeleccionOF _SeleccionOF = eSeleccionOF.Todas;
        public eSeleccionOF SeleccionOF
        {
            get { return _SeleccionOF; }
            set
            {
                _SeleccionOF = value;
                RaisePropertyChanged("SeleccionOF");
            }
        }

        private eSeleccionFecha _SeleccionFecha = eSeleccionFecha.Todas;
        public eSeleccionFecha SeleccionFecha
        {
            get { return _SeleccionFecha; }
            set
            {
                _SeleccionFecha = value;
                RaisePropertyChanged("SeleccionFecha");
            }
        }

        private eSeleccionMQ _SeleccionMQ = eSeleccionMQ.Todas;
        public eSeleccionMQ SeleccionMQ
        {
            get { return _SeleccionMQ; }
            set
            {
                _SeleccionMQ = value;
                RaisePropertyChanged("SeleccionMQ");
            }
        }
        private eSeleccionBobina _SeleccionBobina = eSeleccionBobina.Todas;
        public eSeleccionBobina SeleccionBobina
        {
            get { return _SeleccionBobina; }
            set
            {
                _SeleccionBobina = value;
                RaisePropertyChanged("SeleccionBobina");
            }
        }

        private eSeleccionEstadoOF _SeleccionEstadoOF = eSeleccionEstadoOF.Todas;
        public eSeleccionEstadoOF SeleccionEstadoOF
        {
            get { return _SeleccionEstadoOF; }
            set
            {
                _SeleccionEstadoOF = value;
                RaisePropertyChanged("SeleccionEstadoOF");
            }
        }
        private eSeleccionImpresion _SeleccionImpresion = eSeleccionImpresion.Pantalla;
        public eSeleccionImpresion SeleccionImpresion
        {
            get { return _SeleccionImpresion; }
            set
            {
                _SeleccionImpresion = value;
                if (_SeleccionImpresion == eSeleccionImpresion.Pantalla) { TextoBotonImpresion = "Vista Previa"; }
                else { TextoBotonImpresion = "Reimprimir"; }
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

        private string _IN_FechaDesde = DateTime.Today.ToShortDateString();
        public string IN_FechaDesde
        {
            get { return _IN_FechaDesde; }
            set { _IN_FechaDesde = value; RaisePropertyChanged("IN_FechaDesde"); }
        }

        private string _IN_FechaHasta = DateTime.Today.ToShortDateString();
        public string IN_FechaHasta
        {
            get { return _IN_FechaHasta; }
            set { _IN_FechaHasta = value; RaisePropertyChanged("IN_FechaHasta"); }
        }
        private string _IN_OFDesde = "";
        public string IN_OFDesde
        {
            get { return _IN_OFDesde; }
            set { _IN_OFDesde = value;  RaisePropertyChanged("IN_OFDesde"); }
        }

        private string _IN_OFHasta = "";
        public string IN_OFHasta
        {
            get { return _IN_OFHasta; }
            set { _IN_OFHasta = value; RaisePropertyChanged("IN_OFHasta"); }
        }

        private bool _imprimirZPL = false;
        public bool ImprimirZPL { get => _imprimirZPL; set { _imprimirZPL = value; RaisePropertyChanged("ImprimirZPL"); } }

        private ObservableCollection<DB_pds_progutils_Listado_Etiquetas_GEN01> _ListaLineasGridBobina = new ObservableCollection<DB_pds_progutils_Listado_Etiquetas_GEN01>();
        public ObservableCollection<DB_pds_progutils_Listado_Etiquetas_GEN01> ListaLineasGridBobina
        {
            get { return _ListaLineasGridBobina; }
            set
            {
                _ListaLineasGridBobina = value;
                RaisePropertyChanged("ListaLineasGridBobina");
            }
        }

        private ObservableCollection<DB_pds_progutils_Listado_Etiquetas_GEN01> _ListaLineasGridBobina_SelectedItems = null;
        public ObservableCollection<DB_pds_progutils_Listado_Etiquetas_GEN01> ListaLineasGridBobina_SelectedItems
        {
            get { return _ListaLineasGridBobina_SelectedItems; }
            set { _ListaLineasGridBobina_SelectedItems = value; RaisePropertyChanged("ListaLineasGridBobina_SelectedItems"); }
        }

        private DB_pds_progutils_Listado_Etiquetas_GEN01 _ListaLineasGridBobina_SelectedItem = null;
        public DB_pds_progutils_Listado_Etiquetas_GEN01 ListaLineasGridBobina_SelectedItem
        {
            get { return _ListaLineasGridBobina_SelectedItem; }
            set { _ListaLineasGridBobina_SelectedItem = value;  RaisePropertyChanged("ListaLineasGridBobina_SelectedItem"); }
        }

        private ObservableCollection<DB_pds_progutils_Lista_MQs_GEN01> _ListaMQs = new ObservableCollection<DB_pds_progutils_Lista_MQs_GEN01>();
        public ObservableCollection<DB_pds_progutils_Lista_MQs_GEN01> ListaMQs
        {
            get { return _ListaMQs; }
            set { _ListaMQs = value; RaisePropertyChanged("ListaMQs"); }
        }

        private DB_pds_progutils_Lista_MQs_GEN01 _ListaMQs_SelectedItem = null;
        public DB_pds_progutils_Lista_MQs_GEN01 ListaMQs_SelectedItem
        {
            get { return _ListaMQs_SelectedItem; }
            set
            {
                _ListaMQs_SelectedItem = value;                
                RaisePropertyChanged("ListaMQs_SelectedItem");
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

        public class csitem_ReimprimirEtiquetaBobina : ObservableObject, IDataErrorInfo
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

            public string UIDEtiqueta { get => _uidEtiqueta; set { _uidEtiqueta = value; RaisePropertyChanged("UIDEtiqueta"); } }

            private string _uidEtiqueta = null;
            public string CodArticulo { get => _codArticulo; set { _codArticulo = value; RaisePropertyChanged("CodArticulo"); } }
            public string Lote { get => _lote; set { _lote = value; RaisePropertyChanged("Lote"); } }
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
            #endregion
        }


    }
