using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

using Utilities;

namespace TestUtilitiesLibrary
{
  /// <summary>
  /// Clase 'ClassSample' que hace algo.
  /// </summary>
  /// <summary xml:lang="es-MX">
  /// Clase 'ClassSample' que hace algo.
  /// </summary>
  /// <summary xml:lang="en">
  /// 'ClassSample' class that does something.
  /// </summary>
  /// <summary xml:lang="en-US">
  /// 'ClassSample' class that does something.
  /// </summary>
  public class Classsample : IInterfaceSample
  {
    private readonly ConnectionStringCollection _settings;
    private readonly IConfiguration _config;
    private readonly IToolService _iToolSvc;
    private readonly IDummieData _iDummieData;
    private readonly IRijndaelEncryptionService _iRijndaelEncryptionService;
    private readonly IDictionaryCollectionService _iDictionaryCollectionService;
    private readonly ISMSService _iSMSService;
    private readonly IGoogleRepositoryService _iGoogleRepositoryService;
    private readonly IXMLSerializationService<List<Customer>> _iXmlSerializationService;
    private readonly IDbManagerService _iDbManagerSQLServer;

    /// <summary>
    /// Carpeta de la aplicación.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Carpeta de la aplicación.
    /// </summary>
    /// <summary xml:lang="en">
    /// Application folder.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// Application folder.
    /// </summary>
    public string folderApp { get; set; }

    /// <summary>
    /// Carpeta de la aplicación.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Carpeta de la aplicación.
    /// </summary>
    /// <summary xml:lang="en">
    /// Application folder.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// Application folder.
    /// </summary>
    public string folderImg { get; set; }

    /// <summary>
    /// Carpeta de la aplicación.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Carpeta de la aplicación.
    /// </summary>
    /// <summary xml:lang="en">
    /// Application folder.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// Application folder.
    /// </summary>
    public string folderSrc { get; set; }

    /// <summary>
    /// Carpeta de la aplicación.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Carpeta de la aplicación.
    /// </summary>
    /// <summary xml:lang="en">
    /// Application folder.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// Application folder.
    /// </summary>
    public string folderSrcDest { get; set; }

    /// <summary>
    /// Carpeta de la aplicación.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Carpeta de la aplicación.
    /// </summary>
    /// <summary xml:lang="en">
    /// Application folder.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// Application folder.
    /// </summary>
    public string folderAuthGoogle { get; set; }

    /// <summary>
    /// Constructor de la clase 'ClassSample'.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Constructor de la clase 'ClassSample'.
    /// </summary>
    /// <summary xml:lang="en">
    /// Constructor of 'clsDato' class.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// Constructor of 'clsDato' class.
    /// </summary>
    public Classsample(IConfiguration config, IToolService iToolSvc, IDummieData dummieData,
               IDictionaryCollectionService dictionaryCollectionService, IRijndaelEncryptionService rijndaelEncryptionService,
               ISMSService sMSService, IGoogleRepositoryService googleRepositoryService, IXMLSerializationService<List<Customer>> xMLSerializationService,
               IOptions<ConnectionStringCollection> settings, IDbManagerService dbManager)
    {
      _config = config; _iToolSvc = iToolSvc; _iDummieData = dummieData;
      _iRijndaelEncryptionService = rijndaelEncryptionService; _iDictionaryCollectionService = dictionaryCollectionService;
      _iSMSService = sMSService; _iGoogleRepositoryService = googleRepositoryService;
      _iXmlSerializationService = xMLSerializationService; _settings = settings.Value; _iDbManagerSQLServer = dbManager;

      folderApp = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}";
      folderImg = $"{string.Concat(folderApp, "\\img")}";
      folderSrc = $"{string.Concat(folderApp, "\\src")}";
      folderSrcDest = $"{string.Concat(folderApp, "\\imgfinal")}";
      folderAuthGoogle = $"{string.Concat(folderApp, "\\auth-google")}";
    }

    /// <summary>
    /// Una función que hace algo.
    /// </summary>
    /// <summary xml:lang="es-MX">
    /// Una función que hace algo.
    /// </summary>
    /// <summary xml:lang="en">
    /// A function that does something.
    /// </summary>
    /// <summary xml:lang="en-US">
    /// A function that does something.
    /// </summary>
    public async Task DoActionSomethingAsync()
    {
      try
      {
        Console.Title = "Prueba de la librería Utilities.Core para .NET Core";

        var _rommieData = _iDummieData.GetCustomers().ToList();

        if (!Directory.Exists(folderImg))
          Directory.CreateDirectory(folderImg);

        if (!Directory.Exists(folderSrc))
          Directory.CreateDirectory(folderSrc);

        if (!Directory.Exists(folderSrcDest))
          Directory.CreateDirectory(folderSrcDest);

        string _archivoCSV1 = $@"{folderSrc}\\{System.Guid.NewGuid()}.csv";
        string _archivoCSV2 = $@"{folderSrc}\\{System.Guid.NewGuid()}.csv";
        string _archivoCSV3 = $@"{folderSrc}\\{System.Guid.NewGuid()}.csv";
        string _archivoCSV4 = $@"{folderSrc}\\{System.Guid.NewGuid()}.csv";
        string _archivoCSV5 = $@"{folderSrc}\\{System.Guid.NewGuid()}.csv";
        string _archivoImgPng = $@"{folderSrcDest}\\{System.Guid.NewGuid()}.jpg";
        string _credentialsGoogle = $@"{folderAuthGoogle}\\credentials.json";
        string _accountMailGoogle = $@"accountservice1@quickstart2-323601.iam.gserviceaccount.com";
        string _keyFileGoogle = $@"{folderAuthGoogle}\\quickstart2.p12";
        string _jsonFileGoogle = $@"{folderAuthGoogle}\\quickstart2.json";
        string _xmlListFile = $@"{folderSrc}\\{System.Guid.NewGuid()}.xml";

        CultureInfo culture = CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name);
        CultureInfo.CurrentCulture = culture; CultureInfo.CurrentUICulture = culture;
        Console.WriteLine("The current culture is {0} {1} {2} [{3}]", culture.EnglishName, culture.NativeName, culture.TwoLetterISOLanguageName, culture.Name);

        // Valores de fecha.
        Console.WriteLine("Valores de fecha:\n");
        Console.WriteLine($"{_iToolSvc.ToDateUniversal(DateTime.Now)}");
        Console.WriteLine($"{_iToolSvc.ToDateUniversalFirstMinute(DateTime.Now)}");
        Console.WriteLine($"{_iToolSvc.ToDateUniversalLastMinute(DateTime.Now)}");
        Console.WriteLine($"{_iToolSvc.ToObjectDateTime(DateTime.Now)}");
        Console.WriteLine($"{_iToolSvc.ToObjectDateTimeFirstMinute(DateTime.Now)}");
        Console.WriteLine($"{_iToolSvc.ToObjectDateTimeLastMinute(DateTime.Now)}");
        Console.WriteLine($"{_iToolSvc.ToDateTime(DateTime.Now.ToString())}");

        Console.WriteLine($"{_iToolSvc.ToBigInt("23")}");
        Console.WriteLine($"{_iToolSvc.ToInteger("23")}");
        Console.WriteLine($"{_iToolSvc.ToShort("23")}");
        Console.WriteLine($"{_iToolSvc.ToSingle("23")}");
        Console.WriteLine($"{_iToolSvc.ToDouble("23")}");
        Console.WriteLine($"{_iToolSvc.ToReal("23")}");

        Console.WriteLine($"{_iToolSvc.CheckRegularExpression("4125-3903-3903-1022", Patterns.CreditCardPattern)}");
        Console.WriteLine($"{_iToolSvc.CheckRegularExpression("BORO800727BD9", Patterns.RFCSATPattern)}");
        Console.WriteLine($"{_iToolSvc.CheckRegularExpression("BORO800727HDFNML09", Patterns.CURPPattern)}");
        Console.WriteLine($"{_iToolSvc.ClearString("AMLO CHINGAS A TU RETEPUTAMADRE !@1201$$$")}");

        await _iToolSvc.ListToCSVAsync<Customer>(_rommieData, _archivoCSV1, ",");
        Console.WriteLine("Migración de la lista genérica a archivo de texto plano realizada correctamente.");
        var xxx = await _iToolSvc.FileToArrayBytesAsync(_archivoCSV1);
        Console.WriteLine("El primer archivo de texto plano se generó a arreglo de bytes.");
        await _iToolSvc.ArrayBytesToFileAsync(xxx, _archivoCSV2);
        Console.WriteLine("El arreglo de bytes anterior se guardó a un nuevo archivo de texto plano.");

        Console.WriteLine($"Información del equipo: \nIP:{_iToolSvc.LocalIPAddress()}\nUsuario de dominio:{_iToolSvc.GetUserDomain()}\nNombre del equipo:{_iToolSvc.GetMachineName()}");

        Console.WriteLine($"Cadena cifrada: {await _iToolSvc.RandomStringAsync(25)}");

        await _iToolSvc.ListToCSVAsync<SelectWeeksYear>(await _iToolSvc.SelectWeeksYearAsync(1950, 5), $@"{folderSrc}\\{System.Guid.NewGuid()}.csv", ",");
        Console.WriteLine($"Generación del archivo de semanas realizado correctamente.");

        var _uDataTableWeeks = await _iToolSvc.ToDataTableAsync<SelectWeeksYear>(await _iToolSvc.SelectWeeksYearAsync(1950, 5));
        Console.WriteLine($"Generación del objeto DataTable realizado correctamente.");

        await _iToolSvc.ConvertFileEncodingAsync(_archivoCSV2, _archivoCSV3, Encoding.UTF8, Encoding.GetEncoding(65001));
        Console.WriteLine($"Cambio de codificación del archivo realizado correctamente.");

        var _oCollectionObj = await _iToolSvc.ToCollectionAsync<Customer>(_rommieData);
        Console.WriteLine($"Colección genérica de objetos realizada correctamente.");

        var _objByteArry = await _iToolSvc.StringToArrayBytesAsync(@"AMLO CHINGAS A TU RETEPUTAMADRE !@1201$$$");
        Console.WriteLine($"Cadena de texto convertida a arreglo de bytes realizada correctamente.");

        var _objStrFinal = await _iToolSvc.ArrayByteToStringAsync(_objByteArry);
        Console.WriteLine($"Arreglo de bytes convertida a texto realizada correctamente: {_objStrFinal}");

        var _strJSONDReader = await _iToolSvc.DataReaderToJSONAsync(_uDataTableWeeks.CreateDataReader());
        Console.WriteLine($"Objeto DataReader migrado a JSON correctamente.");

        var _strReaderList = await _iToolSvc.DataReaderMapToListAsync<SelectWeeksYear>(_uDataTableWeeks.CreateDataReader());
        Console.WriteLine($"Objeto DataReader migrado lista genérica correctamente.");

        var _newList = await _iToolSvc.ConvertDataTableAsync<SelectWeeksYear>(_uDataTableWeeks);
        Console.WriteLine($"El objeto DataTable fue portado a lista generica. Total de elementos: {_newList.Count}.");

        await _iToolSvc.DataTableToCSVAsync(_uDataTableWeeks, _archivoCSV4, ",");
        Console.WriteLine($"El objeto DataTable fue exportado a archivo CSV de manera satisfactoria.");

        var _strImageBase64 = await _iToolSvc.ImageToBase64Async($"{folderImg}\\animaniacs-reboot.jpg");
        Console.WriteLine($"La imagen de esta aplicación se ha convertido a Base64 de manera satisfactoria.");

        await _iToolSvc.Base64StringToImage(_strImageBase64, _archivoImgPng);
        Console.WriteLine($"La cadena de Base64 se ha convertido a archivo de imagen de manera satisfactoria.");

        /* Probamos el armador de clausulas WHERE dinámicamente. */
        var _newFilter = new WhereFilter()
        {
          Condition = GroupOp.OR,
          Rules = new List<WhereFilter>()
          {
            new WhereFilter { Field = "Country", Operator = WhereConditionsOp.Equal, Data = new[] { "Mexico" } },
            new WhereFilter { Field = "ContactName", Operator = WhereConditionsOp.Contains, Data = new[] { "M" } }
          }
        };

        var _ss = _rommieData.BuildQuery(_newFilter).ToList();
        Console.WriteLine($"Registros obtenidos después del filtro (BuildQuery):\n{_ss.Count}");

        var _predicate = QueryBuilder.BuildPredicate<Customer>(_newFilter, new BuildExpressionOptions() { ParseDatesAsUtc = false });
        var _ss1 = _rommieData.Where(_predicate).ToList();
        Console.WriteLine($"Registros obtenidos después del filtro (predicate):\n{_ss1.Count}");

        var parsedQuery = string.Empty;
        var _lambda = QueryBuilder.BuildExpressionLambda<Customer>(_newFilter, new BuildExpressionOptions() { ParseDatesAsUtc = false }, out parsedQuery);
        var _ss2 = _rommieData.AsQueryable().Where(_lambda).ToList();
        Console.WriteLine($"Registros obtenidos después del filtro (lambda):\n{_ss2.Count}");
        Console.WriteLine($"Filtro de texto: {parsedQuery}");


        var _campoGuid = $"{System.Guid.NewGuid().ToString()}";
        var _campoEncriptar = $"QueChingueASuMadreAMLO";

        var _aa1 = await _iRijndaelEncryptionService.EncryptRijndaelAsync(_campoEncriptar, _campoGuid);
        Console.WriteLine($"Cadena cifrada: {_aa1}");

        var _aa2 = await _iRijndaelEncryptionService.DecryptRijndaelAsync(_aa1, _campoGuid);
        Console.WriteLine($"Cadena descifrada: {_aa2}");

        var _aa3 = await _iDictionaryCollectionService.GetMimeTypeAsync(_archivoCSV1);
        Console.WriteLine($"Tipo de archivo MIME: {_aa3}");

        var _importedData = await _iToolSvc.ConvertCSVToListAsync<Customer>(_archivoCSV1, ",");
        Console.WriteLine($"Total de elementos recuperados de la lista: {_importedData.Count}");

        /* Envio de mensaje. */
        //string strProviderLink = "http://www.calixtaondemand.com/Controller.php/__a/sms.send.remote.ol.sa";
        //string strUser = "d.pereda@controlexpert.com";
        //string strPassword = "fac2fae07ea52a2cdca2e9945f214142df86aac69884a7ded26017a6a9e79b61";
        //string strNumber = "5524072102";
        //string strMensaje = $"{DateTime.Now.ToString()} - Nueva clave generada: {await _iToolSvc.RandomStringAsync(25)}.";
        //string strParameters = $"encpwd={strPassword}&email={strUser}&msg={strMensaje.Replace(" ", "%20")}&numtel={strNumber.Trim()}&mtipo=SMS";

        //_iSMSService.smsProviderLink = strProviderLink;
        //_iSMSService.smsUser = strUser;
        //_iSMSService.smsSecretKey = strPassword;
        //_iSMSService.smsParametersLink = $"encpwd={strPassword}&email={strUser}&msg={strMensaje.Replace(" ", "%20")}&numtel={strNumber.Trim()}&mtipo=SMS";
        //_iSMSService.smsConfirmationSucessFull = $"OK|3";
        //_iSMSService.smsProxy = null;

        //await _iSMSService.SendSmsMessage(strNumber, strMensaje);

        /* Gooogle Drive y Sheets. */
        //var _carpeta1 = $"Folder_{System.Guid.NewGuid().ToString()}";
        //var _carpeta2 = $"Folder_{System.Guid.NewGuid().ToString()}";
        //var _carpeta3 = $"Folder_{System.Guid.NewGuid().ToString()}";
        //var _carpeta4 = $"Folder_{System.Guid.NewGuid().ToString()}";

        //_iGoogleRepositoryService.FolderAuth = folderAuthGoogle;
        //await _iGoogleRepositoryService.CreateFolderAsync("395661611636-eqhveud4ukalq1ocbkjgc3mhotv2seht.apps.googleusercontent.com", "GYVY5PW0ffeu3_Pnv3AhJRsi", "QuickStart2", _carpeta1, false);
        //Console.WriteLine($"Carpeta creada correctamente en Google Drive: {_iGoogleRepositoryService.IdKeyGoogleDrive}, {_iGoogleRepositoryService.URLSourceDrive}.");

        //await _iGoogleRepositoryService.CreateSubFolderAsync("395661611636-eqhveud4ukalq1ocbkjgc3mhotv2seht.apps.googleusercontent.com", "GYVY5PW0ffeu3_Pnv3AhJRsi", "QuickStart2", _iGoogleRepositoryService.IdKeyGoogleDrive, _carpeta1, false);
        //Console.WriteLine($"Carpeta creada correctamente en Google Drive: {_iGoogleRepositoryService.IdKeyGoogleDrive}, {_iGoogleRepositoryService.URLSourceDrive}.");

        //var _subCarpetaId = _iGoogleRepositoryService.IdKeyGoogleDrive;

        //await _iGoogleRepositoryService.FileUploadAsync("395661611636-eqhveud4ukalq1ocbkjgc3mhotv2seht.apps.googleusercontent.com", "GYVY5PW0ffeu3_Pnv3AhJRsi", "QuickStart2", _archivoCSV1, false, false);
        //Console.WriteLine($"Archivo subido en la carpeta de manera correcta en Google Drive: {_iGoogleRepositoryService.IdKeyGoogleDrive}.");

        //await _iGoogleRepositoryService.FileUploadInFolderAsync("395661611636-eqhveud4ukalq1ocbkjgc3mhotv2seht.apps.googleusercontent.com", "GYVY5PW0ffeu3_Pnv3AhJRsi", "QuickStart2", _subCarpetaId, _archivoCSV1, false, false);
        //Console.WriteLine($"Archivo subido en la carpeta de manera correcta en Google Drive: {_iGoogleRepositoryService.IdKeyGoogleDrive}.");

        //await _iGoogleRepositoryService.GenerateSheetFromDataFileAsync("395661611636-eqhveud4ukalq1ocbkjgc3mhotv2seht.apps.googleusercontent.com", "GYVY5PW0ffeu3_Pnv3AhJRsi", "QuickStart2", _archivoCSV2);
        //Console.WriteLine($"Archivo de Google Sheets generado manera correcta en Google Drive: {_iGoogleRepositoryService.IdKeyGoogleDrive}.");

        /* Ejemplo con archivo de credenciales. */
        //await _iGoogleRepositoryService.CreateFolderAsync(_credentialsGoogle, "QuickStart2", _carpeta2, false);
        //Console.WriteLine($"Carpeta creada correctamente en Google Drive: {_iGoogleRepositoryService.IdKeyGoogleDrive}, {_iGoogleRepositoryService.URLSourceDrive}.");

        //await _iGoogleRepositoryService.CreateSubFolderAsync(_credentialsGoogle, "QuickStart2", _iGoogleRepositoryService.IdKeyGoogleDrive, _carpeta2, false);
        //Console.WriteLine($"Carpeta creada correctamente en Google Drive: {_iGoogleRepositoryService.IdKeyGoogleDrive}, {_iGoogleRepositoryService.URLSourceDrive}.");

        //await _iGoogleRepositoryService.FileUploadAsync(_credentialsGoogle, "QuickStart2", _archivoCSV1, false, false);
        //Console.WriteLine($"Archivo subido en la carpeta de manera correcta en Google Drive: {_iGoogleRepositoryService.IdKeyGoogleDrive}, {_iGoogleRepositoryService.URLSourceDrive}.");

        /* Ejemplo con cuenta de servicio. */
        //await _iGoogleRepositoryService.CreateFolderAsync(_accountMailGoogle, _keyFileGoogle, "QuickStart2", _carpeta3, GoogleAPIModeAccessServiceAccount.ByKeyFile, false);
        //Console.WriteLine($"Carpea creada correctamente en Google Drive: {_iGoogleRepositoryService.IdKeyGoogleDrive}, {_iGoogleRepositoryService.URLSourceDrive}.");

        //await _iGoogleRepositoryService.CreateFolderAsync(_accountMailGoogle, _jsonFileGoogle, "QuickStart2", _carpeta3, GoogleAPIModeAccessServiceAccount.ByJSONFile, false);
        //Console.WriteLine($"Carpea creada correctamente en Google Drive: {_iGoogleRepositoryService.IdKeyGoogleDrive}, {_iGoogleRepositoryService.URLSourceDrive}.");

        /* XML Serializacion. */
        var _strXML = await _iXmlSerializationService.XmlSerializeAsync(_rommieData);
        Console.WriteLine($"Lista de elementos genéricos convertidos a XML correctamente.");
        var _objFromXML1 = await _iXmlSerializationService.XmlDeserializeAsync(_strXML);
        Console.WriteLine($"Cadena de texto XML importada correctamente.");
        await _iXmlSerializationService.WriteToXmlFileAsync(_xmlListFile, _rommieData, false);
        Console.WriteLine($"Lista de elementos genéricos exportado a archivo XML correctamente.");
        var _objFromXML2 = await _iXmlSerializationService.ReadFromXmlFileAsync(_xmlListFile);
        Console.WriteLine($"Archivo XML de datos importado correctamente.");

        /* Conexión a Bases de Datos. */
        _iDbManagerSQLServer.CadenaConexion = _settings.ConnectionDbFirst;
        _iDbManagerSQLServer.ProveedorDatos = DataBaseProvidersOp.SQLServer;
        var UUU = await _iDbManagerSQLServer.GetDataToDataTableAsync("SELECT t1.[name], t1.[id] FROM dbo.sysobjects t1;", System.Data.CommandType.Text, null);
        Console.WriteLine($"DataTable generado correctamente desde Base de Datos.");
        var sysobj = await _iDbManagerSQLServer.GetDataToMappingToSingleAsync<SysObjects>("SELECT top 1 t1.[name], t1.[id] FROM dbo.sysobjects t1;", System.Data.CommandType.Text, null);
        Console.WriteLine($"DataSet generado correctamente desde Base de Datos.");
        var sysobjJSON = await _iDbManagerSQLServer.GetDataToJSONAsync("SELECT t1.* FROM dbo.sysobjects t1;", System.Data.CommandType.Text, null);
        Console.WriteLine($"JSON string generado correctamente desde Base de Datos.");
        var sysobjjQGridJSON = await _iDbManagerSQLServer.GetDataTojqGridJSONAsync("Reporte", "SELECT t1.* FROM dbo.sysobjects t1;", System.Data.CommandType.Text, null);
        Console.WriteLine($"JSON string para jqGrid generado correctamente desde Base de Datos.");
        await _iDbManagerSQLServer.ExportDataAsync(_archivoCSV5, ",", "SELECT t1.* FROM dbo.sysobjects t1;", System.Data.CommandType.Text, null);
        Console.WriteLine($"Generación del archivo de reporte realizado correctamente.");

        Console.WriteLine("Terminado.");
      }
      catch (Exception oEx)
      {
        Console.WriteLine($"{oEx.Message.Trim()}");
      }
      finally
      {
        Console.WriteLine("Pulse cualquier tecla para salir..."); Console.ReadLine();
      }
    }
  }
}
