// It is desirable to put javascript function into .js file. However, pure js file cannot interpret server-side "@" clause for Razor engine.
// At the same time, JSDoc tags, such as @param cannot be used in Razor cshtml file, either.
// Our solution is to define getGlobalVar() to seperate server side razor from javascript function. So that javascript functions can be unit tested.

// Functions in this file are mostly DOM operations, so we didn't provide unit tests. Use web browser to test related functions directly (Unit test by QUnit is roughly the same).

/**
* The html template for generating phase tab
*/
var phaseTabTemplate = "<li><a href='#{href}'><span id='#{titleId}'>#{label}</span></a> <span class='ui-icon ui-icon-close' role='presentation'>Remove Tab</span> </li>";

var tabCounter = 2; // for constructing tab id
var orderGroupCounter = 2; // for constructing order group id
var orderCounter = 2; // for constructing order id

var freqOptions = [];
var routeOptions = [];
var dosageUnitOptions = [];
var optionKeys = ["text", "value"];

/**
* Move element up/ahead 
*
* @param {string} id Element id.
*
*/
function up(id) {
    $('#' + id).prev().before($('#' + id));
}

/**
* Move element down/afterward 
*
* @param {string} id Element id.
*
*/
function down(id) {
    $('#' + id).next().after($('#' + id));
}

/**
* Before saving, syn all shared data field in grouped order. 
* 
* @param {string} _id The html element for an order group, usually a table.
*/
function syncOrderGroupings(_id) {
    var orders = $("tr", "#" + _id);
    var gids = []; // record all group ids    

    // these fields will be synced
    var checks = {};
    var freqs = {};
    var tempo = {};
    var routes = {};

    for (var i = 0; i < orders.length; i++) {
        var id = orders[i].id;
        if (!id) continue; // means the header row, skip it

        // if item is not grouped, do nothing
        var gid = $("#" + id + "_grouping").val();
        if (!gid) continue;

        if (gids.contains(gid) === false) { // this is the first item in a group, record its fields.
            gids.push(gid);
            checks[gid] = document.getElementById(id + "_check").checked;
            freqs[gid] = document.getElementById(id + "_freq").value;
            tempo[gid] = document.getElementById(id + "_tempo").value;
            routes[gid] = document.getElementById(id + "_route").value;
        }
        else { // this is not the first item in a group, set its fields same as first item.
            document.getElementById(id + "_check").checked = checks[gid];
            document.getElementById(id + "_freq").value = freqs[gid];
            document.getElementById(id + "_tempo").value = tempo[gid];
            document.getElementById(id + "_route").value = routes[gid];
        }
    }
}

/**
* Under an order group, some orders may be grouped/combined into one composite order. This function rearranges order visual status. 
* 
* @param {string} _id The html element for an order group, usually a table.
*/
function updateOrderGroupings(_id) {
    var orders = $("tr", "#" + _id);
    var gids = []; // record all group ids
    var gleaders = {};  // record the first item in grouped order
    for (var i = 0; i < orders.length; i++) {
        var id = orders[i].id;
        if (!id) continue; // means the header row, skip it

        var gid=$("#"+id+"_grouping").val();
        if (!gid) { // this item is not grouped
            updateByOrderGrouping(true, id);
            continue; 
        }

        if (gids.contains(gid) === false) { // this is the first item in a group, record its id and group id.
            gids.push(gid);
            gleaders[gid] = id;
            updateByOrderGrouping(true, id);
        }
        else { // this is not the first item in a group, move it to its companions.
            $("#" + id).insertAfter($("#" + gleaders[gid]));
            gleaders[gid] = id; // next group item will be attached to current one
            updateByOrderGrouping(false, id);
        }
    }    
}

/**
* When order is put into a grouping order, hide check, frequency, and tempo type.
* 
* @param {boolean} visibility If value is false, hide check, frequency, and tempo type. Pass true only for the first item in a grouping order (means must be performed together).
* @param {string} id The html element for an medical order, usually a table row.
*/
function updateByOrderGrouping(visibility, id) {
    if (visibility === false) {
        $('#' + id + "_check").hide();
        $('#' + id + "_freq").hide();
        $('#' + id + "_tempo").hide();
        $('#' + id + "_route").hide();
    }
    else {
        $('#' + id + "_check").show();
        $('#' + id + "_freq").show();
        $('#' + id + "_tempo").show();

        updateByOrderType($('#' + id + "_type").val(), id); // hide route according to type
        updateByOrderTempoType($('#' + id + "_tempo").val(), id); // hide freq according to tempo
    }
}

/**
* When order is temporary, hide frequency.
* @param {string} tempo Longterm or temporary.
* @param {string} id The html element for an medical order, usually a table row.
*/
function updateByOrderTempoType(tempo, id) {
    if (tempo == getGlobalVar('enumTemporary')) {
        $('#' + id + "_freq").hide();
    }
    else {
        $('#' + id + "_freq").show();  
    }
}

/**
* When order type is not Medication, hide dosage, dosage unit, and administration route.
* @param {string} type Order type.
* @param {string} id The html element for an medical order, usually a table row.
*/
function updateByOrderType(type, id) {
    if (type != getGlobalVar('enumMedication')) {
        $('#' + id + "_dosage").hide();
        $('#' + id + "_dosageUnit").hide();
        $('#' + id + "_route").hide();
    }
    else {
        $('#' + id + "_dosage").show();
        $('#' + id + "_dosageUnit").show();
        $('#' + id + "_route").show();
        //updateOrderGroupings($("#" + id).parents('table')[0].id);
    }    
}

/**
* Add a medical order item into specified order table.
* 
* @param {string} id Table id.
* @param {string} _name Medical order name.
* @param {string} _type Medical order type. e.g. Medication, Operation, Surgery, etc.
* @param {string} _tempo Medical order temporal type. i.e. temporary or longterm
* @param {string} _dosage Medical order dosage. e.g. 50mg
* @param {string} _freq Medical order frequency.
* @param {string} _route Administration route.
* @param {string} _instruction Additional instruction, such as the duration of a longterm medication order.
* @param {string} _fk FK_EMR_Id, the id of this medical order in the global dictionary, for exchanging message with EMR/CPOE system.
* @param {bool} _checked Whether the order item is selected by default.
* @param {string} _grouping Grouping Id.
* @param {string} _plugins Associated plugin ids.
* @param {string} _skin_test Skin test.
* @param {string} isMedicationGuide A control flag to indicate Medication Guide page.
*
*/
function addOrder(id, _name, _type, _tempo, _dosage, _dosage_unit, _freq, _route, _instruction, _fk, _checked, _grouping, _plugins, _skin_test, isMedicationGuide) {
    if (freqOptions.length === 0 && routeOptions.length === 0 && dosageUnitOptions.length === 0) {
        $.ajax({
            url: "/AjaxHandler/OrderEntryOptionsAjaxHandler",
            type: 'POST',
            async: false, // wait until ajax returns
            cache: false,
            timeout: 10000,
            dataType: 'json'
        }).done(function (data) {            
            freqOptions = data.freqOptions;
            routeOptions = data.routeOptions;
            dosageUnitOptions = data.dosageUnitOptions;
        });
    }
    
    var thisId = "order_" + (orderCounter++); //specify a unique id
    var text = '<tr id="' + thisId + '">';

    var checked = document.createElement("input");
    var checkedId = thisId + "_check";
    checked.setAttribute("id", checkedId);
    checked.setAttribute("type", "checkbox");
    checked.setAttribute("onmouseover", "this.style.cursor='pointer';");
    // checked.checked = _checked;
    if (_checked === true)
        checked.setAttribute("checked", "checked");
    if (isMedicationGuide) {
        text += "<td style='display:none'>";
    }
    else {
        text += "<td>";
    }
    
    text += checked.outerHTML + '</td><td>';
    
    var orderEntryId = thisId + "_entry";
    if (!_name) _name="";
    var name = "<input id='" + orderEntryId + "' style='width:150px;'></input>";
    if (isMedicationGuide)
    {
        name = "<input id='" + orderEntryId + "' style='width:150px;' disabled='disabled'></input>";
    }
    text += name + '</td><td>';

    var type = document.createElement('select');
    var orderTypeId = thisId + "_type";
    type.setAttribute("id", orderTypeId);
    // type.setAttribute("disabled", "disabled");
    if (!_type) _type = "";
    addSelectOption(type, getGlobalVar('enumMedication'), getGlobalVar('Medication'),_type);
    addSelectOption(type, getGlobalVar('enumSurgery'), getGlobalVar('Surgery'), _type);
    addSelectOption(type, getGlobalVar('enumTest'), getGlobalVar('Test'),_type);
    addSelectOption(type, getGlobalVar('enumExamination'), getGlobalVar('Examination'), _type);
    addSelectOption(type, getGlobalVar('enumOperation'), getGlobalVar('Operation'), _type);
    addSelectOption(type, getGlobalVar('enumNursing'), getGlobalVar('Nursing'), _type);
    addSelectOption(type, getGlobalVar('enumDiet'), getGlobalVar('Diet'), _type);
    addSelectOption(type, getGlobalVar('enumOther'), getGlobalVar('Other'), _type);
    addSelectOption(type, getGlobalVar('enumCheckList'), getGlobalVar('CheckList'), _type);
    addSelectOption(type, getGlobalVar('enumNursingCheckList'), getGlobalVar('NursingCheckList'), _type);
//        if (_type)
//            type.value = _type; // .value property doesn't work on IE9
//        else
//            type.selectedIndex = 0;
    text += type.outerHTML + '</td><td>';

    var temporalType = document.createElement("select");
    var orderTempoId = thisId + "_tempo";
    temporalType.setAttribute("id", orderTempoId);
    if (!_tempo) _tempo = "";
    addSelectOption(temporalType, getGlobalVar('enumTemporary'),getGlobalVar('Temporary') , _tempo);
    addSelectOption(temporalType, getGlobalVar('enumLongterm'), getGlobalVar('LongTerm'), _tempo);
//        if (_tempo)
//            temporalType.value = _tempo;
//        else
//            temporalType.selectedIndex = 0;
    text += temporalType.outerHTML + '</td><td>';

    if (!_dosage) _dosage = "";
    var orderDosageId = thisId + "_dosage";
    var dosage = "<input id='" + orderDosageId + "' value='" + _dosage + "' style='width:40px;'></input>";
    text += dosage + '</td><td>';

//    var orderDosageUnitId = thisId + "_dosageUnit";
//    if (!_dosage_unit) _dosage_unit = "";
//    var dosageUnit = "<input id='" + orderDosageUnitId + "' value='" + _dosage_unit + "' style='width:20px;'></input>"
    //    text += dosageUnit + '</td><td>';

    var dosageUnit = document.createElement("select");
    var orderDosageUnitId = thisId + "_dosageUnit";
    if (!_dosage_unit) _dosage_unit = "";    
    dosageUnit.setAttribute("id", orderDosageUnitId);
    for (var i = 0; i < dosageUnitOptions.length; i++) {
        addSelectOption(dosageUnit, dosageUnitOptions[i][optionKeys[1]], dosageUnitOptions[i][optionKeys[0]], _dosage_unit);
    }
    text += dosageUnit.outerHTML + '</td><td>';

    /*
    var dosageUnit = document.createElement("input");
    var orderDosageUnitId = thisId + "_dosageUnit";
    if (!_dosage_unit) _dosage_unit = "";
    dosageUnit.setAttribute("id", orderDosageUnitId);
    var dosageUnitOptionsArr = [];
    for (var i = 0; i < dosageUnitOptions.length; i++) {
    dosageUnitOptionsArr.push(dosageUnitOptions[i][optionKeys[0]]);
    //addSelectOption(dosageUnit, dosageUnitOptions[i][optionKeys[1]], dosageUnitOptions[i][optionKeys[0]], _dosage_unit);
    }
    $("#" + orderDosageUnitId).autocomplete({
    source: dosageUnitOptionsArr
    });    
    text += dosageUnit.outerHTML + '</td><td>';
    */

//        var freq = document.createElement("input");
//        var orderFreqId = thisId + "_freq";
//        if (_freq) freq.value = _freq;
//        freq.setAttribute("id", orderFreqId);
//        freq.setAttribute("style", "width:70px;");
//        text += freq.outerHTML + '</td><td>';
    var freq = document.createElement("select");
    var orderFreqId = thisId + "_freq";
    if (_freq) freq.value = _freq;
    freq.setAttribute("id", orderFreqId);
    for (var j = 0; j < freqOptions.length; j++) {
         addSelectOption(freq, freqOptions[j][optionKeys[1]], freqOptions[j][optionKeys[0]],_freq);
     }
    text += freq.outerHTML + '</td><td>';

    var route = document.createElement("select");
    var orderRouteId = thisId + "_route";
    route.setAttribute("id", orderRouteId);
    if (!_route) _route = "";
    route.setAttribute("id", orderRouteId);
    for (var k = 0; k < routeOptions.length; k++) {
        addSelectOption(route, routeOptions[k][optionKeys[1]], routeOptions[k][optionKeys[0]], _route);
    }
    text += route.outerHTML + '</td><td>';

    var instructId = thisId + "_instruct";
    if (!_instruction) _instruction = "";
    var instruct = "<input id='" + instructId + "' value='" + _instruction + "' style='width:120px;'></input>";
    text += instruct + '</td><td>';

    var skinTest = document.createElement("input");
    var skinTestId = thisId + "_skin_test";
    skinTest.setAttribute("id", skinTestId);
    skinTest.setAttribute("type", "checkbox");
    skinTest.setAttribute("onmouseover", "this.style.cursor='pointer';");    
    if (_skin_test === true)
        skinTest.setAttribute("checked", "checked");
    text += skinTest.outerHTML + '</td>';
    
    var groupingId = thisId + "_grouping";
    if (!_grouping) _grouping = "";
    var grouping = document.createElement("select");
    grouping.setAttribute("id", groupingId);
    addSelectOption(grouping, "", "", _grouping);
    for (var l = 1; l <= 10; l++) {
        addSelectOption(grouping, l.toString(), l.toString(), _grouping);
    }
    // to support medication order grouping for drug medication guide, make a Drug - TaskDefinition association
    if (isMedicationGuide) {
        text += "<td style='display:none'>";
    }
    else {
        text += "<td>";
    }
    text += grouping.outerHTML + '</td>';

    text += '<td style="display:none;">';
    var fkId = thisId + "_fk";
    if (!_fk) _fk = "";
    var fk = "<input type='hidden' id='" + fkId + "' value='" + _fk + "'></input>";
    text += fk + '</td>';

    text += '<td style="display:none;">';
    var pluginsId = thisId + "_plugins";
    if (!_plugins) _plugins = "";
    var plugins = "<input type='hidden' id='" + pluginsId + "' value='" + _plugins + "'></input>";
    text += plugins + '</td><td>';

    var pluginIcon = createImgBtnHtml("/Content/images/tool.jpg", 'register(\'' + pluginsId + '\');', getGlobalVar('ToolTip_Plugin'));
    if (isMedicationGuide) {
        pluginIcon = "";
    }
    text += '<div style="width:40px;">' + createImgBtnHtml("/Content/images/trash.png", 'del(\'' + thisId + '\');', getGlobalVar('ToolTip_RemoveOrder')) + pluginIcon + "</div>" + '</td></tr>';
    
    $('#' + id + ' tr:last').after(text);

    // hide frequency edit when order is temporary
    updateByOrderTempoType($('#' + orderTempoId).val(), thisId);
    $('#' + orderTempoId).change(function () {
        updateByOrderTempoType($(this).val(), thisId);
    });

    // hide dosage and route if order type is not medication
    updateByOrderType($('#' + orderTypeId).val(), thisId);
    $('#' + orderTypeId).change(function () {
        updateByOrderType($(this).val(), thisId);
    });

    $('#' + groupingId).change(function () {
        $('#' + orderEntryId).css('background-color', getStockColor($(this).val()));
        $(this).css('background-color', getStockColor($(this).val()));
        updateOrderGroupings($("#" + thisId).parents('table')[0].id);
    });

    /*
    $('#' + groupingId).spinner({
        // min: 0,
        max:10, // set max spinnable value, but user can manually enter number greater than this.
        step: 1,
        // spin event will not trigger change event.
        spin: function (event, ui) {
            if (ui.value < 1) {
                $(this).spinner("value", "");
                $('#' + orderEntryId).css('background-color', getStockColor(""));
                $(this).css('background-color', getStockColor(""));
                updateOrderGroupings($("#" + thisId).parents('table')[0].id);
                return false;
            }
            $('#' + orderEntryId).css('background-color', getStockColor(ui.value));
            $(this).css('background-color', getStockColor(ui.value));
            updateOrderGroupings($("#" + thisId).parents('table')[0].id);
        },
        // change event is for directly inputing value
        change: function (event, ui) {
            $('#' + orderEntryId).css('background-color', getStockColor($(this).spinner("value")));
            $(this).css('background-color', getStockColor($(this).spinner("value")));
            updateOrderGroupings($("#"+thisId).parents('table')[0].id);
        }
    });
    $('#' + groupingId).spinner("value", _grouping);
    */
    $('#' + orderEntryId).css('background-color', getStockColor(_grouping));
    $('#' + groupingId).css('background-color', getStockColor(_grouping));

    $('#' + orderEntryId).autocomplete({
        minLength: 2,
        source: function (request, response) {
            $.ajax({
                url: '/AjaxHandler/OrderEntryAjaxHandler',
                type: "GET",
                dataType: "json",
                data: { term: request.term, type: $('#' + orderTypeId).val() },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { orderName: item.Name, orderType: item.OrderType, orderDosage: item.Dosage, orderDosageUnit: item.DosageUnit, code: item.Code, codingSystem: item.CodingSystem, fk: item.FK_EMR_Id, group: item.Group, description: item.Description };
                    }));
                }
            });
        },
        //        focus: function (event, ui) {
        //            //$('#' + orderEntryId).val(ui.item.orderName); 
        //            return false;
        //        },
        select: function (event, ui) {
            $('#' + orderEntryId).val(ui.item.orderName);
            $('#' + orderTypeId).val(ui.item.orderType);
            $('#' + orderDosageId).val(ui.item.orderDosage);
            $('#' + fkId).val(ui.item.fk);
            return false;
        }
    }).data("ui-autocomplete")._renderItem = function (ul, item) {
        var ln = "<a>" + item.orderName + "</a>";
        if (item.description) {
            ln = "<a>" + item.orderName + "<br>" + item.description + "</a>";
        }
        return $("<li></li>").data("item.autocomplete", item).append(ln).appendTo(ul);
    };
    // Defect: Upon page enter, autocomplete control get focus and show drop list. e.g. Plan Edit & Drug Edit
    // This Defect only happens in IE. Chrome is OK.
    // Solve: set value after .autocomplete()
    $('#' + orderEntryId).val(_name); 
    $("#" + id).tableDnD();
}

/**
* Add a order group into specified node.
* Order group concept corresponds to Task class
* 
* @param {string} id Element id.
* @param {string} _name Group name.
* @param {string} _optional Whether the group is optional.
* @param {string} _multiselect Whether allow select more than one orders within the group.
* return {string} Element id into which orders of this group should be inserted. It is usually a table
*
*/
function addTask(id, _name, _optional, _multiselect) {
    if (!id) {
        return "";
    }
    var bounds = document.createElement("li");
    var groupId = id + "-" + orderGroupCounter;
    var groupOptionalId = id + "task-optional";
    var tableId = id + "-" + orderGroupCounter + "-table";
    var groupNameId = id + "-" + orderGroupCounter + "-name";
    var groupNameLabelId = id + "-" + orderGroupCounter + "-label";
    orderGroupCounter++;
    bounds.setAttribute("id", groupId);
    bounds.setAttribute("class", "ui-state-default");

    // create accordion header
    var header = document.createElement("h3");
    // header.appendChild(document.createTextNode(getGlobalVar("GroupName")));

    // create task name inside header
    if (!_name) _name = "";
    header.innerHTML += "&nbsp;<span id='" + groupNameLabelId + "'>" + _name + "</span>";

    // create add order and remove task img buttons inside header
    header.innerHTML += '<div style="float:right; position: relative; left:-5px;">' +
        createImgBtnHtml("/Content/images/UpArrow.png", 'up(\'' + groupId + '\');', getGlobalVar("ToolTip_Up")) +
        createImgBtnHtml("/Content/images/DownArrow.png", 'down(\'' + groupId + '\');', getGlobalVar("ToolTip_Down")) +
        createImgBtnHtml("/Content/images/add_black.png", 'addOrder(\'' + tableId + '\');', getGlobalVar("ToolTip_AddNewOrder")) +
        createImgBtnHtml("/Content/images/import.png", 'importOrder(\'' + tableId + '\');', getGlobalVar("ToolTip_ImportOrder")) +
        createImgBtnHtml("/Content/images/trash.png", 'del(\'' + groupId + '\');', getGlobalVar("ToolTip_RemoveOrderGroup")) + '</div>';

    bounds.appendChild(header);

    // create accordion content
    var content = document.createElement("div");
    content.setAttribute("style", "margin:0; padding:0; border-style:none");

    // create task optional and multiselect checkboxes inside accordion content
    var groupHeader = document.createElement("div");
    groupHeader.setAttribute("style", "margin:5px; padding:0; border-style:none");

    groupHeader.appendChild(document.createTextNode(getGlobalVar("GroupName")));
    groupHeader.innerHTML += "<input style='margin:5px' id='" + groupNameId + "' name='task-name' style='width:200px;'/>";

    groupHeader.innerHTML += "<br/>";

    var optional = document.createElement("input");
    optional.setAttribute("name", "task-optional");
    optional.setAttribute("id", groupOptionalId);
    optional.setAttribute("type", "checkbox");
    // optional.setAttribute("style", "float:right"); //width:70px; height:30px
    // optional.setAttribute("style", "display:none");
    if (_optional === true)
        optional.setAttribute("checked", "checked");
    groupHeader.appendChild(optional);
    groupHeader.appendChild(document.createTextNode(getGlobalVar("IsOptional")));

    groupHeader.innerHTML += "&nbsp;&nbsp;";

    var mutliselect = document.createElement("input");
    mutliselect.setAttribute("name", "task-multiselect");
    mutliselect.setAttribute("type", "checkbox");
    // mutliselect.checked = _multiselect;
    if (_multiselect === true) {
        mutliselect.setAttribute("checked", "checked");
    }
    groupHeader.appendChild(mutliselect);
    groupHeader.appendChild(document.createTextNode(getGlobalVar("IsMultiSelect")));

    content.appendChild(groupHeader);

    // create order table inside accordion content
    content.innerHTML = content.innerHTML +
        '<table style=" font-weight:lighter" id="' +
        tableId + '" name="order-table"><thead><tr class="nodrop nodrag"><th>' +
        getGlobalVar('Checked') + '</th><th>' +
        getGlobalVar('Name') + '</th><th>' +
        getGlobalVar('Type') + '</th><th>' +
        getGlobalVar('LongtermOrTemp') + '</th><th>' +
        getGlobalVar('Dosage') + '</th><th>' +
        getGlobalVar('DosageUnit') + '</th><th>' +
        getGlobalVar('Frequency') + '</th><th>' +
        getGlobalVar('Route') + '</th><th>' +
        getGlobalVar('AdditionalInstruction') + '</th><th>' +
        getGlobalVar('SkinTest') + '</th><th>' +
        getGlobalVar('Grouping') + '</th><th style="display:none;">' +
        getGlobalVar('FK_MedicalOrder_Id') + '</th><th style="display:none;">' +
        getGlobalVar('Plugins') + '<th></th></tr></thead><tbody/></table>';

    bounds.appendChild(content);

    document.getElementById(id).appendChild(bounds);

    // make order table drag'n'drop
    $("#" + tableId).tableDnD();

    // clicking in text input and img button without triggering accordion action
    $("#" + groupId + " > h3 > input[type=text]").click(function (event) {
        event.stopPropagation();
        event.preventDefault();
    });
    $("#" + groupId + " > h3 > div").click(function (event) {
        event.stopPropagation();
        event.preventDefault();
    });

    // Defect: checkbox cannot be checked if placed on accordion header. Solved by moving them to accordion content area.
    // $("#" + groupId + " > h3 > input.headercheckbox").live("click", function (event) {
    //    event.stopPropagation();
    //    event.preventDefault();
    // var o = $(this);
    // o.append("checked='yes'");
    // $(this)[0].prop("checked", !$(this)[0].is(":checked"));            
    // $(this)[0].checked = !$(this)[0].checked;

    //$(this)[0].attr('checked', 'checked');
    //$(this)[0].attributes['checked'].value='checked';
    //$(this)[0].removeAttr('checked')
    //$(this)[0].prop('checked', true);

    // this.prop('checked', true);
    //$(this).find("input").attr("checked", "checked");
    // });
    //$(".headercheckbox").prop('checked', true);

    // make order group accordion
    $("#" + groupId).accordion({ heightStyle: "content", header: "h3", collapsible: true, clearStyle: true });

    $("#" + groupNameId).autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/AjaxHandler/OrderGroupAjaxHandler',
                type: "GET",
                dataType: "json",
                data: { term: request.term },
                success: function (data) {
                    response(data);
                }
            });
        },
        select: function (event, data) {
            document.getElementById(groupNameLabelId).innerHTML = data.item.value;
            // document.getElementById(groupNameId).innerHTML = data.item.value;
            return true; // return false to cancel copying the suggestion to the textbox
        }
    });
    $("#" + groupNameId).val(_name);
    $("#" + groupNameId).change(function () {
        document.getElementById(groupNameLabelId).innerHTML = $(this).val();
    });

    // Defect: autocomplete textbox cannot use left/right/up/down arrow keys when put inside the accordion tab
//    $("#" + groupNameId).live("keydown", function (event) {
//        // 37 - left; 38 - up; 39 - right; 40 - down
//        if (event.keyCode == 38 || event.keyCode == 40) {
//            event.stopPropagation();
//            event.preventDefault();
//        }
//    });

    // Defect: ezMark can only render 1st instance of checkbox
    // $('#' + groupOptionalId).ezMark({ checkboxCls: 'ez-checked-green', checkedCls: 'ez-checkbox-green' }); // here the checkbox state is OPPOSITE: the checkbox means "expand the order group in client" or "not optional"

    return tableId;
}

/**
* Add a phase to UI for clinical pathway
* 
* @param {string} id Element id under which phase info will be appended. It is usually a tabs control.
* @param {string} _name Phase name.
* @param {string} _period Phase period.
* return {string} The element id into which tasks of this phase should be appened.
*/
function addPhase(id, _name, _period) {
    var name = _name;
    if (!_name)
        name = "Phase " + tabCounter;
    var tab_id = "tabs-" + tabCounter;
    var li = $(phaseTabTemplate.replace(/#\{href\}/g, "#" + tab_id).replace(/#\{titleId\}/g, "title-" + tab_id).replace(/#\{label\}/g, name));
    var content = "<div id='" + tab_id + "' name='phase-id'><div>" +
    getGlobalVar('Phase') + '&nbsp;<input type="text" name="phase-name" style="width:100px" value="' + name + '" onchange="document.getElementById(\'title-' + tab_id + '\').innerHTML = $(this).val();"' + '/>' +
    '<span>&nbsp;&nbsp;</span>' +
    getGlobalVar('Period') + '&nbsp;<input type="text" name="phase-period" style="width:100px" value="' + _period + '"/>' + "</div><br/>" +
    "<fieldset><legend>" + "<span style='font-size:small'>" + getGlobalVar('OrderGroups') + "&nbsp;</span>" +
    createImgBtnHtml("/Content/images/add_black.png", 'addTask(\'' + "tasks-" + tab_id + '\');', getGlobalVar('ToolTip_AddNewOrderGroup')) +
    "</legend>" +
    "<ul id='" + "tasks-" + tab_id + "' class='sortable'/>" + "</fieldset>" +
    "<div style='font-style:italic;font-size:small'>" + getGlobalVar('ToolTip_CanDragGroupAndOrdersToAdjustOrder') + "<br/>" + getGlobalVar('ToolTip_GroupingOrders') + "</div>" + "</div>";

    $("#" + id).find(".ui-tabs-nav").append(li);
    $("#" + id).append(content);
    $("#" + id).tabs("refresh");
    $("#" + id).tabs("option", "active", -1);
    tabCounter++;

    //$("#" + "tasks-" + tab_id).sortable({ revert: true }); //Disable dnd for tasks. Sometimes causes scroll bar focus error.
        
    return ("tasks-" + tab_id);
}

/**
* Add order ui controls from corresponding json object. This function calls addOrder() inside.
* 
* @param {string} id Element id under which order ui controls will be appended.
* @param {string} obj Json object.
*/
function ordersFromJson(id, obj, isMedicationGuide) {
    var i = 0;
    for (; i < obj.length; i++) {
        x = obj[i];
        // no need to escape parameter. the parameters are surrounded by "".
        addOrder(id, x[getGlobalVar('Name')],
                x[getGlobalVar('Type')],
                x[getGlobalVar('LongtermOrTemp')],
                x[getGlobalVar('Dosage')],
                x[getGlobalVar('DosageUnit')],
                x[getGlobalVar('Frequency')],
                x[getGlobalVar('Route')],
                x[getGlobalVar('AdditionalInstruction')],
                x[getGlobalVar('FK_MedicalOrder_Id')],
                x[getGlobalVar('Checked')],
                x[getGlobalVar('Grouping')],
                x[getGlobalVar('Plugins')],
                x[getGlobalVar('SkinTest')],
                isMedicationGuide);
    }
}

/**
* Add task ui controls from corresponding json object. This function calls addTask() inside.
* 
* @param {string} id Element id under which task ui controls will be appended.
* @param {string} obj Json object.
*/
function tasksFromJson(id, obj) {
    for (var i = 0; i < obj.length; i++) {
        _id = addTask(id, obj[i][getGlobalVar('Name')], obj[i][getGlobalVar('IsOptional')], obj[i][getGlobalVar('IsMultiSelect')]);
        ordersFromJson(_id, obj[i][getGlobalVar('Orders')]);
        updateOrderGroupings(_id);
    }
}

/**
* Add phase tabs from corresponding json object. This function calls addPhase() inside.
* 
* @param {string} id Element id under which phase tab will be appended. It is usually a tabs control.
* @param {string} obj Json object.
*/
function phasesFromJson(id, obj) {
    for (var i = 0; i < obj.length; i++) {
        _id = addPhase(id, obj[i][getGlobalVar('Name')], obj[i][getGlobalVar('Period')]);
        tasksFromJson(_id, obj[i][getGlobalVar('Tasks')]);
    }
}

/**
* Read orders info to json object
* 
* @param {string} id Html table id that contains order info.
*/
function ordersToJson(id) {
    var table = document.getElementById(id);
    var trs = table.rows,
        trl = trs.length,
        i = 0,
        j = 0,
        keys = [],
        obj, ret = [];

    for (; i < trl; i++) {
        if (i === 0) {
            for (; j < trs[i].children.length; j++) {
                keys.push(trs[i].children[j].innerHTML.trim());
            }
        } else {
            obj = {};
            for (j = 0; j < trs[i].children.length; j++) {
                var ctl = trs[i]/*<tr>*/.children[j]/*<td>*/.children[0]/**/;
                if (ctl.type === undefined) {
                    for (var k = 1; k < trs[i].children[j].children.length; k++) {
                        if (trs[i].children[j].children[k].type == "text" && trs[i].children[j].children[k].id) {
                            obj[keys[j]] = $('#'+trs[i].children[j].children[k].id).val(); // Autocomplete can only get value by jQuery method
                            break;
                        }
                    }
                }
                else if (ctl.type == "checkbox")
                    obj[keys[j]] = ctl.checked;
                else
                    obj[keys[j]] = ctl.value;
            }
            ret.push(obj);
        }
    }
    return ret;
}

/**
* Read tasks info to json object
* 
* @param {string} id Html element id that contains task info.
*/
function tasksToJson(id) {
    // <ul id='" + "tasks-" + id + "' class='sortable'/>
    var ret = [];
    var keys = [getGlobalVar('Name'), getGlobalVar('IsOptional'), getGlobalVar('IsMultiSelect'), getGlobalVar('Orders')];

    //var html = document.getElementById(id).outerHTML;        
    //writeClipboard(html);

    var names = $('#' + id + ' > li :input[name="task-name"]');
    var optionals = $('#' + id + ' > li :input[name="task-optional"]');
    var multiselects = $('#' + id + ' > li :input[name="task-multiselect"]');
    var orderTableIDs = $('#' + id + ' > li table[name="order-table"]');

    if (names && optionals && multiselects && orderTableIDs && names.length == optionals.length && optionals.length == multiselects.length && multiselects.length == orderTableIDs.length && names.length > 0) {
        for (var i = 0; i < names.length; i++) {
            syncOrderGroupings(orderTableIDs[i].attributes['id'].value);
            var obj = {}; // {} means a key-value pair; [] means an array
            obj[keys[0]] = names[i].value;
            obj[keys[1]] = optionals[i].checked;
            obj[keys[2]] = multiselects[i].checked;
            obj[keys[3]] = orderTableIDs[i].attributes['id'].value;
            obj[keys[3]] = ordersToJson(obj[keys[3]]);
            ret.push(obj);
        }
    }

    return ret;
}

/**
* Read phases info to json object
* 
* @param {string} id Html element id that contains phase info.
*/
function phasesToJson(id) {
    var ret = [];
    var keys = [getGlobalVar('Name'), getGlobalVar('Period'), getGlobalVar('Tasks')];

    var names = $(':input[name="phase-name"]', $('#' + id));
    var periods = $(':input[name="phase-period"]', $('#' + id));
    var tasksIDs = $('div[name="phase-id"] ul', $('#' + id));
    if (names.length == periods.length && names.length > 0) {
        var i = 0;
        for (; i < names.length; i++) {
            var obj = {}; // {} means a key-value pair; [] means an array
            obj[keys[0]] = names[i].value;
            obj[keys[1]] = periods[i].value;
            obj[keys[2]] = tasksIDs[i].attributes['id'].value;
            ret.push(obj);
        }
    }

    // adjust phase order in case user dragged them
    // [Compatibility Issue] 
    // IE & Chrome support innerText, while Firefox doesn't.
    // Firefox supports textContent, while IE doesn't.
    // Best bet is to use innerHTML instead of innerText or textContent.
    // http://stackoverflow.com/questions/1359469/innertext-works-in-ie-but-not-in-firefox
    var phaseNames = $('.ui-tabs-nav > li > a > span', $('#' + id)).map(function () {
        return $(this)[0].innerHTML;
    }).get();

    // adjust phase json (phase names should be different to identify each phase)
    var ajusted = [];
    i = 0;
    for (; i < phaseNames.length; i++) {
        var j = 0;
        for (; j < ret.length; j++) {
            if (ret[j][keys[0]] == phaseNames[i]) {
                ret[j][keys[2]] = tasksToJson(ret[j][keys[2]]); // get tasks json
                ajusted.push(ret[j]);
                break;
            }
        }
    }

    return ajusted;
}

function addPhaseDlg() {
    $("#dialog-phase").dialog("open");
}

function importOrder(id) {
    $("#import-target-order-group-id").val(id);
    $("#dialog-import-order").dialog('open');
}

function importPathway() {
    $("#dialog-import-pathway").dialog('open');
}

$(document).ready(function () {
    var OK = getGlobalVar('OK');
    var Cancel = getGlobalVar('Cancel');

    var dialog = $("#dialog-phase").dialog(
        {
            autoOpen: false,
            modal: true,
            buttons: {
                OK: function () {
                    addPhase('phase_tabs', $("#phase_name").val(), $("#phase_period").val());
                    $(this).dialog("close");
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            },
            close: function () {
                $("#phase_name").val("");
                $("#phase_period").val("");
            }
        });

    $("#phase_tabs").tabs({
        select: function (event, ui) {
            //                var data = $("#common_form").serialize();
            //                if (ui.index == 1) {
            //                    var url = '${tab2_url}' + "&" + data;
            //                    $("#tabs").tabs("url", 1, url); //this is new !
            //                }
            return true;
        }
    });
    $("#phase_tabs").delegate("span.ui-icon-close", "click", function () {
        var panelId = $(this).closest("li").remove().attr("aria-controls");
        $("#" + panelId).remove();
        $("#tabs").tabs("refresh");
    });
    $('#phase_tabs .ui-tabs-nav').sortable({
        axis: 'x'
    }); // can drag to adjust order
});

/**
* Judge whether current plan is DIRTY OR NOT
* 
* @param {string} _id Plan object id.
*
* @return {boolean} whether current plan has been edited
*/
function isDirty(_id) {
    var ret = false;
    $.ajax({
        url: '/AjaxHandler/PlanDefinitionAjaxHandler',
        type: 'GET',
        data: { id: _id },
        async: false, // wait until ajax returns
        cache: false,
        timeout: 5000,
        dataType: 'json'
    }).done(function (json) {
        ret = checkChange(json);
    });
    return ret;
}

/**
* Judge whether current plan is DIRTY OR NOT. Called by isDirty().
* 
* @param {object} json A plan object returned from ajax call.
*
* @return {boolean} whether current plan has been changed
*/
function checkChange(json) {
    if (!json.plan) {
        return false;
    }

    if (json.plan.Id === 0) {  // the create case
        return true;
    }

    if ($('#Name').val() || json.plan.Name) {
        if ($('#Name').val() != json.plan.Name)
            return true;
    }
    if ($('#Cost').val() || json.plan.Cost) {
        if ($('#Cost').val() != json.plan.Cost)
            return true;
    }
    if ($('#Criteria').val() || json.plan.Criteria) {
        if ($('#Criteria').val() != json.plan.Criteria)
            return true;
    }
    if ($('#Duration').val() || json.plan.Duration) {
        if ($('#Duration').val() != json.plan.Duration)
            return true;
    }
    if ($('#InputCode').val() || json.plan.InputCode) {
        if ($('#InputCode').val() != json.plan.InputCode)
            return true;
    }

    prepareSubmit();
    var o1 = JSON.parse($('#jsonString').val());
    var o2 = JSON.parse(json.plan.Json);

    var diff = objectDiff.diff(o1, o2); // objectDiff.diff() performs a recursive/deep comparison
    return !(diff.changed == 'equal');
}