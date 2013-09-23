using System;
using System.Net;
using System.Net.Mail;
using Pulsus.Internal;

namespace Pulsus.Targets
{
	public class EmailTarget : Target
	{
		public string From { get; set; }
		public string To { get; set; }
		public string Subject { get; set; }
		public string Title { get; set; }
	    public string TitleUri { get; set; }
	    public string SmtpServer { get; set; }
		public int SmtpPort { get; set; }
		public string SmtpUsername { get; set; }
		public string SmtpPassword { get; set; }
		public bool SmtpEnableSsl { get; set; }

	    public string IpAddressInfoUri { get; set; }
	    public string UserAgentInfoUri { get; set; }

	    public override void Push(LoggingEvent[] loggingEvents)
		{
			if (loggingEvents == null)
				throw new ArgumentNullException("loggingEvents");

			foreach (var loggingEvent in loggingEvents)
			{
				var mailMessage = PrepareEmail(loggingEvent);
				Send(mailMessage);
			}
		}

		protected MailMessage PrepareEmail(LoggingEvent loggingEvent)
		{
		    var model = new EmailTemplateModel(loggingEvent)
		    {
                Title = Title,
                TitleUri = TitleUri,
                IpAddressInfoUri = IpAddressInfoUri,
                UserAgentInfoUri = UserAgentInfoUri
		    };

			var mailMessage = new MailMessage(From, To);

			mailMessage.Subject = model.GetFormattedSubject();
			mailMessage.IsBodyHtml = true;
			mailMessage.Body = PrepareEmailBody(model);
			
			return mailMessage;
		}

		private string PrepareEmailBody(EmailTemplateModel model)
		{
			var emailTemplate = new EmailTemplate(model);
			return emailTemplate.TransformText();
		}

		public void Send(MailMessage mailMessage)
		{
			var client = new SmtpClient();

			var host = SmtpServer ?? string.Empty;

			if (host.Length > 0)
			{
				client.Host = host;
				client.DeliveryMethod = SmtpDeliveryMethod.Network;
			}

			var port = SmtpPort;
			if (port > 0)
				client.Port = port;

			client.EnableSsl = SmtpEnableSsl;

			if (string.IsNullOrEmpty(SmtpUsername))
				client.UseDefaultCredentials = true;
			else
			{
				var userName = SmtpUsername;
				var password = SmtpPassword ?? string.Empty;

				if (userName.Length > 0 && password.Length > 0)
					client.Credentials = new NetworkCredential(userName, password);
			}

			client.Send(mailMessage);
		}
	}
}
