$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmSearchFundAllocation');
    

    $(function () {
        $("#accordion").accordion({
            //fillSpace: true,
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    $("input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#btnListFundDetails").click(function () {
        blockPage();
        searchDetails();
        unblockPage();
    });

    $("#idFilterDiv").click(function () {
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s").toggleClass("ui-icon-circle-triangle-n");
        $("#searchForm").toggle("slow");
    });

    $("#ddlStateSearch").val($("#State").val());

    $("#btnAddFundDetails").click(function () {

        var stateCode = $('#ddlStateSearch option:selected').val();
        var fundType = $('#ddlFundTypeSearch option:selected').val();

        $("#accordion div").html("");
        $("#accordion h3").html(
                "<a href='#' style= 'font-size:.9em;' >Fund Allocation Details</a>" +
                '<a href="#" style="float: right;">' +
                '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseFundAllocationDetails();" /></a>'
                );
        $('#accordion').show('fold', function () {
            blockPage();

            $("#divAddFundAllocationForm").load("/Fund/AddEditFundAllocation?stateCode=" + stateCode+"&fundType="+fundType, function () {
                $.validator.unobtrusive.parse($('#divAddFundAllocationForm'));
                unblockPage();
            });
            $('#divAddFundAllocationForm').show('slow');
            $("#divAddFundAllocationForm").css('height', 'auto');

        });

        $("#tbFundAllocationList").jqGrid('setGridState', 'hidden');
        $("#tbFundAllocationList").setGridParam('hidegrid', false);
        $('#idFilterDiv').trigger('click');

    });

    LoadFundAllocationDetails();
});
function CloseFundAllocationDetails() {
    $('#accordion').hide('slow');
    $('#divAddFundAllocationForm').hide('slow');
    $("#tbFundAllocationList").jqGrid('setGridState', 'visible');
    showFilter();
}

function searchDetails() {

    $('#tbFundAllocationList').setGridParam({
        url: '/Fund/GetFundAllocationList', datatype: 'json'
    });
    $('#tbFundAllocationList').jqGrid("setGridParam", { "postData": { stateCode: $("#ddlStateSearch option:selected").val(), fundType: $("#ddlFundTypeSearch option:selected").val(), yearCode: $("#ddlYearSearch option:selected").val(), fundingAgencyCode: $("#ddlFundingAgencySearch").val() } });
    $('#tbFundAllocationList').trigger("reloadGrid", [{ page: 1 }]);
}

function showFilter() {

    if ($('#searchForm').is(":hidden")) {
        $("#searchForm").show('slow');
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
    }
}

function CloseProposalDetails() {
    $('#accordion').hide('slow');
    $('#divAddFundAllocationForm').hide('slow');
    $("#tbFundAllocationList").jqGrid('setGridState', 'visible');

    showFilter();
}

function FormatColumn(cellvalue, options, rowObject) {

        return "<center><table><tr><td  style='border-color:white;cursor:pointer'><span  class='ui-icon ui-icon-thrash' title='Delete Fund Allocation Details' onClick ='DeleteFundAllocationDetails(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}


function LoadFundAllocationDetails() {

    jQuery("#tbFundAllocationList").jqGrid({
        url: '/Fund/GetFundAllocationList',
        datatype: "json",
        mtype: "POST",
        postData: { stateCode: $("#ddlStateSearch option:selected").val(), fundType: $("#ddlFundTypeSearch option:selected").val(), yearCode: $("#ddlYearSearch option:selected").val(),fundingAgencyCode:$("#ddlFundingAgencySearch").val() },
        colNames: ['Phase','Allocation No.' ,'Executing Agency', 'Collaboration','Allocation Amount [Rs. in Lakhs.]', 'Allocation Date', 'Sanction Order No.','Upload','Edit','Delete'],
        colModel: [
                            { name: 'MAST_YEAR_TEXT', index: 'MAST_YEAR_TEXT', height: 'auto', width: 70, align: "left" },
                            { name: 'MAST_TRANSACTION_NO', index: 'MAST_TRANSACTION_NO', width: 50, sortable: true, align: "center" },
                            { name: 'ADMIN_ND_NAME', index: 'ADMIN_ND_NAME', width: 200, sortable: true, align: "left" },
                            { name: 'MAST_FUNDING_AGENCY_NAME', index: 'MAST_FUNDING_AGENCY_NAME',width: 100, sortable: true, align: "left" },
                            { name: 'MAST_ALLOCATION_AMOUNT', index: 'MAST_ALLOCATION_AMOUNT',  width: 100, align: "center" },
                            { name: 'MAST_ALLOCATION_DATE', index: 'MAST_ALLOCATION_DATE',  width: 100, align: "center" },
                            { name: 'MAST_ALLOCATION_ORDER', index: 'MAST_ALLOCATION_ORDER', width: 70, sortable: true, align: "center" },
                            { name: 'e', width: 50, sortable: false, resize: false, align: "center" },
                            //{ name: 'd', width: 50, sortable: false, resize: false, align: "center",formatter:AnchorFormatter},
                            { name: 'a', width: 50, sortable: false, resize: false, align: "center" },
                            { name: 'u', width: 50, sortable: false, resize: false, align: "center" },

        ],
        pager: jQuery('#pagerFundAllocation').width(20),
        rowNum: 10,
        //altRows: true,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'MAST_YEAR_TEXT'+','+'MAST_TRANSACTION_NO',
        sortorder: "asc",
        cmTemplate:{title:false},
        caption: "&nbsp;&nbsp; Fund Allocation List",
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

function DeleteFundAllocationDetails(urlparameter) {

    if (confirm("Are you sure you want to delete Fund Allocation Details?")) {
        $.ajax({
            type: 'POST',
            url: '/Fund/DeleteFundAllocation/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            data:{urlparameter:urlparameter},
            success: function (data) {
                if (data.success == true) {
                    alert("Fund Allocation details deleted successfully. ");
                    $("#tbFundAllocationList").trigger('reloadGrid');
                    //$("#divAddFundAllocationForm").load('/Fund/EditFundAllocation/' + urlparameter, function () {
                    //    $.validator.unobtrusive.parse($('#divAddFundAllocationForm'));
                    //    unblockPage();
                    //});
                }
                else if (data.success == false)
                {
                    alert(data.message);
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

function EditFundAllocationDetails(urlparameter) {

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Edit Fund Allocation Details</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddFundAllocationForm").load('/Fund/EditFundAllocation/' + urlparameter, function () {
            $.validator.unobtrusive.parse($('#divAddFundAllocationForm'));
            unblockPage();
        });
        $('#divAddFundAllocationForm').show('slow');
        $("#divAddFundAllocationForm").css('height', 'auto');
    });

    $("#tbFundAllocationList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');


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

            if (this.Selected == true) {
                $(dropdown).append("<option selected value=" + this.Value + ">" + this.Text + "</option>");
            }
            else {
                $(dropdown).append("<option value=" + this.Value + ">" + this.Text + "</option>");
            }
        });

    }, "json");
    unblockPage();
}
function UploadFundAllocationFiles(urlParameter) {

    $("#UrlParameter").val(urlParameter);
    jQuery('#tbFundAllocationList').jqGrid('setSelection', urlParameter);

    $("#accordion div").html("");
    $("#accordion h3").html(
            "<a href='#' style= 'font-size:.9em;' >Upload File</a>" +
            '<a href="#" style="float: right;">' +
            '<img src="" class="ui-icon ui-icon-closethick" onclick="CloseProposalDetails();" /></a>'
            );

    $('#accordion').show('fold', function () {

        $("#divAddFundAllocationForm").load('/Fund/FileUpload/' + urlParameter, function (e) {
            $.validator.unobtrusive.parse($('#divAddFundAllocationForm'));
            unblockPage();
        });
        $('#divAddFundAllocationForm').show('slow');
        $("#divAddFundAllocationForm").css('height', 'auto');
    });

    $("#tbFundAllocationList").jqGrid('setGridState', 'hidden');

    $('#idFilterDiv').trigger('click');

}
function DownloadFundReleaseFiles(urlparameter) {

    $.ajax({
        url: '/Fund/DownloadFundReleaseFile/' + urlparameter,
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        //data: { param: urlparameter },
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
function AnchorFormatter(cellvalue, options, rowObject) {

    var url = "/Fund/DownloadFundAllocationFile/" + cellvalue;

    //return "<a href='#' onclick=DownloadFile('" + cellvalue+  "'); return false;> <img height='16' width='16' border=0 src='../../Content/images/PDF.ico' /> </a>";

    if (cellvalue == "") {
        return "<span>-</span>";
    }
    else {
        return "<a href='#' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onclick=downloadFileFromAction('" + url + "'); return false;> </a>";
    }
    

}