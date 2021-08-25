// Archivo: XMLSerializationService.cs.
// Clase 'XMLSerializationService' para la serialización de valores XML.
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
using System.Xml.Serialization;
using System.Collections.Generic;

using Utilities.Resources;

namespace Utilities
{
  /// <summary>
  /// Clase 'XMLSerializationService' para la serialización de valores XML.
  /// </summary>
  /// <typeparam name="T">Tipo de dato genérico.</typeparam>
  [Serializable]
  public class XMLSerializationService<T> : IXMLSerializationService<T> where T : class, new()
  {
    /* Objetos no administrados (variables locales a nivel de la clase). */
    protected ushort _intNumberErr;
    protected string _strMessage;
    protected ResourceManager _resourceData;

    /// <summary>
    /// Creación de variables locales.
    /// </summary>
    protected void InitVars()
    {
      _intNumberErr = 0; _strMessage = string.Empty;
      _resourceData = new ResourceManager(typeof(LanguageSource));
    }

    public async Task<string> XmlSerializeAsybc(T obj)
    {
      InitVars(); var _objValue = string.Empty;

      try
      {
        if (obj == null)
        {
          _intNumberErr = 8501;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strObjectToXMLEmpty")}";
        }
        else
        {
          await Task.Run(() =>
          {
            using (StringWriter stringWriter = new UTF8StringWriter())
            {
              XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
              xmlSerializer.Serialize(stringWriter, obj);
              _objValue = stringWriter.ToString();
            }

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 8500;
        _strMessage = string.Concat("Ocurrió un error del tipo '", oEx.GetType(), "': ", ((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.ToString()));
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _objValue;
    }

    public async Task<T> XmlDeserializeAsync(string xmlDoc)
    {
      InitVars(); var _objValue = new T();

      try
      {
        if (string.IsNullOrEmpty(xmlDoc) | xmlDoc.Length == 0)
        {
          _intNumberErr = 8601;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strStringXMLRequired")}";
        }
        else
        {
          await Task.Run(() =>
          {
            using (StringReader stringReader = new StringReader(xmlDoc))
            {
              XmlSerializer serializer = new XmlSerializer(typeof(T));
              _objValue = (T)serializer.Deserialize(stringReader);
            }

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 8600;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _objValue;
    }

    public async Task WriteToXmlFileAsync(string strFileName, T objectToWrite, bool append = false)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 8701;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (objectToWrite == null)
        {
          _intNumberErr = 8702;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strStringXMLRequired")}";
        }
        else
        {
          await Task.Run(() =>
          {
            var serializer = new XmlSerializer(typeof(T));
            var writer = new StreamWriter(strFileName, append);
            serializer.Serialize(writer, objectToWrite);
            writer.Close(); writer.Dispose(); writer = null;

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 8700;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task<T> ReadFromXmlFileAsync(string strFileName)
    {
      InitVars(); var _objValue = new T();

      try
      {
        if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 8801;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!File.Exists(strFileName))
        {
          _intNumberErr = 8802;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else
        {
          await Task.Run(() =>
          {
            var serializer = new XmlSerializer(typeof(T));
            var reader = new StreamReader(strFileName);
            _objValue = (T)serializer.Deserialize(reader);
            reader.Close(); reader.Dispose(); reader = null;

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 8800;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _objValue;
    }
  }
}
