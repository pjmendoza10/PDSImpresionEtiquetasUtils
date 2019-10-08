using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using PDS.DataBaseLayerBasico.SQL;
using PDSIEUCoBD = PDSImpresionEtiquetasUtils.Comun.DB;

namespace PDSImpresionEtiquetasUtils.Conectores
{
        public class DBConector_Internal : DataBaseLayerBasicoSQL
        {
            public DBConector_Internal()
                : base(csEstadoPermanente.Configuracion.Datos.connectionString_RPS2013)
            {

            }

            public DBConector_Internal(string p_connection_string)
                : base(p_connection_string)
            {

            }

            #region Basico

            #region Usuarios
            private PDSIEUCoBD.DBPermisos_Usuario Permisos_Usuario_GetObjByDataRow(DataRow p_row)
            {
                PDSIEUCoBD.DBPermisos_Usuario b_item = new PDSIEUCoBD.DBPermisos_Usuario();

                b_item.IDUsuario = p_row["IDUsuario"].ToString();

                b_item.IDPantalla = p_row["IDPantalla"].ToString();
                b_item.CodPantalla = p_row["CodPantalla"].ToString();
                b_item.Permiso = (int)p_row["Permiso"];

            return b_item;
            }
            public List<PDSIEUCoBD.DBPermisos_Usuario> Permisos_Usuario_GetListObj(string usuario)
            {
                List<PDSIEUCoBD.DBPermisos_Usuario> b_resultado = new List<PDSIEUCoBD.DBPermisos_Usuario>();

                string _mi_sql = string.Empty;

                _mi_sql = "select * from TABLA_PERMISOS_USUARIO PER INNER JOIN USUARIOS USU ON PER.IDUsuario = USU.IDUsuario WHERE USU.CodUsuario = '" + usuario + "'";
    
                DataTable b_dt = MyExecuteQuery(_mi_sql);

                foreach (DataRow i_dr in b_dt.Rows)
                {
                    try
                    {
                        PDSIEUCoBD.DBPermisos_Usuario b_item = Permisos_Usuario_GetObjByDataRow(i_dr);

                        b_resultado.Add(b_item);
                    }
                    catch (Exception ex)
                    {
                        Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
                    }
                }

                return b_resultado;
            }

            private PDSIEUCoBD.DBInt_Usuario Usuario_GetObjByDataRow(DataRow p_row)
            {
                PDSIEUCoBD.DBInt_Usuario b_item = new PDSIEUCoBD.DBInt_Usuario();

                b_item.IDUsuario = p_row["IDUsuario"].ToString();

                b_item.CodUsuario = p_row["CodUsuario"].ToString();
                b_item.Contrasena = p_row["Contrasena"].ToString();
                b_item.Nombre = p_row["Nombre"].ToString();
                b_item.EsAdmin = ((int)p_row["EsAdmin"]) == 1;

                return b_item;
            }

            public List<PDSIEUCoBD.DBInt_Usuario> Usuario_GetListObj()
            {

                List<PDSIEUCoBD.DBInt_Usuario> b_resultado = new List<PDSIEUCoBD.DBInt_Usuario>();

                string _mi_sql = string.Empty;

                _mi_sql = "select * from Usuarios";

                DataTable b_dt = MyExecuteQuery(_mi_sql);

                foreach (DataRow i_dr in b_dt.Rows)
                {
                    try
                    {
                    PDSIEUCoBD.DBInt_Usuario b_item = Usuario_GetObjByDataRow(i_dr);

                        b_resultado.Add(b_item);
                    }
                    catch (Exception ex)
                {
                    Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
                }
                }

                return b_resultado;
            }

            public bool Usuario_Existe(string p_cod_usuario)
            {
                string _mi_sql = "select count(*) from Usuarios where CodUsuario = '" + p_cod_usuario + "'";

                int filas;

                using (SqlConnection b_conexion = new SqlConnection(base.ConnectionString))
                {
                    using (SqlCommand cmdSel = new SqlCommand(_mi_sql, b_conexion))
                    {
                        try
                        {
                            b_conexion.Open();
                            filas = Convert.ToInt32(cmdSel.ExecuteScalar());
                        }
                        catch (Exception e)
                    {
                        Comun.Utilidades.csLogUtils.EscribeLineaLogError(e);
                        filas = 0;
                        }
                    }
                    b_conexion.Close();
                }

                return (filas > 0);
            }

            public PDSIEUCoBD.DBInt_Usuario Usuario_GetObj(string p_cod_usuario)
            {
            PDSIEUCoBD.DBInt_Usuario b_resultado = null;

                string _mi_sql = string.Empty;

                _mi_sql = "select * from Usuarios where CodUsuario = '" + p_cod_usuario.ToString() + "' ";

                DataTable b_dt = MyExecuteQuery(_mi_sql);

                try
                {
                    b_resultado = Usuario_GetObjByDataRow(b_dt.Rows[0]);
                }
                catch (Exception ex)
                {
                    Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex); return null;
                }


                return b_resultado;
            }

            public bool Usuario_Insert(PDSIEUCoBD.DBInt_Usuario p_item)
            {
                Guid b_IDUsuario;

                if (!Guid.TryParse(p_item.IDUsuario, out b_IDUsuario)) return false;

                using (var command = new SqlCommand(
                        @"INSERT INTO Usuarios 
                    ([IDUsuario],
                    [CodUsuario], 
                    [Contrasena], 
                    [Nombre],
                    [EsAdmin]
                    ) VALUES
                    (@value1, @value2, @value3, @value4, @value5) "))
                {
                    command.Parameters.AddWithValue("@value1", b_IDUsuario);
                    command.Parameters.AddWithValue("@value2", p_item.CodUsuario);
                    command.Parameters.AddWithValue("@value3", p_item.Contrasena);
                    command.Parameters.AddWithValue("@value4", p_item.Nombre);
                    command.Parameters.AddWithValue("@value5", p_item.EsAdmin ? 1 : 0);

                    bool b_ok = MyExecuteNonQueryCommand(command);

                    return b_ok;
                }
            }

            public bool Usuario_Update(PDSIEUCoBD.DBInt_Usuario p_item)
            {
                //Guid b_guid;

                using (var command = new SqlCommand(
                        @"UPDATE Usuarios SET
                    [CodUsuario] = @value1,
                    [Contrasena] = @value2,
                    [Nombre] = @value3,
                    [EsAdmin] = @value4
                    WHERE [IDUsuario] = @w_value1"))
                {
                    command.Parameters.AddWithValue("@value1", p_item.CodUsuario);
                    command.Parameters.AddWithValue("@value2", p_item.Contrasena);
                    command.Parameters.AddWithValue("@value3", p_item.Nombre);
                    command.Parameters.AddWithValue("@value4", p_item.EsAdmin ? 1 : 0);

                    command.Parameters.AddWithValue("@w_value1", p_item.IDUsuario);

                    bool b_ok = MyExecuteNonQueryCommand(command);

                    return b_ok;
                }
            }

            public bool Usuario_Delete(string p_idusuario)
            {
                using (var command = new SqlCommand(
                        @"DELETE FROM Usuarios 
                    WHERE [IDUsuario] = @w_value1"))
                {
                    command.Parameters.AddWithValue("@w_value1", p_idusuario);

                    bool b_ok = MyExecuteNonQueryCommand(command);

                    return b_ok;
                }
            }

            #endregion

            #endregion

            #region Utilidades

            #region Login

            public int CompruebaCredencialesUsuario(string p_cod_usuario, string p_contrasena, out string p_id_usuario)
            {
                p_id_usuario = "";

            PDSIEUCoBD.DBInt_Usuario b_user = Usuario_GetObj(p_cod_usuario);

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
                        return 2;
                    }
                }
            }

            #endregion

            #endregion
        }
}
