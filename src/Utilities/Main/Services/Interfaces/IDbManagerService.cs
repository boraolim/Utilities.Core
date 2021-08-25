// Archivo: IDbManagerService.cs
// Interfaz 'IDbManagerService' para administrar las operaciones de Bases de Datos.
// Basado en: http://csharpdocs.com/generic-data-access-layer-in-c-using-factory-pattern/
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2021. All rights reserved.

using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Utilities
{
  /// <summary>
  /// Interfaz 'IDbManagerService' para administrar las operaciones de Bases de Datos.
  /// </summary>
  /// <typeparam name="T">Tipo genérico.</typeparam>
  public interface IDbManagerService
  {
    /// <summary>
    /// Cadena de conexión extraída del nombre de la sección del archivo de configuración.
    /// </summary>
    public string CadenaConexion { get; set; }

    /// <summary>
    /// Tipos de proveedor a Base de Datos.
    /// </summary>
    public DataBaseProviders ProveedorDatos { get; set; }

    /// <summary>
    /// Función que crea un parámetro de consulta a Base de Datos.
    /// </summary>
    /// <param name="nameParameter">Nombre del parametro.</param>
    /// <param name="valueObject">Valor del parámetro.</param>
    /// <param name="dbType">Tipo de dato del parámetro.</param>
    /// <returns>Genera un objeto 'IDbDataParameter' para guardar un parámetro en Base de Datos.</returns>
    IDbDataParameter CreateParameter(string nameParameter, object valueObject, DbType dbType);

    /// <summary>
    /// Función que crea un parámetro de consulta a Base de Datos.
    /// </summary>
    /// <param name="nameParameter">Nombre del parametro.</param>
    /// <param name="valueObject">Valor del parámetro.</param>
    /// <param name="size">Longitud del tipo de parámetro de consulta.</param>
    /// <param name="dbType">Tipo de dato del parámetro.</param>
    /// <returns>Genera un objeto 'IDbDataParameter' para guardar un parámetro en Base de Datos.</returns>
    IDbDataParameter CreateParameter(string nameParameter, object valueObject, int size, DbType dbType);

    /// <summary>
    /// Función que crea un parámetro de consulta a Base de Datos.
    /// </summary>
    /// <param name="nameParameter">Nombre del parametro.</param>
    /// <param name="valueObject">Valor del parámetro.</param>
    /// <param name="size">Longitud del tipo de parámetro de consulta.</param>
    /// <param name="dbType">Tipo de dato del parámetro.</param>
    /// <param name="direction">Tipo de dirección de parametro de Base de Datos, ya sea, IN/OUT.</param>
    /// <returns>Genera un objeto 'IDbDataParameter' para guardar un parámetro en Base de Datos.</returns>
    IDbDataParameter CreateParameter(string nameParameter, object valueObject, int size, DbType dbType, ParameterDirection direction);

    /// <summary>
    /// Función que devuelve una función que devuelve el resultado de un query del tipo SELECT, o un stored procedure que devuelve un conjunto de datos en forma de un DataTable.
    /// </summary>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando: sentencia de texto, stored procedure o vista.</param>
    /// <param name="parameters">Lista de parámetros (opcional).</param>
    /// <returns>Devuelve un set de datos si la consulta se ejecutó correctamente. En caso contrario, lanza una excepción.</returns>
    Task<DataTable> GetDataToDataTableAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters = null);

    /// <summary>
    /// Función que devuelve una función que devuelve el resultado de un query del tipo SELECT, o un stored procedure que devuelve un conjunto de datos en forma de un Dataset.
    /// </summary>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando: sentencia de texto, stored procedure o vista.</param>
    /// <param name="parameters">Lista de parámetros (opcional).</param>
    /// <returns>Devuelve un set de datos si la consulta se ejecutó correctamente. En caso contrario, lanza una excepción.</returns>
    Task<DataSet> GetDataToDataSet(string commandText, CommandType commandType, IDbDataParameter[] parameters = null);

    /// <summary>
    /// Función que devuelve una función que devuelve el resultado de un query del tipo SELECT de un solo registro, o un stored procedure que devuelve un solo registro de datos en un objeto genérico.
    /// </summary>
    /// <typeparam name="T">Tipo de dato genérico.</typeparam>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando: sentencia de texto, stored procedure o vista.</param>
    /// <param name="parameters">Lista de parámetros (opcional).</param>
    /// <returns>Devuelve un objeto de datos si la consulta se ejecutó correctamente. En caso contrario, lanza una excepción.</returns>
    Task<T> GetDataToMappingToSingleAsync<T>(string commandText, CommandType commandType, IDbDataParameter[] parameters = null) where T : class, new();

    /// <summary>
    /// Función que devuelve una función que devuelve el resultado de un query del tipo SELECT, o un stored procedure que devuelve un conjunto de datos en una lista mapeada genérica.
    /// </summary>
    /// <typeparam name="T">Tipo de dato genérico.</typeparam>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando: sentencia de texto, stored procedure o vista.</param>
    /// <param name="parameters">Lista de parámetros (opcional).</param>
    /// <returns>Devuelve una lista mapeada de datos si la consulta se ejecutó correctamente. En caso contrario, lanza una excepción.</returns>
    Task<List<T>> GetDataToMappingAsync<T>(string commandText, CommandType commandType, IDbDataParameter[] parameters = null) where T : class, new();

    /// <summary>
    /// Función que devuelve una función que devuelve el resultado de un query del tipo SELECT, o un stored procedure que devuelve un conjunto de datos en una cadena de texto tipo JSON.
    /// </summary>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando: sentencia de texto, stored procedure o vista.</param>
    /// <param name="parameters">Lista de parámetros (opcional).</param>
    /// <returns>Devuelve una cadena de datos en formato JSON si la consulta se ejecutó correctamente. En caso contrario, lanza una excepción.</returns>
    Task<string> GetDataToJSONAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters = null);

    /// <summary>
    /// Función que devuelve una función que devuelve el resultado de un query del tipo SELECT, o un stored procedure que devuelve un conjunto de datos en una cadena de texto tipo JSON oara el componente jqGrid.
    /// </summary>
    /// <param name="Titulo">Título del componente jqGrid.</param>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando: sentencia de texto, stored procedure o vista.</param>
    /// <param name="parameters">Lista de parámetros (opcional).</param>
    /// <returns>Devuelve una cadena de datos en formato JSON si la consulta se ejecutó correctamente. En caso contrario, lanza una excepción.</returns>
    Task<string> GetDataTojqGridJSONAsync(string Titulo, string commandText, CommandType commandType, IDbDataParameter[] parameters = null);

    /// <summary>
    /// Función que exporta el set de datos de una consulta SQL en la Base de Datos a archivo de texto plano.
    /// </summary>
    /// <param name="strFileName">Archivo de texto plano donde se genera el set de datos en formato de texto plano.</param>
    /// <param name="strSeparator">Separador de datos.</param>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando: sentencia de texto, stored procedure o vista.</param>
    /// <param name="parameters">Lista de parámetros (opcional).</param>
    /// <returns>Devuelve un set de datos si la consulta se ejecutó correctamente. En caso contrario, lanza una excepción.</returns>
    Task ExportDataAsync(string strFileName, string strSeparator, string commandText, CommandType commandType, IDbDataParameter[] parameters = null);

    /// <summary>
    /// Función que para realizar operaciones de catálogo o para cambiar la información de una base de datos ejecutando las instrucciones UPDATE, INSERT o DELETE.
    /// </summary>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando: sentencia de texto, stored procedure o vista.</param>
    /// <param name="parameters">Lista de parámetros (opcional).</param>
    /// <returns>Para las instrucciones UPDATE, INSERT y DELETE, el valor devuelto corresponde al número de filas afectadas por el comando. Para los demás tipos de instrucciones, el valor devuelto es -1. En caso contrario, lanza una excepción.</returns>
    Task ExecuteSQLAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters = null);

    /// <summary>
    /// Función que para realizar operaciones de catálogo o para cambiar la información de una base de datos ejecutando las instrucciones UPDATE, INSERT o DELETE por medio de transacciones.
    /// </summary>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando: sentencia de texto, stored procedure o vista.</param>
    /// <param name="parameters">Lista de parámetros (opcional).</param>
    /// <returns>Para las instrucciones UPDATE, INSERT y DELETE, el valor devuelto corresponde al número de filas afectadas por el comando. Para los demás tipos de instrucciones, el valor devuelto es -1. En caso contrario, lanza una excepción.</returns>
    Task ExecuteSQLWithTransactionAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters);

    /// <summary>
    /// Función que para realizar operaciones de catálogo o para cambiar la información de una base de datos ejecutando las instrucciones UPDATE, INSERT o DELETE por medio de transacciones (con tipo de bloqueo).
    /// </summary>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando: sentencia de texto, stored procedure o vista.</param>
    /// <param name="isolationLevel">Tipo de bloqeo de transacción.</param>
    /// <param name="parameters">Lista de parámetros (opcional).</param>
    /// <returns>Para las instrucciones UPDATE, INSERT y DELETE, el valor devuelto corresponde al número de filas afectadas por el comando. Para los demás tipos de instrucciones, el valor devuelto es -1. En caso contrario, lanza una excepción.</returns>
    Task ExecuteSQLWithTransactionAsync(string commandText, CommandType commandType, IsolationLevel isolationLevel, IDbDataParameter[] parameters);

    /// <summary>
    /// Función que ejecuta una consulta SQL y devuelve la primera columna de la primera fila del conjunto de resultados que devuelve la consulta. Se omiten todas las demás columnas y filas.
    /// </summary>
    /// <param name="commandText">Sentencia SQL.</param>
    /// <param name="commandType">Tipo de comando: sentencia de texto, stored procedure o vista.</param>
    /// <param name="parameters">Lista de parámetros (opcional).</param>
    /// <returns>Solo recupera un único valor (por ejemplo, un valor agregado) de una base de datos</returns>
    Task<object> GetScalarValueAsync(string commandText, CommandType commandType, IDbDataParameter[] parameters = null);
  }
}
