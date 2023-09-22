$(document).ready(function () {
-
    $("#spCollapseIconCN").click(function () {
 
        $("#searchExecution").show('slow')
        $('#tbExecutionList').trigger('reloadGrid');
        $('#gview_tbExecutionList .ui-jqgrid-titlebar-close>span').trigger('click');
        
    });

    $('#btnAddQCR').click(function () {

        var Uploadeddate = $("#txtUploaddate").val().split("/");

        var form = $('#formAddQCR');
        var formadata = new FormData(form.get(0)); //__RequestVerificationToken 
        var fileUpload = $("#BGFile").get(0);
        var FileBG = fileUpload.files[0]
        formadata.append("BGFile", FileBG);
        formadata.append("IMS_PR_ROAD_CODE", $("#IMS_PR_ROAD_CODE").val());
        formadata.append("UploadedDate", $("#txtUploaddate").val());
        formadata.append("uploadRemark", $('#txtUploadRemark').val());

        if ($('#btnAddQCR').valid()) {
        
            if ($("#formAddQCR").valid())
               {
                $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: '/QualityMonitoring/AddQCRdDetails',
                type: 'POST',
                cache: false,
                async: false,
                contentType: false,
                processData: false,
                beforeSend: function () {

                },
               
                data: formadata,
                success: function (response) {
                    alert(response.message);
                    $("#formAddQCR")[0].reset();
                    if (response.success) {                              
                        $("#formAddQCR")[0].reset();
                        $('#tbExecutionListPDF').trigger('reloadGrid');                  
                    }
                    if (response.file == false)
                        $('#BGFile').val('');
                    $.unblockUI();

                },
                error: function () {                   
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });
        }
       }
         
    });
  
    $('#btnUpdateQCR').click(function () {

        var Uploadeddate = $("#txtUploaddate").val().split("/");

        var form = $('#formAddQCR');
        var formadata = new FormData(form.get(0)); //__RequestVerificationToken 
        var fileUpload = $("#BGFile").get(0);
        var FileBG = fileUpload.files[0]
        formadata.append("BGFile", FileBG);
        formadata.append("IMS_PR_ROAD_CODE", $("#IMS_PR_ROAD_CODE").val());
        formadata.append("UploadedDate", $("#txtUploaddate").val());
        formadata.append("uploadRemark", $('#txtUploadRemark').val());

        if ($('#btnUpdateQCR').valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
            $.ajax({
                url: '/QualityMonitoring/AddQCRdDetails',
                type: 'POST',
                cache: false,
                async: false,
                contentType: false,
                processData: false,
                beforeSend: function () {

                },
               
                data: formadata,
                success: function (response) {
                    alert(response.message);
                    if (response.success) {

                        $('#divAddQCR').hide('slow');
                        $("#searchExecution").show('slow')
                        $('#tbExecutionList').trigger('reloadGrid');
                        $('#gview_tbExecutionList .ui-jqgrid-titlebar-close>span').trigger('click');
                    }
                    if (response.file == false)
                        $('#BGFile').val('');
                    $.unblockUI();

                },
                error: function () {
                    $.unblockUI();
                    alert("An Error");
                    return false;
                },
            });

        }

    });

 })

