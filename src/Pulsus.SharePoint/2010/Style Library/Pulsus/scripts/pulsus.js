var dataSource = new kendo.data.DataSource({
    transport: {
        contentType: "application/json",
        dataType: "json",
        read: {
            url: window.location,
            type: 'post',
            data: function () {
                return {
                    period: $('#pulsus-period').val(),
                    search: $('#pulsus-search').val()
                };
            }
        }
    },
    schema: {
        model: {
            fields: {
                EventId: { type: 'string' },
                Date: { type: 'date' },
                Level: { type: 'string' },
                Text: { type: 'string' },
                Tags: { type: 'string' }
            }
        },
        total: function (response) {
            return response.Total;
        },
        data: function(response) {
            return response.Data;
        }
    },
    serverPaging: true,
    pageSize: 100,
    serverFiltering: false,
    serverSorting: false
});

var refresh = function () {
    var grid = $('#pulsus-grid').data("kendoGrid");
    if (grid != null)
        grid.clearSelection();
    
    $('#pulsus-details').empty();
    dataSource.read();
};

var view = function (eventdId) {
    $.get(window.location, { eventId: eventId }, function(result) {
        $('#pulsus-details').html(result);
    });
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

    $('#pulsus-period').daterangepicker({
        dateFormat: 'mm/dd/yy',
        onChange: function () {
            refresh();
        }
    }).val(initialDateRange);

    $('#pulsus-search').on("keypress", function(e) {
        if (e.keyCode == 13)
            refresh();
    });

    refresh();
    
    $("#pulsus-grid").kendoGrid({
        dataSource: dataSource,
        autoBind: false,
        height: 400,
        filterable: false,
        sortable: false,
        pageable: true,
        selectable: true,
        columns: [
            { field: "Date", width: 80, format: "{0: MMM dd HH:mm}" },
            { field: "Text", width: 300 },
            { field: "Level", width: 70 },
            { field: "Tags", width: 100 }
        ],
        change: function(e) {
            var selectedRow = this.select();
            var selectedData = this.dataItem(selectedRow);
            view(selectedData.EventId);
        }
    });
});
