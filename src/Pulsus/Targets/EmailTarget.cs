using System;
using System.Net;
using System.Net.Mail;
using Pulsus.Configuration;
using Pulsus.Internal;

namespace Pulsus.Targets
{
	public class EmailTarget : ITarget
	{
		private readonly IPulsusSettings _settings;

		public EmailTarget(IPulsusSettings settings)
		{
			_settings = settings;
		}

		public bool Enabled
		{
			get
			{
				return _settings.Email.Enabled;
			}
		}

		public void Push(LoggingEvent[] loggingEvents)
		{
			if (loggingEvents == null)
				throw new ArgumentNullException("loggingEvents");

			if (!Enabled)
				return;

			foreach (var loggingEvent in loggingEvents)
			{
				var mailMessage = PrepareEmail(loggingEvent);
				Send(mailMessage);
			}
		}

		protected MailMessage PrepareEmail(LoggingEvent loggingEvent)
		{
			var model = new EmailTemplateModel(loggingEvent, _settings);

			var mailMessage = new MailMessage(_settings.Email.From, _settings.Email.To);
			mailMessage.Subject = model.Subject;
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

			var host = _settings.Email.SmtpServer ?? string.Empty;

			if (host.Length > 0)
			{
				client.Host = host;
				client.DeliveryMethod = SmtpDeliveryMethod.Network;
			}

			var port = _settings.Email.SmtpPort;
			if (port > 0)
				client.Port = port;

			client.EnableSsl = _settings.Email.SmtpEnableSsl;

			if (string.IsNullOrEmpty(_settings.Email.SmtpUsername))
				client.UseDefaultCredentials = true;
			else
			{
				var userName = _settings.Email.SmtpUsername;
				var password = _settings.Email.SmtpPassword ?? string.Empty;

				if (userName.Length > 0 && password.Length > 0)
					client.Credentials = new NetworkCredential(userName, password);
			}

			client.Send(mailMessage);
		}

		public override string ToString()
		{
			return string.Format("[Email] From: {0}, To: {1}", _settings.Email.From, _settings.Email.To);
		}
	}
}
