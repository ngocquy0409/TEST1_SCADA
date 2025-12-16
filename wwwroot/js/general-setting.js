
let dongTruongCaDangChon = null;
let dongSanPhamDangChon = null;


// chọn dòng
function chonDong(row, loai) {

    if (loai === 'truongca') {
        if (dongTruongCaDangChon)
            dongTruongCaDangChon.classList.remove("selected");

        dongTruongCaDangChon = row;
        row.classList.add("selected");
    }

    if (loai === 'sanpham') {
        if (dongSanPhamDangChon)
            dongSanPhamDangChon.classList.remove("selected");

        dongSanPhamDangChon = row;
        row.classList.add("selected");
    }
}


// xóa trưởng ca
function xoaTruongCa() {

    if (!dongTruongCaDangChon) {
        alert("Vui lòng chọn trưởng ca cần xóa!");
        return;
    }

    if (!confirm("Bạn có chắc muốn xóa trưởng ca này?"))
        return;

    let id = dongTruongCaDangChon.dataset.id;

    fetch('/Settings/DeleteTruongCa', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `id=${id}`
    })
        .then(res => {
            if (res.ok) {
                dongTruongCaDangChon.remove();
                dongTruongCaDangChon = null;
            } else {
                alert("Xóa thất bại!");
            }
        });
}


// xóa sản phẩm
function xoaSanPham() {
    if (!dongSanPhamDangChon) {
        alert("Vui lòng chọn sản phẩm cần xóa!");
        return;
    }


    if (!confirm("Bạn có chắc muốn xóa sản phẩm này?"))
        return;

    let id = dongSanPhamDangChon.dataset.id;

    fetch('/Settings/DeleteSanPham', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `id=${id}`
    })
        .then(res => {
            if (res.ok) {
                dongSanPhamDangChon.remove();
                dongSanPhamDangChon = null;
            } else {
                alert("Xóa thất bại!");
            }
        });
}