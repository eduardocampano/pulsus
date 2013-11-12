var dataSource = new kendo.data.DataSource({
    transport: {
        contentType: "application/json",
        dataType: "json",
        read: {
            url: window.location,
            type: 'post',
            data: function () {
                return {
                    period: $('#pulsus-period').val()
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
        }
    },
    serverPaging: false,
    serverFiltering: false,
    serverSorting: false
});

var refresh = function () {
    dataSource.read();
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

    $('#pulsus-period').daterangepicker({
        dateFormat: 'mm/dd/yy',
        onChange: function () {
            refresh();
        }
    }).val(initialDateRange);
    
    refresh();
    
    $("#pulsus-grid").kendoGrid({
        dataSource: dataSource,
        autoBind: false,
        height: 400,
        filterable: false,
        sortable: false,
        pageable: false,
        columns: [
            { field: "Date", width: 100, format: "{0: MMM dd HH:mm}" },
            { field: "Text", width: 200 },
            { field: "Level", width: 100 },
            { field: "Tags", width: 100 }
        ]
    });
});
