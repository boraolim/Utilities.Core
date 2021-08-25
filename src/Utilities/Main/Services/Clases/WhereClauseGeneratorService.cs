// Clase: WhereClauseGeneratorService.cs.
// Clase 'WhereClauseGenerator' para el armado de sentencias dinamicas SQL para la consulta a Base de Datos por medio del modelo ORM/Data.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2020. All rights reserved

using System;
using System.Linq;
using System.Text;
using System.Resources;
using System.Threading;
using System.Reflection;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using Utilities.Resources;

namespace Utilities
{
  /// <summary>
  /// Clase 'WhereClauseGeneratorService' para el armado de sentencias dinamicas SQL para la consulta a Base de Datos por medio del modelo ORM/Data.
  /// </summary>
  [Serializable]
  public class WhereClauseGeneratorService : IWhereClauseGeneratorService
  {
    internal ushort _intNumberErr;
    internal string _strMessage;
    internal string _strRet;
    internal bool _iEvalCond;
    protected ResourceManager _resourceData;

    /// <summary>
    /// Array de cadenas strings para los identificadores de condicion para Linq.
    /// </summary>
    internal readonly string[] FormatMappingToDynamicLinq =
    {
      " {0} = {1} ",                                        // "eq" - equal
      " {0} != {1} ",                                       // "ne" - not equal
      " {0} < {1} ",                                        // "lt" - less than
      " {0} <= {1} ",                                       // "le" - less than or equal to
      " {0} > {1} ",                                        // "gt" - greater than
      " {0} >= {1} ",                                       // "ge" - greater than or equal to
      " {0}.StartsWith({1}) ",                              // "bw" - begins with => {0} LIKE '{1}%'
      " !{0}.StartsWith({1}) ",                             // "bn" - does not begin with => {0} NOT LIKE '{1}%'
      " {0}.EndsWith({1}) ",                                // "ew" - ends with => {0} LIKE '%{1}'
      " !{0}.EndsWith({1}) ",                               // "en" - does not end with => {0} NOT LIKE '%{1}'
      " {0}.Contains({1}) ",                                // "cn" - contains => {0} LIKE '%{1}%'
      " !{0}.Contains({1}) "                                // "nc" - does not contain => {0} NOT LIKE '%{1}%'
    };

    /// <summary>
    /// Array de cadenas strings para los identificadores de condicion para Entity Framework (Correcto).
    /// </summary>
    internal readonly string[] FormatMappingToEF =
    {
      " (it.{0} = {1}) ",                                   // "eq" - equal
      " (it.{0} != {1}) ",                                  // "ne" - not equal
      " (it.{0} < {1}) ",                                   // "lt" - less than
      " (it.{0} <= {1}) ",                                  // "le" - less than or equal to
      " (it.{0} > {1}) ",                                   // "gt" - greater than
      " (it.{0} >= {1}) ",                                  // "ge" - greater than or equal to
      " (it.{0}.StartsWith({1})) ",                         // "bw" - begins with
      " !(it.{0}.StartsWith({1})) ",                        // "bn" - does not begin with
      " (it.{0}.EndsWith({1})) ",                           // "ew" - ends with
      " !(it.{0}.EndsWith({1})) ",                          // "en" - does not end with
      " (it.{0}.Contains({1})) ",                           // "cn" - contains
      " !(it.{0}.Contains({1})) "                           // "nc" - does not contain
    };

    /// <summary>
    /// Creación de variables locales.
    /// </summary>
    protected void InitVars()
    {
      _intNumberErr = 0; _strMessage = string.Empty; _strRet = string.Empty; _iEvalCond = false;
      _resourceData = new ResourceManager(typeof(LanguageSource));
    }

    public async Task<string> ParserFilterToDynamicLinqAsync<T>(WhereFilter Filtro) where T : new()
    {
      InitVars(); var oSb = new StringBuilder();

      await Task.Run(() =>
      {
        try
        {
          if (Filtro != null)
          {
            if (Filtro.rules.Count > 0 && Filtro.rules != null)
            {
              var _props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>().ToList();

              foreach (var oRuleTemp in Filtro.rules)
              {
                var _oProps = _props.Where(u => u.Name == oRuleTemp.field).Single();

                if (_oProps == null)
                  continue; // skip wrong entries

                if (oSb.Length != 0)
                  oSb.Append(Filtro.groupOp.ToString());

                switch (_oProps.PropertyType.FullName)
                {
                  case "System.SByte": // tinyint
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Int16": // smallint
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Int32": // int
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Int64": // bigint
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Single": // Edm.Single, in SQL: float
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Double": // float(53), double precision
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Boolean": // Edm.Boolean, in SQL: bit
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 1 ? true : false;

                    if (_iEvalCond)
                    {
                      switch (oRuleTemp.data)
                      {
                        case "1":
                          oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, true);
                          break;
                        case "yes":
                          oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, false);
                          break;
                        case "true":
                          oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, true);
                          break;
                        default:
                          oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, false);
                          break;
                      }

                      //oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, string.Compare(oRuleTemp.data, "1", StringComparison.Ordinal) == 0 &&
                      //                                                                                 string.Compare(oRuleTemp.data, "yes", StringComparison.OrdinalIgnoreCase) == 0 &&
                      //                                                                                 string.Compare(oRuleTemp.data, "true", StringComparison.OrdinalIgnoreCase) == 0 ? true : false);
                    }
                    break;
                  case "System.DateTime": // smalldatetime, datetime
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, string.Format("Convert.ToDateTime(\"{0}\")", oRuleTemp.data)); }
                    break;
                  case "System.String": // nvarchar, varchar, char
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 1 || ((int)oRuleTemp.op >= 6 && (int)oRuleTemp.op <= 11) ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, "\"" + oRuleTemp.data + "\""); }
                    break;
                  case "System.Guid": // Guid
                    _iEvalCond = ((int)oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 1) ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, "\"" + oRuleTemp.data + "\""); }
                    break;
                  default:

                    // Verificamos si los tipos de datos son nulos. Si no son nulos y son de otro tipo creamos un filtro sencillo.
                    if (_oProps.PropertyType.FullName.Contains("System.Nullable`1[[System.Int16")) // Entero de 16 bits nulo.
                    {
                      _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                      if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    }
                    else if (_oProps.PropertyType.FullName.Contains("System.Nullable`1[[System.Int32")) // Entero de 32 bits nulo.
                    {
                      _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                      if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    }
                    else if (_oProps.PropertyType.FullName.Contains("System.Nullable`1[[System.Int64")) // Entero de 64 bits nulo.
                    {
                      _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                      if (_iEvalCond) { oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    }
                    else
                    {
                      oSb.AppendFormat(FormatMappingToDynamicLinq[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data);
                      _iEvalCond = true;
                    }
                    break;
                }

                if (_iEvalCond == true)
                {
                  _strRet = oSb.ToString();
                }
                else
                {
                  _intNumberErr = 3501;
                  _strMessage = $"{_resourceData.GetString("strMessageErr")} Operador condicional \'" + oRuleTemp.op + "\' no permitido para el campo o atributo \'" + oRuleTemp.field + "\'.";
                }
              }
            }
          }
        }
        catch (Exception oEx)
        {
          _intNumberErr = 3500;
          _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
        }
        finally
        {
          if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
        }

        Thread.Sleep(450);
      }).ConfigureAwait(false);

      return (_strRet);
    }

    public async Task<string> ParserFilterToEntityFrameworkAsync<T>(WhereFilter Filtro) where T : new()
    {
      InitVars(); var oSb = new StringBuilder();

      await Task.Run(() =>
      {
        try
        {
          if (Filtro != null)
          {
            if (Filtro.rules.Count > 0 && Filtro.rules != null)
            {
              var _props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>().ToList();

              foreach (var oRuleTemp in Filtro.rules)
              {
                var _oProps = _props.Where(u => u.Name == oRuleTemp.field).Single();

                if (_oProps == null)
                  continue; // skip wrong entries

                if (oSb.Length != 0)
                  oSb.Append(Filtro.groupOp.ToString());

                switch (_oProps.PropertyType.FullName)
                {
                  case "System.SByte": // tinyint
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Int16": // smallint
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Int32": // int
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Int64": // bigint
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Single": // Edm.Single, in SQL: float
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Double": // float(53), double precision
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    break;
                  case "System.Boolean": // Edm.Boolean, in SQL: bit
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 1 ? true : false;
                    if (_iEvalCond)
                    {
                      switch (oRuleTemp.data)
                      {
                        case "1":
                          oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, true);
                          break;
                        case "yes":
                          oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, false);
                          break;
                        case "true":
                          oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, true);
                          break;
                        default:
                          oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, false);
                          break;
                      }
                      //oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, string.Compare(oRuleTemp.data, "1", StringComparison.Ordinal) == 0 &&
                      //                                                                        string.Compare(oRuleTemp.data, "yes", StringComparison.OrdinalIgnoreCase) == 0 &&
                      //                                                                        string.Compare(oRuleTemp.data, "true", StringComparison.OrdinalIgnoreCase) == 0 ? true : false);
                    }
                    break;
                  case "System.DateTime": // smalldatetime, datetime
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                    //if (_iEvalCond) { oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, "DATETIME '" + DateTime.Parse(oRuleTemp.data).ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff") + "'"); }
                    if (_iEvalCond)
                    {
                      var _oDateTime = DateTime.Parse(oRuleTemp.data);
                      var _oSbFecha = new StringBuilder();
                      _oSbFecha.AppendFormat("DateTime({0}, {1}, {2}, {3}, {4}, {5})", _oDateTime.Year, _oDateTime.Month, _oDateTime.Day, _oDateTime.Hour, _oDateTime.Minute, _oDateTime.Second);
                      oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, _oSbFecha.ToString());
                    }
                    break;
                  case "System.String": // nvarchar, varchar, char, ntext
                    _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 1 || ((int)oRuleTemp.op >= 6 && (int)oRuleTemp.op <= 11) ? true : false;
                    if (_iEvalCond)
                    {
                      if (oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 1)
                      {
                        oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, "\"" + oRuleTemp.data + "\"");
                      }
                      else
                      {
                        oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, "\"" + oRuleTemp.data + "\"");
                      }
                    }
                    break;
                  case "System.Guid": // Guid
                    _iEvalCond = ((int)oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 1) ? true : false;
                    if (_iEvalCond) { oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, "\"" + oRuleTemp.data + "\""); }
                    break;
                  default:

                    // Verificamos si los tipos de datos son nulos. Si no son nulos y son de otro tipo creamos un filtro sencillo.
                    if (_oProps.PropertyType.FullName.Contains("System.Nullable`1[[System.Int16")) // Entero de 16 bits nulo.
                    {
                      _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                      if (_iEvalCond) { oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    }
                    else if (_oProps.PropertyType.FullName.Contains("System.Nullable`1[[System.Int32")) // Entero de 32 bits nulo.
                    {
                      _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                      if (_iEvalCond) { oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    }
                    else if (_oProps.PropertyType.FullName.Contains("System.Nullable`1[[System.Int64")) // Entero de 64 bits nulo.
                    {
                      _iEvalCond = oRuleTemp.op >= 0 && (int)oRuleTemp.op <= 5 ? true : false;
                      if (_iEvalCond) { oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data); }
                    }
                    else
                    {
                      oSb.AppendFormat(FormatMappingToEF[(int)oRuleTemp.op], oRuleTemp.field, oRuleTemp.data);
                      _iEvalCond = true;
                    }
                    break;
                }
                if (_iEvalCond == true)
                {
                  _strRet = oSb.ToString();
                }
                else
                {
                  _intNumberErr = 3601;
                  _strMessage = $"{_resourceData.GetString("strMessageErr")} Operador condicional \'" + oRuleTemp.op + "\' no permitido para el campo o atributo \'" + oRuleTemp.field + "\'.";
                }
              }
            }
          }
        }
        catch (Exception oEx)
        {
          _intNumberErr = 3600;
          _strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
        }
        finally
        {
          if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
        }

        Thread.Sleep(450);
      }).ConfigureAwait(false);

      return (_strRet);
    }
  }
}
