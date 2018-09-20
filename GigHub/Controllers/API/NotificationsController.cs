using AutoMapper;
using GigHub.Dtos;
using GigHub.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace GigHub.Controllers.API {
    //   [Authorize]
    public class NotificationsController : ApiController {
        private readonly ApplicationDbContext _context;
        public NotificationsController() {
            _context = new ApplicationDbContext();
        }

        public IEnumerable<NotificationDto> GetNewNotifications() {
            var userId = User.Identity.GetUserId();
            var notifications = _context.UserNotifications
                .Where(u => u.UserId == userId && !u.IsRead)
                .Select(u => u.Notification)
                .Include(u => u.Gig.Artist)
                .ToList();


            return notifications
                .Select(Mapper.Map<Notification, NotificationDto>);
        }
        [HttpPost]
        public IHttpActionResult MarkAsRead() {
            var userId = User.Identity.GetUserId();
            var notifications = _context.UserNotifications.Where(n => n.UserId == userId && !n.IsRead).ToList();

            notifications.ForEach(n => n.Read());

            _context.SaveChanges();
            return Ok();
        }
    }
}
