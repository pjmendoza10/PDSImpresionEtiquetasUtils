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
    public partial class panImpresionEtiquetaSIRO : UserControl, IPantallasContenedor
    {
        private panImpresionEtiquetaSIRO_ViewModel _viewmodel;
        public panImpresionEtiquetaSIRO()
        {
            InitializeComponent();

            PantallaAnterior = null;
            _viewmodel = (Pantallas.panImpresionEtiquetaSIRO_ViewModel)this.DataContext;
            _viewmodel.View = View;
            _viewmodel.PantallaPrincipal = csEstadoPermanente.PantallaPrincipal;
            _viewmodel.Configuracion = csEstadoPermanente.Configuracion;

            _viewmodel.Inicializa();
        }

        #region IPantallasContenedor
        public static string CIdentificador
        {
            get { return "PANTIPOETIQSIRO"; }
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

        private void DatePicker_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.H)
            {
                this._viewmodel.Entity.FechaEntrega = DateTime.Now.ToString("dd/MM/yyyy");
            } else if (e.Key == Key.M)
            {
                this._viewmodel.Entity.FechaEntrega = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            } else if (e.Key == Key.P)
            {
                this._viewmodel.Entity.FechaEntrega = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            }
            else if (e.Key == Key.A)
            {
                this._viewmodel.Entity.FechaEntrega = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            }
            else { return; }
        }

        private void DatePicker_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.H)
            {
                this._viewmodel.Entity.FechaFabricacion = DateTime.Now.ToString("dd/MM/yyyy");
            } else if (e.Key == Key.M)
            {
                this._viewmodel.Entity.FechaFabricacion = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            } else if (e.Key == Key.P)
            {
                this._viewmodel.Entity.FechaFabricacion = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            } else if (e.Key == Key.A)
            {
                this._viewmodel.Entity.FechaFabricacion = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            } else { return; }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) this._viewmodel.BuscarDatos_Command_Execute();
        }

        private void TextBox_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) this._viewmodel.BuscarDatos_Command_Execute();
        }
        #endregion

        /* public void OnRendered()
         {
             //this._viewmodel.FiltrarFicheroArticuloLote_Command_Execute();
             this._viewmodel.OnRendered();
         }*/
    }
}