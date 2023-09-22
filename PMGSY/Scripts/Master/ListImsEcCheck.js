

$(document).ready(function () {
    if ($('#IMS_EC_TYPE_HD').val() == "P") {
        $('#spandivhdAddEditEC').html("Pre EC Check List Details");
        $('#spnListAddEC').html("Add Pre EC Check List");
        $('#btnAdd').attr("title", "Add Pre EC Check List");
        $('#spnListSearchEC').html("Search Pre EC Check List");
        $('#btnSearch').attr("title", "Search Pre EC Check List");
        $('#btnImsEcCheckSearch').attr("title", "Search Pre EC Check List");


    } else {
        $('#spandivhdAddEditEC').html("EC Check List Details");
        $('#spnListAddEC').html("Add EC Check List");
        $('#btnAdd').attr("title", "Add EC Check List");
        $('#spnListSearchEC').html("Search EC Check List");
        $('#btnSearch').attr("title", "Search EC Check List");
        $('#btnImsEcCheckSearch').attr("title", "Search EC Check List");
    }
    if ($('#RoleCode').val() == 25) {
        $('#btnAdd').hide();
    }
    if (!$("#ImsEcCheckSearchDetails").is(":visible")) {

        $('#ImsEcCheckSearchDetails').load('/Master/SearchImsEcCheck');
        $('#ImsEcCheckSearchDetails').show('slow');

        $("#btnSearch").hide();
    }

    $.validator.unobtrusive.parse('#frmAddImsEcCheck');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnAdd').click(function (e) {
        if ($("#ImsEcCheckSearchDetails").is(":visible")) {
            $('#ImsEcCheckSearchDetails').hide('slow');
        }

        $('#ImsEcCheckAddDetails').load("/Master/AddEditImsEcCheck");
        $('#ImsEcCheckAddDetails').show('slow');

        $('#btnAdd').hide();
        $('#btnSearch').show();

    });

    $('#btnSearch').click(function (e) {

        if ($("#ImsEcCheckAddDetails").is(":visible")) {
            $('#ImsEcCheckAddDetails').hide('slow');

            $('#btnSearch').hide();

            $('#btnAdd').show();
            if ($('#RoleCode').val() == 25) {
                $('#btnAdd').hide();
            }
        }

        if (!$("#ImsEcCheckSearchDetails").is(":visible")) {

            $('#ImsEcCheckSearchDetails').load('/Master/SearchImsEcCheck', function () {
                var data = $('#tblImsEcCheck').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {

                    $('#ddlStateSerach').val(data.stateCode);
                    $('#ddlAgencySerach').val(data.agency);
                    $('#ddlPhaseYearSerach').val(data.year);
                    $('#ddlBatchSerach').val(data.batch)


                }
                $('#ImsEcCheckSearchDetails').show('slow');
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





function LoadImsEcCheckGrid() {
    if ($("#frmSearchImsEcCheck").valid()) {
        jQuery("#tblImsEcCheck").jqGrid({
            url: '/Master/GetImsEcCheckList',
            datatype: "json",
            mtype: "POST",
            colNames: ['State', 'Agency', 'Year', 'Batch', 'CN Ready ', 'DDRP OMMS ', 'CNCP/CUP ', 'DP ', 'SLSC ', 'DPRs Scrutinized ',
                      'PCI Register ', 'Unsealed Surface ', 'MP Data ', 'Maint. Year ', 'Estimate SSR ',
                      'SSR Date ', 'DPR STA ', 'Document Uploded ', 'Work Capacity ', 'IPAI Account ',
                      'Border/LWE ', 'Batch Size ', 'ECop ', 'TE/PIC ', 'Action', 'View', 'PDF', 'Definalize'],
            colModel: [

                        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 80, align: "left", sortable: true },
                        { name: 'Agency', index: 'Agency', height: 'auto', width: 100, align: "left", sortable: true },
                        { name: 'Year', index: 'Year', height: 'auto', width: 50, align: "center", sortable: true },
                        { name: 'Batch', index: 'Batch', height: 'auto', width: 60, align: "center", sortable: true },
                        { name: 'IMS_CN_READY', index: 'IMS_CN_READY', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_DRRP_OMMAS', index: 'IMS_DRRP_OMMAS', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_CNCUPL_READY', index: 'IMS_CNCUPL_READY', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_DP_APPROVAL', index: 'IMS_DP_APPROVAL', height: 'auto', width: 40, align: "center", sortable: true, hidden: true },
                        { name: 'IMS_SLSC_PROCEEDING', index: 'IMS_SLSC_PROCEEDING', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_DPR_SCRUTINY', index: 'IMS_DPR_SCRUTINY', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_PCI_REGISTER', index: 'IMS_PCI_REGISTER', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_UNSEALED', index: 'IMS_UNSEALED', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_MP_DATA', index: 'IMS_MP_DATA', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_MAINT_YEARWISE', index: 'IMS_MAINT_YEARWISE', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_ESTIMATE_SSR', index: 'IMS_ESTIMATE_SSR', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_SSR_DATE', index: 'IMS_SSR_DATE', height: 'auto', width: 60, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_DPR_STA', index: 'IMS_DPR_STA', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_NIT_UPLOADED', index: 'IMS_NIT_UPLOADED', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_WORK_CAPACITY', index: 'IMS_WORK_CAPACITY', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_IPAI_ACCOUNTS', index: 'IMS_IPAI_ACCOUNTS', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_LWE_MHA', index: 'IMS_LWE_MHA', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_WB_BATCH_SIZE', index: 'IMS_WB_BATCH_SIZE', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_WB_ECOP', index: 'IMS_WB_ECOP', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        { name: 'IMS_WB_STA_CLEARED', index: 'IMS_WB_STA_CLEARED', height: 'auto', width: 40, align: "center", sortable: false, hidden: true },
                        //{ name: 'ActionImsEcCheck', width: 60, resize: false, formatter: FormatColumn_ImsEcCheck, align: "center" },
                        { name: 'ActionImsEcCheck', width: 60, resize: false, align: "center" },
                        { name: 'ViewIMSECCheck', width: 30, resize: false, align: "center" },
                        { name: 'ViewPDFIMSECCheck', width: 30, resize: false, align: "center" },
                        { name: 'DefinalizeECCheck', width: 30, resize: false, align: "center" }
            ],
            postData: { stateCode: $('#ddlStateSerach option:selected').val(), agency: $('#ddlAgencySerach option:selected').val(), year: $('#ddlPhaseYearSerach option:selected').val(), batch: $('#ddlBatchSerach option:selected').val(), EcType: $('#IMS_EC_TYPE_HD').val() },
            pager: jQuery('#divImsEcCheckpager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_STATE_NAME',
            sortorder: "asc",
            caption: "" + ($('#IMS_EC_TYPE_HD').val() == "P" ? "Pre EC Check List" : "EC Check List") + "",
            height: 'auto',
            autowidth: true,
            //width:'250',
            //shrinkToFit: false,
            rownumbers: true,
            loadComplete: function () {

                //$("#tblImsEcCheck").jqGrid('setGridWidth', $("#ImsEcCheckList").width(), true);
                var recordCount = jQuery('#tblImsEcCheck').jqGrid('getGridParam', 'reccount');
                if (recordCount > 0) {
                    //RoleCope= 2 SRRDA
                    if ($('#RoleCode').val() == 2) {
                        var button = '<input type="button" id="btnFinalizeECCheck" name="btnFinalizeECCheck" value="Finalize" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" title="Finalize Check List" tabindex="200" style="font-size:1em; margin-left:25px" onclick="FinalizeECCheck()" />'
                        $('#divImsEcCheckpager_left').html(button);
                    }

                }


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

function DefinalizeECCheckList(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'POST',
        url: '/Master/DeFinalizeEC/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        //data: { EcCode: urlparameter },
        success: function (data) {
            alert('Definalized successfully!!');
            $('#tblImsEcCheck').trigger('reloadGrid');
            //jQuery("#tblImsEcCheck").jqGrid('reload');
            //if ($("#ImsEcCheckSearchDetails").is(":visible")) {
            //    $('#ImsEcCheckSearchDetails').hide('slow');
            //}
            //$('#btnAdd').hide();
            //$('#btnSearch').show();
            //$("#ImsEcCheckAddDetails").html("");
            //$("#ImsEcCheckAddDetails").html(data);
            //$("#ImsEcCheckAddDetails").show();
            //$("#ADMIN_ND_NAME").focus();
            //$('#trAddNewSearch').show();

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}


function FormatColumn_ImsEcCheck(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit  Check List Details' onClick ='EditIMSECCheckList(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete  Check List Details' onClick ='DeleteIMSECCheckList(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}

function EditIMSECCheckList(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Master/EditImsEcCheck/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#ImsEcCheckSearchDetails").is(":visible")) {
                $('#ImsEcCheckSearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();
            $("#ImsEcCheckAddDetails").html("");
            $("#ImsEcCheckAddDetails").html(data);
            $("#ImsEcCheckAddDetails").show();
            $("#ADMIN_ND_NAME").focus();
            $('#trAddNewSearch').show();

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function DeleteIMSECCheckList(urlparameter) {
    if (confirm("Are you sure you want to delete Check List details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Master/DeleteImsEcCheck/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    //if ($("#ImsEcCheckSearchDetails").is(":visible")) {
                    //    $('#btnImsEcCheckSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tblImsEcCheck').trigger('reloadGrid');
                    //}
                    //$("#ImsEcCheckAddDetails").load("/Master/AddEditImsEcCheck");
                    if ($("#ImsEcCheckAddDetails").is(":visible")) {
                        $('#ImsEcCheckAddDetails').hide('slow');

                        $('#btnSearch').hide();

                        $('#btnAdd').show();
                        if ($('#RoleCode').val() == 25) {
                            $('#btnAdd').hide();
                        }
                    }
                    if (!$("#ImsEcCheckSearchDetails").is(":visible")) {
                        $('#ImsEcCheckSearchDetails').show('slow');
                        $('#tblImsEcCheck').trigger('reloadGrid');
                    }
                    else {
                        $('#tblImsEcCheck').trigger('reloadGrid');
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

        if (!$("#ImsEcCheckAddDetails").is(':visible')) {
            $('#btnImsEcCheckSearch').trigger('click');
            $('#ImsEcCheckSearchDetails').show();
            $('#trAddNewSearch').show();
        }

    }
    else {
        return false;
    }

}

function ViewCheckList(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Master/ViewImsEcCheck/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#ImsEcCheckSearchDetails").is(":visible")) {
                $('#ImsEcCheckSearchDetails').hide('slow');
            }

            $('#btnAdd').hide();
            $('#btnSearch').show();
            $("#ImsEcCheckAddDetails").html("");
            $("#ImsEcCheckAddDetails").html(data);
            $("#ImsEcCheckAddDetails").show();
            $("#ADMIN_ND_NAME").focus();
            $('#trAddNewSearch').show();

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function FinalizeECCheck() {

    var id = $('#tblImsEcCheck').jqGrid('getGridParam', 'selrow');
    if (id != null) {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        //blockPage();
        $.ajax({
            type: 'POST',
            url: '/Master/FinalizeECCheckList/' + id,
            async: false,
            cache: false,
            success: function (data) {
                if (data.success == true) {
                    alert(data.message);
                    $("#tblImsEcCheck").trigger('reloadGrid');
                    $.unblockUI();
                    //unblockPage();
                }
                else if (data.success == false) {
                    alert(data.message);
                    $.unblockUI();
                    //unblockPage();
                }
            },
            error: function () { }
        });
    }
    else {
        alert('Please select check list to finalize.');
    }

}

function DownLoadViewECCheckListPDFFile(paramurl) {

    url = "/Master/DownloadViewECCheckList/" + paramurl,

    //window.location = url;


    $.ajax({
        url: url,
        aysnc: false,
        catche: false,
        error: function (xhr, status, msg) {
            alert("An Error occured while processing your request.");
            return false;
        },
        success: function (response) {

            if (response.success == "false") {
                alert(response.message);
                return false;
            }
            else {
                window.location = url;
            }
        }
    });

    //window.location = paramurl;
}

