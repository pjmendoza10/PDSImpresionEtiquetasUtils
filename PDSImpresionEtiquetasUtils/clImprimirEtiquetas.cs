using Stimulsoft.Base;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Management;
using System.Threading;

namespace PDSImpresionEtiquetasUtils.Comun
{
    public class clImprimirEtiquetas
    {
        private string vLocal = "";
        private string vMaquinaId;
        private string vTerminal;

        public int PrinterLabelReturn(params object[] x)
        {
            //StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkwZq45nPGAko2oALZBw1FWWXY16RykCKFf+yV7YGSudKKAFabWFnX8ZxzW9A94fjHt13HDD/9VuqFr/I6w2xLWlgK1LdLJ44jKwYl2fdnU4tRqW6TikEGFH8EkNQelPjRK4U9ZZvfEJ1s9zEAqZHSqHSK492vSZsdfD6kTWbvLVnI0VewxJEG5macBZ0t0i/UBE6L9ru0Msx0R5U8XbAFyyLtVHUjXkAXbV3B2NodfLXEUuaeA4OKKnqzzCYe0x5YymVDe0hqYeiGgAATUK9hFrSwsbfUD2I59QF15kQ4TD54JHiKB4nKWP1xnPHS6aVn3wLlk3YNlDpTdN6L97fblL8NgfX+Gl8Pcyim4hdjfebHK7VBXXRsiic5s1nH9Z7eAXRm55i0FnoLe+ctwmwEpdVAFDX+FyvK/1oNiksKj12YqfY3MJz6fVr/T3WiUR8BMz8KhjLrlLAxyqybi2jOE7pvhyyVM8ZiFEW7OJoALd70asnZG7U781k+JB8fQbZX1KWYniUgWhzE5gsW7t6sBG8AfSsFYyPzoLvinSqD5SQ==";
            int num = 0;
            foreach (object obj in x)
            {
                if (num == 0)
                    this.vMaquinaId = obj.ToString();
                else
                    this.vTerminal = obj.ToString();
                ++num;
            }
            return this.ImprimirEtiquetasDevolucion(this.vMaquinaId) ? 1 : 2;
        }

        public int PrinterLabelReturnQuality(params object[] x)
        {
            //StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkwZq45nPGAko2oALZBw1FWWXY16RykCKFf+yV7YGSudKKAFabWFnX8ZxzW9A94fjHt13HDD/9VuqFr/I6w2xLWlgK1LdLJ44jKwYl2fdnU4tRqW6TikEGFH8EkNQelPjRK4U9ZZvfEJ1s9zEAqZHSqHSK492vSZsdfD6kTWbvLVnI0VewxJEG5macBZ0t0i/UBE6L9ru0Msx0R5U8XbAFyyLtVHUjXkAXbV3B2NodfLXEUuaeA4OKKnqzzCYe0x5YymVDe0hqYeiGgAATUK9hFrSwsbfUD2I59QF15kQ4TD54JHiKB4nKWP1xnPHS6aVn3wLlk3YNlDpTdN6L97fblL8NgfX+Gl8Pcyim4hdjfebHK7VBXXRsiic5s1nH9Z7eAXRm55i0FnoLe+ctwmwEpdVAFDX+FyvK/1oNiksKj12YqfY3MJz6fVr/T3WiUR8BMz8KhjLrlLAxyqybi2jOE7pvhyyVM8ZiFEW7OJoALd70asnZG7U781k+JB8fQbZX1KWYniUgWhzE5gsW7t6sBG8AfSsFYyPzoLvinSqD5SQ==";
            int num = 0;
            foreach (object obj in x)
            {
                if (num == 0)
                    this.vMaquinaId = obj.ToString();
                else
                    this.vTerminal = obj.ToString();
                ++num;
            }
            return this.ImprimirEtiquetasDevolucionQuality(this.vMaquinaId) ? 1 : 2;
        }

        public int PrinterLabel(params object[] x)
        {
            //StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkwZq45nPGAko2oALZBw1FWWXY16RykCKFf+yV7YGSudKKAFabWFnX8ZxzW9A94fjHt13HDD/9VuqFr/I6w2xLWlgK1LdLJ44jKwYl2fdnU4tRqW6TikEGFH8EkNQelPjRK4U9ZZvfEJ1s9zEAqZHSqHSK492vSZsdfD6kTWbvLVnI0VewxJEG5macBZ0t0i/UBE6L9ru0Msx0R5U8XbAFyyLtVHUjXkAXbV3B2NodfLXEUuaeA4OKKnqzzCYe0x5YymVDe0hqYeiGgAATUK9hFrSwsbfUD2I59QF15kQ4TD54JHiKB4nKWP1xnPHS6aVn3wLlk3YNlDpTdN6L97fblL8NgfX+Gl8Pcyim4hdjfebHK7VBXXRsiic5s1nH9Z7eAXRm55i0FnoLe+ctwmwEpdVAFDX+FyvK/1oNiksKj12YqfY3MJz6fVr/T3WiUR8BMz8KhjLrlLAxyqybi2jOE7pvhyyVM8ZiFEW7OJoALd70asnZG7U781k+JB8fQbZX1KWYniUgWhzE5gsW7t6sBG8AfSsFYyPzoLvinSqD5SQ==";
            int num = 0;
            foreach (object obj in x)
            {
                if (num == 0)
                    this.vMaquinaId = obj.ToString();
                if (num == 1)
                    this.vTerminal = obj.ToString();
                if (num == 2)
                    this.vLocal = obj.ToString();
                if (num == 3)
                    if (obj.ToString() == "True")
                        return this.ImpresionEtiquetas(true) ? 1 : 2;
                    ++num;
            }
            return (!(this.vMaquinaId.Substring(0, 1) == "7") && !(this.vMaquinaId.Substring(0, 2) == "30") && (!(this.vMaquinaId.Substring(0, 1) == "6") && !(this.vMaquinaId.Substring(0, 2) == "28")) && !(this.vMaquinaId.Substring(0, 2) == "33") ? this.ImpresionEtiquetas(true) : this.ImpresionEtiquetas(false)) ? 1 : 2;
        }

        private bool ImprimirEtiquetasDevolucion(string pMaquinaID)
        {
            //StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkwZq45nPGAko2oALZBw1FWWXY16RykCKFf+yV7YGSudKKAFabWFnX8ZxzW9A94fjHt13HDD/9VuqFr/I6w2xLWlgK1LdLJ44jKwYl2fdnU4tRqW6TikEGFH8EkNQelPjRK4U9ZZvfEJ1s9zEAqZHSqHSK492vSZsdfD6kTWbvLVnI0VewxJEG5macBZ0t0i/UBE6L9ru0Msx0R5U8XbAFyyLtVHUjXkAXbV3B2NodfLXEUuaeA4OKKnqzzCYe0x5YymVDe0hqYeiGgAATUK9hFrSwsbfUD2I59QF15kQ4TD54JHiKB4nKWP1xnPHS6aVn3wLlk3YNlDpTdN6L97fblL8NgfX+Gl8Pcyim4hdjfebHK7VBXXRsiic5s1nH9Z7eAXRm55i0FnoLe+ctwmwEpdVAFDX+FyvK/1oNiksKj12YqfY3MJz6fVr/T3WiUR8BMz8KhjLrlLAxyqybi2jOE7pvhyyVM8ZiFEW7OJoALd70asnZG7U781k+JB8fQbZX1KWYniUgWhzE5gsW7t6sBG8AfSsFYyPzoLvinSqD5SQ==";
            if (!File.Exists("\\\\Olanet\\etiquetas\\DEV_STD.mrt"))
                return false;
            PrinterSettings printerSettings = new PrinterSettings();
            printerSettings.PrinterName = this.ImpresoraDefaultBobinaStim();
            string connectionString = csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_DATOS_2013;//ConfigurationManager.ConnectionStrings["COM_OLANET"].ConnectionString;
            StiReport stiReport = new StiReport();
            stiReport.Load("\\\\Olanet\\etiquetas\\DEV_STD.mrt");
            StiSqlDatabase stiSqlDatabase = new StiSqlDatabase();
            ((StiSqlDatabase)stiReport.Dictionary.Databases[0]).ConnectionString = connectionString;
            ((StiSqlSource)stiReport.Dictionary.DataSources[0]).SqlCommand = "SELECT * FROM HISTORICO_DEVOLUCIONES WHERE IMPRESO='No' AND MAQUINA_ID='" + pMaquinaID + "'";
            stiReport.Print(false, printerSettings);
            bool flag = true;
            cDBOvt cDbOvt = new cDBOvt();
            if (!cDbOvt.EjecutarSQL("update HISTORICO_DEVOLUCIONES set Impreso='Si' where maquina_id= '" + pMaquinaID + "' and Impreso='No'"))
                flag = false;
            cDbOvt.CerrarConexion();
            return flag;
        }

        private bool ImprimirEtiquetasDevolucionQuality(string pMaquinaID)
        {
            //StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkwZq45nPGAko2oALZBw1FWWXY16RykCKFf+yV7YGSudKKAFabWFnX8ZxzW9A94fjHt13HDD/9VuqFr/I6w2xLWlgK1LdLJ44jKwYl2fdnU4tRqW6TikEGFH8EkNQelPjRK4U9ZZvfEJ1s9zEAqZHSqHSK492vSZsdfD6kTWbvLVnI0VewxJEG5macBZ0t0i/UBE6L9ru0Msx0R5U8XbAFyyLtVHUjXkAXbV3B2NodfLXEUuaeA4OKKnqzzCYe0x5YymVDe0hqYeiGgAATUK9hFrSwsbfUD2I59QF15kQ4TD54JHiKB4nKWP1xnPHS6aVn3wLlk3YNlDpTdN6L97fblL8NgfX+Gl8Pcyim4hdjfebHK7VBXXRsiic5s1nH9Z7eAXRm55i0FnoLe+ctwmwEpdVAFDX+FyvK/1oNiksKj12YqfY3MJz6fVr/T3WiUR8BMz8KhjLrlLAxyqybi2jOE7pvhyyVM8ZiFEW7OJoALd70asnZG7U781k+JB8fQbZX1KWYniUgWhzE5gsW7t6sBG8AfSsFYyPzoLvinSqD5SQ==";
            if (!File.Exists("\\\\Olanet\\etiquetas\\DEV_STD.mrt"))
                return false;
            PrinterSettings printerSettings = new PrinterSettings();
            printerSettings.PrinterName = this.ImpresoraDefaultBobinaStim();
            string connectionString = csEstadoPermanente.Configuracion.Datos.connectionString_OLANET_BASE_DATOS_2013;//ConfigurationManager.ConnectionStrings["COM_OLANET"].ConnectionString;
            StiReport stiReport = new StiReport();
            stiReport.Load("\\\\Olanet\\etiquetas\\DEV_STDQ.mrt");
            StiSqlDatabase stiSqlDatabase = new StiSqlDatabase();
            ((StiSqlDatabase)stiReport.Dictionary.Databases[0]).ConnectionString = connectionString;
            ((StiSqlSource)stiReport.Dictionary.DataSources[0]).SqlCommand = "SELECT * FROM HISTORICO_DEVOLUCIONES WHERE IMPRESO='No' AND MAQUINA_ID='" + pMaquinaID + "'";
            stiReport.Print(false, printerSettings);
            bool flag = true;
            cDBOvt cDbOvt = new cDBOvt();
            if (!cDbOvt.EjecutarSQL("update HISTORICO_DEVOLUCIONES set Impreso='Si' where maquina_id= '" + pMaquinaID + "' and Impreso='No'"))
                flag = false;
            cDbOvt.CerrarConexion();
            return flag;
        }

        private bool ImpresionEtiquetas(bool bStimul)
        {
            //StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkwZq45nPGAko2oALZBw1FWWXY16RykCKFf+yV7YGSudKKAFabWFnX8ZxzW9A94fjHt13HDD/9VuqFr/I6w2xLWlgK1LdLJ44jKwYl2fdnU4tRqW6TikEGFH8EkNQelPjRK4U9ZZvfEJ1s9zEAqZHSqHSK492vSZsdfD6kTWbvLVnI0VewxJEG5macBZ0t0i/UBE6L9ru0Msx0R5U8XbAFyyLtVHUjXkAXbV3B2NodfLXEUuaeA4OKKnqzzCYe0x5YymVDe0hqYeiGgAATUK9hFrSwsbfUD2I59QF15kQ4TD54JHiKB4nKWP1xnPHS6aVn3wLlk3YNlDpTdN6L97fblL8NgfX+Gl8Pcyim4hdjfebHK7VBXXRsiic5s1nH9Z7eAXRm55i0FnoLe+ctwmwEpdVAFDX+FyvK/1oNiksKj12YqfY3MJz6fVr/T3WiUR8BMz8KhjLrlLAxyqybi2jOE7pvhyyVM8ZiFEW7OJoALd70asnZG7U781k+JB8fQbZX1KWYniUgWhzE5gsW7t6sBG8AfSsFYyPzoLvinSqD5SQ==";
            cDBOvt cDbOvt1 = new cDBOvt();
            string miSeccion = "";
            List<StiReport> reports = new List<StiReport>();
            string Msql1 = "select Seccion_id, orden_id, Metros_bobina, operacion_id," + " Cod_mat_sal, Kilogramos,KgsCalculados, NUMERO_ELEMENTO " + " from HISTORICO_TRAZABILIDAD where " + "impreso='No' and maquina_id ='" + this.vMaquinaId + "'";
            bool flag1 = false;
            SqlDataReader sqlDataReader1 = cDbOvt1.RunSQLReturnRS(Msql1);
            while (sqlDataReader1.Read())
            {
                bool flag2 = false;
                int num1 = 0;
                cDBOvt cDbOvt2 = new cDBOvt();
                string Msql2 = "select * from OLANET_BASE_DATOS_2013.dbo.MONITOR_LIN_PALET_REP WHERE N_BOBINA='" + sqlDataReader1["cod_mat_sal"].ToString() + "' " + " AND MAQUINA_ID='" + this.vMaquinaId + "'";
                SqlDataReader sqlDataReader2 = cDbOvt2.RunSQLReturnRS(Msql2);
                if (sqlDataReader2.Read())
                    flag2 = true;
                sqlDataReader2.Close();
                cDbOvt2.CerrarConexion();
                if (!flag2)
                {
                    string Msql3 = "select PN.Tipo, LIN.SERIE from OLANET_BASE_DATOS_2013.dbo.HISTORICO_PALETS_NEW PN " + " INNER JOIN  OLANET_BASE_DATOS_2013.dbo.HISTORICO_PALETS_LIN LIN ON PN.sscc = LIN.SSCC" + " where LIN.SERIE = '" + sqlDataReader1["cod_mat_sal"].ToString() + "' and PN.Maquina_id = '" + this.vMaquinaId + "' AND PN.Tipo>=10";
                    SqlDataReader sqlDataReader3 = cDbOvt2.RunSQLReturnRS(Msql3);
                    if (sqlDataReader3.Read())
                    {
                        if (System.Convert.ToInt32(sqlDataReader3["Tipo"]) > 10)
                            num1 = System.Convert.ToInt32(sqlDataReader3["Tipo"]);
                        flag2 = true;
                        if (System.Convert.ToInt32(sqlDataReader3["Tipo"]) == 50)
                        {
                            num1 = 0;
                            flag2 = false;
                        }
                    }
                    sqlDataReader3.Close();
                    cDbOvt2.CerrarConexion();
                }
                string str1 = "";
                string str2 = !(System.Convert.ToDecimal(sqlDataReader1["NUMERO_ELEMENTO"]) == Decimal.Zero) ? string.Format("{0:0.##}", (object)System.Convert.ToInt32(sqlDataReader1["numero_elemento"])) : "";
                string str3;
                if (System.Convert.ToDecimal(sqlDataReader1["Kilogramos"]) == Decimal.Zero)
                {
                    str1 = "";
                    str3 = !(System.Convert.ToDecimal(sqlDataReader1["KgsCalculados"]) == Decimal.Zero) ? "" : "";
                }
                else
                    str3 = string.Format("{0:#,##0.00}", (object)System.Convert.ToDecimal(sqlDataReader1["Kilogramos"].ToString()));
                string str4 = sqlDataReader1["Seccion_id"].ToString();
                miSeccion = sqlDataReader1["Seccion_id"].ToString();
                string str5 = sqlDataReader1["Operacion_id"].ToString();
                string str6 = sqlDataReader1["orden_id"].ToString();
                string str7 = sqlDataReader1["Metros_bobina"].ToString();
                string str8 = sqlDataReader1["Cod_mat_sal"].ToString();
                string Msql4;
                if (System.Convert.ToDecimal(sqlDataReader1["Metros_bobina"]) != Decimal.Zero)
                    Msql4 = "update HISTORICO_TRAZABILIDAD set Impreso='Si' where maquina_id= '" + this.vMaquinaId + "' and Cod_mat_sal = '" + str8 + "' ";
                else
                    Msql4 = "DELETE FROM HISTORICO_TRAZABILIDAD where Metros_bobina=0 and  maquina_id= '" + this.vMaquinaId + "' and Cod_mat_sal = '" + str8 + "' ";
                if (cDbOvt1.EjecutarSQL(Msql4))
                    ;
                cDBOvt cDbOvt3 = new cDBOvt();
                string Msql5 = "SELECT dbo.cpcurcab.codart AS codart, dbo.alart.descri1 as articulo," + "dbo.alart.concad AS GARANTIA,dbo.alart.diascad AS mesesgaran, dbo.cpcurcab.numped,dbo.cpcurcab.PedCliente,dbo.alart.SemiElaborado, isnull(DBO.ALART.SReferencia,'') SReferencia, dbo.cpcurcab.CodigoCliente,  dbo.cpcurcab.Largo FROM  dbo.cpcurcab " + " INNER JOIN dbo.alart ON dbo.cpcurcab.codart = dbo.alart.codart" + " WHERE  dbo.cpcurcab.nordfab = '" + str6 + "'";
                string str9 = "";
                long num2 = 0;
                string pArticulo = "";
                string str10 = "";
                SqlDataReader sqlDataReader4 = (SqlDataReader)null;
                string str11 = "";
                string str12 = "";
                int num3 = 0;
                string str13 = "";
                string pCliente = "";
                Decimal num4 = new Decimal();
                SqlDataReader sqlDataReader5 = cDbOvt3.RunSQLReturnRS(Msql5);
                if (sqlDataReader5.Read())
                {
                    pCliente = sqlDataReader5["CodigoCliente"].ToString();
                    pArticulo = sqlDataReader5["codart"].ToString();
                    str10 = sqlDataReader5["articulo"].ToString();
                    str9 = sqlDataReader5["GARANTIA"].ToString();
                    str11 = sqlDataReader5["PedCliente"].ToString();
                    num3 = System.Convert.ToInt32(sqlDataReader5["SemiElaborado"]);
                    str13 = sqlDataReader5["SReferencia"].ToString();
                    num4 = System.Convert.ToDecimal(sqlDataReader5["Largo"]);
                    str12 = "";
                    string str14 = sqlDataReader5["numped"].ToString().Trim();
                    if (str14 != "")
                    {
                        string Msql3 = "Select top 1 dbo.fapedl.descr as descr from dbo.fapedl where codemp = '001' and tipcont = '0' and numped = '" + str14 + "' and codart ='" + pArticulo + "' and nordfab = " + str6;
                        sqlDataReader4 = cDbOvt3.RunSQLReturnRS(Msql3);
                        if (sqlDataReader4.Read())
                            str10 = sqlDataReader4["descr"].ToString();
                    }
                    try
                    {
                        num2 = sqlDataReader5["mesesgaran"] != null ? (long)System.Convert.ToInt32(sqlDataReader5["mesesgaran"].ToString()) : 0L;
                    }
                    catch (Exception ex)
                    {
                    }
                }
                string str15 = string.Empty;
                bool flag3 = false;
                string Msql6 = "SELECT A.*," + "\t   B.EAN13 AS EAN13, " + "\t   dbo.uo_overtel_comprobar_esUltimaOperacion('" + this.vMaquinaId.ToString() + "') AS EsUltima " + " FROM alartdv A FULL JOIN " + "\t alcodbar B ON A.codemp = B.codemp AND A.codart = B.codart " + " WHERE A.dva14 = 1 AND A.codart = '" + pArticulo + "' AND A.codemp = '001'";
                SqlDataReader sqlDataReader6 = cDbOvt3.RunSQLReturnRS(Msql6);
                if (sqlDataReader6.Read())
                {
                    flag3 = true;
                    str15 = sqlDataReader6["EAN13"] == null ? string.Empty : sqlDataReader6["EAN13"].ToString();
                    if ((sqlDataReader6["ESULTIMA"] == null ? 0 : System.Convert.ToInt32(sqlDataReader6["ESULTIMA"])) != 2)
                        flag3 = false;
                }
                if (sqlDataReader6 != null)
                {
                    sqlDataReader6.Close();
                    cDbOvt3.CerrarConexion();
                    sqlDataReader6.Dispose();
                }
                if (sqlDataReader4 != null)
                {
                    sqlDataReader4.Close();
                    cDbOvt3.CerrarConexion();
                    sqlDataReader4.Dispose();
                }
                sqlDataReader5.Close();
                cDbOvt3.CerrarConexion();
                sqlDataReader5.Dispose();
                string str16 = "";
                string str17 = str4;
                if (!(str17 == "IMPRESION"))
                {
                    if (!(str17 == "REBOBINADO"))
                    {
                        if (str17 == "CONFECCION")
                            str16 = "Bolsas";
                    }
                    else
                        str16 = "Metros";
                }
                else
                    str16 = "Metros";
                if (this.vMaquinaId.ToString() == "200" || this.vMaquinaId.ToString() == "201" || this.vMaquinaId.ToString() == "202")
                    str16 = "Bandejas";
                int num5 = 0;
                cDBOvt cDbOvt4 = new cDBOvt();
                string Msql7 = "select * from  CPCUROPE WHERE CODEMP='001' " + " AND NORDFAB='" + str6 + "' AND NUMOPE='" + str5 + "'";
                SqlDataReader sqlDataReader7 = cDbOvt1.RunSQLReturnRS(Msql7);
                int num6 = 0;
                if (sqlDataReader7.Read())
                    num6 = System.Convert.ToInt32(sqlDataReader7["EsUltima"]);
                sqlDataReader7.Close();
                cDbOvt1.CerrarConexion();
                string Msql8 = num6 != 1 ? "SELECT 1 MaxNEtiqueta FROM V_ETIQUETAS_ARTICULOS WHERE CodArticle='" + pArticulo + "' AND CodCompany='001'" : "SELECT MAX(NEtiqueta) MaxNEtiqueta FROM V_ETIQUETAS_ARTICULOS WHERE CodArticle='" + pArticulo + "' AND CodCompany='001'";
                SqlDataReader sqlDataReader8 = cDbOvt4.RunSQLReturnRS(Msql8);
                if (sqlDataReader8.Read())
                    num5 = !System.Convert.IsDBNull(sqlDataReader8["MaxNEtiqueta"]) ? System.Convert.ToInt32(sqlDataReader8["MaxNEtiqueta"]) : 0;
                sqlDataReader8.Close();
                cDbOvt4.CerrarConexion();
                cDBOvt cDbOvt5 = new cDBOvt();
                string Msql9 = num6 != 1 ? "SELECT top 1 * FROM V_ETIQUETAS_ARTICULOS WHERE CodArticle='" + pArticulo + "' AND CodCompany='001' order by NEtiqueta" : "SELECT * FROM V_ETIQUETAS_ARTICULOS WHERE CodArticle='" + pArticulo + "' AND CodCompany='001' order by NEtiqueta";
                SqlDataReader sqlDataReader9 = cDbOvt5.RunSQLReturnRS(Msql9);
                while (sqlDataReader9.Read())
                {
                    StiVariable stiVariable1 = new StiVariable("codigobarras", typeof(string));
                    StiVariable stiVariable2 = new StiVariable("Logo", typeof(string));
                    StiVariable stiVariable3 = new StiVariable("Seccion", typeof(string));
                    StiVariable stiVariable4 = new StiVariable("LOTE", typeof(string));
                    StiVariable stiVariable5 = new StiVariable("FECCAD", typeof(string));
                    StiVariable stiVariable6 = new StiVariable("MAQ", typeof(string));
                    StiVariable stiVariable7 = new StiVariable("METROS", typeof(string));
                    StiVariable stiVariable8 = new StiVariable("KILOS", typeof(string));
                    StiVariable stiVariable9 = new StiVariable("CODART", typeof(string));
                    StiVariable stiVariable10 = new StiVariable("SREF", typeof(string));
                    StiVariable stiVariable11 = new StiVariable("DESCRIPCION", typeof(string));
                    StiVariable stiVariable12 = new StiVariable("SPED", typeof(string));
                    StiVariable stiVariable13 = new StiVariable("NUMELE", typeof(string));
                    StiVariable stiVariable14 = new StiVariable("APTOALIMENTACION", typeof(bool));
                    StiVariable stiVariable15 = new StiVariable("IMPRIMIRFECHA", typeof(bool));
                    try
                    {
                        if (num3 == 1)
                        {
                            stiVariable2.Value = "S E M I E L A B O R A D O";
                            stiVariable3.Value = "";
                        }
                        else
                        {
                            if (System.Convert.ToInt32(sqlDataReader9["EsAnonimo"]) == 1)
                                stiVariable2.Value = "";
                            else
                                stiVariable2.Value = "PLASTICOS DEL SEGURA";
                            if (flag2)
                            {
                                stiVariable2.Value = "** A REPROCESAR **";
                                if (num1 == 12)
                                    stiVariable2.Value = "** A REPROCESAR EN  49 **";
                                if (num1 == 15)
                                    stiVariable2.Value = "** A REPROCESAR PUESTO EN 49 **";
                                if (num1 == 120)
                                    stiVariable2.Value = "** A REPROCESAR YA GASTADO **";
                                if (num1 == 0)
                                    stiVariable2.Value = "** A REPROCESAR **";
                                if (num1 == 50)
                                    stiVariable2.Value = "PLASTICOS DEL SEGURA";
                            }
                            stiVariable3.Value = str4;
                        }
                        bool flag4 = false;
                        if (num6 == 0)
                        {
                            string Msql3 = "select CodWork MBono  from rps2013.dbo.CPRMultiImputationTask it " + "inner join rps2013.dbo.CPRMultiImputation ic on it.IDMultiImputation = ic.IDMultiImputation " + "inner join rps2013.dbo._CPRMOTask_Custom tc on it.IDMOTask = tc.IDMOTask " + " inner join rps2013.dbo.CPRMOTask tc1 on tc.IDMOTask = tc1.IDMOTask " + " inner join rps2013.dbo.CPRManufacturingOrder mr on it.IDManufacturingOrder = mr.IDManufacturingOrder " + " inner join olanet_base_datos_2013.dbo.cpcurope oper on tc1.IDMOTask = oper.IdMOTask " + " where oper.CODEMP='001' and oper.NORDFAB='" + str6 + "' and oper.numope='" + str5 + "'";
                            SqlDataReader sqlDataReader3 = cDbOvt1.RunSQLReturnRS(Msql3);
                            if (sqlDataReader3.Read())
                            {
                                if (!System.Convert.IsDBNull(sqlDataReader3["MBono"]) && !string.IsNullOrEmpty(sqlDataReader3["MBono"].ToString()))
                                {
                                    stiVariable4.Value = str6;
                                    flag4 = true;
                                }
                            }
                            else if (System.Convert.ToInt32(sqlDataReader9["EsOF"]) == 1)
                                stiVariable4.Value = str6;
                            sqlDataReader3.Close();
                            cDbOvt1.CerrarConexion();
                        }
                        else if (System.Convert.ToInt32(sqlDataReader9["EsOF"]) == 1)
                            stiVariable4.Value = str6;
                        if (System.Convert.ToInt32(sqlDataReader9["EsFechaImpresion"]) == 1)
                            stiVariable15.ValueObject = (object)true;
                        if (str9 == "N")
                            stiVariable5.Value = "";
                        else
                            stiVariable5.Value = "F.Cad.: " + string.Format("{0:dd/MM/yyyy}", (object)DateTime.Now.AddDays((double)num2));
                        if (num3 == 1)
                        {
                            stiVariable6.Value = "AL: 39";
                        }
                        else
                        {
                            stiVariable6.Value = "";
                            if (System.Convert.ToInt32(sqlDataReader9["EsMaquina"]) == 1)
                                stiVariable6.Value = "MQ: " + this.vMaquinaId;
                        }
                        stiVariable7.Value = "";
                        if (System.Convert.ToInt32(sqlDataReader9["EsCantidad"]) == 1)
                            stiVariable7.Value = str16 + ":" + str7;
                        int pTipo = 0;
                        stiVariable8.Value = "";
                        if (str16 == "Metros")
                        {
                            double num7 = Math.Round(this.dConversion(pArticulo, System.Convert.ToDouble(str7) / 1000.0, ref pTipo) * 1000.0, 0);
                            if (num7 != 0.0)
                            {
                                if (pTipo == 1)
                                {
                                    string str14 = string.Format("{0:#,##0.00}", (object)num7);
                                    if (System.Convert.ToInt32(sqlDataReader9["EsKilogramos"]) == 1)
                                        stiVariable8.Value = "Bol:" + str14;
                                }
                                else if (str3 == "")
                                {
                                    string str14 = string.Format("{0:#,##0.00}", (object)(num7 / 1000.0));
                                    if (System.Convert.ToInt32(sqlDataReader9["EsKilogramos"]) == 1)
                                        stiVariable8.Value = "Kg:" + str14;
                                }
                                else if (System.Convert.ToInt32(sqlDataReader9["EsKilogramos"]) == 1)
                                    stiVariable8.Value = "Kg:" + str3;
                            }
                            else if (System.Convert.ToInt32(sqlDataReader9["EsKilogramos"]) == 1)
                                stiVariable8.Value = "Kg:" + str3;
                        }
                        else if (System.Convert.ToInt32(sqlDataReader9["EsKilogramos"]) == 1)
                            stiVariable8.Value = "Kg:" + str3;
                        stiVariable1.Value = "";
                        if (!flag3)
                        {
                            Font font = new Font("C39HrP24DhTt", 36f);
                            if (System.Convert.ToInt32(sqlDataReader9["EsCodigoBobCajas"]) == 1)
                                stiVariable1.Value = str8;
                        }
                        else if (!string.IsNullOrEmpty(str15))
                        {
                            try
                            {
                                stiVariable1.Value = str15;
                            }
                            catch (Exception ex)
                            {
                                stiVariable1.Value = "Código EAN13 Inválido";
                            }
                        }
                        else
                            stiVariable1.Value = "Código EAN13 Inválido";
                        stiVariable9.Value = "";
                        stiVariable13.Value = "";
                        if (System.Convert.ToInt32(sqlDataReader9["EsCodArticulo"]) == 1)
                        {
                            string str14 = num3 != 1 ? "C.ART: " + pArticulo : "SM: " + pArticulo;
                            stiVariable9.Value = str14;
                            stiVariable10.Value = "";
                            if (System.Convert.ToInt32(sqlDataReader9["EsRef"]) == 1)
                                stiVariable10.Value = "/S.Ref: " + str13;
                        }
                        else
                        {
                            stiVariable10.Value = "";
                            if (System.Convert.ToInt32(sqlDataReader9["EsRef"]) == 1)
                                stiVariable10.Value = "S.Ref: " + str13;
                        }
                        stiVariable11.Value = "";
                        if (System.Convert.ToInt32(sqlDataReader9["EsDescArt"]) == 1)
                            stiVariable11.Value = str10;
                        stiVariable12.Value = "";
                        if (System.Convert.ToInt32(sqlDataReader9["NPedido"]) == 1)
                            stiVariable12.Value = "S/Ped.: " + str11.ToString();
                        stiVariable13.Value = "";
                        if (System.Convert.ToInt32(sqlDataReader9["NBobina"]) == 1)
                            stiVariable13.Value = "ELE:" + str2;
                        stiVariable14.ValueObject = (object)true;
                        flag1 = true;
                        string path1 = !(str7 != "0") ? "\\\\Olanet\\etiquetas\\STD_0.mrt" : (!flag2 ? (num6 != 1 ? (!this.vMaquinaId.Equals("49") ? "\\\\Olanet\\etiquetas\\STD_CURSO.mrt" : this.fFile(pArticulo, pCliente)) : (num3 == 1 ? "\\\\Olanet\\etiquetas\\STD_CURSO.mrt" : this.fFile(pArticulo, pCliente))) : "\\\\Olanet\\etiquetas\\STD_REP.mrt");
                        StiReport stiReport = new StiReport();
                        if (!bStimul && (path1 == "\\\\Olanet\\etiquetas\\STD_CURSO.mrt" || path1 == "\\\\Olanet\\etiquetas\\STD.mrt" || path1 == "\\\\Olanet\\etiquetas\\C_100296.mrt" || path1 == "\\\\Olanet\\etiquetas\\C_999986.mrt"))
                        {
                            string path2 = !(path1 == "\\\\Olanet\\etiquetas\\STD_CURSO.mrt") ? this.fFileZebra(pArticulo, pCliente) : "\\\\Olanet\\etiquetas\\ZEBRA\\EtiquetaSTD.zpl";
                            flag1 = false;
                            string[] strArray = File.ReadAllLines(path2);
                            DateTime now = DateTime.Now;
                            string str14 = "C:\\TEMPORAL\\ETIQ" + now.ToString("yyyyMMddHHmmss") + ".TMP";
                            StreamWriter streamWriter = new StreamWriter(str14);
                            for (int index = 0; index < strArray.Length; ++index)
                            {
                                string str18 = strArray[index].Replace("<<CODIGOBARRAS>>", str8.Substring(0, 14) + ">6" + str8.Substring(14, 1)).Replace("<<LOGO>>", (string)((StiExpression)stiVariable2)).Replace("<<NUMORDEN>>", (string)((StiExpression)stiVariable4));
                                string str19 = (!(path1 == "\\\\Olanet\\etiquetas\\STD.mrt") ? str18.Replace("<<ELEMENTOS>>", "").Replace("<<PROVEEDOR>>", "").Replace("<<SUPEDIDO>>", "").Replace("<<SUREFERENCIA>>", "") : str18.Replace("<<ELEMENTOS>>", (string)((StiExpression)stiVariable13)).Replace("<<PROVEEDOR>>", "").Replace("<<SUPEDIDO>>", (string)((StiExpression)stiVariable12)).Replace("<<SUREFERENCIA>>", (string)((StiExpression)stiVariable10))).Replace("<<DESCRIPCIONART>>", (string)((StiExpression)stiVariable11)).Replace("<<CODIGOARTICULO>>", (string)((StiExpression)stiVariable9)).Replace("<<KILOGRAMOS>>", (string)((StiExpression)stiVariable8));
                                if (str19.Contains("<<PIE>>"))
                                {
                                    try
                                    {
                                        if (miSeccion == "IMPRESION")
                                        {
                                            string Msql3 = "select clic.CodCliche from rps2013.dbo.CPRManufacturingOrder mr  inner join rps2013.dbo._CPRManufacturingOrder_custom mrc  on mr.IDManufacturingOrder = mrc.IDManufacturingOrder  inner join rps2013.dbo._STKCliche_Custom clic  on mrc.IDCliche = clic.IDCliche where mr.CodManufacturingOrder = '" + str6 + "'";
                                            cDBOvt cDbOvt6 = new cDBOvt();
                                            string newValue = "";
                                            SqlDataReader sqlDataReader3 = cDbOvt6.RunSQLReturnRS(Msql3);
                                            if (sqlDataReader3.Read())
                                                newValue = "ID CLICHE : " + sqlDataReader3["CodCliche"].ToString();
                                            if (sqlDataReader3 != null)
                                            {
                                                sqlDataReader3.Close();
                                                cDbOvt6.CerrarConexion();
                                                sqlDataReader3.Dispose();
                                            }
                                            str19 = !flag4 ? str19.Replace("<<PIE>>", newValue) : str19.Replace("<<PIE>>", "Multicaida");
                                        }
                                        else
                                            str19 = str19.Replace("<<PIE>>", "Para cualquier reclamaci\\A2n rogamos adjunten esta etiqueta");
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                if (str19.Contains("<<SOLOMETROS>>"))
                                {
                                    string str20 = stiVariable7.ToString();
                                    str19 = str19.Replace("<<SOLOMETROS>>", str20.Substring(6));
                                }
                                if (str19.Contains("<<MOTIVOS>>"))
                                {
                                    if (num6 == 1)
                                    {
                                        if (num4 != Decimal.Zero && str16 == "Metros")
                                        {
                                            try
                                            {
                                                string newValue = "I.Aprx " + Decimal.Multiply(System.Convert.ToDecimal(str7) / (num4 / new Decimal(1000)), Decimal.One).ToString("##,##0.00");
                                                str19 = str19.Replace("<<MOTIVOS>>", newValue);
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }
                                    }
                                    else
                                        str19 = str19.Replace("<<MOTIVOS>>", "");
                                }
                                string str21 = str19.Replace("<<METROS>>", (string)((StiExpression)stiVariable7)).Replace("<<FECHACADUCIDAD>>", (string)((StiExpression)stiVariable5));
                                now = DateTime.Now;
                                string newValue1 = now.ToString("dd/MM/yyyy HH:mm:ss");
                                string str22 = str21.Replace("<<FECHAIMPRESION>>", newValue1).Replace("<<MAQUINA>>", (string)((StiExpression)stiVariable6));
                                if (!(bool)stiVariable15.ValueObject)
                                    str22 = str22.Replace("<<FECHAIMPRESION>>", "");
                                string str23 = str22.Replace("<<SECCION>>", (string)((StiExpression)stiVariable3));
                                streamWriter.WriteLine(str23);
                            }
                            streamWriter.Close();
                            if (File.Exists(str14))
                            {
                                streamWriter.Dispose();
                                string str18 = this.ImpresoraDefaultBobinaStim();
                                PrinterSettings printerSettings = new PrinterSettings();
                                //this.ImpresoraPuerto(str18);
                                try
                                {
                                    string str19 = miSeccion;
                                    if (!(str19 == "IMPRESION"))
                                    {
                                        if (str19 == "LAMINADO")
                                        {
                                            RawPrinterHelper.SendFileToPrinter(str18, str14);
                                            RawPrinterHelper.SendFileToPrinter(str18, str14);
                                        }
                                        else
                                            RawPrinterHelper.SendFileToPrinter(str18, str14);
                                    }
                                    else
                                    {
                                        RawPrinterHelper.SendFileToPrinter(str18, str14);
                                        RawPrinterHelper.SendFileToPrinter(str18, str14);
                                        RawPrinterHelper.SendFileToPrinter(str18, str14);
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                        else
                        {
                            stiReport.Load(path1);
                            StiVariable stiVariable16 = new StiVariable("CODPROV", typeof(string));
                            stiVariable16.Value = "";
                            stiReport.Dictionary.Variables["codigobarras"] = stiVariable1;
                            stiReport.Dictionary.Variables["Logo"] = stiVariable2;
                            stiReport.Dictionary.Variables["Seccion"] = stiVariable3;
                            stiReport.Dictionary.Variables["LOTE"] = stiVariable4;
                            stiReport.Dictionary.Variables["FECCAD"] = stiVariable5;
                            stiReport.Dictionary.Variables["MAQ"] = stiVariable6;
                            stiReport.Dictionary.Variables["METROS"] = stiVariable7;
                            stiReport.Dictionary.Variables["KILOS"] = stiVariable8;
                            stiReport.Dictionary.Variables["CODART"] = stiVariable9;
                            stiReport.Dictionary.Variables["SREF"] = stiVariable10;
                            stiReport.Dictionary.Variables["DESCRIPCION"] = stiVariable11;
                            stiReport.Dictionary.Variables["SPED"] = stiVariable12;
                            stiReport.Dictionary.Variables["CODPROV"] = stiVariable16;
                            stiReport.Dictionary.Variables["APTOALIMENTACION"] = stiVariable14;
                            stiReport.Dictionary.Variables["IMPRIMIRFECHA"] = stiVariable15;
                            stiReport.Dictionary.Variables["NUMELE"] = stiVariable13;
                            if (path1 == "\\\\Olanet\\etiquetas\\STD_0.mrt")
                            {
                                string Msql3 = "select clic.CodCliche from rps2013.dbo.CPRManufacturingOrder mr  inner join rps2013.dbo._CPRManufacturingOrder_custom mrc  on mr.IDManufacturingOrder = mrc.IDManufacturingOrder  inner join rps2013.dbo._STKCliche_Custom clic  on mrc.IDCliche = clic.IDCliche where mr.CodManufacturingOrder = '" + str6 + "'";
                                cDBOvt cDbOvt6 = new cDBOvt();
                                string str14 = "";
                                SqlDataReader sqlDataReader3 = cDbOvt6.RunSQLReturnRS(Msql3);
                                if (sqlDataReader3.Read())
                                    str14 = "" + sqlDataReader3["CodCliche"].ToString();
                                if (sqlDataReader3 != null)
                                {
                                    sqlDataReader3.Close();
                                    cDbOvt6.CerrarConexion();
                                    sqlDataReader3.Dispose();
                                }
                                if (flag4)
                                    str14 = "Multicaída";
                                stiReport.Dictionary.Variables["CLICHE"].Value = str14;
                            }
                            List<StiVariable> list = stiReport.Dictionary.Variables.ToList();
                            bool flag5 = false;
                            foreach (StiVariable stiVariable17 in list)
                            {
                                if (stiVariable17.Name.ToUpper() == "MOTIVOS")
                                {
                                    flag5 = true;
                                    break;
                                }
                            }
                            StiVariable stiVariable18 = new StiVariable("MOTIVOS", typeof(string));
                            if (flag5 && num6 == 1)
                            {
                                if (num4 != Decimal.Zero && str16 == "Metros")
                                {
                                    try
                                    {
                                        stiVariable18.Value = "I.Aprx " + Decimal.Multiply(System.Convert.ToDecimal(str7) / (num4 / new Decimal(1000)), Decimal.One).ToString("##,##0.00");
                                        stiReport.Dictionary.Variables["Motivos"].Value = "I.Aprx " + Decimal.Multiply(System.Convert.ToDecimal(str7) / (num4 / new Decimal(1000)), Decimal.One).ToString("##,##0.00");
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                            StiVariable stiVariable19 = new StiVariable("SegundaEtiqueta", typeof(string));
                            StiVariable variable = stiReport.Dictionary.Variables["SegundaEtiqueta"];
                            new PrinterSettings().PrinterName = "BOBINAS";
                            reports.Add(stiReport);
                            if (variable.Value.ToString().Trim() != "-")
                            {
                                PrinterSettings printerSettings = new PrinterSettings();
                                try
                                {
                                    printerSettings.PrinterName = "ETIQUETA_DOS";
                                    string path2 = Path.Combine("\\\\Olanet\\etiquetas", variable.Value + ".mrt");
                                    if (File.Exists(path2))
                                    {
                                        stiReport.Load(path2);
                                        stiReport.Render();
                                        StiVariable stiVariable17 = new StiVariable("parEAN", typeof(string));
                                        StiVariable stiVariable20 = new StiVariable("parCodigoLote", typeof(string));
                                        StiVariable stiVariable21 = new StiVariable("parSuPedido", typeof(string));
                                        stiVariable20.Value = str6;
                                        stiVariable17.Value = str15;
                                        stiVariable21.Value = str11;
                                        stiReport.Dictionary.Variables["parEAN"] = stiVariable17;
                                        stiReport.Dictionary.Variables["parCodigoLote"] = stiVariable20;
                                        stiReport.Dictionary.Variables["parSuPedido"] = stiVariable21;
                                        stiReport.Compile();
                                        stiReport.Render();
                                        stiReport.Print(true, printerSettings);
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string Msql3 = "INSERT INTO ERROR_ETIQUETAS(mensaje,traza) VALUES('" + ex.Message + "','" + ex.StackTrace + "')";
                        if (cDbOvt1.EjecutarSQL(Msql3))
                            ;
                    }
                }
                sqlDataReader9.Close();
                cDbOvt5.CerrarConexion();
            }
            if (flag1)
                ThreadPool.QueueUserWorkItem((WaitCallback)(_ =>
                {
                    StiReport stiReport1 = (StiReport)null;
                    if (reports.Count == 1)
                    {
                        stiReport1 = reports[0];
                        stiReport1.Render();
                    }
                    else
                    {
                        foreach (StiReport stiReport2 in reports)
                        {
                            stiReport2.Render();
                            if (stiReport1 == null)
                            {
                                stiReport1 = stiReport2;
                            }
                            else
                            {
                                foreach (StiPage renderedPage in (CollectionBase)stiReport2.RenderedPages)
                                {
                                    renderedPage.Report = stiReport1;
                                    stiReport1.RenderedPages.Add(renderedPage);
                                }
                            }
                        }
                    }
                    PrinterSettings printerSettings1 = new PrinterSettings();
                    printerSettings1.PrinterName = this.ImpresoraDefaultBobinaStim();
                    printerSettings1.DefaultPageSettings.PaperSize = new PaperSize("CUSTOM", 397, 216);
                    try
                    {
                        string str = miSeccion;
                        if (!(str == "IMPRESION"))
                        {
                            if (str == "LAMINADO")
                                //stiReport1.Print(false, 1, 999, (short)2, printerSettings1);
                                stiReport1.Print(true, 1, 999, (short)2, printerSettings1);
                            else
                                //stiReport1.Print(false, printerSettings1);
                                stiReport1.Print(true, printerSettings1);
                        }
                        else
                            //stiReport1.Print(false, 1, 999, (short)3, printerSettings1);
                            stiReport1.Print(true, 1, 999, (short)3, printerSettings1);
                    }
                    catch (Exception ex)
                    {
                        PrinterSettings printerSettings2 = new PrinterSettings();
                        //stiReport1.Print(false, printerSettings2);
                        stiReport1.Print(true, printerSettings2);
                    }
                }));
            sqlDataReader1.Close();
            cDbOvt1.CerrarConexion();
            sqlDataReader1.Dispose();
            return true;
        }

        /*private string ImpresoraPuerto(string pImpresora)
        {
            string queryString = string.Format("SELECT * from Win32_Printer WHERE Name LIKE '%{0}'", (object)pImpresora);
            string str = "";
            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString))
            {
                using (ManagementObjectCollection objectCollection = managementObjectSearcher.Get())
                {
                    try
                    {
                        foreach (ManagementBaseObject managementBaseObject in objectCollection)
                        {
                            foreach (PropertyData property in managementBaseObject.Properties)
                            {
                                if (property.Name == "PortName")
                                    str = (string)property.Value;
                            }
                        }
                    }
                    catch (ManagementException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return str;
        }*/

        private string ImpresoraDefaultBobinaStim()
        {
            if (this.vLocal == "S")
                return "\\\\PCSOS\\BOBINAS";
            cDBOvt cDbOvt = new cDBOvt();
            string str = "";
            SqlDataReader sqlDataReader = cDbOvt.RunSQLReturnRS("SELECT * FROM dbo._OVT_OLANET_IMPTERMINAL WHERE  TERMINAL_ID='" + this.vTerminal + "'");
            if (sqlDataReader.Read())
                str = sqlDataReader["IMPRESOR_BOBINA"].ToString().ToUpper();
            sqlDataReader.Close();
            cDbOvt.CerrarConexion();
            return str;
        }

        private double dConversion(string pArticulo, double pCantidadOriginal, ref int pTipo)
        {
            cDBOvt cDbOvt1 = new cDBOvt();
            SqlDataReader sqlDataReader1 = cDbOvt1.RunSQLReturnRS("SELECT TOP 1 CODUNID,CODUNIO,FACTOR,OPFACTOR,OPSUMA,SUMA FROM dbo.ALCONVMEDART WHERE CODEMP='001' AND CODART='" + pArticulo + "' AND CODUNID='BOLSA' ");
            pTipo = 0;
            double num1 = 0.0;
            if (sqlDataReader1.Read())
            {
                pTipo = 1;
                double num2 = !(sqlDataReader1["opfactor"].ToString() == "/") ? pCantidadOriginal * System.Convert.ToDouble(sqlDataReader1["factor"].ToString()) : pCantidadOriginal / System.Convert.ToDouble(sqlDataReader1["factor"].ToString());
                num1 = !(sqlDataReader1["OPSUMA"].ToString() == "+") ? num2 - System.Convert.ToDouble(sqlDataReader1["suma"].ToString()) : num2 + System.Convert.ToDouble(sqlDataReader1["suma"].ToString());
            }
            sqlDataReader1.Close();
            cDbOvt1.CerrarConexion();
            if (num1 == 0.0)
            {
                cDBOvt cDbOvt2 = new cDBOvt();
                SqlDataReader sqlDataReader2 = cDbOvt2.RunSQLReturnRS("SELECT TOP 1 CODUNID,CODUNIO,FACTOR,OPFACTOR,OPSUMA,SUMA FROM dbo.ALCONVMEDART WHERE CODEMP='001' AND CODART='" + pArticulo + "' AND CODUNID='KG' ");
                num1 = 0.0;
                if (sqlDataReader2.Read())
                {
                    pTipo = 2;
                    double num2 = !(sqlDataReader2["opfactor"].ToString() == "/") ? pCantidadOriginal * System.Convert.ToDouble(sqlDataReader2["factor"].ToString()) : pCantidadOriginal / System.Convert.ToDouble(sqlDataReader2["factor"].ToString());
                    num1 = !(sqlDataReader2["OPSUMA"].ToString() == "+") ? num2 - System.Convert.ToDouble(sqlDataReader2["suma"].ToString()) : num2 + System.Convert.ToDouble(sqlDataReader2["suma"].ToString());
                }
                sqlDataReader2.Close();
                cDbOvt2.CerrarConexion();
            }
            return num1;
        }

        private string fFile(string pArticulo, string pCliente)
        {
            if (File.Exists("\\\\Olanet\\etiquetas\\AC_" + pArticulo + "_" + pCliente + ".mrt"))
                return "\\\\Olanet\\etiquetas\\AC_" + pArticulo + "_" + pCliente + ".mrt";
            if (File.Exists("\\\\Olanet\\etiquetas\\A_" + pArticulo + ".mrt"))
                return "\\\\Olanet\\etiquetas\\A_" + pArticulo + ".mrt";
            if (File.Exists("\\\\Olanet\\etiquetas\\C_" + pCliente + ".mrt"))
                return "\\\\Olanet\\etiquetas\\C_" + pCliente + ".mrt";
            return File.Exists("\\\\Olanet\\etiquetas\\STD.mrt") ? "\\\\Olanet\\etiquetas\\STD.mrt" : "";
        }

        private string fFileZebra(string pArticulo, string pCliente)
        {
            if (File.Exists("\\\\Olanet\\etiquetas\\ZEBRA\\AC_" + pArticulo + "_" + pCliente + ".zpl"))
                return "\\\\Olanet\\etiquetas\\ZEBRA\\AC_" + pArticulo + "_" + pCliente + ".zpl";
            if (File.Exists("\\\\Olanet\\etiquetas\\ZEBRA\\A_" + pArticulo + ".zpl"))
                return "\\\\Olanet\\etiquetas\\ZEBRA\\A_" + pArticulo + ".zpl";
            if (File.Exists("\\\\Olanet\\etiquetas\\ZEBRA\\C_" + pCliente + ".zpl"))
                return "\\\\Olanet\\etiquetas\\ZEBRA\\C_" + pCliente + ".zpl";
            return File.Exists("\\\\Olanet\\etiquetas\\ZEBRA\\EtiquetaSTD.zpl") ? "\\\\Olanet\\etiquetas\\ZEBRA\\EtiquetaSTD.zpl" : "";
        }

        private string fFilePalet(string pArticulo, string pCliente)
        {
            if (File.Exists("\\\\Olanet\\etiquetas\\PAC_" + pArticulo + "_" + pCliente + ".mrt"))
                return "\\\\Olanet\\etiquetas\\PAC_" + pArticulo + "_" + pCliente + ".mrt";
            if (File.Exists("\\\\Olanet\\etiquetas\\PA_" + pArticulo + ".mrt"))
                return "\\\\Olanet\\etiquetas\\PA_" + pArticulo + ".mrt";
            if (File.Exists("\\\\Olanet\\etiquetas\\PC_" + pCliente + ".mrt"))
                return "\\\\Olanet\\etiquetas\\PC_" + pCliente + ".mrt";
            return File.Exists("\\\\Olanet\\etiquetas\\PALETSTD.mrt") ? "\\\\Olanet\\etiquetas\\PALETSTD.mrt" : "";
        }
    }
}
