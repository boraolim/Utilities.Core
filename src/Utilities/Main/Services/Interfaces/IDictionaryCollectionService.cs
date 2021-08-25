// Archivo: IDictionaryCollectionService.cs
// Interfaz 'IDictionaryCollectionService' para el catálogo de códigos y claves de Utilities.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2021. All rights reserved.

using System.Threading.Tasks;
using System.Collections.Generic;

namespace Utilities
{
  /// <summary>
  /// Interfaz 'IDictionaryCollectionService' para el catálogo de códigos y claves de Utilities.
  /// </summary>
  public interface IDictionaryCollectionService
  {
    /// <summary>
    /// Función que muestra el catálogo de códigos de status de petición HTTP|HTTPS.
    /// </summary>
    /// <returns>Devuelve una lista generica de códigos de status de petición HTTP|HTTPS (400 en adelante).</returns>
    Dictionary<uint, DescriptionStatusCodes> GetStatusHTTPCodes();

    /// <summary>
    /// Gets the MIME-type for the given file name, or <see cref="FallbackMimeType"/> if a mapping doesn't exist.
    /// </summary>
    /// <param name="strFileName">The name of the file.</param>
    /// <returns>The MIME-type for the given file name.</returns>
    Task<string> GetMimeTypeAsync(string strFileName);
  }
}
