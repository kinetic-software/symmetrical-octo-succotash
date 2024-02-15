using Kx.Availability.Data.Connection;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Kx.Availability.Data.Tests
{
    //DC: Your probably thinking "But we have our Specflow (BDD) tests - yes, and they're great! BUT - really, you want "unit test" coverage
    //    at all layers - heres a micro example of doing that, to target specific bits of logic without the heavy lifting of spinning up
    //    Mongo DB's (or any other type of mocked infrastructure)
    public class TenantTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<HttpContext> _httpContextMock;
        private Mock<HttpRequest> _httpRequestMock;

        public TenantTests()
        {
            SetupMocks();
        }

        private void SetupMocks()
        {
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _httpContextMock = new Mock<HttpContext>();
            _httpRequestMock = new Mock<HttpRequest>();

            SetupMockBehaviour();
        }

        private void SetupMockBehaviour()
        {
            _httpContextMock.Setup(hc => hc.Request).Returns(_httpRequestMock.Object);
            _httpContextAccessor.Setup(hca => hca.HttpContext).Returns(_httpContextMock.Object);
        }

        //DC: I have just written a basic test to cover the slightly extended validation of the Request.Path (Found in the 
        //    Tenant class) - it was previously quite "loose" before in that it only checked for a null value; not empty.
        //    This unit test targets both specifics and drills down to the outcome; it drove the change.
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TenantConstructed_WhenRequestPathisNullEmptyOrWhitespace_ThrowsBadHttpRequestException(string requestPath)
        {
            _httpRequestMock.Setup(hr => hr.Path).Returns(requestPath);

            var thrownException = Assert.Throws<BadHttpRequestException>(() =>
            {
                var tenant = new Tenant(_httpContextAccessor.Object);
            });

            Assert.Equal("Request path cannot be null or empty", thrownException.Message);
        }

        //DC: I could go on to cover the extistence of the "TenantId" in the request path but given this is just an example, I wont bother writing it.
    }
}