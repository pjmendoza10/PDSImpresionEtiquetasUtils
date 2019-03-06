using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PDSImpresionEtiquetasUtilsInit
{
    public class csConfiguracion
    {
        public ConfiguracionDatosApp Datos;
        public ConfiguracionDatosUser OpcionesAplicacion;

        private static string _nombre_fichero_configuracion_App = "PDSImpresionEtiquetasInit.ini.xml";
        private static string _nombre_fichero_configuracion_User = "PDSImpresionEtiquetasInit.userappsettings.xml";

        public csConfiguracion()
        {
            this.Datos = new ConfiguracionDatosApp();
            this.OpcionesAplicacion = new ConfiguracionDatosUser();

            this.OpcionesAplicacion.UsuarioLogin = "Defecto";
            this.OpcionesAplicacion.UsuarioPassword = "Defecto";

            this.Datos.smtpFromAddress = "-";
            this.Datos.smtpPassword = "-";
            this.Datos.smtpPort = 110;
            this.Datos.smtpServer = "-";
            this.Datos.smtpSSL = true;
            this.Datos.smtpToAddresses = "-";
            this.Datos.smtpUser = "-";

            //<smtpServer>smtp.office365.com</smtpServer>
            //<smtpPort>587</smtpPort>
            //<smtpUser>erp@pdsgroup.es</smtpUser>
            //<smtpPassword>Fogu0384</smtpPassword>
            //<smtpSSL>true</smtpSSL>
            //<smtpFromAddress>erp@pdsgroup.es</smtpFromAddress>
            //<smtpToAddresses>dsistemas@pdsgroup.es;jcfernandez@pdsgroup.es</smtpToAddresses>
        }

        public void Guardar()
        {
            XmlSerializer serializer;

            // config.xml
            //string b_file_conf = System.IO.Path.Combine(csConfiguracion.DamePath_CommonApplicationData, _nombre_fichero_configuracion_App);
            string b_file_conf = System.IO.Path.Combine(csConfiguracion.DamePath_ApplicationDirectory, _nombre_fichero_configuracion_App);

            serializer = new XmlSerializer(typeof(ConfiguracionDatosApp));
            using (TextWriter writer = new StreamWriter(b_file_conf))
            {
                serializer.Serialize(writer, this.Datos);
            }

            // appsettings.xml
            string b_file_appsettings = System.IO.Path.Combine(csConfiguracion.DamePath_UserAppDataPath, _nombre_fichero_configuracion_User);

            serializer = new XmlSerializer(typeof(ConfiguracionDatosUser));
            using (TextWriter writer = new StreamWriter(b_file_appsettings))
            {
                serializer.Serialize(writer, this.OpcionesAplicacion);
            }
        }



        public void Cargar()
        {
            XmlSerializer deserializer;
            object obj;
            bool b_guardar = false;

            //try
            //{

            TextReader reader = null;

            // config.xml
            //string b_file_conf = System.IO.Path.Combine(csConfiguracion.DamePath_ApplicationDirectory, _nombre_fichero_configuracion_App);
            string b_file_conf = System.IO.Path.Combine(csConfiguracion.DamePath_CommonApplicationData, _nombre_fichero_configuracion_App);
            if (File.Exists(b_file_conf))
            {
                deserializer = new XmlSerializer(typeof(ConfiguracionDatosApp));
                reader = new StreamReader(b_file_conf);
                obj = deserializer.Deserialize(reader);
                this.Datos = (ConfiguracionDatosApp)obj;
                reader.Close();
            }
            else
            {
                this.Datos = new ConfiguracionDatosApp();
                b_guardar = true;
            }

            // appsettings.xml
            string b_file_appsettings = System.IO.Path.Combine(csConfiguracion.DamePath_UserAppDataPath, _nombre_fichero_configuracion_User);
            if (File.Exists(b_file_appsettings))
            {
                deserializer = new XmlSerializer(typeof(ConfiguracionDatosUser));
                reader = new StreamReader(b_file_appsettings);
                obj = deserializer.Deserialize(reader);
                this.OpcionesAplicacion = (ConfiguracionDatosUser)obj;
                reader.Close();
            }
            else
            {
                this.OpcionesAplicacion = new ConfiguracionDatosUser();

                this.OpcionesAplicacion.RPS_Usuario = "utilssga";
                this.OpcionesAplicacion.RPS_Password = "utilssga";

                //this.OpcionesAplicacion.MODO_ROL_App = "DEFECTO";
                //this.OpcionesAplicacion.UsuarioLogin = "Defecto";
                //this.OpcionesAplicacion.UsuarioPassword = "Defecto";

                b_guardar = true;
            }

            // Datos por defecto
            if (string.IsNullOrWhiteSpace(this.OpcionesAplicacion.UsuarioLogin)) this.OpcionesAplicacion.UsuarioLogin = "Defecto";
            if (string.IsNullOrWhiteSpace(this.OpcionesAplicacion.UsuarioPassword)) this.OpcionesAplicacion.UsuarioPassword = "Defecto";
            if (string.IsNullOrWhiteSpace(this.OpcionesAplicacion.MODO_ROL_App)) this.OpcionesAplicacion.MODO_ROL_App = "DEFECTO";


            if (b_guardar) Guardar();

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        public void EscribirFicheroVersion()
        {
            //try
            //{
            string b_file_version = System.IO.Path.Combine(csConfiguracion.DamePath_CommonApplicationData, "version_instalador.txt");

            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName assemName = assem.GetName();
            Version ver = assemName.Version;

            File.Delete(b_file_version);

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(b_file_version))
            {
                file.WriteLine("VER|" + ver.ToString());
            }
            //}
            //catch (Exception ex) 
            //{
            //    System.Windows.MessageBox.Show(ex.Message);
            //}
        }

        public static string DamePath_CommonApplicationData
        {
            get
            {
                //Assembly assem = Assembly.GetEntryAssembly();
                //AssemblyName assemName = assem.GetName();
                //Version ver = assemName.Version;

                string b_base = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string b_subdir = "PDSImpresionEtiquetas";
                string b_dir = System.IO.Path.Combine(b_base, b_subdir);

                b_subdir = "PDSImpresionEtiquetas";
                b_dir = System.IO.Path.Combine(b_dir, b_subdir);

                return b_dir;
            }
        }

        public static string DamePath_UserAppDataPath
        {
            get
            {
                string b_base = System.Windows.Forms.Application.LocalUserAppDataPath;
                //string b_subdir = "PDSScdService";
                string b_dir = Directory.GetParent(b_base).ToString();

                //if (csConfiguracion._es_version_desarrollo)
                //{
                //    //Assembly assem = Assembly.GetEntryAssembly();
                //    //AssemblyName assemName = assem.GetName();
                //    //Version ver = assemName.Version;
                //    //string b_Version = ver.Major.ToString() + "." + ver.Minor.ToString();

                //    b_subdir = "ClienteSCADA." + _cod_version_desarrollo;
                //}
                //else
                //{
                //    b_subdir = "ClienteSCADA";
                //}

                //b_dir = System.IO.Path.Combine(b_dir, b_subdir);

                return b_dir;
            }
        }

        //public static string DamePath_TempPathInformes
        //{
        //    get
        //    {
        //        string b_subdir = "SCADATempInformes";
        //        string b_dir = Path.GetTempPath();

        //        b_dir = System.IO.Path.Combine(b_dir, b_subdir);

        //        return b_dir;
        //    }
        //}

        public static string DamePath_ApplicationDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static string DameNombreFicheroConfiguracion
        {
            get
            {
                return _nombre_fichero_configuracion_App;
            }
        }

        public string IDUsuarioActual { get; set; }
        public string CodUsuarioActual { get; set; }

        #region Clases Auxiliares

        public class ConfiguracionDatosApp
        {
            public string Vacio { get; set; }
            //public string connectionString { get; set; }
            //public string MQTTBrokerHostName { get; set; }

            //public string PathVPNProgram { get; set; }

            //public string PathVersionActualizada { get; set; }
            //public string PathBaseDeRecursos { get; set; }

            //public double PeriodoLecturaAcumuladosMaquina { get; set; }

            public string connectionString_PDSImpresionEtiquetas { get; set; }

            public string connectionString_RPS2013_OLANET { get; set; }
            public string connectionString_OLANET_BASE_2013 { get; set; }
            public string connectionString_OLANET_BASE_DATOS_2013 { get; set; }
            public string connectionString_RPS2013 { get; set; }

            public string Ruta_informe_GER01_Palet01 { get; set; }
            public string Ruta_informe_GER01_Bobina01 { get; set; }

            public string Impresora_Palets_GER01 { get; set; }
            public string Impresora_Bobinas_GER01 { get; set; }

            public string PathVersionActualizada { get; set; }

            public bool ActivarAlertaSmtp_ReenvioOFEjecutableAlSGA { get; set; }

            // Email
            public string smtpServer { get; set; }
            public int smtpPort { get; set; }
            public string smtpUser { get; set; }
            public string smtpPassword { get; set; }
            public bool smtpSSL { get; set; }
            public string smtpFromAddress { get; set; }
            public string smtpToAddresses { get; set; }
        }

        public class ConfiguracionDatosUser
        {
            #region RPS

            public string RPS_Usuario { get; set; }
            public string RPS_Password { get; set; }

            #endregion

            public string MODO_ROL_App { get; set; }

            public string UsuarioLogin { get; set; }
            public string UsuarioPassword { get; set; }

            public string PantallaInicial { get; set; }


        }

        #endregion
    }
}
