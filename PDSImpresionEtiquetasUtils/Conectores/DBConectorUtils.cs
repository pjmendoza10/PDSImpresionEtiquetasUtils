using PDSImpresionEtiquetasUtils.Comun;
using PDSImpresionEtiquetasUtils.Comun.DB;
using PDSImpresionEtiquetasUtils.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDSImpresionEtiquetasUtils.Conectores
{
    public class DBConectorUtils : DataBaseLayer
    {
        private csConfiguracion _configuracion = null;

        //SqlConnection _conexion = null;

        public DBConectorUtils(csConfiguracion p_config)
            : base(p_config.Datos.connectionString_PDSImpresionEtiquetas)
        {
            _configuracion = p_config;


            //_conexion = new SqlConnection(_configuracion.Datos.connectionString);
        }

        //public bool ConexionTest()
        //{
        //    using (SqlConnection b_conexion = new SqlConnection(_configuracion.Datos.connectionString))
        //    {

        //        try
        //        {
        //            if (b_conexion != null)
        //            {
        //                if (b_conexion.State != ConnectionState.Open) b_conexion.Open();
        //            }
        //            else return false;
        //        }
        //        catch (Exception ex)
        //        {
        //            return false;
        //        }

        //        try
        //        {
        //            if (b_conexion != null)
        //            {
        //                b_conexion.Close();
        //            }
        //        }
        //        catch { }
        //    }

        //    return true;
        //}

        //public bool Conectar(SqlConnection p_conexion) 
        //{
        //    try 
        //    {
        //        if (p_conexion != null) p_conexion.Open();
        //        else return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public bool Desconectar(SqlConnection p_conexion)
        //{
        //    try
        //    {
        //        if (p_conexion != null) p_conexion.Close();
        //        else return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //#region "Acciones"

        //public DataTable MyExecuteQuery(string p_mi_sql)
        //{
        //    DataTable b_dt = null;

        //    using (SqlConnection b_conexion = new SqlConnection(_configuracion.Datos.connectionString))
        //    {

        //        //Conectar(b_conexion);
        //        using (SqlCommand cmdSel = new SqlCommand(p_mi_sql, b_conexion))
        //        {
        //            SqlDataAdapter da = new SqlDataAdapter(cmdSel);
        //            b_dt = new DataTable();
        //            da.Fill(b_dt);
        //            da.Dispose();
        //        }
        //        //Desconectar(b_conexion);
        //    }

        //    return b_dt;
        //}

        //public DataTable MyExecuteQueryCommand(SqlCommand p_mi_command)
        //{
        //    DataTable b_dt = null;

        //    using (SqlConnection b_conexion = new SqlConnection(_configuracion.Datos.connectionString))
        //    {

        //        //Conectar(b_conexion);

        //        p_mi_command.Connection = b_conexion;
        //        SqlDataAdapter da = new SqlDataAdapter(p_mi_command);
        //        b_dt = new DataTable();
        //        da.Fill(b_dt);
        //        da.Dispose();

        //        //Desconectar(b_conexion);
        //    }

        //    return b_dt;
        //}

        //public bool MyExecuteNonQuery(string p_mi_sql)
        //{

        //    int filas;

        //    using (SqlConnection b_conexion = new SqlConnection(_configuracion.Datos.connectionString))
        //    {

        //        //Conectar(b_conexion);
        //        using (SqlCommand cmdSel = new SqlCommand(p_mi_sql, b_conexion))
        //        {
        //            filas = cmdSel.ExecuteNonQuery();
        //        }
        //        //Desconectar(b_conexion);
        //    }

        //    return (filas > 0);
        //}

        //public bool MyExecuteNonQueryCommand(SqlCommand p_mi_command)
        //{

        //    int filas;

        //    using (SqlConnection b_conexion = new SqlConnection(_configuracion.Datos.connectionString))
        //    {

        //        //Conectar(b_conexion);

        //        p_mi_command.Connection = b_conexion;
        //        filas = p_mi_command.ExecuteNonQuery();

        //        //Desconectar(b_conexion);
        //    }

        //    return (filas > 0);
        //}

        //#endregion

        
       /*#region Login

        public int CompruebaCredencialesUsuario(string p_cod_usuario, string p_contrasena, out string p_id_usuario)
        {
            p_id_usuario = "";

            PDSScdService.DataBaseLayerComun.DataBaseLayer b_db = new PDSScdService.DataBaseLayerComun.DataBaseLayer(_configuracion.Datos.connectionString);

            if (b_db != null)
            {
                DB_Usuario b_user = b_db.Usuario_GetObj(p_cod_usuario);

                if (b_user == null)
                {
                    // Usuario no encontrado
                    return 0;
                }
                else
                {
                    if (b_user.Contrasena == p_contrasena)
                    {
                        // Usuario logeado con exito
                        p_id_usuario = b_user.IDUsuario;
                        return 1;
                    }
                    else
                    {
                        // Contraseña incorrecta
                        return 0;
                    }
                }
            }
            else
            {
                // Comprobar si es administrador local
                return 0;
            }
        }

        #endregion
        
        #region Permisos Usuarios


         public void CreaEstructuraPermisosNuevoUsuario(string p_IDUsuario)
           {
               DataBaseLayer b_db = new DataBaseLayer(_configuracion.Datos.connectionString_PDSImpresionEtiquetas);

               // Secciones de App
               List<DB_SeccionesApp> b_lista_secciones = b_db.SeccionesApp_GetListObj();

               foreach (DB_SeccionesApp i_seccion in b_lista_secciones)
               {
                   DB_Usuarios_PermisosSeccionesApp b_permiso = new DB_Usuarios_PermisosSeccionesApp();

                   b_permiso.IDUsuario = p_IDUsuario;
                   b_permiso.CodSeccionApp = i_seccion.CodSeccionApp;

                   b_permiso.Valor = false;

                   b_db.Usuarios_PermisosSeccionesApp_Insert(b_permiso);
               }

               // Maquinas
               List<DB_Maquina> b_lista_maquinas = b_db.Maquina_GetListObj();

               foreach (DB_Maquina i_maq in b_lista_maquinas)
               {
                   DB_Usuarios_PermisosMaquinas b_permiso = new DB_Usuarios_PermisosMaquinas();

                   b_permiso.IDUsuario = p_IDUsuario;
                   b_permiso.CodMaquina = i_maq.CodMaquina;

                   b_permiso.Valor = false;

                   b_db.Usuarios_PermisosMaquinas_Insert(b_permiso);
               }

           }

           public void BorraEstructuraPermisosUsuario(string p_IDUsuario)
           {
               PDSScdService.DataBaseLayerComun.DataBaseLayer b_db = new PDSScdService.DataBaseLayerComun.DataBaseLayer(_configuracion.Datos.connectionString);

               // Secciones de App
               b_db.Usuarios_PermisosSeccionesApp_Delete(p_IDUsuario);

               // Maquinas
               b_db.Usuarios_PermisosMaquinas_DeleteByUser(p_IDUsuario);
           }

           #endregion

           */
    }
    #region Clases Auxiliares

    public static class SqlCommandExt
    {
        /// <summary>
        /// This will add an array of parameters to a SqlCommand. This is used for an IN statement.
        /// Use the returned value for the IN part of your SQL call. (i.e. SELECT * FROM table WHERE field IN ({paramNameRoot}))
        /// </summary>
        /// <param name="cmd">The SqlCommand object to add parameters to.</param>
        /// <param name="values">The array of strings that need to be added as parameters.</param>
        /// <param name="paramNameRoot">What the parameter should be named followed by a unique value for each value. This value surrounded by {} in the CommandText will be replaced.</param>
        /// <param name="start">The beginning number to append to the end of paramNameRoot for each value.</param>
        /// <param name="separator">The string that separates the parameter names in the sql command.</param>
        public static SqlParameter[] AddArrayParameters<T>(this SqlCommand cmd, IEnumerable<T> values, string paramNameRoot, int start = 1, string separator = ", ")
        {
            /* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually. 
             * Each item in the array will end up being it's own SqlParameter so the return value for this must be used as part of the
             * IN statement in the CommandText.
             */
            var parameters = new List<SqlParameter>();
            var parameterNames = new List<string>();
            var paramNbr = start;
            foreach (var value in values)
            {
                var paramName = string.Format("@{0}{1}", paramNameRoot, paramNbr++);
                parameterNames.Add(paramName);
                parameters.Add(cmd.Parameters.AddWithValue(paramName, value));
            }

            cmd.CommandText = cmd.CommandText.Replace("{" + paramNameRoot + "}", string.Join(separator, parameterNames));

            return parameters.ToArray();
        }
    }

    #endregion

}