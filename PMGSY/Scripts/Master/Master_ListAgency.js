$(document).ready(function () {

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $.ajax({
        url: "/Master/SearchAgencyType/",
        type: "GET",
        dataType: "html",
        success: function (data) {
            $("#dvSearchAgency").html(data);
          },
        error: function (xhr, ajaxOptions, thrownError) {

            alert(xhr.responseText);
        }

    });

     $('#tblMasterAgencyList').jqGrid({
        url: '/Master/GetMasterAgencyList',
        datatype: 'json',
        mtype: "POST",
        colNames: ['Agency Name', 'Agency Type', 'Action'],
        colModel: [
         { name: 'MAST_AGENCY_NAME', index: 'MAST_AGENCY_NAME', height: 'auto', width: 50, align: "left", sortable: true },
         { name: 'MAST_AGENCY_TYPE', index: 'MAST_AGENCY_TYPE', height: 'auto', width: 50, align: "left", sortable: true },
         { name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumn, align: "left", sortable: false }
        ],
        pager: jQuery('#divPagerMasterAgency'),
        rowNum: 15,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_AGENCY_NAME',
        sortorder: "asc",
        caption: 'Agency List',
        height: '100%',
        rownumbers: true,
        hidegrid: false,
        autowidth: true,
        emptyrecords: 'No Records Found',
        loadComplete: function () { },

        loadError: function (xhr, status, error) {

            if (xhr.responseText == "session expired") {

                alert(xht.responseText);
                window.location.href = "Login/login";
            }
            else {
                alert("Invalid Data. Please Check and Try Again");
            }
        }
    });

    $('#btnAddAgency').click(function (e) {

        if ($("#dvSearchAgency").is(":visible")) {
            $('#dvSearchAgency').hide('slow');
        }

        if (!$("#dvAgencyDetails").is(":visible")) {

            $("#dvAgencyDetails").load("/Master/AddEditMasterAgency/");

            $('#dvAgencyDetails').show('slow');

            $('#btnAddAgency').hide();
            $('#btnSearchView').show();

        }

    });

   $('#btnSearchView').click(function (e) {
          $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        if ($("#dvAgencyDetails").is(":visible")) {
            $('#dvAgencyDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnAddAgency').show();

        }

if (!$("#dvSearchAgency").is(":visible")) {

            $('#dvSearchAgency').load('/Master/SearchAgencyType/', function () {

                $('#tblMasterAgencyList').trigger('reloadGrid');

               var data = $('#tblMasterAgencyList').jqGrid("getGridParam", "postData");
                 if (!(data === undefined)) {

                    $('#AgencyType').val(data.AgencyType);
                }
                $('#dvSearchAgency').show('slow');

            });
        }
$.unblockUI();
   });

});

function FormatColumn(cellvalue, options, rowObject) {
    if (cellvalue.toString() == "") {
        //return "<center><table><tr><td style='border:none;'><span>-</span></td><td style='border:none;'><span>-</span></td></tr></table></center>";
        return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-locked');'></span></td><td style='border:none'><span class='ui-icon ui-icon-locked');'></span></td></tr></table></center>";
    }

    return "<center><table><tr><td  style='border:none'><span class='ui-icon ui-icon-pencil' title='Edit Agency Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none'><span class='ui-icon ui-icon-trash' title='Delete Agency Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function editData(id) {

    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        url: "/Master/EditMasterAgency/" + id,
        type: "GET",
        async: false,
        dataType: "html",
        catche: false,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if ($("#dvSearchAgency").is(":visible")) {
                $('#dvSearchAgency').hide('slow');
            }
            $('#btnAddAgency').hide();
            $('#btnSearchView').show();
 
            $("#dvAgencyDetails").html(data);
            $("#dvAgencyDetails").show();
            $("#MAST_AGENCY_NAME").focus();
        },
        error: function (xht, ajaxOptions, throwError) {
            alert(xht.responseText);
          
        }
        
    });
    $.unblockUI();
}

function deleteData(urlParam) {
    if (confirm("Are you sure you want to delete Agency details?")) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: "/Master/DeleteMasterAgency/" + urlParam,
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //$("#tblMasterAgencyList").trigger('reloadGrid');
                    //$("#dvAgencyDetails").load("/Master/AddEditMasterAgency");
                    if ($("#dvAgencyDetails").is(":visible")) {
                        $('#dvAgencyDetails').hide('slow');

                        $('#btnSearchView').hide();
                        $('#btnAddAgency').show();

                    }

                    if (!$("#dvSearchAgency").is(":visible")) {
                        $("#dvSearchAgency").show('slow');
                    }
                    $('#tblMasterAgencyList').trigger('reloadGrid');
                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }
                $("#dvAgencyDetails").load("/Master/AddEditMasterAgency/");

            },
            error: function (xht, ajaxOptions, throwError)
            { alert(xht.responseText); $.unblockUI(); }

        });
    }
    else {
        return false;
    }
}
