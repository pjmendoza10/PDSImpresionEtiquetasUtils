using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMvvm;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace PDSImpresionEtiquetasUtilsInit
{
    public class MainWindow_ViewModel : ObservableObject
    {
        Version _enVersion;

        BackgroundWorker _wk_actualizar = new BackgroundWorker();

        public MainWindow_ViewModel()
        {
            _wk_actualizar.WorkerReportsProgress = true;
            _wk_actualizar.WorkerSupportsCancellation = true;
            _wk_actualizar.DoWork += _wk_actualizar_DoWork;
            _wk_actualizar.ProgressChanged += _wk_actualizar_ProgressChanged;
            _wk_actualizar.RunWorkerCompleted += _wk_actualizar_RunWorkerCompleted;

            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName assemName = assem.GetName();
            _enVersion = assemName.Version;
            _Version = _enVersion.ToString();
        }


        #region Propiedades

        private MainWindow _PantallaPrincipal;
        public MainWindow PantallaPrincipal
        {
            get { return _PantallaPrincipal; }
            set { _PantallaPrincipal = value; }
        }

        private csConfiguracion _configuracion = null;
        public csConfiguracion Configuracion
        {
            get { return _configuracion; }
            set { _configuracion = value; }
        }

        private string _Version = "Version";
        public string Version
        {
            get { return _Version; }
        }


        int _MaximoProgressBar = 1;
        public int MaximoProgressBar
        {
            get { return _MaximoProgressBar; }
            set
            {
                _MaximoProgressBar = value;
                RaisePropertyChanged("MaximoProgressBar");
            }
        }

        int _ValorProgressBar = 0;
        public int ValorProgressBar
        {
            get { return _ValorProgressBar; }
            set
            {
                _ValorProgressBar = value;
                RaisePropertyChanged("MaximoProgressBar");
                LabelProgreso = "Fichero: " + value + " de " + _MaximoProgressBar.ToString();
                PantallaPrincipal.Progreso(value);
            }
        }

        string _LabelProgreso = "Progreso";
        public string LabelProgreso
        {
            get { return _LabelProgreso; }
            set { _LabelProgreso = value; RaisePropertyChanged("LabelProgreso"); }
        }

        #endregion

        #region Metodos

        public void Inicializa()
        {
            // Grabamos el fichero logs
            string b_path = System.IO.Path.Combine(_configuracion.Datos.PathVersionActualizada, "Logs");
            string b_linea = Environment.MachineName + " | " + "PDSImpresionEtiquetasInit - Inicio del Actualizador (MainWindow_ViewModel). Version: " + _enVersion.ToString();
            csLogUtil01.EscribeLineaLog(b_path, b_linea);
        }

        public void EjecutarActualizacionInstalador()
        {
            try
            {

                // Grabamos el fichero logs
                string b_path = System.IO.Path.Combine(_configuracion.Datos.PathVersionActualizada, "Logs");
                string b_linea = Environment.MachineName + " | " + "PDSImpresionEtiquetasInit - EjecutarActualizacionInstalador";
                csLogUtil01.EscribeLineaLog(b_path, b_linea);

                string b_fichero_origen;
                string b_fichero_destino = _PantallaPrincipal._nombre_ejecutable_nuevo_instalador;

                b_fichero_origen = System.IO.Path.Combine(_configuracion.Datos.PathVersionActualizada, _PantallaPrincipal._nombre_ejecutable_normal_instalador);

                System.IO.File.Copy(b_fichero_origen, b_fichero_destino, true);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public void EjecutarActualizacionProgramaPrincipal()
        {
            if (_wk_actualizar.IsBusy != true)
            {
                // Grabamos el fichero logs
                string b_path = System.IO.Path.Combine(_configuracion.Datos.PathVersionActualizada, "Logs");
                string b_linea = Environment.MachineName + " | " + "PDSImpresionEtiquetasInit - EjecutarActualizacionProgramaPrincipal";
                csLogUtil01.EscribeLineaLog(b_path, b_linea);

                _wk_actualizar.RunWorkerAsync();
            }
        }

        public bool ComprobarActualizacionProgramaPrincipal()
        {
            try
            {

                if (_configuracion != null)
                {
                    string line;
                    string[] b_d_line;
                    Version b_version_local = new Version();
                    Version b_version_remota = new Version();

                    string b_file_version_local = System.IO.Path.Combine(csConfiguracion.DamePath_CommonApplicationData, "version.txt");

                    // Leemos la version local
                    if (File.Exists(b_file_version_local))
                    {
                        System.IO.StreamReader file = new System.IO.StreamReader(b_file_version_local);
                        while ((line = file.ReadLine()) != null)
                        {
                            b_d_line = line.Split('|');
                            switch (b_d_line[0])
                            {
                                case "VER":
                                    b_version_local = new Version(b_d_line[1]);
                                    break;
                            }
                        }
                        file.Close();
                    }

                    // Leemos el manifiesto del repositorio

                    string b_file_version_rep = System.IO.Path.Combine(_configuracion.Datos.PathVersionActualizada, "version.txt");

                    if (File.Exists(b_file_version_rep))
                    {
                        System.IO.StreamReader file = new System.IO.StreamReader(b_file_version_rep);
                        while ((line = file.ReadLine()) != null)
                        {
                            b_d_line = line.Split('|');
                            switch (b_d_line[0])
                            {
                                case "VER":
                                    b_version_remota = new Version(b_d_line[1]);
                                    break;
                            }
                        }
                        file.Close();
                    }

                    // Comparamos las versiones
                    if (b_version_local.CompareTo(b_version_remota) < 0)
                    {
                        // Hay que actualizar 
                        return true;
                    }
                }
            }
            catch (Exception ex) { }

            return false;
        }

        public bool ComprobarActualizacionInstalador()
        {
            try
            {

                if (_configuracion != null)
                {
                    string line;
                    string[] b_d_line;
                    Version b_version_local = _enVersion;
                    Version b_version_remota = new Version();

                    // Leemos el manifiesto del repositorio
                    string b_file_version_rep = System.IO.Path.Combine(_configuracion.Datos.PathVersionActualizada, "version_instalador.txt");

                    if (File.Exists(b_file_version_rep))
                    {
                        System.IO.StreamReader file = new System.IO.StreamReader(b_file_version_rep);
                        while ((line = file.ReadLine()) != null)
                        {
                            b_d_line = line.Split('|');
                            switch (b_d_line[0])
                            {
                                case "VER":
                                    b_version_remota = new Version(b_d_line[1]);
                                    break;
                            }
                        }
                        file.Close();
                    }

                    // Comparamos las versiones
                    if (b_version_local.CompareTo(b_version_remota) < 0)
                    {
                        // Hay que actualizar 
                        return true;
                    }
                }
            }
            catch (Exception ex) { }

            return false;
        }

        #endregion

        #region  BackgroundWorker

        void _wk_actualizar_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Diagnostics.Process.Start("PDSImpresionEtiquetasUtils.exe");
            PantallaPrincipal.Close();
        }

        void _wk_actualizar_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ValorProgressBar = e.ProgressPercentage;
            //pgbPrincipal.Value = e.ProgressPercentage;
        }

        void _wk_actualizar_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;


            // Perform a time consuming operation and report progress.
            //System.Threading.Thread.Sleep(500);
            //worker.ReportProgress((i * 10));
            int b_cuenta = 0;
            string b_fichero_origen;
            string b_fichero_destino;
            string[] files = System.IO.Directory.GetFiles(_configuracion.Datos.PathVersionActualizada);

            // Copy the files and overwrite destination files if they already exist.
            ValorProgressBar = 0;
            MaximoProgressBar = files.Count();
            //pgbPrincipal.Value = 0;
            //pgbPrincipal.Maximum = files.Count();

            try
            {

                foreach (string s in files)
                {
                    if (b_cuenta == 43)
                    {

                    }
                    if ((worker.CancellationPending == true))
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        // Use static Path methods to extract only the file name from the path.
                        b_fichero_origen = System.IO.Path.GetFileName(s);
                        if ((b_fichero_origen.ToUpper() != "PDSIMPRESIONETIQUETASUTILSINIT.EXE") && ((b_fichero_origen.ToUpper() != "PDSIMPRESIONETIQUETASUTILSINIT.NEW.EXE")))
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(s)))
                            {
                                // Creamos el directorio
                            }
                            System.IO.File.Copy(s, b_fichero_origen, true);
                        }

                        b_cuenta++;
                        System.Threading.Thread.Sleep(100);
                        worker.ReportProgress(b_cuenta, null);
                    }
                }

                // Fichero de configuracion
                b_fichero_origen = Path.Combine(_configuracion.Datos.PathVersionActualizada, "FicheroINI", csConfiguracion.DameNombreFicheroConfiguracion);
                b_fichero_destino = Path.Combine(csConfiguracion.DamePath_CommonApplicationData, csConfiguracion.DameNombreFicheroConfiguracion);
                if (File.Exists(b_fichero_origen))
                {
                    File.Copy(b_fichero_origen, b_fichero_destino, true);
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
