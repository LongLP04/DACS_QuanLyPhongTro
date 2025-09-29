using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DACS_QuanLyPhongTro.Models;
using System;

public class HopDongPdfGenerator
{
    public static byte[] Generate(HopDong hopDong)
    {
        var chuTro = hopDong.PhongTro?.ToaNha?.ChuTro;
        var toaNha = hopDong.PhongTro?.ToaNha;
        var khachThue = hopDong.KhachThue;
        var phong = hopDong.PhongTro;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(45);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(TextStyle.Default.FontFamily("Times New Roman"));
                page.Content().Column(col =>
                {
                    col.Item().Text("CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM").FontSize(12).Bold().AlignCenter();
                    col.Item().Text("Độc Lập - Tự Do - Hạnh Phúc").FontSize(11).AlignCenter();
                    col.Item().Container().PaddingBottom(10).Text("\"oOoOoOo\"").FontSize(10).AlignCenter();
                    col.Item().Text($"HỢP ĐỒNG THUÊ PHÒNG TRỌ").FontSize(16).Bold().AlignCenter();
                    col.Item().Container().PaddingBottom(10).Text($"(HD số: {hopDong.MaHopDong} / {DateTime.Now.Year})").FontSize(11).AlignCenter();

                    // Bên A
                    col.Item().Text("BÊN A: BÊN CHO THUÊ").FontSize(11).Bold();
                    col.Item().PaddingTop(5).Text(
                        t =>
                        {
                            t.Span("Ông / Bà: ");
                            t.Span($"{chuTro?.HoTen ?? "..."}").Bold();
                        }
                    );
                    col.Item().Text($"Ngày sinh: 27/01/2004 ");
                    col.Item().Text(
                        t =>
                        {
                            t.Span("CMND: ");
                            t.Span($"{chuTro?.CCCD ?? "..."}").Bold();
                            t.Span("   Ngày cấp : 15/08/2022   Nơi cấp: Cục cảnh sát quản lý hành chính.");
                        }
                    );
                    col.Item().Text(
                        t =>
                        {
                            t.Span("Thường trú: ");
                            t.Span($"{toaNha?.DiaChi ?? "..."}").Bold();
                        }
                    );
                    col.Item().Text("");
                    // Bên B
                    col.Item().PaddingTop(15).Text("BÊN B: BÊN THUÊ").FontSize(11).Bold();
                    col.Item().PaddingTop(5).Text(
                        t =>
                        {
                            t.Span("Ông / Bà : ");
                            t.Span($"{khachThue?.HoTen ?? "..."}").Bold();
                        }
                    );
                    col.Item().Text($"Ngày sinh: 13/07/2011 ");
                    col.Item().Text(
                        t =>
                        {
                            t.Span("CMND: ");
                            t.Span($"{khachThue?.CCCD ?? "..."}").Bold();
                            t.Span("   Ngày cấp : 25/09/2022   Nơi cấp: Cục cảnh sát quản lý hành chính.");
                        }
                    );
                    col.Item().Text(
                        t =>
                        {
                            t.Span("Thường trú: ");
                            t.Span($"Đội 4, thôn Năng Xã, xã Nghĩa Hiệp, huyện Tư Nghĩa, tỉnh Quảng Ngãi").Bold();
                        }
                    );
                    col.Item().Text("");
                    col.Item().Text("");
                    col.Item().Text("*****************************************************");
                    col.Item().Text($"Hai bên thống nhất và đồng ý với các điều khoản sau:").Italic();
                    
                    // Điều 1
                    col.Item().Text("ĐIỀU 1: THOẢ THUẬN THUÊ.").Bold();
                    col.Item().PaddingTop(5).Text(t =>
                    {
                        t.Span("Bên A đồng ý cho bên B thuê và bên B đồng ý thuê phòng số ");
                        t.Span($"{phong?.SoPhong ?? "..."}").Bold();
                        t.Span(" thuộc địa chỉ phòng trọ của nhà số ");
                        t.Span($"{toaNha?.DiaChi ?? "..."}").Bold();
                        t.Span(".");
                    });

                    col.Item().Text( t =>
                    {
                        t.Span("Hợp đồng thuê phòng bắt đầu từ ngày ");
                        t.Span($"{hopDong.NgayBatDau:dd/MM/yyyy}").Bold();
                        t.Span(" đến ngày ");
                        t.Span($"{hopDong.NgayKetThuc:dd/MM/yyyy}").Bold();
                        t.Span(".");
                    });
                    col.Item().Text("");
                    // Điều 2
                    col.Item().Text("ĐIỀU 2: TIỀN THUÊ NHÀ TRỌ VÀ TIỀN ĐẶT CỌC.").Bold();
                    col.Item().PaddingTop(5).Text( t =>
                    {
                        t.Span("- Giá thuê nhà trọ: ");
                        t.Span($"{phong?.GiaThue:N0} VNĐ/tháng.").Bold();
                    }
                    );
                    col.Item().Text(t =>
                        {
                            t.Span("- Tiền đặt cọc: ");
                            t.Span($"{hopDong.TienCoc:N0} VNĐ.").Bold();
                        }
                    );
                    col.Item().Text(
                        t =>
                        {
                            t.Span("Tiền thuê nhà trọ bên B phải trả cho bên A vào ngày ");
                            t.Span(" 05 hàng tháng").Bold();
                            t.Span(" qua hình thức chuyển khoản hoặc tiền mặt.");
                            t.Span(" Mọi sự cố, trả chậm hoặc trễ hạn của bên B đều phải được sự đồng ý của bên A.");
                        }
                    );
                    col.Item().Text("");
                    // Điều 3
                    col.Item().Text("ĐIỀU 3: TRÁCH NHIỆM BÊN A.").Bold();
                    col.Item().PaddingTop(5).Text("- Giao nhà, trạng thái thiết bị nhà cho bên B đúng ngày ký hợp đồng.");
                    col.Item().Text("- Bảo đảm quyền sử dụng nhà trọ cho bên B trong thời hạn hợp đồng.");
                    col.Item().Text("- Bảo trì, sửa chữa các trang thiết bị trong nhà trọ khi có yêu cầu của bên B.");
                    col.Item().Text("- Hướng dẫn bên B chấp hành đúng các quy định của địa phương về việc lưu trú, tạm trú tạm vắng theo Pháp luật hiện hành.");

                    col.Item().Text("");
                    // Điều 4
                    col.Item().Text("ĐIỀU 4: TRÁCH NHIỆM BÊN B.").Bold();
                    col.Item().PaddingTop(5).Text("- Trả tiền thuê nhà hàng tháng theo hợp đồng.");
                    col.Item().Text("- Sử dụng đúng mục đích thuê nhà, chấp hành các quy định của địa phương, hoàn tất mọi thủ tục giấy tờ đăng ký tạm trú cho bên B.");
                    col.Item().Text("- Đối tác trang thiết bị trong nhà phải có trách nhiệm bảo quản, nếu hỏng phải báo chủ nhà.");
                    col.Item().Text("- Không được tự ý chuyển nhượng, thêm người, cho thuê lại, sửa chữa, cải tạo nhà trọ khi chưa có sự đồng ý của bên A.");
                    col.Item().Text("- Trả lại nhà cho bên A đúng thời hạn hợp đồng, nếu bên B muốn tiếp tục thuê nhà thì phải báo trước cho bên A ít nhất 15 ngày trước khi hợp đồng hết hạn.");
                    col.Item().Text("- Nếu có hư hại về trang thiết bị liên quan đến bên B, bên B phải chịu trách nhiệm bồi thường thiệt hại theo quy giá hiện hành được đề ra trong phiếu hiện trạng nhận phòng.");
                    col.Item().Text("");
                    // Điều 5
                    col.Item().Text("");
                    col.Item().Text("");
                    col.Item().Text("");
                    col.Item().Text("ĐIỀU 5: ĐIỀU KHOẢN CHUNG.").Bold();
                    col.Item().PaddingTop(5).Text("Bên A và B thực hiện các điều khoản ghi trong hợp đồng.");
                    col.Item().Text("- Trường hợp có tranh chấp hoặc một bên vi phạm hợp đồng thì hai bên cùng nhau bàn bạc giải quyết, nếu không giải quyết được thì nhờ cơ quan có thẩm quyền giải quyết.");
                    col.Item().Text("- Hợp đồng này sẽ được thanh lý khi hết hạn hoặc khi hai bên tự thoả thuận hoặc bên B muốn gia hạn hợp đồng thì phải báo trước bên A ít nhất 20 ngày kể từ thời điểm kết thúc hợp đồng.");
                    col.Item().Text("- Hợp đồng được lập thành 02 bản mỗi bên giữ 01 bản có giá trị ngang nhau, mỗi bên giữ 01 bản, hai bên đã đọc, hiểu và đồng ý các điều khoản trên và ký tên dưới đây.");
                    col.Item().Text("");
                    col.Item().Text($"Thành phố Hồ Chí Minh, ngày {hopDong.NgayLap:dd} tháng {hopDong.NgayLap:MM} năm {hopDong.NgayLap:yyyy}").AlignRight();
                    col.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Text("BÊN A (BÊN CHO THUÊ)").AlignCenter().Bold();
                        row.RelativeItem().Text("BÊN B (BÊN THUÊ)").AlignCenter().Bold();
                    });
                    col.Item().PaddingTop(3).Row(row =>
                    {
                        row.RelativeItem().Text("Ký và ghi rõ họ tên.").AlignCenter().FontSize(8);
                        row.RelativeItem().Text("Ký và ghi rõ họ tên.").AlignCenter().FontSize(8);
                    });
                });
            });
        });
        return document.GeneratePdf();
    }
}
