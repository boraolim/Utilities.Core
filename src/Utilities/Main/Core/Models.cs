// Archivo: Models.cs
// Colección de objetos para las funciones de la librería Utilities.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 3 de julio de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2021. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

namespace Utilities
{

  #region "Tipos genéricos."

  /// <summary>
  /// Modelo que devuelve el resultado de una petición JSON.
  /// </summary>
  [Serializable]
  public class ResponseForList<T> where T : class, new()
  {
    /// <summary>
    /// Lista generica de un set de datos o tipo genérico.
    /// </summary>
    /// <value></value>
    public List<T> ObjectData { get; set; }

    /// <summary>
    /// Flag de status de respuesta.
    /// </summary>
    /// <value>True si es satisfactorio.</value>        
    public bool success { get; set; }

    /// <summary>
    /// Total de registros obtenidos.
    /// </summary>
    /// <value>Número de elementos devueltos.</value>    
    public int total_elements { get; set; }
  }

  /// <summary>
  /// Modelo que devuelve el resultado de una petición JSON.
  /// </summary>
  [Serializable]
  public class ResponseForObject<T> where T : class, new()
  {
    /// <summary>
    /// Objeto generico de un tipo genérico.
    /// </summary>
    /// <value></value>
    public T ObjectData { get; set; }

    /// <summary>
    /// Flag de status de respuesta.
    /// </summary>
    /// <value>True si es satisfactorio.</value>        
    public bool success { get; set; }
  }

  /// <summary>
  /// Objeto que devuelve resultado de usuario validado.
  /// </summary>
  [Serializable]
  public class UserResponseModel
  {
    /// <summary>
    /// Flag de status de respuesta.
    /// </summary>
    /// <value>True si es satisfactorio.</value>    
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Codigo HTTP.
    /// </summary>
    /// <value>Según el valor de la variable 'IsSucess', devuelve un código de error HTTP.</value>
    public int StatusCode { get; set; }

    /// <summary>
    /// Mensaje generado.
    /// </summary>
    /// <value>Mensaje de texto.</value>
    public string Message { get; set; }

    /// <summary>
    /// Cuenta de usuario autenticado.
    /// </summary>
    /// <value>Cuenta de usuario activa con la autenticación satisfactoria.</value>
    public string UserName { get; set; }

    /// <summary>
    /// Token JWT si el usuario activo existe.
    /// </summary>
    /// <value>Cadena de texto con el token JWT generado.</value>
    public string AccessToken { get; set; }

    /// <summary>
    /// Fecha de expiración del token.
    /// </summary>
    /// <value>Texto de la fecha de expiración del token.</value>
    public string ExpiresIn { get; set; }
  }


  /// <summary>
  /// Clase 'JWTResponse' para obtener el status de autenticación.
  /// </summary>
  [Serializable]
  public class JWTReponse
  {
    /// <summary>
    /// Flag de status de respuesta.
    /// </summary>
    /// <value>True si es satisfactorio.</value>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Enumeración que indica el status de validación de autenticación.
    /// </summary>
    /// <value>Devuelve un valor de estatus de autenticación de usuario.</value>
    public StatusAccunt StatusUser { get; set; }

    /// <summary>
    /// Token JWT si el usuario activo existe.
    /// </summary>
    /// <value>Cadena de texto con el token JWT generado.</value>
    public string JWTTokenGenerate { get; set; }

    /// <summary>
    /// Mensaje generado.
    /// </summary>
    /// <value>Mensaje de texto.</value>
    public string Message { get; set; }

    /// <summary>
    /// Expiración del token JWT en mínutos.
    /// </summary>
    /// <value>Expiración del token JWT en mínutos.</value>
    public int ExpireInMinutes { get; set; }
  }

  /// <summary>
  /// Clase 'PDFFileInfo' para información de documentos portables.
  /// </summary>
  [Serializable]
  public class PDFFileInfo
  {
    /// <summary>
    /// Título del documento PDF.
    /// </summary>
    public string Titulo { get; set; }

    /// <summary>
    /// Asunto u objetivo del archivo PDF.
    /// </summary>
    public string Asunto { get; set; }

    /// <summary>
    /// Autor del archivo PDF.
    /// </summary>
    public string Autor { get; set; }

    /// <summary>
    /// Palabras clave del archivo PDF.
    /// </summary>
    public string PalabraClave { get; set; }

    /// <summary>
    /// Autor del archivo PDF.
    /// </summary>
    public string Creador { get; set; } 

    /// <summary>
    /// Contraseña para la edición del archivo PDF.
    /// </summary>
    public string Contrasenia { get; set; }
  }

  /// <summary>
  /// Clase 'SelectWeeksYear' base para mostrar los días de la semana de un año.
  /// </summary>
  [Serializable]
  public class SelectWeeksYear
  {
    /// <summary>
    /// Semana de inicio.
    /// </summary>
    public DateTime WeekStart { get; set; }

    /// <summary>
    /// Semana final.
    /// </summary>
    public DateTime WeekFinish { get; set; }

    /// <summary>
    /// Mes en formato entero númerico.
    /// </summary>
    public short MonthNumber { get; set; }

    /// <summary>
    /// Semana en formato númerico.
    /// </summary>
    public short WeekNum { get; set; }
  }

  /// <summary>
  /// Clase 'DescriptionStatusCodes' que almacena los detalles de los códigos HTTP.
  /// </summary>
  [Serializable]
  public class DescriptionStatusCodes
  {
    /// <summary>
    /// Mensaje corto en inglés del código HTTP.
    /// </summary>
    public string StatusEnglishMessage { get; set; }

    /// <summary>
    /// Mentaje corto del código HTTP.
    /// </summary>
    public string StatusMessage { get; set; }

    /// <summary>
    /// Mensaje completo del código HTTP.
    /// </summary>
    public string DescriptionMessage { get; set; }
  }

  #endregion

  #region "Componentes para paginación."

  /// <summary>
  /// Estructura 'bootgridResults' para el componente web 'bootgrid'.
  /// Fuente: http://www.jquery-bootgrid.com/
  /// </summary>
  /// <typeparam name="T">Tipo genérico.</typeparam>
  [Serializable]
  public class bootgridResults<T> where T : class, new()
  {
    /// <summary>
    /// Atributo de número de página.
    /// </summary>
    public int current { get; set; }

    /// <summary>
    /// Atributo de total de registros por página.
    /// </summary>
    public int rowCount { get; set; }

    /// <summary>
    /// Atributo de total de registros del catálogo.
    /// </summary>
    public int total { get; set; }

    /// <summary>
    /// Atributo de la estructura interna de las filas.
    /// </summary>
    public List<T> rows { get; set; }
  }

  /// <summary>
  /// Estructura 'DataTablesResult' para el componente web 'jquery DataTables'.
  /// Fuente: https://datatables.net/
  /// </summary>
  /// <typeparam name="T">Tipo genérico.</typeparam>
  [Serializable]
  public class DataTablesResult<T> where T : class, new()
  {
    /// <summary>
    /// Atributo de número de página.
    /// </summary>
    public int draw { get; set; }

    /// <summary>
    /// Atributo de total de registros por página.
    /// </summary>
    public int recordsFiltered { get; set; }

    /// <summary>
    /// Atributo de total de registros del catálogo.
    /// </summary>
    public int recordsTotal { get; set; }

    /// <summary>
    /// Atributo de la estructura interna de las filas.
    /// </summary>
    public List<T> data { get; set; }
  }

  /// <summary>
  /// Estructura 'jqGridRow' para la estructura de una fila en el componente jqGrid.
  /// Fuente: http://www.trirand.com/jqgridwiki/doku.php?id=wiki:retrieving_data#json_dot_notation
  /// </summary>
  /// <typeparam name="T">Tipo genérico.</typeparam>
  [Serializable]
  public class jqGridRow<T> where T : class, new()
  {
    /// <summary>
    /// Identificador de la fila.
    /// </summary>
    public int id;

    /// <summary>
    /// Array de contenido de la fila.
    /// </summary>
    public T cell { get; set; }
  }

  /// <summary>
  /// Estructura 'jqGridTemplate' para la estructura final para el componente jqGrid que incluye el array de datos y modelo de columnas.
  /// </summary>
  [Serializable]
  public class jqGridTemplate
  {
    /// <summary>
    /// Array de objetos de columnas.
    /// </summary>
    public List<colModel> colModel { get; set; }

    /// <summary>
    /// Array de datos del componente jqGrid.
    /// </summary>
    public ArrayList data { get; set; }

    /// <summary>
    /// Título del encabezado de jqGrid.
    /// </summary>
    public string caption { get; set; }
  }

  /// <summary>
  /// Estructura 'colModel' para la estructura de un modelo de columnas en el componente jqGrid.
  /// </summary>
  [Serializable]
  public class colModel
  {
    /// <summary>
    /// Campo o atributo de datos que se asociará la columna del grid.
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// Título de la columna del jqGrid.
    /// </summary>
    public string label { get; set; }

    /// <summary>
    /// Flag que indica el ancho de la columna del jqGrid (por defecto, 150).
    /// </summary>
    public uint width { get; set; }

    /// <summary>
    /// Flag que indica si la alineación del texto de la columna es a la izquierda, derecha o justificada.
    /// </summary>
    public string align { get; set; }

    /// <summary>
    /// Flag que indica si la columna es aplicable para búsqueda.
    /// </summary>
    public bool search { get; set; }

    /// <summary>
    /// Flag que indica si la columna del grid es congelada o no. 
    /// </summary>
    public bool frozen { get; set; }

    /// <summary>
    /// Flag que indica si la columna del grid se puede aumentar o reducir su ancho.
    /// </summary>
    public bool resizable { get; set; }

    /// <summary>
    /// Flag que indica si la columna del grid es editable.
    /// </summary>
    public bool editable { get; set; }

    /// <summary>
    /// Flag que indica si la columna se oculta en el desplegado de datos del componente jqGrid.
    /// </summary>
    public bool hidden { get; set; }

    /// <summary>
    /// Flag que indica si la columna es ordenable, es decir, que se le aplicar ordenamiento ascendente o descendente.
    /// </summary>
    public bool sortable { get; set; }

    /// <summary>
    /// Indicador de tipo de formato.
    /// </summary>
    public string formatter { get; set; }

    /// <summary>
    /// Objeto 'formatOptions'.
    /// </summary>
    public formatoptions formatoptions { get; set; }

    /// <summary>
    /// Objeto 'colModel'.
    /// </summary>
    public colModel() { width = 150; align = "left"; }
  }

  /// <summary>
  /// Estructura 'formatoptions' para el formato de tipos de datos del módelo de columnas del componente jqGrid.
  /// </summary>
  public class formatoptions
  {
    /// <summary>
    /// String que indica el tipo de separador decimal, si es punto o coma.
    /// </summary>
    public string decimalSeparator { get; set; }

    /// <summary>
    /// String que indica el separador de millares, si es punto o coma.
    /// </summary>
    public string thousandsSeparator { get; set; }

    /// <summary>
    /// Entero que indica el número de digitos decimales de precisión.
    /// </summary>
    public uint? decimalPlaces { get; set; }

    /// <summary>
    /// String que indica el valor de defecto para los datos númericos (enteros o reales).
    /// </summary>
    public string defaultValue { get; set; }

    /// <summary>
    /// String que indica el prefijo para los datos númericos (enteros, reales o monetarios).
    /// </summary>
    public string prefix { get; set; }

    /// <summary>
    /// String que indica el postfijo para los datos númericos (enteros, reales o monetarios).
    /// </summary>
    public string suffix { get; set; }

    /// <summary>
    /// String que indica la liga URL de una dirección en internet.
    /// </summary>
    public string baseLinkUrl { get; set; }

    /// <summary>
    /// String que indica el Valor adicional que se agrega después de la baseLinkUrl.
    /// </summary>
    public string showAction { get; set; }

    /// <summary>
    /// String que indica un parámetro adicional que se puede agregar después de la propiedad idName.
    /// </summary>
    public string addParam { get; set; }

    /// <summary>
    /// String que indica, Si se establece, se agrega como un atributo adicional.
    /// </summary>
    public string target { get; set; }

    /// <summary>
    /// String que indica que es el primer parámetro que se agrega después de showAction. Por defecto, esto es ID.
    /// </summary>
    public string idName { get; set; }

    /// <summary>
    /// Flag que indica si un checkbox es habilitado o no.
    /// </summary>
    public bool? disabled { get; set; }

    /// <summary>
    /// Formato de fecha de origen.
    /// </summary>
    public string srcformat { get; set; }

    /// <summary>
    /// Formato de fecha nuevo.
    /// </summary>
    public string newformat { get; set; }
  }

  /// <summary>
  /// Estructura 'jqGridResults' para el componente jqGrid.
  /// Fuente: http://www.trirand.com/jqgridwiki/doku.php?id=wiki:retrieving_data#json_dot_notation
  /// </summary>
  /// <typeparam name="T">Tipo genérico.</typeparam>
  [Serializable]
  public class jqGridResults<T> where T : class, new()
  {
    /// <summary>
    /// Atributo de número de página.
    /// </summary>
    public int page { get; set; }

    /// <summary>
    /// Atributo de total de registros por página.
    /// </summary>
    public int total { get; set; }

    /// <summary>
    /// Atributo de total de registros del catálogo.
    /// </summary>
    public int records { get; set; }

    /// <summary>
    /// Atributo de la estructura interna de las filas.
    /// </summary>
    public List<jqGridRow<T>> rows { get; set; }
  }

  /// <summary>
  /// Clase 'jQueryGrid' para almacenar los datos de un catálogo para el componente web jQuery Grid.
  /// Fuente: http://versions.gijgo.com/0_4/Documentation/Grid
  /// </summary>
  /// <typeparam name="T">Tipo genérico.</typeparam>
  [Serializable]
  public class jQueryGrid<T> where T : class, new()
  {
    /// <summary>
    /// Atributo de total de registros del catálogo.
    /// </summary>
    public int total { get; set; }

    /// <summary>
    /// Atributo de la estructura interna de las filas.
    /// </summary>
    public List<T> records { get; set; }
  }

  #endregion

  #region "Componentes para el objeto 'IWhereClauseGenerator'."

  /// <summary>
  /// Clase 'WhereFilter' para la generación de criterios de consulta.
  /// </summary>
  [Serializable]
  public class WhereFilter
  {
    /// <summary>
    /// Propiedad que almacena el nombre del operador booleano de decisión.
    /// </summary>
    public GroupOp groupOp { get; set; }

    /// <summary>
    /// Propiedad que almacena el arreglo de reglas de condición WHERE.
    /// </summary>
    public List<WhereRule> rules { get; set; }
  }

  /// <summary>
  /// Clase 'WhereRule' para la generación de condiciones o reglas de consulta.
  /// </summary>
  [Serializable]
  public class WhereRule
  {
    /// <summary>
    /// Propiedad que almacena el nombre del campo de la tabla.
    /// </summary>
    public string field { get; set; }

    /// <summary>
    /// Propiedad que almacena el operador de condición de consulta.
    /// </summary>
    public WhereConditions op { get; set; }

    /// <summary>
    /// Propiedad que almacena el valor del campo para la condición de consulta.
    /// </summary>
    public string data { get; set; }
  }

  #endregion

  #region "Atributos."

  /// <summary>
  /// Clase 'ColumAttribute' que especifica los atributos de un campo.
  /// </summary>
  [Serializable]
  [AttributeUsage(AttributeTargets.Property)]
  public class ColumnAttribute : Attribute
  {
    public string Name { get; set; }
    public Type DataType { get; set; }
  }

  #endregion

  #region "Google API."

  /// <summary>
  /// Clase 'GoogleDriveFiles' para la información de un archivo en Google Drive.
  /// </summary>
  [Serializable]
  public class GoogleDriveFiles
  {
    /// <summary>
    /// Identificador del archivo en Google Drive.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Nombre del archivo en Google Drive.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Tamaño en bytes del archivo en Google Drive.
    /// </summary>
    public long? Size { get; set; }

    /// <summary>
    /// Versión del archivo de Google Drive.
    /// </summary>
    public long? Version { get; set; }

    /// <summary>
    /// Identificador único en MD5.
    /// </summary>
    public string MD5Checksum { get; set; }

    /// <summary>
    /// Tipo de archivo.
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// Fecha de creación del archivo en Google Drive.
    /// </summary>
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// Propietario del archivo de Google Drive.
    /// </summary>
    public IList<Google.Apis.Drive.v3.Data.User> Owners { get; set; }

    /// <summary>
    /// URL Web del recurso de Google Drive.
    /// </summary>
    public string webViewLink { get; set; }

    /// <summary>
    /// Parents del archivo.
    /// </summary>
    public IList<string> Parents { get; set; }

    /// <summary>
    /// Identificadores del permisos.
    /// </summary>
    public IList<string> PermissionIds { get; set; }

    /// <summary>
    /// Flag que indica si se puede ver el archivo internamente.
    /// </summary>
    public bool? ViewersCanCopyContent { get; set; }

    /// <summary>
    /// Lista de permisos de usuario.
    /// </summary>
    public IList<Google.Apis.Drive.v3.Data.Permission> Permissions { get; set; }
  }

  /// <summary>
  /// Clase que guarda la información de la cuenta de servicio de Google tipo JSON.
  /// </summary>
  [Serializable]
  public class PersonalServiceAccountCredential
  {
    /// <summary>
    /// Tipo de cuenta.
    /// </summary>
    public string type { get; set; }

    /// <summary>
    /// Identificador de proyecto.
    /// </summary>
    public string project_id { get; set; }

    /// <summary>
    /// Clave privada ID.
    /// </summary>
    public string private_key_id { get; set; }

    /// <summary>
    /// Contenido de la clave privada.
    /// </summary>
    public string private_key { get; set; }

    /// <summary>
    /// Correo electrónico del cliente.
    /// </summary>
    public string client_email { get; set; }

    /// <summary>
    /// Id. Cliente.
    /// </summary>
    public string client_id { get; set; }

    /// <summary>
    /// URL de autentificación.
    /// </summary>
    public string auth_uri { get; set; }

    /// <summary>
    /// Token de autenticación.
    /// </summary>
    public string token_uri { get; set; }

    /// <summary>
    /// Proveedor x509 del certificado asociado a la cuenta de servicio.
    /// </summary>
    public string auth_provider_x509_cert_url { get; set; }

    /// <summary>
    /// Cliente x509 del certificado asociado a la cuenta de servicio.
    /// </summary>
    public string client_x509_cert_url { get; set; }

  }

  #endregion

  #region "UTF8StringWrite para XML"

  /// <summary>
  /// Clase 'UTF8StringWriter' para la serialización de valores XML en codificación UTF-8.
  /// </summary>
  [Serializable]
  public class UTF8StringWriter : StringWriter
  {
    /// <summary>
    /// Propiedad de lectura que convierte el formato de codificación a UTF-8.
    /// </summary>
    public override Encoding Encoding { get => Encoding.UTF8; }
  }

  #endregion

  #region "Proveedores de Bases de Datos."

  /// <summary>
  /// Clase 'DataParameterManager' para el almacenamiento de los objetos 'IDbDataParameter'.
  /// </summary>
  public class DataParameterManager
  {
    /// <summary>
    /// Función que crea un parámetro 'IDbDataParameter'.
    /// </summary>
    /// <param name="providerName">Nombre del proveedor de datos.</param>
    /// <param name="name">Nombre del parámetro.</param>
    /// <param name="value">Valor del parámetro.</param>
    /// <param name="dbType">Tipo de parámetro.</param>
    /// <param name="direction">Dirección de entrada o salida del parámetro.</param>
    /// <returns>Devuelve un objeto 'IDbDataParameter' configurado, según el gestor de Base de Datos.</returns>
    public static IDbDataParameter CreateParameter(DataBaseProviders providerName, string name, object value, DbType dbType, ParameterDirection direction = ParameterDirection.Input)
    {
      IDbDataParameter parameter = null;

      switch (providerName)
      {
        case DataBaseProviders.SQLServer:                                          // Conexión a SQL Server.
          parameter = CreateSQLServerParameter(name, value, dbType, direction);
          break;
        case DataBaseProviders.Odbc:                                               // Conexión a ODBC.
          parameter = CreateODBCParameter(name, value, dbType, direction);
          break;
        case DataBaseProviders.MySQLServer:                                        // Conexión a MySQL/MariaDB.
          parameter = CreateMySQLParameter(name, value, dbType, direction);
          break;
      }

      return parameter;
    }

    /// <summary>
    /// Función que crea un parámetro 'IDbDataParameter'.
    /// </summary>
    /// <param name="providerName">Nombre del proveedor de datos.</param>
    /// <param name="name">Nombre del parámetro.</param>
    /// <param name="size">Longitud del parámetro.</param>
    /// <param name="value">Valor del parámetro.</param>
    /// <param name="dbType">Tipo de parámetro.</param>
    /// <param name="direction">Dirección de entrada o salida del parámetro.</param>
    /// <returns>Devuelve un objeto 'IDbDataParameter' configurado, según el gestor de Base de Datos.</returns>
    public static IDbDataParameter CreateParameter(DataBaseProviders providerName, string name, object value, int size, DbType dbType, ParameterDirection direction = ParameterDirection.Input)
    {
      IDbDataParameter parameter = null;

      switch (providerName)
      {
        case DataBaseProviders.SQLServer:                                          // Conexión a SQL Server.
          parameter = CreateSQLServerParameter(name, size, value, dbType, direction);
          break;
        case DataBaseProviders.Odbc:                                               // Conexión a ODBC.
          parameter = CreateODBCParameter(name, size, value, dbType, direction);
          break;
        case DataBaseProviders.MySQLServer:                                        // Conexión a MySQL/MariaDB.
          parameter = CreateMySQLParameter(name, size, value, dbType, direction);
          break;
      }

      return parameter;
    }

    /// <summary>
    /// Función protegida que crea un objeto 'IDbDataParameter' para Microsoft SQL Server (SqlParameter).
    /// </summary>
    /// <param name="name">Nombre del parámetro.</param>
    /// <param name="value">Valor del parámetro.</param>
    /// <param name="dbType">Tipo de parámetro.</param>
    /// <param name="direction">Dirección de entrada o salida del parámetro.</param>
    /// <returns>Devuelve un objeto 'IDbDataParameter' configurado, desde el objeto 'SqlParameter'.</returns>
    protected static IDbDataParameter CreateSQLServerParameter(string name, object value, DbType dbType, ParameterDirection direction)
    {
      return new SqlParameter
      {
        DbType = dbType,
        ParameterName = name,
        Direction = direction,
        Value = value
      };
    }

    /// <summary>
    /// Función protegida que crea un objeto 'IDbDataParameter' para Microsoft SQL Server (SqlParameter).
    /// </summary>
    /// <param name="name">Nombre del parámetro.</param>
    /// <param name="size">Valor del parámetro.</param>
    /// <param name="value">Valor del parámetro.</param>
    /// <param name="dbType">Tipo de parámetro.</param>
    /// <param name="direction">Dirección de entrada o salida del parámetro.</param>
    /// <returns>Devuelve un objeto 'IDbDataParameter' configurado, desde el objeto 'SqlParameter'.</returns>
    protected static IDbDataParameter CreateSQLServerParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
    {
      return new SqlParameter
      {
        DbType = dbType,
        Size = size,
        ParameterName = name,
        Direction = direction,
        Value = value
      };
    }

    /// <summary>
    /// Función protegida que crea un objeto 'IDbDataParameter' para Microsoft SQL Server (OdbcParameter).
    /// </summary>
    /// <param name="name">Nombre del parámetro.</param>
    /// <param name="value">Valor del parámetro.</param>
    /// <param name="dbType">Tipo de parámetro.</param>
    /// <param name="direction">Dirección de entrada o salida del parámetro.</param>
    /// <returns>Devuelve un objeto 'IDbDataParameter' configurado, desde el objeto 'OdbcParameter'.</returns>
    protected static IDbDataParameter CreateODBCParameter(string name, object value, DbType dbType, ParameterDirection direction)
    {
      return new OdbcParameter
      {
        DbType = dbType,
        ParameterName = name,
        Direction = direction,
        Value = value
      };
    }

    /// <summary>
    /// Función protegida que crea un objeto 'IDbDataParameter' para Microsoft SQL Server (OdbcParameter).
    /// </summary>
    /// <param name="name">Nombre del parámetro.</param>
    /// <param name="size">Valor del parámetro.</param>
    /// <param name="value">Valor del parámetro.</param>
    /// <param name="dbType">Tipo de parámetro.</param>
    /// <param name="direction">Dirección de entrada o salida del parámetro.</param>
    /// <returns>Devuelve un objeto 'IDbDataParameter' configurado, desde el objeto 'OdbcParameter'.</returns>
    protected static IDbDataParameter CreateODBCParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
    {
      return new OdbcParameter
      {
        DbType = dbType,
        Size = size,
        ParameterName = name,
        Direction = direction,
        Value = value
      };
    }

    /// <summary>
    /// Función protegida que crea un objeto 'IDbDataParameter' para Microsoft SQL Server (MySqlParameter).
    /// </summary>
    /// <param name="name">Nombre del parámetro.</param>
    /// <param name="value">Valor del parámetro.</param>
    /// <param name="dbType">Tipo de parámetro.</param>
    /// <param name="direction">Dirección de entrada o salida del parámetro.</param>
    /// <returns>Devuelve un objeto 'IDbDataParameter' configurado, desde el objeto 'MySqlParameter'.</returns>
    protected static IDbDataParameter CreateMySQLParameter(string name, object value, DbType dbType, ParameterDirection direction)
    {
      return new MySqlParameter
      {
        DbType = dbType,
        ParameterName = name,
        Direction = direction,
        Value = value
      };
    }

    /// <summary>
    /// Función protegida que crea un objeto 'IDbDataParameter' para Microsoft SQL Server (MySqlParameter).
    /// </summary>
    /// <param name="name">Nombre del parámetro.</param>
    /// <param name="size">Valor del parámetro.</param>
    /// <param name="value">Valor del parámetro.</param>
    /// <param name="dbType">Tipo de parámetro.</param>
    /// <param name="direction">Dirección de entrada o salida del parámetro.</param>
    /// <returns>Devuelve un objeto 'IDbDataParameter' configurado, desde el objeto 'MySqlParameter'.</returns>
    protected static IDbDataParameter CreateMySQLParameter(string name, int size, object value, DbType dbType, ParameterDirection direction)
    {
      return new MySqlParameter
      {
        DbType = dbType,
        Size = size,
        ParameterName = name,
        Direction = direction,
        Value = value
      };
    }
  }

  /// <summary>
  /// Clase 'SQLServerConnection' que hereda las funciones de la interfaz 'IDataBaseHandler' para el acceso a Base de datos de SQL Server.
  /// </summary>
  [Serializable]
  public class SQLServerConnection : IDatabaseHandler
  {
    /// <summary>
    /// Nodo que guarda la cadena de conexión extraída del nombre de la sección del archivo de configuración.
    /// </summary>
    protected string _CadenaConexion { get; set; }

    #region "Atributos."
    public string CadenaConexion { get => _CadenaConexion; set => _CadenaConexion = value; }

    #endregion

    /// <summary>
    /// Método para cerrar una conexión activa.
    /// </summary>
    /// <param name="connection">Objeto conexión.</param>
    public void CloseConnection(IDbConnection connection)
    {
      var sqlConnection = (SqlConnection)connection;
      sqlConnection.Close(); sqlConnection.Dispose();
    }

    /// <summary>
    /// Crear un objeto Adapter.
    /// </summary>
    /// <param name="command">Objeto comando</param>
    /// <returns>Devuelve un objeto del tipo SqlDataAdapter.</returns>
    IDataAdapter IDatabaseHandler.CreateAdapter(IDbCommand command) => new SqlDataAdapter((SqlCommand)command);

    /// <summary>
    /// Método para crear un comando para Base de Datos.
    /// </summary>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando SQL.</param>
    /// <param name="connection">Objeto conexión.</param>
    /// <returns>Devuelve un objeto 'SqlCommand' con los datos y opciones configuradas.</returns>
    public IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection)
    {
      return new SqlCommand
      {
        CommandText = commandText,
        Connection = (SqlConnection)connection,
        CommandType = commandType
      };
    }

    /// <summary>
    /// Método para crear un objeto conexión.
    /// </summary>
    /// <returns>Devuelve un objeto del tipo 'SqlConnection'.</returns>
    IDbConnection IDatabaseHandler.CreateConnection() => new SqlConnection(_CadenaConexion);

    /// <summary>
    /// Crear un objeto Parameter.
    /// </summary>
    /// <param name="command">Objeto comando.</param>
    /// <returns>Devuelve un objeto del tipo 'SqlParameter'.</returns>
    public IDbDataParameter CreateParameter(IDbCommand command)
    {
      SqlCommand SQLcommand = (SqlCommand)command;
      return SQLcommand.CreateParameter();
    }
  }

  /// <summary>
  /// Clase 'ODBCConnection' que hereda las funciones de la interfaz 'IDataBaseHandler' para el acceso a Base de datos por medio de ODBC.
  /// </summary>
  [Serializable]
  public class ODBCConnection : IDatabaseHandler
  {
    /// <summary>
    /// Nodo que guarda la cadena de conexión extraída del nombre de la sección del archivo de configuración.
    /// </summary>
    protected string _CadenaConexion { get; set; }

    #region "Atributos."

    public string CadenaConexion { get => _CadenaConexion; set => _CadenaConexion = value; }

    #endregion

    /// <summary>
    /// Método para crear un objeto conexión.
    /// </summary>
    /// <returns>Devuelve un objeto del tipo 'OdbcConnection'.</returns>
    IDbConnection IDatabaseHandler.CreateConnection() => new OdbcConnection(_CadenaConexion);

    /// <summary>
    /// Método para cerrar una conexión activa.
    /// </summary>
    /// <param name="connection">Objeto conexión.</param>
    public void CloseConnection(IDbConnection connection)
    {
      var odbcConnection = (OdbcConnection)connection;
      odbcConnection.Close(); odbcConnection.Dispose();
    }

    /// <summary>
    /// Método para crear un comando para Base de Datos.
    /// </summary>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando SQL.</param>
    /// <param name="connection">Objeto conexión.</param>
    /// <returns>Devuelve un objeto 'OdbcCommand' con los datos y opciones configuradas.</returns>
    public IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection)
    {
      return new OdbcCommand
      {
        CommandText = commandText,
        Connection = (OdbcConnection)connection,
        CommandType = commandType
      };
    }

    /// <summary>
    /// Crear un objeto Adapter.
    /// </summary>
    /// <param name="command">Objeto comando</param>
    /// <returns>Devuelve un objeto del tipo OdbcDataAdapter.</returns>
    IDataAdapter IDatabaseHandler.CreateAdapter(IDbCommand command) => new OdbcDataAdapter((OdbcCommand)command);

    /// <summary>
    /// Crear un objeto Parameter.
    /// </summary>
    /// <param name="command">Objeto comando.</param>
    /// <returns>Devuelve un objeto del tipo 'OdbcParameter'.</returns>
    public IDbDataParameter CreateParameter(IDbCommand command)
    {
      OdbcCommand SQLcommand = (OdbcCommand)command;
      return SQLcommand.CreateParameter();
    }
  }

  /// <summary>
  /// Clase 'MySQLServerConnection' que hereda las funciones de la interfaz 'IDataBaseHandler' para el acceso a Base de datos de MySQL o MariaDB Server.
  /// </summary>
  [Serializable]
  public class MySQLServerConnection : IDatabaseHandler
  {
    /// <summary>
    /// Nodo que guarda la cadena de conexión extraída del nombre de la sección del archivo de configuración.
    /// </summary>
    protected string _CadenaConexion { get; set; }

    #region "Atributos."

    public string CadenaConexion { get => _CadenaConexion; set => _CadenaConexion = value; }

    #endregion

    /// <summary>
    /// Método para cerrar una conexión activa.
    /// </summary>
    /// <param name="connection">Objeto conexión.</param>
    public void CloseConnection(IDbConnection connection)
    {
      var MySqlConnection = (MySqlConnection)connection;
      MySqlConnection.Close(); MySqlConnection.Dispose();
    }

    /// <summary>
    /// Crear un objeto Adapter.
    /// </summary>
    /// <param name="command">Objeto comando</param>
    /// <returns>Devuelve un objeto del tipo MySqlDataAdapter.</returns>
    IDataAdapter IDatabaseHandler.CreateAdapter(IDbCommand command) => new MySqlDataAdapter((MySqlCommand)command);

    /// <summary>
    /// Método para crear un comando para Base de Datos.
    /// </summary>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando SQL.</param>
    /// <param name="connection">Objeto conexión.</param>
    /// <returns>Devuelve un objeto 'MySqlCommand' con los datos y opciones configuradas.</returns>
    public IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection)
    {
      return new MySqlCommand
      {
        CommandText = commandText,
        Connection = (MySqlConnection)connection,
        CommandType = commandType
      };
    }

    /// <summary>
    /// Método para crear un objeto conexión.
    /// </summary>
    /// <returns>Devuelve un objeto del tipo 'MySqlConnection'.</returns>
    IDbConnection IDatabaseHandler.CreateConnection() => new MySqlConnection(_CadenaConexion);

    /// <summary>
    /// Crear un objeto Parameter.
    /// </summary>
    /// <param name="command">Objeto comando.</param>
    /// <returns>Devuelve un objeto del tipo 'SqlParameter'.</returns>
    public IDbDataParameter CreateParameter(IDbCommand command)
    {
      MySqlCommand MySqlCommand = (MySqlCommand)command;
      return MySqlCommand.CreateParameter();
    }
  }

  #endregion

}
