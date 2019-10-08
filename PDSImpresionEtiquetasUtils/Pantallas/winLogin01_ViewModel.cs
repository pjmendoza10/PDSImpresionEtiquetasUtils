using MicroMvvm;
using PDSImpresionEtiquetasUtils.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    public class winLogin01_ViewModel : ObservableObject
    {
        csConfiguracion _configuracion;

        public winLogin01_ViewModel() 
        {
            Accept_Command = new RelayCommand<object>(Accept_Command_Execute, Accept_Command_CanExecute);
            Cancel_Command = new RelayCommand(Cancel_Command_Execute, Cancel_Command_CanExecute);
        }


        #region Properties

        public csConfiguracion Configuracion
        {
            get { return _configuracion; }
            set { _configuracion = value; }
        }

        public string CodUsuario { get; set; }

        public string Contrasena { get; set; }

        public string IDUsuario { get; set; }

        private string _Nota = "";
        public string Nota 
        { 
            get {return _Nota;} 
            set {_Nota = value; RaisePropertyChanged("Nota");}
        }

        #endregion

        #region Commands

        public ICommand Accept_Command { get; set; }

        public bool Accept_Command_CanExecute(object p_objeto)
        {
            return true;
        }

        public void Accept_Command_Execute(object p_objeto)
        {
            if (string.IsNullOrEmpty(CodUsuario)) { return; }
            if (string.IsNullOrEmpty(Contrasena)) { return; }

            string b_IDUsuario;

            Conectores.DBConector_Internal b_conector = new Conectores.DBConector_Internal(this.Configuracion.Datos.connectionString_PDSImpresionEtiquetas);
            int b_res = b_conector.CompruebaCredencialesUsuario(CodUsuario, Contrasena, out b_IDUsuario);
            if (b_res == 0)
            {
                Nota = "Usuario inexistente";
                return;
            } else if (b_res == 2) {
                Nota = "Contraseña incorrecta";
                return;
            }
            IDUsuario = b_IDUsuario;

            ((winLogin01)p_objeto).DialogResult = true;
        }

        public ICommand Cancel_Command { get; set; }

        public bool Cancel_Command_CanExecute()
        {
            return true;
        }

        public void Cancel_Command_Execute() { }

        #endregion
    }
}
