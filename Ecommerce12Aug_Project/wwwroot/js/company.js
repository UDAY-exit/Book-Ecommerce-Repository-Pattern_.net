var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "12%" },
            { "data": "streetAddress", "width": "12%" },
            { "data": "city", "width": "12%" },
            { "data": "state", "width": "12%" },
            { "data": "postalCode", "width": "12%" },
            { "data": "phoneNumber", "width": "12%" },
            {
                "data": "isAuthorizedCompany",
                "render": function (data) {
                    if (data)
                    {
                        return `<input type="checkbox" checked disabled />`;
                    }
                    else
                    {
                        return `<input type="checkbox" disabled />`;
                    }
                }
            },
            {
                "data":"id",
                "render": function(data) {
                    return `
                    <div class="text-center">
                    <a href="/Admin/Company/Upsert/${data}" class="btn btn-info" >
                     <i class="fas fa-edit"></i></a>
                     <a class="btn btn-danger" onclick=Delete("/Admin/Company/Delete/${data}")>
                      <i class="fas fa-trash"></i></a>
                    </div>
                    `;
                }
            }
        ]
    })
}

function Delete(url) {
    swal({
        title: "Want to delete  ???",
        text: "Really!!!!!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    });
}