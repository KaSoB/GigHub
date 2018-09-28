using FluentAssertions;
using GigHub.Models;
using GigHub.Repositories;
using GigHub.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.Entity;

namespace GigHub.Tests.Persistence.Repositories {

    [TestClass]
    public class GigRepositoryTests {
        private GigRepository _repository;
        private Mock<DbSet<Gig>> mockGigs;
        [TestInitialize]
        public void TestInitialize() {
            mockGigs = new Mock<DbSet<Gig>>();

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.SetupGet(c => c.Gigs).Returns(mockGigs.Object);

            _repository = new GigRepository(mockContext.Object);

        }

        [TestMethod]
        public void GetUpcomingGigsByArtist_GigIsInThePast_ShouldNotBeReturned() {
            var gig = new Gig() { DateTime = DateTime.Now.AddDays(-1), ArtistId = "1" };
            mockGigs.SetSource(new[] { gig });
            var gigs = _repository.GetUpcomingGigsByArtist("1");

            gigs.Should().BeEmpty();
        }

        [TestMethod]
        public void GetUpcomingGigsByArtist_GigIsCanceled_ShouldNotBeReturned() {
            var gig = new Gig() { DateTime = DateTime.Now.AddDays(1), ArtistId = "1" };
            gig.Cancel();
            mockGigs.SetSource(new[] { gig });
            var gigs = _repository.GetUpcomingGigsByArtist("1");

            gigs.Should().BeEmpty();
        }

        [TestMethod]
        public void GetUpcomingGigsByArtist_GigIsForADifferentArtist_ShouldNotBeReturned() {
            var gig = new Gig() { DateTime = DateTime.Now.AddDays(1), ArtistId = "1" };

            mockGigs.SetSource(new[] { gig });

            var gigs = _repository.GetUpcomingGigsByArtist(gig.ArtistId + "-");

            gigs.Should().BeEmpty();
        }

        [TestMethod]
        public void GetUpcomingGigsByArtist_GigIsForADifferentArtistAndIsInTheFuture_ShouldNotBeReturned() {
            var gig = new Gig() { DateTime = DateTime.Now.AddDays(1), ArtistId = "1" };

            mockGigs.SetSource(new[] { gig });

            var gigs = _repository.GetUpcomingGigsByArtist(gig.ArtistId);

            gigs.Should().Contain(gig);
        }
    }
}
