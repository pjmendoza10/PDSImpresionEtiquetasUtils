using PDSImpresionEtiquetasUtils.Comun;
using PDSImpresionEtiquetasUtils.Comun.DB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    /// <summary>
    /// Lógica de interacción para panImpresionEtiquetaGen01.xaml
    /// </summary>
    public partial class panReImpresionEtiquetaGen01 : UserControl, IPantallasContenedor
    {
        private panReImpresionEtiquetaGen01_ViewModel _viewmodel;

        public panReImpresionEtiquetaGen01()
        {
            InitializeComponent();

            PantallaAnterior = null;
            _viewmodel = (Pantallas.panReImpresionEtiquetaGen01_ViewModel)this.DataContext;
            _viewmodel.View = View;
            _viewmodel.PantallaPrincipal = csEstadoPermanente.PantallaPrincipal;
            _viewmodel.Configuracion = csEstadoPermanente.Configuracion;

            _viewmodel.Inicializa();

            CultureInfo ci = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            ci.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = ci;

        }
        /*public bool SetItem(csitem_EtiquetaGeneralPaletT1 p_item)
        {
            _viewmodel.Entity.Ean13 = p_item.Ean13;
            _viewmodel.Entity.Fecha = p_item.Fecha;
            _viewmodel.IN_CodArticulo = p_item.CodArticulo;
            _viewmodel.Entity.Descripcion = p_item.Descripcion;
            _viewmodel.Entity.ListaLineasGridEtiqueta = p_item.ListaLineasGridEtiqueta;
            _viewmodel.Entity.ListaLineasGridEtiqueta_SelectedItem = p_item.ListaLineasGridEtiqueta_SelectedItem;
            _viewmodel.IN_CodLote = p_item.Lote;
            _viewmodel.Entity.Sscc = p_item.Sscc;
            _viewmodel.Entity.TotalCajas = p_item.TotalCajas;
            _viewmodel.Entity.TotalKgAprox = p_item.TotalKgAprox;
            _viewmodel.Entity.TotalUds = p_item.TotalUds;
            
            return true;
        }*/

        #region IPantallasContenedor
        public static string CIdentificador
        {
            get { return "PANTIPOETIQ01"; }
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

        private void txtCodArticulo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter & (sender as TextBox).AcceptsReturn == false)
            {
                //_viewmodel.txtCodOFplusCodTarea_LostF();
                if (e.Key == Key.Enter) this._viewmodel.BuscarEtiquetas_Command_Execute();
                txtLoteArticulo.Focus();
            }
        }

        #endregion

        #region Private Methods

        private void NumberIntegerValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cmdOF_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(!String.IsNullOrWhiteSpace(_viewmodel.IN_OFDesde))_viewmodel.IN_OFHasta = _viewmodel.IN_OFDesde;
        }

        private void CmdFechas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(_viewmodel.IN_FechaDesde)) _viewmodel.IN_FechaHasta = _viewmodel.IN_FechaDesde;
        }

        private void CmdMaquina_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _viewmodel.ActualizarListaMQs();
        }

        private void PART_Grid_ListaLineasGridBobina_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewmodel.ListaLineasGridBobina_SelectedItems.Count() > 0) _viewmodel.ListaLineasGridBobina_SelectedItems.Clear();
            foreach (DB_pds_progutils_Listado_Etiquetas_GEN01 item in PART_Grid_ListaLineasGridBobina.SelectedItems)
            {
                _viewmodel.ListaLineasGridBobina_SelectedItems.Add(item);
            }
        }

        #endregion
        /* public void OnRendered()
         {
             //this._viewmodel.FiltrarFicheroArticuloLote_Command_Execute();
             this._viewmodel.OnRendered();
         }*/
    }

}