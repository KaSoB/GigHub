var GigsController = function (attendanceService) {
    var button;
    var init = function (container) {
        $(container).on("click", ".js-toggle-attendance", toggleAttendance);
    };

    var toggleAttendance = function (e) {
        button = $(e.target);
        var gigId = button.attr("data-gig-id");
        if (button.hasClass("btn-default")) {
            attendanceService.createAttendance(gigId, done, fail);
        } else {
            attendanceService.deleteAttendance(gigId, done, fail);
        }
    };



    var fail = function () {
        alert("Something failed");
    };

    var done = function () {
        var text = (button.text() == "Going") ? "Going?" : "Going";

        button.toggleClass("btn-info").toggleClass("btn-default").text(text);
    };



    var follow = function () {
        $(".js-toggle-follow").click(function (e) {
            var button = $(e.target);
            $.post("/api/following", { followeeId: button.attr("data-user-id") })
                .done(function () {
                    button.text("Following");
                })
                .fail(function () {
                    alert("Something failed");
                });
        });
    };

    return {
        init: init,
        follow: follow
    };

}(AttendanceService);
