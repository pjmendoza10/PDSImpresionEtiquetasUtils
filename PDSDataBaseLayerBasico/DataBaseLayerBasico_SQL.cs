using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDS.DataBaseLayerBasico.SQL
{
    public class DataBaseLayerBasicoSQL
    {
       //private Configuracion _configuracion = null;
        private string _ConnectionString = "";

        //SqlConnection _conexion = null;

        public DataBaseLayerBasicoSQL(string p_ConnectionString)
        {
            _ConnectionString = p_ConnectionString;

            //_conexion = new SqlConnection(_Config_ConnectionString);
        }

        public bool ConexionTest()
        {
            try
            {
                using (SqlConnection b_conexion = new SqlConnection(_ConnectionString))
                {
                    try
                    {
                        if (b_conexion != null)
                        {
                            if (b_conexion.State != ConnectionState.Open) b_conexion.Open();
                        }
                        else return false;
                    }
                    catch (Exception ex)
                    {
                        //Utilidades.csLogUtils.EscribeLineaLogError(ex);
                        return false;
                    }

                    try
                    {
                        if (b_conexion != null)
                        {
                            b_conexion.Close();
                        }
                    }
                    catch { }
                }
            }
            catch { return false; }

            return true;
        }

        public bool Conectar()
        {
            //using (SqlConnection b_conexion = new SqlConnection(_Config_ConnectionString))
            //{
            //    try
            //    {
            //        if (b_conexion != null) b_conexion.Open();
            //        else return false;
            //    }
            //    catch (Exception ex)
            //    {
            //        return false;
            //    }
            //}
            return true;
        }

        public bool Desconectar()
        {
            //try
            //{
            //    if (_conexion != null) _conexion.Close();
            //    else return false;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            return true;
        }

        #region Propiedades

        public string ConnectionString 
        {
            get { return _ConnectionString; }
        }
 
	    #endregion

        #region "Acciones"

        public DataTable MyExecuteQuery(string p_mi_sql)
        {
            DataTable b_dt = null;

            using (SqlConnection b_conexion = new SqlConnection(_ConnectionString))
            {

                //Conectar(b_conexion);
                using (SqlCommand cmdSel = new SqlCommand(p_mi_sql, b_conexion))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmdSel);
                    b_dt = new DataTable();
                    da.Fill(b_dt);
                    da.Dispose();
                }
                //Desconectar(b_conexion);
            }

            return b_dt;
        }

        public DataTable MyExecuteQueryCommand(SqlCommand p_mi_command)
        {
            return MyExecuteQueryCommand(p_mi_command, _ConnectionString);
        }

        public DataTable MyExecuteQueryCommand(SqlCommand p_mi_command, string p_connectionstring)
        {
            DataTable b_dt = null;

            using (SqlConnection b_conexion = new SqlConnection(p_connectionstring))
            {

                //Conectar(b_conexion);

                p_mi_command.Connection = b_conexion;
                SqlDataAdapter da = new SqlDataAdapter(p_mi_command);
                b_dt = new DataTable();
                da.Fill(b_dt);
                da.Dispose();

                //Desconectar(b_conexion);
            }

            return b_dt;
        }



        public bool MyExecuteNonQuery(string p_mi_sql)
        {
            int filas;

            using (SqlConnection b_conexion = new SqlConnection(_ConnectionString))
            {

                //Conectar(b_conexion);
                using (SqlCommand cmdSel = new SqlCommand(p_mi_sql, b_conexion))
                {
                    filas = cmdSel.ExecuteNonQuery();
                }
                //Desconectar(b_conexion);
            }

            return (filas > 0);
        }

        public bool MyExecuteNonQueryCommand(SqlCommand p_mi_command)
        {
            int filas;

            using (SqlConnection b_conexion = new SqlConnection(_ConnectionString))
            {

                //Conectar(b_conexion);
                b_conexion.Open();

                p_mi_command.Connection = b_conexion;
                filas = p_mi_command.ExecuteNonQuery();

                //Desconectar(b_conexion);
            }

            return (filas > 0);
        }



        //public bool MyExecuteNonQuery2(string p_mi_sql)
        //{
        //    // NO FUNCIONA
        //    int filas;

        //    Conectar();
        //    using (SqlCommand cmdSel = new SqlCommand(p_mi_sql, _conexion))
        //    {
        //        cmdSel.ExecuteNonQueryAsync();
        //    }
        //    Desconectar();

        //    return (filas > 0);
        //}

        #endregion
    }
}
