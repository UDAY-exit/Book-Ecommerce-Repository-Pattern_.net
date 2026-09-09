

var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tbData').DataTable({
        "ajax": {
            "url": "/Admin/CoverType/GetAll"
        },
        "lengthMenu": [
            [2, 4, 6, 8],
            ["two","four","six","eight"]

        ],
        "columns": [
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                    <a href="/Admin/CoverType/Upsert/${data}" class="btn btn-info">
                    <i class="fas fa-edit"></i></a>
                   
                    </div>
                    `;
                }
            },
            { "data":"name","width":"50%"},
           {
                "data": "id",
                "render": function (data) {
                    return `
                   <div class="text-center">
                    <a class="btn btn-danger" onclick=Delete("/Admin/CoverType/Delete/${data}")>
                    <i class="fas fa-trash"></i>
                    </a>
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
        dangerModel: true
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