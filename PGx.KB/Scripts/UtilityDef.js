//javascript utility

function addSelectOption(combo, value, text, selectedValue) {
    //if (!combo || combo.outerHTML.toLowerCase().startWith("<select") === false)
    //    return;
    var option = document.createElement("option");
    option.value = value;
    option.text = text;
    if (selectedValue && selectedValue.toLowerCase() == value.toLowerCase())
        option.setAttribute("selected", "selected");
    try {
        combo.add(option, null); //Standard    
    } catch (error) {
        //combo.add(option); // IE only     
    }
}

function getGenes() {
    var s = "";
    var genes = $('.geneOption');
    for (var i = 0; i < genes.length; i++) {
        if (genes[i].selected === true)
            s += genes[i].value + ",";
    }
    return s;
}

function getMultiSelectVal(id) {
    debugger;
    var r = getMultiSelectValAsArray(id);
    var s = r.join(",");
    //for (var i = 0; i < r.length; i++) {
    //    s += r[i] + ",";
    //}
    return s;
}

/**
* Return an array of multi select values.
*
* param {string} id Multi select element id.
*
* return {string} An array selected option values.
*/
function getMultiSelectValAsArray(id) {
    debugger;
    var r = [];

    var select = document.getElementById(id);
    if (!select || select.outerHTML.toLowerCase().startWith("<select") === false)
        return r;

    // escape special character for JQuery selector
    debugger;
    r = $('#' + id.replace(/\./g, "\\.")).val();
    if (r) {
        for (var i = 0; i < r.length; i++) {
            // trim
            r[i] = r[i].trim();
        }
        // remove duplicate
        r = r.filter(function (v, i) { return r.indexOf(v) == i; });
        return r;
    }
    else {
        return [];
    }
}

String.prototype.startWith = function (str) {
    if (str === null || str === "" || this.length === 0 || str.length > this.length) {
        return false;
    }
    if (this.substr(0, str.length) == str) {
        return true;
    }
    else {
        return false;
    }
    return true;
};