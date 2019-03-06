using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PDSImpresionEtiquetasUtilsInit
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        csConfiguracion _configuracion = null;

        private MainWindow_ViewModel _viewmodel;

        bool _ejecutar_actualizacion = false;
        bool _ejecutar_actulizacion_instalador = false;

        public string _nombre_ejecutable_normal_instalador = "PDSImpresionEtiquetasUtilsInit.exe";
        public string _nombre_ejecutable_nuevo_instalador = "PDSImpresionEtiquetasUtilsInit.new.exe";

        public bool _soy_new = false;

        public MainWindow()
        {
            bool b_res = false;

            InitializeComponent();

            try
            {
                //// Grabamos el fichero ligs
                //string b_path = System.IO.Path.Combine(_configuracion.Datos.PathVersionActualizada, "Logs");
                //string b_linea = Environment.MachineName + " | " + "PDSImpresionEtiquetasInit - Inicio del Actualizador (MainWindow).";
                //csLogUtil01.EscribeLineaLog(b_path, b_linea);


                // Compruebo si soy NEW
                string b_nombre = System.AppDomain.CurrentDomain.FriendlyName;
                _soy_new = b_nombre == _nombre_ejecutable_nuevo_instalador;
                if (_soy_new)
                {
                    // SOY NEW
                    // me copia a normal
                    System.IO.File.Copy(_nombre_ejecutable_nuevo_instalador, _nombre_ejecutable_normal_instalador, true);

                    // Continuo normalmente
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(System.AppDomain.CurrentDomain.FriendlyName + " " + ex.Message);
                this.Close();
                return;
            }

            _configuracion = new csConfiguracion();

            _viewmodel = (MainWindow_ViewModel)base.DataContext;
            _viewmodel.PantallaPrincipal = this;

            try
            {
                _configuracion.Cargar();
            }
            catch (Exception ex1)
            {
                // No encuentra el fichero de configuracion -> Intenta lanzar el programa principal
                System.Diagnostics.Process.Start("PDSImpresionEtiquetasUtilsInit.exe");
                this.Close();
            }

            _viewmodel.Configuracion = _configuracion;
            _configuracion.EscribirFicheroVersion();
            _viewmodel.Inicializa();

            if (!_soy_new)
            {
                _ejecutar_actulizacion_instalador = _viewmodel.ComprobarActualizacionInstalador();

                try
                {
                    if (_ejecutar_actulizacion_instalador)
                    {
                        _viewmodel.EjecutarActualizacionInstalador();

                        //MessageBox.Show(System.AppDomain.CurrentDomain.FriendlyName + " A2.1");

                        // Ejecutamos la nueva
                        System.Diagnostics.Process.Start(_nombre_ejecutable_nuevo_instalador);
                        // Salimos
                        this.Close();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(System.AppDomain.CurrentDomain.FriendlyName + " A2");
                    MessageBox.Show(System.AppDomain.CurrentDomain.FriendlyName + " " + ex.Message);
                }
            }

            _ejecutar_actualizacion = _viewmodel.ComprobarActualizacionProgramaPrincipal();

            try
            {
                if (!_ejecutar_actualizacion)
                {

                    System.Diagnostics.Process.Start("PDSImpresionEtiquetasUtils.exe");
                    this.Close();
                }
                else
                {
                    _viewmodel.EjecutarActualizacionProgramaPrincipal();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(System.AppDomain.CurrentDomain.FriendlyName + " " + ex.Message);
            }

        }

        public void Progreso(int p_valor)
        {
            pgbPrincipal.Dispatcher.Invoke(() => pgbPrincipal.Value = p_valor, DispatcherPriority.Background);
            //pgbPrincipal.Value = p_valor;

        }
    }
}