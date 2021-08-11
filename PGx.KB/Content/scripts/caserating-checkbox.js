

function Reset() {
    $("#CaseRatingProfile-edit").resetForm();
}
function Clear() {
    $("#CaseRatingProfile-edit").clearForm();
}
//checkboxForLabTest
function status_checkbox1(item) {
    var checkbox_value = item.value;
    if ($(item).is(":checked")) {
        var statue_checkbox_count = 0;
        for (var j = 0; j < checked_Item1.length; j++) {
            if (checked_Item1[j] == checkbox_value) {
                statue_checkbox_count++;
            }
        }
        if (statue_checkbox_count == 0)
        { checked_Item1.push(checkbox_value); }
    }
    else {
        debugger;
        var k;
        for (var i = 0; i < checked_Item1.length; i++) {

            if (checked_Item1[i] == checkbox_value) {
                k = i;
            }
        }
        checked_Item1.splice(k, 1);
    }
}
//checkboxSymptoms
function status_checkbox2(item) {
    var checkbox_value = item.value;
    if ($(item).is(":checked")) {
        var statue_checkbox_count = 0;
        for (var j = 0; j < checked_Item2.length; j++) {
            if (checked_Item2[j] == checkbox_value) {
                statue_checkbox_count++;
            }
        }
        if (statue_checkbox_count == 0)
        { checked_Item2.push(checkbox_value); }
    }
    else {
        var k;
        for (var i = 0; i < checked_Item2.length; i++) {

            if (checked_Item2[i] == checkbox_value) {
                k = i;
            }
        }
        checked_Item2.splice(k, 1);
    }
}
//LabTestFeatures全选
function btnCheckAllOrNone1(select_all) {
    var a = document.getElementsByName("chkItem1");
    if ($(select_all).is(":checked")) {
        for (var i = 0; i < a.length; i++) {
            if (a[i].type == "checkbox") {
                a[i].checked = true;
                var btnCheckAll_count = 0;
                for (var j = 0; j < checked_Item1.length; j++) {
                    if (checked_Item1[j] == a[i].value) {
                        btnCheckAll_count++;
                    }
                }
                if (btnCheckAll_count == 0) { checked_Item1.push(a[i].value); }
            }
        }
    }
    else {
        for (var i = 0; i < a.length; i++) {
            if (a[i].type == "checkbox") {
                a[i].checked = false;
            }
        }
        var count = checked_Item1.length;
        checked_Item1.splice(0, count);
    }
}
//SymptomsFeatures全选

function btnCheckAllOrNone2(select_all) {
    var a = document.getElementsByName("chkItem2");
    if ($(select_all).is(":checked")) {
        for (var i = 0; i < a.length; i++) {
            if (a[i].type == "checkbox") {
                a[i].checked = true;
                var btnCheckAll_count = 0;
                for (var j = 0; j < checked_Item2.length; j++) {
                    if (checked_Item2[j] == a[i].value) {
                        btnCheckAll_count++;
                    }
                }
                if (btnCheckAll_count == 0) { checked_Item2.push(a[i].value); }
            }
        }
    }
    else {
        for (var i = 0; i < a.length; i++) {
            if (a[i].type == "checkbox") {
                a[i].checked = false;
            }
        }
        var count = checked_Item2.length;
        checked_Item2.splice(0, count);
    }
}
function LoadItemLabTest(department) {
    var lan = getLanguage();
    $("#itemforlabtest").dataTable({
        "oLanguage": lan,
        "bJQueryUI": true,
        "sAjaxSource": "/CaseRatingProfile/DisplayLabTestItemCapitaAmount?Department=" + department,
        "bServerSide": true,
        "bDestroy": true,
        "bRetrieve": true,
        "bAutoWidth": true,
        "bProcessing": true,
        "bSort": false,
        "aLengthMenu": [[20, 50, 100, 100000], [20, 50, 100, "All"]],
        "iDisplayLength": 20,
        "aoColumns": [
            { "bVisible": false },
            {
                "bSearchable": false,
                "fnRender": function (oObj) {
                    var ln = '<input type="checkbox" name="chkItem1" onclick="status_checkbox1(this);" value="' + oObj.aData[0] + '">';
                    return ln;
                }
            },
            {},
            {}
        ]
    });
}
function LoadItemSymptoms(department) {
    var lan = getLanguage();
    $("#itemforsymptom").dataTable({
        "oLanguage": lan,
        "bJQueryUI": true,
        "sAjaxSource": "/CaseRatingProfile/DisplaySymptomsItemCapitaAmount?Department=" + department,
        "bServerSide": true,
        "bDestroy": true,
        "bRetrieve": true,
        "bAutoWidth": true,
        "bProcessing": true,
        "bSort": false,
        "aLengthMenu": [[20, 50, 100, 100000], [20, 50, 100, "All"]],
        "iDisplayLength": 20,
        "aoColumns": [
            { "bVisible": false },
            {
                "bSearchable": false,
                "fnRender": function (oObj) {
                    var ln = '<input type="checkbox" name="chkItem2" onclick="status_checkbox2(this);" value="' + oObj.aData[0] + '">';
                    return ln;
                }
            },
            {},
            {}
        ]
    });
}
function LoadFeatures(allFeatures) {
    var lan = getLanguage();
    $("#selected-context-items").dataTable({
        "oLanguage": lan,
        "bJQueryUI": true,
        "sAjaxSource": "/CaseRatingProfile/DisplayFeatures?Features=" + allFeatures,
        "bServerSide": true,
        "bDestroy": true,
        "bRetrieve": true,
        "bAutoWidth": true,
        "bSearch": false,
        "bProcessing": true,
        "bSort": false,
        "aLengthMenu": [[10, 50, 100, 100000], [10, 50, 100, "All"]],
        "aoColumns": [
            { "bVisible": false },
            {},
            {},
            {},
            {},
            {
                "fnRender": function (oObj) {
                    var ln='';
                    if(oObj.aData[5].toLowerCase()=='labtest')
                        ln='检验';
                    else if(oObj.aData[5].toLowerCase()=='adversedrugevent')
                        ln='症状';
                    return ln;
                }
            },
            {
                "fnRender": function (oObj) {
                    var FeatureId = oObj.aData[0];
                    var FeatureName = oObj.aData[1];
                    var type = oObj.aData[5];
                    var ln = '<img src="../Content/images/trash.png" style="cursor:pointer; onclick="removeFeature(\'' + FeatureId + '\',\'' + FeatureName + '\',\''+type+'\')">' + '</img>';
                    return ln;
                }
            }
        ],
        "fnDrawCallback": function () {
            $.unblockUI();
        }
    });
}

function removeFeature(ItemId,ItemName,type) {
    $("#selected-context-items tbody td:contains(" + ItemName + ")").parent().remove();
    var ids_array = ids.split(',');
    for (var i = 0; i < ids_array.length; i++) {
        if (ids_array[i] == ItemId)
            ids_array.splice(i, 1);
    }
    ids = '';
    for (var i = 0; i < ids_array.length; i++) 
        ids += ids_array[i].trim() + ',';
}
