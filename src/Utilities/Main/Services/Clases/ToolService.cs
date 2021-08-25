// Archivo: ToolService.cs
// Clase 'ToolService' para las funciones especificas del programador.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 4 de agosto de 2021.
// Fecha de ultima modificación de código fuente: 4 de agosto de 2021.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2021. All rights reserved.

using System;
using System.IO;
using System.Net;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net.Http;
using System.Resources;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Utilities.Resources;

namespace Utilities
{ 
  /// <summary>
  /// Clase 'ToolService' para las funciones especificas del programador.
  /// </summary>
  [Serializable]
  public class ToolService : IToolService
  {
    /* Generador de instancias de números aleatorios. */
    /* Es mejor mantener una sola instancia aleatoria */
    /* y siguir usando la función "Next" en la misma instancia. */
    private readonly Random _random = new Random();

    /* Objetos no administrados (variables locales a nivel de la clase). */
    protected ushort _intNumberErr;
    protected string _strMessage;
    protected object _ObjValue;
    protected bool _bRetorno;
    protected byte[] _byteBuff;
    protected static List<SelectWeeksYear> Weeks;
    protected List<string> _Lista;

    /* Objetos para las funciones de lectura de CSV a tipo genérico. */
    protected Dictionary<string, PropertyInfo> _headerPropertyInfos = new Dictionary<string, PropertyInfo>();
    protected Dictionary<string, Type> _headerDaytaTypes = new Dictionary<string, Type>();

    protected CultureInfo _cultureObject;
    protected ResourceManager _resourceData;

    /// <summary>
    /// Creación de variables locales.
    /// </summary>
    protected void InitVars()
    {
      _intNumberErr = 0; _strMessage = string.Empty; _ObjValue = null; _bRetorno = false; _byteBuff = null;

      /* Ajustamos la localización. */
      _cultureObject = new CultureInfo(CultureInfo.CurrentCulture.Name.Trim());
      CultureInfo.CurrentCulture = _cultureObject; CultureInfo.CurrentUICulture = _cultureObject;
      LanguageSource.Culture = _cultureObject;
      _resourceData = new ResourceManager(typeof(LanguageSource));
      _headerDaytaTypes.Clear(); _headerPropertyInfos.Clear();
      /* Fin de la carga de localización. */
    }

    public string ToRfc3339String(DateTime objDate) => objDate.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);

    public string ToDateUniversal(DateTime objDate) => (objDate.ToString("yyyy''MM''dd' 'HH':'mm':'ss'.'fff"));

    public string ToDateUniversalFirstMinute(DateTime objDate) => (objDate.ToString("yyyy''MM''dd' '00:00:00.000"));

    public string ToDateUniversalLastMinute(DateTime objDate) => (objDate.ToString("yyyy''MM''dd' '23:59:59.999"));

    public object ToObjectDateTime(DateTime objDate)
    {
      InitVars();

      try
      {
        _ObjValue = new DateTime(objDate.Year, objDate.Month, objDate.Day, objDate.Hour, objDate.Minute, objDate.Second, objDate.Millisecond);
      }
      catch (Exception oEx)
      {
        _intNumberErr = 100;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _ObjValue;
    }

    public object ToObjectDateTimeFirstMinute(DateTime objDate)
    {
      InitVars();

      try
      {
        _ObjValue = new DateTime(objDate.Year, objDate.Month, objDate.Day, 0, 0, 0, 0);
      }
      catch (Exception oEx)
      {
        _intNumberErr = 200;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _ObjValue;
    }

    public object ToObjectDateTimeLastMinute(DateTime objDate)
    {
      InitVars();

      try
      {
        _ObjValue = new DateTime(objDate.Year, objDate.Month, objDate.Day, 23, 59, 59, 999);
      }
      catch (Exception oEx)
      {
        _intNumberErr = 300;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _ObjValue;
    }

    public object ToDateTime(string dateString)
    {
      InitVars();

      try
      {
        DateTime myDate = default(DateTime);
        DateTimeStyles OStyle = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal;

        if (DateTime.TryParse(dateString, _cultureObject, OStyle, out myDate) == true)
        {
          _ObjValue = ToObjectDateTime(myDate);
        }
        else
        {
          _intNumberErr = 401;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strDateTimeFormatInvalid")}";
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 400;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _ObjValue;
    }

    public object ToBigInt(string strValue)
    {
      InitVars();

      try
      {
        // Esta variable determina el estilo numerico, que debe de ser:
        // * 1. Que sea numerico.
        // * 2. Que sea entero completamente.
        // * 3. Puede tener o no signo negativo.
        // -------------------------------------
        NumberStyles Sty = NumberStyles.Number | NumberStyles.Integer | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign;

        long myInteger;

        // Para un idioma en especifico, podemos usar esta linea:
        // ------------------------------------------------------
        myInteger = long.Parse(strValue, Sty, _cultureObject);
        _ObjValue = myInteger;
      }
      catch (Exception oEx)
      {
        _intNumberErr = 500;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _ObjValue;
    }

    public object ToInteger(string strValue)
    {
      InitVars();

      try
      {
        // Esta variable determina el estilo numerico, que debe de ser:
        // * 1. Que sea numerico.
        // * 2. Que sea entero completamente.
        // * 3. Puede tener o no signo negativo.
        // -------------------------------------
        NumberStyles Sty = NumberStyles.Number | NumberStyles.Integer | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign;

        int myInteger;

        // Para un idioma en especifico, podemos usar esta linea:
        // ------------------------------------------------------
        myInteger = int.Parse(strValue, Sty, _cultureObject);
        _ObjValue = myInteger;
      }
      catch (Exception oEx)
      {
        _intNumberErr = 600;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _ObjValue;
    }

    public object ToShort(string strValue)
    {
      InitVars();

      try
      {
        // Esta variable determina el estilo numerico, que debe de ser:
        // * 1. Que sea numerico.
        // * 2. Que sea entero completamente.
        // * 3. Puede tener o no signo negativo.
        // -------------------------------------
        NumberStyles Sty = NumberStyles.Number | NumberStyles.Integer | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign;

        short myInteger;

        // Para un idioma en especifico, podemos usar esta linea:
        // ------------------------------------------------------
        myInteger = short.Parse(strValue, Sty, _cultureObject);
        _ObjValue = myInteger;
      }
      catch (Exception oEx)
      {
        _intNumberErr = 700;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _ObjValue;
    }

    public object ToSingle(string strValue)
    {
      InitVars();

      try
      {
        // Esta variable determina el estilo numerico, que debe de ser:
        // * 1. Que sea numerico.
        // * 2. Que tenga punto decimal.
        // * 3. Puede tener o no signo negativo.
        // -------------------------------------
        NumberStyles Sty = NumberStyles.Number | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.Float;

        float myDecimal;

        // Para un idioma en especifico, podemos usar esta linea:
        // ------------------------------------------------------
        myDecimal = float.Parse(strValue, Sty, _cultureObject);
        _ObjValue = myDecimal;
      }
      catch (Exception oEx)
      {
        _intNumberErr = 800;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _ObjValue;
    }

    public object ToDouble(string strValue)
    {
      InitVars();

      try
      {
        // Esta variable determina el estilo numerico, que debe de ser:
        // * 1. Que sea numerico.
        // * 2. Que tenga punto decimal.
        // * 3. Puede tener o no signo negativo.
        // -------------------------------------
        NumberStyles Sty = NumberStyles.Number | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.Float;

        double myDecimal;

        // Para un idioma en especifico, podemos usar esta linea:
        // ------------------------------------------------------
        myDecimal = double.Parse(strValue, Sty, _cultureObject);
        _ObjValue = myDecimal;
      }
      catch (Exception oEx)
      {
        _intNumberErr = 900;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _ObjValue;
    }

    public object ToReal(string strValue)
    {
      InitVars();

      try
      {
        // Esta variable determina el estilo numerico, que debe de ser:
        // * 1. Que sea numerico.
        // * 2. Que sea entero completamente.
        // * 3. Puede tener o no signo negativo.
        // -------------------------------------
        NumberStyles Sty = NumberStyles.Number | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowThousands;

        decimal myReal;

        // Para un idioma en especifico, podemos usar esta linea:
        // ------------------------------------------------------
        myReal = decimal.Parse(strValue, Sty, _cultureObject);
        _ObjValue = myReal;
      }
      catch (Exception oEx)
      {
        _intNumberErr = 1000;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _ObjValue;
    }

    public int RandomRoulette(int intInicio, int intFinal) => new Random().Next(intInicio, intFinal);

    public string ClearString(string strValue) => Regex.Replace(strValue, @"[^\w\.]", string.Empty);

    public string LocalIPAddress()
    {
      var strLocalIP = string.Empty;

      var heserver = Dns.GetHostEntry(Environment.MachineName.Trim());
      var ip = heserver.AddressList.ToList()
                       .Where(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                       .FirstOrDefault().ToString();
      strLocalIP = ip.ToString().Trim();

      return strLocalIP;
    }

    public string GetUserDomain() => Environment.UserName.Trim();

    public string GetMachineName() => Environment.MachineName.Trim();

    public async Task<string> RandomStringAsync(int maxLength)
    {
      string _ret = string.Empty;

      await Task.Run(() => {
        _ret = new string(Enumerable.Repeat(@"ABCDEFGHIJKLMNOPQRSTUVWXYZ_@$5&¿?¡!#=.*-[]{}abcdefghijklmnopqrstuvwxyz0123456789", maxLength).Select(s => s[_random.Next(s.Length - 1)]).ToArray());
        Thread.Sleep(500);
      }).ConfigureAwait(false);

      return _ret;
    }

    public async Task<string> RandomStringAsync(int maxLength, string strListChars)
    {
      string _ret = string.Empty;

      await Task.Run(() => {
        _ret = new string(Enumerable.Repeat(strListChars, maxLength).Select(s => s[_random.Next(s.Length - 1)]).ToArray());
        Thread.Sleep(500);
      }).ConfigureAwait(false);

      return _ret;
    }

    public bool CheckRegularExpression(string strValue, string strPattern)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(strValue) | strValue.Length == 0)
        {
          _intNumberErr = 1301;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strMessageErrTextValueRequired")}";
        }
        else if (string.IsNullOrEmpty(strPattern) | strPattern.Length == 0)
        {
          _intNumberErr = 1302;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strMessageErrPatternRequired")}";
        }
        else
        {
          // Aplicamos las expresiones regulares.
          _bRetorno = Regex.IsMatch(strValue, strPattern);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 1300;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _bRetorno;
    }

    public async Task OpenFileByProcessAsync(string strFileName)
    {
      InitVars();

      await Task.Run(() =>
      {
        try
        {
          if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
          {
            _intNumberErr = 1401;
            _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
          }
          else
          {
            using (var oPrc = new Process())
            {
              oPrc.EnableRaisingEvents = false; Process.Start(strFileName);
            }
          }
        }
        catch (Exception oEx)
        {
          _intNumberErr = 1400;
          _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
        }
        finally
        {
          if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
        }

        Thread.Sleep(450);
      }).ConfigureAwait(false);
    }

    public async Task DestroyProcess(string strNameProcess)
    {
      InitVars();

      await Task.Run(() =>
      {
        try
        {
          if (string.IsNullOrEmpty(strNameProcess) | strNameProcess.Length == 0)
          {
            _intNumberErr = 1501;
            _strMessage = $"{_resourceData.GetString("strMessageErr")} El nombre del proceso no puede ser vacío o nulo.";
          }
          else
          {
            var pProcess = Process.GetProcesses();

            foreach (Process p in pProcess)
              if (p.ProcessName == strNameProcess) p.Kill();
          }
        }
        catch (Exception oEx)
        {
          _intNumberErr = 1500;
          _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
        }
        finally
        {
          if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
        }

        Thread.Sleep(450);
      }).ConfigureAwait(false);
    }

    public async Task ListToCSVAsync<T>(List<T> iList, string strFileName, string strSeparator) where T : new()
    {
      InitVars();

      try
      {
        if (iList == null | iList.Count == 0)
        {
          _intNumberErr = 1601;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGenericListRequired")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 1602;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!Regex.IsMatch(strFileName, @"^.*\.(txt|csv|dat)"))
        {
          _intNumberErr = 1603;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strCSVExtensionRequired")}";
        }
        else if (Regex.IsMatch(strSeparator, @"^.*\.(\||,|\t)$"))
        {
          _intNumberErr = 1604;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSeparatorRequired")}";
        }
        else
        {
          /* Codificación (UTF-8) y colección del tipo List<string> de la lista migrada. */
          var _oEncoding = Encoding.UTF8;   /* var _oEncoding = Encoding.GetEncoding (65001); */
          var _lines = new List<string>();

          /* Extraemos la lista de propiedades del tipo generico. */
          IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>();

          await Task.Run(() =>
          {
            /* De la lista de propiedades del tipo generico, armamos el encabezado. */
            var _header = string.Join(strSeparator, props.ToList().Select(x => x.Name));
            _lines.Add(_header);

            /* Contenido de la lista que se va a exportar a CSV. */
            var valueLines = iList.Select(row => string.Join(strSeparator, _header.Split(strSeparator).Select(a => row.GetType().GetProperty(a).GetValue(row, null))));
            _lines.AddRange(valueLines);

            /* Finalmente escribimos el archivo final. */
            File.WriteAllLines(strFileName, _lines.ToArray(), _oEncoding);

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 1600;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task<byte[]> FileToArrayBytesAsync(string strFileName)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(strFileName) == true | strFileName.Length == 0)
        {
          _intNumberErr = 1701;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (File.Exists(strFileName) == false)
        {
          _intNumberErr = 1702;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strFileName)}";
        }
        else
        {

          await Task.Run(() =>
          {
            using (FileStream oFs = File.OpenRead(strFileName))
            {
              _byteBuff = new byte[oFs.Length]; oFs.Read(_byteBuff, 0, Convert.ToInt32(oFs.Length));
            }

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 1700;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _byteBuff;
    }

    public async Task ArrayBytesToFileAsync(byte[] byteArray, string strFileName)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(strFileName) == true | strFileName.Length == 0)
        {
          _intNumberErr = 1801;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (byteArray.Length == 0 | byteArray == null)
        {
          _intNumberErr = 1802;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameEmpty")}";
        }
        else
        {
          /* Escribimos el arreglo de byteArray en el archivo destino. */
          await Task.Run(() =>
          {
            using (FileStream oFs = File.OpenWrite(strFileName))
            {
              /* Write to file. The 0 included is for the offset (where to start reading from). */
              oFs.Write(byteArray, 0, byteArray.Count());
            }

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 1800;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task<List<SelectWeeksYear>> SelectWeeksYearAsync(short yearValue, short monthValue)
    {
      InitVars();

      try
      {
        Weeks = new List<SelectWeeksYear>();

        if (yearValue <= 0)
        {
          _intNumberErr = 1901;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGenericListRequired")}";
        }
        else if (monthValue > 13 & monthValue < 1)
        {
          _intNumberErr = 1902;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strRangeMonthRequired")}";
        }
        else
        {
          await Task.Run(() =>
          {
            /* Extraemos la lista de las semanas comerciales. */
            var jan1 = new DateTime(yearValue, 1, 1);
            var startOfFirstWeek = jan1;
            var weeks = Enumerable.Range(0, 54).Select(i => new { WeekStart = startOfFirstWeek.AddDays((i * 7)) })
                                  .TakeWhile(x => x.WeekStart.Year <= jan1.Year)
                                  .Select(x => new { x.WeekStart, WeekFinish = x.WeekStart.AddDays(6) })
                                  .SkipWhile(x => x.WeekFinish < jan1.AddDays(1))
                                  .Select((x, i) => new { x.WeekStart, x.WeekFinish, WeekNum = i + 1, MonthNumber = x.WeekStart.Month });

            Weeks = (from A in weeks
                     where (A.MonthNumber == monthValue)
                     select new SelectWeeksYear
                     {
                       MonthNumber = (short)A.MonthNumber,
                       WeekStart = A.WeekStart,
                       WeekFinish = A.WeekFinish,
                       WeekNum = (short)A.WeekNum
                     }).ToList();

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 1900;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return Weeks;
    }

    public async Task<DataTable> ToDataTableAsync<T>(IList<T> iList) where T : new()
    {
      InitVars(); var table = new DataTable();

      try
      {
        if (iList.Count == 0 | iList == null)
        {
          _intNumberErr = 2001;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGenericListRequired")}";
        }
        else
        {
          await Task.Run(() =>
          {
            /* Extraemos la lista de propiedades del tipo generico. */
            IEnumerable<PropertyDescriptor> properties = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>();

            foreach (PropertyDescriptor prop in properties)
              table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in iList)
            {
              var row = table.NewRow();
              foreach (PropertyDescriptor prop in properties)
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

              table.Rows.Add(row);
            }

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 2000;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return table;
    }

    public async Task ConvertFileEncodingAsync(string strFileNameIn, string strFileNameOut, Encoding sourceEncoding, Encoding destEncoding)
    {
      InitVars(); string _strFileTemporalName = string.Empty; // Variable auxiliar para el nombre del archivo.

      try
      {
        /* Obtengo la lista de todas las posibles codificaciones. */
        var _oLstEncodings = Encoding.GetEncodings().Select(o => new { Nombre = o.Name }).ToList();

        if (_oLstEncodings.Count == 0)
        {
          _intNumberErr = 2101;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strEncodingListEmpty")}";
        }
        else
        {
          /* Verifico la existencia de las codificaciones de texto. */
          var _IfExistsEncodingIni = _oLstEncodings.Where(u => u.Nombre == sourceEncoding.BodyName).ToList();
          var _IfExistsEncodingFin = _oLstEncodings.Where(u => u.Nombre == destEncoding.BodyName).ToList();

          if (_IfExistsEncodingIni.Count == 0 | _IfExistsEncodingFin.Count == 0)
          {
            _intNumberErr = 2102;
            _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strEncodingInitEmpty")}";
          }
          else
          {
            if (string.IsNullOrEmpty(strFileNameIn) == true | strFileNameIn.Length == 0)
            {
              _intNumberErr = 2103;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileInitEmpty")}.";
            }
            else if (string.IsNullOrEmpty(strFileNameOut) == true | strFileNameOut.Length == 0)
            {
              _intNumberErr = 2104;
              _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileEndEmpty")}";
            }
            else
            {
              /* Si el archivo destino no existe, se crea. */
              if (File.Exists(strFileNameOut))
                File.Delete(strFileNameOut);

              /* Creamos un archivo temporal. */
              _strFileTemporalName = Path.GetTempFileName();

              /* Leemos la información del archivo de origen con la codificación de origen. */
              using (StreamReader sr = new StreamReader(strFileNameIn, sourceEncoding, false))
              {
                /* Escribo la información del archivo de origen al archivo temporal con la nueva configuración de codificación. */
                using (StreamWriter sw = new StreamWriter(_strFileTemporalName, false, destEncoding))
                {
                  int charsRead;
                  char[] buffer = new char[128 * 1024];
                  while ((charsRead = sr.ReadBlock(buffer, 0, buffer.Length)) > 0)
                    await sw.WriteAsync(buffer, 0, charsRead);
                }
              }

              /* Eliminamos el archivo destino, si existe y movemos el contenido del mismo al nuevo destino final. */
              File.Move(_strFileTemporalName, strFileNameOut, true);
            }
          }
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 2100;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); } 
      }
    }

    public async Task<Collection<T>> ToCollectionAsync<T>(List<T> iList) where T : new()
    {
      InitVars(); var collection = new Collection<T>();

      try
      {
        if (iList == null | iList.Count == 0)
        {
          _intNumberErr = 2201;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strGenericListRequired")}";
        }
        else
        {
          await Task.Run(() =>
          {
            /* Leemos el objeto List y pasamos los datos al objeto 'Collection'. */
            for (int i = 0; i < iList.Count; i++)
              collection.Add(iList[i]);

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 2200;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return collection;
    }

    public async Task<List<T>> DataReaderMapToListAsync<T>(IDataReader drReader)
    {
      InitVars(); var list = new List<T>(); var obj = default(T);

      try
      {
        if (drReader == null | !((System.Data.Common.DbDataReader)drReader).HasRows | drReader.FieldCount == 0)
        {
          _intNumberErr = 2301;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strDataReaderEmpty")}.";
        }
        else
        {
          await Task.Run(() =>
          {
            /* Leemos el DataReader cargando primero las propiedades y datos y lo pasamos a la lista. */
            while (drReader.Read())
            {
              obj = Activator.CreateInstance<T>();
              foreach (PropertyInfo prop in obj.GetType().GetProperties())
              {
                if (!Equals(drReader[prop.Name], DBNull.Value))
                  prop.SetValue(obj, drReader[prop.Name], null);
              }

              list.Add(obj);
            }

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch(Exception oEx)
      {
        _intNumberErr = 2300;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return list;
    }

    public async Task<byte[]> StringToArrayBytesAsync(string strValue)
    {
      InitVars(); byte[] _bytesArray = null;

      try
      {
        if (string.IsNullOrEmpty(strValue) | strValue.Length == 0)
        {
          _intNumberErr = 2401;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strMessageErrTextValueRequired")}";
        }
        else
        {
          _bytesArray = new byte[strValue.Length * sizeof(char)];
          await Task.Run(() => {
            Buffer.BlockCopy(strValue.ToCharArray(), 0, _bytesArray, 0, _bytesArray.Length); Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 2400;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _bytesArray;
    }

    public async Task<string> ArrayByteToStringAsync(byte[] byteArray)
    {
      InitVars(); char[] _chars = null;

      try
      {
        if (byteArray.Length == 0 | byteArray == null)
        {
          _intNumberErr = 2501;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strArrayByteRequired")}";
        }
        else
        {
          /* Convertimos el arreglo de bytes a cadena de texto. */
          await Task.Run(() => {
            _chars = new char[byteArray.Length / sizeof(char)];
            Buffer.BlockCopy(byteArray, 0, _chars, 0, byteArray.Length); Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 2500;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return new string(_chars);
    }

    public async Task<string> DataReaderToJSONAsync(IDataReader drReader)
    {
      InitVars(); var _strRetJSON = string.Empty;

      try
      {
        if (drReader == null | !((System.Data.Common.DbDataReader)drReader).HasRows | drReader.FieldCount == 0)
        {
          _intNumberErr = 2601;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strDataReaderEmpty")}.";
        }
        else
        {
          await Task.Run(() => {
            var rows = ConvertToDictionary(drReader);
            _strRetJSON = JsonConvert.SerializeObject(rows, Formatting.None);

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 2600;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _strRetJSON;
    }

    public async Task<List<T>> ConvertDataTableAsync<T>(DataTable dtDataSet) where T : new()
    {
      InitVars(); var data = new List<T>();

      try
      {
        if (dtDataSet.Rows.Count == 0 | dtDataSet == null)
        {
          _intNumberErr = 2701;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strDataTableEmpty")}";
        }
        else
        {
          await Task.Run(() => 
          {
            /* Obtengo el nombre de las columnas. */
            var columnNames = dtDataSet.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

            /* Obtengo el nombre de las propiedades del objeto. */
            var properties = typeof(T).GetProperties();

            /* Obtengo los registros. */
            DataRow[] rows = dtDataSet.Select();

            /* Transformo los elementos "row" en objeto generico y los voy guardando en una lista generica. */
            data = rows.Select(row =>
            {
              var objT = Activator.CreateInstance<T>();
              foreach (var pro in properties)
              {
                if (columnNames.Contains(pro.Name))
                {
                  PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                  pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
                }
              }
              return objT;
            }).ToList();

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 2700;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return data;
    }

    public async Task DataTableToCSVAsync(DataTable dtDataSet, string strFileName, string strSeparator)
    {
      InitVars();

      try
      {
        // Codificación (UTF-8)
        var _oEncoding = Encoding.GetEncoding(65001);

        if (dtDataSet == null || dtDataSet.Rows.Count == 0)
        {
          _intNumberErr = 2801;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strDataTableEmpty")}";
        }
        else if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 2802;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!Regex.IsMatch(strFileName, @"^.*\.(txt|csv|dat)"))
        {
          _intNumberErr = 2803;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strCSVExtensionRequired")}";
        }
        else if (CheckRegularExpression(strSeparator, @"^.*\.(\||,|\t)$"))
        {
          _intNumberErr = 2804;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSeparatorRequired")}";
        }
        else
        {
          await Task.Run(() =>
          {
            var _lines = new List<string>();                  /* Lineas finales donde se guarda a archivo. */
            var _list = dtDataSet.AsEnumerable().ToList();    /* Convierto el DataTable en IEnumerable. */

            /* Obtengo el nombre de las columnas del DataTable. */
            var _header = dtDataSet.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            _lines.Add(string.Join(strSeparator, _header));

            /* Cargo el set de datos a una linea de texto. */
            _list.ForEach(v => {
              var fields = v.ItemArray.Select(field => Regex.Replace(Regex.Replace(field.ToString(), Patterns.AlphaLatinPattern.Trim(), string.Empty), @"\r\n?|\n", string.Empty));
              _lines.Add(string.Join(strSeparator, fields));
            });

            /* Finalmente escribimos el archivo final. */
            File.WriteAllLines(strFileName, _lines.ToArray(), _oEncoding);

            Thread.Sleep(450);
          }).ConfigureAwait(false);

        } // Fin de la validación de registros.
      }
      catch (Exception oEx)
      {
        _intNumberErr = 2800;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public List<string> SelectFilesByFolderFullPaths(string strFolderName, string strFilter)
    {
      try
      {
        InitVars(); _Lista = new List<string>();

        if (string.IsNullOrEmpty(strFolderName) | strFolderName.Length == 0)
        {
          _intNumberErr = 2901; _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFolderEmpty")}.";
        }
        else if (string.IsNullOrEmpty(strFilter) | strFilter.Length == 0)
        {
          _intNumberErr = 2902; _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFilterFileEmpty")}";
        }
        else
        {
          // Aplicamos las expresiones regulares.
          _Lista = Directory.GetFiles(strFolderName, strFilter).ToList();
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 2900; 
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _Lista;
    }

    public List<string> SelectFilesByFolderFileNames(string strFolderName, string strFilter)
    {
      try
      {
        _intNumberErr = 0; _strMessage = string.Empty; _Lista = new List<string>();

        if (string.IsNullOrEmpty(strFolderName) | strFolderName.Length == 0)
        {
          _intNumberErr = 3001; _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFolderEmpty")}.";
        }
        else if (string.IsNullOrEmpty(strFilter) | strFilter.Length == 0)
        {
          _intNumberErr = 3002; _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFilterFileEmpty")}";
        }
        else
        {
          // Aplicamos las expresiones regulares.
          _Lista = Directory.GetFiles(strFolderName, strFilter).Select(f => Path.GetFileName(f)).ToList();
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 3000; _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _Lista;
    }

    public Task<string> ExecuteRequestToReadTask(string urlSource, EnumRequestType typeRequest, IReadOnlyDictionary<string, string> headersArray, string parametersToJSONString)
    {
      // Adding the support for TLS 1.2 protocol (we need this line in case of use HTTPS address of this provider
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;

      using (HttpClient client = new HttpClient())
      {
        HttpResponseMessage response;

        // Validamos la existencia de Headers.
        foreach (var header in headersArray)
          client.DefaultRequestHeaders.Add(header.Key, header.Value);

        // Ejecutamos la petición asíncrona
        switch (typeRequest)
        {
          case EnumRequestType.GET:
            response = client.GetAsync(urlSource, HttpCompletionOption.ResponseContentRead).Result;
            break;
          case EnumRequestType.POST:
            response = client.PostAsync(urlSource, new StringContent(parametersToJSONString, Encoding.UTF8, "application/json")).Result;
            break;
          case EnumRequestType.PUT:
            response = client.PutAsync(urlSource, new StringContent(parametersToJSONString, Encoding.UTF8, "application/json")).Result;
            break;
          default:
            response = client.DeleteAsync(urlSource).Result;
            break;
        }

        // Devolvemos el resultado.
        return response.Content.ReadAsStringAsync();
      }
    }

    public async Task<string> ImageToBase64Async(string strImageFileName)
    {
      InitVars(); var _retBase64 = string.Empty;

      try
      {
        if (string.IsNullOrEmpty(strImageFileName) | strImageFileName.Length == 0)
        {
          _intNumberErr = 3101;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!Regex.IsMatch(strImageFileName, Patterns.ImageExtensionFilePattern))
        {
          _intNumberErr = 3102;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFormatImageRequired")}";
        }
        else if (!File.Exists(strImageFileName))
        {
          _intNumberErr = 3103;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {string.Format(_resourceData.GetString("strFileNameExists"), strImageFileName)}";
        }
        else
        {
          /* Cargamos el archivo de imagen. */
          await Task.Run(() => {
            using (var _image = Image.FromFile(strImageFileName))
            {
              using (var _m = new MemoryStream())
              {
                _image.Save(_m, _image.RawFormat);
                byte[] _imageBytesArray = _m.ToArray();
                _retBase64 = Convert.ToBase64String(_imageBytesArray);
              }
            }

            /* Generamos el metadata URI de la imagen. */
            var _extensionFile = (Path.GetExtension(strImageFileName).Replace(".", string.Empty).ToLower() == "jpg" | Path.GetExtension(strImageFileName).Replace(".", string.Empty).ToLower() == "jpeg") ? "data:image/jpeg;base64," :
                                 (Path.GetExtension(strImageFileName).Replace(".", string.Empty).ToLower() == "gif") ? "data:image/gif;base64," :
                                 (Path.GetExtension(strImageFileName).Replace(".", string.Empty).ToLower() == "bmp") ? "data:image/bmp;base64," :
                                 (Path.GetExtension(strImageFileName).Replace(".", string.Empty).ToLower() == "webp") ? "data:image/webp;base64," : "data:image/svg\\+xml; base64,";

            _retBase64 = _extensionFile + _retBase64;

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 3100;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _retBase64;
    }

    public async Task Base64StringToImage(string strValueBase64, string strImageFileName)
    {
      InitVars();

      try
      {
        if (string.IsNullOrEmpty(strImageFileName) | strImageFileName.Length == 0)
        {
          _intNumberErr = 3201;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!Regex.IsMatch(strImageFileName, Patterns.ImageExtensionFilePattern))
        {
          _intNumberErr = 3202;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFormatImageRequired")}";
        }
        else if (!Regex.IsMatch(strValueBase64, Patterns.Base64ImageURIPattern))
        {
          _intNumberErr = 3203;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strBase64URIInvalid")}";
        }
        else
        {
          /* Cargamos el archivo de imagen. */
          await Task.Run(() => {
            /* Aquí removemos el metadata de la URI de la imagen. */
            var _dataBase64String = strValueBase64.Substring((strValueBase64.IndexOf(",", StringComparison.Ordinal) + 1), (strValueBase64.Length - (strValueBase64.IndexOf(",", StringComparison.Ordinal) + 1)));

            /* El resto de la cadena de texto, lo usamos para crear el archivo de imagen final. */
            byte[] _imageBytes = Convert.FromBase64String(_dataBase64String);
            MemoryStream _ms = new MemoryStream(_imageBytes, 0, _imageBytes.Length);
            _ms.Write(_imageBytes, 0, _imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(_ms, true);
            image.Save(strImageFileName);

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 3200;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }
    }

    public async Task<List<T>> ConvertCSVToListAsync<T>(string strFileName, string strSeparator) where T : new()
    {
      InitVars(); var _iListRet = new List<T>();

      try
      {
        if (string.IsNullOrEmpty(strFileName) | strFileName.Length == 0)
        {
          _intNumberErr = 3301;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFileNameRequired")}";
        }
        else if (!Regex.IsMatch(strFileName, @"^.*\.(txt|csv|dat)"))
        {
          _intNumberErr = 3302;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strCSVExtensionRequired")}";
        }
        else if (Regex.IsMatch(strSeparator, @"^.*\.(\||,|\t)$"))
        {
          _intNumberErr = 3303;
          _strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strSeparatorRequired")}";
        }
        else
        {
          await Task.Run(() =>
          {
            using (StreamReader streamReader = new StreamReader(strFileName))
            {
              string[] headers = streamReader.ReadLine().Split(new string[] { strSeparator }, StringSplitOptions.None);
              ReadHeader<T>(headers);

              while (!streamReader.EndOfStream)
              {
                T item = new T();

                string[] rowData = streamReader.ReadLine().Split(new string[] { strSeparator }, StringSplitOptions.None);

                for (int index = 0; index < headers.Length; index++)
                {
                  string header = headers[index];
                  var valuetype = _headerPropertyInfos[header];

                  switch (valuetype.PropertyType.Name)
                  {
                    case "Guid":
                      _headerPropertyInfos[header].SetValue(item, Convert.ChangeType(new Guid(rowData[index]), _headerDaytaTypes[header]), null);
                      break;
                    default:
                      _headerPropertyInfos[header].SetValue(item, Convert.ChangeType(rowData[index], _headerDaytaTypes[header]), null);
                      break;
                  }
                }

                /* Cargo finalmente el elemento "casteado" a la lista. */
                _iListRet.Add(item);
              }
            }

            Thread.Sleep(450);
          }).ConfigureAwait(false);
        }
      }
      catch (Exception oEx)
      {
        _intNumberErr = 3300;
        _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
      }
      finally
      {
        if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
      }

      return _iListRet;
    }

    #region "Funciones privadas."

    /// <summary>
    /// Convertir un objeto DataReader en una lista de diccionario.
    /// </summary>
    /// <param name="drReader">Objeto IDataReader.</param>
    /// <returns>Devuelve una colección del tipo lista diccionario con los datos del objeto 'IDataReader'.</returns>
    protected IEnumerable<Dictionary<string, object>> ConvertToDictionary(IDataReader drReader)
    {
      var columns = new List<string>();
      var rows = new List<Dictionary<string, object>>();

      for (var i = 0; i < drReader.FieldCount; i++)
        columns.Add(drReader.GetName(i));

      while (drReader.Read())
        rows.Add(columns.ToDictionary(column => column, column => drReader[column]));

      return rows;
    }

    /// <summary>
    /// Función privada que lee el encabezado del archivo de texto plano para validar si los campos cumplen con los atributos de importación a CSV.
    /// </summary>
    /// <typeparam name="T">Tipo generico.</typeparam>
    /// <param name="headers">Arreglo del tipo "string" que contiene los nombres de los encabezados.</param>
    private void ReadHeader<T>(string[] headers)
    {
      foreach (string header in headers)
      {
        foreach (var propertyInfo in (typeof(T)).GetProperties())
        {
          foreach (object attribute in propertyInfo.GetCustomAttributes(true))
          {
            if (attribute is ColumnAttribute)
            {
              ColumnAttribute columnAttribute = attribute as ColumnAttribute;

              if (columnAttribute.Name == header)
              {
                _headerPropertyInfos[header] = propertyInfo;
                _headerDaytaTypes[header] = columnAttribute.DataType;
                break;
              }
            }
          }
        }
      }
    }

    #endregion

  }
}

