using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using PDSIEUCo = PDSImpresionEtiquetasUtils.Comun;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using static PDSImpresionEtiquetasUtils.Pantallas.panImpresionEtiquetaCOliveRueda_ViewModel;

namespace PDSImpresionEtiquetasUtils.Pantallas
{
    public class csGeneraEtiqRepro_Comun
    {        
        #region Impresion de etiquetas

        public void imprimirZPL (string ZPLCode)
        {
            System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
            pd.PrinterSettings = new PrinterSettings();
            string defaultPrinter = pd.PrinterSettings.PrinterName;
            pd.PrinterSettings.PrinterName = csEstadoPermanente.Configuracion.Datos.Impresora_Palets_GER01;
            if (!pd.PrinterSettings.IsValid) pd.PrinterSettings.PrinterName = defaultPrinter;
            if (DialogResult.OK == pd.ShowDialog())
            {
                RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, ZPLCode);
            }
        }
        public string GenerarPDFdesdeZPL (string ZPLCode)
        {
            /*byte[] zpl = Encoding.UTF8.GetBytes("^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR8,8~SD23^JUS^LRN^CI0^XZ^XA^MMT^PW1183^LL1662^LS0^FT381,1373^A0N,32,31^FH\\\\^FD(01)08480000933454(10)059505^FS^BY5,3,255^FT166,1330^BCN,,N,N^FD>" +
                ";>8010848000093345410059505^FS^FT40,954^A0N,39,38^FH\\^FDTotal Unidades^FS^FT40,907^A0N,39,38^FH\\^FDTotal Cajas^FS^FT446,1618^A0N,32,31^FH\\^FD(00)384360379402652750^FS^BY6,3,195^FT139,1581^BCN,,N,N^FD>;>800384360379402652750^FS" +
                "^FT10,47^A0N,34,33^FH\\^FDPLASTICOS DEL SEGURA^FS^FO12,387^GB1159,0,8^FS^FO9,299^GB1165,0,8^FS^FO8,156^GB1166,0,9^FS^FO7,94^GB1153,0,10^FS^FT12,152^A0N,39,38^FH\\^FDBOLSA PAPEL EMPANADA 500 GR (93345-1000-V01/18) PE^FS" +
                "^FT869,271^A0N,102,100^FH\\^FD059505^FS^FT741,225^A0N,51,50^FH\\^FDLOTE:^FS^FT11,216^A0N,45,45^FH\\^FDCOD.ART: 0133940^FS^FO713,909^GB439,107,4^FS^FO30,859^GB465,154,4^FS^FT340,998^A0N,39,38^FH\\^FD210,16^FS" +
                "^FT300,951^A0N,39,38^FH\\^FD40000^FS^FT379,905^A0N,39,38^FH\\^FD40^FS^FO35,953^GB460,0,6^FS^FO36,906^GB457,0,12^FS^FO266,862^GB0,148,5^FS^FT750,885^A0N,39,38^FH\\^FDFecha: 15/02/2019 15:33:55^FS^FT40,1000^A0N,39,38^FH\\^FDKgs Aprox.^FS" +
                "^FT338,360^A0N,39,38^FH\\^FDUds por Caja^FS^FT43,359^A0N,39,38^FH\\^FDN\\A7 de Cajas^FS^FT964,362^A0N,39,38^FH\\^FDTotal Unidades^FS^FT726,361^A0N,39,38^FH\\^FDKgs Aprox.^FS^FO1025,440^A0N,39,38^FH\\^FD     40000^FS" +
                "^FO750,440^A0n,39,38^FH\\^FD   210,16^FS^FO450,440^A0N,39,38^FH\\^FD       1000^FS^FO83,440^A0N,39,38^FH\\^FD           40^FS^PQ1,0,1,Y^XZ");*/

            byte[] zpl = Encoding.UTF8.GetBytes(ZPLCode);

            // adjust print density (8dpmm), label width (4 inches), label height (6 inches), and label index (0) as necessary
            var request = (HttpWebRequest)WebRequest.Create("http://api.labelary.com/v1/printers/8dpmm/labels/6x8/0/");
            request.Method = "POST";
            //request.Accept = "application/pdf"; // omit this line to get PNG images back
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = zpl.Length;

            var requestStream = request.GetRequestStream();
            requestStream.Write(zpl, 0, zpl.Length);
            requestStream.Close();

            string filename = "ETIQ"+DateTime.Now.ToString("ddMMyyyy_HHmm")+".png";
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                //var fileStream = File.Create("C:\\Temporales\\label.pdf"); // change file name for PNG images
                if (!Directory.Exists(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01))
                {
                    Directory.CreateDirectory(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01);
                }

                var fileStream = File.Create(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + filename); // change file name for PNG images
                responseStream.CopyTo(fileStream);
                responseStream.Close();
                fileStream.Close();
            }
            catch (WebException e)
            {
                Comun.Utilidades.csLogUtils.EscribeLineaLogError("Error: " + e.Status.ToString());
            }

            return filename;
        }


        public void ImprimeEtiquetaPalet(PrinterSettings p_impre_palet, csitem_EtiquetaGeneralCajaBobinaT1 p_Etiqueta, bool p_pantalla, bool p_dialogo_impresion, Dispatcher p_dispatcher = null)
        {

            StiReport joinedReport = new StiReport();
            joinedReport.NeedsCompiling = false;
            joinedReport.IsRendered = true;
            joinedReport.RenderedPages.Clear();

            for (int i = 0; i < p_Etiqueta.TotalEtiquetas; i++)
            {
                Stimulsoft.Report.StiReport report_palet = new Stimulsoft.Report.StiReport();

                if (File.Exists(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + "C_" + p_Etiqueta.CodCliente + ".mrt"))
                {
                    report_palet.Load(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + "C_" + p_Etiqueta.CodCliente + ".mrt");
                }
                else
                {
                    report_palet.Load(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + "C_GEN01.mrt");
                    //if (p_dialogo_impresion) report_palet.Load(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + "\\etiquetas_chinos\\P_GEN02.mrt");
                    //else report_palet.Load(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + "\\etiquetas_chinos\\P_GEN03.mrt");
                }

                report_palet.Compile();

                report_palet["SREF"] = p_Etiqueta.CodArticulo;
                report_palet["DESCRIPCION"] = p_Etiqueta.Descripcion;
                report_palet["LOTE"] = p_Etiqueta.Lote;
                
                report_palet["METROS"] = "Bolsas: " + p_Etiqueta.TotalUds;
                if (File.Exists(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + "C_" + p_Etiqueta.CodCliente + ".mrt"))
                {
                    report_palet["codigobarras"] = p_Etiqueta.Sscc.ElementAt(12 * i);
                    report_palet["codigobarras2"] = p_Etiqueta.Sscc.ElementAt(12 * i + 1);
                    report_palet["codigobarras3"] = p_Etiqueta.Sscc.ElementAt(12 * i + 2);
                    report_palet["codigobarras4"] = p_Etiqueta.Sscc.ElementAt(12 * i + 3);
                    report_palet["codigobarras5"] = p_Etiqueta.Sscc.ElementAt(12 * i + 4);
                    report_palet["codigobarras6"] = p_Etiqueta.Sscc.ElementAt(12 * i + 5);
                    report_palet["codigobarras7"] = p_Etiqueta.Sscc.ElementAt(12 * i + 6);
                    report_palet["codigobarras8"] = p_Etiqueta.Sscc.ElementAt(12 * i + 7);
                    report_palet["codigobarras9"] = p_Etiqueta.Sscc.ElementAt(12 * i + 8);
                    report_palet["codigobarras10"] = p_Etiqueta.Sscc.ElementAt(12 * i + 9);
                    report_palet["codigobarras11"] = p_Etiqueta.Sscc.ElementAt(12 * i + 10);
                    report_palet["codigobarras12"] = p_Etiqueta.Sscc.ElementAt(12 * i + 11);
                }else{
                    report_palet["codigobarras"] = p_Etiqueta.CodArticulo;
                }
                report_palet.Render();
                foreach (StiPage page in report_palet.CompiledReport.RenderedPages)
                {
                    page.Report = joinedReport;
                    page.NewGuid();
                    joinedReport.RenderedPages.Add(page);
                }

            }

            string defaultPrinter = p_impre_palet.PrinterName;
            p_impre_palet.PrinterName = csEstadoPermanente.Configuracion.Datos.Impresora_Palets_GER01;
            if (!p_impre_palet.IsValid) p_impre_palet.PrinterName = defaultPrinter;
            //DoImpresion(report_palet, p_impre_palet, p_pantalla, p_dialogo_impresion, p_dispatcher);
            DoImpresion(joinedReport, p_impre_palet, p_pantalla, true, p_dispatcher);
        }


        public void ImprimeEtiquetaPalet(PrinterSettings p_impre_palet, csitem_EtiquetaSiroT1 p_Etiqueta, bool p_pantalla, bool p_dialogo_impresion, Dispatcher p_dispatcher = null)
        {
            //PDSIEUCo.DB.DataBaseLayer b_db = new PDSIEUCo.DB.DataBaseLayer(csEstadoPermanente.Configuracion.Datos.connectionString_RPS2013_OLANET);

                
            //string b_informe = @csEstadoPermanente.Configuracion.Datos.Ruta_informe_GER01_Palet01;
            //b_informe = DameFormatoEtiquetaPalet(b_dato.CodArticulo, b_dato.CodCliente);
            //if (string.IsNullOrWhiteSpace(b_informe)) b_informe = @csEstadoPermanente.Configuracion.Datos.Ruta_informe_GER01_Palet01;

            Stimulsoft.Report.StiReport report_palet = new Stimulsoft.Report.StiReport();
            //report_palet.Load(@b_informe);
            //report_palet.Load(@"C:\Temporales\PALET_CGRUPOSIRO_T1.mrt");
            report_palet.Load(csEstadoPermanente.Configuracion.Datos.Ruta_informe_Palet_CGrupoSiroT1);
            report_palet.Compile();

            report_palet["NumProveedor"] = p_Etiqueta.NumProveedor;
            report_palet["Proveedor"] = p_Etiqueta.Proveedor;
            report_palet["PedidoGrupoSiro"] = p_Etiqueta.PedidoGrupoSIRO;
            report_palet["Lote"] = p_Etiqueta.Lote;
            report_palet["ReferenciaSIRO"] = p_Etiqueta.ReferenciaSIRO;
            report_palet["Descripcion"] = p_Etiqueta.DescripcionReferencia;
            report_palet["PickupSheet"] = p_Etiqueta.PickupSheet;
            report_palet["Cantidad"] = p_Etiqueta.Cantidad;
            report_palet["Unidad"] = p_Etiqueta.Unidad ;
            report_palet["Volumen"] = p_Etiqueta.Volumen;
            report_palet["Peso"] = p_Etiqueta.Peso; 
            report_palet["FechaEntrega"] = p_Etiqueta.FechaEntrega.Substring(0,10); 
            report_palet["FechaFabricacion"] = p_Etiqueta.FechaFabricacion.Substring(0, 10);
            report_palet["CuentaPaletAlbaran"] = p_Etiqueta.CuentaPaletAlbaran;
            report_palet["Observaciones"] = p_Etiqueta.Observaciones;
            report_palet["FabricaDestino"] = p_Etiqueta.FabricaDestino;

            report_palet.Render();

            string defaultPrinter = p_impre_palet.PrinterName;
            p_impre_palet.PrinterName = csEstadoPermanente.Configuracion.Datos.Impresora_Palets_GER01;
            if (!p_impre_palet.IsValid) p_impre_palet.PrinterName = defaultPrinter;
            DoImpresion(report_palet, p_impre_palet, p_pantalla, p_dialogo_impresion, p_dispatcher);
        }

        public void ImprimeEtiquetaPalet(PrinterSettings p_impre_palet, csItem_EtiquetaChinos p_Etiqueta, bool p_pantalla, bool p_dialogo_impresion, Dispatcher p_dispatcher = null)
        {

            long num0 = 0;
            if (long.TryParse(p_Etiqueta.SSCC, out long aux_long)) num0 = aux_long;

            StiReport joinedReport = new StiReport();
            joinedReport.NeedsCompiling = false;
            joinedReport.IsRendered = true;
            joinedReport.RenderedPages.Clear();

            for (int int32 = System.Convert.ToInt32(p_Etiqueta.NumDesde); int32 <= System.Convert.ToInt32(p_Etiqueta.NumHasta); ++int32)
            {
                Stimulsoft.Report.StiReport report_palet = new Stimulsoft.Report.StiReport();

                if (File.Exists(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + "\\etiquetas_chinos\\P" + p_Etiqueta.CodArticulo + ".mrt"))
                {
                    report_palet.Load(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + "\\etiquetas_chinos\\P" + p_Etiqueta.CodArticulo + ".mrt");
                } else
                {
                    if (p_dialogo_impresion) report_palet.Load(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + "\\etiquetas_chinos\\P_GEN02.mrt");
                    else report_palet.Load(csEstadoPermanente.Configuracion.Datos.Ruta_imagenes_GER01_Palet01 + "\\etiquetas_chinos\\P_GEN03.mrt");
                }

                report_palet.Compile();

                long num1 = num0 + int32 * 4L;
                long num2 = num1 + 1L;
                long num3 = num2 + 1L;
                long num4 = num3 + 1L;

                if (p_dialogo_impresion) report_palet["EAN13"] = p_Etiqueta.EAN13;
                report_palet["CodArticulo"] = p_Etiqueta.CodArticulo;
                report_palet["Descripcion"] = p_Etiqueta.Descripcion;
                report_palet["Lote"] = p_Etiqueta.Lote;
                foreach (csItem_ListaLineasGridEtiqueta item in p_Etiqueta.ListaLineasGridEtiqueta)
                {
                    report_palet["NumCajas"] += item.NumCajas.ToString() + "\r";
                    report_palet["UdsPorCaja"] += item.UdsPorCaja.ToString() + "\r";
                    report_palet["TotalUds"] += item.TotalUds.ToString() + "\r";
                }
                report_palet["Variable1"] = num1.ToString("000000000");
                report_palet["Variable2"] = num2.ToString("000000000");
                report_palet["Variable3"] = num3.ToString("000000000");
                report_palet["Variable4"] = num4.ToString("000000000");

                report_palet.Render();
                foreach(StiPage page in report_palet.CompiledReport.RenderedPages)
                {
                    page.Report = joinedReport;
                    page.NewGuid();
                    joinedReport.RenderedPages.Add(page);
                }

            }

            //StiReportResponse.ResponseAsPdf(this, joinedReport, false);

            string defaultPrinter = p_impre_palet.PrinterName;
            p_impre_palet.PrinterName = csEstadoPermanente.Configuracion.Datos.Impresora_Palets_GER01;
            if (!p_impre_palet.IsValid) p_impre_palet.PrinterName = defaultPrinter;
            //DoImpresion(report_palet, p_impre_palet, p_pantalla, p_dialogo_impresion, p_dispatcher);
            DoImpresion(joinedReport, p_impre_palet, p_pantalla, true, p_dispatcher);
        }

        public void ImprimeEtiquetaPalet(PrinterSettings p_impre_palet, csitem_EtiquetaEstiuT1 p_Etiqueta, bool p_pantalla, bool p_dialogo_impresion, Dispatcher p_dispatcher = null)
        {
            
            Stimulsoft.Report.StiReport report_palet = new Stimulsoft.Report.StiReport();
            report_palet.Load(csEstadoPermanente.Configuracion.Datos.Ruta_informe_Palet_CEstiu);
            report_palet.Compile();

            report_palet["AlbaranPDS"] = p_Etiqueta.AlbaranPDS;
            report_palet["CodArticulo"] = p_Etiqueta.CodArticulo;
            report_palet["Descripcion"] = p_Etiqueta.Descripcion;
            report_palet["Lote"] = p_Etiqueta.Lote;
            foreach (csItem_ListaLineasGridEtiquetaEstiu item in p_Etiqueta.ListaLineasGridEtiqueta)
            {
                report_palet["NumBobinas"] += item.NumBobinas.ToString() +"\r";
                report_palet["MetrosBobina"] += item.MetrosPorBobina.ToString() + "\r";
                report_palet["TotalMetros"] += item.TotalMetros.ToString() + "\r";
            }
            report_palet["PedidoCliente"] = p_Etiqueta.PedidoCliente;
            report_palet["PesoNetoSMandril"] = p_Etiqueta.PesoNetoSMandril.ToString();
            report_palet["PesoBrutoPaletizado"] = p_Etiqueta.PesoBrutoPaletizado.ToString();
            report_palet["PesoPalet"] = p_Etiqueta.PesoPalet.ToString();
            report_palet["PesoNetoPaletizado"] = p_Etiqueta.PesoNetoPaletizado.ToString();
            report_palet["NumPalet"] = p_Etiqueta.NumPalet;

            report_palet.Render();

            string defaultPrinter = p_impre_palet.PrinterName;
            p_impre_palet.PrinterName = csEstadoPermanente.Configuracion.Datos.Impresora_Bobinas_GER01;
            if (!p_impre_palet.IsValid) p_impre_palet.PrinterName = defaultPrinter;
            DoImpresion(report_palet, p_impre_palet, p_pantalla, p_dialogo_impresion, p_dispatcher);
        }
        public delegate void DoImpresion_Callback(Stimulsoft.Report.StiReport p_report, PrinterSettings p_printersettings, bool p_pantalla, bool p_dialogo_impresion, Dispatcher p_dispatcher);
        private void DoImpresion(Stimulsoft.Report.StiReport p_report, PrinterSettings p_printersettings, bool p_pantalla, bool p_dialogo_impresion, Dispatcher p_dispatcher)
        {
            Dispatcher b_dispatcher = p_dispatcher;
            if (p_dispatcher == null)
            {
                b_dispatcher = Dispatcher.CurrentDispatcher;
            }

            if (!b_dispatcher.CheckAccess())
            {
                DoImpresion_Callback d = new DoImpresion_Callback(DoImpresion);
                b_dispatcher.Invoke(d, p_report, p_printersettings, p_pantalla, p_dialogo_impresion, p_dispatcher);
            }
            else
            {

                IDisposable d = null;

                try
                {
                    if (p_pantalla)
                    {
                        p_report.Show();
                    }
                    else
                    {
                        p_report.Print(p_dialogo_impresion, p_printersettings);
                    }
                }
                catch (Exception ex1)
                {
                    PDSIEUCo.Utilidades.csLogUtils.EscribeLineaLogError("DoImpresion", ex1);
                }
                finally
                {
                    if (d != null) d.Dispose();
                }
            }
        }

        private string DameFormatoEtiquetaBobina(string p_CodArticulo, string p_CodCliente)
        {
            if (System.IO.File.Exists(@"\\Olanet\etiquetas\AC_" + p_CodArticulo + "_" + p_CodCliente + ".mrt"))
            {
                return (@"\\Olanet\etiquetas\AC_" + p_CodArticulo + "_" + p_CodCliente + ".mrt");

            }

            if (System.IO.File.Exists(@"\\Olanet\etiquetas\A_" + p_CodArticulo + ".mrt"))
            {
                return (@"\\Olanet\etiquetas\A_" + p_CodArticulo + ".mrt");

            }



            if (System.IO.File.Exists(@"\\Olanet\etiquetas\C_" + p_CodCliente + ".mrt"))
            {
                return (@"\\Olanet\etiquetas\C_" + p_CodCliente + ".mrt");

            }
            if (System.IO.File.Exists(@"\\Olanet\etiquetas\" + "STD" + ".mrt"))
            {
                return (@"\\Olanet\etiquetas\" + "STD" + ".mrt");

            }
            return ("");
        }

        private string DameFormatoEtiquetaPalet(string p_CodArticulo, string p_CodCliente)
        {
            string b_fichero = "";

            b_fichero = @"\\Olanet\etiquetas\PALETSTD-UTILS-AC_" + p_CodArticulo + "_" + p_CodCliente + ".mrt";
            if (System.IO.File.Exists(b_fichero))
            {
                return (b_fichero);
            }

            b_fichero = @"\\Olanet\etiquetas\PALETSTD-UTILS-A_" + p_CodArticulo + ".mrt";
            if (System.IO.File.Exists(b_fichero))
            {
                return (b_fichero);
            }


            b_fichero = @"\\Olanet\etiquetas\PALETSTD-UTILS-C_" + p_CodCliente + ".mrt";
            if (System.IO.File.Exists(b_fichero))
            {
                return (b_fichero);
            }

            b_fichero = @"\\Olanet\etiquetas\" + "PALETSTD-UTILS-STD" + ".mrt";
            if (System.IO.File.Exists(b_fichero))
            {
                return (b_fichero);
            }

            return ("");
        }

        #endregion
    }

    public class RawPrinterHelper
    {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }
        public static bool SendTextFileToPrinter(string szFileName, string printerName)
        {
            var sb = new StringBuilder();

            using (var sr = new StreamReader(szFileName, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    sb.AppendLine(sr.ReadLine());
                }
            }

            return RawPrinterHelper.SendStringToPrinter(printerName, sb.ToString());
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
    }
}
