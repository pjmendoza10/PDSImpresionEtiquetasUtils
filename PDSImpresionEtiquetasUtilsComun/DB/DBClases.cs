using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDSImpresionEtiquetasUtils.Comun.DB
{
    #region PDSImpresionEtiquetas

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
    }

    public class DB_pds_progutils_GER01_PALETS
    {

        //    157	1	IdEtiquetaPalet	dbo.BAID	varchar(40)	Unchecked	Unchecked	Unchecked	Checked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	2	IdArticulo	dbo.BAID	varchar(40)	Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	3	CodArticulo	varchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	4	DescripcionArticulo	varchar(MAX)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	5	Lote	varchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	6	SSCC	varchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	7	EAN_ArticuloCliente	varchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	8	SeleccionDescripcion	nvarchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	9	IdCliente	dbo.BAID	varchar(40)	Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	10	CodCliente	varchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	11	FechaCreacion	datetime		Unchecked	Unchecked	Unchecked	Unchecked							Unchecked	Unchecked	Unchecked
        //1051	13	EnviadoSGA	int		Unchecked	Unchecked	Unchecked	Unchecked							Unchecked	Unchecked	Unchecked
        //1051	14	CodUnidAlm	nvarchar(10)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked


        public string IdEtiquetaPalet { get; set; }
        public string IdArticulo { get; set; }
        public string CodArticulo { get; set; }
        public string DescripcionArticulo { get; set; }
        public string Lote { get; set; }
        public string SSCC { get; set; }
        public string EAN_ArticuloCliente { get; set; }
        public string SeleccionDescripcion { get; set; }
        public string IdCliente { get; set; }
        public string CodCliente { get; set; }
        public string TipoElementos { get; set; }
        public bool EnviadoSGA { get; set; }
        public bool EnviadoSGAResultado { get; set; }
        public string CodUnidAlm { get; set; }
        public bool Impreso { get; set; }
        public DateTime FechaCreacion { get; set; }

    }

    public class DB_pds_progutils_GER01_PALETS_LINEAS
    {
        //1051	1	IdEtiquetaPalet	dbo.BAID	varchar(40)	Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	2	Linea	int		Unchecked	Unchecked	Unchecked	Unchecked							Unchecked	Unchecked	Unchecked
        //1051	3	NumeroElementos	int		Unchecked	Unchecked	Unchecked	Unchecked							Unchecked	Unchecked	Unchecked
        //1051	4	UnidadesXElemento	int		Unchecked	Unchecked	Unchecked	Unchecked							Unchecked	Unchecked	Unchecked
        //1051	5	TotalUnidades	int		Unchecked	Unchecked	Unchecked	Unchecked							Unchecked	Unchecked	Unchecked

        public string IdEtiquetaPalet { get; set; }
        public int Linea { get; set; }
        public int NumeroElementos { get; set; }
        public int UnidadesXElemento { get; set; }
        public int TotalUnidades { get; set; }
        public decimal KgPorElemento { get; set; }

    }

    public class DB_pds_progutils_GER01_PALETS_BOBINAS
    {
        //57	1	CodElemento	nvarchar(50)		Checked	Unchecked	Unchecked	Checked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	2	IdEtiquetaPalet	dbo.BAID	varchar(40)	Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	3	NLinea	int		Unchecked	Unchecked	Unchecked	Unchecked							Unchecked	Unchecked	Unchecked
        //1051	4	Seccion	nvarchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	5	Lote	nvarchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	6	FechaCaducidad	nvarchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	7	CodMaquina	nvarchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	8	Metros	nvarchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	9	Kilos	nvarchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	10	CodArticulo	nvarchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	11	RefArticuloCliente	nvarchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	12	DesArticulo	nvarchar(MAX)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	13	AptoAlimentacion	bit		Unchecked	Unchecked	Unchecked	Unchecked							Unchecked	Unchecked	Unchecked
        //1051	14	ImprimirFecha	bit		Unchecked	Unchecked	Unchecked	Unchecked							Unchecked	Unchecked	Unchecked
        //1051	15	FechaCreacion	datetime		Unchecked	Unchecked	Unchecked	Unchecked							Unchecked	Unchecked	Unchecked
        //1051	17	CodCliente	nvarchar(50)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked

        public string CodElemento { get; set; }
        public string IdEtiquetaPalet { get; set; }
        public int NLinea { get; set; }
        public string Seccion { get; set; }
        public string Lote { get; set; }
        public string FechaCaducidad { get; set; }
        public string CodMaquina { get; set; }
        public string Metros { get; set; }
        public string Kilos { get; set; }
        public string CodArticulo { get; set; }
        public string RefArticuloCliente { get; set; }
        public string DesArticulo { get; set; }
        public bool AptoAlimentacion { get; set; }
        public bool ImprimirFecha { get; set; }
        public string Unidad { get; set; }
        public string CodCliente { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaHoraMostradaImpresa { get; set; }
    }

    #endregion

    #endregion

    #region OLANET_BASE_2013

    public class DB_LYC_MAQUINAS_SECCION
    {
        //57	1	seccion_id	varchar(15)		Checked	Unchecked	Unchecked	Checked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //57	2	maquina_id	varchar(15)		Checked	Unchecked	Unchecked	Checked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	3	maquina_nme	varchar(20)		Unchecked	Unchecked	Unchecked	Unchecked			Modern_Spanish_CI_AS				Unchecked	Unchecked	Unchecked
        //1051	4	orden_seq	int		Unchecked	Unchecked	Unchecked	Unchecked							Unchecked	Unchecked	Unchecked

        public string seccion_id { get; set; }
        public string maquina_id { get; set; }
        public string maquina_nme { get; set; }
        public int orden_seq { get; set; }
    }

    #endregion

    #region RPS2013

    public class DB_RPS_STKFichaTecnica
    {
        public string IDFichaTecnica { get; set; }
        public string IDArticle { get; set; }

        public decimal Largo { get; set; }
    }

    #endregion
}
