// Archivo: IToolService.cs
// Interfaz 'IToolService' para las funciones de la clase 'Tool' de Utilities.
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
using System.Collections.ObjectModel;

namespace Utilities
{
  /// <summary>
  /// Interfaz 'IToolService' para las funciones de la clase 'Tool' de Utilities.
  /// </summary>
  public interface IToolService
  {
    /// <summary>
    /// Función que muestra un formato de fecha "RFC3339".
    /// </summary>
    /// <param name="objDate">Fecha.</param>
    /// <returns>Devuelve una cadena de texto con el formato fecha RFC3339.</returns>
    string ToRfc3339String(DateTime objDate);

    /// <summary>
    /// Función que devuelve una fecha con el formato universal.
    /// </summary>
    /// <param name="objDate">Fecha.</param>
    /// <remarks>Convierte la fecha en fecha con formato universal devuelta como cadena de texto.</remarks>
    string ToDateUniversal(DateTime objDate);

    /// <summary>
    /// Función que devuelve una fecha con el formato universal con el primer minuto del dia..
    /// </summary>
    /// <param name="objDate">Fecha.</param>
    /// <remarks>Convierte la fecha en fecha con formato universal devuelta como cadena de texto con el primer minuto del día.</remarks>
    string ToDateUniversalFirstMinute(DateTime objDate);

    /// <summary>
    /// Función que devuelve una fecha con el formato universal con el ultimo minuto del dia.
    /// </summary>
    /// <param name="objDate">Fecha.</param>
    /// <remarks>Convierte la fecha en fecha con formato universal devuelta como cadena de texto con el último minuto del día.</remarks>
    string ToDateUniversalLastMinute(DateTime objDate);

    /// <summary>
    /// Función que devuelve una fecha como tipo object.
    /// </summary>
    /// <param name="objDate">Fecha.</param>
    /// <remarks>Convierte la fecha tipeada a 'DateTime' a fecha tipeada al tipo 'object'.</remarks>
    object ToObjectDateTime(DateTime objDate);

    /// <summary>
    /// Función que devuelve una fecha como tipo object con el primer minuto.
    /// </summary>
    /// <param name="objDate">Fecha.</param>
    /// <remarks>Convierte la fecha tipeada a 'DateTime' a fecha tipeada al tipo 'object' con el primer minuto del día.</remarks>
    object ToObjectDateTimeFirstMinute(DateTime objDate);

    /// <summary>
    /// Función que devuelve una fecha como tipo object con el último minuto.
    /// </summary>
    /// <param name="objDate">Fecha.</param>
    /// <remarks>Convierte la fecha tipeada a 'DateTime' a fecha tipeada al tipo 'object' con el último minuto del día.</remarks>
    object ToObjectDateTimeLastMinute(DateTime objDate);

    /// <summary>
    /// Función que convierte una cadena de texto en formato 'object'.
    /// </summary>
    /// <param name="dateString">Fecha en formato cadena de texto.</param>
    /// <remarks>Si la cadena contiene el formato de fecha establecido, lo convierte al tipo 'object' con el valor de fecha ajustado. En caso contrario, lanza una excepción.</remarks>
    object ToDateTime(string dateString);

    /// <summary>
    /// Función que convierte un string a formato entero para valores enteros grandes.
    /// </summary>
    /// <param name="strValue">Cadena de texto a convertir a formato entero.</param>
    /// <returns>Retorna un valor decimal que puede ser convertido al tipo entero grandes (long o Int64).</returns>
    object ToBigInt(string strValue);

    /// <summary>
    /// Función que convierte un string a formato entero para valores enteros normales.
    /// </summary>
    /// <param name="strValue">Cadena de texto a convertir a formato entero.</param>
    /// <returns>Retorna un valor decimal que puede ser convertido al tipo entero normales (int o Int32).</returns>
    object ToInteger(string strValue);

    /// <summary>
    /// Función que convierte un string a formato entero para valores enteros cortos.
    /// </summary>
    /// <param name="strValue">Cadena de texto a convertir a formato entero.</param>
    /// <returns>Retorna un valor decimal que puede ser convertido al tipo entero corto (short o Int16).</returns>
    object ToShort(string strValue);

    /// <summary>
    /// Función que convierte un string a formato decimal para valores reales (flotantes).
    /// </summary>
    /// <param name="strValue">Cadena de texto a convertir al tipo de dato númerico flotante.</param>
    /// <returns>Retorna un valor decimal que puede ser convertido al tipo de dato real (flotantes).</returns>
    object ToSingle(string strValue);

    /// <summary>
    /// Función que convierte un string a formato decimal para valores reales (doble precision).
    /// </summary>
    /// <param name="strValue">Cadena de texto a convertir al tipo de dato númerico real.</param>
    /// <returns>Retorna un valor decimal que puede ser convertido al tipo de dato reale (doble precision).</returns>
    object ToDouble(string strValue);

    /// <summary>
    /// Función que convierte un string a formato decimal para valores reales (monetarios).
    /// </summary>
    /// <param name="strValue">Cadena de texto a convertir a formato moneda.</param>
    /// <returns>Retorna un valor decimal que puede ser convertido al tipo real (monetarios).</returns>
    object ToReal(string strValue);

    /// <summary>
    /// Función que devuelve un número de manera aleatoria (ruleta).
    /// </summary>
    /// <param name="intInicio">Número inicial.</param>
    /// <param name="intFinal">Número final.</param>
    /// <returns>Devuelve un número comprendido entre el número inicial y el número final.</returns>
    int RandomRoulette(int intInicio, int intFinal);

    /// <summary>
    /// Función que elimina caracteres extraños de una variable string.
    /// </summary>
    /// <param name="strValue">Cadena a evaluar.</param>
    /// <returns>Devuelve una cadena sin caracteres extraños.</returns>
    string ClearString(string strValue);

    /// <summary>
    /// Función que obtiene la dirección IPv4 del equipo local conectado en red.
    /// </summary>
    /// <returns>La dirección IPv4 del equipo local.</returns>
    string LocalIPAddress();

    /// <summary>
    /// Función que obtiene el usuario de dominio actual.
    /// </summary>
    /// <returns>Devuelve un string con el nombre de usuario actual.</returns>
    string GetUserDomain();

    /// <summary>
    /// Función que obtiene el nombre del equipo actual.
    /// </summary>
    /// <returns>Devuelve un string con el nombre del equipo actual.</returns>
    string GetMachineName();

    /// <summary>
    /// Función que obtiene una cadena aleatoria de caracteres.
    /// </summary>
    /// <param name="maxLength">Longitud de la cadena.</param>
    /// <returns>Devuelve un string con una cadena aleatoria de caracteres.</returns>
    Task<string> RandomStringAsync(int maxLength);

    /// <summary>
    /// Función que obtiene una cadena aleatoria de caracteres.
    /// </summary>
    /// <param name="maxLength">Longitud de la cadena.</param>
    /// <param name="strListChars">Patrón de cadena con caracteres definidos.</param>
    /// <returns>Devuelve un string con una cadena aleatoria de caracteres.</returns>
    Task<string> RandomStringAsync(int maxLength, string strListChars);

    /// <summary>
    /// Función que evalua una expresión regular.
    /// </summary>
    /// <param name="strValue">Un valor en cadena de texto.</param>
    /// <param name="strPattern">El patrón de validación de una cadena.</param>
    /// <returns>Retorna 'True' si la cadena cumple con el patrón de formato. 'False' en caso contrario.</returns>
    bool CheckRegularExpression(string strValue, string strPattern);

    /// <summary>
    /// Función que abre un archivo para un programa asociado.
    /// </summary>
    /// <param name="strFileName">Nombre del archivo que se desea abrir.</param>
    /// <remarks>Al abrir el archivo, ejecuta el programa asociado al archivo.</remarks>
    Task OpenFileByProcessAsync(string strFileName);

    /// <summary>
    /// Función que mata procesos activos del Sistema.
    /// </summary>
    /// <param name="strNameProcess">Nombre del proceso que se desea eliminar.</param>
    /// <remarks>Destruye el proceso que se está ejecutándose en el Administrador de Procesos de Windows.</remarks>
    Task DestroyProcess(string strNameProcess);

    /// <summary>
    /// Función que exporta una lista genérica a un archivo de texto plano.
    /// </summary>
    /// <param name="iList">Lista genérica.</param>
    /// <param name="strFileName">Nombre del archivo en formato CSV.</param>
    /// <param name="strSeparator">Separador de datos.</param>
    /// <remarks>Convierte la información de una lista generica a un archivo de texto plano.</remarks>
    Task ListToCSVAsync<T>(List<T> iList, string strFileName, string strSeparator) where T : new();

    /// <summary>
    /// Función que convierte el contenido de un archivo a una cadena de bytes.
    /// </summary>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <returns>Devuelve un arreglo de bytes del archivo seleccionado.</returns>
    Task<byte[]> FileToArrayBytesAsync(string strFileName);

    /// <summary>
    /// Función que convierte el contenido de un arreglo de bytes a un archivo destino.
    /// </summary>
    /// <param name="byteArray">Arreglo de bytes.</param>
    /// <param name="strFileName">Destino del archivo donde se va a crear.</param>
    /// <remarks>Genera un archivo nuevo con los bytes escritos.</remarks>
    Task ArrayBytesToFileAsync(byte[] byteArray, string strFileName);

    /// <summary>
    /// Función que muestra la lista de las semanas completas de un año y mes en cuestión.
    /// </summary>
    /// <param name="yearValue">Año actual u año.</param>
    /// <param name="monthValue">Mes actual o mes del año.</param>
    /// <returns>Devuelve una lista generica dinamica con todas las semanas del mes actual y año actual, con su día de inicio y día final. Se toma como inicio el primer día del año en cuestión.</returns>
    Task<List<SelectWeeksYear>> SelectWeeksYearAsync(short yearValue, short monthValue);

    /// <summary>
    /// Función que convierte un objeto del tipo Lista Genérica a un objeto DataTable.
    /// </summary>
    /// <param name="iList">Lista generica a convertir.</param>
    /// <returns>Devuelve un objeto DataTable con toda la información de la lista genérica de origen.</returns>
    Task<DataTable> ToDataTableAsync<T>(IList<T> iList) where T : new();

    /// <summary>
    /// Función que convierte un archivo a una codificación de texto especifica.
    /// </summary>
    /// <param name="strFileNameIn">Nombre del archivo de origen.</param>
    /// <param name="strFileNameOut">Nombre del archivo destino.</param>
    /// <param name="sourceEncoding">Codificación de origen.</param>
    /// <param name="destEncoding">Codificación destino.</param>
    /// <remarks>Genera una copia del archivo original en un nuevo archivo, con una codificiación de texto distinto a la codificiación de texto del archivo de origen.</remarks>
    Task ConvertFileEncodingAsync(string strFileNameIn, string strFileNameOut, Encoding sourceEncoding, Encoding destEncoding);

    /// <summary>
    /// Función que convierte un archivo a una codificación de texto especifica.
    /// </summary>
    /// <param name="iList">La lista generica en cuestión.</param>
    /// <remarks>Devuelve una colección generica convertida.</remarks>
    Task<Collection<T>> ToCollectionAsync<T>(List<T> iList) where T : new();

    /// <summary>
    /// Función que convierte un objeto IDataReader en una lista genérica.
    /// </summary>
    /// <typeparam name="T">Tipo generico.</typeparam>
    /// <param name="drReader">El objeto DataReader</param>
    /// <returns>Devuelve el contenido del objeto IDataReader a una lista generica.</returns>
    Task<List<T>> DataReaderMapToListAsync<T>(IDataReader drReader);

    /// <summary>
    /// Función que convierte una cadena de texto a un arreglo de bytes.
    /// </summary>
    /// <param name="strValue">Cadena de texto.</param>
    /// <returns>Genera un arreglo de bytes de la cadena de texto.</returns>
    Task<byte[]> StringToArrayBytesAsync(string strValue);

    /// <summary>
    /// Función que convierte un arreglo de bytes a una cadena de texto.
    /// </summary>
    /// <param name="byteArray">Arreglo de bytes.</param>
    /// <returns>Genera una cadena de texto desde un arreglo de bytes.</returns>
    Task<string> ArrayByteToStringAsync(byte[] byteArray);

    /// <summary>
    /// Función que convierte un objeto DataReader en una cadena de texto JSON.
    /// </summary>
    /// <param name="drReader">El objeto IDataReader</param>
    /// <returns>Devuelve un string en formato JSON del objeto DataTable.</returns>
    Task<string> DataReaderToJSONAsync(IDataReader drReader);

    /// <summary>
    /// Función que hace un mapeo de un objeto DataTable a lista generica.
    /// </summary>
    /// <typeparam name="T">Tipo generico.</typeparam>
    /// <param name="dtDataSet">Objeto DataTable.</param>
    /// <returns>Devuelve una lista generica con los datos del objeto DataTable.</returns>
    Task<List<T>> ConvertDataTableAsync<T>(DataTable dtDataSet) where T : new();

    /// <summary>
    /// Función que convierte un objeto DataTable a un archivo de texto plano.
    /// </summary>
    /// <param name="dtDataSet">El objeto DataTable.</param>
    /// <param name="strFileName">Nombre del archivo destino donde se guarda el set de datos.</param>
    /// <param name="strSeparator">Separador de datos.</param>
    /// <returns>Genera un archivo de texto plano con el contenido del set de datos de origen.</returns>
    Task DataTableToCSVAsync(DataTable dtDataSet, string strFileName, string strSeparator);

    /// <summary>
    /// Función que muestra las rutas completas de los nombres de archivos de una carpeta.
    /// </summary>
    /// <param name="strFolderName">Nombre de la carpeta.</param>
    /// <param name="strFilter">Filtro de extensión de archivo.</param>
    /// <returns>Devuelve una lista generica del tipo string con las rutas completas de los nombres de los archivos contenidos en la carpeta seleccionada.</returns>
    List<string> SelectFilesByFolderFullPaths(string strFolderName, string strFilter);

    /// <summary>
    /// Función que muestra los nombres de archivos de una carpeta.
    /// </summary>
    /// <param name="strFolderName">Nombre de la carpeta.</param>
    /// <param name="strFilter">Filtro de extensión de archivo.</param>
    /// <returns>Devuelve una lista generica del tipo string con los nombres de los archivos contenidos en la carpeta seleccionada.</returns>
    List<string> SelectFilesByFolderFileNames(string strFolderName, string strFilter);

    /// <summary>
    /// Función que ejecuta una petición HTTP con salida en formato texto plano.
    /// </summary>
    /// <param name="urlSource">URL de origen.</param>
    /// <param name="typeRequest">Tipo de petición: GET, POST, PUT, DELETE.</param>
    /// <param name="headersArray">Encabezados, si existen.</param>
    /// <param name="parametersToJSONString">Parametro de entrada en formato JSON string.</param>
    /// <returns>Devuelve un resultado si la petición es satisfactoria. En caso contrario, lanza una excepción del tipo 404 o 500.</returns>
    Task<string> ExecuteRequestToReadTask(string urlSource, EnumRequestType typeRequest, IReadOnlyDictionary<string, string> headersArray, string parametersToJSONString);

    /// <summary>
    /// Función que convierte una imagen a Base64.
    /// </summary>
    /// <param name="strImageFileName">Nombre del archivo de imagen de origen.</param>
    /// <returns>Devuelve una cadena de texto en Base64 de la imagen de origen.</returns>
    Task<string> ImageToBase64Async(string strImageFileName);

    /// <summary>
    /// Función que guarda el contenido de una cadena de texto en Base64 a imagen.
    /// </summary>
    /// <param name="strValueBase64">Valor en Base64.</param>
    /// <param name="strImageFileName">Nomnre del archivo de imagen de destino.</param>
    /// <returns>Genera un archivo de imagen final, si la cadena de Base64 es correctamente aplicable a un formato de imagen.<returns>
    Task Base64StringToImage(string strValueBase64, string strImageFileName);

    /// <summary>
    /// Función que convierte un archivo de texto plano y CSV delimitado para tipos genericos.
    /// </summary>
    /// <param name="strFileName">Nombre del archivo.</param>
    /// <param name="strSeparator">Separador que contiene el archivo de texto plano.</param>
    /// <typeparam name="T">Tipo generico.</typeparam>
    /// <returns>Devuelve una lista genérica con los datos del archivo de texto plano importado.</returns>
    Task<List<T>> ConvertCSVToListAsync<T>(string strFileName, string strSeparator) where T : new();
  }
}
