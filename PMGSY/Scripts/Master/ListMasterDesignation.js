$(document).ready(function () {


    if (!$("#desigSearchDetails").is(":visible")) {

        $('#desigSearchDetails').load('/Master/SearchDesignation', function () {
            loadGrid();
        });
        $('#desigSearchDetails').show('slow');
        $("#btnAdd").show();
        $("#btnSearch").hide();
    }

    $.validator.unobtrusive.parse('#frmAddDesignation');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });


    $("#btnAddNew").click(function (e) {
        if (!$("#desigDetails").is(":visible")) {
            $('#desigDetails').show();
            $('#desigDetails').load("/Master/AddDesignation");
        }
    });
     $('#btnAdd').click(function (e) {
        if ($("#desigSearchDetails").is(":visible")) {
            $('#desigSearchDetails').hide('slow');
        }

        if (!$("#desigAddDetails").is(":visible")) {
            $('#desigAddDetails').load("/Master/AddDesignation");
            $('#desigAddDetails').show('slow');

            $('#btnAdd').hide();
            $('#btnSearch').show();
        }


    });

    $('#btnSearch').click(function (e) {

        if ($("#desigAddDetails").is(":visible")) {
            $('#desigAddDetails').hide('slow');
        
            $('#btnSearch').hide();
            $('#btnAdd').show();

        }

        if (!$("#desigSearchDetails").is(":visible")) {

            $('#desigSearchDetails').load('/Master/SearchDesignation', function () {

                $('#desigCategory').trigger('reloadGrid');

                var data = $('#desigCategory').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {

                    $('#Designation').val(data.desigCode);
                }
                $('#desigSearchDetails').show('slow');
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


function FormatColumn(cellvalue, options, rowObject) {


    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit Designation Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete Designation Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}



function editData(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
    $.ajax({
        type: 'GET',
        url: '/Master/EditDesignation/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#desigSearchDetails").is(":visible")) {
                $('#desigSearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();
 
            $("#desigAddDetails").html(data);
            $("#desigAddDetails").show();
            $("#MAST_DESIG_NAME").focus();
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
        }
    })
}

function deleteData(urlparameter) {
    if (confirm("Are you sure you want to delete Designation details?")) {
        $.ajax({
            type: 'POST',
            url: '/Master/DeleteDesignation/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {
                    alert(data.message);
                    //$("#desigCategory").trigger('reloadGrid');
                    //$('#desigDetails').show();
                    //$('#desigDetails').load("/Master/AddDesignation");
                    if ($("#desigAddDetails").is(":visible")) {
                        $('#desigAddDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }

                    if (!$("#desigSearchDetails").is(":visible")) {
                        $("#desigSearchDetails").show('slow');
                    }
                    $("#desigCategory").trigger('reloadGrid');
                }
                else {
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
function ClearDetails() {
    $('#MAST_DESIG_NAME').val('');
    $('#MAST_DESIG_CODE').val('');
    $('#MAST_DESIG_TYPE').val('');
    $('#dvErrorMessage').hide('slow');
    $('#message').html('');
}

function loadGrid() {

    jQuery("#desigCategory").jqGrid({
        url: '/Master/GetDesignationList',
        datatype: "json",
        mtype: "POST",
        postData: { desigCode: $('#Designation').val() },
        colNames: ['Designation Name','Designation Type', 'Action'],
        colModel: [
                            { name: 'MAST_DESIG_NAME', index: 'MAST_DESIG_NAME', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'MAST_DESIG_TYPE', index: 'MAST_DESIG_TYPE', height: 'auto', width: 200, align: "left", sortable: true },
                            { name: 'a', width: 150, sortable: false, resize: false, formatter: FormatColumn, align: "center", sortable: false }
        ],
        pager: jQuery('#pager'),
        rowNum: 15,      
        rowList: [10, 15, 20, 30],
        viewrecords: true,
        recordtext: '{2} records found',    
        sortname: 'MAST_DESIG_NAME',
        sortorder: "asc",
        caption: "Designation List",
        height: 'auto',
        autowidth:true,
        rownumbers: true,
        hidegrid: false,        
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

}