require(['jquery'], function ($) {

  var countdown = (function ($) {
    var result = {},
      members = {};

    result.init = function (selector) {
      $(selector).each(function () {
        var $this = $(this);

        var time = $this.find('[data-type="countdown-init-data"] [data-type="FinalDate"]'),
          date = $(this).find('[data-type="FinalDate"]'),
          timeout = $(this).find('[data-type="TimeOutSeconds"]').attr("data-value") * 1 || 10;

        var cnt = {
          time: new Date(Date.UTC(date.attr("data-year"), date.attr("data-month") * 1 - 1, date.attr("data-day") * 1, date.attr("data-hour"), date.attr("data-minutes"))),
          element: $this,
          timeout: timeout
        };
        members.countdown(cnt);
      });
    };

    members.countdown = function (cnt) {
      var timeNow = new Date();

      if (timeNow >= cnt.time) {
        cnt.element.hide();

        return;
      }

      var timeDiff = Math.abs(timeNow - cnt.time) / 1000;
      var days = Math.floor(timeDiff / 86400);
      timeDiff -= days * 86400;

      cnt.daysContainer = cnt.daysContainer || cnt.element.find("[data-type='day-container']")
      cnt.daysContainer.text(days);

      var hours = Math.floor(timeDiff / 3600) % 24;
      timeDiff -= hours * 3600;
      cnt.hoursContainer = cnt.hoursContainer || cnt.element.find("[data-type='hour-container']");
      cnt.hoursContainer.text(hours);

      var minutes = Math.floor(timeDiff / 60) % 60;
      cnt.minutesContainer = cnt.minutesContainer || cnt.element.find("[data-type='minute-container']");
      cnt.minutesContainer.text(minutes);

      setTimeout(function () {
        members.countdown(cnt);
      }, cnt.timeout * 1000);
    };

    return result;
  })($);

  $(function () {
    countdown.init('[data-type="deltatre-countdown"]');
  });
});