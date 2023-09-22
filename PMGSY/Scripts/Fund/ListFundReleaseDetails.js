$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmSearchFundRelease');


    $(function () {
        $("#accordion").accordion({
            //fillSpace: true,
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    //$("#tabs").tabs();

    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#ddlStateSearch").val($("#State").val());

    $("#btnListFundReleaseDetails").click(function (e) {

        blockPage();
        //$('#tbFundReleaseList').jqGrid('GridUnload');
        searchReleaseDetails();
        unblockPage();
    });

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#searchForm").toggle("slow");
    });

    $("#btnAddFundDetails").click(function () {

        var stateCode = $('#ddlStateSearch option:selected').val();
        if (stateCode === undefined) {
            stateCode = "0";
        }
        var fundType = $('#ddlFundTypeSearch option:selected').val();

        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Fund Release Details</a>" +
                '<a href="#" style="float: right;">' +
                '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseFundReleaseDetails();" /></a>'
                );
        $('#accordion').show('fold', function () {
            blockPage();

            $("#divAddFundReleaseForm").load("/Fund/AddEditFundRelease?stateCode=" + stateCode + "&fundType=" + fundType, function () {
                $.validator.unobtrusive.parse($('#divAddFundReleaseForm'));
                unblockPage();
            });
            $('#divAddFundReleaseForm').show('slow');
            $("#divAddFundReleaseForm").css('height', 'auto');

        });

        $("#tbFundReleaseList").jqGrid('setGridState', 'hidden');
        $("#tbFundReleaseList").setGridParam('hidegrid', false);
        $('#idFilterDiv').trigger('click');


    });

    LoadFundAllocationDetails();
});
function CloseFundReleaseDetails() {
    $('#accordion').hide('slow');
    $('#divAddFundReleaseForm').hide('slow');
    $("#tbFundReleaseList").jqGrid('setGridState', 'visible');
    showFilter();
}

function searchReleaseDetails() {

    $('#tbFundReleaseList').setGridParam({ url: '/Fund/GetFundReleaseList', datatype: 'json' });

    $('#tbFundReleaseList').jqGrid("setGridParam", { "postData": { stateCode: $("#ddlStateSearch option:selected").val(), fundType: $("#ddlFundTypeSearch option:selected").val(), yearCode: $("#ddlYearSearch option:selected").val(), fundingAgencyCode: $("#ddlFundingAgencySearch").val(), ReleaseBy: $("#ddlReleaser option:selected").val() } });

    $('#tbFundReleaseList').trigger("reloadGrid", [{ page: 1 }]);
}

function showFilter() {

    if ($('#searchForm').is(":hidden")) {
        $("#searchForm").show('slow');
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function CloseProposalDetails() {
    $('#accordion').hide('slow');
    $('#divAddFundReleaseForm').hide('slow');
    $("#tbFundReleaseList").jqGrid('setGridState', 'visible');

    showFilter();
}

function FormatColumn(cellvalue, options, rowObject) {

    if (cellvalue.toString() == "") {
        return "<center><table><tr><td  style='border-color:white;cursor:pointer'><span class='ui-icon ui-icon-locked ui-align-center' title='Delete Fund Release Details';'></span></td></tr></table></center>";
    }
    else {
        return "<center><table><tr><td  style='border-color:white;cursor:pointer'><span  class='ui-icon ui-icon-thrash' title='Delete Fund Release Details' onClick ='DeleteFundReleaseDetails(\"" + cellvalue.toString() + "\");'>Habitation</span></td></tr></table></center>";
    }
}


function LoadFundAllocationDetails() {

    jQuery("#tbFundReleaseList").jqGrid({
        url: '/Fund/GetFundReleaseList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), fundType: $("#ddlFundTypeSearch option:selected").val(), yearCode: $("#ddlYearSearch option:selected").val(), fundingAgencyCode: $("#ddlFundingAgencySearch").val(), ReleaseBy: $("#ddlReleaser option:selected").val() },
        colNames: ['Phase','Release No.','Released By','Release Year', 'Executing Agency', 'Collaboration','Release Amount[Rs. in Lakhs.]', 'Release Date', 'Sanction Order No.','Upload','Edit','Delete'],
        colModel: [
                            { name: 'MAST_YEAR', index: 'MAST_YEAR', height: 'auto', width: 100, align: "center" },
                            { name: 'MAST_TRANSACTION_NO', index: 'MAST_TRANSACTION_NO', width: 70, align: "center" },
                            { name: 'MAST_RELEASE_TYPE', index: 'MAST_RELEASE_TYPE', width: 70, align: "center",hidden:true },
                            { name: 'MAST_RELEASE_YEAR', index: 'MAST_RELEASE_YEAR', height: 'auto', width: 80, align: "center" },
                            { name: 'ADMIN_ND_NAME', index: 'ADMIN_ND_NAME', height: 'auto', width: 200, align: "left" },
                            { name: 'MAST_FUNDING_AGENCY_NAME', index: 'MAST_FUNDING_AGENCY_NAME', width: 100, sortable: true, align: "left" },
                            { name: 'MAST_RELEASE_AMOUNT', index: 'MAST_RELEASE_AMOUNT', width: 90, align: "center" },
                            { name: 'MAST_RELEASE_DATE', index: 'MAST_RELEASE_DATE', width: 100, align: "center" },
                            { name: 'MAST_RELEASE_ORDER', index: 'MAST_RELEASE_ORDER', width: 80, align: "center" },
                            { name: 'u', width: 50, sortable: false, resize: false,align: "center" },
                            //{ name: 'd', width: 55, sortable: false, resize: false, formatter: AnchorFormatter, align: "center" },
                            { name: 'e', width: 50, sortable: false, resize: false, align: "center" },
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center" },
                            //{ name: 'a', width: 50, sortable: false, resize: false, formatter: FormatColumn, align: "center" },

        ],
        pager: jQuery('#pagerFundRelease').width(20),
        rowNum: 10,
        //altRows: true,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_YEAR' + ',' + 'MAST_TRANSACTION_NO',
        sortorder: "asc",
        cmTemplate: { title: false },
        caption: "&nbsp;&nbsp; Fund Release List",
        height: 'auto',
        autowidth: true,
        hidegrid: true,

        rownumbers: true,
        loadComplete: function () {

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        }
    });
}

function DeleteFundReleaseDetails(urlparameter) {

    if (confirm("Are you sure you want to delete Fund Release Details?")) {
        $.ajax({
            type: 'POST',
            url: '/Fund/DeleteFundRelease/'+urlparameter,
            //data:{urlparameter:urlparameter},
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert("Fund Release details deleted successfully. ");
                    $("#tbFundReleaseList").trigger('reloadGrid');
                }
                else {
                    alert("Fund Allocation details can't be deleted");
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.responseText);
            }
        });
    }
    else {
        return false;
    }
}
function EditFundReleaseDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Edit Fund Release Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        //$("#divAddForm").load('/CoreNetwork/DetailsCoreNetwork?id=' + urlparameter, function () {
        $("#divAddFundReleaseForm").load('/Fund/EditFundRelease/'+urlparameter, function () {
            $.validator.unobtrusive.parse($('#divAddFundReleaseForm'));
            unblockPage();
        });
        $('#divAddFundReleaseForm').show('slow');
        $("#divAddFundReleaseForm").css('height', 'auto');
    });

    $("#tbFundReleaseList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');


}

function AnchorFormatter(cellvalue, options, rowObject) {

    var url = "/Fund/DownloadFundReleaseFile/" + cellvalue;

         //return "<a href='#' onclick=DownloadFile('" + cellvalue+  "'); return false;> <img height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";

    if (cellvalue != "") {
        return "<a href='#' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onclick=downloadFileFromAction('" + url + "'); return false;></a>";
    }
    else {
        return "<span>-</span>"
    }
   
}

function FillInCascadeDropdown(map, dropdown, action) {
    var message = '';

    if (dropdown == '#ddlFundingAgency') {
        message = '<h4><label style="font-weight:normal"> Loading Agencies... </label></h4>';
    }

    $(dropdown).empty();
    blockPage();
    $.post(action, map, function (data) {
        $.each(data, function () {

            if (this.Selected == true)
            {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else
            {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
            
        });
        $("#ddlExecutingAgency").val($("#ADMIN_NO_CODE").val());
    }, "json");
    unblockPage();
}

function UploadFundReleaseFiles(urlParameter) {

    $("#UrlParameter").val(urlParameter);

    jQuery('#tbFundAllocationList').jqGrid('setSelection', urlParameter);

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Upload File</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {

        $("#divAddFundReleaseForm").load('/Fund/FileUploadFundRelease/' + urlParameter, function (e) {
            $.validator.unobtrusive.parse($('#divAddFundReleaseForm'));
            unblockPage();
        });
        $('#divAddFundReleaseForm').show('slow');
        $("#divAddFundReleaseForm").css('height', 'auto');
    });

    $("#tbFundReleaseList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');
}

function DownloadFundReleaseFiles(urlparameter) {

    $.ajax({
        url: '/Fund/DownloadFundReleaseFile/'+urlparameter,
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        success: function (response) {
            unblockPage();
            if (response.Success) {

            }
            else {

            }
        },
        error: function (xhr, AjaxOptions, thrownError) {
            alert(xhr.responseText);
            unblockPage();
        }
    });


}
function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}