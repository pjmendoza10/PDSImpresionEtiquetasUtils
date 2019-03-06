using PDSImpresionEtiquetasUtils.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    /// <summary>
    /// Lógica de interacción para panImpresionEtiqueta.xaml
    /// </summary>
    public partial class panImpresionEtiquetaESTIU : UserControl, IPantallasContenedor
    {
        private panImpresionEtiquetaESTIU_ViewModel _viewmodel;
        public panImpresionEtiquetaESTIU()
        {
            InitializeComponent();

            PantallaAnterior = null;
            _viewmodel = (Pantallas.panImpresionEtiquetaESTIU_ViewModel)this.DataContext;
            _viewmodel.View = View;
            _viewmodel.PantallaPrincipal = csEstadoPermanente.PantallaPrincipal;
            _viewmodel.Configuracion = csEstadoPermanente.Configuracion;

            _viewmodel.Inicializa();
        }

        #region IPantallasContenedor
        public static string CIdentificador
        {
            get { return "PANTIPOETIQESTIU"; }
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

        private csConfiguracion _configuracion;
        public csConfiguracion Configuracion
        {
            get { return _configuracion; }
            set { _configuracion = value; }
        }

        Guid _id_unico = Guid.NewGuid();
        public Guid IDUnico
        {
            get { return _id_unico; }
        }
        public IPantallasContenedor PantallaAnterior { get; set; }

        public delegate void SetCursor_Callback(string p_cursor_name);
        public void SetCursor(string p_cursor_name)
        {
            MainWindow b_vista = (MainWindow)csEstadoPermanente.PantallaPrincipal;

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
                    this.Cursor = Utilidades.VariosComunesWindows.GetCursorByName(p_cursor_name);
                }
                finally
                {
                    if (d != null) d.Dispose();
                }
            }
        }

        public object Window { get; set; }

        private void cmdHome_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                csEstadoPermanente.PantallaPrincipal.CambiarAPantallaHome();
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) this._viewmodel.BuscarDatos_Command_Execute();
        }
        
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_viewmodel.Entity.NumPalet.Count()==20) this._viewmodel.BuscarDatos_Command_Execute();
        }

        private void PART_Grid_ListaLineasGridEtiqueta_LostFocus(object sender, RoutedEventArgs e)
        {
            _viewmodel.ActualizarDatosTotales();
        }


        #endregion

        /* public void OnRendered()
         {
             //this._viewmodel.FiltrarFicheroArticuloLote_Command_Execute();
             this._viewmodel.OnRendered();
         }*/
    }
}