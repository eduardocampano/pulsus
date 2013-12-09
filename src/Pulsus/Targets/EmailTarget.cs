using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Pulsus.Internal;

namespace Pulsus.Targets
{
    public class EmailTarget : Target
    {
        private static object _lock = new object();

        private DateTime? _throttlingStarted;
        private DateTime _currentWindow = DateTime.MinValue;
        private int _currentCount = 0;

        /// <summary>
        /// Gets or sets the from address for the sent emails
        /// </summary>
        public virtual string From { get; set; }

        /// <summary>
        /// Gets or sets the destination address for the emails. Use comma (,) to specify more than one.
        /// </summary>
        public virtual string To { get; set; }

        /// <summary>
        /// Gets or sets the subject for the emails. Wildcard can be use to build it like {Level}, {Text}, etc. 
        /// </summary>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Gets or sets the title shown at the begining of the email.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the URI used to link to an external event visualization tool. For example: http://pulsus.mycompany.com/Event/{EventId}
        /// </summary>
        public virtual string TitleUri { get; set; }

        /// <summary>
        /// Gets or sets the SMTP server IP or hostname
        /// </summary>
        public virtual string SmtpServer { get; set; }

        /// <summary>
        /// Gets or sets the SMTP server port
        /// </summary>
        public virtual int SmtpPort { get; set; }

        /// <summary>
        /// Gets or sets the SMTP server username
        /// </summary>
        public virtual string SmtpUsername { get; set; }

        /// <summary>
        /// Gets or sets the SMTP server password
        /// </summary>
        public virtual string SmtpPassword { get; set; }

        /// <summary>
        /// Gets or sets whether SMTP server a SSL connection 
        /// </summary>
        public virtual bool SmtpEnableSsl { get; set; }

        /// <summary>
        /// Gets or sets the URL used for the IP Address rendering for linking to an IP address information service. For example: http://iplocationinformationservice.com/?ip={0}.
        /// </summary>
        public virtual string IpAddressInfoUri { get; set; }

        /// <summary>
        /// Gets or sets the URL used for the User Agent rendering for linking to an User Agent information service. For example: http://usergentinfo.com/?ua={0}
        /// </summary>
        public virtual string UserAgentInfoUri { get; set; }

        /// <summary>
        /// Get or sets 
        /// </summary>
        [DefaultValue(1)]
        public virtual int ThrottlingWindow { get; set; }

        /// <summary>
        /// Gets or sets the threshold of emails sent within the throttling window
        /// </summary>
        [DefaultValue(60)]
        public virtual int ThrottlingThreshold { get; set; }

        /// <summary>
        /// Gets or sets the throttling duration in minutes after the threshold has been reached
        /// </summary>
        [DefaultValue(10)]
        public virtual int ThrottlingDuration { get; set; }

        public override void Push(LoggingEvent[] loggingEvents)
        {
            if (loggingEvents == null)
                throw new ArgumentNullException("loggingEvents");

            foreach (var loggingEvent in loggingEvents)
            {
                if (!ShouldSendEmail())
                {
                    PulsusDebugger.Write(this, "Event '{0}' was not pushed because email throttling is activated", loggingEvent.Text);
                    continue;
                }

                var mailMessage = PrepareEmail(loggingEvent);
                Send(mailMessage);
            }
        }

        protected virtual bool ShouldSendEmail()
        {
            var window = RoundUp(DateTime.UtcNow, TimeSpan.FromMinutes(ThrottlingWindow));

            lock (_lock)
            {
                // if the throttling is activated we ignore everything
                if (_throttlingStarted.HasValue && DateTime.UtcNow.Subtract(_throttlingStarted.Value).TotalMinutes < ThrottlingDuration)
                    return false;

                // if the current window changes we start counting again
                if (window != _currentWindow)
                {
                    _currentWindow = window;
                    _currentCount = 1;
                    _throttlingStarted = null;
                    return true;
                }

                _currentCount++;

                // count is below the threshold
                if (_currentCount <= ThrottlingThreshold)
                    return true;

                // start throttling
                _throttlingStarted = DateTime.UtcNow;
            }

            PulsusDebugger.Write(this, "Throttling started at {0}", _throttlingStarted);
            SendThrottlingEmail();
            return false;
        }

        protected virtual void SendThrottlingEmail()
        {
            var loggingEvent = new LoggingEvent()
            {
                Level = LoggingEventLevel.Error,
                Text = string.Format("Email throttling has started at {0} UTC for {1} minutes", DateTime.UtcNow, ThrottlingDuration),
                Tags = new[] { "email-throttling" }
            };

            var mailMessage = PrepareEmail(loggingEvent);
            Send(mailMessage);
            PulsusDebugger.Write(this, "Throttling email sent");
        }

        protected virtual MailMessage PrepareEmail(LoggingEvent loggingEvent)
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

        protected virtual void Send(MailMessage mailMessage)
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

        private string PrepareEmailBody(EmailTemplateModel model)
        {
            var emailTemplate = new EmailTemplate(model);
            return emailTemplate.TransformText();
        }

        public static DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks);
        }
    }
}
