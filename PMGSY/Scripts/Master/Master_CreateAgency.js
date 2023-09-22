
$(document).ready(function () {

    $.validator.unobtrusive.parse("#frmMasterAgency");

    $("input[type=text]").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            return false;
        }
    });

    $("#ddlReasonType").append("<option value='0' selected>--select--</option>");
    $('#btnSave').click(function (e) {
    if ($('#frmMasterAgency').valid()) {
        var agencyCode = $("#ddlAgencyType option:selected").val();

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/AddMasterAgency/",
                type: "POST",
          
                data: $("#frmMasterAgency").serialize(),
                success: function (data) {
                    if (data.success==true) {
                        alert(data.message);
                        
                       
                        //$('#tblMasterAgencyList').trigger('reloadGrid');
                        //$("#dvAgencyDetails").load("/Master/AddEditMasterAgency");
                        if ($("#dvAgencyDetails").is(":visible")) {
                            $('#dvAgencyDetails').hide('slow');

                            $('#btnSearchView').hide();
                            $('#btnAddAgency').show();

                        }

                        if (!$("#dvSearchAgency").is(":visible")) {
                            $("#dvSearchAgency").show('slow');
                        }
                        SearchAgencyCreateDetail(agencyCode);
                    }
                    else if (data.success==false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAgencyDetails").html(data);
                    }
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });

        }
    });



    $('#btnUpdate').click(function (e) {

        if ($('#frmMasterAgency').valid()) {
            var agencyCode = $("#ddlAgencyType option:selected").val();
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: "/Master/EditMasterAgency/",
                type: "POST",           
                data: $("#frmMasterAgency").serialize(),
                success: function (data) {

                    if (data.success==true) {
                        alert(data.message);
                        //$('#tblMasterAgencyList').trigger('reloadGrid');
                        //$("#dvAgencyDetails").load("/Master/AddEditMasterAgency");
                        if ($("#dvAgencyDetails").is(":visible")) {
                            $('#dvAgencyDetails').hide('slow');
                            $('#btnSearchView').hide();
                            $('#btnAddAgency').show();
                        }

                        if (!$("#dvSearchAgency").is(":visible")) {
                            $("#dvSearchAgency").show('slow');
                        }
                        SearchAgencyCreateDetail(agencyCode);
                    }
                    else if (data.success == false) {
                        if (data.message != "") {
                            $('#message').html(data.message);
                            $('#dvErrorMessage').show('slow');
                        }
                    }
                    else {
                        $("#dvAgencyDetails").html(data);
                    }

                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $.unblockUI();
                }

            });
        }
    });

    $('#btnCancel').click(function (e) {

        //$.ajax({
        //    url: "/Master/AddEditMasterAgency",
        //    type: "GET",
        //    dataType: "html",
        //    success: function (data) {
        //        $("#dvAgencyDetails").html(data);
        //    },
        //    error: function (xhr, ajaxOptions, thrownError) {
        //        alert(xhr.responseText);
        //    }
        //});
        if ($("#dvAgencyDetails").is(":visible")) {
            $('#dvAgencyDetails').hide('slow');

            $('#btnSearchView').hide();
            $('#btnAddAgency').show();

        }

        if (!$("#dvSearchAgency").is(":visible")) {
            $("#dvSearchAgency").show('slow');
        }
      
    });

    $("#btnReset").click(function () {
      
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');

    });
    $("#dvhdCreateNewAgencyDetails").click(function () {

        if ($("#dvCreateNewAgencyDetails").is(":visible"))
        {

            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-n").addClass("ui-icon-circle-triangle-s");

            $(this).next("#dvCreateNewAgencyDetails").slideToggle(300);
        }

        else
        {
            $("#spCollapseIconCN").removeClass("ui-icon-circle-triangle-s").addClass("ui-icon-circle-triangle-n");

            $(this).next("#dvCreateNewAgencyDetails").slideToggle(300);
        }
    });

    $("#MAST_AGENCY_NAME").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });

    $("#MAST_AGENCY_TYPE").focus(function () {
        $('#dvErrorMessage').hide('slow');
        $('#message').html('');
    });
});

function SearchAgencyCreateDetail(agencyCode) {
    $('#AgencyType').val(agencyCode);
       $('#tblMasterAgencyList').setGridParam({
        url: '/Master/GetMasterAgencyList', datatype: 'json'
    });
    $('#tblMasterAgencyList').jqGrid("setGridParam", { "postData": { AgencyType: agencyCode } });
    $('#tblMasterAgencyList').trigger("reloadGrid", [{ page: 1 }]);
}