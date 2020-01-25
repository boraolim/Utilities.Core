namespace TestUtilities
{
  using System;
  using System.IO;
  using System.Text;
  using System.Linq;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;

  using Utilities;

  using Microsoft.Extensions.Configuration;

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
    [DataNames("Database", "Database")]
    public string Database { get; set;}
  }

  [Serializable]
  public class Person
  {
    [DataNames("first_name", "firstName")]
    public string FirstName { get; set; }

    [DataNames("last_name", "lastName")]
    public string LastName { get; set; }

    [DataNames("dob", "dateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    [DataNames("job_title", "jobTitle")]
    public string JobTitle { get; set; }

    [DataNames("taken_name", "nickName")]
    public string TakenName { get; set; }

    [DataNames("is_american", "isAmerican")]
    public bool IsAmerican { get; set; }
  }

  public class Program
  {
    static DateTime _oScheduledTime = DateTime.Now;
    private static DateTime _oTime0;
    private static DateTime _oTime1;
    private static TimeSpan _oTimeTotal;
    private static StringBuilder _oSb = null;
    private static string strFileNameCSV = string.Concat(Tool.RandomString(15), ".csv");

    protected static string _strODBCString = string.Empty;
    protected static string server = "warehouse.ckckn9gxbosq.us-west-2.redshift.amazonaws.com";
    protected static string port = "5439";
    protected static string masterUsername = "admin_prod";
    protected static string masterUserPassword = "622jGpjhISfcycLsEXqO";
    protected static string DBName = "batchreportes";

    [STAThread()]
    static void Main(string[] args)
    {
      try
      {
        GlobalApp.oLog = new LOGFiles(GlobalApp.FolderLog.Trim(), "TestConsole", "Program.cs", "Main", "TestConsole", AssemblyInfo.Company.Trim());
        GlobalApp.DetailLog = new List<DetailLOG>(); GlobalApp.iCount = 1; GlobalApp.Numero = 0; GlobalApp.Mensaje = string.Empty; GlobalApp.FechaActual = DateTime.Now; _oTime0 = DateTime.Now;
        if (Directory.Exists(GlobalApp.FolderLog.Trim()) == false) { Directory.CreateDirectory(GlobalApp.FolderLog.Trim()); }                  // Carpeta de los archivos LOG de la aplicación.
        if (Directory.Exists(GlobalApp.FolderTemporal.Trim()) == false) { Directory.CreateDirectory(GlobalApp.FolderTemporal.Trim()); }        // Carpeta de archivo de reportes finales.
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

        // Mapeo de DataTables.
        var priestsDataSet = DataSetGenerator.Priests();
        DataNamesMapper<Person> mapper = new DataNamesMapper<Person>();
        List<Person> persons = mapper.Map(priestsDataSet.Tables[0]).ToList();

        var ranchersDataSet = DataSetGenerator.Ranchers();
        persons.AddRange(mapper.Map(ranchersDataSet.Tables[0]));

        foreach (var person in persons)
        {
          Console.WriteLine("First Name: " + person.FirstName + ", Last Name: " + person.LastName
                            + ", Date of Birth: " + person.DateOfBirth.ToShortDateString()
                            + ", Job Title: " + person.JobTitle + ", Nickname: " + person.TakenName
                            + ", Is American: " + person.IsAmerican);
        }

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

        var strDato = "CapMax = ((DepositoConceptoJubilacionPension - CargosDomiciliados - CargosConceptoCreditos) * 0.4);nnCapMax=n((DepositoConceptoJubilacionPension - CargosDomiciliados - CargosConceptoCreditos) - CapMax) n> (NúmeroCasasComerciales < 2 ? 400 : 800)? CapMax : (NúmeroCasasComerciales < 2 ? 400 : 800);nnCapMax = CapMax > 0 ? CapMax : 0;tnnCapMax = NúmeroCasasComerciales >= 3 ? 0 : CapMax;";
        Console.WriteLine("Dato anterior: {0}", strDato);
        strDato = Regex.Replace(strDato, Patrones.PatronAlphaLatino.Trim(), string.Empty);
        Console.WriteLine("Dato nuevo: {0}", strDato);

        var ArchivoReporteStr = string.Empty;

        // Uso de DBFactory.
		    // El primer parametro es el nombre de la conexión a Base de Datos, según en el archivo AppSettings.json.
		    // El segundo parametro es el tipo de conexión por plataforma de Base de Datos.
        Console.WriteLine("Haciendo una consulta SQL a Base de Datos (AWS Redshift via ODBC...)");
        using (var oDb = new DBManager("AWSConnectionBD", DataBaseProviders.Odbc))
        {
          // Carga de parametros.
          // var _oParam = new List<IDbDataParameter>();
          // _oParam.Add(oDb.CreateParameter("@Id", valor1, System.Data.DBType.String))
          // _oParam.Add(oDb.CreateParameter("@Id2", valor2, System.Data.DBType.String))
          // var oDt = oDb.GetDataToDataTable("SQL_Command_Strng", System.Data.CommandType.Text, _oParam.ToArray());

          var oDt = oDb.GetDataToDataTable("SELECT * FROM public.\"schema-convdatosgenerales-keops\" t1;", System.Data.CommandType.Text, null);
          Console.WriteLine("Consulta ejecutada correctamente. Total de registros: {0}.", oDt.Rows.Count);

          // Exportar un query de Base de Datos directo a archivo de texto plano.
          Console.WriteLine("Generando reporte de bases de datos...");
          ArchivoReporteStr = Path.Combine(GlobalApp.FolderTemporal.Trim(), string.Format("{0}.txt", Guid.NewGuid().ToString()));
          oDb.ExportData(ArchivoReporteStr, "|", "SELECT * FROM public.\"schema-convdatosgenerales-keops\" t1;", System.Data.CommandType.Text, null);
          Console.WriteLine("El reporte se ha generado correctamente en {0}", ArchivoReporteStr.Trim());
        } // Fin de la conexión de AWS Redshift via ODBC.

        Console.WriteLine("Haciendo una consulta SQL a Base de Datos (SQL Server Azure...)");
        using (var oDb = new DBManager("SQLServerConnectionBD", DataBaseProviders.SQLServer))
        {
          var oDt = oDb.GetDataToDataTable("SELECT * FROM products t1;", System.Data.CommandType.Text, null);
          Console.WriteLine("Consulta ejecutada correctamente. Total de registros: {0}.", oDt.Rows.Count);

          Console.WriteLine("Generando reporte de bases de datos...");
          ArchivoReporteStr = Path.Combine(GlobalApp.FolderTemporal.Trim(), string.Format("{0}.csv", Guid.NewGuid().ToString()));
          oDb.ExportData(ArchivoReporteStr, ",", "SELECT * FROM products t1;", System.Data.CommandType.Text, null);
          Console.WriteLine("El reporte se ha generado correctamente en {0}", ArchivoReporteStr.Trim());
        } // Fin de la conexión para SQL Server Azure.

        Console.WriteLine("Haciendo una consulta SQL a Base de Datos (MySQL Server/MariaDB Server...)");
        using (var oDb = new DBManager("MariaDBConnectionBD", DataBaseProviders.MySQLServer))
        {
          _oSb.Clear().AppendFormat("SHOW DATABASES;");

          var s = oDb.GetDataToMapping<ListaBD>(_oSb.ToString(), System.Data.CommandType.Text, null);

          Console.WriteLine("Consulta ejecutada correctamente. Total de registros: {0}.", s.Count);

          // Recorremos la lista.
          Console.WriteLine("Leyendo la lista de Bases de Datos.");

          foreach (var u in s)
            Console.WriteLine("{0}", u.Database.Trim());

          // Guardamos la lista de bases de datos en un archivo CSV.
          Console.WriteLine("Generando reporte de bases de datos...");
          ArchivoReporteStr = Path.Combine(GlobalApp.FolderTemporal.Trim(), string.Format("{0}.dat", Guid.NewGuid().ToString()));
          oDb.ExportData(ArchivoReporteStr, "|", _oSb.ToString(), System.Data.CommandType.Text, null);
          Console.WriteLine("El reporte se ha generado correctamente en {0}", ArchivoReporteStr.Trim());

          // Exportar directamente a Google Drive.
          Console.WriteLine("Generando reporte de bases de datos para Google Drive...");
          var UrlSourceDrive = string.Empty; var IdKeyGoogleDrive = string.Empty;
          oDb.ExportDataToGoogleSheetsInFolderWithPermissions(ArchivoReporteStr.Replace(".dat", ".csv"), ",", "ClientId", "SecretId", GlobalApp.FolderPersonal.Trim(), "AplicacionGoogleAPI", "Identificador_Google_Drive", "correo@gmail.com",
                                                              GoogleDrivePermissions.Reader, GoogleDriveGroups.User, false, false, true, out UrlSourceDrive, out IdKeyGoogleDrive, _oSb.ToString(), System.Data.CommandType.Text, null);

          Console.WriteLine("Identificadores de Google Drive: {0} y {1}", UrlSourceDrive, IdKeyGoogleDrive);
        } // Fin de la conexión para MySQL Server/ MariaDB.
          
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

        Console.WriteLine("Pulse cualquier tecla para salir..."); Console.ReadLine();
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