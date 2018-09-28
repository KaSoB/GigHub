using GigHub.Persistence;
using Microsoft.AspNet.Identity;
using System.Web.Http;

namespace GigHub.Controllers.API {
    [Authorize]
    public class GigsController : ApiController {
        private readonly IUnitOfWork unitOfWork;
        public GigsController(IUnitOfWork @object) {
            unitOfWork = @object;
        }

        [HttpDelete]
        public IHttpActionResult Cancel(int id) {
            var userId = User.Identity.GetUserId();
            var gig = unitOfWork.Gigs.GetGigWithAttendees(id);
            if (gig == null || gig.IsCanceled) {
                return NotFound();
            }

            if (gig.ArtistId != userId) {
                return Unauthorized();
            }
            gig.Cancel();

            unitOfWork.Complete();

            return Ok();
        }
    }
}
