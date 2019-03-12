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
    public class panImpresionEtiquetaSIRO_ViewModel : panPantallaBase_ViewModel
    {
        BackgroundWorker _bkgwk_GuardarEtiquetaEnBDD = new BackgroundWorker();
        BackgroundWorker _bkgwk_BuscarDatosEnBDD = new BackgroundWorker();
        public enum eSeleccionImpresion { Impresora = 0, Pantalla = 1 }

        public panImpresionEtiquetaSIRO_ViewModel()
        {
            VolverPantallaAnterior_Command = new RelayCommand(VolverPantallaAnterior_Command_Execute, VolverPantallaAnterior_Command_CanExecute);
            NuevaEtiqueta_Command = new RelayCommand(NuevaEtiqueta_Command_Execute, NuevaEtiqueta_Command_CanExecute);
            ModificarEtiquetaActual_Command = new RelayCommand(ModificarEtiquetaActual_Command_Execute, ModificarEtiquetaActual_Command_CanExecute);
            VerHistoricoEtiqueta_Command = new RelayCommand(VerHistoricoEtiqueta_Command_Execute, VerHistoricoEtiqueta_Command_CanExecute);
            ImprimirEtiqueta_Command = new RelayCommand(ImprimirEtiqueta_Command_Execute, ImprimirEtiqueta_Command_CanExecute);
            BuscarDatos_Command = new RelayCommand(BuscarDatos_Command_Execute, BuscarDatos_Command_CanExecute);
        }
        #region Overrides

        internal override void Inicializa()
        {
            base.Inicializa();

            Entity.NumProveedor = "5968";
            Entity.Proveedor = "PDS GROUP (Plásticos del Segura)";

            _bkgwk_GuardarEtiquetaEnBDD.DoWork += _bkgwk_GuardarEtiquetaEnBDD_DoWork;
            _bkgwk_GuardarEtiquetaEnBDD.RunWorkerCompleted += _bkgwk_GuardarEtiquetaEnBDD_RunWorkerCompleted;

            _bkgwk_BuscarDatosEnBDD.DoWork += _bkgwk_BuscarDatosEnBDD_DoWork;
            _bkgwk_BuscarDatosEnBDD.RunWorkerCompleted += _bkgwk_BuscarDatosEnBDD_RunWorkerCompleted;
            
            //Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkwZq45nPGAko2oALZBw1FWWXY16RykCKFf+yV7YGSudKKAFa" +
            //                "bWFnX8ZxzW9A94fjHt13HDD/9VuqFr/I6w2xLWlgK1LdLJ44jKwYl2fdnU4tRqW6TikEGFH8EkNQelPjRK4U9ZZvfE" +
            //                "J1s9zEAqZHSqHSK492vSZsdfD6kTWbvLVnI0VewxJEG5macBZ0t0i/UBE6L9ru0Msx0R5U8XbAFyyLtVHUjXkAXbV3" +
            //                "B2NodfLXEUuaeA4OKKnqzzCYe0x5YymVDe0hqYeiGgAATUK9hFrSwsbfUD2I59QF15kQ4TD54JHiKB4nKWP1xnPHS6" +
            //                "aVn3wLlk3YNlDpTdN6L97fblL8NgfX+Gl8Pcyim4hdjfebHK7VBXXRsiic5s1nH9Z7eAXRm55i0FnoLe+ctwmwEpdV" +
            //                "AFDX+FyvK/1oNiksKj12YqfY3MJz6fVr/T3WiUR8BMz8KhjLrlLAxyqybi2jOE7pvhyyVM8ZiFEW7OJoALd70asnZG" +
            //                "7U781k+JB8fQbZX1KWYniUgWhzE5gsW7t6sBG8AfSsFYyPzoLvinSqD5SQ==";

            //Stimulsoft.Base.sti

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

            ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

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
                    DBConector_OLANET_BASE_2013 b_con_obase2013 = new DBConector_OLANET_BASE_2013(csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_2013);
                    string b_error = "";
                    string b_sscc = b_con_obase2013.GetNewSSCCCode(ref b_error);
                    Entity.Sscc = b_sscc;
                    // TODO Guardar En BDD
                    Entity.UIDEtiqueta = Guid.NewGuid().ToString(); 
                    dbl.DB_pds_progutils_PALETS_Insert(Guid.Parse(Entity.UIDEtiqueta), Entity.Sscc, dbl.DB_pds_progutils_PALETS_GetUIDEtiqueta("2"), serializado);

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

        void _bkgwk_BuscarDatosEnBDD_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();

            _bkgwk_BuscarDatosEnBDD.Dispose();

            ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

            return;
        }

        void _bkgwk_BuscarDatosEnBDD_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;
            try
            {
                if (e.Argument.ToString() == "BUSCARENBDD")
                {
                    //DataBaseLayer b_base = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_RPS2013_OLANET);
                    //DataBaseLayer b_base = new DataBaseLayer("Data Source=OLANET;Initial Catalog=RPS2013_OLANET;User ID=sa;Password=plasticos");
                    //DataBaseLayer b_base = new DataBaseLayer("Data Source=PDS-BBDD;Initial Catalog=RPS2013;Trusted_Connection=No;User=RPSUser;Password=rpsuser;MultipleActiveResultSets=True");/* FUNCIONA*/
                    DataBaseLayer b_base = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_RPS2013);
                    DB_pds_progutils_ETIQ01_PALETS_SIRO_LINEAS db_SIRO_item = b_base.dB_Pds_Progutils_ETIQ01_PALETS_SIRO_LINEAS_GetItem(ReferenciaPedido, Entity.PedidoGrupoSIRO);
                    ReferenciaPedido = db_SIRO_item.CodPedido;
                    Entity.CodArticulo = db_SIRO_item.CodArticulo.ToString();
                    Entity.Cantidad = db_SIRO_item.Cantidad.ToString();
                    Entity.Observaciones = db_SIRO_item.ComentariosEnvio;
                    Entity.DescripcionReferencia = db_SIRO_item.DescripcionArticulo;
                    Entity.Unidad = db_SIRO_item.DescripcionUdMedida;
                    Entity.FabricaDestino = db_SIRO_item.FabricaDestino;
                    Entity.FechaEntrega = db_SIRO_item.FechaRecepcionEstimada;
                    Entity.PedidoGrupoSIRO = db_SIRO_item.PedidoGrupoSIRO;
                    Entity.ReferenciaSIRO = db_SIRO_item.ReferenciaSIRO;

                    DB_pds_progutils_DIMENSIONES_ARTICULO_GEN01 db_SIRO_item_dim = b_base.DB_Pds_Progutils_DIMENSIONES_ARTICULO_GEN01_GetItem(Entity.CodArticulo);
                    List<DB_pds_progutils_FEATURES_ARTICLE_GEN01> db_SIRO_item_feat_palet = b_base.DB_Pds_Progutils_FEATURES_ARTICLE_GEN01_GetItems(db_SIRO_item_dim.CodPalet);
                    List<DB_pds_progutils_FEATURES_ARTICLE_GEN01> db_SIRO_item_feat_caja = new List<DB_pds_progutils_FEATURES_ARTICLE_GEN01>();
                    if (db_SIRO_item_dim.TieneCaja) db_SIRO_item_feat_caja = b_base.DB_Pds_Progutils_FEATURES_ARTICLE_GEN01_GetItems(db_SIRO_item_dim.CodCaja);
                    DatosPalet.Volumen = CalcularVolumen(db_SIRO_item_dim, db_SIRO_item_feat_palet, db_SIRO_item_feat_caja);
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

        public string CalcularVolumen (DB_pds_progutils_DIMENSIONES_ARTICULO_GEN01 articulo, List<DB_pds_progutils_FEATURES_ARTICLE_GEN01> palet, List<DB_pds_progutils_FEATURES_ARTICLE_GEN01> caja)
        {
            string resultado = "";
            DatosPalet.AnchoPalet = Convert.ToDouble(palet.ElementAt(palet.FindIndex(x => x.CodFeature.Contains("X"))).ValueFeature) / 100;
            resultado += Math.Truncate(DatosPalet.AnchoPalet) + "x";
            DatosPalet.LargoPalet = Convert.ToDouble(palet.ElementAt(palet.FindIndex(x => x.CodFeature.Contains("Y"))).ValueFeature) / 100;
            resultado += Math.Truncate(DatosPalet.LargoPalet) + "x";
            DatosPalet.NumAlturas = Convert.ToDouble(articulo.Altura);
            DatosPalet.AltoPalet = Convert.ToDouble(palet.ElementAt(palet.FindIndex(x => x.CodFeature.Contains("Z"))).ValueFeature) / 100;
            if (articulo.TieneCaja) //sumamos Alto de la caja (Coordenada Z) * Altura posible en palet y a eso le sumamos la altura del palet (Coordenada Z)
            {
                DatosPalet.AltoMaterial = Convert.ToDouble(caja.ElementAt(caja.FindIndex(x => x.CodFeature.Contains("Z"))).ValueFeature) / 100;
            } else //si no tiene caja, lo que tenemos son bobinas. Utilizamos el ancho del articulo como altura de elemento en el palet
            {
                DatosPalet.AltoMaterial = Convert.ToDouble(articulo.Ancho);
            }
                resultado += Math.Truncate((DatosPalet.AltoMaterial * DatosPalet.NumAlturas) + DatosPalet.AltoPalet) ;

            resultado += " cm3";

            return resultado;
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
                ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());
                Entity.Cantidad = null;
                Entity.CuentaPaletAlbaran = null;
                Entity.DescripcionReferencia = null;
                Entity.FabricaDestino = null;
                Entity.FechaCaducidad = null;
                Entity.FechaEntrega = null;
                Entity.FechaFabricacion = null;
                Entity.Lote = null;
                //Entity.NumProveedor = null;
                Entity.Observaciones= null;
                Entity.PedidoGrupoSIRO = null;
                Entity.Peso = null;
                Entity.PickupSheet = null;
                //Entity.Proveedor = null;
                Entity.ReferenciaSIRO = null;
                Entity.Unidad = null;
                Entity.Volumen = null;
                ReferenciaPedido = null;
                DatosPalet.LargoPalet = 0;
                DatosPalet.NumAlturas = 0;
                DatosPalet.AnchoPalet = 0;
                DatosPalet.AltoMaterial = 0;
                DatosPalet.AltoPalet= 0;
                DatosPalet.Volumen = "";

                TextoBotonImpresion = "Imprimir y Guardar";
                IsStoredInBD = false;
                /*if (_bkgwk_FiltrarFicheroArticuloLote.IsBusy != true)
                {
                    _bkgwk_FiltrarFicheroArticuloLote.RunWorkerAsync("FILTRAR");
                }*/
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoBotonImpresion = "Imprimir y Guardar";
                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
            try
            {
                ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                /*if (_bkgwk_FiltrarFicheroArticuloLote.IsBusy != true)
                {
                    _bkgwk_FiltrarFicheroArticuloLote.RunWorkerAsync("FILTRAR");
                }*/
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
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
                ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                if (!CamposCorrectosParaImpresion()) return;
                if (!IsStoredInBD) {
                if (_bkgwk_GuardarEtiquetaEnBDD.IsBusy != true)
                    {
                        _bkgwk_GuardarEtiquetaEnBDD.RunWorkerAsync("GUARDARENBDD");
                    }
                }

                csGeneraEtiqRepro_Comun imprimir = new csGeneraEtiqRepro_Comun();
                PrinterSettings printerSettings = new PrinterSettings();
                Entity.Volumen = DatosPalet.Volumen;
                imprimir.ImprimeEtiquetaPalet(printerSettings, Entity, Convert.ToBoolean(SeleccionImpresion), true, ((panImpresionEtiquetaSIRO)View).Dispatcher);

            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public Boolean CamposCorrectosParaImpresion() {
            if (String.IsNullOrWhiteSpace(Entity.Cantidad)) { MessageBox.Show("Falta el campo Cantidad","Debe rellenar todos los campos obligatorios"); return false; }
            if (String.IsNullOrWhiteSpace(Entity.CuentaPaletAlbaran)) { MessageBox.Show("Falta el campo Cuenta Palet Albaran", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.DescripcionReferencia)) { MessageBox.Show("Falta el campo Descripcion Referencia", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.FabricaDestino)) { MessageBox.Show("Falta el campo Fabrica Destino", "Debe rellenar todos los campos obligatorios"); return false;}
            //if (String.IsNullOrWhiteSpace(Entity.FechaCaducidad)) return false;}
            if (String.IsNullOrWhiteSpace(Entity.FechaEntrega)) { MessageBox.Show("Falta el campo Fecha Entrega", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.FechaFabricacion)) { MessageBox.Show("Falta el campo Fecha Fabricacion", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.Lote)) { MessageBox.Show("Falta el campo Lote", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.NumProveedor)) { MessageBox.Show("Falta el campo Num Proveedor", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.PedidoGrupoSIRO)) { MessageBox.Show("Falta el campo Pedido Grupo SIRO", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.Peso)) { MessageBox.Show("Falta el campo Peso", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.PickupSheet)) { MessageBox.Show("Falta el campo Pickup Sheet", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.Proveedor)) { MessageBox.Show("Falta el campo Proveedor", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.ReferenciaSIRO)) { MessageBox.Show("Falta el campo Referencia SIRO", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.Unidad)) { MessageBox.Show("Falta el campo Unidad", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(DatosPalet.Volumen)) { MessageBox.Show("Falta el campo Volumen", "Debe rellenar todos los campos obligatorios"); return false;}
            return true;
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
                    ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());
                    _bkgwk_BuscarDatosEnBDD.RunWorkerAsync("BUSCARENBDD");
                }
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaSIRO)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }
        #endregion

        #region Properties
        private csitem_DatosPalet _DatosPalet = new csitem_DatosPalet();
        private csitem_EtiquetaSiroT1 _Entity = new csitem_EtiquetaSiroT1();
        public csitem_DatosPalet DatosPalet { get => _DatosPalet; set => _DatosPalet = value; }
        public csitem_EtiquetaSiroT1 Entity { get => _Entity; set => _Entity = value; }
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

        public string ReferenciaPedido { get => _referenciaPedido; set { _referenciaPedido = value; RaisePropertyChanged("ReferenciaPedido"); } }

        private string _referenciaPedido;
        #endregion


    }

    public class csitem_DatosPalet : ObservableObject, IDataErrorInfo
    {
        private string _volumen;
        private double _numAlturas;
        private double _anchoPalet;
        private double _altoPalet;
        private double _largoPalet;
        private double _altoMaterial;

        public double NumAlturas { get => _numAlturas; set { _numAlturas = value; RecalcularVolumen(); RaisePropertyChanged("NumAlturas"); } }
        public double AnchoPalet { get => _anchoPalet; set { _anchoPalet = value; RecalcularVolumen(); RaisePropertyChanged("AnchoPalet"); } }
        public double AltoPalet { get => _altoPalet; set { _altoPalet = value; RecalcularVolumen(); RaisePropertyChanged("AltoPalet"); } }
        public double LargoPalet { get => _largoPalet; set { _largoPalet = value; RecalcularVolumen(); RaisePropertyChanged("LargoPalet"); } }
        public double AltoMaterial { get => _altoMaterial; set { _altoMaterial = value; RecalcularVolumen(); RaisePropertyChanged("AltoMaterial"); } }
        public string Volumen { get => _volumen; set { _volumen = value; RaisePropertyChanged("Volumen"); } }

        public string Error
        {
            get { return ""; }
        }
        public string this[string columnName]
        {
            get { return ""; }
        }
        public void RecalcularVolumen()
        {
            Volumen = AnchoPalet + "x" + LargoPalet + "x" + Math.Truncate((AltoMaterial * NumAlturas) + AltoPalet) + " cm3";
        }

    }

    public class csitem_EtiquetaSiroT1 : ObservableObject, IDataErrorInfo
    {
        public string Sscc { get => _sscc; set { _sscc = value; RaisePropertyChanged("Sscc"); } }
        public string CodArticulo { get => _codArticulo; set { _codArticulo = value; RaisePropertyChanged("CodArticulo"); } }
        public string NumProveedor { get => _numProveedor; set { _numProveedor = value; RaisePropertyChanged("NumProveedor"); } }
        public string Proveedor { get => _proveedor; set { _proveedor = value; RaisePropertyChanged("Proveedor"); } }
        public string PedidoGrupoSIRO { get => _pedidoGrupoSIRO; set { _pedidoGrupoSIRO = value; RaisePropertyChanged("PedidoGrupoSIRO"); } }
        public string PickupSheet { get => _pickupSheet; set { _pickupSheet = value; RaisePropertyChanged("PickupSheet"); } }
        public string FabricaDestino { get => _fabricaDestino; set { _fabricaDestino = value; RaisePropertyChanged("FabricaDestino"); } }
        public string DescripcionReferencia { get => _descripcionReferencia; set { _descripcionReferencia = value; RaisePropertyChanged("DescripcionReferencia"); } }
        public string ReferenciaSIRO { get => _referenciaSIRO; set { _referenciaSIRO = value; RaisePropertyChanged("ReferenciaSIRO"); } }
        public string Cantidad { get => _cantidad; set { _cantidad = value; RaisePropertyChanged("Cantidad"); } }
        public string Unidad { get => _unidad; set { _unidad = value; RaisePropertyChanged("Unidad"); } }
        public string Peso { get => _peso; set { _peso = value; RaisePropertyChanged("Peso"); } }
        public string Volumen { get => _volumen; set { _volumen = value; RaisePropertyChanged("Volumen"); } }
        public string FechaEntrega { get => _fechaEntrega; set { _fechaEntrega = value; RaisePropertyChanged("FechaEntrega"); } }
        public string FechaFabricacion { get => _fechaFabricacion; set { _fechaFabricacion = value; RaisePropertyChanged("FechaFabricacion"); } }
        public string FechaCaducidad { get => _fechaCaducidad; set { _fechaCaducidad = value; RaisePropertyChanged("FechaCaducidad"); } }
        public string Lote { get => _lote; set { _lote = value; RaisePropertyChanged("Lote"); } }
        public string CuentaPaletAlbaran { get => _cuentaPaletAlbaran; set { _cuentaPaletAlbaran = value; RaisePropertyChanged("CuentaPaletAlbaran"); } }
        public string Observaciones { get => _observaciones; set { _observaciones = value; RaisePropertyChanged("Observaciones"); } }
        public string UIDEtiqueta { get => _uidEtiqueta; set { _uidEtiqueta = value; RaisePropertyChanged("UIDEtiqueta"); } }

        private string _sscc  = null;

        private string _uidEtiqueta = null;

        private string _codArticulo = null;

        private string _numProveedor = null;

        private string _proveedor = null;

        private string _pedidoGrupoSIRO = null;

        private string _pickupSheet = null;

        private string _fabricaDestino = null;

        private string _descripcionReferencia = null;

        private string _referenciaSIRO = null;

        private string _cantidad = null;

        private string _unidad = null;

        private string _peso = null;

        private string _volumen = null;

        private string _fechaEntrega = null;

        private string _fechaFabricacion = null;

        private string _fechaCaducidad = null;

        private string _lote = null;

        private string _cuentaPaletAlbaran = null;

        private string _observaciones = null;

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
