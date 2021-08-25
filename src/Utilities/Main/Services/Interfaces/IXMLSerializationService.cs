// Archivo: IXMLSerializationService.cs
// Interfaz 'IXMLSerializationService' para la serialización de valores XML.
// Basado en: https://ngohungphuc.wordpress.com/2018/05/01/generic-repository-pattern-in-asp-net-core/
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2021. All rights reserved.

using System.Threading.Tasks;

namespace Utilities
{
  /// <summary>
  /// Interfaz 'IXMLSerializationService' para la serialización de valores XML.
  /// </summary>
  /// <typeparam name="T">Tipo genérico.</typeparam>
  public interface IXMLSerializationService<T> where T : class, new()
  {
    /// <summary>
    /// Función para convertir un objeto al tipo XML por medio de serialización.
    /// </summary>
    /// <param name="obj">Objeto del tipo genérico.</param>
    /// <returns>Devuelve un string con el formato XML del objeto serializado.</returns>
    Task<string> XmlSerializeAsybc(T obj);

    /// <summary>
    /// Función para convertir un valor XML a un tipo de dato.
    /// </summary>
    /// <param name="xmlDoc">Cadena XML en formato string.</param>
    /// <returns>Devuelve el objeto serializado en formato XML.</returns>
    Task<T> XmlDeserializeAsync(string xmlDoc);

    /// <summary>
    /// Función que escribe la instancia de objeto determinada en un archivo XML.
    /// </summary>
    /// <param name="strFileName">La ruta del archivo para escribir la instancia del objeto y su contenido.</param>
    /// <param name="objectToWrite">La instancia de objeto para escribir en el archivo.</param>
    /// <param name="append">Si es falso el archivo se sobrescribe si ya existe. De ser cierto el contenido se añadirá al archivo.</param>
    /// <returns>Genera un archivo en formato XML con la información del objeto y datos del tipo genérico.</returns>
    Task WriteToXmlFileAsync(string strFileName, T objectToWrite, bool append = false);

    /// <summary>
    /// Función que lee una instancia de un objeto desde un archivo XML.
    /// </summary>
    /// <param name="strFileName">La ruta del archivo a leer la instancia del objeto que contiene información del mismo.</param>
    /// <returns>Devuelve una nueva instancia del objeto una vez leído desde el archivo XML.</returns>
    Task<T> ReadFromXmlFileAsync(string strFileName);
  }
}
