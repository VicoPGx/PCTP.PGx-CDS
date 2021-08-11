// DataTables support localization, refer to  http://www.datatables.net/plug-ins/i18n

var en = {
    "sEmptyTable": "No data available in table",
    "sInfo": "Showing _START_ to _END_ of _TOTAL_ entries",
    "sInfoEmpty": "Showing 0 to 0 of 0 entries",
    "sInfoFiltered": "(filtered from _MAX_ total entries)",
    "sInfoPostFix": "",
    "sInfoThousands": ",",
    "sLengthMenu": "Show _MENU_ entries.",
    "sLoadingRecords": "Loading...",
    "sProcessing": "Processing...",
    "sSearch": "Search:",
    "sZeroRecords": "No matching records found",
    "oPaginate": {
        "sFirst": "First", "sLast": "Last",
        "sNext": "Next", "sPrevious": "Previous"
    },
    "oAria": {
        "sSortAscending": ": activate to sort column ascending",
        "sSortDescending": ": activate to sort column descending"
    }
};

var cn = {
    "sProcessing": "处理中...",
    "sLengthMenu": "显示 _MENU_ 项结果",
    "sZeroRecords": "没有匹配结果",
    "sInfo": "显示第 _START_ 至 _END_ 项结果，共 _TOTAL_ 项",
    "sInfoEmpty": "显示第 0 至 0 项结果，共 0 项",
    "sInfoFiltered": "(由 _MAX_ 项结果过滤)",
    "sInfoPostFix": "",
    "sSearch": "搜索(支持汉字或拼音):",
    "sUrl": "",
    "oPaginate": {
        "sFirst": "首页",
        "sPrevious": "上页",
        "sNext": "下页",
        "sLast": "末页"
    }
};

var cnt = {
    "sProcessing": "處理中...",
    "sLengthMenu": "顯示 _MENU_ 項結果",
    "sZeroRecords": "沒有匹配結果",
    "sInfo": "顯示第 _START_ 至 _END_ 項結果，共 _TOTAL_ 項",
    "sInfoEmpty": "顯示第 0 至 0 項結果，共 0 項",
    "sInfoFiltered": "(從 _MAX_ 項結果過濾)",
    "sInfoPostFix": "",
    "sSearch": "搜索:",
    "sUrl": "",
    "oPaginate": {
        "sFirst": "首頁",
        "sPrevious": "上頁",
        "sNext": "下頁",
        "sLast": "尾頁"
    }
};

var jp = {
    "sProcessing": "処理中...",
    "sLengthMenu": "_MENU_ 件表示",
    "sZeroRecords": "データはありません。",
    "sInfo": " _TOTAL_ 件中 _START_ から _END_ まで表示",
    "sInfoEmpty": " 0 件中 0 から 0 まで表示",
    "sInfoFiltered": "（全 _MAX_ 件より抽出）",
    "sInfoPostFix": "",
    "sSearch": "検索:",
    "sUrl": "",
    "oPaginate": {
        "sFirst": "先頭",
        "sPrevious": "前",
        "sNext": "次",
        "sLast": "最終"
    }
};

var dk = {
    "sProcessing": "Henter...",
    "sLengthMenu": "Vis _MENU_ linjer",
    "sZeroRecords": "Ingen linjer matcher s&oslash;gningen",
    "sInfo": "Viser _START_ til _END_ af _TOTAL_ linjer",
    "sInfoEmpty": "Viser 0 til 0 af 0 linjer",
    "sInfoFiltered": "(filtreret fra _MAX_ linjer)",
    "sInfoPostFix": "",
    "sSearch": "S&oslash;g:",
    "sUrl": "",
    "oPaginate": {
        "sFirst": "F&oslash;rste",
        "sPrevious": "Forrige",
        "sNext": "N&aelig;ste",
        "sLast": "Sidste"
    }
};
var br={
    "sProcessing": "processamento...",
    "sLengthMenu": "Mostrar _MENU_ resultados",
    "sZeroRecords": "Nenhum resultado de acordo",
    "sInfo": "Mostrando _START_ para _END_ resultados，Total de itens _TOTAL_",
    "sInfoEmpty": "Mostrando 0-0 resultados, um total de 0",
    "sInfoFiltered": "(Filtrado por resultados _MAX_)",
    "sInfoPostFix": "",
    "sSearch": "Busca (suporte a caracteres chineses ou pinyin):",
    "sUrl": "",
    "oPaginate": {
        "sFirst": "casa",
        "sPrevious": "anterior",
        "sNext": "próxima página",
        "sLast": "última página"
    }
}
// DataTable is created only once, and cannot dynamically set language without recreating it.
function getLanguage() {
    var lan = en; // en;
    $.ajax({
        url: '/Home/QueryCurrentCultureString',
        type: 'GET',
        dataType: 'json',
        async:false
    }).done(function (json) {
        debugger;
        // 1 = zh-CN, 2 = en-US
        if (json == "1")
            lan = cn;
        else if (json == "2")
            lan = en;
        else if (json == "3")
            lan = dk;
        else if (json = "4")
            lan = br;
    });
    return lan;
}

function createDataTable(id, url, columns) {
    if (columns)
        return $(id).dataTable({
            // "sPaginationType": "full_numbers",
            "oLanguage": getLanguage(),
            "bJQueryUI": true, /* use JQueryUI style */
            "bServerSide": true,
            "sAjaxSource": url,
            "bProcessing": false,
            "bAutoWidth": false,
            "bSort": false, /* disable sorting */
            "bRetrieve": false,
            // "bDestroy": true,
            "aoColumns": columns
        });
    else {
        return $(id).dataTable({
            "oLanguage": getLanguage(),
            "bJQueryUI": true,
            "bServerSide": true,
            "sAjaxSource": url,
            "bProcessing": false,
            "bAutoWidth": false,
            "bSort": false,
            // "bDestroy": true
            "bRetrieve": true
        });
    }
}


/**
* Re-read an Ajax source and update data table
*
*/
$.fn.dataTableExt.oApi.fnReloadAjax = function (oSettings, sNewSource) {
    if (typeof sNewSource != 'undefined') {
        oSettings.sAjaxSource = sNewSource;
    }
    this.fnClearTable(this);
    this.oApi._fnProcessingDisplay(oSettings, true);
    var that = this;
    $.getJSON(oSettings.sAjaxSource, null, function (json) {
        /* Got the data - add it to the table */
        for (var i = 0; i < json.aaData.length; i++) {
            that.oApi._fnAddData(oSettings, json.aaData[i]);
        }
        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
        that.fnDraw(that);
        that.oApi._fnProcessingDisplay(oSettings, false);
    });
};

var ContextItemDictTableColumnsStyle1 = [
    {
        "sName": "Name",
        "bSortable": false,
        "fnRender": function (oObj) {
            var ln = '<span style="color:blue;cursor:pointer;" onclick="editContextItem(\'' + oObj.aData["9"] + '\')">' + oObj.aData["0"] + '</span>';
            return ln;
        }
    }
];

var ContextItemDictTableColumnsStyle2 = [
    {
        "sName": "Name",
        "bSortable": false,
        "sWidth": "90%"
    },
    {
        "bSortable": false,
        "sWidth": "10%",
        "fnRender": function (oObj) {
            return '<span style="color:blue; cursor:pointer;" onclick="createContextItemControl(\'' + oObj.aData["9"] + '\',\'' + escape(oObj.aData["0"]) + '\',\'' + escape(oObj.aData["6"]) + '\',\'' + escape(oObj.aData["7"]) + '\',\'' + escape(oObj.aData["8"]) + '\',' + false + ')">' + '[+]' + '</span>';
        }
    }
];

var ContextItemDictTableColumnsStyle3 = [
    {
        "sName": "Name",
        "bSortable": false,
        "sWidth": "90%"
    },
    {
        "bSortable": false,
        "sWidth": "10%",
        "fnRender": function (oObj) {
            return '<span style="color:blue; cursor:pointer;" onclick="createContextItemControl(\'' + oObj.aData["9"] + '\',\'' + escape(oObj.aData["0"]) + '\',\'' + escape(oObj.aData["6"]) + '\',\'' + escape(oObj.aData["7"]) + '\',\'' + escape(oObj.aData["8"]) + '\',' + true + ')">' + '[+]' + '</span>';
        }
    }
];


var ContextItemDictTableColumnsStyle4 = [
    {
        "sName": "Name",
        "bSortable": false,
        "fnRender": function (oObj) {
            var ln = '<span style="color:blue;cursor:pointer;" title="' + oObj.aData[4] +'" onclick="addSemanticRelationItem(\'' + oObj.aData[0] + '\',\'' + oObj.aData[9] + '\')">' + oObj.aData[0] + '</span>';
            return ln;
        }
    }
];

var ContextItemDictTableColumnsStyle5 = [
    {
        "sName": "Name",
        "bSortable": false,
        "sWidth": "90%"
    },
    {
        "bSortable": false,
        "sWidth": "10%",
        "fnRender": function (oObj) {
            return '<span style="color:blue; cursor:pointer;" onclick="createContextItemControlsForMapping(\'' + oObj.aData["9"] + '\')">' + '[+]' + '</span>';
        }
    }
];

var ContextItemDictTableColumnsStyle6 = [
    {
        "sName": "Name",
        "bSortable": false,
        "fnRender": function (oObj) {
            var ln = '<span style="color:blue;cursor:pointer;" onclick="displayContextItem(\'' + oObj.aData["9"] + '\')">' + oObj.aData["0"] + '</span>';
            return ln;
        }
    }
];

var ContextItemDictTableColumnsStyle7 = [
    {
        "sName": "Name",
        "bSortable": false,
        "sWidth": "90%"
    },
    {
        "bSortable": false,
        "sWidth": "10%",
        "fnRender": function (oObj) {
            return '<span style="color:blue; cursor:pointer;" onclick="createContextItemControl(\'' + oObj.aData["9"] + '\',\'' + escape(oObj.aData["0"]) + '\',\'' + escape(oObj.aData["6"]) + '\',\'' + escape(oObj.aData["7"]) + '\',\'' + escape(oObj.aData["8"]) + '\',' + false + ',' + false + ',' + true + ')">' + '[+]' + '</span>';
        }
    }
];


var PlanDefintionTableColumns = [
    { "bSortable": false,
        "fnRender": function (oObj) {
            var isPathway = false;
            var img = '<img src="/Content/images/calendar.png"/>';
            if (oObj.aData["2"] && oObj.aData["2"].toLowerCase() == "clinicalpathway") {
                isPathway = true;
            }
            var link = '<span style="color:blue; cursor:pointer;" onclick = "return editPlan(\'' + oObj.aData["0"] + '\',\'' + oObj.aData["2"] + '\',' + true + ');">' + oObj.aData["1"] + '</span>';
            if (isPathway)
                return link + img;
            else
                return link;
        }
    }
];

function editPlan(id, type, showLoadingMessage) {
    // prompt user to save current work before leaving this page
    if ($('#plan-view').html().indexOf('plan-edit-form') >= 0 && isDirty($('#Id').val()) && confirm(getGlobalVar('Message_LeavePage')) === false) {
        return false;
    }
    if (showLoadingMessage === true || showLoadingMessage.toLowerCase() == 'true' || showLoadingMessage == 1) {
        // $('#dialog-loading').dialog("open");
        $.blockUI({ message: '<h4>' + getGlobalVar('Message_Wait') + '</h4>', fadeIn: 100 });
    }
    $('#plan_id').val(id);
    $('#plan_type').val(type);
    $("#plan-list-form").submit();
    return true;
}
