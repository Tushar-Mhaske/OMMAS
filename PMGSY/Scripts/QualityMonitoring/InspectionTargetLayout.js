$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmQMInspectionTarget'));

     $("#btnView").click(function () {
         if ($('#ddlMonth').val() == 0 ||  $('#ddlYear').val() == 0)
        {
        alert("Please select month and year..");
        return false;
        }

        $.ajax({
            url: "/QualityMonitoring/CheckTargetEntered",
            type: "GET",
            dataType: "html",
            data: { Month: $('#ddlMonth').val(), Year: $('#ddlYear').val() },
            success: function (data) {
                //console.log(JSON.parse(data).flg);
                //$("#flg").val(JSON.parse(data).flg);

                //alert(data);
                //alert(JSON.parse(data).success);
                if (JSON.parse(data).success == false) {
                    InspectionTargetDetailsList();
                }
                else {
                    //InspectionTargetDetailsEnteredList();
                    InspectionTargetDetailsEnteredList();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("Error in Processing,Please try Again");
            }
        });
    });
});


function InspectionTargetDetailsList() {
    $("#tblInspectionTargetGrid").jqGrid('GridUnload');
    $('#tblInspectionTargetGrid').jqGrid({
        url: '/QualityMonitoring/ListInspectionTarget',
        datatype: "json",
        mtype: "POST",
        postData: { month: $('#ddlMonth').val(), year: $('#ddlYear').val() },
        colNames: ['State', 'Month', 'Year', 'Target'],
        colModel: [
                      { name: 'State', index: 'State', height: 'auto', width: 10, align: "center", sortable: false },
                      { name: 'Month', index: 'Month', height: 'auto', width: 10, align: "left", sortable: false },
                      { name: 'Year', index: 'Year', height: 'auto', width: 10, align: "center", editable: true, edittype: 'text', sortable: false, hidden: false },
                      { name: 'Target', index: 'Target', height: 'auto', width: 10, align: "center", sortable: false, },
        ],
        pager: jQuery('#divInspectionTargetPager'),
        rowNum: 40,
        //rowList: [15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_MATRIX_NO',
        sortorder: "asc",
        caption: "Matrix Parameters List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadonce: true,
        loadComplete: function () {
            $("#jqgh_divInspectionTargetPager_rn").html("Sr.<br/> No");

            $('input').on('blur', function () {

                var rId = $(this).parent().next().text();

                var id = ($(this).attr('id'));
                var cls = $(this).attr('class');
                var val = $(this).attr('value');

                $('#' + id).text($(this).val());

            });



            //alert(jQuery("#tblInspectionTargetGrid").jqGrid('getGridParam', 'records'));



            ///Submit Button 
            $('#divInspectionTargetPager_left').html("<input type='button' style='margin-left:27px' id='btnSubmit' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'SubmitTarget();return false;' value='Submit'/>")
            setTimeout(function () {
                $('input[type="textbox"]').first().focus();
                //$('input[type="textbox"]').trigger('click');
            }, 300);
        },
    });
}

function InspectionTargetDetailsEnteredList() {
    $("#tblInspectionTargetGrid").jqGrid('GridUnload');
    $('#tblInspectionTargetGrid').jqGrid({
        url: '/QualityMonitoring/ListInspectionTargetEntered',
        datatype: "json",
        mtype: "POST",
        postData: { month: $('#ddlMonth').val(), year: $('#ddlYear').val() },
        colNames: ['State', 'Month', 'Year', 'Target'],
        colModel: [
                      { name: 'State', index: 'State', height: 'auto', width: 10, align: "center", sortable: false },
                      { name: 'Month', index: 'Month', height: 'auto', width: 10, align: "left", sortable: false },
                      { name: 'Year', index: 'Year', height: 'auto', width: 10, align: "center", editable: true, edittype: 'text', sortable: false, hidden: false },
                      { name: 'Target', index: 'Target', height: 'auto', width: 10, align: "center", sortable: false, },
        ],
        pager: jQuery('#divInspectionTargetPager'),
        rowNum: 40,
        //rowList: [15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_MATRIX_NO',
        sortorder: "asc",
        caption: "Matrix Parameters List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,
        loadonce: true,
        loadComplete: function () {

            //alert(jQuery("#tblInspectionTargetGrid").jqGrid('getGridParam', 'records'));

            ///Delete Button 
            //$('#divInspectionTargetPager_left').html("<input type='button' style='margin-left:27px' id='btnDelete' class='jqueryButton ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only' onClick = 'DeleteTarget();return false;' value='Delete'/>")
        },
    });
}


function SubmitTarget() {
    var arr = [];
    var myIDs = $('#tblInspectionTargetGrid').jqGrid('getDataIDs');

    for (var i = 0; i < myIDs.length; i++) {
        //// DO YOUR STUFF HERE
        var v = $('#tblInspectionTargetGrid').jqGrid('getCell', i + 1, 'State');
        var m = $('#tblInspectionTargetGrid').jqGrid('getCell', i + 1, 'Month');
        var y = $('#tblInspectionTargetGrid').jqGrid('getCell', i + 1, 'Year');
        var tg = $('#tblInspectionTargetGrid').jqGrid('getCell', i + 1, 'Target');

        var s = $(tg).attr('id').substring(1, $(tg).attr('id').length);
        var id = $(tg).attr('id');
        var val = $('#' + id).text() == '' ? 0 : $('#' + id).text();

       
        arr.push(s + ',' + $('#ddlMonth').val() + ',' + y + ',' + val);
        console.log(arr[i]);
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
        url: '/QualityMonitoring/AddInspTargetDetails/',
        async: false,
        cache: false,
        data: { MatrixParams: arr, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
        traditional: true,
        success: function (data) {
            arr.splice(0, arr.length); //Clear the preveious value
            //alert(data.success);
            if (data.success) {
                InspectionTargetDetailsEnteredList();
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

function DeleteTarget() {
    
    $.ajax({
        type: 'POST',
        url: '/QualityMonitoring/DeleteInspTargetDetails/',
        async: false,
        cache: false,
        data: { month: $('#ddlMonth').val(), year: $('#ddlYear').val(), __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
        //traditional: true,
        success: function (data) {
            //arr.splice(0, arr.length); //Clear the preveious value
            //alert(data.success);
            if (data.success) {
                InspectionTargetDetailsList();
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

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
      && (charCode < 48 || charCode > 57))
        return false;
    return true;
}