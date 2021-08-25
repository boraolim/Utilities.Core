// Archivo: IMessageService.cs
// Interface 'IMessageService' para el manejo de envío de mensajes a dispositivos móviles y correos electrónicos.
// 
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 20 de marzo de 2016.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2020. All rights reserved

using System.Net;
using System.Threading.Tasks;

namespace Utilities
{
  /// <summary>
  /// Interface 'ISmsService' para el manejo de envío de mensajes a dispositivos móviles y correos electrónicos.
  /// </summary>
  public interface ISMSService
  {
		/// <summary>
		/// Proveedor de servicios. Generalmente es una API de envío de SMS.
		/// </summary>
		public string smsProviderLink { get; set; }

		/// <summary>
		/// Parámetros de envío hacia el servicio.
		/// </summary>
		public string smsParametersLink { get; set; }

		/// <summary>
		/// Usuario.
		/// </summary>
		public string smsUser { get; set; }

		/// <summary>
		/// Contraseña.
		/// </summary>
		public string smsSecretKey { get; set; }

		/// <summary>
		/// Texto o flag que indica si el mensaje fue realizado.
		/// </summary>
		public string smsConfirmationSucessFull { get; set; }

		/// <summary>
		/// Objeto proxy.
		/// </summary>
		public WebProxy smsProxy { get; set; }

		/// <summary>
		/// Función que envía un mensaje de texto a un número móvil.
		/// </summary>
		/// <param name="strNumberMobile">Número de telefono móvil.</param>
		/// <param name="strMessageText">Mensaje de texto.</param>
		/// <remarks>Si el envío de SMS se realizó de manera correcta, fue satisfactorio. En caso contrario, lanza una excepción. Esto se debe a un error del servicio de mensajería donde se realiza el proceso de envío.</remarks>
		public Task SendSmsMessage(string strNumberMobile, string strMessageText);
  }
}
