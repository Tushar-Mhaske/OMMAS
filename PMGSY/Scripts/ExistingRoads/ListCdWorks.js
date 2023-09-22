$(document).ready(function () {

    $.validator.unobtrusive.parse("divAddCdWorks");

    //$("#divAddCdWorks").load('/ExistingRoads/CdWorkAddEdit?id=' + $("#EncryptedRoadCode").val(), function () {

    //    $.validator.unobtrusive.parse("divAddCdWorks");

    //});
    LoadAddView();
    $.validator.unobtrusive.parse("divAddCdWorks");
    GetCdWorks($("#ExistingRoadCode").val());

});
function GetCdWorks(MAST_ER_ROAD_CODE) {
    jQuery("#tbCdWorks").jqGrid({
        url: '/ExistingRoads/GetCdWorksList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['CD Works Type', 'CD Works Length', "CD Works Discharge", "CD Works Chainage", "Construction Year", "Rehabilitation Year", "Span", "Carriage Way", "Foot Path", "Edit", "Delete"],
        colModel: [
                    { name: 'CDWorksType', index: 'CDWorksType', width: 160, sortable: true, align: "left" },
                    { name: 'CDWorksLength', index: 'CDWorksLength', width: 100, sortable: true, align: "center" },
                    { name: 'CDWorksDischarge', index: 'CDWorksDischarge', width: 100, sortable: true, align: "center" },
                    { name: 'CDWorksChainage', index: 'CDWorksChainage', width: 100, sortable: true, align: "center" },
                    { name: 'ConstructionYear', index: 'ConstructionYear', width: 100, sortable: true, align: "center" },
                    { name: 'RehabilitationYear', index: 'RehabilitationYear', width: 100, sortable: true, align: "center" },
                    { name: 'Span', index: 'CDWorksChainage', width: 50, sortable: true, align: "center" },
                    { name: 'CarriageWay', index: 'CarriageWay', width: 50, sortable: true, align: "center" },
                    { name: 'FootPath', index: 'FootPath', width: 70, sortable: true, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", formatter: FormatColumnCdWorksEdit },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", formatter: FormatColumnCdWorksDelete }
        ],
        pager: jQuery('#dvCdWorksPager'),
        rowNum: 8,
        postData: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE },
        viewrecords: true,
        sortname: 'CDWorksType',
        sortorder: 'asc',
        recordtext: '{2} records found',
        caption: "CD Works List",
        height: 'auto',
        width: '100%',
        //autowidth: true,
        sutowidth: true,
        rownumbers: true,
        loadError: function (xhr, ststus, error) {
            if (xhr.responseText == "session expired") {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else {
                // alert(xhr.responseText);
                // alert("Invalid data.Please check and Try again!")
                //  window.location.href = "/Login/LogIn";
            }
        },
        loadComplete: function () {
            $("#gview_tbCdWorks > .ui-jqgrid-titlebar").hide();
        }

    });

}


function EditCdWorks(key) {

    $("#divCdWorks").html("");

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divAddCdWorks").load('/ExistingRoads/EditCdWorkDetails/' + key, function () {
            $.validator.unobtrusive.parse($('#frmCdWorks'));

            if ($("#Operation").val() == "U") {
                $("#MAST_CD_LENGTH").focus();
            }
            unblockPage();
        });
    });

}

function DeleteCdWork(key) {

    if (confirm("Are you sure you want to delete the CD Works details ? ")) {

        $.ajax({
            url: "/ExistingRoads/DeleteCDWorksDetails/" + key,
            type: "POST",
            cache: false,
            beforeSend: function () {
                blockPage();
            },
            error: function (xhr, status, error) {
                unblockPage();
                Alert("Request can not be processed at this time,please try after some time!!!");
                return false;
            },
            success: function (response) {
                unblockPage();
                $("#tbCdWorks").trigger('reloadGrid');

                if ($("#Operation").val() == "U") {
                    $("#btnCancel").trigger('click');
                }


                if (response.success) {
                    alert(response.message);
                    $("#tbExistingRoadsList").trigger('reloadGrid');
                    //$("#divAddCdWorks").load('/ExistingRoads/CdWorkAddEdit?id=' + $("#EncryptedRoadCode").val(), function () {

                    //    $.validator.unobtrusive.parse("divAddCdWorks");

                    //});
                    LoadAddView();
                } else {
                    alert(response.message)
                }

            }
        });

    }
    else {
        return;
    }
}
function FormatColumnCdWorksEdit(cellvalue, options, rowObject) {

    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to edit the CD Works Details' onClick ='EditCdWorks(\"" + cellvalue.toString() + "\");'></span></center>";
}

function FormatColumnCdWorksDelete(cellvalue, options, rowObject) {

    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete the CD Works Details' onClick ='DeleteCdWork(\"" + cellvalue.toString() + "\");'></span></center>";
}
function LoadAddView() {
    $.ajax({

        type: 'GET',
        url: '/ExistingRoads/CdWorkAddEdit/',
        data: { id: $("#EncryptedRoadCode").val() },
        error: function () { },
        success: function (data) {
            if (data.success == false) {
                alert(data.message);
            }
            else {
                $("#divAddCdWorks").html(data);
            }

        },


    });
}