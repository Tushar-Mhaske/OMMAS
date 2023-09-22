
$(document).ready(function () {
 $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });
 $.ajax({
     url: "/Master/SearchAdminAutonomousBody/",
     type: "GET",
     dataType: "html",
     success: function (data) {
         $("#dvSearchAutonomousBody").html(data);
     },
     error: function (xhr, ajaxOptions, thrownError) {

         alert(xhr.responseText);
     }

 });

    $('#btnCreateNew').click(function () {


        if ($("#dvSearchAutonomousBody").is(":visible")) {
            $('#dvSearchAutonomousBody').hide('slow');
        }

        //if (!$("#dvAgencyDetails").is(":visible")) {

        //    $("#dvAgencyDetails").load("/Master/AddEditMasterAgency/");

        //    $('#dvAgencyDetails').show('slow');

        //    $('#btnAddAgency').hide();
        //    $('#btnSearchView').show();

        //}
        if (!$('#dvAutonomousBodyDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#dvAutonomousBodyDetails").load("/Master/AddEditMasterAdminAutonomousBody", function () {
                $("#btnCreateNew").hide('slow');
                $('#dvAutonomousBodyDetails').show('slow');
                $('#btnSearchView').show();
                $.unblockUI();
            });
        }
    });

    $('#btnSearchView').click(function (e) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvAutonomousBodyDetails").is(":visible")) {
            $('#dvAutonomousBodyDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnCreateNew').show();

        }
        if (!$("#dvSearchAutonomousBody").is(":visible")) {

            $('#dvSearchAutonomousBody').load('/Master/SearchAdminAutonomousBody/', function () {

                $('#tblAutonomousBodyList').trigger('reloadGrid');

                var data = $('#tblAutonomousBodyList').jqGrid("getGridParam", "postData");
                if (!(data === undefined)) {

                    $('#ddlSearchStates').val(data.stateCode);
                }
                $('#dvSearchAutonomousBody').show('slow');

            });
        }
        $.unblockUI();
    });

   
});


function LoadAutonomousGrid() {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $("#tblAutonomousBodyList").jqGrid('GridUnload');
    $('#tblAutonomousBodyList').jqGrid({
        url: '/Master/GetMasterAdminAutonomousBodyList/',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Autonomous Body', 'State', 'Action'],
        colModel: [
         { name: 'ADMIN_AUTONOMOUS_BODY1', index: 'ADMIN_AUTONOMOUS_BODY1', height: 'auto', width: 600, align: "left", sortable: true },
         { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 170, align: "left", sortable: true },
         { name: 'a', width: 80, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        postData: { stateCode: $('#ddlSearchStates option:selected').val() },
        pager: jQuery('#divPagerAutonomousBody'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_STATE_NAME,ADMIN_AUTONOMOUS_BODY1',
        sortorder: "asc",
        caption: 'Autonomous Body List',
        height: '100%',
        autowidth: true,
        rownumbers: true,
        hidegrid: false,

        loadComplete: function () {
            $.unblockUI();
        },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again");
            }
            $.unblockUI();
        }

    });


}

function FormatColumn(cellvalue, options, rowObject) {

    if ($('#RoleCode').val() == 36) {

        return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Autonomous Body Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-locked ui-align-center'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-pencil' title='Edit Autonomous Body Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer;'><span class='ui-icon ui-icon-trash' title='Delete Autonomous Body Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    }

}


function editData(id) {
    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        url: "/Master/EditMasterAdminAutonomousBody/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche:false,
        contentType: "application/json; charset=utf-8",       
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            
            $("#dvAutonomousBodyDetails").html(data);
            $("#dvAutonomousBodyDetails").show();
            $("#ADMIN_AUTONOMOUS_BODY1").focus();          

            if ($("#dvSearchAutonomousBody").is(":visible")) {
                $('#dvSearchAutonomousBody').hide('slow');
            }
            $('#btnCreateNew').hide();
            $('#btnSearchView').show();

            $.unblockUI();
          
        },
        error: function (xht, ajaxOptions, throwError)
        {
            alert(xht.responseText);
            $.unblockUI();
        }
    });
}

function deleteData(urlParam)
{  
    if (confirm("Are you sure you want to delete Autonomous Body details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            url: "/Master/DeleteMasterAutonomousBody/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {

                if (data.success) {
                    alert(data.message);
                    //$("#dvAutonomousBodyDetails").load("/Master/AddEditMasterAdminAutonomousBody");

                    //$("#tblAutonomousBodyList").trigger('reloadGrid');
                    if ($("#dvAutonomousBodyDetails").is(":visible")) {
                        $('#dvAutonomousBodyDetails').hide('slow');

                        $('#btnSearchView').hide();
                        $('#btnCreateNew').show();

                    }
                    if (!$("#dvSearchAutonomousBody").is(":visible")) {
                        $("#dvSearchAutonomousBody").show('slow');
                        $("#tblAutonomousBodyList").trigger('reloadGrid');
                    }
                    else {
                        $("#tblAutonomousBodyList").trigger('reloadGrid');
                    }

                }
                else {
                    alert(data.message);
                }
                $.unblockUI();
            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); }
        });
    }
    else {
        return false;
    }
}
