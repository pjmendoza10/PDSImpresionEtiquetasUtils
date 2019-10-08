using PDSImpresionEtiquetasUtils.Comun;
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
using System.Windows.Shapes;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    /// <summary>
    /// Lógica de interacción para winLogin01.xaml
    /// </summary>
    public partial class winLogin01 : Window
    {
        private winLogin01_ViewModel _viewmodel;

        public winLogin01(csConfiguracion p_configuracion)
        {
            InitializeComponent();

            this._viewmodel = (winLogin01_ViewModel)this.DataContext;
            this._viewmodel.Configuracion = p_configuracion;

            PART_TextBox_Usuario.Focus();
        }

        public winLogin01_ViewModel ViewModel { get { return _viewmodel; } }

        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewmodel.Contrasena = ((PasswordBox)sender).Password;
        }
    }
}
