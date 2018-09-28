using GigHub.Models;
using GigHub.Persistence;
using GigHub.Repositories;
using GigHub.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace GigHub.Controllers {
    public class GigsController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly AttendanceRepository _attendanceRepository;
        private readonly FollowRepository _followRepository;
        private readonly GenreRepository _genreRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GigsController(IUnitOfWork unitOfWork) {
            _context = new ApplicationDbContext();
            _attendanceRepository = new AttendanceRepository(_context);
            _followRepository = new FollowRepository(_context);
            _genreRepository = new GenreRepository(_context);
            _unitOfWork = unitOfWork;
        }
        // GET: Gigs
        [Authorize]
        public ActionResult Create() {
            var viewModel = new GigFormViewModel() {
                Genres = _genreRepository.GetGenres(),
                Heading = "Add a Gig"
            };

            return View("GigForm", viewModel);
        }

        [Authorize]
        public ActionResult Mine() {
            var userId = User.Identity.GetUserId();
            var gigs = _unitOfWork.Gigs.GetUpcomingGigsByArtist(userId);

            return View(gigs);
        }

        [HttpPost]
        public ActionResult Search(GigsViewModel viewModel) {
            return RedirectToAction("Index", "Home", new { query = viewModel.SearchTerm });
        }

        public ActionResult Details(int id) {
            var gig = _context.Gigs
                .Include(g => g.Artist)
                .Include(g => g.Genre)
                .SingleOrDefault(g => g.Id == id);
            if (gig == null) {
                return HttpNotFound();
            }
            var viewModel = new GigDetailsViewModel { Gig = gig };

            if (User.Identity.IsAuthenticated) {
                var userId = User.Identity.GetUserId();

                viewModel.IsAttending = _attendanceRepository.GetAttendance(gig.Id, userId) != null;
                viewModel.IsFollowing = _followRepository.GetFollowing(userId, gig.ArtistId) != null;
            }

            return View("Details", viewModel);
        }

        [Authorize]
        public ActionResult Attending() {
            var userId = User.Identity.GetUserId();

            var viewModel = new GigsViewModel() {
                UpcomingGigs = _unitOfWork.Gigs.GetGigsUserAttending(userId),
                ShowActions = false,
                Heading = "Gigs I'm Attending",
                Attendances = _attendanceRepository.GetFutureAttendances(userId).ToLookup(a => a.GigId)
            };

            return View("Gigs", viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GigFormViewModel viewModel) {
            if (!ModelState.IsValid) {
                viewModel.Genres = _context.Genres.ToList(); // TODO?
                return View("GigForm", viewModel);
            }

            var gig = new Gig {
                ArtistId = User.Identity.GetUserId(),
                DateTime = viewModel.GetDateTime(),
                GenreId = viewModel.Genre,
                Venue = viewModel.Venue
            };

            _unitOfWork.Gigs.Add(gig);
            _unitOfWork.Complete();

            return RedirectToAction("Mine", "Gigs");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(GigFormViewModel viewModel) {
            if (!ModelState.IsValid) {
                viewModel.Genres = _context.Genres.ToList(); // TODO?
                return View("GigForm", viewModel);
            }
            var gig = _unitOfWork.Gigs.GetGigWithAttendees(viewModel.Id);

            if (gig == null) {
                return HttpNotFound();
            }
            if (gig.ArtistId != User.Identity.GetUserId()) {
                return new HttpUnauthorizedResult();
            }
            gig.Modify(viewModel.GetDateTime(), viewModel.Venue, viewModel.Genre);

            _unitOfWork.Complete();

            return RedirectToAction("Mine", "Gigs");
        }

        [Authorize]
        public ActionResult Edit(int id) {
            var gig = _unitOfWork.Gigs.GetGig(id);

            if (gig == null) {
                return HttpNotFound();
            }

            if (gig.ArtistId != User.Identity.GetUserId()) {
                return new HttpUnauthorizedResult();
            }

            var viewModel = new GigFormViewModel {
                Heading = "Edit a Gig",
                Id = gig.Id,
                Genres = _context.Genres.ToList(),
                Date = gig.DateTime.ToString("d MMM yyyy"),
                Time = gig.DateTime.ToString("HH:mm"),
                Genre = gig.GenreId,
                Venue = gig.Venue
            };


            return View("GigForm", viewModel);
        }


    }
}