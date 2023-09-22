$(document).ready(function () {
    if (!$("#ImsEcTrainingSearchDetails").is(":visible")) {

        $('#ImsEcTrainingSearchDetails').load('/Master/SearchImsEcTraining');
        $('#ImsEcTrainingSearchDetails').show('slow');

        $("#btnSearch").hide();
    }

    $.validator.unobtrusive.parse('#frmAddImsEcTraining');

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $('#btnAdd').click(function (e) {
        if ($("#ImsEcTrainingSearchDetails").is(":visible")) {
            $('#ImsEcTrainingSearchDetails').hide('slow');
        }

        $('#ImsEcTrainingAddDetails').load("/Master/AddEditImsEcTraining");
        $('#ImsEcTrainingAddDetails').show('slow');

        $('#btnAdd').hide();
        $('#btnSearch').show();

    });

    $('#btnSearch').click(function (e) {

        if ($("#ImsEcTrainingAddDetails").is(":visible")) {
            $('#ImsEcTrainingAddDetails').hide('slow');
            $('#btnSearch').hide();
            $('#btnAdd').show();
        }

        if (!$("#ImsEcTrainingSearchDetails").is(":visible")) {

            $('#ImsEcTrainingSearchDetails').load('/Master/SearchImsEcTraining', function () {
                var data = $('#tblImsEcTraining').jqGrid("getGridParam", "postData");

                if (!(data === undefined)) {

                    $('#ddlStateSerach').val(data.stateCode);
                    $('#ddlDesignationSerach').val(data.agency);
                    $('#ddlPhaseYearSerach').val(data.year);     


                }
                $('#ImsEcTrainingSearchDetails').show('slow');
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





function LoadImsEcTrainingGrid() {
    if ($("#frmSearchImsEcTraining").valid()) {
        jQuery("#tblImsEcTraining").jqGrid({
            url: '/Master/GetImsEcTrainingList',
            datatype: "json",
            mtype: "POST",
            colNames: ['State', 'Year', 'Designation','Total Person','Action'],
            colModel: [
                        { name: 'MAST_STATE_NAME', index: 'MAST_STATE_NAME', height: 'auto', width: 100, align: "left", sortable: true },
                        { name: 'Year', index: 'Year', height: 'auto', width: 50, align: "center", sortable: false },
                        { name: 'Designation', index: 'Designation', height: 'auto', width: 100, align: "center", sortable: false },
                        { name: 'TotalPerson', index: 'TotalPerson', height: 'auto', width: 100, align: "center", sortable: false },
                        { name: 'ActionImsEcTraining', width: 70, resize: false, formatter: FormatColumn_ImsEcTraining, align: "center" }
            ],
            postData: { stateCode: $('#ddlStateSerach option:selected').val(), designation: $('#ddlDesignationSerach option:selected').val(), year: $('#ddlPhaseYearSerach option:selected').val()},
            pager: jQuery('#divImsEcTrainingpager'),
            rowNum: 10,
            rowList: [10, 15, 20, 30],
            viewrecords: true,
            recordtext: '{2} records found',
            sortname: 'MAST_STATE_NAME',
            sortorder: "asc",
            caption: "Training List",
            height: 'auto',
            autowidth: true,
            //width:'250',
           // shrinkToFit: false,
            rownumbers: true,
            loadComplete: function () {
                //$("#tblImsEcTraining").jqGrid('setGridWidth', $("#ImsEcTrainingList").width(), true);



            },
            loadError: function (xhr, ststus, error) {

                if (xhr.responseText == "session expired") {
                    alert(xhr.responseText);
                    window.location.href = "/Login/Login";
                }
                else {
                    alert("Invalid data.Please Training and Try again!")

                }
            },



        });
    }



}




function FormatColumn_ImsEcTraining(cellvalue, options, rowObject) {
    return "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-pencil' title='Edit  Training Details' onClick ='editData(\"" + cellvalue.toString() + "\");'></span></td><td style='border:none;cursor:pointer'><span class='ui-icon ui-icon-trash' title='Delete  Training  Details' onClick ='deleteData(\"" + cellvalue.toString() + "\");'></span></td></tr></table></center>";
}


function editData(urlparameter) {
    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

    $.ajax({
        type: 'GET',
        url: '/Master/EditImsEcTraining/' + urlparameter,
        dataType: "html",
        async: false,
        cache: false,
        success: function (data) {
            if ($("#ImsEcTrainingSearchDetails").is(":visible")) {
                $('#ImsEcTrainingSearchDetails').hide('slow');
            }
            $('#btnAdd').hide();
            $('#btnSearch').show();

            $("#ImsEcTrainingAddDetails").html(data);
            $("#ImsEcTrainingAddDetails").show();
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
    if (confirm("Are you sure you want to delete Training  details?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'POST',
            url: '/Master/DeleteImsEcTraining/' + urlparameter,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data.success) {

                    alert(data.message);
                    //if ($("#ImsEcTrainingSearchDetails").is(":visible")) {
                    //    $('#btnImsEcTrainingSearch').trigger('click');

                    //}
                    //else {
                    //    $('#tblImsEcTraining').trigger('reloadGrid');
                    //}
                    //$("#ImsEcTrainingAddDetails").load("/Master/AddEditImsEcTraining");

                    if ($("#ImsEcTrainingAddDetails").is(":visible")) {
                        $('#ImsEcTrainingAddDetails').hide('slow');
                        $('#btnSearch').hide();
                        $('#btnAdd').show();
                    }
                    if (!$("#ImsEcTrainingSearchDetails").is(":visible")) {
                        $("#ImsEcTrainingSearchDetails").show('slow');
                        $('#tblImsEcTraining').trigger('reloadGrid');
                    }
                    else {
                        $('#tblImsEcTraining').trigger('reloadGrid');
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

        if (!$("#ImsEcTrainingAddDetails").is(':visible')) {
            $('#btnImsEcTrainingSearch').trigger('click');
            $('#ImsEcTrainingSearchDetails').show();
            $('#trAddNewSearch').show();
        }

    }
    else {
        return false;
    }

}


