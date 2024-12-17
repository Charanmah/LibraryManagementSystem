$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    const dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'title', "width": "20%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "20%" },
            { data: 'category.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <button class="btn btn-danger mx-2" onClick="deleteProduct(${data})">
                                <i class="bi bi-trash-fill"></i> Delete
                            </button>
                        </div>
                    `;
                },
                "width": "25%"
            }
        ]
    });
}

function deleteProduct(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: `/admin/product/delete/${id}`,
                type: 'DELETE',
                success: function (response) {
                    if (response.success) {
                        dataTable.ajax.reload(); // Reload the DataTable
                        Swal.fire(
                            'Deleted!',
                            response.message,
                            'success'
                        );
                    } else {
                        Swal.fire(
                            'Error!',
                            response.message,
                            'error'
                        );
                    }
                },
                error: function (xhr, status, error) {
                    const errorMessage = xhr.responseJSON?.message || 'An error occurred while deleting the product.';
                    Swal.fire(
                        'Error!',
                        errorMessage,
                        'error'
                    );
                }
            });
        }
    });
}
