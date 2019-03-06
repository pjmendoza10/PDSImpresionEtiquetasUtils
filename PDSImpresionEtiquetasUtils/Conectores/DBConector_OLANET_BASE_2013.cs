using PDS.DataBaseLayerBasico.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDSImpresionEtiquetasUtils.Conectores
{
    public class DBConector_OLANET_BASE_2013 : DataBaseLayerBasicoSQL
    {
        public DBConector_OLANET_BASE_2013()
            : base(csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_2013)
        {

        }

        public DBConector_OLANET_BASE_2013(string p_connection_string)
            : base(p_connection_string)
        {

        }


        public List<string> DameNuevosCodigosSSCC(int p_cantidad)
        {
            List<string> b_resultado = new List<string>();

            for (int i = 0; i < p_cantidad; i++)
            {
                string b_error = "";
                string b_sscc = GetNewSSCCCode(ref b_error);
                if (b_error != null)
                {
                    b_resultado.Add(b_sscc);
                }
            }
            return b_resultado;
        }

        /// <summary>
        /// Obtiene un nuevo codigo SSCC e incrementa el contador en OLANET
        /// </summary>
        /// <returns></returns>
        public string GetNewSSCCCode(ref string p_error)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();

                    SqlCommand command = new SqlCommand("uo_prContadorSSCC", conn);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramCodRetorno = new SqlParameter("iContador", SqlDbType.Int);
                    paramCodRetorno.Direction = ParameterDirection.Output;
                    command.Parameters.Add(paramCodRetorno);

                    SqlParameter paramCodRetorno2 = new SqlParameter("nvcSSCC", SqlDbType.VarChar, 18);
                    paramCodRetorno2.Direction = ParameterDirection.Output;
                    command.Parameters.Add(paramCodRetorno2);

                    command.ExecuteNonQuery();

                    return command.Parameters["nvcSSCC"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                p_error = ex.Message;
            }

            return string.Empty;

        }

    }
}
