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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PDSIEUCo = PDSImpresionEtiquetasUtils.Comun;

    namespace PDSImpresionEtiquetasUtils
    {
        /// <summary>
        /// Lógica de interacción para MainWindow.xaml
        /// </summary>
        public partial class MainWindow : Window
        {
            private PDSIEUCo.IPantallasContenedor _pantalla_Inicial;
            private PDSIEUCo.IPantallasContenedor _pantalla_actual = null;
            private PDSIEUCo.IPantallasContenedor _pantalla_anterior = null;

            private bool _menu_principal_oculto = false;
            //private bool _forzar_ocultar_menu_principal = false;
            private int _punto_inicio_contenedor_principal_rein = 50;

            public MainWindow()
            {
                InitializeComponent();

                csEstadoPermanente.Configuracion = new PDSIEUCo.csConfiguracion();
                csEstadoPermanente.PantallaPrincipal = this;

                _viewmodel = (MainWindow_ViewModel)base.DataContext;
                _viewmodel.PantallaPrincipal = this;
                _viewmodel.InicializarDirectorioDatos();
                csEstadoPermanente.Configuracion.Cargar();
                _configuracion = csEstadoPermanente.Configuracion;
                _configuracion.EscribirFicheroVersion();
                _viewmodel.Configuracion = _configuracion;
                _viewmodel.Inicializa();
            }

        #region Propiedades

        PDSIEUCo.csConfiguracion _configuracion = null;
            public PDSIEUCo.csConfiguracion Configuracion
            {
                get { return _configuracion; }
            }

            public PDSIEUCo.IPantallasContenedor PantallaActual
            {
                get { return _pantalla_actual; }
            }

            private MainWindow_ViewModel _viewmodel;
            public MainWindow_ViewModel ViewModel
            {
                get { return _viewmodel; }
            }

            public Window ObjetoWindow { get { return this; } }


            #endregion

            #region Funciones

            #region Pantallas

            public void CambiarPantalla(PDSIEUCo.IPantallasContenedor p_pantalla_nueva, bool b_inversa = false)
            {

                if (p_pantalla_nueva.Identificador != Pantallas.panInicial.CIdentificador)
                {
                    contenedorPrincipal.Width = gridPrincipal.ActualWidth;
                }
                else
                {
                    contenedorPrincipal.Width = gridPrincipal.ActualWidth - gridMenuPrincipal.ActualWidth;
                }

                _pantalla_anterior = _pantalla_actual;
                contenedorPrincipal.Content = p_pantalla_nueva;

                _pantalla_actual = p_pantalla_nueva;
                contenedorPrincipal.UpdateLayout();

                if (_pantalla_anterior.Identificador == Pantallas.panInicial.CIdentificador)
                {
                    AnimaContenedorPrincipal_In();
                    _pantalla_anterior = null;
                }
                else
                {
                    AnimaContenedorPrincipal_SlideAndFadeIn(b_inversa);
                    //_pantalla_anterior = null;
                }

                if (_pantalla_actual.Identificador == Pantallas.panInicial.CIdentificador)
                {
                    AnimaContenedorPrincipal_Out();
                    AnimaMenuPrincipal_SlideIn(gridMenuPrincipal);
                }

            }

            public void PantallaLoaded(PDSIEUCo.IPantallasContenedor p_pantalla)
            {
                _viewmodel.PantallaLoaded(p_pantalla);
            }

            public void PantallaUnloaded(PDSIEUCo.IPantallasContenedor p_pantalla)
            {
                _viewmodel.PantallaUnloaded(p_pantalla);
            }

            public void CambiarAPantallaHome()
            {
                _viewmodel.ViewInicial_Command_Execute();
            }

            #endregion

            //public void DoInicializarDatosPresupuestoOP(Action p_RemoteInicializa)
            //{
            //    //_viewmodel.DoInicializarDatosPresupuestoOP(p_RemoteInicializa);
            //}

            #endregion

            #region Eventos

            private void Window_Loaded(object sender, RoutedEventArgs e)
            {

                _pantalla_Inicial = new Pantallas.panInicial(this, _configuracion);
                _pantalla_actual = _pantalla_Inicial;
                contenedorPrincipal.Width = gridPrincipal.ActualWidth - gridMenuPrincipal.ActualWidth;
                contenedorPrincipal.Content = _pantalla_actual;

            //_viewmodel.InicializaSCADA();

            PDSIEUCo.Utilidades.csLogUtils.EscribeLineaLog("MainWindow.Window_Loaded OK");
            }

            private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
            {
                // Cerramos las pantallas abiertas

                foreach (object i_pan in _viewmodel.ListaPantallasAbiertas)
                {
                    if (((PDSIEUCo.IPantallasContenedor)i_pan).Window is Window)
                    {
                        ((Window)((PDSIEUCo.IPantallasContenedor)i_pan).Window).Close();
                    }
                    else
                    {
                        try
                        {
                            ((Window)i_pan).Close();
                        }
                        catch { }
                    }
                }

                //_viewmodel.DetenerSCADA();
            }

            private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
            {
                if ((_pantalla_actual != null) && (_pantalla_actual.Identificador != Pantallas.panInicial.CIdentificador))
                {
                    contenedorPrincipal.Width = gridPrincipal.ActualWidth;
                }
                else
                {
                    double b_diff = gridPrincipal.ActualWidth - gridMenuPrincipal.ActualWidth;
                    contenedorPrincipal.Width = b_diff >= 100 ? b_diff : 100;
                }
            }

            private void gridMenuPrincipal_MouseEnter(object sender, MouseEventArgs e)
            {
                if (_pantalla_actual.Identificador == Pantallas.panInicial.CIdentificador) return;

                AnimaMenuPrincipal_SlideIn(gridMenuPrincipal);
            }

            private void gridMenuPrincipal_MouseLeave(object sender, MouseEventArgs e)
            {
                if (_pantalla_actual.Identificador == Pantallas.panInicial.CIdentificador) return;

                AnimaMenuPrincipal_SlideOut(gridMenuPrincipal);
            }

            #endregion

            #region Animaciones

            private void AnimaContenedorPrincipal_FadeIn()
            {
                // Create a storyboard to contain the animations.
                Storyboard storyboard = new Storyboard();
                TimeSpan duration = new TimeSpan(0, 0, 0, 0, 150);

                // Create a DoubleAnimation to fade the not selected option control
                DoubleAnimation animation = new DoubleAnimation();

                animation.From = 0.0;
                animation.To = 1.0;
                animation.Duration = new Duration(duration);
                // Configure the animation to target de property Opacity
                Storyboard.SetTargetName(animation, contenedorPrincipal.Name);
                Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));
                // Add the animation to the storyboard
                storyboard.Children.Add(animation);

                // Begin the storyboard
                storyboard.Begin(this);
            }

            private void AnimaContenedorPrincipal_SlideAndFadeIn(bool b_inversa = false)
            {
                // Create a storyboard to contain the animations.
                Storyboard storyboard = new Storyboard();
                TimeSpan duration = new TimeSpan(0, 0, 0, 0, 100);

                int b_punto_inicio_contenedor_principal_rein = _punto_inicio_contenedor_principal_rein;

                // FADE IN
                // Create a DoubleAnimation to fade the not selected option control
                DoubleAnimation animation = new DoubleAnimation();
                //if (b_inversa)
                //{
                //    animation.From = 1.0;
                //    animation.To = 0.0;
                //}
                //else 
                //{
                animation.From = 0.0;
                animation.To = 1.0;
                //}
                animation.Duration = new Duration(duration);
                // Configure the animation to target de property Opacity
                Storyboard.SetTargetName(animation, contenedorPrincipal.Name);
                Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));

                // SLIDE
                b_punto_inicio_contenedor_principal_rein = (int)Math.Round(contenedorPrincipal.Width * 0.5);
                Thickness reta;
                if (b_inversa)
                {
                    reta = new Thickness(-b_punto_inicio_contenedor_principal_rein, contenedorPrincipal.Margin.Top, contenedorPrincipal.Margin.Right - b_punto_inicio_contenedor_principal_rein, contenedorPrincipal.Margin.Bottom);
                }
                else
                {
                    reta = new Thickness(b_punto_inicio_contenedor_principal_rein, contenedorPrincipal.Margin.Top, contenedorPrincipal.Margin.Right, contenedorPrincipal.Margin.Bottom);
                }
                ThicknessAnimation ta = new ThicknessAnimation();
                //your first place
                ta.From = reta;
                //this move your grid 1000 over from left side
                //you can use -1000 to move to left side
                ta.To = new Thickness(0, contenedorPrincipal.Margin.Top, contenedorPrincipal.Margin.Right, contenedorPrincipal.Margin.Bottom);
                //time the animation playes
                //ta.Duration = new Duration(TimeSpan.FromSeconds(0.1));
                ta.Duration = duration;
                ta.AccelerationRatio = 0.5;
                Storyboard.SetTargetName(ta, contenedorPrincipal.Name);
                Storyboard.SetTargetProperty(ta, new PropertyPath(Control.MarginProperty));

                // Add the animation to the storyboard
                storyboard.Children.Add(ta);
                storyboard.Children.Add(animation);

                // Begin the storyboard
                storyboard.Begin(this);
            }

            private void AnimaContenedorPrincipal_In()
            {
                ThicknessAnimation ta = new ThicknessAnimation();
                //your first place
                ta.From = contenedorPrincipal.Margin;
                //this move your grid 1000 over from left side
                //you can use -1000 to move to left side
                ta.To = new Thickness(0, contenedorPrincipal.Margin.Top, contenedorPrincipal.Margin.Right, contenedorPrincipal.Margin.Bottom);
                //time the animation playes
                ta.Duration = new Duration(TimeSpan.FromSeconds(0.1));
                ta.AccelerationRatio = 0.5;
                //dont need to use story board but if you want pause,stop etc use story board
                contenedorPrincipal.BeginAnimation(Grid.MarginProperty, ta);
            }

            private void AnimaContenedorPrincipal_ReIn()
            {

                Thickness reta = new Thickness(_punto_inicio_contenedor_principal_rein, contenedorPrincipal.Margin.Top, contenedorPrincipal.Margin.Right, contenedorPrincipal.Margin.Bottom);
                ThicknessAnimation ta = new ThicknessAnimation();
                //your first place
                ta.From = reta;
                //this move your grid 1000 over from left side
                //you can use -1000 to move to left side
                ta.To = new Thickness(0, contenedorPrincipal.Margin.Top, contenedorPrincipal.Margin.Right, contenedorPrincipal.Margin.Bottom);
                //time the animation playes
                ta.Duration = new Duration(TimeSpan.FromSeconds(0.1));
                ta.AccelerationRatio = 0.5;
                //dont need to use story board but if you want pause,stop etc use story board
                contenedorPrincipal.BeginAnimation(Grid.MarginProperty, ta);
            }

            private void AnimaContenedorPrincipal_Out()
            {
                TimeSpan duration = new TimeSpan(0, 0, 0, 0, 100);

                ThicknessAnimation ta = new ThicknessAnimation();
                //your first place
                ta.From = contenedorPrincipal.Margin;
                //this move your grid 1000 over from left side
                //you can use -1000 to move to left side
                ta.To = new Thickness(190, contenedorPrincipal.Margin.Top, contenedorPrincipal.Margin.Right, contenedorPrincipal.Margin.Bottom);
                //time the animation playes
                ta.Duration = duration;
                //ta.AccelerationRatio = 0.5;
                //dont need to use story board but if you want pause,stop etc use story board
                contenedorPrincipal.BeginAnimation(Grid.MarginProperty, ta);
            }

            private void AnimaMenuPrincipal_SlideIn(Grid grd)
            {
                TimeSpan duration = new TimeSpan(0, 0, 0, 0, 100);

                ThicknessAnimation ta = new ThicknessAnimation();
                //your first place
                ta.From = grd.Margin;
                //this move your grid 1000 over from left side
                //you can use -1000 to move to left side
                ta.To = new Thickness(0, grd.Margin.Top, grd.Margin.Right, grd.Margin.Bottom);
                //time the animation playes
                ta.Duration = duration;
                ta.AccelerationRatio = 0.5;
                //dont need to use story board but if you want pause,stop etc use story board
                grd.BeginAnimation(Grid.MarginProperty, ta);

                _menu_principal_oculto = false;
            }

            private void AnimaMenuPrincipal_SlideOut(Grid grd)
            {
                if (_menu_principal_oculto) return;

                ThicknessAnimation ta = new ThicknessAnimation();
                //your first place
                ta.From = grd.Margin;
                //this move your grid 1000 over from left side
                //you can use -1000 to move to left side
                ta.To = new Thickness(-(grd.Width - 10), grd.Margin.Top, grd.Margin.Right, grd.Margin.Bottom);
                //time the animation playes
                ta.Duration = new Duration(TimeSpan.FromSeconds(0.3));
                ta.AccelerationRatio = 0.5;
                //dont need to use story board but if you want pause,stop etc use story board
                grd.BeginAnimation(Grid.MarginProperty, ta);

                _menu_principal_oculto = true;
            }

            private void AnimaMenuPrincipal_Hide(Grid grd)
            {
                if (_menu_principal_oculto) return;

                ThicknessAnimation ta = new ThicknessAnimation();
                //your first place
                ta.From = grd.Margin;
                //this move your grid 1000 over from left side
                //you can use -1000 to move to left side
                ta.To = new Thickness(-(grd.Width - 10), grd.Margin.Top, grd.Margin.Right, grd.Margin.Bottom);
                //time the animation playes
                ta.Duration = new Duration(TimeSpan.FromSeconds(0));

                //dont need to use story board but if you want pause,stop etc use story board
                grd.BeginAnimation(Grid.MarginProperty, ta);

                _menu_principal_oculto = true;
            }

            public void BotonMenuPrincipalPulsado_Animaciones()
            {
                // Ocultamos el menu
                //AnimaMenuPrincipal_SlideOut(gridMenuPrincipal);
                AnimaMenuPrincipal_Hide(gridMenuPrincipal);

                contenedorPrincipal.Width = gridPrincipal.ActualWidth;
                //AnimaContenedorPrincipal_SlideAndFadeIn();
                AnimaContenedorPrincipal_FadeIn();
                //AnimaContenedorPrincipal_ReIn();
            }

            #endregion

        }

}
