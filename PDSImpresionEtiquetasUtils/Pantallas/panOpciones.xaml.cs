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
using PDSIEUCo = PDSImpresionEtiquetasUtils.Comun;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
        /// <summary>
        /// Lógica de interacción para panInicial.xaml
        /// </summary>
        public partial class panOpciones : UserControl, PDSIEUCo.IPantallasContenedor
        {
            MainWindow _pantalla_principal = null;
            private Pantallas.panOpciones_ViewModel _viewmodel;

            public panOpciones(MainWindow p_main_window, PDSIEUCo.csConfiguracion p_configuracion)
            {
                InitializeComponent();

                PantallaAnterior = null;
                _pantalla_principal = p_main_window;
                _viewmodel = (Pantallas.panOpciones_ViewModel)this.DataContext;
                _viewmodel.View = View;
                _viewmodel.PantallaPrincipal = p_main_window;
                _viewmodel.Configuracion = p_configuracion;
                _viewmodel.Entity = csEstadoPermanente.Configuracion; 

                this.Loaded += Pantalla_Loaded;
                this.Unloaded += Pantalla_Unloaded;

                _viewmodel.Inicializa();
            }

            #region IPantallasContenedor

            public static string CIdentificador
            {
                get { return "OPCIONES"; }
            }

            public string Identificador
            {
                get { return CIdentificador; }
            }

            public object View
            {
                get { return this; }
            }

            //public wUserControlToWindow Window { get; set; }

            private PDSIEUCo.csConfiguracion _configuracion;
            public PDSIEUCo.csConfiguracion Configuracion
            {
                get { return _configuracion; }
                set { _configuracion = value; }
            }

            Guid _id_unico = Guid.NewGuid();
            public Guid IDUnico
            {
                get { return _id_unico; }
            }

            public delegate void SetCursor_Callback(string p_cursor_name);
            public void SetCursor(string p_cursor_name)
            {
                MainWindow b_vista = (MainWindow)_pantalla_principal;

                if (!b_vista.Dispatcher.CheckAccess())
                {
                    SetCursor_Callback d = new SetCursor_Callback(SetCursor);
                    b_vista.Dispatcher.Invoke(d, p_cursor_name);
                }
                else
                {

                    IDisposable d = null;

                    try
                    {
                        this.Cursor = PDSIEUCo.Utilidades.VariosComunesWindows.GetCursorByName(p_cursor_name);
                    }
                    finally
                    {
                        if (d != null) d.Dispose();
                    }
                }
            }


            public object Window { get; set; }

            public PDSIEUCo.IPantallasContenedor PantallaAnterior { get; set; }

            #endregion

            #region Eventos

            void Pantalla_Loaded(object sender, RoutedEventArgs e)
            {
                _pantalla_principal.PantallaLoaded((PDSIEUCo.IPantallasContenedor)this);
            }

            void Pantalla_Unloaded(object sender, RoutedEventArgs e)
            {
                _pantalla_principal.PantallaUnloaded(this);
            }

            #endregion


        }
    }
