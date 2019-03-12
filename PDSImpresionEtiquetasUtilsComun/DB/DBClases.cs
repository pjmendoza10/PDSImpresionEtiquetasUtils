using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDSImpresionEtiquetasUtils.Comun.DB
{
    #region PDSImpresionEtiquetas
    public class DB_pds_progutils_ETIQ01_PALETS_HIST01
    {
        public string UidEtiqueta;
        public string CodEtiqueta;
        public string SSCC;
        public string FechaCreacion;
        public string Datos;
    }

        public class DBInt_Usuario : ICloneable
    {
        public string IDUsuario { get; set; }
        public string CodUsuario { get; set; }
        public string Contrasena { get; set; }
        public string Nombre { get; set; }
        public bool EsAdmin { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    #endregion

    #region RPS2013_OLANET

    #region pds_progutils_GER01
    public class DB_pds_progutils_DIMENSIONES_ARTICULO_GEN01
    {
        public string Altura { get; set; }
        public string Ancho { get; set; }
        public string Largo { get; set; }
        public string CodPalet { get; set; }
        public string CodCaja { get; set; }
        public Boolean TieneCaja { get; set; }
    }

    public class DB_pds_progutils_FEATURES_ARTICLE_GEN01
    {
        public string CodFeature { get; set; }
        public string ValueFeature { get; set; }
    }

    public class DB_pds_progutils_LINEAS_SSCC_GEN01
    {
        public double NumBobinas { get; set; }
        public double MetrosXBobina { get; set; }
    }
    public class DB_pds_progutils_LINEAS_SSCC_REP01
    {
        public double NumElementos { get; set; }
        public double UnidadesXElemento { get; set; }
        public double TotalUnidades { get; set; }
    }

    public class DB_pds_progutils_ETIQ01_PALETS_GEN01
    {
        public string CodArticulo { get; set; }
        public string ReferenciaCliente { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionCliente { get; set; }
        public string NombreCliente { get; set; }
        public string IDCustomer { get; set; }
        public string EAN13 { get; set; }
    }
    public class DB_pds_progutils_ETIQ01_PALETS_SIRO_LINEAS
    {
        public string CodArticulo { get; set; }
        public string CodPedido { get; set; }
        public string FechaRecepcionEstimada { get; set; }
        public string ReferenciaSIRO { get; set; }
        public string ComentariosEnvio { get; set; }
        public string DescripcionUdMedida { get; set; }
        public double Cantidad { get; set; }
        public string DescripcionArticulo { get; set; }
        public string PedidoGrupoSIRO { get; set; }
        public string FabricaDestino { get; set; }
    }
    public class DB_pds_progutils_ETIQ01_PALETS_ESTIU_GEN01
    {
        public string CodArticulo { get; set; }
        public string ReferenciaESTIU { get; set; }
        public double TotalMetros { get; set; }
        public double CantidadPalet { get; set; }
        public string Descripcion { get; set; }
        public string PedidoESTIU { get; set; }
        public string Cliente { get; set; }
        public string IdCliente { get; set; }
        public string Lote { get; set; }
        public string IDEtiquetaPalet { get; set; }
    }

    #endregion

    #endregion

    #region OLANET_BASE_2013

    #endregion

    #region RPS2013
    
    #endregion
}
