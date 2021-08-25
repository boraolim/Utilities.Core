// Archivo: RijndaelEncryptionService.cs.
// Clase 'RijndaelEncryptionService' para el cifrado de información bajo el algoritmo de encriptamiento Rijndael.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2020. All rights reserved

using System;
using System.IO;
using System.Text;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using Utilities.Resources;

namespace Utilities
{
  /// <summary>
  /// Clase 'RijndaelEncryptionService' para el cifrado de información bajo el algoritmo de encriptamiento Rijndael.
  /// </summary>
  [Serializable]
  public class RijndaelEncryptionService : IRijndaelEncryptionService
  {
    internal ushort _intNumberErr;
    internal string _strMessage;
    internal string _strRet;
    protected ResourceManager _resourceData;

    /// <summary>
    /// Creación de variables locales.
    /// </summary>
    protected void InitVars()
    {
      _intNumberErr = 0; _strMessage = string.Empty; _strRet = string.Empty;
      _resourceData = new ResourceManager(typeof(LanguageSource));
    }

    public async Task<string> EncryptRijndaelAsync(string strValue, string strGuidSeed)
    {
      InitVars();

      try
      {
        if (strValue.Length == 0 | string.IsNullOrEmpty(strValue))
        {
          _intNumberErr = 3701;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strValueToEncryptRequired")}";
        }
        else if (strGuidSeed.Length == 0 | string.IsNullOrEmpty(strGuidSeed))
        {
          _intNumberErr = 3702;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strValueGuidSeedRequired")}";
        }
        else if (!Regex.IsMatch(strGuidSeed, Patterns.GuidPattern))
        {
          _intNumberErr = 3703;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strValueGuidInvalid")}";
        }
        else
        {
          await Task.Run(() =>
          {
            var aesAlg = NewRijndaelManaged(strGuidSeed);
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            var msEncrypt = new MemoryStream();

            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
              swEncrypt.Write(strValue);

            _strRet = Convert.ToBase64String(msEncrypt.ToArray());

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 3700;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _strRet;
    }

    public async Task<string> DecryptRijndaelAsync(string strValue, string strGuidSeed)
    {
      InitVars();

      try
      {
        if (strValue.Length == 0 | string.IsNullOrEmpty(strValue))
        {
          _intNumberErr = 3801;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strValueToEncryptRequired")}";
        }
        else if (strGuidSeed.Length == 0 | string.IsNullOrEmpty(strGuidSeed))
        {
          _intNumberErr = 3802;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strValueGuidSeedRequired")}";
        }
        else if (!Regex.IsMatch(strGuidSeed, Patterns.GuidPattern))
        {
          _intNumberErr = 3803;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strValueGuidInvalid")}";
        }
        else if (!IsBase64String(strValue))
        {
          _intNumberErr = 3804;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strValueDecryptNotBase64")}";
        }
        else
        {
          await Task.Run(() =>
          {
            var aesAlg = NewRijndaelManaged(strGuidSeed);
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            var cipher = Convert.FromBase64String(strValue);

            using (var msDecrypt = new MemoryStream(cipher))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt))
              _strRet = srDecrypt.ReadToEnd();

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 3800;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _strRet;
    }

    /// <summary>
    /// Generador de semillas AES.
    /// </summary>
    /// <param name="strGuidSeed">Semilla Guid.</param>
    /// <returns>Genera un objeto 'RijndaelManaged' con la semilla generada.</returns>
    protected RijndaelManaged NewRijndaelManaged(string strGuidSeed)
    {
      var saltBytes = Encoding.UTF8.GetBytes(strGuidSeed);
      var key = new Rfc2898DeriveBytes(strGuidSeed, saltBytes);

      var aesAlg = new RijndaelManaged();
      aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
      aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

      return aesAlg;
    }

    /// <summary>
    /// Validador de cadenas de texto en formato Base64.
    /// </summary>
    /// <param name="strBase64String">strValue de texto escrita en Base 64.</param>
    /// <returns>True si es correcta la codificación a Base 64.</returns>
    protected bool IsBase64String(string strBase64String) => (strBase64String.Trim().Length % 4 == 0) && Regex.IsMatch(strBase64String.Trim(), Patterns.Base64StringPattern, RegexOptions.None);
  }
}
