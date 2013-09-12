using NUnit.Framework;

namespace Pulsus.Tests
{
    [TestFixture]
    public class PulsusConfigurationTests
    {
        [Test]
        public void TargetWithIgnoresShouldBeProperlyParsed()
        {
            var xml = @"<pulsus>
                            <targets>
                                <target name=""email"" type=""EmailTarget"">
                                    <ignores>
                                        <ignore textContains=""filter-text"" />
                                    </ignores>
                                </target> 
                            </targets>
                        </pulsus>";
        }
    }
}
