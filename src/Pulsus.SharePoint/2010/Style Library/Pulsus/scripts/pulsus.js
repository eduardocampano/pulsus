var dataSource = new kendo.data.DataSource({
    transport: {
        contentType: "application/json",
        dataType: "json",
        read: function(options) {
            $.ajax({
                url: window.location,
                type: 'post',
                dataType: "json",
                data: {
                    period: $('#pulsus-period').val(),
                    search: $('#pulsus-search').val(),
                    minLevel: $('#pulsus-minLevel').val(),
                    maxLevel: $('#pulsus-maxLevel').val(),
                    tags: $('#pulsus-tags').val(),
                    skip: options.data.skip,
                    take: options.data.take
                },
                success: function(result) {
                    options.success(result);
                }
            });
        }
    },
    schema: {
        model: {
            fields: {
                EventId: { type: 'string' },
                Date: { type: 'date' },
                LevelString: { type: 'string' },
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

var previousPeriod = '';
var previousSearch = '';

var refresh = function () {
    if (previousPeriod == $('#pulsus-period').val() && previousSearch == $('#pulsus-search').val())
        return;

    var grid = $('#pulsus-grid').data("kendoGrid");
    if (grid != null)
        grid.clearSelection();
    
    $('#pulsus-details').empty();
    dataSource.read();
};

var view = function (eventId) {
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

var resize = function() {
    var $container = $('.pulsus-container');
    var $grid = $('#pulsus-grid');
    var $details = $('#pulsus-details');
    var newHeight = $(window).height() - $container.offset().top - 10;
    $container.height(newHeight);
    $details.height(newHeight);
    var gridHeight = newHeight - $('.pulsus-container .parameters').height() - 20;
    $grid.height(gridHeight);
    var heightDiff = gridHeight - $grid.height();
    var $gridContent = $grid.find(".k-grid-content");
    $gridContent.height($gridContent.height() + heightDiff);

    var $left = $container.find('.left');
    var $right = $container.find('.right');
    var rightWidth = $(window).width() - $left.offset().left - $left.width() - 44;
    $right.width(rightWidth);
};

$().ready(function () {

    resize();
    $(window).resize(resize);
    
    var initialDateRange = formatDate(Date.parse('today').addDays(-30)) + ' - ' + formatDate(Date.parse('today'));

    $('#pulsus-period').daterangepicker({
        dateFormat: 'mm/dd/yy',
        onChange: function () {
            refresh();
        }
    }).val(initialDateRange);

    $('#pulsus-search').on("keypress focusout", refresh);
    refresh();

    $("#pulsus-grid").kendoGrid({
        dataSource: dataSource,
        autoBind: false,
        filterable: false,
        sortable: false,
        pageable: true,
        selectable: true,
        columns: [
            { field: "Date", width: 80, format: "{0: MMM dd HH:mm}" },
            { field: "Text", width: 300 },
            { field: "LevelString", width: 70, title: "Level", template: kendo.template($("#level-template").html()) },
            { field: "Tags", width: 100, template: kendo.template($("#tags-template").html()) }
        ],

        change: function(e) {
            var selectedRow = this.select();
            if (selectedRow == null)
                return;
            var selectedData = this.dataItem(selectedRow);
            if (selectedData == null)
                return;

            view(selectedData.EventId);
        }
    });

    $("#pulsus-grid").data("kendoGrid").bind('dataBound', function (e) {
        this.select("tr:eq(1)");
    });

    $('#filters').kendoButton({ spriteCssClass: 'k-icon k-i-funnel' }).click(function (e) {
        $('#pulsus-filters-minLevel').val($('#pulsus-minLevel').val());
        $('#pulsus-filters-maxLevel').val($('#pulsus-maxLevel').val());
        $('#pulsus-filters-tags').val($('#pulsus-tags').val());
        $('#filtersModal').data('kendoWindow').center().open();
        e.preventDefault();
    });

    $('#filtersModal').kendoWindow({
        width: "600px",
        title: "Filters",
        modal: true,
        draggable: false,
        resizable: false,
        visible: false,
        actions: [
            "Close"
        ],
        close: refresh
    });

    $('#filtersOK').kendoButton().click(function () {
        $('#filtersModal').data('kendoWindow').close();
        $('#pulsus-minLevel').val($('#pulsus-filters-minLevel').val());
        $('#pulsus-maxLevel').val($('#pulsus-filters-maxLevel').val());
        $('#pulsus-tags').val($('#pulsus-filters-tags').val());
        refresh();
    });
    $('#filtersCancel').kendoButton().click(function () {
        $('#filtersModal').data('kendoWindow').close();
    });
});
