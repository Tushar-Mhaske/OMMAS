$(document).ready(function () {
    LoadExecutionGrid();
});

function LoadExecutionGrid() {

    jQuery("#tblTechnicalExpertListDetails").jqGrid('GridUnload');
    jQuery("#tblTechnicalExpertListDetails").jqGrid({
        url: '/QualityMonitoring/LoadTechnicalExpertDetailsGrid',
        datatype: "json",
        async: false,
        cache: false,
        colNames: ['Id', 'Name', 'PAN Number', 'Mobile Number', 'Email', 'Edit', 'Create/View User', 'Deactivate'],
        colModel: [
            { name: 'ID', index: 'ID', height: 'auto', width: 80, align: "left", search: false, hidden: true },
            { name: 'NAME', index: 'NAME', height: 'auto', width: 100, align: "center" },
            { name: 'PAN_NUMBER', index: 'PAN_NUMBER', height: 'auto', width: 70, align: "center" },
            { name: 'MOBILE_NUMBER', index: 'MOBILE_NUMBER', height: 'auto', width: 70, align: "center" },
            { name: 'EMAIL', index: 'EMAIL', height: 'auto', width: 100, align: "center" },
            { name: 'EDIT', index: 'EDIT', height: 'auto', width: 50, align: "center" },
            { name: 'CREATE_USER', index: 'CREATE_USER', height: 'auto', width: 50, align: "center" },
            { name: 'DEACTIVATE', index: 'DEACTIVATE', height: 'auto', width: 50, align: "center" },
        ],
        pager: jQuery('#divPagerTechnicalExpertDetails').width(20),
        rowNum: 20,
        rowList: [20, 30, 40],
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: "ID",
        sortorder: "asc",
        caption: "&nbsp;&nbsp; Technical Expert Details",
        height: 'auto',
        autowidth: true,
        hidegrid: true,
        rownumbers: true,
        cmTemplate: { title: false },
        loadComplete: function (data) {
            $("#tblTechnicalExpertListDetails #divPagerTechnicalExpertDetails").css({ height: '40px' });

            var windowWidth = window.innerWidth;
            var grid = $("#tblTechnicalExpertListDetails");
            grid.setGridWidth(windowWidth - 100);
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

$('#btnCreateNew').click(function () {

    $.ajax({
        url: '/QualityMonitoring/AddNewTechnicalExpertDetails',
        async: false,
        cache: false,
        success: function (data) {

            $('#btnCreateNew').hide('slow');
            $('#divTechnicalExpertList').hide('slow');

            $('#dvAddTechnicalExpertDetails').html(data);
            $('#dvAddTechnicalExpertDetails').show('slow');
        },
        error: function (err) {
            $.unblockUI();
        }
    });
});

$('#btnSave').click(function () {

    var form = $('#formAddNewTechnicalExpert');
    var formadata = new FormData(form.get(0));

    if (confirm("Do you want to save details ?")) {
        if ($('#btnSave').valid()) {
            if ($("#formAddNewTechnicalExpert").valid()) {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $.ajax({
                    url: '/QualityMonitoring/AddTechnicalExpertDetails',
                    type: 'POST',
                    cache: false,
                    async: false,
                    contentType: false,
                    processData: false,
                    beforeSend: function () { },
                    data: formadata,
                    success: function (response) {
                        alert(response.message);
                        $.unblockUI();
                        if (response.success) {
                            $("#formAddNewTechnicalExpert")[0].reset();
                            $('#btnBack').trigger('click');
                        }
                    },
                    error: function () {
                        $.unblockUI();
                        alert("An Error");
                        return false;
                    },
                });
            }
        }
    }
});

$('#btnBack').click(function () {

    $('#btnCreateNew').show('slow');
    $('#divTechnicalExpertList').show('slow');

    $('#dvAddTechnicalExpertDetails').hide('slow');

    LoadExecutionGrid();
});

function EditTechnicalExpertDetails(technicalExpertId) {
    $.ajax({
        url: '/QualityMonitoring/EditTechnicalExpertDetails',
        async: false,
        cache: false,
        data: { technicalExpertId: technicalExpertId },
        success: function (data) {

            $('#btnCreateNew').hide('slow');
            $('#divTechnicalExpertList').hide('slow');

            $('#dvAddTechnicalExpertDetails').html(data);
            $('#dvAddTechnicalExpertDetails').show('slow');
        },
        error: function (err) {
            $.unblockUI();
        }
    });
}



$('#btnUpdate').click(function () {

    var form = $('#formAddNewTechnicalExpert');
    var formadata = new FormData(form.get(0));

    if (confirm("Do you want to edit details ?")) {
        if ($('#btnUpdate').valid()) {
            if ($("#formAddNewTechnicalExpert").valid()) {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                $.ajax({
                    url: '/QualityMonitoring/UpdateTechnicalExpertDetails',
                    type: 'POST',
                    cache: false,
                    async: false,
                    contentType: false,
                    processData: false,
                    beforeSend: function () { },
                    data: formadata,
                    success: function (response) {
                        alert(response.message);
                        $.unblockUI();
                        if (response.success) {
                            $('#btnCreateNew').show('slow');
                            $('#divTechnicalExpertList').show('slow');

                            $('#dvAddTechnicalExpertDetails').hide('slow');

                            LoadExecutionGrid();
                        }
                    },
                    error: function () {
                        $.unblockUI();
                        alert("An Error");
                        return false;
                    },
                });
            }
        }
    }
});

function CreateTechnicalExpertUser(technicalExpertId) {
    $.ajax({
        url: '/QualityMonitoring/CreateTechnicalExpertUser',
        type: 'POST',
        async: false,
        cache: false,
        data: { technicalExpertId: technicalExpertId },
        success: function (response) {
            alert(response.message);

            if (response.Success) {
                $('#btnCreateNew').show('slow');
                $('#divTechnicalExpertList').show('slow');

                $('#dvAddTechnicalExpertDetails').hide('slow');

                LoadExecutionGrid();
            }
        },
        error: function (err) {
            $.unblockUI();
        }
    });
}

function DeactivateTechnicalExpert(technicalExpertId) {

    if (confirm("Do you want to deactivate the technical expert ?")) {

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            url: '/QualityMonitoring/DeactivateTechnicalExpertDetails/?technicalExpertId=' + technicalExpertId,
            type: 'POST',
            cache: false,
            async: false,
            contentType: false,
            processData: false,
            beforeSend: function () { },
            success: function (response) {
                alert(response.message);
                if (response.success) {
                    LoadExecutionGrid();
                    $.unblockUI();
                }
            },
            error: function () {
                $.unblockUI();
                alert("An Error");
                return false;
            },
        });
    }
}