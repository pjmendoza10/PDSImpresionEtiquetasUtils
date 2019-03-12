using PDS.DataBaseLayerBasico.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDSImpresionEtiquetasUtils.Comun.DB
{
    public class DataBaseLayer : DataBaseLayerBasicoSQL
    {
        public DataBaseLayer(string p_ConnectionString)
            : base(p_ConnectionString)
        {
            //_Config_ConnectionString = p_ConnectionString;

            //_conexion = new SqlConnection(_Config_ConnectionString);
        }

        #region RPS2013_OLANET

        #region DB_pds_progutils_PALETS

        #region Canonicas       
        public Guid DB_pds_progutils_PALETS_GetUIDEtiqueta(string codEtiqueta)
        {
            Guid b_resultado = new Guid();

            using (var command = new SqlCommand(@"SELECT [uid_tipo_etiqueta]      
                                      FROM [PDSImpresionEtiquetasUtils].[dbo].[TIPO_ETIQUETA]
                                      WHERE CodEtiqueta =  @codEtiqueta"))
            {
                command.Parameters.AddWithValue("@codEtiqueta", codEtiqueta);
                DataTable b_dt = MyExecuteQueryCommand(command);

                foreach (DataRow i_dr in b_dt.Rows)
                {
                    try
                    {
                        b_resultado = Guid.Parse(i_dr["uid_tipo_etiqueta"].ToString()); 
                    }
                    catch (Exception ex) { }
                }
            }

            return b_resultado;
        }

        public bool DB_pds_progutils_PALETS_Insert(Guid guid, string sscc, Guid uidEtiqueta, string datosSerialiados)
        {
            using (var command = new SqlCommand(
                    @"INSERT INTO PDSImpresionEtiquetasUtils.dbo.HISTORICO_ETIQUETAS 

                    ([uid_etiqueta],
                    [SSCC],
                    [FechaCreacion],
                    [uid_tipo_etiqueta],
                    [Datos]
                    ) 
                    VALUES
                    (@value1, @value2, @value3, @value4, @value5) "))
            {
                command.Parameters.AddWithValue("@value1", guid);
                command.Parameters.AddWithValue("@value2", sscc);
                command.Parameters.AddWithValue("@value3", DateTime.Now);
                command.Parameters.AddWithValue("@value4", uidEtiqueta);
                command.Parameters.AddWithValue("@value5", datosSerialiados);

                bool b_ok = MyExecuteNonQueryCommand(command);

                return b_ok;
            }
        }

        public bool DB_pds_progutils_PALETS_Update(string sscc, string idEtiqueta, string datosSerialiados)
        {
            using (var command = new SqlCommand(
                    @"UPDATE PDSImpresionEtiquetasUtils.dbo.HISTORICO_ETIQUETAS SET      
                    [uid_tipo_etiqueta] = @value1,
                    [Datos] = @value2
                    WHERE [SSCC] = @w_value1"))
            {
                command.Parameters.AddWithValue("@value1", idEtiqueta);
                command.Parameters.AddWithValue("@value2", datosSerialiados);
                command.Parameters.AddWithValue("@w_value1", sscc);

                bool b_ok = MyExecuteNonQueryCommand(command);

                return b_ok;
            }
        }

        public bool DB_pds_progutils_PALETS_Delete(string p_IdEtiquetaPalet)
        {
            using (var command = new SqlCommand(
                    @"DELETE FROM PDSImpresionEtiquetasUtils.dbo.HISTORICO_ETIQUETAS 
                    WHERE [IdEtiquetaPalet] = @w_value1"))
            {
                command.Parameters.AddWithValue("@w_value1", p_IdEtiquetaPalet);

                bool b_ok = MyExecuteNonQueryCommand(command);

                return b_ok;
            }
        }
        #endregion

        #region Especificas
        
        public DB_pds_progutils_ETIQ01_PALETS_SIRO_LINEAS dB_Pds_Progutils_ETIQ01_PALETS_SIRO_LINEAS_GetItem(string CodPedido, string ReferenciaSIRO)
        {
            DB_pds_progutils_ETIQ01_PALETS_SIRO_LINEAS db_item = new DB_pds_progutils_ETIQ01_PALETS_SIRO_LINEAS();

            using (var command = new SqlCommand("select ORD.CodOrder, ORDL.ReceptionDemandDate, OrderNumberCustomer, CommentSend, ORDL.Description, ORDL.Quantity, UNITV.Abreviature, ART.CodArticle, " +
                                                               "CART.ReferenceCustomer, ORD.ZipCodeDelivery, ORD.AddressDelivery, ORD.CityDelivery, CUS.CompanyName " +
                                                     "from RPS2013.dbo.facordersl ORD inner join RPS2013.dbo.FACOrderLineSL ORDL on ORDL.IDOrder = ORD.IDOrder " +
                                                        "inner join RPS2013.dbo.STKArticle ART ON ART.IDArticle = ORDL.IDArticle " +
                                                        "inner join RPS2013.dbo.GENMeasureUnit UNITV ON UNITV.IDMeasureUnit = ORDL.IDUnitQuantity " +
                                                        "inner join RPS2013.dbo.FACCustomer CUS on CUS.IDCustomer = ORD.IDCustomer " +
                                                        "left join RPS2013.dbo.FACCustomerArticleSL CART on(CART.IDCustomer = ORD.IDCustomer and CART.IDArticle = ORDL.IDArticle) " +
                                                        "WHERE (ORD.CodOrder LIKE @CodPedido AND ORD.OrderNumberCustomer LIKE @CodSIRO) AND ORDL.NumLine = '1'"))
            {
                command.Parameters.AddWithValue("@CodPedido", "%" + CodPedido + "%");
                command.Parameters.AddWithValue("@CodSIRO", "%" + ReferenciaSIRO + "%");
                DataTable b_dt = MyExecuteQueryCommand(command);
                if (b_dt.Rows.Count > 0)
                {
                    try
                    {
                        db_item.CodArticulo = b_dt.Rows[0]["CodArticle"].ToString();
                        db_item.CodPedido = b_dt.Rows[0]["CodOrder"].ToString();
                        db_item.FechaRecepcionEstimada = b_dt.Rows[0]["ReceptionDemandDate"].ToString();
                        db_item.PedidoGrupoSIRO = b_dt.Rows[0]["OrderNumberCustomer"].ToString();
                        db_item.ComentariosEnvio = b_dt.Rows[0]["CommentSend"].ToString();
                        db_item.DescripcionArticulo = b_dt.Rows[0]["Description"].ToString();
                        if (double.TryParse(b_dt.Rows[0]["Quantity"].ToString(), out double aux_double)) db_item.Cantidad = aux_double;
                        db_item.DescripcionUdMedida = b_dt.Rows[0]["Abreviature"].ToString();
                        db_item.ReferenciaSIRO = b_dt.Rows[0]["ReferenceCustomer"].ToString();
                        db_item.FabricaDestino = b_dt.Rows[0]["CompanyName"].ToString() + "\n" + b_dt.Rows[0]["AddressDelivery"].ToString() + ", " + b_dt.Rows[0]["CityDelivery"].ToString() + " (" + b_dt.Rows[0]["ZipCodeDelivery"].ToString() + ")";
                    }
                    catch (Exception ex) { }
                }
            }
            return db_item;
        }
        public List<DB_pds_progutils_ETIQ01_PALETS_HIST01> DB_pds_progutils_HIST_GetHistoricoEtiqueta(string codEtiq)
        {
            List<DB_pds_progutils_ETIQ01_PALETS_HIST01> dB_item = new List<DB_pds_progutils_ETIQ01_PALETS_HIST01>();
            using (var command = new SqlCommand(@"SELECT HIS.uid_etiqueta,TIP.CodEtiqueta, SSCC, FechaCreacion, Datos 
                FROM [PDSImpresionEtiquetasUtils].[dbo].[HISTORICO_ETIQUETAS] HIS 
                inner join PDSImpresionEtiquetasUtils.dbo.TIPO_ETIQUETA TIP on TIP.uid_tipo_etiqueta = HIS.uid_tipo_etiqueta  
                WHERE TIP.CodEtiqueta = @codEtiqueta order by FechaCreacion desc"))
            {
                command.Parameters.AddWithValue("@codEtiqueta", codEtiq);
                DataTable b_dt = MyExecuteQueryCommand(command);
                foreach (DataRow i_dr in b_dt.Rows)
                {
                    try
                    {
                        DB_pds_progutils_ETIQ01_PALETS_HIST01 b_item = DB_pds_progutils_ETIQ01_PALETS_HIST01_GetObjByDataRow(i_dr);

                        dB_item.Add(b_item);
                    }
                    catch (Exception ex) { }
                }
            }

            return dB_item;
        }

        public List<DB_pds_progutils_ETIQ01_PALETS_GEN01> dB_Pds_Progutils_ETIQ01_PALETS_GEN01_GetItems(string Articulo)
        {
            List < DB_pds_progutils_ETIQ01_PALETS_GEN01> b_resultado = new List<DB_pds_progutils_ETIQ01_PALETS_GEN01>();

            using (var command = new SqlCommand("select CodArticle, ART.Description, CAR.DescriptionCustomer, CAR.ReferenceCustomer, CUST.CodCustomer, CUST.CompanyName, BART.Ean13 from STKArticle ART " +
                                                "left join FACCustomerArticleSL CAR on CAR.IDArticle = ART.IDArticle " +
                                                "left join FACCustomer CUST on CUST.IDCustomer = CAR.IDCustomer " +
                                                "left join STKArticleBarCode BART on ART.IDArticle = BART.IDArticle " +
                                                "where ART.CodArticle LIKE @CodArticulo"))
            {
                //command.Parameters.AddWithValue("@CodArticulo", "%" + Articulo + "%");
                command.Parameters.AddWithValue("@CodArticulo",  Articulo );
                DataTable b_dt = MyExecuteQueryCommand(command);

                foreach (DataRow i_dr in b_dt.Rows)
                {
                    try
                    {
                        DB_pds_progutils_ETIQ01_PALETS_GEN01 b_item = DB_pds_progutils_ETIQ01_PALETS_GEN01_GetObjByDataRow(i_dr);

                        b_resultado.Add(b_item);
                    }
                    catch (Exception ex) { }
                }
            }
            return b_resultado;
        }

        public List<DB_pds_progutils_FEATURES_ARTICLE_GEN01> DB_Pds_Progutils_FEATURES_ARTICLE_GEN01_GetItems(string Articulo)
        {
            List<DB_pds_progutils_FEATURES_ARTICLE_GEN01> b_resultado = new List<DB_pds_progutils_FEATURES_ARTICLE_GEN01>();

            using (var command = new SqlCommand("SELECT CodArticleLabel, Value FROM STKArticleFeature FEAT " +
                                              "inner join STKARTICLE ART on art.IDArticle = FEAT.IDArticle " +
                                              "inner join STKArticleLabel ALAB on ALAB.IDArticleLabel = FEAT.IDArticleLabel " +
                                              "WHERE CodArticle = @CodArticulo"))
            {
                command.Parameters.AddWithValue("@CodArticulo", Articulo);
                DataTable b_dt = MyExecuteQueryCommand(command);

                foreach (DataRow i_dr in b_dt.Rows)
                {
                    try
                    {
                        DB_pds_progutils_FEATURES_ARTICLE_GEN01 b_item = DB_Pds_Progutils_FEATURES_ARTICLE_GEN01_GetObjByDataRow(i_dr);

                        b_resultado.Add(b_item);
                    }
                    catch (Exception ex) { }
                }
            }

            return b_resultado;
        }

        public DB_pds_progutils_DIMENSIONES_ARTICULO_GEN01 DB_Pds_Progutils_DIMENSIONES_ARTICULO_GEN01_GetItem(string Articulo)
        {
            DB_pds_progutils_DIMENSIONES_ARTICULO_GEN01 db_item = new DB_pds_progutils_DIMENSIONES_ARTICULO_GEN01();

            using (var command = new SqlCommand("SELECT FTEC.Altura, FTEC.Ancho, FTEC.Largo, PAL.CodArticle as Palet, FTEC.LlevaCajas, CAJ.CodArticle as Caja " +
                                          "FROM[RPS2013].[dbo].[_STKFichaTecnica_Custom] FTEC " +
                                          "inner join STKArticle ART on ART.IDArticle = FTEC.IDArticle " +
                                          "inner join STKArticle PAL on FTEC.IDPalet = PAL.IDArticle " +
                                          "left join STKArticle CAJ ON FTEC.IDCaja = CAJ.IDArticle  " +
                                          "WHERE ART.CodArticle = @CodArticulo"))
            {
                command.Parameters.AddWithValue("@CodArticulo", Articulo);
                DataTable b_dt = MyExecuteQueryCommand(command);

                if (b_dt.Rows.Count > 0)
                {
                    try
                    {
                        db_item.Altura = b_dt.Rows[0]["Altura"].ToString();
                        db_item.Ancho = b_dt.Rows[0]["Ancho"].ToString();
                        db_item.Largo = b_dt.Rows[0]["Largo"].ToString();
                        db_item.CodPalet = b_dt.Rows[0]["Palet"].ToString();
                        db_item.CodCaja = b_dt.Rows[0]["Caja"].ToString();
                        if (Boolean.TryParse(b_dt.Rows[0]["LlevaCajas"].ToString(), out bool aux_bool)) db_item.TieneCaja = aux_bool;

                        }
                    catch (Exception ex) { }
                }                
            }
            return db_item;
            
        } 

        public DB_pds_progutils_ETIQ01_PALETS_ESTIU_GEN01 DB_pds_progutils_ETIQ01_PALETS_ESTIU_GEN01_GetItem (string sscc)
        {
            DB_pds_progutils_ETIQ01_PALETS_ESTIU_GEN01 db_item = new DB_pds_progutils_ETIQ01_PALETS_ESTIU_GEN01();

            using (var command = new SqlCommand(@"SELECT Orden_id, N_Pedido, Cliente_id, Cliente, Elementos_palet, OrderNumberCustomer, Cantidad_palet, ART.CodArticle, ART.Description, CART.ReferenceCustomer 
                                                  FROM[OLANET_BASE_DATOS_2013].[dbo].[HISTORICO_PALETS_NEW] PAL 
                                                  inner join RPS2013.dbo.FACOrderSL PED on PED.CodOrder = PAL.N_Pedido 
                                                  inner join RPS2013.dbo.FACOrderLineSL PEDL on PEDL.IDOrder = PED.IDOrder 
                                                  inner join RPS2013.dbo.STKArticle ART on ART.IDArticle = PEDL.IDArticle 
                                                  left join RPS2013.dbo.FACCustomerArticleSL CART on(CART.IDCustomer = PED.IDCustomer and CART.IDArticle = art.IDArticle) 
                                                  WHERE sscc = @sscc"))
            {
                command.Parameters.AddWithValue("@sscc", sscc);
                DataTable b_dt = MyExecuteQueryCommand(command);

                if (b_dt.Rows.Count > 0)
                {
                    try
                    {
                        db_item.Lote = b_dt.Rows[0]["Orden_id"].ToString();
                        //db_item.Pedido = b_dt.Rows[0]["N_Pedido"].ToString();
                        db_item.PedidoESTIU = b_dt.Rows[0]["OrderNumberCustomer"].ToString();
                        db_item.IdCliente = b_dt.Rows[0]["Cliente_id"].ToString();
                        db_item.Cliente = b_dt.Rows[0]["Cliente"].ToString();
                        db_item.CodArticulo = b_dt.Rows[0]["CodArticle"].ToString();
                        db_item.Descripcion = b_dt.Rows[0]["Description"].ToString();
                        db_item.ReferenciaESTIU = b_dt.Rows[0]["ReferenceCustomer"].ToString();
                        if (Double.TryParse(b_dt.Rows[0]["Elementos_palet"].ToString(), out double aux_double)) db_item.CantidadPalet = aux_double;
                        if (Double.TryParse(b_dt.Rows[0]["Cantidad_palet"].ToString(), out double aux_double1)) db_item.TotalMetros = aux_double1;

                    }
                    catch (Exception ex) { }
                }
            }
            return db_item;
        }

        public DB_pds_progutils_ETIQ01_PALETS_ESTIU_GEN01 DB_pds_progutils_ETIQ01_PALETS_ESTIU_REP01_GetItem(string sscc)
        {
            DB_pds_progutils_ETIQ01_PALETS_ESTIU_GEN01 db_item = new DB_pds_progutils_ETIQ01_PALETS_ESTIU_GEN01();

            using (var command = new SqlCommand(@"SELECT REP.IdEtiquetaPalet, REP.CodArticulo, Lote, SSCC, CodCliente, CUS.Description as Cliente, ART.Description, CART.ReferenceCustomer  FROM [RPS2013_OLANET].[dbo].[_pds_progutils_GER01_PALETS] REP
				                            inner join RPS2013.dbo.STKArticle ART on ART.IDArticle = REP.IDArticulo 
                                            left join RPS2013.dbo.FACCustomerArticleSL CART on(CART.IDCustomer = REP.IdCliente  and CART.IDArticle = art.IDArticle) 
				                            inner join RPS2013.dbo.FACCustomer CUS on CUS.IDCustomer = REP.IdCliente
                                            WHERE sscc = @sscc"))
            {
                command.Parameters.AddWithValue("@sscc", sscc);
                DataTable b_dt = MyExecuteQueryCommand(command);

                if (b_dt.Rows.Count > 0)
                {
                    try
                    {
                        db_item.IDEtiquetaPalet = b_dt.Rows[0]["IdEtiquetaPalet"].ToString();
                        db_item.Lote = b_dt.Rows[0]["Lote"].ToString();
                        db_item.IdCliente = b_dt.Rows[0]["CodCliente"].ToString();
                        db_item.Cliente = b_dt.Rows[0]["Cliente"].ToString();
                        db_item.CodArticulo = b_dt.Rows[0]["CodArticulo"].ToString();
                        db_item.Descripcion = b_dt.Rows[0]["Description"].ToString();
                        db_item.ReferenciaESTIU = b_dt.Rows[0]["ReferenceCustomer"].ToString();
                    }
                    catch (Exception ex) { }
                }
            }
            return db_item;
        }
        public List<DB_pds_progutils_LINEAS_SSCC_GEN01> DB_Pds_Progutils_LINEAS_SSCC_GEN01_GetItems(string sscc)
        {
            List<DB_pds_progutils_LINEAS_SSCC_GEN01> b_resultado = new List<DB_pds_progutils_LINEAS_SSCC_GEN01>();

            using (var command = new SqlCommand("SELECT N_ELEMENTOS, UDS_POR_ELEMENTO FROM[OLANET_BASE_DATOS_2013].[dbo].[HISTORICO_LIN_PALETS_NEW] where sscc = @sscc"))
            {
                command.Parameters.AddWithValue("@sscc", sscc);
                DataTable b_dt = MyExecuteQueryCommand(command);

                foreach (DataRow i_dr in b_dt.Rows)
                {
                    try
                    {
                        DB_pds_progutils_LINEAS_SSCC_GEN01 b_item = DB_Pds_Progutils_LINEAS_SSCC_GEN01_GetObjByDataRow(i_dr);

                        b_resultado.Add(b_item);
                    }
                    catch (Exception ex) { }
                }
            }

            return b_resultado;
        }

        public List<DB_pds_progutils_LINEAS_SSCC_REP01> DB_Pds_Progutils_LINEAS_SSCC_REP01_GetItems(string idEtiquetaPalet)
        {
            List<DB_pds_progutils_LINEAS_SSCC_REP01> b_resultado = new List<DB_pds_progutils_LINEAS_SSCC_REP01>();

            using (var command = new SqlCommand(@"SELECT NumeroElementos, UnidadesXElemento, TotalUnidades
                                                 FROM[RPS2013_OLANET].[dbo].[_pds_progutils_GER01_PALETS_LINEAS]
                                                 WHERE IdEtiquetaPalet = @idEtiquetaPalet"))
            {
                command.Parameters.AddWithValue("@idEtiquetaPalet", idEtiquetaPalet);
                DataTable b_dt = MyExecuteQueryCommand(command);

                foreach (DataRow i_dr in b_dt.Rows)
                {
                    try
                    {
                        DB_pds_progutils_LINEAS_SSCC_REP01 b_item = DB_Pds_Progutils_LINEAS_SSCC_REP01_GetObjByDataRow(i_dr);

                        b_resultado.Add(b_item);
                    }
                    catch (Exception ex) { }
                }
            }

            return b_resultado;
        }
        #endregion

        #endregion

        #region DB_pds_progutils_PALETS_LINEAS
        private DB_pds_progutils_ETIQ01_PALETS_SIRO_LINEAS DB_pds_progutils_ETIQ01_PALETS_SIRO_LINEAS_GetObjByDataRow(DataRow p_row)
        {
            DB_pds_progutils_ETIQ01_PALETS_SIRO_LINEAS b_item = new DB_pds_progutils_ETIQ01_PALETS_SIRO_LINEAS();
            b_item.Cantidad = (double) p_row["Quantity"];
            b_item.ComentariosEnvio = p_row["CommentSend"].ToString();
            b_item.FechaRecepcionEstimada = DateTime.Parse(p_row["ReceptionDemandDate"].ToString()).ToString("dd/MM/yyyy");
            b_item.FabricaDestino = p_row["AddressDelivery"].ToString() +", "+p_row["CityDelivery"].ToString() +" ("+ p_row["ZipCodeDelivery"].ToString() +")";
            b_item.ReferenciaSIRO = p_row["ReferenceCustomer"].ToString();
            b_item.DescripcionUdMedida = p_row["Abreviature"].ToString();
            b_item.DescripcionArticulo = p_row["Description"].ToString();
            b_item.PedidoGrupoSIRO = p_row["OrderNumberCustomer"].ToString();

            return b_item;
        }

        private DB_pds_progutils_ETIQ01_PALETS_HIST01 DB_pds_progutils_ETIQ01_PALETS_HIST01_GetObjByDataRow(DataRow p_row)
        {
            DB_pds_progutils_ETIQ01_PALETS_HIST01 b_item = new DB_pds_progutils_ETIQ01_PALETS_HIST01();

            b_item.Datos = p_row["Datos"].ToString();
            b_item.FechaCreacion = p_row["FechaCreacion"].ToString();
            b_item.SSCC = p_row["SSCC"].ToString();
            b_item.UidEtiqueta = p_row["uid_etiqueta"].ToString();
            b_item.CodEtiqueta = p_row["CodEtiqueta"].ToString();

            return b_item;
        }

        private DB_pds_progutils_ETIQ01_PALETS_GEN01 DB_pds_progutils_ETIQ01_PALETS_GEN01_GetObjByDataRow(DataRow p_row)
        {
            DB_pds_progutils_ETIQ01_PALETS_GEN01 b_item = new DB_pds_progutils_ETIQ01_PALETS_GEN01();

            b_item.CodArticulo= p_row["CodArticle"].ToString();
            b_item.Descripcion = p_row["Description"].ToString();
            b_item.DescripcionCliente = p_row["DescriptionCustomer"].ToString();
            b_item.NombreCliente = p_row["CompanyName"].ToString();
            b_item.ReferenciaCliente = p_row["ReferenceCustomer"].ToString();
            b_item.IDCustomer = p_row["CodCustomer"].ToString();
            b_item.EAN13 = p_row["Ean13"].ToString();

            return b_item;
        }

        private DB_pds_progutils_FEATURES_ARTICLE_GEN01 DB_Pds_Progutils_FEATURES_ARTICLE_GEN01_GetObjByDataRow (DataRow p_row)
        {
            DB_pds_progutils_FEATURES_ARTICLE_GEN01 b_item = new DB_pds_progutils_FEATURES_ARTICLE_GEN01();
            b_item.CodFeature = p_row["CodArticleLabel"].ToString();
            b_item.ValueFeature = p_row["Value"].ToString();
            return b_item;
        }

        private DB_pds_progutils_LINEAS_SSCC_GEN01 DB_Pds_Progutils_LINEAS_SSCC_GEN01_GetObjByDataRow(DataRow p_row)
        {
            DB_pds_progutils_LINEAS_SSCC_GEN01 b_item = new DB_pds_progutils_LINEAS_SSCC_GEN01();
            if (Double.TryParse(p_row["N_ELEMENTOS"].ToString(), out double aux_double)) b_item.NumBobinas = aux_double;
            if (Double.TryParse(p_row["UDS_POR_ELEMENTO"].ToString(), out double aux_double1)) b_item.MetrosXBobina = aux_double1;

            return b_item;
        }

        private DB_pds_progutils_LINEAS_SSCC_REP01 DB_Pds_Progutils_LINEAS_SSCC_REP01_GetObjByDataRow(DataRow p_row)
        {
            DB_pds_progutils_LINEAS_SSCC_REP01 b_item = new DB_pds_progutils_LINEAS_SSCC_REP01();
            if (Double.TryParse(p_row["NumeroElementos"].ToString(), out double aux_double)) b_item.NumElementos = aux_double;
            if (Double.TryParse(p_row["UnidadesXElemento"].ToString(), out double aux_double1)) b_item.UnidadesXElemento = aux_double1;
            if (Double.TryParse(p_row["TotalUnidades"].ToString(), out double aux_double2)) b_item.TotalUnidades = aux_double2;

            return b_item;
        }
        #endregion

        #region DB_pds_progutils_GER01_PALETS_BOBINAS
        #endregion

        #endregion


        #region OLANET_BASE_2013
        #endregion

        #region RPS2013

        #region Canonicas
        #endregion

        #region Especiales       
        #endregion

        #endregion
    }
}


