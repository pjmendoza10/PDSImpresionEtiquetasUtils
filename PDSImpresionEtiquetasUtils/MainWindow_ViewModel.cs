using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basoa.Framework;
using Basoa.Framework.Configuration;
using Basoa.Services.Security;
using MicroMvvm;
using PDSImpresionEtiquetasUtils.Conectores;
using System.Reflection;
using System.Windows.Input;
using PDSIEUCo = PDSImpresionEtiquetasUtils.Comun;
using System.ComponentModel;

namespace PDSImpresionEtiquetasUtils
{ 
    public class MainWindow_ViewModel : ObservableObject
    {
        Dictionary<string, PDSIEUCo.IPantallasContenedor> _pantallas_abiertas = new Dictionary<string, PDSIEUCo.IPantallasContenedor>();

        BackgroundWorker _bkgwk_IniciaRPS = new BackgroundWorker();

        //private bool _RPS_Cargado = false;

        public MainWindow_ViewModel()
        {
            // commands
            ViewInicial_Command = new RelayCommand(ViewInicial_Command_Execute, ViewInicial_Command_CanExecute);
            ViewTipoEtiqueta_Command = new RelayCommand(ViewTipoEtiqueta_Command_Execute, ViewTipoEtiqueta_Command_CanExecute);
            ViewEtiquetaCajaBobina_Command = new RelayCommand(ViewEtiquetaCajaBobina_Command_Execute, ViewEtiquetaCajaBobina_Command_CanExecute);
            /*ViewGeneraEtiqRepro01_Command = new RelayCommand(ViewGeneraEtiqRepro01_Command_Execute, ViewGeneraEtiqRepro01_Command_CanExecute);
            ViewAdiccionMatTarea01_Command = new RelayCommand(ViewAdiccionMatTarea01_Command_Execute, ViewAdiccionMatTarea01_Command_CanExecute);
            ReevioOFEjecutableAlSGA01_Command = new RelayCommand(ReevioOFEjecutableAlSGA01_Command_Execute, ReevioOFEjecutableAlSGA01_Command_CanExecute);*/

            ViewPanOpcionesGlobal_Command = new RelayCommand(ViewPanOpcionesGlobal_Command_Execute, ViewPanOpcionesGlobal_Command_CanExecute);


            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName assemName = assem.GetName();
            Version ver = assemName.Version;
            _Version = ver.ToString();
        }

    #region Propiedades

        PDSIEUCo.csConfiguracion _configuracion = null;
        public PDSIEUCo.csConfiguracion Configuracion
        {
            get { return _configuracion; }
            set { _configuracion = value; }
        }

        MainWindow _pantalla_Principal = null;
        public MainWindow PantallaPrincipal
        {
            get { return _pantalla_Principal; }
            set { _pantalla_Principal = value; }
        }

        public List<PDSIEUCo.IPantallasContenedor> ListaPantallasAbiertas
        {
            get { return _pantallas_abiertas.Values.ToList<PDSIEUCo.IPantallasContenedor>(); }
        }

        private string _Version = "Version";
        public string Version
        {
            get { return _Version; }
        }

        //public SROUCo.Conectores.IConector Conector { get; set; }

        /*#region Comportamiento y Estilo

        private bool _ModifCantMatTareas_Command_MuestraCargando = false;
        public bool ModifCantMatTareas_Command_MuestraCargando
        {
            get { return _ModifCantMatTareas_Command_MuestraCargando; }
            set { _ModifCantMatTareas_Command_MuestraCargando = value; RaisePropertyChanged("ModifCantMatTareas_Command_MuestraCargando"); }
        }

        private bool _GeneraEtiqRepro01_Command_MuestraCargando = false;
        public bool GeneraEtiqRepro01_Command_MuestraCargando
        {
            get { return _GeneraEtiqRepro01_Command_MuestraCargando; }
            set { _GeneraEtiqRepro01_Command_MuestraCargando = value; RaisePropertyChanged("GeneraEtiqRepro01_Command_MuestraCargando"); }
        }

        private bool _AdiccionMatTareas_Command_MuestraCargando = false;
        public bool AdiccionMatTareas_Command_MuestraCargando
        {
            get { return _AdiccionMatTareas_Command_MuestraCargando; }
            set { _AdiccionMatTareas_Command_MuestraCargando = value; RaisePropertyChanged("AdiccionMatTareas_Command_MuestraCargando"); }
        }

        private bool _ReevioOFEjecutableAlSGA01_Command_MuestraCargando = false;
        public bool ReevioOFEjecutableAlSGA01_Command_MuestraCargando
        {
            get { return _ReevioOFEjecutableAlSGA01_Command_MuestraCargando; }
            set { _ReevioOFEjecutableAlSGA01_Command_MuestraCargando = value; RaisePropertyChanged("ReevioOFEjecutableAlSGA01_Command_MuestraCargando"); }
        }

        #endregion*/

        #endregion

        #region Command
        public ICommand ViewInicial_Command { get; set; }
        public bool ViewInicial_Command_CanExecute()
        {
            return true;
        }
        public void ViewInicial_Command_Execute()
        {
            if (_pantalla_Principal.PantallaActual.Identificador == Pantallas.panInicial.CIdentificador) return;

            Pantallas.panInicial b_pantalla = new Pantallas.panInicial(PantallaPrincipal, Configuracion);

            PantallaPrincipal.BotonMenuPrincipalPulsado_Animaciones();

            PantallaPrincipal.CambiarPantalla(b_pantalla);

            return;

            /*
            // Diseñador de informes Oculto
            Stimulsoft.Report.StiReport report = new Stimulsoft.Report.StiReport();

            PDSIEUCo.Informes.Origenes.csOrigen_PALETSTD_UTILS01 b_d_palet = new PDSIEUCo.Informes.Origenes.csOrigen_PALETSTD_UTILS01();
            b_d_palet.CodArticulo = "1";
            b_d_palet.DescripcionArticulo = "D1";

            b_d_palet.Lista = new List<PDSIEUCo.Informes.Origenes.csOrigen_PALETSTD_UTILS01.csItemLineas>();
            b_d_palet.Lista.Add(new PDSIEUCo.Informes.Origenes.csOrigen_PALETSTD_UTILS01.csItemLineas() { NumeroElementos = "2", UnidadesXElemento = "22", TotalUnidades = "T2" });

            report.Load(@csEstadoPermanente.Configuracion.Datos.Ruta_informe_GER01_Palet01);
            report.RegBusinessObject("D1", "D1", b_d_palet);
            report.Dictionary.SynchronizeBusinessObjects(3);
            report.DesignWithWpf();
            //report.Show();*/
        }

        public ICommand ViewTipoEtiqueta_Command { get; set; }
        public bool ViewTipoEtiqueta_Command_CanExecute()
        {
            return true;
        }

        public void ViewTipoEtiqueta_Command_Execute()
        {
            if (_pantalla_Principal.PantallaActual.Identificador == Pantallas.panTipoEtiqueta.CIdentificador) return;

            Pantallas.panTipoEtiqueta b_pantalla = new Pantallas.panTipoEtiqueta();

            if (Utilidades.UtilesCarga._pantallas_abiertas.Any(z => z.Key == b_pantalla.ToString()))
            {
                b_pantalla = (Pantallas.panTipoEtiqueta)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value;
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
        
        public ICommand ViewEtiquetaCajaBobina_Command { get; set; }
        public bool ViewEtiquetaCajaBobina_Command_CanExecute()
        {
            return true;
        }

        public void ViewEtiquetaCajaBobina_Command_Execute()
        {
            if (_pantalla_Principal.PantallaActual.Identificador == Pantallas.panEtiquetaCajaBobina.CIdentificador) return;

            Pantallas.panEtiquetaCajaBobina b_pantalla = new Pantallas.panEtiquetaCajaBobina();

            if (Utilidades.UtilesCarga._pantallas_abiertas.Any(z => z.Key == b_pantalla.ToString()))
            {
                b_pantalla = (Pantallas.panEtiquetaCajaBobina)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == b_pantalla.ToString()).Value;
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
        public ICommand ViewPanOpcionesGlobal_Command { get; set; }
        public bool ViewPanOpcionesGlobal_Command_CanExecute()
        {
            return true;
        }

        public void ViewPanOpcionesGlobal_Command_Execute()
        {
            if (_pantalla_Principal.PantallaActual.Identificador == Pantallas.panOpciones.CIdentificador) return;

            Pantallas.panOpciones b_pantalla = new Pantallas.panOpciones(PantallaPrincipal, Configuracion);

            PantallaPrincipal.BotonMenuPrincipalPulsado_Animaciones();

            PantallaPrincipal.CambiarPantalla(b_pantalla);

            return;
        }
        #endregion

        #region Public Functions

        public void Inicializa()
        {
            int b_login = 0;
            do
            {

                //b_login = EjecutaProcesoLogin(false);
                b_login = 1;

            } while (b_login == 0);

            if (b_login == -1)
            {
                this._pantalla_Principal.Close();
            }


            /*_bkgwk_IniciaRPS.WorkerSupportsCancellation = false;
            _bkgwk_IniciaRPS.DoWork += _bkgwk_IniciaRPS_DoWork;
            //_bkgwk_inicializar.ProgressChanged += _wk_actualizar_ProgressChanged;
            _bkgwk_IniciaRPS.RunWorkerCompleted += _bkgwk_IniciaRPS_RunWorkerCompleted;

            if (_bkgwk_IniciaRPS.IsBusy != true)
            {
                _bkgwk_IniciaRPS.RunWorkerAsync();
            }*/
        }

        public void PantallaLoaded(PDSIEUCo.IPantallasContenedor p_pantalla)
        {
            _pantallas_abiertas.Add(p_pantalla.IDUnico.ToString(), p_pantalla);
        }

        public void PantallaUnloaded(PDSIEUCo.IPantallasContenedor p_pantalla)
        {
            _pantallas_abiertas.Remove(p_pantalla.IDUnico.ToString());
        }

        public void InicializarDirectorioDatos()
        {
            // Inicializamos el directorio de datos
            try
            {
                string b_dir_app;

                Assembly a = Assembly.GetEntryAssembly();
                string exeDir = System.IO.Path.GetDirectoryName(a.Location);
                //_stream_log = new StreamWriter(new FileStream(exeDir + "\\Log.txt", FileMode.Append, FileAccess.Write));

                string b_base = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

                //string b_subdir = "PDSScdService";
                //string b_dir = System.IO.Path.Combine(b_base, b_subdir);
                //b_subdir = "MQTTBroker";
                //b_dir = System.IO.Path.Combine(b_dir, b_subdir);



                // Creamos el subdirectorio principal de datos comunes a todos los usuarios
                b_dir_app = PDSIEUCo.csConfiguracion.DamePath_CommonApplicationData;
                if (!System.IO.Directory.Exists(b_dir_app))
                {
                    PDSImpresionEtiquetasUtils.Utilidades.CommonApplicationData.CreateFolder(b_dir_app, true);
                }

                // C
                string b_dir_usr = PDSIEUCo.csConfiguracion.DamePath_UserAppDataPath;
                if (!System.IO.Directory.Exists(b_dir_usr))
                {
                    System.IO.Directory.CreateDirectory(b_dir_usr);
                }

                string b_file_config = System.IO.Path.Combine(b_dir_app, PDSIEUCo.csConfiguracion.DameNombreFicheroConfiguracion);
                string b_file_config_default = System.IO.Path.Combine(exeDir, PDSIEUCo.csConfiguracion.DameNombreFicheroConfiguracion);

                //LogUtil.EscribeLineaLog(b_file_config_default);
                //LogUtil.EscribeLineaLog(b_file_config);

                // Si no existe el fichero de configuracion lo copiamos
                if (!System.IO.File.Exists(b_file_config)) System.IO.File.Copy(b_file_config_default, b_file_config, true);



            }
            catch (Exception ex)
            {
            //this.EventLog.WriteEntry("this.ServiceName. " + ex.ToString(), EventLogEntryType.Error, 1);
            PDSIEUCo.Utilidades.csLogUtils.EscribeLineaLogError("InicializarDirectorioDatos", ex);
            }
        }

        #endregion

        #region BackgroundWorker

       /* void _bkgwk_IniciaRPS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Boolean)
            {
                csEstadoPermanente.isRPSCargado = (bool)e.Result;
            }
            CommandManager.InvalidateRequerySuggested();


            _bkgwk_IniciaRPS.Dispose();

            return;
        }
        void _bkgwk_IniciaRPS_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;

            try
            {
                ModifCantMatTareas_Command_MuestraCargando = true;
                GeneraEtiqRepro01_Command_MuestraCargando = true;
                AdiccionMatTareas_Command_MuestraCargando = true;
                ReevioOFEjecutableAlSGA01_Command_MuestraCargando = true;

                var a = new StaticSessionProvider();
                BasoaServiceConfiguration.Initialize(new StaticSessionProvider());


                RPS.csRPSFunciones b_f = new RPS.csRPSFunciones();

                var b_com = b_f.GetCompany();

                if (b_com != null) e.Result = true;
                //e.Result = true;
            }
            catch (Exception ex)
            {
                SROUCo.Utilidades.csLogUtils.EscribeLineaLogError("_bkgwk_IniciaRPS_DoWork", ex);
                e.Result = false;
            }
        }*/

        #endregion
        /*
        #region private function

        public int EjecutaProcesoLogin(bool p_cambiar)
        {

            bool b_res = false;
            bool? res;
            string b_IDUsuario = "";
            bool b_hay_login = false;

            // Comprobamos primero las guardadas en el fichero config
            //System.Windows.Forms.MessageBox.Show("1");

            DBConector_Internal b_conector = new DBConector_Internal(this.Configuracion.Datos.connectionString_PDSImpresionEtiquetas);


            int b_res_auto = b_conector.CompruebaCredencialesUsuario(_configuracion.OpcionesAplicacion.UsuarioLogin ?? string.Empty, _configuracion.OpcionesAplicacion.UsuarioPassword ?? string.Empty, out b_IDUsuario);
            if ((b_res_auto == 1) && (!p_cambiar))
            {
                _configuracion.IDUsuarioActual = b_IDUsuario;
                _configuracion.CodUsuarioActual = _configuracion.OpcionesAplicacion.UsuarioLogin;
                if (b_IDUsuario != "") b_hay_login = true;
            }
            else
            {

                //PDSScdClienteSCADA.Pantallas.Dialogos.winLogin01 p_logon = new Pantallas.Dialogos.winLogin01(_configuracion);

                //res = p_logon.ShowDialog();

                //if ((res != null) && ((bool)res))
                //{
                //    _configuracion.IDUsuarioActual = p_logon.ViewModel.IDUsuario;
                //    _configuracion.CodUsuarioActual = p_logon.ViewModel.CodUsuario;
                //    if (p_logon.ViewModel.IDUsuario != "") b_hay_login = true;
                //}
            }

            if (b_hay_login)
            {
                // Cargar las opciones del usuario
                AplicarOpcionesUsuario(_configuracion.IDUsuarioActual);
                return 1;
            }

            // Cancelado
            return -1;
        }

        public void AplicarOpcionesUsuario(string p_IDUsuarioActual)
        {

            //DBConector_Internal b_conector = new DBConector_Internal(this.Configuracion.Datos.connectionString_PDSImpresionEtiquetas);

            //List<DB_Usuarios_PermisosSeccionesApp> b_lista_permisos = b_db.Usuarios_PermisosSeccionesApp_GetListObj(p_IDUsuarioActual);

            //foreach (DB_Usuarios_PermisosSeccionesApp i_item in b_lista_permisos)
            //{
            //    bool b_valor = i_item.Valor;
            //    switch (i_item.CodSeccionApp)
            //    {
            //        case "EDITORMAIN01":
            //            _HayPermiso_EDITORMAIN01 = b_valor;
            //            RaisePropertyChanged("HayPermiso_EDITORMAIN01");
            //            break;
            //        case "PANVIEWALLTAGS01":
            //            _HayPermiso_PANVIEWALLTAGS01 = b_valor;
            //            RaisePropertyChanged("HayPermiso_PANVIEWALLTAGS01");
            //            break;
            //        case "PANSCADAGENERAL01":
            //            _HayPermiso_PANSCADAGENERAL01 = b_valor;
            //            RaisePropertyChanged("HayPermiso_PANSCADAGENERAL01");
            //            break;
            //        case "PANSCADAMAQUINASSECCIONES01":
            //            _HayPermiso_PANSCADAMAQUINASSECCIONES01 = b_valor;
            //            RaisePropertyChanged("HayPermiso_PANSCADAMAQUINASSECCIONES01");
            //            break;
            //        case "OPCIONES01":
            //            _HayPermiso_OPCIONES01 = b_valor;
            //            RaisePropertyChanged("HayPermiso_OPCIONES01");
            //            break;
            //        case "PANSGAUT_MODCANTIDADESMATERIALES01":
            //            _HayPermiso_PANSGAUT_MODCANTIDADESMATERIALES01 = b_valor;
            //            RaisePropertyChanged("HayPermiso_PANSGAUT_MODCANTIDADESMATERIALES01");
            //            break;
            //    }
            //}
        }

        #endregion*/
    }
}
