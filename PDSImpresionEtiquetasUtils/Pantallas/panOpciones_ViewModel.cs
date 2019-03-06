using PDSImpresionEtiquetasUtils.Comun;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDSIEUCo = PDSImpresionEtiquetasUtils.Comun;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    public class panOpciones_ViewModel : panPantallaBase_ViewModel
    {
        Dictionary<string, PDSIEUCo.IPantallasContenedor> _pantallas_abiertas = new Dictionary<string, PDSIEUCo.IPantallasContenedor>();
        public panOpciones_ViewModel()
            : base()
        {

        }

        #region Overrides

        internal override void Inicializa()
        {
            base.Inicializa();

            //RaisePropertyChanged("NombreUsuarioActual");
        }

        #endregion
        public void ActualizaConfiguracionPantallas()
        {
            MainWindow _pantalla_Inicial = new MainWindow();

            if (Utilidades.UtilesCarga._pantallas_abiertas.Any(z => z.Key == _pantalla_Inicial.ToString()))
            {
                _pantalla_Inicial = (MainWindow)Utilidades.UtilesCarga._pantallas_abiertas.FirstOrDefault(z => z.Key == _pantalla_Inicial.ToString()).Value;
            
                foreach (IPantallasContenedor entry in _pantalla_Inicial.ViewModel.ListaPantallasAbiertas)
                {
                    entry.Configuracion = this.Configuracion;
                }
            }
        }

        /*public void TestConnection(string connectionString)
        {
            SqlConnection sqlcon = new SqlConnection();
            string `rpta = "";
            try
            {
                sqlcon.ConnectionString = connectionString;
                sqlcon.Open();
            }

            catch (Exception ex)
            {
                rpta = ex.Message;
            }

            if (sqlcon.State == ConnectionState.Open)
                MessageBox.Show("ok");
            else
                MessageBox.Show(rpta);

        }*/

        #region Properties
        private csConfiguracion _entity = new csConfiguracion();

        public csConfiguracion Entity { get => _entity; set { _entity = value; RaisePropertyChanged("Entity"); }  }
        //public string NombreUsuarioActual
        //{
        //    get
        //    {
        //        if (Configuracion != null) return Configuracion.NombreUsuarioActual;
        //        else return "--";
        //    }
        //}

        #endregion

    }
}

