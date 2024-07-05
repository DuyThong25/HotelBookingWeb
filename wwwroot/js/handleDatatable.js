$(document).ready(function () {
    $('#myTable').DataTable();
});
function DeleteConfirm(url, element) {
    Swal.fire({
        title: "Bạn có chắc chắn?",
        text: "Bạn sẽ không thể hoàn tác",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Có, xóa!",
        cancelButtonText: "Không!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: (data) => {
                    // Xóa hàng từ DataTable và DOM
                    //var table = $('#myTable').DataTable();
                    //table.row($(element).parents('tr'))
                    //    .remove()
                    //    .draw();
                    Swal.fire({
                        title: "Xóa!",
                        text: data.message,
                        icon: "success"
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.reload();
                        }
                    });
                }
            })
        }
    });
}