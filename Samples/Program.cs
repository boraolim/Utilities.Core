namespace TestConsole
{
  using System;
  using System.IO;
  using System.Text;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;

  using Utilities;

  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.Configuration.FileExtensions;
  using Microsoft.Extensions.Configuration.Json;

  public class mtClases
  {
    public int Id_clase;
    public int Id_tipoact;
    public string Descripcion;
    public bool IsDeleted;
    public DateTime Fech_alt;
    public DateTime? Fech_act;
  }

  public class ListaBD
  {
    public string Database { get; set;}
  }

  public class Program
  {
    private static DateTime _oScheduledTime = DateTime.Now;
    private static DateTime _oTime0;
    private static DateTime _oTime1;
    private static TimeSpan _oTimeTotal;
    private static StringBuilder _oSb = null;
    private static string strFileNameCSV = string.Concat(Tool.RandomString(15), ".csv");

    protected static string _strODBCString = string.Empty;
    protected static string server = "";
    protected static string port = "";
    protected static string masterUsername = "";
    protected static string masterUserPassword = "";
    protected static string DBName = "";

    [STAThread()]
    static void Main(string[] args)
    {
      try
      {
        GlobalApp.oLog = new LOGFiles(GlobalApp.FolderLog.Trim(), "TestConsole", "Program.cs", "Main", "TestConsole", AssemblyInfo.Company.Trim());
        GlobalApp.DetailLog = new List<DetailLOG>(); GlobalApp.iCount = 1; GlobalApp.Numero = 0; GlobalApp.Mensaje = string.Empty; GlobalApp.FechaActual = DateTime.Now; _oTime0 = DateTime.Now;
        if (Directory.Exists(GlobalApp.FolderLog.Trim()) == false) { Directory.CreateDirectory(GlobalApp.FolderLog.Trim()); }                  // Carpeta de los archivos LOG de la aplicación.
        if (Directory.Exists(GlobalApp.FolderRpt.Trim()) == false) { Directory.CreateDirectory(GlobalApp.FolderRpt.Trim()); }                  // Carpeta de archivo de reportes finales.
        if (Directory.Exists(GlobalApp.FolderLayOut.Trim()) == false) { Directory.CreateDirectory(GlobalApp.FolderLayOut.Trim()); }            // Carpeta de archivo de layout.

        Console.WriteLine("Consola de aplicación en .NET Core.\nVersión " + AssemblyInfo.Version.ToString());
        Console.WriteLine("México " + DateTime.Now.Year.ToString() + ".\n");

        Console.WriteLine("Fecha de inicio: " + _oTime0.ToString());

        GlobalApp.DetailLog.Add(new DetailLOG()
        {
          Id = GlobalApp.iCount++,
          Fecha = DateTime.Now.ToString("yyyy'/'MM'/'dd' 'hh':'mm':'ss'.'fff' 'tt"),
          TipoEvento = TipoInformacion.Informacion,
          Numero = 0,
          Comentario = "Fecha de inicio: " + _oTime0.ToString()
        });

        // Inicializamos las variables.
        InitVars();

        Console.WriteLine("Fecha universal: {0}", Tool.ToDateUniversal(DateTime.Now));

        var _ValoraCifrar = "Que_Chingue_A_Su_Mother_AMLO_Y_EL_AMERICA";
        var _strNewGUID = Guid.NewGuid().ToString();
        Console.WriteLine("Valor a cifrar: {0}. Valor cifrado: {1}.", _ValoraCifrar, RijndaelManagedEncryption.EncryptRijndael(_ValoraCifrar, _strNewGUID));
        Console.WriteLine("Valor desencriptado: {0}.", RijndaelManagedEncryption.DecryptRijndael(RijndaelManagedEncryption.EncryptRijndael(_ValoraCifrar, _strNewGUID), _strNewGUID));

        // Cargando el archivo de configuración de la aplicación 'appsettings.json'.
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfigurationRoot configuration = builder.Build();

        // Leemos algunos de sus valores de la configuración,.
        string dbConn = configuration.GetSection("ConnectionStrings").GetSection("SQLServerConnectionBD").Value;
        string dbConn2 = configuration.GetSection("ConnectionStrings").GetSection("MariaDBConnectionBD").Value;  

        // Limpiamos esta variable.
        configuration = null;

        Console.WriteLine("Cadena de conexión a Microsoft SQL Server: {0}", dbConn);
        Console.WriteLine("Cadena de conexión a MariaDB/MySQL Server: {0}", dbConn2);

        Console.WriteLine("Fecha universal: {0}", Tool.ToDateUniversal(DateTime.Now));

        Console.WriteLine("Haciendo una consulta SQL a Base de Datos...");
        _oSb.Clear().AppendFormat("SHOW DATABASES;");

        var s = MySQLConnectionDB.GetDataToMapping<ListaBD>(dbConn2, _oSb.ToString(), 60, 5).ToList();
        Console.WriteLine("Total de registros cargados: {0}.", s.Count);

        // Recorremos la lista.
        Console.WriteLine("Leyendo la lista de Bases de Datos.");
        
        foreach (var u in s)
          Console.WriteLine("{0}", u.Database.Trim());

        var _strArchivoCredenciales = Path.Combine(GlobalApp.FolderCredentialsGoogleAPI, "credentials_sheets.json");

        // Guardamos la lista de bases de datos en un archivo CSV.
        Console.WriteLine("Generando reporte de bases de datos...");       
        Tool.ListToCSV(s, Path.Combine(GlobalApp.FolderRpt, strFileNameCSV), ",");
        Console.WriteLine("El reporte se ha generado correctamente en {0}", Path.Combine(GlobalApp.FolderRpt, strFileNameCSV));

        var strDato = "CapMax = ((DepositoConceptoJubilacionPension - CargosDomiciliados - CargosConceptoCreditos) * 0.4);nnCapMax=n((DepositoConceptoJubilacionPension - CargosDomiciliados - CargosConceptoCreditos) - CapMax) n> (NúmeroCasasComerciales < 2 ? 400 : 800)? CapMax : (NúmeroCasasComerciales < 2 ? 400 : 800);nnCapMax = CapMax > 0 ? CapMax : 0;tnnCapMax = NúmeroCasasComerciales >= 3 ? 0 : CapMax;";
        Console.WriteLine("Dato anterior: {0}", strDato);
        strDato = Regex.Replace(strDato, Patrones.PatronAlphaLatino.Trim(), string.Empty);
        Console.WriteLine("Dato nuevo: {0}", strDato);
      
        // Conexión a Amazon Redshift.
        // Cargo la cadena de conexión a la Base de Datos de Amazon Redshift.
        Console.WriteLine("{0} | Iniciando una conexión a Amazon Redshift...", DateTime.Now);
        _strODBCString = string.Concat("Driver={Amazon Redshift (x64)};",
                                       string.Format("Server={0};Port={4};Database={1};UID={2};PWD={3};",
                                       server, DBName, masterUsername, masterUserPassword, port));

        // Armado de la consulta SQL.
        _oSb.Clear().AppendFormat("SELECT CASE WHEN t1.domiciliacion = 1 THEN 'Domi' ELSE 'DxN' END AS \"Tipo_Descuento\", ");
        _oSb.AppendFormat("CASE t1.KeyTipoSolicitud WHEN 'LiqP' THEN 'Saneamiento' WHEN 'CfA' THEN 'Refinanciamiento' ELSE 'Tradicional' END AS \"Tipo_Producto\", ");
        _oSb.AppendFormat("CASE t1.Keycampana WHEN 'ConfLP' THEN 'Liquidacion de Pasivos' WHEN 'ConfLP2' THEN 'Liquidacion de Pasivos V2' ELSE CASE WHEN t1.IdCita IS NOT NULL THEN 'Cita' ELSE CASE WHEN t7.IdTemp IS NOT NULL THEN 'Conexión A+' ELSE 'Originacion Normal' END END END AS \"Tipo_Venta\", ");
        _oSb.AppendFormat("t1.FechaUltimaM::timestamp as \"Fecha_Ultima_Modificacion\", t1.IdSolicitud as \"Transaccion\", nvl(t7.IdTemp, 0)::int AS \"PreSolicitud\", Case When (t1.Folio != '-' or t1.Folio is null) then 'Cheque' else case when (t1.keybanco != '-' or t1.keybanco is null) then 'Transferencia' else '' end end AS \"Tipo_de_Pago\", ");
        _oSb.AppendFormat("case when (t1.Folio = '-') then null else t1.Folio end AS \"Numero_de_Cheque\", Case when (t1.keybanco = '-') then null else t1.descBanco end AS \"Banco\", ");
        _oSb.AppendFormat("Case when (t1.keybanco = '-') then null else  t1.CLABE end AS \"CLABE\", Case when (t1.keybanco = '-') then null else t1.CTA end AS \"Cuenta\", ");
        _oSb.AppendFormat("round(t1.MontoCheque, 2) AS \"Monto_Cheque\", round(t1.MontoPagare, 2) AS \"Monto_Pagare\", round((t1.MontoCheque + t1.MontoChequeComision), 2) AS \"MontoCapital\", ");
        _oSb.AppendFormat("round(t1.MontoArticulo, 2) AS \"MontoArticulo\", round(t1.Descuento, 2) AS \"Descuento\", t1.Nombre as \"Nombre_Cliente\", t6.RFC as \"RFC_Cliente\", t3.DescEstatus as \"Estatus_Transaccion\", ");
        _oSb.AppendFormat("Case When t8.dictamen = 0 then 'En Validación Telefónica' else t8.DescDictamen end AS \"Estatus_Validacion_Telefonica\", t9.DescValEspecial AS \"Estatus_Validacion_Especial\", ");
        _oSb.AppendFormat("t10.KeyConvenio AS \"Clave_ORUS_Zell\", btrim(t10.id) + '-' + t10.DescConvenio AS \"Convenio\", t5.DescUsuario AS \"Vendedor\", t5.Nomina AS \"Num_Nomina_Vendedor\", ");
        _oSb.AppendFormat("t1.Vendedor, t2.fecha::timestamp AS \"Fecha_Captura\", t4.fecha::timestamp AS \"Fecha_Envio_Mesa\", case when (t1.KeyMedioCaptacion = 'wsAdminSolV2') then 'V.2.0' else null end AS \"Medio_Captacion\", ");
        _oSb.AppendFormat("getdate()::timestamp at time zone 'UTC' at time zone 'America/Mexico_City' AS \"Fecha_Actualizacion\" ");
        _oSb.AppendFormat("FROM public.\"schema-sosolicitudes-keops\" t1  INNER JOIN public.\"schema-soidsolicitudes-keops\" t2 on (t2.idsolicitud = t1.idsolicitud)  ");
        _oSb.AppendFormat("INNER JOIN public.\"schema-segusuarios-keops\" t5 ON (t1.Vendedor = t5.KeyUsuario) ");
        _oSb.AppendFormat("LEFT JOIN public.\"schema-soestatussolicitudes-keops\" t3 ON (t3.KeyEstatus = t1.KeyEstatusUltimaM) ");
        _oSb.AppendFormat("LEFT JOIN ( SELECT _t4.IdSolicitud, MAX(_t4.fecha::timestamp) AS \"fecha\" FROM public.\"schema-sohistoriasolicitudes-keops\" _t4 WHERE (_t4.estatus = 'Abi') GROUP BY _t4.IdSolicitud ) t4 ON (t4.idsolicitud = t1.idsolicitud) ");
        _oSb.AppendFormat("LEFT JOIN public.\"schema-ventasclientes-keops\" t6 ON (t6.idcliente = t1.KeyCliente) ");
        _oSb.AppendFormat("LEFT JOIN public.\"schema-ventasclientestmp-keops\" t7 ON (t7.IdSolicitud = t1.IdSolicitud) ");
        _oSb.AppendFormat("LEFT JOIN public.\"schema-validaciontelefonica-keops\" t8 ON (t8.idsolicitud = t1.IdSolicitud) ");
        _oSb.AppendFormat("LEFT JOIN public.\"schema-catvalespecial-keops\" t9 ON (t9.keyValEspecial = t1.ValEspecial) ");
        _oSb.AppendFormat("LEFT JOIN public.\"schema-convdatosgenerales-keops\" t10 ON (t1.keyexpediente = t10.id) WHERE (t2.fecha::date between '20190601' and '20190630') ORDER BY t1.IdSolicitud desc;");                                       

        var ssResult = ODBCConnectionDB.GetDataTojqGridJSON(_strODBCString, _oSb.ToString(), 60, 5, "Prueba");
        Console.WriteLine("{0} | Resultados traidos correctamente desde Amazon Redshift...", DateTime.Now);

        // Exportamos el CSV.
        Console.WriteLine("{0} | Exportando desde Amazon Redshift...", DateTime.Now);
        var ArchivoReporteStr = Path.Combine(GlobalApp.FolderRpt.Trim(), string.Format("{0}.txt", Guid.NewGuid().ToString()));
        ODBCConnectionDB.ExportData(_strODBCString, _oSb.ToString(), 60, 5, ArchivoReporteStr, "|");
        Console.WriteLine("{0} | Exportación realizada correctamente.", DateTime.Now);

        ArchivoReporteStr = Path.Combine(GlobalApp.FolderRpt.Trim(), string.Format("{0}.csv", Guid.NewGuid().ToString()));
        ODBCConnectionDB.ExportData(_strODBCString, _oSb.ToString(), 60, 5, ArchivoReporteStr, ",");
        Console.WriteLine("{0} | Otra exportación realizada correctamente.", DateTime.Now);

        ArchivoReporteStr = Path.Combine(GlobalApp.FolderRpt.Trim(), string.Format("{0}.dat", Guid.NewGuid().ToString()));
        ODBCConnectionDB.ExportData(_strODBCString, _oSb.ToString(), 60, 5, ArchivoReporteStr, ",");
        Console.WriteLine("{0} | Otra exportación realizada correctamente.", DateTime.Now);

        ArchivoReporteStr = Path.Combine(GlobalApp.FolderRpt.Trim(), string.Format("{0}.dat", Guid.NewGuid().ToString()));
        ODBCConnectionDB.ExportData(_strODBCString, _oSb.ToString(), 60, 5, ArchivoReporteStr, "|");
        Console.WriteLine("{0} | Otra exportación realizada correctamente.", DateTime.Now);

        ArchivoReporteStr = Path.Combine(GlobalApp.FolderRpt.Trim(), string.Format("{0}.csv", Guid.NewGuid().ToString()));
        ODBCConnectionDB.ExportDataToGoogleSheets(_strODBCString, _oSb.ToString(), 60, 5, ArchivoReporteStr, ",",
                                                  _strArchivoCredenciales, GlobalApp.FolderPersonal, "AlphaReports2.0");
        Console.WriteLine("{0} | Se ha subido el set de datos a Google Drive correctamente. URL: {1}", DateTime.Now, ODBCConnectionDB.URLSourceDrive);

        ArchivoReporteStr = Path.Combine(GlobalApp.FolderRpt.Trim(), string.Format("{0}.csv", Guid.NewGuid().ToString()));
        ODBCConnectionDB.ExportDataToGoogleSheets(_strODBCString, _oSb.ToString(), 60, 5, ArchivoReporteStr, ",",
                                                  _strArchivoCredenciales, GlobalApp.FolderPersonal, "AlphaReports2.0", "134Q1gVp8QdlXeeedwlo3n8PuPszB0MIk");
        Console.WriteLine("{0} | Se ha subido el set de datos a un recurso de Google Drive correctamente. URL: {1}", DateTime.Now, ODBCConnectionDB.URLSourceDrive);
                              
        _oSb = null;
      }
      catch(Exception oEx)
      {
        GlobalApp.Numero = 100; GlobalApp.Mensaje = string.Concat(((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
        GlobalApp.DetailLog.Add(new DetailLOG()
        {
          Id = GlobalApp.iCount++,
          Fecha = DateTime.Now.ToString("yyyy'/'MM'/'dd' 'hh':'mm':'ss'.'fff' 'tt"),
          TipoEvento = TipoInformacion.ErrorProceso,
          Numero = GlobalApp.Numero,
          Comentario = GlobalApp.Mensaje
        });
        Console.WriteLine("Ocurrieron errores al ejecutar este proceso: " + GlobalApp.Mensaje.Trim() + ". Seguimiento de pila: " + oEx.StackTrace.Trim());
      }
      finally
      {
        // Limpiamos variables.
        _oTime1 = DateTime.Now; _oTimeTotal = new TimeSpan(_oTime1.Ticks - _oTime0.Ticks); DestroyVars();

        // Obtengo la fecha de termino.
        GlobalApp.DetailLog.Add(new DetailLOG()
        {
          Id = GlobalApp.iCount++,
          Fecha = DateTime.Now.ToString("yyyy'/'MM'/'dd' 'hh':'mm':'ss'.'fff' 'tt"),
          TipoEvento = TipoInformacion.Informacion,
          Numero = 0,
          Comentario = "Fecha de termino: " + _oTime1.ToString()
        });
        Console.WriteLine("Fecha de termino: " + _oTime1.ToString());

        // Obtengo el tiempo transcurrido.
        GlobalApp.DetailLog.Add(new DetailLOG()
        {
          Id = GlobalApp.iCount++,
          Fecha = DateTime.Now.ToString("yyyy'/'MM'/'dd' 'hh':'mm':'ss'.'fff' 'tt"),
          TipoEvento = TipoInformacion.Informacion,
          Numero = 0,
          Comentario = "Tiempo transcurrido en ejecutarse este proceso: " + _oTimeTotal.ToString()
        });
        Console.WriteLine("Tiempo transcurrido en ejecutarse este proceso: " + _oTimeTotal.ToString());

        // Guardamos los mensajes en el log y limpiamos las variables.
        GlobalApp.oLog.ListEvents = GlobalApp.DetailLog; XMLSerializacion<LOGFiles>.WriteToXmlFile(Path.Combine(GlobalApp.FolderLog, string.Concat("LOGTestConsole_", DateTime.Now.ToString("yyyy''MM''dd''hh''mm''ss''fff"), ".xml")), GlobalApp.oLog, false);
        GlobalApp.DetailLog = null; GlobalApp.oLog = null;
      }
    }

    /// <summary>
    /// Inicialización de variables.
    /// </summary>
    private static void InitVars()
    {
      _oSb = new StringBuilder(); _oSb.Clear();
    }

    /// <summary>
    /// Destrucción de variables.
    /// </summary>
    private static void DestroyVars()
    {
      _oSb = null;
    }
  }
}