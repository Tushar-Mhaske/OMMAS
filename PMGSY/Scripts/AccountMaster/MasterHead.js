

$(document).ready(function () {

    $.validator.unobtrusive.parse($('#AccountMasterHeadForm'));


    //Save Details 
    $("#btnSave").click(function () {
        
        if ($("#AccountMasterHeadForm").valid())
        {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $.ajax({
                type: 'POST',
                url: '/AccountMaster/AddMasterHeadDetails/',
                data: $("#AccountMasterHeadForm").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();

                    if (data.success === undefined) {
                        $("#dvMasterHeadAddEdit").html(data);
                    }
                    else if (data.success) {
                        alert(data.message);

                        //$("#btnReset").trigger('click');
                        ResetHeadForm();
                        $("#tbHeadDetailsList").trigger('reloadGrid');
                    }
                    else {
                        $("#divError").show();
                        $("#errorSpan").html(data.message);
                    }
                },
                error: function () {
                    $.unblockUI();

                    alert("Request can not be processed at this time.");
                }
            })

        }

    });

    //Update Details 
    $("#btnUpdate").click(function () {

        if ($("#AccountMasterHeadForm").valid()) {
            $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

            $("#ddlParentHead").attr("disabled",false);

            $.ajax({
                type: 'POST',
                url: '/AccountMaster/EditMasterHeadDetails/',
                data: $("#AccountMasterHeadForm").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    $.unblockUI();

                    if (data.success === undefined) {
                        $("#dvMasterHeadAddEdit").html(data);
                    }
                    else if (data.success) {
                        alert(data.message);
                        $("#tbHeadDetailsList").trigger('reloadGrid');
                        $("#btnCancel").trigger('click');
                    }
                    else {
                        $("#divError").show();
                        $("#errorSpan").html(data.message);
                    }
                },
                error: function () {
                    $.unblockUI();
                    $("#ddlParentHead").attr("disabled", true);
                    alert("Request can not be processed at this time.");
                }
            })

        }

    });

    //Cancel
    $("#btnCancel").click(function () {
        ResetHeadForm();           
    });

    $("#rdoParentHead").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'GET',
            url: '/AccountMaster/AddEditMasterHeadDetails/' + "P",
            //data: $("#AccountMasterHeadForm").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                $("#dvMasterHeadAddEdit").html(data);
                $.unblockUI();

            },
            error: function () {
                $.unblockUI();

                alert("Request can not be processed at this time.");
            }
        })
    });

    $("#rdoSubHead").click(function () {
        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });
        $.ajax({
            type: 'GET',
            url: '/AccountMaster/AddEditMasterHeadDetails/' + "S",
            //data: $("#AccountMasterHeadForm").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                $("#dvMasterHeadAddEdit").html(data);
                $.unblockUI();

            },
            error: function () {
                $.unblockUI();

                alert("Request can not be processed at this time.");
            }
        })
    });

    $("#btnReset").click(function () {

        //$("#ddlFundType").val('');
        //$("#ddlParentHead").val('');
        //$("#ddlLevel").val(0);
        //$("#ddlHeadCategory").val("");
        //$("#HEAD_CODE").val("");
        //$("#HEAD_NAME").val("");
    });

    $("#ddlParentHead").change(function () {

        if ($("#EncryptedHeadID").val()!=null || ($("#EncryptedHeadID").val()!=""))
        {
            //$("#HEAD_CODE").val($("#ddlParentHead").val() + ".");

            $.ajax({
                type: 'POST',
                url: '/AccountMaster/GetParentHeadCode/' + $("#ddlParentHead").val(),
                //data: $("#AccountMasterHeadForm").serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    //$("#dvMasterHeadAddEdit").html(data);

                    $("#HEAD_CODE").val(data.HeadCode + ".");

                    $.unblockUI();
                },
                error: function () {
                    $.unblockUI();
                    alert("Request can not be processed at this time.");
                }
            })


        }

        if ($("#ddlParentHead").val()=="")
        {
            $("#HEAD_CODE").val("");
        }
    });



});

function ResetHeadForm()
{
    var ParentSubHead;

    if ($("#rdoParentHead").is(":checked")) {
        ParentSubHead = "P";
    }
    else {
        ParentSubHead = "S";
    }

        $.blockUI({ message: '<img src="/Content/images/ajax-loader.gif"/>' });

        $.ajax({
            type: 'GET',
            url: '/AccountMaster/AddEditMasterHeadDetails/' + ParentSubHead,
            //data: $("#AccountMasterHeadForm").serialize(),
            async: false,
            cache: false,
            success: function (data) {
                $("#dvMasterHeadAddEdit").html(data);
                $.unblockUI();

            },
            error: function () {
                $.unblockUI();

                alert("Request can not be processed at this time.");
            }
        })
}