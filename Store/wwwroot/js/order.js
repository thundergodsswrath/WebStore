var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/Admin/Order/getall?status=' + status },
        "columns": [
            { data: 'id'},
            { data: 'name'},
            { data: 'phoneNumber'},
            { data: 'applicationUser.email'},
            { data: 'orderStatus'},
            { data: 'orderTotal'},
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>               
                    
                    </div>`
                }
            }
        ]
    });
}
