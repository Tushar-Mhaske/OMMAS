
$(document).ready(function () {

    //Load test result List
    ShowTestResultList();

    ShowTestResultSampleList();

    //validation 
    //$.validator.unobstrusive.parse($("#frmTestResult"));
    $.validator.unobtrusive.parse($('#frmTestResult'));

    //Save Details
    
    $("#btnSave").click(function () {

        if($("#frmTestResult").valid())
        {
            //set hidden_ims_pr_road_code
            if ($("#RoleID").val() != 36) {
                $("#hidden_ims_pr_road_code").val($("#IMS_PR_ROAD_CODE").val());
            }
            $.ajax({

                url: '/Proposal/AddTestResultDetails',
                type: 'POST',
                catche: false,
                data: $("#frmTestResult").serialize(),
                beforeSend: function () {
                    blockPage();
                },
                error: function (xhr,status,error) {
                    unblockPage();
                    alert("Request can not be  processed at this time, please try after some time...");
                    return false;
                },
                success: function (response) {
                    
                    if (response.success === undefined)
                    {
                        $("#dvTestResultForm").html(response); //error 
                        unblockPage();
                    }
                    else if (response.success) {
                        alert(response.message);
                        $("#btnReset").trigger("click");
                        $('#tbTestResultList').trigger('reloadGrid');
                        $('#tbTestResultSampleList').trigger('reloadGrid');
                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");                        
                        $("#divError span:eq(1)").html("<strong>Alert: </strong>" + response.message);
                        unblockPage();
                    }
                },
            });
        }

    });//end of save


    $("#btnUpdate").click(function () {

        if ($("#frmTestResult").valid()) {
            $.ajax({
                url: '/Proposal/UpdateTestResultDetails/',
                type: 'POST',
                catche: false,
                data: $("#frmTestResult").serialize(),
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
                        $("#dvTestResultForm").html(response);
                        unblockPage();
                    } else if (response.success) {

                        alert(response.message);

                        //alert($("#hidden_ims_pr_road_code").val());
                        
                        //$("#dvTestResultForm").load("/Proposal/TestResultDetails/", $("#hidden_ims_pr_road_code").val());

                        //if ($("#dvError").is(":visible")) {
                        //    $("#divError").hide("slow");
                        //    $("#divError span:eq(1)").html('');
                        //}                        

                        // LoadTestResultForm();

                        loadTestResultDetailsForm();
                        
                        unblockPage();
                    }
                    else {
                        $("#divError").show("slow");
                        $("#divError span:eq(1)").html("<strong>Alert: </strong> " + response.message);
                        unblockPage();
                    }
                }
            });//end
        }
    });
    
    $("#btnReset").click(function () {
        $("#divError").hide('slow');
        $("#divError span:eq(1)").html('');
    });

    $("#btnCancel").click(function () {        
        //LoadTestResultForm();

        //$("#dvTestResultForm").load("/Proposal/TestResultDetails/");

        //if ($("#dvError").is(":visible")) {
        //    $("#divError").hide("slow");
        //    $("#divError span:eq(1)").html('');
        //}

        loadTestResultDetailsForm();
    });    
    
});




function ShowTestResultList()
{

    IMS_PR_ROAD_CODE = $('#IMS_PR_ROAD_CODE').val();
    
    jQuery("#tbTestResultList").jqGrid({


        url: '/Proposal/GetTestResultList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Sample', 'Chainage', 'Test Name', 'Value', 'Edit', 'Delete'],
        colModel: [
            { name: 'IMS_SAMPLE_ID', index: 'IMS_SAMPLE_ID', width: '300px', sortable: true, align: 'left' },
            { name: 'IMS_CHAINAGE', index: 'IMS_CHAINAGE', width: '130px', sortable: true, align: 'center' },
            { name: 'IMS_TEST_CODE', index: 'IMS_TEST_CODE', width: '300px', sortable: true, align: "left" },
            { name: 'IMS_TEST_RESULT', index: 'IMS_TEST_RESULT', width: '130px', sortable: true, align: "center" },
            { name: 'Edit', width: '50px', sortable: false, resize: false, align: "center", formatter: formatColumnEdit },
            {name:'Delete', width:'50px',sortable:false,resize:false,align:"center",formatter:formatColumDelete}
        ],
        pager: $("#dvTestResultListPager"),
        sortorder: "asc",
        sortname: "IMS_SAMPLE_ID",
        rowNum: 9,
        pginput: true,
        rowList:[5,10,15,20],
        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        viewrecords: true,
        recordtext: '{2} records found',
        caption: ' Test Result Details',
        height: 'auto',
        width: '100%',
        
        rownumbers: true,
        loaderror: function (xhr, status, error) {
            
            if (xhr.responseText == 'session expired') {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else { }            
        },        
    });

}


function ShowTestResultSampleList()
{

    IMS_PR_ROAD_CODE = $('#IMS_PR_ROAD_CODE').val();

    jQuery("#tbTestResultSampleList").jqGrid({

        url: '/Proposal/GetTestResultSampleList/',
        datatype: "json",
        mtype: "POST",
        colNames: ['Test Name','Sample', 'Chainage', 'Value'],
        colModel: [
            { name: 'IMS_TEST_CODE', index: 'IMS_TEST_CODE', width: '350px', sortable: false, align: "left" ,hidden:true},
            { name: 'IMS_SAMPLE_ID', index: 'IMS_SAMPLE_ID', width: '600px', sortable: false, align: 'center' },
            { name: 'IMS_CHAINAGE', index: 'IMS_CHAINAGE', width: '205px', sortable: false, align: 'center' },
            { name: 'IMS_TEST_RESULT', index: 'IMS_TEST_RESULT', width: '205px', sortable: false, align: "center" }
            //{ name: 'Edit', width: '40px', sortable: false, resize: false, align: "center", formatter: formatColumnEdit ,hidden:true},
            //{ name: 'Delete', width: '40px', sortable: false, resize: false, align: "center", formatter: formatColumDelete ,hidden:true}
        ],
        pager: $("#dvTestResultSampleListPager"),
        sortorder: "asc",
        sortname: "IMS_TEST_CODE",
        rowNum: 9,
        pginput: true,
        rowList: [5, 10, 15, 20],
        postData: { IMS_PR_ROAD_CODE: IMS_PR_ROAD_CODE, value: Math.random() },
        viewrecords: true,
        recordtext: '{2} records found',
        caption: ' Test Result Sample Details',
        height: 'auto',
        width: '100%',
        grouping: true,
        groupingView:{        
            groupField: ['IMS_TEST_CODE'],
            //groupColumnShow:false
        },
        rownumbers: true,
        loaderror: function (xhr, status, error) {

            if (xhr.responseText == 'session expired') {
                alert(xhr.responseText);
                window.location.href = "/Login/Login";
            }
            else { }
        },

    });

}

function formatColumnEdit(cellvalue,options,rowObject)
{
    if (cellvalue == "") {
        return "<center><span style='border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span style='border-color:white;cursor:pointer' class='ui-icon ui-icon-pencil ui-align-center' title='Click here to Edit Test Result Details' onClick='EditTestResultDetails(\"" + cellvalue.toString() + "\" );'></span></center> ";
    }
}

function formatColumDelete(cellvalue,options,rowObject)
{
    if (cellvalue == "") {
        return "<center><span style=' border-color:white;cursor:pointer;' class='ui-icon ui-icon-locked ui-align-center'></span></center>";
    }
    else {
        return "<center><span style=' border-color:white;cursor:pointer;' title='Click here to Delete Test Result Details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteTestResultDetails(\""+cellvalue.toString()+"\");'></span></center>";
    }
}

function EditTestResultDetails(urlparam)
{   
    //$("#dvTestResultForm").load('/Proposal/EditTestResultDetails/', urlparam);    
    $.ajax({
        url: '/Proposal/EditTestResultDetails/' + urlparam,
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

                $('#dvTestResultForm').html('');
                $("#dvTestResultForm").html(response);
         
            unblockPage();
        }
    });
}

function DeleteTestResultDetails(urlParam) {
    //alert("Delete");

    if (confirm("Are you sure you want to delete test result details ? ")) {
        $.ajax({

            url: '/Proposal/DeleteTestDetails/' + urlParam,
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
                    //$("#tbTestResultList").trigger('reloadGrid');
                    //LoadTestResultForm();

                    $("#btnReset").trigger("click");
                    $('#tbTestResultList').trigger('reloadGrid');
                    $('#tbTestResultSampleList').trigger('reloadGrid');

                    unblockPage();
                }
                else {
                    alert(response.message);
                }
                unblockPage();
            }
        });//end of delete ajax call
    }
}


function loadTestResultDetailsForm()
{
    //$("#dvTestResultForm").load("/Proposal/TestResultDetails/", $("#hidden_ims_pr_road_code").val());
    
    $.ajax({
        url: '/Proposal/TestResultDetails/' + $("#hidden_ims_pr_road_code").val(),
        type: 'GET',
        catche: false,
        error: function (xhr,status,error)
        {
            alert("An error occured while processing your request.");
            return false;
        },
        success: function (response) {
            $("#dvTestResultForm").html(response);

            if ($("#dvError").is(":visible")) {
                $("#divError").hide("slow");
                $("#divError span:eq(1)").html('');
            }
        }
    });

    
}