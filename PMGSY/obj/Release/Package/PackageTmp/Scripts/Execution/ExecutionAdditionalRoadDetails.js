
$(document).ready(function () {

    //Load test result List
    //ShowExecAddRoadList();


    //validation 
    $.validator.unobtrusive.parse($('#frmExecAddRoad'));

    //Save Details

    //$("#btnSave").click(function () {

    $("#frmExecAddRoad").on('submit', function (event) {

        debugger;

        if ($("#frmExecAddRoad").valid()) {

            event.stopPropagation(); // Stop stuff happening call double avoid to action
            event.preventDefault(); // call double avoid to action

            var form_data = new FormData();

            var objLetterFile = $("input#fileLetter").prop("files");

            form_data.append("fileLetter", objLetterFile[0]);

            var data = $("#frmExecAddRoad").serializeArray();

            for (var i = 0; i < data.length; i++) {
                form_data.append(data[i].name, data[i].value);
            }
            if (confirm("Do you want to change the work status to in progress ? ")) {
                if (confirm("Once Updated , it cannot be modified ")) {

                    $.ajax({

                        url: '/Execution/AddAdditionalRoadDetails',
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
                                //$('#tbExecAddRoadList').trigger('reloadGrid');
                                //here id 'tbExecutionList' is from ListNewRoadList.cshtml page
                                $('#tbExecutionList').trigger('reloadGrid');
                                CloseExecutionDetails();
                                unblockPage();
                            }
                            else {
                                $("#divError").show("slow");
                                $("#divError span:eq(1)").html("<strong>Alert: </strong>" + response.message);
                                unblockPage();
                            }

                        },
                    });
                } else {
                    return false;
                }
            } else {
                return false;
            }

        }

    });//end of save

    //close the accordion of Additional Road Details
    //here all the id are from ListNewRoadList.cshtml page
    function CloseExecutionDetails() {

        $("#accordion").hide('slow');
        $("#divAddExecution").hide('slow');
        $("#tbExecutionList").jqGrid('setGridState', 'visible');
        ShowFilter();
    }

    //show the filter view 
    //here all the id are from ListNewRoadList.cshtml page
    function ShowFilter() {

        $("#divSearchExecution").show('slow');
        $("#idFilterDiv").toggleClass("ui-icon-circle-triangle-s");
        $('#idFilterDiv').trigger('click');
    }

    $("#btnReset").click(function () {
        $("#divError").hide('slow');
        $("#divError span:eq(1)").html('');
    });


});


//function ShowExecAddRoadList() {

//    debugger;
//    IMS_PR_ROAD_CODE = $('#IMS_PR_ROAD_CODE').val();

//    jQuery("#tbExecAddRoadList").jqGrid({
//        url: '/Execution/GetAdditionalRoadList/',
//        datatype: "json",
//        mtype: "POST",
//        colNames: ['File', 'Action'],
//        colModel: [
//            { name: 'FILE_NAME', index: 'FILE_NAME', width: '300px', sortable: true, align: "center", search: true },
//            { name: 'Dowload_File', width: '250px', sortable: false, resize: false, align: "center" },
//        ],
//        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
//        pager: $("#dvExecAddRoadListPager"),
//        sortorder: "asc",
//        sortname: "FILE_NAME",
//        rowNum: 5,
//        pginput: true,
//        rowList: [5, 10, 15, 20],
//        viewrecords: true,
//        recordtext: '{2} records found',
//        caption: ' Execution Additional Road Details',
//        height: 'auto',
//        width: '100%',
//        rownumbers: true,
//        footerrow: true,
//        loaderror: function (xhr, status, error) {

//            if (xhr.responseText == 'session expired') {
//                alert(xhr.responseText);
//                window.location.href = "/Login/Login";
//            }
//            else { }
//        },
//    });

//}

//function downloadFileFromAction(paramurl) {
//    window.location = paramurl;
//}

//function DownloadFile(cellvalue) {
//    var url = "/Execution/DownloadAdditionalRoadDetailsFile/" + cellvalue;
//    downloadFileFromAction(url);
//}