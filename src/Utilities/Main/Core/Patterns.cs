// Archivo: Patterns.cs
// Clase 'Patterns' que contiene valores definidos para la validacion de expresiones por medio de expresiones regulares.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
//© Olimpo Bonilla Ramírez. 2016-2021. All rights reserved.

using System;

namespace Utilities
{
  /// <summary>
  /// Clase 'Patterns' que contiene valores definidos para la validacion de expresiones por medio de expresiones regulares.
  /// </summary>
  [Serializable]
  public class Patterns
  {
    /// <summary>
    /// Clave privada RSA.
    /// </summary>
    public const string RSAPrivateKey = @"<RSAKeyValue><Modulus>xXDHODVaBtLnqPHhFUPYGyUw/Tu91w0S+gCgSqHZ9mSsSmBH4cPAA7kv+kcWrjpMnC6x2LKTRLFItXlZHmyxhR44piUShEHGjUOD9MUurZAPaQ/TJstYEseKRcCHOM0g6Rkz5iXLfg21AeAPvP5H9DXGcjM7+W3+eghCwL/Xd6E=</Modulus><Exponent>AQAB</Exponent><P>/KqPsKVP+qq0YAg7s10D+LkTT4UXiyUuwJ0aH23IvOa3WvjpvofcCZknFupEZCZO590S9Jvh1z7Ur0kJ3MRpjw==</P><Q>yAut5cJ31nEU8t/h3R9GJhiUIXSfSlBnVo4JyeNZmfLhMzqP9B7Z105tljqHYol5GaXA1BhoV0Bsp3GbloeTzw==</Q><DP>HSdpEQ7iHaBY08Pfb6DJ9ocUbVaEEvAlZuA5xWqbgQB2I68Y+X0frJPZaNF7Nrsc27AsocDi3D1JuTgKRUj8jQ==</DP><DQ>UWU/Kp+H9z8n0PL7iedxuYqCq2IydiBlC7jbjsPs71D+BMQtUp1C8fI9cMT71UIufhsNbL9JpUbXbwIw1nzvzQ==</DQ><InverseQ>mcXtnbBYM6Z52nMc8N4XA7HqecAf1htu66v1F007+MtxLDf9xlyCfLRbFSf1FNcpb2J7i7FqbZjgZ+CRFss9QA==</InverseQ><D>G0kfrwvoeiLxcu6ifY7XscCtCYIVFUAwTC6D+Gxvs9Zr3Qd7R58GzTztfXgPkVprNfZaC6l6IrchZjrz7vl0esOG+y4kOFCVuOhqkQ4uTQHcWAK85/UiLLsvGn0kc2w7JM1g7JvpDGTsqterklRtikZaxezQ/Y4X5dzbA3LLn8U=</D></RSAKeyValue>";
    /// <summary>
    /// Clave publica RSA.
    /// </summary>
    public const string RSAPublicKey = @"<RSAKeyValue><Modulus>xXDHODVaBtLnqPHhFUPYGyUw/Tu91w0S+gCgSqHZ9mSsSmBH4cPAA7kv+kcWrjpMnC6x2LKTRLFItXlZHmyxhR44piUShEHGjUOD9MUurZAPaQ/TJstYEseKRcCHOM0g6Rkz5iXLfg21AeAPvP5H9DXGcjM7+W3+eghCwL/Xd6E=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
    /// <summary>
    /// Patron de cadena sin caracteres especiales.
    /// </summary>
    public const string AlphaPattern = @"^[A-Z0-9 a-záéíóúAÉÍÓÚÑñ.,&;/-]{1,255}$";
    /// <summary>
    /// Patron numerico decimal.
    /// </summary>
    public const string DecimalPattern = @"^-?[0-9]{1,100}(\.[0-9]{0,4})?$";
    /// <summary>
    /// Patron numerico entero.
    /// </summary>
    public const string IntegerPattern = @"^-?[0-9]+\z";
    /// <summary>
    /// Patron fecha
    /// </summary>
    public const string DateTimePattern1 = @"^(?:(?:0?[1-9]|1\d|2[0-8])(\/|-)(?:0?[1-9]|1[0-2]))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^(?:(?:31(\/|-)(?:0?[13578]|1[02]))|(?:(?:29|30)(\/|-)(?:0?[1,3-9]|1[0-2])))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^(29(\/|-)0?2)(\/|-)(?:(?:0[48]00|[13579][26]00|[2468][048]00)|(?:\d\d)?(?:0[48]|[2468][048]|[13579][26]))$";
    /// <summary>
    /// Patron fecha version 2.
    /// </summary>
    public const string DateTimePattern2 = @"^(?:(?:(?:0?[13578]|1[02])(\/|-)31)|(?:(?:0?[1,3-9]|1[0-2])(\/|-)(?:29|30)))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^(?:(?:0?[1-9]|1[0-2])(\/|-)(?:0?[1-9]|1\d|2[0-8]))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^(0?2(\/|-)29)(\/|-)(?:(?:0[48]00|[13579][26]00|[2468][048]00)|(?:\d\d)?(?:0[48]|[2468][048]|[13579][26]))$";
    /// <summary>
    /// Patron fecha version 3.
    /// </summary>
    public const string DateTimePattern3 = @"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$";
    /// <summary>
    /// Patrón fecha en formato MM/dd/YYYY HH:MM:SS.fff AM/PM.
    /// </summary>
    public const string DateTimePattern4 = @"(?n:^(?=\d)((?<month>(0?[13578])|1[02]|(0?[469]|11)(?!.31)|0?2(?(.29)(?=.29.((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(16|[2468][048]|[3579][26])00))|(?!.3[01])))(?<sep>[-./])(?<day>0?[1-9]|[12]\d|3[01])\k<sep>(?<year>(1[6-9]|[2-9]\d)\d{2})(?(?=\x20\d)\x20|$))?(?<time>((0?[1-9]|1[012])(:[0-5]\d){0,2}(?i:\x20[AP]M))|([01]\d|2[0-3])(:[0-5]\d){1,2})?$)";
    /// <summary>
    /// Patron direccion IP.
    /// </summary>
    public const string IPPattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
    /// <summary>
    /// Patron direccion URL.
    /// </summary>
    public const string URLPattern = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
    /// <summary>
    /// Patron direccion FTP.
    /// </summary>
    public const string FtpPattern = @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)( [a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$";
    /// <summary>
    /// Patrón correo electronico.
    /// </summary>
    public const string EmailPattern = @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$";
    /// <summary>
    /// Identificador único para el cifrado Rijndael.
    /// </summary>
    public const string GUIDSeed = @"D005D166-A282-494C-A496-932DA0DAA62F";
    /// <summary>
    /// Patrón caracteres latinos.
    /// Fuente: https://www.rapidtables.com/code/text/unicode-characters.html
    /// </summary>
    public const string AlphaLatinPattern = @"[^\s\u0022\u0023\u0024\u0025\u0026\u0027\u0028\u0029\u002A\u002B\u002D\u002E\u002F\u0030-\u0039\u003A\u003B\u003C\u003D\u003E\u003F\u0040\u0041-\u005A\u005B\u005C\u005D\u005F\u0061-\u007A\u007B\u007D\u00A9\u00AE\u00B1\u00BA\u00C1\u00C9\u00CD\u00D3\u00DA\u00E1\u00E9\u00ED\u00F3\u00FA\u00FC\u00F1\u00D1\u00A1\u00A2\u00A3\u00A3\u008F\u008E\u00C7\u00E7\u00BF\u00AF\u221E\u20AC\u2122]";
    /// <summary>
    /// Información del autor del ensamblado.
    /// </summary>
    public const string Author = @"OLIMPO BONILLA RAMIREZ";
    /// <summary>
    /// Patrón RFC para personas físicas o morales (México).
    /// </summary>
    public const string RFCSATPattern = @"^([A-ZÑ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1]))([A-Z\d]{3})?$";
    /// <summary>
    /// Patrón CURPPattern para personas físicas (México).
    /// </summary>
    public const string CURPPattern = @"^([A-Z][AEIOUX][A-Z]{2}\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])[HM](?:AS|B[CS]|C[CLMSH]|D[FG]|G[TR]|HG|JC|M[CNS]|N[ETL]|OC|PL|Q[TR]|S[PLR]|T[CSL]|VZ|YN|ZS)[B-DF-HJ-NP-TV-Z]{3}[A-Z\d])(\d)$";
    /// <summary>
    /// Patrón de tarjeta de débito o crédito para VISA, Master Card y Discover.
    /// Fuente: http://w3.unpocodetodo.info/utiles/regex-ejemplos.php?type=cc
    /// </summary>
    public const string CreditCardPattern = @"^(?:4\d([\- ])?\d{6}\1\d{5}|(?:4\d{3}|5[1-5]\d{2}|6011)([\- ])?\d{4}\2\d{4}\2\d{4})$";
    /// <summary>
    /// Patrón de la cadena de texto en Base 64.
    /// </summary>
    public const string Base64StringPattern = @"^[a-zA-Z0-9\+/]*={0,3}$";
    /// <summary>
    /// Patrón de la cadena de texto Base64 del tipo URL para imágenes.
    /// </summary>
    public const string Base64ImageURIPattern = @"^data:image\/(?:gif|png|jpeg|jpg|bmp|webp|svg\+xml)(?:;charset=utf-8)?;base64,(?:[A-Za-z0-9]|[+/])+={0,2}$";
    /// <summary>
    /// Patrón de extensiones de imagenes válidas.
    /// </summary>
    public const string ImageExtensionFilePattern = @"^.*\.(jpg|JPG|gif|GIF|jpeg|JPEG|bmp|BMP|png|PNG|webp|WEBP|svg\+xml|SVG\+XML)$";
    /// <summary>
    /// Patrón de extensiones de archivos de documentos portables válidos.
    /// </summary>
    public const string PortableDocumentExtensionPattern = @"^.*\.(jpg|JPG|gif|GIF|jpeg|JPEG|bmp|BMP|TIFF|tiff|pdf|PDF|png|PNG|webp|WEBP|svg\+xml|SVG\+XML)$";

    /// <summary>
    /// Patrón de extensiones de archivos de formato JSON válidos.
    /// </summary>
    public const string JSONExtensionPattern = @"^.*\.(json|JSON)$";

    /// <summary>
    /// Patrón de extensiones de archivos de documentos portables válidos.
    /// </summary>
    public const string KeyExtensionPattern = @"^.*\.(pfx|PFX|p12|P12)$";

    /// <summary>
    /// Patrón para validar valores Guid.
    /// </summary>
    public const string GuidPattern = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";

    // Pueden agregarse aquí mas definiciones estandar de patrones de texto.
    // ---------------------------------------------------------------------
    // ...
  }
}
