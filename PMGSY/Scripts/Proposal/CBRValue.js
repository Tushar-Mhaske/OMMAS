$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmCBRValue'));

    GetCBRValues($("#IMS_PR_ROAD_CODE").val());

    $("#IMS_CBR_VALUE1").width(80);

    if ($("#Operation").val() == "A") {
        $("#rowAdd").show();
        $("#rowUpdate").hide();
    } else {
        $("#rowUpdate").show();
        $("#rowAdd").hide();
    }

    $("#btnReset").click(function () {

        $("#IMS_END_CHAIN").val("");
        $("#Segment_Length").val("");
        $("#IMS_CBR_VALUE1").val("");

    });


    $("#IMS_STR_CHAIN").blur(function() {
        calculateSegmentLenght();
    });

    $("#IMS_END_CHAIN").blur(function () {
        calculateSegmentLenght();
    });

    $('#btnSave').click(function (evt) {
        evt.preventDefault();

        if (ValidateCBRForm()) {

            if ($('#frmCBRValue').valid()) {

            if (calculateSegmentLenght()) {
                $.ajax({
                    url: "/Proposal/CBRValue/",
                    type: "POST",
                    cache: false,
                    data: $("#frmCBRValue").serialize(),
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
                        if (response.Success) {
                            $("#tbCBR").trigger('reloadGrid');

                            $("#frmCBRValue").trigger('reset');                            
                            clearForm();
                            $(".spanRemainingLength").html(response.RemainingLength);
                            $("#Remaining_Length").val(response.RemainingLength);
                            $("#IMS_STR_CHAIN").val(response.Start_Chainage);
                            alert("CBR Details Added Succesfully.");                        
                        }
                        else{
                            alert(response.ErrorMessage) ;
                        }
                    }
                });
            }
        }    
        }
    });

    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();
        if ($('#frmCBRValue').valid()) {
            if (calculateSegmentLenght()) {
                $.ajax({
                    url: "/Proposal/UpdateCBRValue/",
                    type: "POST",
                    cache: false,
                    data: $("#frmCBRValue").serialize(),
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
                        if (response.Success) {
                            $("#tbCBR").trigger('reloadGrid');                                               
                            clearForm();

                            $("#Operation").val("A");

                            $("#rowAdd").show();
                            $("#rowUpdate").hide();

                            alert("CBR Details Updated Succesfully.");
                            
                            $(".spanRemainingLength").html(response.Start_Chainage);
                            $("#Remaining_Length").val(response.RemainingLength);
                            $("#IMS_STR_CHAIN").val(response.Start_Chainage);
                        }
                        else {
                            alert(response.ErrorMessage);
                        }
                    }
                });
            }
        }
    });

});

function ValidateCBRForm()
{
    if (parseFloat($("#Remaining_Length").val()) == 0)
    {
        alert("Remaining Length is 0, CBR Details Can not be Added. ");
        return false;
    }
    else if ( parseFloat($("#Segment_Length").val())  > parseFloat($("#Remaining_Length").val()) ) {
        alert("Segment Length Exceeds the Remaining Length.\nPlease Recheck the Start Chainage and End Chainage.");
        return false;
    }
    return true;
}
    
function clearForm() {
    $("#frmCBRValue").find(':input').each(function () {
        switch (this.type) {
            case 'text':
                $(this).val('');
        }
    });
}

function calculateSegmentLenght() {

    if ($("#IMS_STR_CHAIN").val() != "" && $("#IMS_END_CHAIN").val() != "" && !isNaN($("#IMS_STR_CHAIN").val()) && !isNaN($("#IMS_END_CHAIN").val())) {

        if (parseFloat($("#IMS_STR_CHAIN").val()) >= parseFloat($("#IMS_END_CHAIN").val())) {
            alert("Start Chainage must be less than End Chainage");
            return false;
        }
        else {
            $("#Segment_Length").val(   (parseFloat($("#IMS_END_CHAIN").val()) - parseFloat($("#IMS_STR_CHAIN").val())).toFixed(3)   );
            return true;
        }
    }

}

function EditCBR(IMS_PR_ROAD_CODE, IMS_SEGMENT_NO) {

    $("#divCBRDetails").html("");

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divCBRDetails").load('/Proposal/EditCBRValue/' + IMS_PR_ROAD_CODE + "$" + IMS_SEGMENT_NO, function () {
            $.validator.unobtrusive.parse($('#frmCBRValue'));
            unblockPage();
        });
    });
}

function DeleteCBR(IMS_PR_ROAD_CODE, IMS_SEGMENT_NO) {

    if (confirm("Are you sure to delete the CBR details ? ")) {

        $("#btnAdd").show("slow");
        //$("#tblCBRDetails").hide("slow");
        clearForm();

        $.ajax({
            url: "/Proposal/DeleteCBRValue/",
            type: "POST",
            cache: false,
            data: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, IMS_SEGMENT_NO: IMS_SEGMENT_NO, value: Math.random() },
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
                if (response.Success) {
                    $("#tbCBR").trigger('reloadGrid');
                    $(".spanRemainingLength").html(response.RemainingLength);
                    $("#Remaining_Length").val(response.RemainingLength);
                    $("#IMS_STR_CHAIN").val(response.IMS_STR_CHAIN);                        
                    alert("CBR Details Deleted Succesfully.");
                }
                else {
                    alert(response.ErrorMessage);
                }
            }
        });

    }
}

function GetCBRValues(IMS_PR_ROAD_CODE) {

    jQuery("#tbCBR").jqGrid({
        url: '/Proposal/GetCBRList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Segment Number', 'Start Chainage(in Kms.)', 'End Chainage(in Kms.)', "Segment Length", "CBR Value", "Edit", "Delete"],
        colModel: [
                    { name: 'SegmentNumber', index: 'SegmentNumber', width: 150, sortable: false, align: "center", hidden : true },
                    { name: 'StartChainage', index: 'StartChainage', width: 150, sortable: false, align: "center" },
                    { name: 'EndChainage', index: 'EndChainage', width: 150, sortable: false, align: "center" },
                    { name: 'SegmentLength', index: 'SegmentLength', width: 150, sortable: false, align: "center", formatter: 'number', summaryType: 'sum'},
                    { name: 'CBRValue', index: 'CBRValue', width: 140, sortable: false, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center" },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center" }
        ],
        pager: jQuery('#dvCBRPager'),
        rowList: [08, 10, 12],
        rowNum: 08,
        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        //altRows: true,  
        sortname: 'SegmentNumber',
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "CBR Details",
        height: 'auto',
        width: 'auto',
        rownumbers: true,
        userDataOnFooter: true,
        loadComplete: function () {

            var RoadLengthColumn = $('#tbCBR').jqGrid('getCol', 'SegmentLength', false);          
            var RoadLength = 0;
            for (i = 0 ; i < RoadLengthColumn.length; i++) {
                RoadLength = parseFloat(RoadLength) + parseFloat(RoadLengthColumn[i]);
            }

        },
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
        }
    });

}

function CloseCBRDetails() {
    $('#divCBR').hide('slow');
}