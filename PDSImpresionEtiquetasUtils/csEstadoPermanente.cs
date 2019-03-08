using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDSIEUCo = PDSImpresionEtiquetasUtils.Comun;
using MicroMvvm;
using System.Xml.Serialization;
using System.IO;
//using PDSImpresionEtiquetas.RPS;

namespace PDSImpresionEtiquetasUtils
{
        public static class csEstadoPermanente
        {
            public static T Deserialize<T>(this string toDeserialize)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (StringReader textReader = new StringReader(toDeserialize))
                {
                    return (T)xmlSerializer.Deserialize(textReader);
                }
            }

            public static string Serialize<T>(this T toSerialize)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, toSerialize);
                    return textWriter.ToString();
                }
            }

            private static MainWindow _PantallaPrincipal = null;
            internal static MainWindow PantallaPrincipal
            {
                get { return _PantallaPrincipal; }
                set { _PantallaPrincipal = value; }
            }

            private static PDSIEUCo.csConfiguracion _configuracion;
            internal static PDSIEUCo.csConfiguracion Configuracion
            {
                get { return _configuracion; }
                set { _configuracion = value; }
            }

            #region RPS

            internal static bool isRPSCargado = false;

            /*private static RPSSession _rpsSession;
            internal static RPSSession RPSSession
            {
                get
                {
                    return _rpsSession;
                }
                set
                {
                    _rpsSession = value;
                    //NotifyStaticPropertyChanged("RPSSession");
                }
            }*/

            #endregion
        }
    }
