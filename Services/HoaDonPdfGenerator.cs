using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DACS_QuanLyPhongTro.Models;

public class HoaDonPdfGenerator
{
    public static byte[] Generate(HoaDon hoaDon)
    {
    var chuTro = hoaDon.PhongTro?.ToaNha?.ChuTro;
    var toaNha = hoaDon.PhongTro?.ToaNha;
    var qrImagePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "qr_vietcombank.jpg");
    var thangNam = hoaDon.NgayLap.ToString("MM/yyyy");
    var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.Content().Column(col =>
                {
                    // Tiêu đề trên cùng
                    col.Item().Text($"HÓA ĐƠN PHÒNG TRỌ THÁNG {thangNam}").FontSize(20).Bold().AlignCenter();
                    col.Item().PaddingTop(10).Text($"Chủ trọ: {chuTro?.HoTen ?? "..."}").FontSize(12).AlignCenter().Bold();
                    col.Item().Text($"Địa chỉ tòa nhà: {toaNha?.DiaChi ?? "..."}").FontSize(11).AlignCenter();
                    col.Item().Text($"SĐT chủ trọ: {chuTro?.SoDienThoai ?? "..."}").FontSize(11).AlignCenter();

                    // Thông tin khách hàng và bảng hóa đơn
                    col.Item().PaddingTop(30).Text($"Tên khách hàng: {hoaDon.KhachThue?.HoTen ?? "..."}").FontSize(12).Bold();
                    col.Item().Text($"Ngày lập: {hoaDon.NgayLap:dd/MM/yyyy}").FontSize(12);

                    col.Item().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); // STT
                            columns.RelativeColumn(2); // Tên khoản
                            columns.RelativeColumn(2); // Số lượng
                            columns.RelativeColumn(2); // Đơn giá
                            columns.RelativeColumn(2); // Thành tiền
                        });
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("STT").FontSize(10).Bold();
                            header.Cell().Element(CellStyle).Text("Tên khoản").FontSize(10).Bold();
                            header.Cell().Element(CellStyle).Text("Số lượng").FontSize(10).Bold();
                            header.Cell().Element(CellStyle).Text("Đơn giá").FontSize(10).Bold();
                            header.Cell().Element(CellStyle).Text("Thành tiền").FontSize(10).Bold();
                        });
                        int stt = 1;
                        table.Cell().Element(CellStyle).Text((stt++).ToString());
                        table.Cell().Element(CellStyle).Text("Tiền phòng");
                        table.Cell().Element(CellStyle).Text("1");
                        table.Cell().Element(CellStyle).Text($"{hoaDon.PhongTro?.GiaThue:N0} VNĐ");
                        table.Cell().Element(CellStyle).Text($"{hoaDon.TienPhong:N0} VNĐ");

                        table.Cell().Element(CellStyle).Text((stt++).ToString());
                        table.Cell().Element(CellStyle).Text("Tiền điện");
                        table.Cell().Element(CellStyle).Text("1");
                        table.Cell().Element(CellStyle).Text($"{hoaDon.TienDien:N0} VNĐ");
                        table.Cell().Element(CellStyle).Text($"{hoaDon.TienDien:N0} VNĐ");

                        table.Cell().Element(CellStyle).Text((stt++).ToString());
                        table.Cell().Element(CellStyle).Text("Tiền nước");
                        table.Cell().Element(CellStyle).Text("1");
                        table.Cell().Element(CellStyle).Text($"{hoaDon.TienNuoc:N0} VNĐ");
                        table.Cell().Element(CellStyle).Text($"{hoaDon.TienNuoc:N0} VNĐ");

                        table.Cell().Element(CellStyle).Text((stt++).ToString());
                        table.Cell().Element(CellStyle).Text("Tiền dịch vụ");
                        table.Cell().Element(CellStyle).Text("1");
                        table.Cell().Element(CellStyle).Text($"{hoaDon.TienDichVu:N0} VNĐ");
                        table.Cell().Element(CellStyle).Text($"{hoaDon.TienDichVu:N0} VNĐ");

                        table.Cell().Element(CellStyle).Text("");
                        table.Cell().Element(CellStyle).Text("");
                        table.Cell().Element(CellStyle).Text("");
                        table.Cell().Element(CellStyle).Text("Tổng cộng").Bold();
                        table.Cell().Element(CellStyle).Text($"{hoaDon.TongTien:N0} VNĐ").Bold();
                    });

                    // Hàng cuối: ghi chú, QR, ký tên
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().PaddingTop(30).Text("Ghi chú:").FontSize(11).Bold();
                            left.Item().Text("Quý khách có thể thanh toán tiền mặt hoặc chuyển khoản qua mã QR bên dưới.").FontSize(10);
                            left.Item().PaddingLeft(20).Container().Width(120).Image(qrImagePath);
                            left.Item().PaddingLeft(10).Text("LE PHUOC LONG - 1024104454").FontSize(10).Bold();
                        });
                        row.RelativeItem().Column(right =>
                        {
                            right.Item().PaddingLeft(140).PaddingTop(30).Text("Chủ trọ xác nhận").FontSize(13).Bold();
                            right.Item().PaddingLeft(160).Text("Đã ký").FontSize(12).Bold();
                        });
                    });
                });
            });
        }); 
        static IContainer CellStyle(IContainer container) => container.BorderBottom(1).PaddingVertical(2).PaddingHorizontal(4);
        return document.GeneratePdf();
    }
}