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
        public bool SetItem(csitem_EtiquetaEstiuT1 p_item)
        {
            _viewmodel.Entity.AlbaranPDS = p_item.AlbaranPDS;
            _viewmodel.Entity.AnchoBobina = p_item.AnchoBobina;
            _viewmodel.Entity.Cliente = p_item.Cliente;
            _viewmodel.Entity.CodArticulo = p_item.CodArticulo;
            _viewmodel.Entity.CodCliente = p_item.CodCliente;
            _viewmodel.Entity.Descripcion = p_item.Descripcion;
            _viewmodel.Entity.ListaLineasGridEtiqueta = p_item.ListaLineasGridEtiqueta;
            _viewmodel.Entity.ListaLineasGridEtiqueta_SelectedItem = p_item.ListaLineasGridEtiqueta_SelectedItem;
            _viewmodel.Entity.Lote = p_item.Lote;
            _viewmodel.Entity.MetrosBobina = p_item.MetrosBobina;
            _viewmodel.Entity.NumBobinas = p_item.NumBobinas;
            _viewmodel.Entity.NumPalet = p_item.NumPalet;
            _viewmodel.Entity.PedidoCliente = p_item.PedidoCliente;
            _viewmodel.Entity.PesoBrutoPaletizado = p_item.PesoBrutoPaletizado;
            _viewmodel.Entity.PesoMandril = p_item.PesoMandril;
            _viewmodel.Entity.PesoNetoPaletizado = p_item.PesoNetoPaletizado;
            _viewmodel.Entity.PesoNetoSMandril = p_item.PesoNetoSMandril;
            _viewmodel.Entity.PesoPalet = p_item.PesoPalet;
            _viewmodel.Entity.PesoPlastico = p_item.PesoPlastico;
            _viewmodel.Entity.RefCliente = p_item.RefCliente;
            _viewmodel.Entity.TotalMetros = p_item.TotalMetros;
            //_viewmodel.Entity.UIDEtiqueta = p_item.UIDEtiqueta;
            return true;
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
            if (_viewmodel.Entity.NumPalet == null) return;
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