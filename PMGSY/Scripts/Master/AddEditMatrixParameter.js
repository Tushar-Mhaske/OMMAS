
$(document).ready(function () {
    $.validator.unobtrusive.parse('#frmMatrixParam');
   
    $("#btnSave").click(function (e) {

        if ($("#frmMatrixParam").valid()) {

            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            if ($('#frmMatrixParam').valid()) {
                $.ajax({
                    type: 'POST',
                    url: '/Master/AddMatrixParam/',
                    async: false,
                    data: $("#frmMatrixParam").serialize(),
                    success: function (data) {
                        if (data.success == true) {
                            alert(data.message);
                         
                            $("#btnAdd").show();
                            $("#btnSearch").hide();
                            $('#dvAddMatrixParam').hide();
                            $('#tblMatrixParamDetails').trigger('reloadGrid');
                            $.unblockUI();
                        }
                        else if (data.success == false) {
                            if (data.message != "") {
                                $('#message').html(data.message);
                                $('#dvErrorMessage').show('slow');
                                $.unblockUI();
                            }

                        }
                        else {
                            $("#loksabhaDetails").html(data);
                        }
                        $.unblockUI();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.responseText);
                        $.unblockUI();
                    }
                });
            }
        }
    });
     
  
                $("#spCollapseIconCN").click(function () {

                    if ($("#dvAddMatrixParam").is(":visible")) {
                        $("#dvAddMatrixParam").hide("slow");

                        $("#btnAdd").show();
                        $('#btnSearch').hide();
                    }
                });

                $("#btnCancel").click(function (e) {
    
                    $("#btnAdd").show();
                    $('#btnSearch').hide();
                    $('#dvAddMatrixParam').hide();
     
                })


                $('#btnReset').click(function () {
                    $('#frmMatrixParam')[0].reset();
                    $('#Weight').val(''); //value of weight 0
                });

 });
  
          
 

            $("#btnUpdate").click(function (e) {
                if ($("#frmMatrixParam").valid()) {
                    $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
                    $.ajax({
                        type: 'POST',
                        url: '/Master/EditMatrixDetails/',
                        async: false,
                        data: $("#frmMatrixParam").serialize(),
                        success: function (data) {
                            if (data.success==true) {
                                alert(data.message);
                                $('#btnSearch').hide();
                                $("#btnAdd").show();
                                $('#dvAddMatrixParam').hide();
                                $('#tblMatrixParamDetails').trigger('reloadGrid');
                            }
                            else {
                                $('#message').html(data.message);
                                $('#dvErrorMessage').show('slow');
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
 
 
