/// <reference path="../jquery-1.9.1.js" />

var updatedValues = [];
var flag = new Boolean(0);
var glRowId = 0;
var prevClass = '';
var ctr = 0;
$(document).ready(function () {

    if ($('#PMGSYScheme').val() == 2) {
        if (($('#flg').val() == 'False')) {
            LoadmatrixParametersDetailsList();
        }
        else {
            LoadmatrixParametersWeightageDetailsList();
        }
    }

    $("#tblMatrixParams td input").each(function () {
        $(this).click(function (e) {
            // e.target point to <input> DOM element
            var tr = $(e.target).closest('tr');
            alert("Current rowid=" + tr[0].id);
        });
    });
});

function LoadmatrixParametersDetailsList() {
    $("#tblMatrixParams").jqGrid('GridUnload');
    $('#tblMatrixParams').jqGrid({
        url: '/Proposal/GetMatrixParamDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Parameter Code', 'Parameter', 'Score Type', 'Weightage', 'Matrix Id'/*, 'Edit', 'Delete'*/],
        colModel: [
                      { name: 'MAST_MATRIX_NO', index: 'MAST_MATRIX_NO', height: 'auto', width: 10, align: "center", sortable: false },
                      { name: 'MAST_MATRIX_PARAMETER', index: 'MAST_MATRIX_PARAMETER', height: 'auto', width: 50, align: "left", sortable: false },
                      { name: 'MAST_SCORE_TYPE', index: 'MAST_SCORE_TYPE', height: 'auto', width: 10, align: "center", editable: true, edittype: 'text', sortable: false, hidden: false },
                      { name: 'MAST_MATRIX_WEIGHT', index: 'MAST_MATRIX_WEIGHT', height: 'auto', width: 10, align: "center", sortable: false, },
                      { name: 'MAST_MATRIX_ID', index: 'MAST_MATRIX_ID', height: 'auto', width: 20, align: "center", sortable: true, hidden: true },
        ],
        pager: jQuery('#dvPagerMatrix'),
        rowNum: 40,
        rowList: [15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_MATRIX_NO',
        sortorder: "asc",
        caption: "Matrix Parameters List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadonce: true,
        //onCellSelect: function (rowId, iCol, cellcontent) {
        //    //alert('rowId : ' + rowId)
        //    //alert('cellcontent : ' + $(cellcontent).attr('id'))
        //    glRowId = parseInt(rowId);
        //    //$("#tblMatrixParams").saveRow(rowId);
        //    //$("#tblMatrixParams").jqGrid('saveRow', "rowid", false, 'clientArray');
        //    //$(cellcontent).attr('id').trigger('blur');
        //},
        loadComplete: function () {
            $("#jqgh_tblMatrixParam_rn").html("Sr.<br/> No");

            $('input').on('blur', function () {
                //alert($("#tblMatrixParams").jqGrid('getGridParam', 'selrow'));
                //  alert($(this).parent().next().text());

                var rId = $(this).parent().next().text();
                
                var id = ($(this).attr('id'));
                var cls = $(this).attr('class');
                var val = $(this).attr('value');

                $('#' + id).text($(this).val());

                (function /*Old Logic for Reference*/() {
                    //alert($('#' + id).text());
                    //  alert($('#' + id).val());
                    // setCellVaules(rId, id, cls, val);

                    //$("#tblMatrixParams").jqGrid("setCell", rId, "MAST_MATRIX_WEIGHT", "<input type='textbox' id='" + $(this).attr('id') + "' class='" + $(this).attr('class') + "' onkeypress='return (event.charCode == 8 || event.charCode == 0) ? null : event.charCode >= 48 && event.charCode <= 57' value='" + $(this).attr('value') + "' maxlength='5'>");
                })
            });
            
            (function/*Old validations currently disabled*/() {
                //var i = 0;

                /*$('input').on('change', function () {
                    //alert(document.activeElement);
                    //alert('selrow=' + $("#tblMatrixParams").jqGrid('getGridParam', 'selrow'));
                    //alert($(this).parent().next('td').siblings().val());
                    alert('hii')
                    var rId = $(this).parent().next().text();
                    alert(rId);
                    //$(this).parent().trigger('click');
                    //alert("globl" + glRowId);
                    var parentValue = 0;
                    var childValue = 0;
    
                    var currClass = ($(this).attr('class'));
                    parentValue = parseInt($('#' + currClass + "parent").text());
    
                    //console.log($(this).attr('value'));
                    
                    //myGrid.jqGrid('setCell', rowid, 'myColumn', newValue);
                    $("#tblMatrixParams").jqGrid("setCell", rId, "MAST_MATRIX_WEIGHT", "<input type='textbox' id='" + $(this).attr('id') + "' class='" + $(this).attr('class') + "' onkeypress='return (event.charCode == 8 || event.charCode == 0) ? null : event.charCode >= 48 && event.charCode <= 57' value='" + $(this).attr('value') + "' maxlength='5'>");
    
    
                    //updatedValues.push(parentValue);
                    $("." + currClass).each(function (i, value) {
                        var value1 = $(this).attr('value');
                        //console.log($('input[type="textbox"]').val());
                        childValue = parseInt(childValue) + parseInt(value1);
                    });
                    i = 0;
                    //if (childValue > parentValue) {
                    //    //flag = false;
                    //    //console.log(i);
                    //    if (i == 0) {
                    //        var id = ($(this).attr('id'));
                    //        //$('#' + id).focus();
                    //    }
                    //    //alert('Sum of child items in matrix should be less than or equal to parent item for : ' + currClass);
                    //    i++;
                    //    //setTimeout(function()  { $('#' + id).focus(); }, 100);
                    //    return false;
                    //}
                    //else {
                    //    flag = true;
                    //}
                    //$('#' + $(this).attr('id')).blur();
                });*/
            })

            ///Submit Button 
            $('#dvPagerMatrix_left').html("<input type='button' style='margin-left:27px' id='btnSubmit' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'SubmitMatrixParameters();return false;' value='Submit'/>")
            setTimeout(function () {
                $('input[type="textbox"]').first().focus();
                //$('input[type="textbox"]').trigger('click');
            }, 300);
        },
    });
}

function LoadmatrixParametersWeightageDetailsList() {
    $("#tblMatrixParams").jqGrid('GridUnload');
    $('#tblMatrixParams').jqGrid({
        url: '/Proposal/GetMatrixParamWeightageDetailsList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Matrix Id', 'Parameter Code', 'Parameter', 'Score Type', 'Weightage'/*, 'Edit', 'Delete'*/],
        colModel: [
                      { name: 'MAST_MATRIX_ID', index: 'MAST_MATRIX_ID', height: 'auto', width: 20, align: "center", sortable: true, hidden: true },
                      { name: 'MAST_MATRIX_NO', index: 'MAST_MATRIX_NO', height: 'auto', width: 10, align: "center", sortable: false },
                      { name: 'MAST_MATRIX_PARAMETER', index: 'MAST_MATRIX_PARAMETER', height: 'auto', width: 50, align: "left", sortable: false },
                      { name: 'MAST_SCORE_TYPE', index: 'MAST_SCORE_TYPE', height: 'auto', width: 10, align: "center", editable: true, edittype: 'text', sortable: false, hidden: false },
                      { name: 'MAST_MATRIX_WEIGHT', index: 'MAST_MATRIX_WEIGHT', height: 'auto', width: 10, align: "center", sortable: false },
        ],
        pager: jQuery('#dvPagerMatrix'),
        rowNum: 40,
        rowList: [15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_MATRIX_NO',
        sortorder: "asc",
        caption: "Matrix Parameters List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
    });
}

function SubmitMatrixParameters() {
    var arr = [];
    var myIDs = $('#tblMatrixParams').jqGrid('getDataIDs');

    for (var i = 0; i < myIDs.length; i++) {
        //// DO YOUR STUFF HERE
        var v = $('#tblMatrixParams').jqGrid('getCell', i + 1, 'MAST_MATRIX_WEIGHT');
        var vr = $('#tblMatrixParams').jqGrid('getCell', i + 1, 'MAST_MATRIX_ID');
        var gr = $('#tblMatrixParams').jqGrid('getCell', i + 1, 'MAST_SCORE_TYPE');

        var id = $(v).attr('id');

        var index = $(v).attr('id').indexOf("parent");

        if (index != -1) {
            arr.push(vr + ',parent,' + $(v).text() + ',' + $(v).attr('id').substring(0, 1) + ',' + gr);
            console.log(arr[i]);
        }
        else {
            if ($('#' + id).text() == '') {
                arr.push(vr + ',child,' + $(v).val() + ',' + $(v).attr('class') + ',' + gr);
                console.log(arr[i]);
            }
            else {
                arr.push(vr + ',child,' + $('#' + id).text() + ',' + $(v).attr('class') + ',' + gr);
                console.log(arr[i]);
            }
            //console.log('v=' + v);
            //console.log($('#' + id).text());
        }
    }

    ///Print values for MAST_MATRIX_WEIGHT columns
    //for (var i = 0; i < arr.length; i++) {
    //    console.log(arr[i]);
    //};

    /* for Reference
    //var griddata = $("#tblMatrixParams").jqGrid('getGridParam', 'data');
    //var griddata = $("#tblMatrixParams").jqGrid('getRowData');
    //for (var i = 0; i < griddata.length; i++) {
    //    console.log(griddata[i]);
    //};
    //var dataToSend = JSON.stringify(griddata);
    */

    $.ajax({
        type: 'POST',
        url: '/Proposal/AddMatrixMasterDetails/',
        async: false,
        cache: false,
        data: { MatrixParams: arr, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
        traditional: true,
        success: function (data) {
            arr.splice(0, arr.length); //Clear the preveious value
            //alert(data.success);
            if (data.success) {
                LoadmatrixParametersWeightageDetailsList();
            }
            if (data.success == false) {
                /**/
            }
            alert(data.message);
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            arr.splice(0, arr.length);//Clear the preveious value
            alert(data.message);
            $.unblockUI();
        }
    });
}

//function formatColumnOB(cellvalue, options, rowObject) {
//    return "<input type='textbox' id='" +   + "' class='" + $(this).attr('class') + "' onkeypress='return (event.charCode == 8 || event.charCode == 0) ? null : event.charCode >= 48 && event.charCode <= 57' value='" + cellvalue + "' maxlength='5'>"
//}

function setCellVaules(rId, id, cls, val) {
    //alert(val);
    $("#tblMatrixParams").jqGrid("setCell", rId, "MAST_MATRIX_WEIGHT", val);

    //$("#tblMatrixParams").jqGrid("setCell", rId, "MAST_MATRIX_WEIGHT", "<input type='textbox' id='" + id + "' class='" + cls + "' onkeypress='return (event.charCode == 8 || event.charCode == 0) ? null : event.charCode >= 48 && event.charCode <= 57' value='" + val + "' maxlength='5'>");
    //$("#tblMatrixParams").trigger('loadComplete');
    //    jQuery("#tblMatrixParams").jqGrid('saveRow', rId,
    //{
    //    successfunc: function (response) {
    //        return true;
    //    }
    //});
}


function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
      && (charCode < 48 || charCode > 57))
        return false;
    return true;
}