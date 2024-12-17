$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    const dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/company/getall' },
        "columns": [
            { data: 'name', "width": "20%" },
            { data: 'streetAddress', "width": "15%" },
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "20%" },
            { data: 'phoneNumber', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <button class="btn btn-danger mx-2" onClick="deleteCompany(${data})">
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

function deleteCompany(id) {
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
                url: `/admin/company/delete/${id}`,
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
                    const errorMessage = xhr.responseJSON?.message || 'An error occurred while deleting the company.';
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
