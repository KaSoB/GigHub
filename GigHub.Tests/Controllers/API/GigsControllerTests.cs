using FluentAssertions;
using GigHub.Controllers.API;
using GigHub.Models;
using GigHub.Persistence;
using GigHub.Repositories;
using GigHub.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Http.Results;

namespace GigHub.Tests.Controllers.API {
    [TestClass]
    public class GigsControllerTests {
        private GigsController _controller;
        private Mock<IGigRepository> mockRepository;
        private readonly string userId = "1";
        private readonly string userDomain = "user1@domain.com";

        [TestInitialize]
        public void TestInitialize() {
            mockRepository = new Mock<IGigRepository>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.SetupGet(u => u.Gigs).Returns(mockRepository.Object);

            _controller = new GigsController(mockUoW.Object);
            _controller.MockCurrentUser(userId, userDomain);

        }

        [TestMethod]
        public void Cancel_NoGigWithGivenIdExists_ShouldReturnNotFound() {
            var result = _controller.Cancel(1);
            result.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod]
        public void Cancel_GigIsCanceled_ShouldReturnNotFound() {
            var gig = new Gig();
            gig.Cancel();

            mockRepository.Setup(r => r.GetGigWithAttendees(1)).Returns(gig);

            var result = _controller.Cancel(1);
            result.Should().BeOfType<NotFoundResult>();

        }
        [TestMethod]
        public void Cancel_UserCancelingAnotherUserGig_ShouldReturnUnauthorized() {
            var gig = new Gig { ArtistId = userId + "-" };
            gig.Cancel();

            mockRepository.Setup(r => r.GetGigWithAttendees(1)).Returns(gig);

            var result = _controller.Cancel(1);
            result.Should().BeOfType<UnauthorizedResult>();

        }
        [TestMethod]
        public void Cancel_ValidRequest_ShouldReturnOk() {
            var gig = new Gig { ArtistId = userId };
            gig.Cancel();

            mockRepository.Setup(r => r.GetGigWithAttendees(1)).Returns(gig);

            var result = _controller.Cancel(1);
            result.Should().BeOfType<OkResult>();

        }
    }
}
