$(document).ready(function () {

    $.validator.unobtrusive.parse($('#frmGepnicProposalLayout'));

    $('#btnSubmit').click(function () {
        if ($('#frmGepnicProposalLayout').valid()) {

            $.ajax({
                type: 'POST',
                url: '/Proposal/InsertGepnicProposals',
                data: $('#frmGepnicProposalLayout').serialize(),
                async: false,
                cache: false,
                success: function (data) {
                    if (data.success == true)
                    {
                        alert(data.message);
                        $('#tbGepnicProposalList').trigger('reloadGrid');
                      
                        $("#dvGepnicProposalModal").html();
                        $("#dvGepnicProposalModal").dialog('close');
                    }
                    else
                    {
                        alert(data.message);
                    }

                },
                error: function () {
                    alert('Error ocurred');
                }
            });
        }
        else {
            return false;
        }
    });
});