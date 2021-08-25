// Archivo: MessageSMSService.
// Clase 'MessageSMSService' para realizar envíos de correo electrónico y mensajes de texto a disposivitos móviles.
//
// Autor: Olimpo Bonilla Ramírez.
// Fecha de creación: 6 de julio de 2019.
// Fecha de compilación: 25 de agosto de 2021.
// Fecha de ultima modificación de código fuente: N/A.
// Versión del ensamblado: 1.0.24.6031.
//
// © Olimpo Bonilla Ramírez. 2016-2020. All rights reserved.

namespace Utilities
{
  using System;
  using System.Net;
  using System.Text;
  using System.Net.Http;
	using System.Resources;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Net.Http.Headers;
  
	using Utilities.Resources;
  
  /// <summary>
  /// Clase 'MessageSMSService' para realizar envíos de correo electrónico y mensajes de texto a disposivitos móviles.
  /// </summary>	
  public class MessageSMSService : ISMSService
  {
		/* Objetos no administrados (variables locales a nivel de la clase). */
		protected ushort _intNumberErr;
		protected string _strMessage;
		protected ResourceManager _resourceData;

		/// <summary>
		/// URL o proveedor de servicio de mensajería SMS.
		/// </summary>
		private string _smsProviderLink { get; set; }

		/// <summary>
		/// Parámetros de envío hacia el servicio de mensajería SMS.
		/// </summary>
		private string _smsParametersLink { get; set; }

		/// <summary>
		/// Usuario.
		/// </summary>
		private string _smsUser { get; set; }

		/// <summary>
		/// Contraseña.
		/// </summary>
		private string _smsSecretKey { get; set; }

		/// <summary>
		/// Texto o flag que indica si el mensaje fue realizado.
		/// </summary>
		private string _smsConfirmationSucessFull { get; set; }

		/// <summary>
		/// Objeto proxy.
		/// </summary>
		private WebProxy _smsProxy { get; set; }

		/* Asignando atributos a la interfaz. */
		public string smsProviderLink { get => _smsProviderLink; set => _smsProviderLink = value; }
    public string smsParametersLink { get => _smsParametersLink; set => _smsParametersLink = value; }
    public string smsUser { get => _smsUser; set => _smsUser = value; }
    public string smsSecretKey { get => _smsSecretKey; set => _smsSecretKey = value; }
    public string smsConfirmationSucessFull { get => _smsConfirmationSucessFull; set => _smsConfirmationSucessFull = value; }
    public WebProxy smsProxy { get => _smsProxy; set => _smsProxy = value; }

		/// <summary>
		/// Creación de variables locales.
		/// </summary>
		protected void InitVars()
		{
			_intNumberErr = 0; _strMessage = string.Empty;
			_resourceData = new ResourceManager(typeof(LanguageSource));
		}

		public async Task SendSmsMessage(string strNumberMobile, string strMessageText)
	  {
			// // Adding the support for TLS 1.2 protocol (we need this line in case of use HTTPS address of this provider
			InitVars(); HttpClient client = null;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls;
			
			try
			{
				if (string.IsNullOrEmpty(strNumberMobile) | strNumberMobile.Length == 0)
				{
					_intNumberErr = 3901;
					_strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strNumberPhoneRequired")}";
				}
				else if (string.IsNullOrEmpty(strMessageText) | strMessageText.Length == 0)
				{
					_intNumberErr = 3902;
					_strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strMessageTextRequired")}";
				}
				else
				{
					await Task.Run(() =>
					{
						if (smsProxy != null)
						{
							var handler = new HttpClientHandler { Proxy = smsProxy, UseProxy = true };
							client = new HttpClient(handler);
						}
						else
						{
							client = new HttpClient();
						}

						using (var content = new StringContent(smsParametersLink.Trim(), Encoding.UTF8, "application/x-www-form-urlencoded"))
						{
							using (var response = client.PostAsync(smsProviderLink, content))
							{
								response.Wait();
								response.Result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

								var result = response.Result.Content.ReadAsStringAsync();
								result.Wait();

								if (result.IsFaulted || result.Result != smsConfirmationSucessFull.Trim())
                {
									_intNumberErr = 3903;
									_strMessage = $"{_resourceData.GetString("strMessageErr")} {_resourceData.GetString("strFailedSMSSended")}";
								}
							}
						}

						Thread.Sleep(450);
					}).ConfigureAwait(false);
				}
			}
			catch (Exception oEx)
			{
				_intNumberErr = 3900;
				_strMessage = $"Ocurrió un error del tipo '{oEx.GetType()}' : {((oEx.InnerException == null) ? oEx.Message.Trim() : oEx.InnerException.Message.Trim()) }";
			}
			finally
			{
				if (_intNumberErr > 0) { throw new UtilitiesException(_strMessage); }
			}
	  }
  }
}
