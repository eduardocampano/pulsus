using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Pulsus.Configuration;
using SharpTestsEx;

namespace Pulsus.Tests
{
    [TestFixture]
    public class PulsusConfigurationTests
    {
        [Test]
        public void TargetWithIgnoresShouldBeProperlyParsed()
        {
            var pulsusDocument = XDocument.Parse(@"<pulsus>
                                                    <targets>
                                                        <target name=""email"" type=""EmailTarget"">
                                                            <ignores>
                                                                <ignore textContains=""filter-text"" />
                                                            </ignores>
                                                        </target> 
                                                    </targets>
                                                </pulsus>");

            var pulsusElement = pulsusDocument.Root;

            var configuration = new PulsusXmlConfiguration(pulsusElement);

            var target = configuration.Targets.First().Value;
            target.Ignores.Should().Have.Count.EqualTo(1);
            var ignore = target.Ignores.First();
            ignore.Should().Not.Be.Null();
            ignore.TextContains.Should().Be("filter-text");
        }
    }
}
