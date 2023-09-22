$(document).ready(function () {
    $.validator.unobtrusive.parse($('#frmCBRValue'));
    //alert($('#EncryptedCBRCode').val());
    $("#mastEndChain").blur(function () {


        //alert(parseFloat($("#TotalAvailableRoadLength").val()));


        if (parseFloat($("#TotalAvailableRoadLength").val()) == 0) {
            $("#endChainnage").show("slow");
            $("#endChainnage").html("<span style='color:red'><b>Remaining Length is 0, CBR details can not be added.</b></span>");
            return false;
        } else {
            calculateSegmentLenght();
            SegmentLenghtValidation();
        }

        $("#divError").hide("slow");

    });

    //$("input[id$='MAST_END_CHAIN']").blur(function () {
    //    alert($(this).attr('id'));
    //});

    GetCBRValues($("#MAST_ER_ROAD_CODE").val());

    DisplayTotalRemainingRoadLength($("#MAST_ER_ROAD_CODE").val());

    if ($("#Operation").val() == "A") {
        $("#rowAdd").show();
        $("#rowUpdate").hide();

        $(function () {
            $("#mastEndChain").focus();
        });

    } else {
        $("#rowUpdate").show();
        $("#rowAdd").hide();
    }


    $("#btnReset").click(function () {
        $("#spnSegment_Length").html('');
        $("#endChainnage").html('');
        //$("#endChainnage").hide();

        $("#divError").hide("slow");

    });

    //not required
    $("#IMS_STR_CHAIN").blur(function () {
        calculateSegmentLenght();
    });

    $("#mastEndChain").keypress(function (e) {

        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 46 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });


    $("#MAST_CBR_VALUE").keypress(function (e) {

        if (e.which >= 48 && e.which <= 57 || e.which == 8 || e.which == 0) {
        }
        else {
            e.preventDefault();
        }
    });


    $('#btnSave').click(function (evt) {
        evt.preventDefault();

        $("#MAST_END_CHAIN").val(($("#mastEndChain").val()));
        if ($('#frmCBRValue').valid()) {

            if (validateForm() == true) {
                if (calculateSegmentLenght() && SegmentLenghtValidation()) {
                    $.ajax({
                        url: "/ExistingRoads/AddCBRDetailsPmgsy3/",
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
                            if (response.success) {
                                $("#tbCBR").trigger('reloadGrid');
                                $("#frmCBRValue").trigger('reset');

                                DisplayTotalRemainingRoadLength($("#MAST_ER_ROAD_CODE").val());

                                clearForm();

                                alert(response.message);
                            }
                            else {

                                $("#divError").show("slow");
                                $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);
                                //alert(response.message);
                            }
                        }
                    });
                }
            }
        }
    });


    $('#btnUpdate').click(function (evt) {
        evt.preventDefault();
        //alert($('#EncryptedCBRCode').val());
        if ($('#frmCBRValue').valid()) {

            $.ajax({
                url: "/ExistingRoads/EditCBRDetailsPMGSY3/",
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
                    if (response.success == true) {

                        alert("CBR Details Updated Succesfully.");

                    }
                    else {
                        if (response.success == false) {
                            //$("#divError").show();
                            //$("#divError").html(response.message);
                            $("#divError").show("slow");
                            $("#divError span:eq(1)").html('<strong>Alert: </strong>' + response.message);

                        }
                    }
                }
            });

            var urlparamater = $("#MAST_ER_ROAD_CODE").val() + "$" + $("#MAST_SEGMENT_NO").val();
            //alert($('#EncryptedCBRCode').val());
            $('#accordion').show('fold', function () {

                $("#divExistingRoadsForm").load('/ExistingRoads/ListCBRPMGSY3/' + $('#EncryptedCBRCode').val(), function () {
                    $.validator.unobtrusive.parse($('#frmCBRValue'));

                });
                $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
                $('#divExistingRoadsForm').show('slow');
                $("#divExistingRoadsForm").css('height', 'auto');
            });
            unblockPage();

        }
    });


    $('#btnCancel').click(function () {

        //        jQuery('#tbExistingRoadsList').jqGrid('setSelection', urlparamater);

        //alert($('#EncryptedCBRCode').val());
        //var MAST_ER_ROAD_CODE = $("#MAST_ER_ROAD_CODE").val();


        //$("#accordion div").html("");

        //$("#accordion h3").html(
        //        "<a href='#' style= 'font-size:.9em;' >CBR Details</a>" +
        //        '<a href="#" style="float: right;">' +
        //        '<img class="ui-icon ui-icon-closethick" onclick="CloseExistingRoadsDetails();" /></a>'
        //        );

        //alert($('#EncryptedCBRCode').val());
        $('#accordion').show('fold', function () {
            blockPage();

            $("#divExistingRoadsForm").load('/ExistingRoads/ListCBRPMGSY3/' + $('#EncryptedCBRCode').val(), function () {

                $.validator.unobtrusive.parse($('#frmCBRValue'));
                unblockPage();
            });
            $("#tbExistingRoadsList").jqGrid('setGridState', 'hidden');
            $('#divExistingRoadsForm').show('slow');
            $("#divExistingRoadsForm").css('height', 'auto');
        });



    });


});

function clearForm() {

    $("#frmCBRValue").find(':input').each(function () {
        switch (this.type) {
            case 'text':
                $(this).val('');
        }
    });

    $("#spnSegment_Length").html(0);

    $("#divError").hide("slow");

}


function calculateSegmentLenght() {

    if ($("#MAST_STR_CHAIN").val() != "" && $("#mastEndChain").val() != "" && !isNaN($("#MAST_STR_CHAIN").val()) && !isNaN($("#mastEndChain").val())) {

        if (parseFloat($("#MAST_STR_CHAIN").val()) >= parseFloat($("#mastEndChain").val())) {

            $("#endChainnage").show("slow");
            $("#endChainnage").html("<span style='color:red'><b>End Chainage must be greater than start Chainage</b></span>");
            return false;
        }
        else {

            var segLength = (parseFloat($("#mastEndChain").val()) - parseFloat($("#MAST_STR_CHAIN").val())).toFixed(3);
            $("#spnSegment_Length").html(segLength);
            $("#Segment_Length").val(segLength);

            return true;
        }
    }
}

function SegmentLenghtValidation() {

    var segmentLength = parseFloat($("#Segment_Length").val());
    var remainingLength = parseFloat($("#spnTotalAvailableRoadLength").text());


    if (segmentLength > remainingLength) {

        $("#endChainnage").show("slow");
        $("#endChainnage").html("<span style='color:red'><b>Segment length(" + segmentLength + ") exceeds the remaining length(" + remainingLength + "),please check end chainage.</b></span>");
        return false;
    }
    else {
        return true;
    }

}


function EditCBR(key) {

    $("#divCBRDetails").html("");
    //alert($('#EncryptedCBRCode').val());

    $('#accordion').show('fold', function () {
        blockPage();
        $("#divCBRDetails").load('/ExistingRoads/ListCBRPMGSY3/' + key, function () {
            $.validator.unobtrusive.parse($('#frmCBRValue'));
            if ($("#Operation").val() == "U") {
                $("#MAST_CBR_VALUE").focus();
            }

            unblockPage();
        });
    });

    $("#spnStartChainnage").html($("#MAST_STR_CHAIN").val());
    $("#spnEndChainnage").html($("#MAST_END_CHAIN").val());
    $("#spnSegment_Length").html($("#Segment_Length").val());
}

function DeleteCBR(key) {

    if (confirm("Are you sure you want to delete the CBR details ? ")) {

        $.ajax({
            url: "/ExistingRoads/DeleteCBRDetailsPMGSY3/" + key,
            type: "POST",
            cache: false,
            data: { __RequestVerificationToken: $("#frmCBRValue input[name=__RequestVerificationToken]").val() },
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
                if (response.success) {
                    $("#tbCBR").trigger('reloadGrid');
                    if ($("#btnCancel").is(":visible")) {
                        $("#btnCancel").trigger('click');
                    }
                    DisplayTotalRemainingRoadLength($("#MAST_ER_ROAD_CODE").val());
                    alert("CBR Details Deleted Succesfully.");

                }
            }
        });

    }
}


function GetCBRValues(MAST_ER_ROAD_CODE) {

    jQuery("#tbCBR").jqGrid({
        url: '/ExistingRoads/GetCBRListPMGSY3/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Start Chainage(in Kms.)', 'End Chainage(in Kms.)', "Segment Length", "CBR Value", "Edit", "Delete"],
        colModel: [
                    { name: 'StartChainage', index: 'StartChainage', width: '220%', sortable: true, align: "center" },
                    { name: 'EndChainage', index: 'EndChainage', width: '220%', sortable: true, align: "center" },
                    { name: 'SegmentLength', index: 'SegmentLength', width: '220%', sortable: false, align: "center" },//, formatter: 'number', summaryType: 'sum' },
                    { name: 'CBRValue', index: 'CBRValue', width: '195%', sortable: true, align: "center" },
                    { name: 'Edit', index: 'Edit', width: 40, sortable: false, align: "center", formatter: FormatColumnEditCRB },
                    { name: 'Delete', index: 'Delete', width: 40, sortable: false, align: "center", formatter: FormatColumnDeleteCRB }
        ],
        pager: jQuery('#dvCBRPager'),
        rowNum: 10,
        postData: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE },
        //altRows: true,        
        viewrecords: true,
        recordtext: '{2} records found',
        caption: "CBR List",
        sortname: "StartChainage",
        height: 'auto',
        width: '100%',
        rowList: [5, 10, 15, 20],
        rownumbers: true,
        loadComplete: function () {

            var RoadLengthColumn = $('#tbCBR').jqGrid('getCol', 'SegmentLength', false);
            var RoadLength = 0;
            for (i = 0 ; i < RoadLengthColumn.length; i++) {
                RoadLength = parseFloat(RoadLength) + parseFloat(RoadLengthColumn[i]);
            }
            $("#gview_tbCBR > .ui-jqgrid-titlebar").hide();

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
        },

    });

}



function CloseCBRDetails() {
    $('#divCBR').hide('slow');
}



function FormatColumnEditCRB(cellvalue, options, rowObject) {
    return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to edit the CBR Details' onClick ='EditCBR(\"" + cellvalue.toString() + "\");'></span></center>";
}

function FormatColumnDeleteCRB(cellvalue, options, rowObject) {

    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    } else {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-trash ui-align-center' title='Click here to delete the CBR Details' onClick ='DeleteCBR(\"" + cellvalue.toString() + "\");'></span></center>";
    }

}

function DisplayTotalRemainingRoadLength(MAST_ER_ROAD_CODE) {
    //$("#MAST_TI_YEAR").empty();

    $.ajax({
        url: '/ExistingRoads/CBRlengthCalculationPMGSY3/',
        type: 'POST',
        beforeSend: function () {
            blockPage();
        },
        data: { MAST_ER_ROAD_CODE: MAST_ER_ROAD_CODE },
        success: function (jsonData) {

            //set remaining length
            $("#spnTotalAvailableRoadLength").html(jsonData.TotalAvailableLength);
            $("#TotalAvailableRoadLength").val(jsonData.TotalAvailableLength);

            //set start chainage
            if ($("#Operation").val() == "A") {

                $("#MAST_STR_CHAIN").val(jsonData.startChainnage);
                $("#spnStartChainnage").html(jsonData.startChainnage);

            } else { }

            //set entered seg length            
            $(".spanEnteredSegLength").html(jsonData.EnteredSegLength);
            $("#EnteredSegmentLength").val(jsonData.EnteredSegLength);


            unblockPage();
        },
        error: function (err) {
            alert("err");
            alert("error " + err);
            unblockPage();

        }
    });

}

function validateForm() {
    if (parseFloat($("#TotalAvailableRoadLength").val()) == 0) {
        $("#endChainnage").show("slow");
        $("#endChainnage").html("<span style='color:red'><b>Remaining Length is 0, CBR details can not be added.</b></span>");
        return false;
    }
    return true;
}