var refresh = function() {
    var data = {        
        period: $('#period').val()
    };
    $.post(window.location, data, function(result) {
        //$('#pulsus-events').empty();
        for (var i in result) {
            //$('#pulsus-events').append($('<li></li>').text(result[i].Text));
        }
    });
};

var view = function (e) {
};

var formatDate = function (date) {
    if (!date.getDate()) {
        return '';
    }
    var dateFormat = 'mm/dd/yy';
    return $.datepicker.formatDate(dateFormat, date);
};

$().ready(function () {

    var initialDateRange = formatDate(Date.parse('today').moveToFirstDayOfMonth()) + ' - ' + formatDate(Date.parse('today'));

    $('#period').daterangepicker({
        dateFormat: 'mm/dd/yy',
        onChange: function () {
            refresh();
        }
    }).val(initialDateRange);

    refresh();
});
