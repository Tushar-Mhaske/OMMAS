$(document).ready(function () {
    if (!$("#ImsMaintenancePolicySearchDetails").is(":visible")) {

        $('#ImsMaintenancePolicySearchDetails').load('/Master/SearchMaintenancePolicy');
        $('#ImsMaintenancePolicySearchDetails').show('slow');

        $("#btnSearch").hide();
    }

    $.validator.unobtrusive.parse('#frmAddMaintenancePolicy');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnAdd').click(function (e) {
        if ($("#ImsMaintenancePolicySearchDetails").is(":visible")) {
            $('#ImsMaintenancePolicySearchDetails').hide('slow');
        }

        $('#ImsMaintenancePolicyAddDetails').load("/Master/AddEditMaintenancePolicy");
        $('#ImsMaintenancePolicyAddDetails').show('slow');

        $('#btnAdd').hide();
        $('#btnSearch').show();
        //setTimeout(function () {
        //    $("#ddlState").val($('#ddlStateSerach option:selected').val());
        //    $("#ddlState").trigger('change');
        //    setTimeout(function () {
        //        $("#ddlAgency").val($('#ddlAgencySerach option:selected').val());
        //    }, 1000);
        //}, 500);
    });

    $('#btnSearch').click(function (e) {

        if ($("#ImsMaintenancePolicyAddDetails").is(":visible")) {
            $('#ImsMaintenancePolicyAddDetails').hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#ImsMaintenancePolicySearchDetails").is(":visible")) {

            $('#ImsMaintenancePolicySearchDetails').load('/Master/SearchMaintenancePolicy', function () {
                var data = $('#tblMaintenancePolicyUpload').jqGrid("getGridParam", "postData");
                if (!(data === undefined)) {
                    $('#ddlStateSerach').val(data.stateCode);
                    $('#ddlAgencySerach').val(data.agency);
                }
                $('#ImsMaintenancePolicySearchDetails').show('slow');
            });
        }
        $.unblockUI();
    });


    $("#dvhdSearch").click(function () {

        if ($("#dvSearchParameter").is(":visible")) {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }

        else {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvSearchParameter").slideToggle(300);
        }
    });
});

function LoadMaintenancePolicyGrid() {
    if ($("#frmSearchMaintenancePolicy").valid()) {

        jQuery("#tblMaintenancePolicyUpload").jqGrid('GridUnload');

        jQuery("#tblMaintenancePolicyUpload").jqGrid({
            url: '/Master/GetMaintenancePolicyFileUploadList',
            datatype: "json",
            mtype: "POST",
            colNames: ['State', 'Agency', 'Date of Policy', 'PDF', 'MS Word', 'PDF', 'MS Word', 'Delete'],
            colModel: [

                        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "center", sortable: true },
                        { name: 'Agency', index: 'Agency', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'IMS_POLICY_DATE', index: 'IMS_POLICY_DATE', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'H_PDF', index: 'H_PDF', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'H_MSWORD', index: 'H_MSWORD', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'E_PDF', index: 'E_PDF', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'E_MSWORD', index: 'E_MSWORD', height: 'auto', width: 100, align: "center", sortable: true },
                        //{ name: 'IMS_FILE_TYPE', index: 'IMS_FILE_TYPE', height: 'auto', width: 100, align: "center", sortable: true },
                        //{ name: 'IMS_FILE_NAME', index: 'IMS_FILE_NAME', height: 'auto', width: 100, align: "left", sortable: false, hidden: true },
                        //{ name: 'IMS_FILE_PATH', index: 'IMS_FILE_PATH', height: 'auto', width: 100, align: "left", sortable: false, hidden: true },
                        //{ name: 'ViewUpload', index: 'ViewUpload', height: 'auto', width: 100, align: "center", sortable: false, hidden: false },
                        { name: 'ActionImsEcFileUpload', width: 60, resize: false, formatter: FormatColumn_ImsEcFileUpload, align: "center" }
            ],
            postData: { stateCode: $('#ddlStateSerach option:selected').val(), agency: $('#ddlAgencySerach option:selected').val() },
            pager: jQuery('#divMaintenancePolicypager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_STATE_NAME',
            sortorder: "asc",
            caption: "File Upload List",
            height: 'auto',
            autowidth: true,
            rownumbers: true,
            loadComplete: function () {

                if ($('#tblMaintenancePolicyUpload'))
                jQuery("#tblMaintenancePolicyUpload").jqGrid('setGroupHeaders', {
                    useColSpanStyle: false,
                    groupHeaders: [
                      { startColumnName: 'H_PDF', numberOfColumns: 2, titleText: 'Hindi / Regional Language' },
                      { startColumnName: 'E_PDF', numberOfColumns: 2, titleText: 'English' }
                    ]
                });
            },
            loadError: function (xhr, ststus, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    alert("Invalid data.Please check and Try again!");
                }
            },
        });

        
    }


    
}
function FormatColumn_ImsEcFileUpload(cellvalue, options, rowObject) {
    return "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete File Upload Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function editData(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Master/EditMaintenancePolicy/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#ImsMaintenancePolicySearchDetails").is(":visible")) {
                $('#ImsMaintenancePolicySearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();

            $("#ImsMaintenancePolicyAddDetails").html(data);
            $("#ImsMaintenancePolicyAddDetails").show();
            $("#ADMIN_ND_NAME").focus();
            $('#trAddNewSearch').show();

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete File Upload details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Master/DeleteMaintenancePolicyFile/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("Maintenance Policy details deleted successfully.");
                    if ($("#ImsMaintenancePolicyAddDetails").is(":visible")) {
                        $('#ImsMaintenancePolicyAddDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }

                    if (!$("#ImsMaintenancePolicySearchDetails").is(":visible")) {
                        $("#ImsMaintenancePolicySearchDetails").show('slow');
                        LoadMaintenancePolicyGrid();
                    }
                    else {
                        LoadMaintenancePolicyGrid();
                    }
                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
                $.unblockUI();
            }
        });

        if (!$("#ImsMaintenancePolicyAddDetails").is(':visible')) {
            $('#btnMaintenancePolicySearch').trigger('click');
            $('#ImsMaintenancePolicySearchDetails').show();
            $('#trAddNewSearch').show();
        }
    }
    else {
        return false;
    }
}

function UploadECFile(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Master/ImsEcFileUpload/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#ImsMaintenancePolicySearchDetails").is(":visible")) {
                $('#ImsMaintenancePolicySearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();

            $("#ImsMaintenancePolicyAddDetails").html(data);
            $("#ImsMaintenancePolicyAddDetails").show();
            $("#ADMIN_ND_NAME").focus();
            $('#trAddNewSearch').show();

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function DownloadFile(paramurl) {

    url = "/Master/DownloadMaintenancePolicyFile/" + paramurl,

    $.ajax({
        url: url,
        aysnc: false,
        catche: false,
        error: function (xhr, status, msg) {
            alert("An Error occured while processing your request.");
            return false;
        },
        success: function (responce) {

            if (responce.Success == "false") {
                alert("File not available.");
                return false;
            }
            else {
                window.location = url;
            }
        }
    });
}


