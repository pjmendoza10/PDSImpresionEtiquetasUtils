using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PDSImpresionEtiquetasUtils.Comun
{
    public class cDBOvt
    {
        private SqlConnection oC;

        public string ConexionString()
        {
            return csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_DATOS_2013;
        }

        public void CerrarConexion()
        {
            try
            {
                this.oC.Close();
                this.oC.Dispose();
            }
            catch (Exception ex)
            {
            }
        }

        private void AbrirConexion()
        {
            this.oC = new SqlConnection(this.ConexionString());
            try
            {
                this.oC.Open();
                if (this.oC.State != ConnectionState.Open)
                    ;
            }
            catch (Exception ex)
            {
            }
        }

        public SqlDataReader RunSQLReturnRS(string Msql)
        {
            this.AbrirConexion();
            return new SqlCommand(Msql, this.oC).ExecuteReader();
        }

        public bool EjecutarSQL(string Msql)
        {
            try
            {
                this.AbrirConexion();
                new SqlCommand(Msql, this.oC).ExecuteNonQuery();
                this.CerrarConexion();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
