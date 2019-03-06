using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDSImpresionEtiquetasUtils.Comun.Utilidades
{
    public class csLogUtils
    {
        public static void EscribeLineaLog(string p_linea)
        {
            StreamWriter b_stream_log = null;

            try
            {
                //string b_base = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                //string b_subdir = "CTAIMAToWinPak"; // your code goes here
                //string b_dir = System.IO.Path.Combine(b_base, b_subdir);

                string b_dir = csConfiguracion.DamePath_CommonApplicationData;
                string b_machine_name = System.Environment.MachineName;
                b_machine_name = b_machine_name.Replace(' ', '_');

                string b_filename = "Log-" + b_machine_name + "-" + DateTime.Today.ToString("yyyyMM") + ".txt";

                string b_file_log = System.IO.Path.Combine(b_dir, b_filename);

                b_stream_log = new StreamWriter(new FileStream(b_file_log, FileMode.Append, FileAccess.Write));


                if (b_stream_log == null) return;
                b_stream_log.WriteLine(DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " -> " + p_linea);
                b_stream_log.Flush();
            }
            catch (Exception ex)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
            }
            finally
            {
                if (b_stream_log != null) b_stream_log.Close();
            }
        }

        public static void EscribeLineaLogError(string p_error)
        {
            string b_linea = "ERROR: " + p_error;
            csLogUtils.EscribeLineaLog(b_linea);
        }

        public static void EscribeLineaLogError(string p_origen, string p_error)
        {
            string b_linea = "ERROR: " + p_origen + ", " + p_error;
            csLogUtils.EscribeLineaLog(b_linea);
        }

        public static void EscribeLineaLogError(string p_origen, Exception p_ex)
        {
            string b_linea = "ERROR: " + p_origen + ", " + p_ex.Message;
            if (p_ex.InnerException != null)
            {
                b_linea += " // " + p_ex.InnerException.Message;
            }
            csLogUtils.EscribeLineaLog(b_linea);
        }

        public static void EscribeLineaLogError(Exception p_ex)
        {
            string b_linea = "ERROR: " + p_ex.Message;
            if (p_ex.InnerException != null)
            {
                b_linea += " // " + p_ex.InnerException.Message;
            }
            csLogUtils.EscribeLineaLog(b_linea);
        }
    }
}
