// Archivo: IRijndaelEncryptionService.cs.
// Interfaz 'IRijndaelEncryptionService' para el cifrado de información bajo el algoritmo de encriptamiento Rijndael.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2020. All rights reserved

using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Utilities
{
  /// <summary>
  /// Interfaz 'IRijndaelEncryptionService' para el cifrado de información bajo el algoritmo de encriptamiento Rijndael.
  /// </summary>
  public interface IRijndaelEncryptionService
  {
    /// <summary>
    /// Función que realiza el cifrado de cadenas de texto por el algoritmo Rijndael.
    /// </summary>
    /// <param name="strValue">Cadena a cifrar.</param>
    /// <param name="strGuidSeed">La semilla GUID.</param>
    /// <returns>Devuelve una cadena cifrada.</returns>
    Task<string> EncryptRijndaelAsync(string strValue, string strGuidSeed);

    /// <summary>
    /// Funcion que desencripta un valor cifrado por Rijndael y que lo regresa a su cadena original.
    /// </summary>
    /// <param name="strValue">Cadena cifrada en Base64.</param>
    /// <param name="strGuidSeed">La semilla GUID.</param>
    /// <returns>Devuelve una cadena descifrada.</returns>
    Task<string> DecryptRijndaelAsync(string strValue, string strGuidSeed);
  }
}
