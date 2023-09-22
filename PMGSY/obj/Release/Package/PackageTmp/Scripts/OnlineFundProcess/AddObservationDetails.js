$(document).ready(function () {

    $('.trCondition').hide('slow');

    //Multiselect for selecting different level groups
    //---------------------------------------------------
    $("#ConditionList").multiselect({
        minWidth: 150,
        position: {
            my: 'left bottom',
            at: 'left top'
        }
    });

    $("#ConditionList").multiselect("uncheckAll");
    //---------------------------------------------------

    $(function () {
        $("#accordion").accordion({
            icons: false,
            heightStyle: "content",
            autoHeight: false
        });
    });

    //$('#btnAddObservation').click(function () {

    //    if ($('#frmAddObservation').valid()) {

    //        $.ajax({

    //            type: 'POST',
    //            url: '/OnlineFund/AddObservationDetails',
    //            async: false,
    //            cache: false,
    //            data: $('#frmAddObservation').serialize(),
    //            success: function (data) {
    //                if (data.Success == true) {
    //                    alert('Observation details saved successfully.');
    //                    //$("#tblObservationDetails").trigger('reloadGrid');
    //                    AddObservationDetails($("#REQUEST_ID").val());
    //                }
    //                else {
    //                    $('#message').html(data.ErrorMessage);
    //                    $('#dvErrorMessage').show('slow');
    //                }
    //            },
    //            error: function () {
    //                alert('Error occurred while processing your request.');
    //            }

    //        });
    //    }
    //    else {
    //        return false;
    //    }
    //});


    $("#frmAddObservation").on('submit', function (event) {
        if ($('#rdConditionYes').is(':checked')) {
            if (!validateCondition()) {
                alert(validateCondition());
                return false;
            }
        }

        if ($('#frmAddObservation').valid()) {
            event.stopPropagation(); // Stop stuff happening call double avoid to action
            event.preventDefault(); // call double avoid to action

            var form_data = new FormData();

            $.each($("input[type='file']"), function () {

                var id = $(this).attr('id');
                var objFiles = $("input#" + id).prop("files");
                form_data.append(id, (objFiles[0]));
            });

            var data = $("#frmAddObservation").serializeArray();

            for (var i = 0; i < data.length; i++) {
                form_data.append(data[i].name, data[i].value);
            }

            $.ajax({

                type: 'POST',
                url: '/OnlineFund/AddObservationDetails/',
                data: form_data,
                cache: false,
                processData: false,
                contentType: false,
                success: function (data) {
                    if (data.Success == true) {
                        alert('Observation details saved successfully.');
                        AddObservationDetails($("#REQUEST_ID").val());
                    }
                    else if (data.Success == false) {
                        $('#message').html(data.ErrorMessage);
                        $('#dvErrorMessage').show('slow');
                    }
                    else {
                        alert(data.ErrorMessage);
                    }
                },
                error: function () { }

            });
        }

    });

    $('#rdRejectRequest').click(function () {
        $('.trForwardRequest').hide();
        $('#trRejectLetter').show('slow');
    });

    $('#rdApproveRequest').click(function () {
        $('#trRejectLetter').hide('slow');
    });

    $('#rdForwardRequest').click(function () {
        $('.trForwardRequest').show();
        $('#trRejectLetter').hide('slow');
    });

    $('#rdConditionNo').click(function () {
        $('.trCondition').hide();
    });

    $('#rdConditionYes').click(function () {
        $('.trCondition').show();
    });

    LoadObservationDetails();

});

function LoadObservationDetails() {
    $("#tblObservationDetails").jqGrid('GridUnload');

    jQuery("#tblObservationDetails").jqGrid({
        url: '/OnlineFund/GetListofObservationDetails',
        datatype: "json",
        mtype: "POST",
        postData: { RequestId: $("#REQUEST_ID").val() },
        colNames: ['Request Forwarded From', 'Request Forwarded To', 'File Number', 'Approval Date', 'Remarks', 'Approve / Approve & Forward', 'Reject', 'Forward', 'Reject Letter Name', 'Download'],
        colModel: [

                            { name: 'User', index: 'User', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'UserTo', index: 'UserTo', height: 'auto', width: 100, align: "left", search: false },
                            { name: 'FileNo', index: 'FileNo', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'ApprovalDate', index: 'ApprovalDate', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Remarks', index: 'Remarks', height: 'auto', width: 150, align: "center", search: false },
                            { name: 'Approve', index: 'Approve', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Reject', index: 'Reject', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Forward', index: 'Forward', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'LetterName', index: 'LetterName', height: 'auto', width: 100, align: "center", search: false },
                            { name: 'Download', index: 'Download', height: 'auto', width: 100, align: "center", search: false },

        ],
        pager: jQuery('#pgObservationDetails'),
        rowNum: 10,
        viewrecords: true,
        recordtext: '{2} records found',
        sortname: 'DocumentName',
        sortorder: "desc",
        caption: 'Observation List',
        height: 'auto',
        hidegrid: true,
        rownumbers: true,
        autowidth: false,
        shrinkToFit: false,
        cmTemplate: { title: false },
        grouping: false,
        loadComplete: function (data) {
            $("#tblObservationDetails").jqGrid('setGridWidth', $("#tblObservationDetails").width() - 150, true);
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
function DownloadRejectLetter(id) {
    window.location = '/OnlineFund/DownloadRejectLetterFile/' + id;
}

function validateCondition() {
    //alert($('#ConditionList :selected').length);
    //Get all selected values for Group Code
    if ($('#ConditionList :selected').length > 0) {
        //build an array of selected values
        var selectednumbers = [];
        $('#ConditionList :selected').each(function (i, selected) {
            selectednumbers[i] = $(selected).val();

            //append selected values as comma seperated and assign to hidden field
            if (i == 0) {
                $("#ConditionCode").val(selectednumbers[i]);
            }
            else {
                $("#ConditionCode").val($("#ConditionCode").val() + "," + selectednumbers[i]);
            }

            $("#showLevelError").html("");
            $("#showLevelError").removeClass("field-validation-error");
            return true;
        });
    }
    else {
        $("#showLevelError").html("Map at least one of the Levels");
        $("#showLevelError").addClass("field-validation-error");
        return false;
    }
}