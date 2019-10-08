using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace PDSImpresionEtiquetasUtils.Utilidades
{
    #region Errores

    public class csErrores
    {
        public csErrores()
        {
            Errores = new List<string>();
        }

        public List<string> Errores { get; set; }
    }

    #endregion

    #region Resultados

    public class csOK_Error
    {
        public csOK_Error()
        {
            Errores = new csErrores();
        }

        public string Identificador { get; set; }
        public bool Correcto { get; set; }
        public csErrores Errores { get; set; }

        public void AddError(string p_error)
        {
            Errores.Errores.Add(p_error);
        }
    }

    #endregion

    #region Cadenas

    public class csCadenasUtils
    {
        public static string DameCadenaFromListWithNewLines(List<string> p_lista)
        {
            string b_resultado = string.Join(Environment.NewLine, p_lista);

            return b_resultado;
        }

    }
    #endregion

    #region Directorios

    public class CommonApplicationData
    {
        // https://geeks.ms/omarvr/2010/03/29/commonapplicationdata-y-sus-permisos/
        public static void CreateFolder(string folderName, bool allUsers)
        {
            if (Directory.Exists(folderName)) return;


            var m_securityIdentifier =
                new SecurityIdentifier(WellKnownSidType.WorldSid, null);


            var m_directoryInfo = Directory.CreateDirectory(folderName);

            if (!allUsers) return;


            bool m_modified;
            var m_directorySecurity = m_directoryInfo.GetAccessControl();

            AccessRule m_rule =
               new FileSystemAccessRule(m_securityIdentifier,
                           FileSystemRights.FullControl,
                           InheritanceFlags.ContainerInherit |
                           InheritanceFlags.ObjectInherit,
                           PropagationFlags.InheritOnly,
                           AccessControlType.Allow);


            m_directorySecurity.
                    ModifyAccessRule(AccessControlModification.Add,
                                      m_rule, out m_modified);

            m_directoryInfo.SetAccessControl(m_directorySecurity);
        }

    }

    #endregion
 
}
