var dataTable;
$(document).ready(function () {
    loadDataTable();
}
);


function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            url: '/admin/user/getall'
        },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'company.name', "width": "10%" },
            { data: 'roleName', "width": "20%" },
            {

                data: { id: 'id', lockoutEnd:'lockouEnd'},
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        return `<div class="text-center" >
                                <a  onclick="LockUnLock('${data.id}')" class="btn btn-danger text-white"
                       style="cursor:pointer; width=100px"> <i class="bi bi-lock-fill"></i> lock</a > 
 
                         <a href="/admin/user/manageuser?id=${data.id}" class="btn btn-danger text-white"
                       style="cursor:pointer; width=100px"> <i class="bi bi-pencil-square"></i> premission</a > 
                        </div>
                        `;
                    }

                    else {
                        return `<div class="text-center" >
                       <a onclick="LockUnLock('${data.id}')"  class="btn btn-success text-white"
                       style="cursor:pointer; width=100px"> <i class="bi bi-unlock-fill"></i> unlock</a > 
                       <a href="/admin/user/manageuser?id=${data.id}" class="btn btn-danger text-white"
                       style="cursor:pointer; width=100px"> <i class="bi bi-pencil-square"></i> premission</a > 
                        </div>
                        `;
                    }
                },
                "width": "25%"
            }


        ]
    });
}

function LockUnLock(id)
{
    $.ajax({
        type: "POST",
        url: '/admin/user/LockUnLock?id='+id,
        data: JSON.stringify(id),
        contentType : 'application/json',
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}


