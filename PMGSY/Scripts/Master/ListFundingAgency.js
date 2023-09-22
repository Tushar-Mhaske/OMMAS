$(document).ready(function () {

    $.validator.unobtrusive.parse('#frmAddFundingAgency');


    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnCreateNew').click(function () {

        if (!$('#fundDetails').is(':visible')) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');

            $("#fundDetails").load("/Master/AddFundingAgency", function () {
                $("#btnCreateNew").hide('slow');
                $('#fundDetails').show('slow');
                $.unblockUI();
            });
        }
    });

jQuery("#fundCategory").jqGrid({
        url: '/Master/GetFundingAgencyList',
        datatype: "json",
        mtype: "POST",
        colNames: ['Funding Agency Name','Action'],
        colModel: [
                            { name: 'MAST_FUNDING_AGENCY_NAME', index: 'MAST_FUNDING_AGENCY_NAME', height: 'auto', width: 250, align: "left", sortable: true },
                            { name: 'a', width: 150, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false },
        ],
        pager: jQuery('#pager'),
        rowNum: 15,
     
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'FundingAgencyName',
        sortorder: "asc",
        caption: "Funding Agency List",
        height: 'auto',
        
        autowidth:true,
        rownumbers: true,
        hidegrid: false,
        editoptions: { dataInit: function (elem) { $(elem).width(30);}},
        loadComplete: function () {

        },
        loadError: function (xhr, ststus, error) {

            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                alert("Invalid data.Please check and Try again!")
            }
        }

    });

    $("#btnAddNew").click(function (e) {
        if (!$("#fundDetails").is(":visible")) {
            $('#fundDetails').show();
            $('#fundDetails').load("/Master/AddFundingAgency");
        }
    });
});

function FormatColumn(cellvalue, options, rowObject) {


    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Funding Agency Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Funding Agency Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}



function editData(urlparameter) {
    $("#btnCreateNew").hide();
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/EditFundingAgency/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            $('#tblAddNewButton').attr('style', 'padding-top:1.5em; width:100%;');
            $("#fundDetails").show();
            $("#fundDetails").html(data);
            $('#MAST_FUNDING_AGENCY_NAME').focus();
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }

    })
}

function deleteData(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    if (confirm("Are you sure you want to delete Funding Agency details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteFundingAgency/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
               

                if (data.success) {
                    alert(data.message);
                    // $("#fundCategory").trigger('reloadGrid');
                    $("#btnCreateNew").show('slow');
                    $('#fundDetails').hide('slow');
                    $('#fundCategory').trigger('reloadGrid');
                    $.unblockUI();
                }
                else {
                    alert(data.message);
                    $.unblockUI();
                }
            },  
            error: function (xhr, ajaxOptions, thrownError) {
                $.unblockUI();

                alert(xhr.responseText);
            }
        });
    }
    else {
        $.unblockUI();

        return false;
    }

}



























