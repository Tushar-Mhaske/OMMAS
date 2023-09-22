
$(document).ready(function () {

    //Load test result List
    ShowPropAddCostList();



    //validation 
    //$.validator.unobstrusive.parse($("#frmPropAddCost"));
    $.validator.unobtrusive.parse($('#frmPropAddCost'));


    $('#IMS_RELEASE_DATE').datepicker({

        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/Content/Images/calendar_2.png",
        showButtonText: 'Choose date',
        buttonImageOnly: true,
        buttonText: "select date",
        onSelect: function (selectedDate) {
        },
        onClose: function () {
            $(this).focus().blur();
        }
    });

    //Save Details

    //$("#btnSave").click(function () {

    $("#frmPropAddCost").on('submit', function (event) {

        if ($("#frmPropAddCost").valid()) {

            event.stopPropagation(); // Stop stuff happening call double avoid to action
            event.preventDefault(); // call double avoid to action

            var form_data = new FormData();

            var objLetterFile = $("input#fileLetter").prop("files");

            form_data.append("fileLetter", objLetterFile[0]);

            $("#TOTAL_AMOUNT").attr("disabled", false);

            var data = $("#frmPropAddCost").serializeArray();

            for (var i = 0; i < data.length; i++) {
                form_data.append(data[i].name, data[i].value);
            }

            $.ajax({

                url: '/Proposal/AddAdditionalCostDetails',
                type: 'POST',
                catche: false,
                data: form_data,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("Request can not be  processed at this time, please try after some time...");
                    return false;
                },
                success: function (response) {

                    if (response.success === undefined) {
                        $("#dvPropAddCostForm").html(response); //error 
                        unblockPage();
                    }
                    else if (response.success == true) {
                        alert(response.message);
                        $("#btnReset").trigger("click");
                        $('#tbPropAddCostList').trigger('reloadGrid');
                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html("<strong>Alert: </strong>" + response.message);
                        unblockPage();
                    }
                    $("#TOTAL_AMOUNT").attr("disabled", true);

                },
            });
        }

    });//end of save


    $("#btnUpdate").click(function () {

        if ($("#frmPropAddCost").valid()) {
            $("#TOTAL_AMOUNT").attr("disabled", false);
            $.ajax({
                url: '/Proposal/UpdateAdditionalCostDetails/',
                type: 'POST',
                catche: false,
                data: $("#frmPropAddCost").serialize(),
                beforeSend: function () {

                    blockPage();
                },
                error: function (xhr, status, error) {
                    unblockPage();
                    alert("Request can not be processed at this time, please try after some time...");
                    return false;
                },
                success: function (response) {

                    if (response.success === undefined) {
                        $("#dvPropAddCostForm").html(response);
                        unblockPage();
                    } else if (response.success) {

                        alert(response.message);

                        //alert($("#hidden_ims_pr_road_code").val());

                        //$("#dvPropAddCostForm").load("/Proposal/PropAddCostDetails/", $("#hidden_ims_pr_road_code").val());

                        //if ($("#dvError").is(":visible")) {
                        //    $("#divError").hide("slow");
                        //    $("#divError span:eq(1)").html('');
                        //}                        

                        // LoadPropAddCostForm();

                        loadPropAddCostDetailsForm();

                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html("<strong>Alert: </strong> " + response.message);
                        unblockPage();
                    }
                    $("#TOTAL_AMOUNT").attr("disabled", true);

                }
            });//end
        }
    });

    $("#btnReset").click(function () {
        $("#divError").hide('slow');
        $("#divError span:eq(1)").html('');
        $('#lblTotalAmt').val(0);
        $('#lblDisplayTotAmt').html(0);
    });

    $("#btnCancel").click(function () {
        //LoadPropAddCostForm();

        //$("#dvPropAddCostForm").load("/Proposal/PropAddCostDetails/");

        //if ($("#dvError").is(":visible")) {
        //    $("#divError").hide("slow");
        //    $("#divError span:eq(1)").html('');
        //}

        loadPropAddCostDetailsForm();
    });

    $('#IMS_STATE_AMOUNT').blur(function () {
        CalaculateTotalAmount();
    });
    $('#IMS_MORD_AMOUNT').blur(function () {
        CalaculateTotalAmount();
    });

});




function ShowPropAddCostList() {

    IMS_PR_ROAD_CODE = $('#IMS_PR_ROAD_CODE').val();

    jQuery("#tbPropAddCostList").jqGrid({
        url: '/Proposal/GetAdditionalCostList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Additional State Share', 'Additional Mord Share', 'Total Amount', 'Release Date', 'Letter Number', 'File', 'Edit', 'Delete'],
        colModel: [
            { name: 'IMS_STATE_AMOUNT', index: 'IMS_STATE_AMOUNT', width: '100px', sortable: true, align: 'right' },
            { name: 'IMS_MORD_AMOUNT', index: 'IMS_MORD_AMOUNT', width: '100px', sortable: true, align: 'right' },
            { name: 'TOTAL_AMOUNT', index: 'TOTAL_AMOUNT', width: '120px', sortable: false, align: "right" },
            { name: 'IMS_RELEASE_DATE', index: 'IMS_RELEASE_DATE', width: '120px', sortable: true, align: "center" },
            { name: 'IMS_LETTER_NUMBER', index: 'IMS_LETTER_NUMBER', width: '250px', sortable: false, align: "right" },
            { name: 'IMS_FILE_NAME', index: 'IMS_FILE_NAME', width: '250px', sortable: true, align: "center" },
            { name: 'Edit', width: '50px', sortable: false, resize: false, align: "center" },
            { name: 'Delete', width: '50px', sortable: false, resize: false, align: "center" }
        ],
        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        pager: $("#dvPropAddCostListPager"),
        sortorder: "asc",
        sortname: "IMS_SAMPLE_ID",
        rowNum: 5,
        pginput: true,
        rowList: [5, 10, 15, 20],
        viewrecords: true,
        recordtext: '{2} records found',
        caption: ' Proposal Additional Cost Details',
        height: 'auto',
        width: '100%',
        rownumbers: true,
        footerrow: true,
        loadComplete: function () {
            //Total of Columns
            var IMS_STATE_AMOUNT_T = $(this).jqGrid('getCol', 'IMS_STATE_AMOUNT', false, 'sum');
            IMS_STATE_AMOUNT_T = parseFloat(IMS_STATE_AMOUNT_T).toFixed(2);
            var IMS_MORD_AMOUNT_T = $(this).jqGrid('getCol', 'IMS_MORD_AMOUNT', false, 'sum');
            IMS_MORD_AMOUNT_T = parseFloat(IMS_MORD_AMOUNT_T).toFixed(2);
            var TOTAL_AMOUNT_T = $(this).jqGrid('getCol', 'TOTAL_AMOUNT', false, 'sum');
            TOTAL_AMOUNT_T = parseFloat(TOTAL_AMOUNT_T).toFixed(2);


            // $(this).jqGrid('footerData', 'set', { MAST_STATE_NAME: '<b>Total</b>' });
            $(this).jqGrid('footerData', 'set', { IMS_STATE_AMOUNT: IMS_STATE_AMOUNT_T }, true);
            $(this).jqGrid('footerData', 'set', { IMS_MORD_AMOUNT: IMS_MORD_AMOUNT_T }, true);
            $(this).jqGrid('footerData', 'set', { TOTAL_AMOUNT: TOTAL_AMOUNT_T }, true);



        },
        loaderror: function (xhr, status, error) {

            if (xhr.responseText == 'session expired') {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else { }
        },
    });

}

function formatColumnEdit(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span style='border-color:white;cursor:pointer' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to Edit Test Result Details' onClick='EditPropAddCostDetails(\"" + cellvalue.toString() + "\" );'></span></center> ";
    }
}

function formatColumDelete(cellvalue, options, rowObject) {
    if (cellvalue == "") {
        return "<center><span style=' border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span style=' border-color:white;cursor:pointer;' title='Click here to Delete Test Result Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeletePropAddCostDetails(\"" + cellvalue.toString() + "\");'></span></center>";
    }
}

function EditCostProposal(urlparam) {
    //$("#dvPropAddCostForm").load('/Proposal/EditPropAddCostDetails/', urlparam);   

    $.ajax({
        url: '/Proposal/EditAdditionalCostDetails/' + urlparam,
        Type: 'POST',
        catche: false,
        beforeSend: function () {
            blockPage();
        },
        error: function (xhr, status, error) {
            unblockPage();
            alert("An error occured while processing your request.");

            return false;
        },
        success: function (response) {

            $('#divAddPropAddCost').html('');
            $("#divAddPropAddCost").html(response);

            unblockPage();
        }
    });
}

function DeleteCostProposal(urlParam) {
    //alert("Delete");

    if (confirm("Are you sure you want to delete additional cost details ? ")) {
        $.ajax({

            url: '/Proposal/DeleteAdditionalCostDetails/' + urlParam,
            type: 'POST',
            catche: false,
            error: function (xhr, status, error) {
                alert("Request can not be processed at this time, please try after some time...");
                return false;
            },
            beforeSend: function () {
                blockPage();
            },
            success: function (response) {

                if (response.success) {
                    alert(response.message);
                    //$("#tbPropAddCostList").trigger('reloadGrid');
                    //LoadPropAddCostForm();                 
                    loadPropAddCostDetailsForm();
                    $('#tbPropAddCostList').trigger('reloadGrid');

                    unblockPage();
                }
                else {
                    $("#divError").show("slow");
                    $("#divError span:eq(1)").html("<strong>Alert: </strong>" + response.message);
                    unblockPage();

                }
                unblockPage();
            }
        });//end of delete ajax call
    }
}


function loadPropAddCostDetailsForm() {
    //$("#dvPropAddCostForm").load("/Proposal/PropAddCostDetails/", $("#hidden_ims_pr_road_code").val());

    $.ajax({
        url: '/Proposal/AdditionalCostDetails?id=' + $('#EncryptedRoadCode').val(),
        type: 'GET',
        catche: false,
        error: function (xhr, status, error) {
            alert("An error occured while processing your request.");
            return false;
        },
        success: function (response) {
            $('#divAddPropAddCost').html('');
            $("#divAddPropAddCost").html(response);

            if ($("#dvError").is(":visible")) {
                $("#divError").hide("slow");
                $("#divError span:eq(1)").html('');
            }
        }
    });


}

function CalaculateTotalAmount() {
    var totalAmount = 0
    var stateAmount = 0;
    var mordAmount = 0;
    if ($('#IMS_STATE_AMOUNT').val() >= 0) {
        stateAmount = $('#IMS_STATE_AMOUNT').val();
    }
    if ($('#IMS_MORD_AMOUNT').val() >= 0) {
        mordAmount = $('#IMS_MORD_AMOUNT').val();
    }
    totalAmount = (parseFloat(stateAmount) + parseFloat(mordAmount)).toFixed(2);
    $('#lblTotalAmt').val(totalAmount);
    $('#lblDisplayTotAmt').html($('#lblTotalAmt').val());
}

function downloadFileFromAction(paramurl) {
    window.location = paramurl;
}

function DownloadFile(cellvalue) {
    var url = "/Proposal/DownloadAdditionalCostFile/" + cellvalue;
    downloadFileFromAction(url);
}