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

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    public class panImpresionEtiquetaESTIU_ViewModel : panPantallaBase_ViewModel
    {
        BackgroundWorker _bkgwk_GuardarEtiquetaEnBDD = new BackgroundWorker();
        BackgroundWorker _bkgwk_BuscarDatosEnBDD = new BackgroundWorker();
        public enum eSeleccionImpresion { Impresora = 0, Pantalla = 1 }

        private int _n_max_lineas_detalle_palet = 10;

        public panImpresionEtiquetaESTIU_ViewModel()
        {
            VolverPantallaAnterior_Command = new RelayCommand(VolverPantallaAnterior_Command_Execute, VolverPantallaAnterior_Command_CanExecute);
            NuevaEtiqueta_Command = new RelayCommand(NuevaEtiqueta_Command_Execute, NuevaEtiqueta_Command_CanExecute);
            ModificarEtiquetaActual_Command = new RelayCommand(ModificarEtiquetaActual_Command_Execute, ModificarEtiquetaActual_Command_CanExecute);
            VerHistoricoEtiqueta_Command = new RelayCommand(VerHistoricoEtiqueta_Command_Execute, VerHistoricoEtiqueta_Command_CanExecute);
            ImprimirEtiqueta_Command = new RelayCommand(ImprimirEtiqueta_Command_Execute, ImprimirEtiqueta_Command_CanExecute);
            BuscarDatos_Command = new RelayCommand(BuscarDatos_Command_Execute, BuscarDatos_Command_CanExecute);
            
            PART_Grid_ListaLineasGridEtiqueta_DoubleClick = new RelayCommand(PART_Grid_ListaLineasGridEtiqueta_DoubleClick_Execute);

            Entity.ListaLineasGridEtiqueta.CollectionChanged += ListaLineasGridEtiqueta_CollectionChanged;
        }
        #region Overrides

        internal override void Inicializa()
        {
            base.Inicializa();

            Entity.PesoMandril = 0.3;
            Entity.PesoPlastico = 13.76;
            Entity.Cliente = "HELADOS ESTIU, S.A.";
            Entity.PesoPalet = 24;
            Entity.AnchoBobina = "200";

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

            ((panImpresionEtiquetaESTIU)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());

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
                    dbl.DB_pds_progutils_PALETS_Insert(Guid.Parse(Entity.UIDEtiqueta), Entity.NumPalet, dbl.DB_pds_progutils_PALETS_GetUIDEtiqueta("3"), serializado);

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
                    //DataBaseLayer b_base = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_RPS2013_OLANET);
                    //DataBaseLayer b_base = new DataBaseLayer("Data Source=OLANET;Initial Catalog=RPS2013_OLANET;User ID=sa;Password=plasticos");
                    //DataBaseLayer b_base = new DataBaseLayer("Data Source=PDS-BBDD;Initial Catalog=RPS2013;Trusted_Connection=No;User=RPSUser;Password=rpsuser;MultipleActiveResultSets=True");/* FUNCIONA*/
                    DataBaseLayer b_base = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_DATOS_2013);
                    if (Entity.NumPalet.StartsWith("00") && Entity.NumPalet.Count() == 20) Entity.NumPalet = Entity.NumPalet.Substring(2, Entity.NumPalet.Count() - 2);
                    DB_pds_progutils_ETIQ01_PALETS_ESTIU_GEN01 db_ESTIU_item = b_base.DB_pds_progutils_ETIQ01_PALETS_ESTIU_GEN01_GetItem(Entity.NumPalet);
                    if (String.IsNullOrWhiteSpace(db_ESTIU_item.CodArticulo))
                    {
                        DataBaseLayer b_reprocesado = new DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_RPS2013_OLANET);
                        db_ESTIU_item = b_base.DB_pds_progutils_ETIQ01_PALETS_ESTIU_REP01_GetItem(Entity.NumPalet);

                        List<DB_pds_progutils_LINEAS_SSCC_REP01> db_ESTIU_lineas = b_base.DB_Pds_Progutils_LINEAS_SSCC_REP01_GetItems(db_ESTIU_item.IDEtiquetaPalet);
                        RellenaListaArticuloCliente(db_ESTIU_lineas);

                        MessageBox.Show("Este palet proviene de reprocesado", "Atención");
                    }
                    else
                    {
                        List<DB_pds_progutils_LINEAS_SSCC_GEN01> db_ESTIU_lineas = b_base.DB_Pds_Progutils_LINEAS_SSCC_GEN01_GetItems(Entity.NumPalet);
                        RellenaListaArticuloCliente(db_ESTIU_lineas);
                    }
                    Entity.CodArticulo = db_ESTIU_item.CodArticulo;
                    Entity.NumBobinas = db_ESTIU_item.CantidadPalet;
                    Entity.Cliente = db_ESTIU_item.Cliente;
                    Entity.CodCliente = db_ESTIU_item.IdCliente;
                    Entity.Descripcion = db_ESTIU_item.Descripcion;
                    Entity.Lote = db_ESTIU_item.Lote;
                    Entity.PedidoCliente = db_ESTIU_item.PedidoESTIU;
                    Entity.RefCliente = db_ESTIU_item.ReferenciaESTIU;
                    Entity.TotalMetros = db_ESTIU_item.TotalMetros;
                    Entity.MetrosBobina = Math.Round(db_ESTIU_item.TotalMetros / db_ESTIU_item.CantidadPalet,2) ;

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
        public delegate void RellenaListaArticuloCliente_Callback(List<DB_pds_progutils_LINEAS_SSCC_GEN01> db_list_items);
        public void RellenaListaArticuloCliente(List<DB_pds_progutils_LINEAS_SSCC_GEN01> db_list_items)
        {
            panImpresionEtiquetaESTIU b_vista = (panImpresionEtiquetaESTIU)View;

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
                    _Entity.ListaLineasGridEtiqueta.Clear();
                    _Entity.ListaLineasGridEtiqueta_SelectedItem = null;
                    foreach (DB_pds_progutils_LINEAS_SSCC_GEN01 item in db_list_items)
                    {
                        csItem_ListaLineasGridEtiquetaEstiu newitem = new csItem_ListaLineasGridEtiquetaEstiu();
                        newitem.NumBobinas = item.NumBobinas;
                        newitem.MetrosPorBobina = item.MetrosXBobina;
                        Entity.ListaLineasGridEtiqueta.Add(newitem);
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

        public delegate void RellenaListaArticuloClienteRep_Callback(List<DB_pds_progutils_LINEAS_SSCC_REP01> db_list_items);
        public void RellenaListaArticuloCliente(List<DB_pds_progutils_LINEAS_SSCC_REP01> db_list_items)
        {
            panImpresionEtiquetaESTIU b_vista = (panImpresionEtiquetaESTIU)View;

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
                    _Entity.ListaLineasGridEtiqueta.Clear();
                    _Entity.ListaLineasGridEtiqueta_SelectedItem = null;
                    foreach (DB_pds_progutils_LINEAS_SSCC_REP01 item in db_list_items)
                    {
                        csItem_ListaLineasGridEtiquetaEstiu newitem = new csItem_ListaLineasGridEtiquetaEstiu();
                        newitem.NumBobinas = item.NumElementos;
                        newitem.MetrosPorBobina = item.UnidadesXElemento;
                        newitem.TotalMetros = item.TotalUnidades;
                        Entity.TotalMetros += item.TotalUnidades;
                        Entity.NumBobinas += item.NumElementos;
                        Entity.ListaLineasGridEtiqueta.Add(newitem);
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
                ((panImpresionEtiquetaESTIU)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());
                Entity.AlbaranPDS = null;
                Entity.NumBobinas = 0;
                Entity.CodArticulo = null;
                Entity.Lote = null;
                Entity.MetrosBobina = 0;
                Entity.Descripcion = null;
                Entity.TotalMetros = 0;
                Entity.PedidoCliente = null;
                Entity.PesoNetoSMandril = 0;
                Entity.PesoBrutoPaletizado = 0;
                //Entity.PesoPalet = null;
                Entity.PesoNetoPaletizado = 0;
                Entity.NumPalet = null;
                Entity.RefCliente = null;
                //Entity.PesoPlastico = null;
                //Entity.PesoMandril = null;
                //Entity.AnchoBobina = null;

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
                ((panImpresionEtiquetaESTIU)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
                ((panImpresionEtiquetaESTIU)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                TextoBotonImpresion = "Imprimir y Guardar";
                IsStoredInBD = false;
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaESTIU)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
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
            panHistorico b_pantalla = new Pantallas.panHistorico("3"); 
            
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
                ((panImpresionEtiquetaESTIU)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());

                if (!CamposCorrectosParaImpresion()) return;
                if (!IsStoredInBD) {
                if (_bkgwk_GuardarEtiquetaEnBDD.IsBusy != true)
                    {
                        _bkgwk_GuardarEtiquetaEnBDD.RunWorkerAsync("GUARDARENBDD");
                    }
                }

                csGeneraEtiqRepro_Comun imprimir = new csGeneraEtiqRepro_Comun();
                PrinterSettings printerSettings = new PrinterSettings();
                imprimir.ImprimeEtiquetaPalet(printerSettings, Entity, Convert.ToBoolean(SeleccionImpresion), true, ((panImpresionEtiquetaESTIU)View).Dispatcher);
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaESTIU)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public Boolean CamposCorrectosParaImpresion() {
            if (String.IsNullOrWhiteSpace(Entity.AlbaranPDS)) { MessageBox.Show("Falta el campo Albaran PDS", "Debe rellenar todos los campos obligatorios"); return false; }
            if (String.IsNullOrWhiteSpace(Entity.AnchoBobina)) { MessageBox.Show("Falta el campo Ancho Bobina", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.CodArticulo)) { MessageBox.Show("Falta el campo Codigo Articulo", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.Descripcion)) { MessageBox.Show("Falta el campo Descripcion", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.NumPalet)) { MessageBox.Show("Falta el campo Numero Palet", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.PedidoCliente)) { MessageBox.Show("Falta el campo Pedido Cliente", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.Lote)) { MessageBox.Show("Falta el campo Lote", "Debe rellenar todos los campos obligatorios"); return false;}
            if (Entity.PesoNetoPaletizado == 0) { MessageBox.Show("Falta el campo Peso Neto Paletizado o es igual a 0", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.PesoMandril.ToString())) { MessageBox.Show("Falta el campo Peso Mandril", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.PesoPalet.ToString())) { MessageBox.Show("Falta el campo Peso Palet", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.PesoPlastico.ToString())) { MessageBox.Show("Falta el campo Peso Plastico", "Debe rellenar todos los campos obligatorios"); return false;}
            if (String.IsNullOrWhiteSpace(Entity.RefCliente)) { MessageBox.Show("Falta el campo Referencia Cliente", "Debe rellenar todos los campos obligatorios"); return false;}
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
                    ((panImpresionEtiquetaESTIU)this.View).SetCursor(System.Windows.Input.Cursors.Wait.ToString());
                    _bkgwk_BuscarDatosEnBDD.RunWorkerAsync("BUSCARENBDD");
                }
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                ((panImpresionEtiquetaESTIU)this.View).SetCursor(System.Windows.Input.Cursors.Arrow.ToString());
            }
        }

        public ICommand PART_Grid_ListaLineasGridEtiqueta_DoubleClick { get; set; }
        public void PART_Grid_ListaLineasGridEtiqueta_DoubleClick_Execute()
        {
            if (Entity.ListaLineasGridEtiqueta_SelectedItem == null) return;
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
            foreach (csItem_ListaLineasGridEtiquetaEstiu i_item in Entity.ListaLineasGridEtiqueta)
            {
                i_item.NumeroLinea = b_numero_linea;
                b_numero_linea++;
            }
        }

        public void ActualizarDatosTotales()
        {
            Entity.TotalMetros = 0; Entity.NumBobinas = 0;
            foreach (csItem_ListaLineasGridEtiquetaEstiu item in Entity.ListaLineasGridEtiqueta)
            {
                Entity.NumBobinas += item.NumBobinas; Entity.TotalMetros += item.TotalMetros; 
            }
        }

        #endregion

        #region Properties
        private csitem_EtiquetaEstiuT1 _Entity = new csitem_EtiquetaEstiuT1();
        public csitem_EtiquetaEstiuT1 Entity { get => _Entity; set => _Entity = value; }
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
    public class csItem_ListaLineasGridEtiquetaEstiu : ObservableObject, IDataErrorInfo
    {
        private double _NumBobinas;
        private double _MetrosPorBobina;
        private double _TotalMetros;
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
        public double NumBobinas { get => _NumBobinas; set { _NumBobinas = value; RaisePropertyChanged("NumBobinas"); if (_NumBobinas > 0 && _MetrosPorBobina > 0) TotalMetros = NumBobinas * _MetrosPorBobina; } }
        public double MetrosPorBobina { get => _MetrosPorBobina; set { _MetrosPorBobina = value; RaisePropertyChanged("MetrosPorBobina"); if (_NumBobinas > 0 && _MetrosPorBobina > 0) TotalMetros = NumBobinas * _MetrosPorBobina; } }
        public double TotalMetros { get => _TotalMetros; set { _TotalMetros = value; RaisePropertyChanged("TotalMetros"); } }
    }

    public class csitem_EtiquetaEstiuT1 : ObservableObject, IDataErrorInfo
    {
        private string _codArticulo;
        private string _lote;
        private string _descripcion;
        private string _refCliente;
        private string _anchoBobina;
        private string _albaranPDS;
        private string _pedidoCliente;
        private string _numPalet;
        private double _pesoMandril;
        private double _pesoPlastico;
        private double _pesoPalet;
        private double _pesoNetoSMandril;
        private double _pesoBrutoPaletizado;
        private double _pesoNetoPaletizado;
        private double _numBobinas;
        private double _metrosBobina;
        private double _totalMetros;
        private string _cliente;
        private string _codCliente;

        private string _uidEtiqueta = null;

        private ObservableCollection<csItem_ListaLineasGridEtiquetaEstiu> _ListaLineasGridEtiqueta = new ObservableCollection<csItem_ListaLineasGridEtiquetaEstiu>();
        public ObservableCollection<csItem_ListaLineasGridEtiquetaEstiu> ListaLineasGridEtiqueta
        {
            get { return _ListaLineasGridEtiqueta; }
            set
            {
                _ListaLineasGridEtiqueta = value;
                RaisePropertyChanged("ListaLineasGridEtiqueta");
            }
        }

        private csItem_ListaLineasGridEtiquetaEstiu _ListaLineasGridEtiqueta_SelectedItem = null;
        public csItem_ListaLineasGridEtiquetaEstiu ListaLineasGridEtiqueta_SelectedItem
        {
            get { return _ListaLineasGridEtiqueta_SelectedItem; }
            set { _ListaLineasGridEtiqueta_SelectedItem = value; RaisePropertyChanged("ListaLineasGridEtiqueta_SelectedItem"); }
        }

        public string UIDEtiqueta { get => _uidEtiqueta; set { _uidEtiqueta = value; RaisePropertyChanged("UIDEtiqueta"); } }
        public string CodCliente { get => _codCliente; set { _codCliente = value; RaisePropertyChanged("CodCliente"); } }
        public string Cliente { get => _cliente; set { _cliente = value; RaisePropertyChanged("Cliente"); } }
        public string CodArticulo { get => _codArticulo; set { _codArticulo = value; RaisePropertyChanged("CodArticulo"); } }
        public string Lote { get => _lote; set { _lote = value; RaisePropertyChanged("Lote"); } }
        public string Descripcion { get => _descripcion; set { _descripcion = value; RaisePropertyChanged("Descripcion"); } }
        public string RefCliente { get => _refCliente; set { _refCliente = value; RaisePropertyChanged("RefCliente"); } }
        public string AnchoBobina { get => _anchoBobina; set { _anchoBobina = value; RaisePropertyChanged("AnchoBobina"); } }
        public string AlbaranPDS { get => _albaranPDS; set { _albaranPDS = value; RaisePropertyChanged("AlbaranPDS"); } }
        public string PedidoCliente { get => _pedidoCliente; set { _pedidoCliente = value; RaisePropertyChanged("PedidoCliente"); } }
        public string NumPalet { get => _numPalet; set { _numPalet = value; RaisePropertyChanged("NumPalet"); } }
        public double PesoMandril { get => _pesoMandril; set { _pesoMandril = value; RaisePropertyChanged("PesoMandril"); CalcularPesos(); } }
        public double PesoPlastico { get => _pesoPlastico; set { _pesoPlastico = value; RaisePropertyChanged("PesoPlastico"); } }
        public double PesoPalet { get => _pesoPalet; set { _pesoPalet = value; RaisePropertyChanged("PesoPalet"); CalcularPesos(); } }
        public double PesoBrutoPaletizado { get => _pesoBrutoPaletizado; set { _pesoBrutoPaletizado = value; RaisePropertyChanged("PesoBrutoPaletizado"); } }
        public double PesoNetoPaletizado { get => _pesoNetoPaletizado; set { _pesoNetoPaletizado = value; RaisePropertyChanged("PesoNetoPaletizado"); CalcularPesos(); } }
        public double PesoNetoSMandril { get => _pesoNetoSMandril; set { _pesoNetoSMandril = value; RaisePropertyChanged("PesoNetoSMandril"); } }
        public double NumBobinas { get => _numBobinas; set { _numBobinas = value; RaisePropertyChanged("NumBobinas"); CalcularPesos(); } }
        public double MetrosBobina { get => _metrosBobina; set { _metrosBobina = value; RaisePropertyChanged("MetrosBobina"); } }
        public double TotalMetros { get => _totalMetros; set { _totalMetros = value; RaisePropertyChanged("TotalMetros"); } }
        public string Error
        {
            get { return ""; }
        }
        
        public string this[string columnName]
        {
            get { return ""; }
        }
        
        public void CalcularPesos()
        {
            if (NumBobinas > 0 && PesoMandril > 0 && PesoNetoPaletizado > 0) PesoNetoSMandril = PesoNetoPaletizado - NumBobinas * PesoMandril;
            if (PesoPalet > 0 && PesoNetoPaletizado > 0) PesoBrutoPaletizado = PesoNetoPaletizado + PesoPalet;
        }
    }
    
}
