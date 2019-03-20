﻿using PDSImpresionEtiquetasUtils.Comun;
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
    public partial class panImpresionEtiquetaGen03 : UserControl, IPantallasContenedor
    {
        private panImpresionEtiquetaGen03_ViewModel _viewmodel;
        public panImpresionEtiquetaGen03()
        {
            InitializeComponent();

            PantallaAnterior = null;
            _viewmodel = (Pantallas.panImpresionEtiquetaGen03_ViewModel)this.DataContext;
            _viewmodel.View = View;
            _viewmodel.PantallaPrincipal = csEstadoPermanente.PantallaPrincipal;
            _viewmodel.Configuracion = csEstadoPermanente.Configuracion;

            _viewmodel.Inicializa();
        }
        public bool SetItem(csItem_EtiquetaChinos p_item)
        {
            _viewmodel.Entity.CodArticulo = p_item.CodArticulo;
            _viewmodel.Entity.NumDesde = p_item.NumDesde;
            _viewmodel.Entity.NumHasta = p_item.NumHasta;
            _viewmodel.Entity.NumeroInicial = p_item.NumeroInicial;
            _viewmodel.Entity.SSCC = p_item.SSCC;
            return true;
        }

        #region IPantallasContenedor
        public static string CIdentificador
        {
            get { return "PANTIPOETIQGEN03"; }
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

        #endregion
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) this._viewmodel.BuscarMaterialLote_Command_Execute();
        }
        /* public void OnRendered()
         {
             //this._viewmodel.FiltrarFicheroArticuloLote_Command_Execute();
             this._viewmodel.OnRendered();
         }*/
    }
}