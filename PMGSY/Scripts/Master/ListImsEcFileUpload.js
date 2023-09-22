$(document).ready(function () {
    if (!$("#ImsEcFileUploadSearchDetails").is(":visible")) {

        $('#ImsEcFileUploadSearchDetails').load('/Master/SearchImsEcFileUpload');
        $('#ImsEcFileUploadSearchDetails').show('slow');

        $("#btnSearch").hide();
    }

    $.validator.unobtrusive.parse('#frmAddImsEcFileUpload');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnAdd').click(function (e) {
        if ($("#ImsEcFileUploadSearchDetails").is(":visible")) {
            $('#ImsEcFileUploadSearchDetails').hide('slow');
        }

        $('#ImsEcFileUploadAddDetails').load("/Master/AddEditImsEcFileUpload");
        $('#ImsEcFileUploadAddDetails').show('slow');

        $('#btnAdd').hide();
        $('#btnSearch').show();
        setTimeout(function () {
            $("#ddlState").val($('#ddlStateSerach option:selected').val());
            $("#ddlState").trigger('change');
            $("#ddlPhaseYear").val($('#ddlPhaseYearSerach option:selected').val());
            $("#ddlBatch").val($('#ddlBatchSerach option:selected').val());
            setTimeout(function () {
                $("#ddlAgency").val($('#ddlAgencySerach option:selected').val());
            }, 1000);
        }, 500);
    });

    $('#btnSearch').click(function (e) {

        if ($("#ImsEcFileUploadAddDetails").is(":visible")) {
            $('#ImsEcFileUploadAddDetails').hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#ImsEcFileUploadSearchDetails").is(":visible")) {

            $('#ImsEcFileUploadSearchDetails').load('/Master/SearchImsEcFileUpload', function () {
                var data = $('#tblImsEcFileUpload').jqGrid("getGridParam", "postData");                
                if (!(data === undefined)) {                   
                    $('#ddlStateSerach').val(data.stateCode);
                    $('#ddlAgencySerach').val(data.agency);
                    $('#ddlPhaseYearSerach').val(data.year);
                    $('#ddlBatchSerach').val(data.batch)


                }
                $('#ImsEcFileUploadSearchDetails').show('slow');
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

    //loadGrid();



});





function LoadImsEcFileUploadGrid() {
    if ($("#frmSearchImsEcFileUpload").valid()) {
        jQuery("#tblImsEcFileUpload").jqGrid({
            url: '/Master/GetImsEcFileUploadList',
            datatype: "json",
            mtype: "POST",
            colNames: ['State', 'Year', 'Batch', 'Agency', 'File Type', 'File Name', 'File Path', 'Download', 'Action'],
            colModel: [

                        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "left", sortable: true },
                        { name: 'Year', index: 'Year', height: 'auto', width: 50, align: "center", sortable: true },
                        { name: 'Batch', index: 'Batch', height: 'auto', width: 60, align: "center", sortable: true },
                        { name: 'Agency', index: 'Agency', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'IMS_FILE_TYPE', index: 'IMS_FILE_TYPE', height: 'auto', width: 100, align: "center", sortable: true },
                        { name: 'IMS_FILE_NAME', index: 'IMS_FILE_NAME', height: 'auto', width: 100, align: "left", sortable: false, hidden: true },
                        { name: 'IMS_FILE_PATH', index: 'IMS_FILE_PATH', height: 'auto', width: 100, align: "left", sortable: false, hidden: true },
                        { name: 'ViewUpload', index: 'ViewUpload', height: 'auto', width: 100, align: "center", sortable: false, hidden: false },
                        { name: 'ActionImsEcFileUpload', width: 60, resize: false, formatter: FormatColumn_ImsEcFileUpload, align: "center" }
            ],
            postData: { stateCode: $('#ddlStateSerach option:selected').val(), agency: $('#ddlAgencySerach option:selected').val(), year: $('#ddlPhaseYearSerach option:selected').val(), batch: $('#ddlBatchSerach option:selected').val() },
            pager: jQuery('#divImsEcFileUploadpager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_STATE_NAME',
            sortorder: "asc",
            caption: "File Upload List",
            height: 'auto',
            autowidth: true,
            //width:'250',
            //shrinkToFit: false,
            rownumbers: true,
            loadComplete: function () {
                //$("#tblImsEcFileUpload").jqGrid('setGridWidth', $("#ImsEcFileUploadList").width(), true);



            },
            loadError: function (xhr, ststus, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    alert("Invalid data.Please check and Try again!")

                }
            },



        });
    }



}




function FormatColumn_ImsEcFileUpload(cellvalue, options, rowObject) {
    // return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit  Check List Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete  Check List Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
    return "<center><table><tr><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete File Upload Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}


function editData(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Master/EditImsEcFileUpload/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#ImsEcFileUploadSearchDetails").is(":visible")) {
                $('#ImsEcFileUploadSearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();

            $("#ImsEcFileUploadAddDetails").html(data);
            $("#ImsEcFileUploadAddDetails").show();
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
            url: '/Master/DeleteImsEcFileUpload/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    //if ($("#ImsEcFileUploadSearchDetails").is(":visible")) {
                    //    $('#btnImsEcFileUploadSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tblImsEcFileUpload').trigger('reloadGrid');
                    //}
                    //$("#ImsEcFileUploadAddDetails").load("/Master/AddEditImsEcFileUpload");

                    if ($("#ImsEcFileUploadAddDetails").is(":visible")) {
                        $('#ImsEcFileUploadAddDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }

                    if (!$("#ImsEcFileUploadSearchDetails").is(":visible")) {
                        $("#ImsEcFileUploadSearchDetails").show('slow');
                        $('#tblImsEcFileUpload').trigger('reloadGrid');
                    }
                    else {
                        $('#tblImsEcFileUpload').trigger('reloadGrid');
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

        if (!$("#ImsEcFileUploadAddDetails").is(':visible')) {
            $('#btnImsEcFileUploadSearch').trigger('click');
            $('#ImsEcFileUploadSearchDetails').show();
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
            if ($("#ImsEcFileUploadSearchDetails").is(":visible")) {
                $('#ImsEcFileUploadSearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();

            $("#ImsEcFileUploadAddDetails").html(data);
            $("#ImsEcFileUploadAddDetails").show();
            $("#ADMIN_ND_NAME").focus();
            $('#trAddNewSearch').show();

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function DownLoadECFile(paramurl) {

    url = "/Master/DownloadECFile/" + paramurl,

    //window.location = url;


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

    //window.location = paramurl;
}


