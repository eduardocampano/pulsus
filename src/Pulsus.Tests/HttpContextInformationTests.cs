using System;
using System.Collections.Specialized;
using Moq;
using NUnit.Framework;
using SharpTestsEx;
using System.Collections.Generic;
using System.Web;

namespace Pulsus.Tests
{
    [TestFixture]
    public class HttpContextInformationTests
    {
        [Test]
        public void CorrelationIdShouldBeRetrievedFromRequest()
        {
            var httpContextMock = GetHttpContextMock();
            var itemsDictionary = new Dictionary<string, object>() 
            {
                { "X-Pulsus-CorrelationId", "abcd" }
            };

            httpContextMock.Setup(x => x.Items).Returns(itemsDictionary);

            var httpContextInfo = HttpContextInformation.Create(httpContextMock.Object);

            httpContextInfo.CorrelationId.Should().Be("abcd");

            httpContextMock.VerifyAll();
        }

        private Mock<HttpContextBase> GetHttpContextMock()
        {
            var httpContextMock = new Mock<HttpContextBase>();
            var httpRequestMock = new Mock<HttpRequestBase>();
            var httpServerMock = new Mock<HttpServerUtilityBase>();
            var httpFileCollectionMock = new Mock<HttpFileCollectionBase>();

            httpRequestMock.Setup(x => x.Url).Returns(new Uri("/test", UriKind.Relative));
            httpRequestMock.Setup(x => x.Headers).Returns(new NameValueCollection());
            httpRequestMock.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            httpRequestMock.Setup(x => x.ServerVariables).Returns(new NameValueCollection());
            httpRequestMock.Setup(x => x.Files).Returns(httpFileCollectionMock.Object);

            httpContextMock.Setup(x => x.Server).Returns(httpServerMock.Object);
            httpRequestMock.Setup(x => x.UserAgent).Returns("Test user agent");

            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);
            return httpContextMock;
        }
    }
}
