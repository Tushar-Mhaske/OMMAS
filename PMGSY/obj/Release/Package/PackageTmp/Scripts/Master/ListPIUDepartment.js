$(document).ready(function () {
    if (!$("#PIUSearchDetails").is(":visible")) {

        $('#PIUSearchDetails').load('/Master/SearchPIUDepartment');
        $('#PIUSearchDetails').show('slow');

        $("#btnSearch").hide();
    }

    $.validator.unobtrusive.parse('#frmAddPIU');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnAdd').click(function (e) {
        if ($("#PIUSearchDetails").is(":visible")) {
            $('#PIUSearchDetails').hide('slow');
        }

        $('#PIUAddDetails').load("/Master/AddEditPIUDepartment");
        $('#PIUAddDetails').show('slow');

        $('#btnAdd').hide();
        $('#btnSearch').show();

    });

    $('#btnSearch').click(function (e) {

        if ($("#PIUAddDetails").is(":visible")) {
            $('#PIUAddDetails').hide('slow');

            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#PIUSearchDetails").is(":visible")) {

            $('#PIUSearchDetails').load('/Master/SearchPIUDepartment', function () {
                var data = $('#PIUCategory').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {

                    $('#State').val(data.stateCode);
                    $('#Agency').val(data.agency);
                    $('#ddlSSRDA').val(data.adminNdCode);
                    $('#ddlActive').val(data.active)


                }
                $('#PIUSearchDetails').show('slow');
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





function LoadPIUGrid() {

    jQuery("#PIUCategory").jqGrid({
        url: '/Master/GetPIUDepartmentList',
        datatype: "json",
        mtype: "POST",
        colNames: ['DPIU Name','District', 'TAN No.', 'BA Enabled', 'BA Enabled Date', 'E-Pay Enabled', 'E-Pay Enabled Date', 'E-Remittance Enabled', 'E-Remittance Enabled Date','Active Status','Closing Date', 'Action'],
        colModel: [

                    { name: 'DPIUName', index: 'DPIUName', height: 'auto', width: 150, align: "left", sortable: true },
                    { name: 'Disrtict', index: 'Disrtict', height: 'auto', width: 100, align: "left", sortable: true },
                    { name: 'TANNo', index: 'TANNo', height: 'auto', width: 100, align: "left", sortable: true },
                    { name: 'BAEnabled', index: 'BAEnabled', height: 'auto', width: 80, align: "left", sortable: true },
                    { name: 'BAEnabledDate', index: 'BAEnabledDate', height: 'auto', width: 120, align: "left", sortable: true },
                    { name: 'EPayEnabled', index: 'EPayEnabled', height: 'auto', width: 80, align: "left", sortable: true },
                    { name: 'EPayEnabledDate', index: 'EPayEnabledDate', height: 'auto', width: 120, align: "left", sortable: true },
                    { name: 'ERemittanceEnabled', index: 'ERemittanceEnabled', height: 'auto', width: 100, align: "left", sortable: true },
                    { name: 'ERemittanceEnabledDate', index: 'ERemittanceEnabledDate', height: 'auto', width: 120, align: "left", sortable: true },
                    { name: 'Active', index: 'Active', height: 'auto', width: 50, align: "left", sortable: true },
                    { name: 'CloseDate', index: 'CloseDate', height: 'auto', width: 120, align: "left", sortable: true },
                    { name: 'ActionDPIU', width: 60, resize: false, formatter: FormatColumn_DPIU, align: "center" }
        ],
        postData: { stateCode: $('#State option:selected').val(), agency: $('#Agency option:selected').val(), adminNDCode: $('#ddlSSRDA option:selected').val(), active: $('#ddlActive option:selected').val() },
        pager: jQuery('#pager'),
        rowNum: 15,
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'DPIUName',
        sortorder: "asc",
        caption: "DPIU List",
        height: 'auto',
        autowidth: true,
        rownumbers: true,      
       loadComplete: function () {

            //Commented By Abhishek kamble 20-Feb-2014
            //var SRDARowID = $('#SRDARowID').val();
            //if (SRDARowID != '') {
            //    $("#adminCategory").expandSubGridRow(SRDARowID);
            //}

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




function FormatColumn_DPIU(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit DPIU Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete DPIU Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}


function editData(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
       
    $.ajax({
        type: 'GET',
        url: '/Master/EditPIUDepartment/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#PIUSearchDetails").is(":visible")) {
                $('#PIUSearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();

            $("#PIUAddDetails").html(data);
            $("#PIUAddDetails").show();
            $("#ADMIN_ND_NAME").focus();
            $('#trAddNewSearch').show();
            $('#ddlPARENT_ND_CODE_List').trigger('change');
          
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete SRRDA/DPIU details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });     

        $.ajax({
            type: 'POST',
            url: '/Master/DeletePIUDepartment/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    //if ($("#PIUSearchDetails").is(":visible")) {
                    //     $('#btnPIUSearch').trigger('click');

                    //}
                    //else {
                    //    $('#PIUCategory').trigger('reloadGrid');
                    //}
                    //$("#PIUAddDetails").load("/Master/AddEditPIUDepartment");

                    if ($("#PIUAddDetails").is(":visible")) {
                        $('#PIUAddDetails').hide('slow');

                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                        $('#PIUCategory').trigger('reloadGrid');
                    }

                    if (!$("#PIUSearchDetails").is(":visible")) {
                        $("#PIUSearchDetails").show('slow');
                        $('#PIUCategory').trigger('reloadGrid');
                    }
                    else {
                        $('#PIUCategory').trigger('reloadGrid');
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

        if (!$("#PIUAddDetails").is(':visible')) {
            $('#btnPIUSearch').trigger('click');
            $('#PIUSearchDetails').show();
            $('#trAddNewSearch').show();
        }

    }
    else {
        return false;
    }

}


