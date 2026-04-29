using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.Encodings.Web;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // Dùng (Use) IConfiguration
using System.Net.Http; // Dùng (Use) IHttpClientFactory

// !!! Đảm bảo (ensure) đây là namespace (không gian tên) DỰ ÁN CỦA BẠN
namespace PlantShop.Controllers
{
    public class UserQuery
    {
        public string Question { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly string _geminiApiKey;
        private readonly string _connectionString;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _geminiApiKey = config["GeminiApiKey"];
            _connectionString = config.GetConnectionString("DefaultConnection");
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] UserQuery query)
        {
            try
            {
                // === 1. MÔ TẢ DATABASE (Đã lấy CSDL (database) của bạn) ===
                string dbSchema = @"
Đây là cấu trúc CSDL cho hệ thống Bán cây cảnh trực tuyến (PlantShop):
- tb_Role(RoleId PK, RoleName, Description): Bảng (Table) chứa quyền hạn của tài khoản quản trị (ví dụ: 'Admin', 'Manager', 'Sales').
- tb_Account(AccountId PK, Username, Password, FullName, Phone, Email, RoleId FK, LastLogin, IsActive): Bảng (Table) chứa tài khoản quản trị/nhân viên quản lý website.
- tb_Customer(CustomerId PK, Username, Password, FullName, Birthday, Avatar, Phone, Email, Address, LastLogin, IsActive, Gender, Ward, District, Province, VerifyEmail, VerifyPhone, ResetPasswordToken, TokenExpiry): Bảng (Table) chứa thông tin khách hàng mua sắm trên website.
- tb_ProductCategory(CategoryProductId PK, Title, Alias, Description, Icon, Position, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, IsActive): Bảng (Table) chứa danh mục sản phẩm cây cảnh (ví dụ: 'Cây để bàn', 'Cây phong thủy', 'Cây nội thất').
- tb_Product(ProductId PK, Title, Alias, CategoryProductId FK, Description, Detail, Image, Price, PriceSale, Quantity, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, IsNew, IsBestSeller, IsFeatured, UnitInStock, IsActive, Star, SKU, Weight, Dimensions, Views, SoldCount, Tags): Bảng (Table) chứa thông tin sản phẩm cây cảnh.
- tb_ProductImage(ProductImageId PK, ProductId FK, ImageUrl, IsDefault, Position, CreatedDate): Bảng (Table) chứa ảnh phụ của sản phẩm (mỗi sản phẩm có nhiều ảnh).
- tb_Attribute(AttributeId PK, Name, Unit): Bảng (Table) chứa danh sách thuộc tính sản phẩm (ví dụ: 'Chiều cao', 'Đường kính chậu', 'Màu sắc').
- tb_ProductAttribute(ProductAttributeId PK, ProductId FK, AttributeId FK, Value): Bảng (Table) chứa giá trị thuộc tính của từng sản phẩm (quan hệ nhiều-nhiều).
- tb_ProductReview(ProductReviewId PK, Name, Phone, Email, CreatedDate, Detail, Star, ProductId FK, IsActive, CustomerId FK, IsApproved, Reply, ReplyDate): Bảng (Table) chứa đánh giá của khách hàng về sản phẩm.
- tb_Cart(CartId PK, CustomerId FK, SessionId, CreatedDate, ModifiedDate): Bảng (Table) chứa giỏ hàng của khách (hỗ trợ cả khách đăng nhập và chưa đăng nhập).
- tb_CartDetail(CartDetailId PK, CartId FK, ProductId FK, Quantity, Price): Bảng (Table) chi tiết giỏ hàng (sản phẩm được thêm vào giỏ).
- tb_Coupon(CouponId PK, Code, Name, DiscountType, DiscountValue, MinOrderAmount, MaxDiscount, StartDate, EndDate, UsageLimit, UsedCount, IsActive, CreatedDate): Bảng (Table) chứa mã giảm giá/khuyến mãi.
- tb_OrderStatus(OrderStatusId PK, Name, Description): Bảng (Table) chứa trạng thái đơn hàng (ví dụ: 'Chờ xác nhận', 'Đã xác nhận', 'Đang giao hàng', 'Đã giao').
- tb_Order(OrderId PK, Code, CustomerId FK, CustomerName, Phone, Address, TotalAmount, Quantity, OrderStatusId FK, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, PaymentMethod, PaymentStatus, ShippingFee, DiscountAmount, Note, CancelReason, CanceledDate): Bảng (Table) chứa thông tin đơn hàng.
- tb_OrderDetail(OrderDetailId PK, OrderId FK, ProductId FK, Price, Quantity): Bảng (Table) chi tiết đơn hàng (sản phẩm được mua).
- tb_OrderCoupon(OrderCouponId PK, OrderId FK, CouponId FK, DiscountAmount): Bảng (Table) liên kết mã giảm giá với đơn hàng.
- tb_Shipping(ShippingId PK, OrderId FK, ShipperName, TrackingNumber, ShippingFee, EstimatedDelivery, ActualDelivery, Status, Note, CreatedDate): Bảng (Table) chứa thông tin vận chuyển của đơn hàng.
- tb_Wishlist(WishlistId PK, CustomerId FK, ProductId FK, CreatedDate): Bảng (Table) chứa danh sách sản phẩm yêu thích của khách hàng.
- tb_Category(CategoryId PK, Title, Alias, Description, Position, SeoTitle, SeoDescription, SeoKeywords, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy): Bảng (Table) chứa danh mục bài viết Blog (Tin tức, Chăm sóc cây, Phong thủy...).
- tb_Blog(BlogId PK, Title, Alias, CategoryId FK, Description, Detail, Image, SeoTitle, SeoDescription, SeoKeywords, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, AccountId FK, IsActive): Bảng (Table) chứa bài viết Blog về cây cảnh.
- tb_BlogComment(CommentId PK, Name, Phone, Email, CreatedDate, Detail, BlogId FK, IsActive): Bảng (Table) chứa bình luận của người dùng trên bài viết Blog.
- tb_Contact(ContactId PK, Name, Phone, Email, Message, IsRead, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy): Bảng (Table) lưu trữ thông tin liên hệ của khách hàng.
- tb_Menu(MenuId PK, Title, Alias, Description, Levels, ParentId FK, Position, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, IsActive): Bảng (Table) cấu trúc Menu website (hỗ trợ menu đa cấp).
- tb_Notification(NotificationId PK, CustomerId FK, Title, Content, Link, IsRead, CreatedDate): Bảng (Table) chứa thông báo gửi đến khách hàng.
- tb_ActivityLog(LogId PK, AccountId FK, CustomerId FK, Action, IPAddress, UserAgent, CreatedDate): Bảng (Table) ghi nhật ký hoạt động của người dùng (Login, Logout, CreateOrder...).

Các quan hệ (Relationship) chính:
- tb_Account.RoleId -> tb_Role.RoleId
- tb_Customer (không có khóa ngoại trực tiếp, độc lập)
- tb_Product.CategoryProductId -> tb_ProductCategory.CategoryProductId
- tb_ProductImage.ProductId -> tb_Product.ProductId
- tb_ProductAttribute.ProductId -> tb_Product.ProductId
- tb_ProductAttribute.AttributeId -> tb_Attribute.AttributeId
- tb_ProductReview.ProductId -> tb_Product.ProductId
- tb_ProductReview.CustomerId -> tb_Customer.CustomerId
- tb_Cart.CustomerId -> tb_Customer.CustomerId
- tb_CartDetail.CartId -> tb_Cart.CartId
- tb_CartDetail.ProductId -> tb_Product.ProductId
- tb_Order.CustomerId -> tb_Customer.CustomerId
- tb_Order.OrderStatusId -> tb_OrderStatus.OrderStatusId
- tb_OrderDetail.OrderId -> tb_Order.OrderId
- tb_OrderDetail.ProductId -> tb_Product.ProductId
- tb_OrderCoupon.OrderId -> tb_Order.OrderId
- tb_OrderCoupon.CouponId -> tb_Coupon.CouponId
- tb_Shipping.OrderId -> tb_Order.OrderId
- tb_Wishlist.CustomerId -> tb_Customer.CustomerId
- tb_Wishlist.ProductId -> tb_Product.ProductId
- tb_Blog.CategoryId -> tb_Category.CategoryId
- tb_Blog.AccountId -> tb_Account.AccountId
- tb_BlogComment.BlogId -> tb_Blog.BlogId
- tb_Menu.ParentId -> tb_Menu.MenuId (Quan hệ tự tham chiếu cho Menu đa cấp)
- tb_Notification.CustomerId -> tb_Customer.CustomerId
- tb_ActivityLog.AccountId -> tb_Account.AccountId
- tb_ActivityLog.CustomerId -> tb_Customer.CustomerId
";

                // === 2. PROMPT (CÂU LỆNH) YÊU CẦU GEMINI SINH JSON (Một định dạng dữ liệu) (PHIÊN BẢN (VERSION) MỚI (NEW)) ===
                string finalPrompt = $@"
                Bạn là một chatbot (chatbot) trợ lý thông minh và thân thiện 💖.
                
                Nhiệm vụ của bạn là phân loại câu hỏi của người dùng (user): ""{query.Question}""
                
                1. NẾU câu hỏi có vẻ liên quan đến CSDL (database) (ví dụ (example): 'giá sản phẩm (product)', 'có bao nhiêu đơn hàng (order)', 'khách hàng tên An'), hãy trả lời (answer) bằng JSON (Một định dạng dữ liệu):
                {{
                  ""type"": ""sql"",
                  ""query"": ""SELECT ... ""
                }}
                (LƯU Ý SQL: KHÔNG cho phép UPDATE/DELETE/INSERT. Nếu họ cố tình yêu cầu → trả: {{ ""type"": ""sql"", ""query"": ""ERROR_MODIFICATION"" }})

                2. NẾU câu hỏi là chat thông thường (ví dụ (example): 'chào bạn', 'bạn khỏe không', 'thời tiết hôm nay thế nào', 'kể chuyện cười'),
                   hãy TỰ BẠN trả lời (answer) câu đó một (1) cách tự nhiên, thân thiện, và thêm (add) icon.
                   VÀ trả lời (answer) bằng JSON (Một định dạng dữ liệu):
                {{
                  ""type"": ""chat"",
                  ""response"": ""[Câu trả lời (answer) do bạn tự nghĩ ra ở đây]""
                }}

                Dưới đây là mô tả CSDL (database) (chỉ dùng (use) cho Lựa chọn 1):
                {dbSchema}
                ";

                // === 3. Gọi Gemini API ===
                string geminiJsonResponse = await CallGeminiApi(finalPrompt); // Bây giờ 'finalPrompt' đã tồn tại

                // === 4. Parse (Phân tích) JSON (Một định dạng dữ liệu) từ Gemini ===
                using (JsonDocument doc = JsonDocument.Parse(geminiJsonResponse))
                {
                    JsonElement root = doc.RootElement;
                    string responseType = root.GetProperty("type").GetString();

                    if (responseType == "sql")
                    {
                        string sqlQuery = root.GetProperty("query").GetString();

                        // 1. Kiểm tra (Check) an toàn (safe) (Giữ nguyên)
                        if (string.IsNullOrWhiteSpace(sqlQuery) ||
                            sqlQuery.StartsWith("ERROR_") ||
                            !sqlQuery.Trim().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                        {
                            // Trả lời (answer) thân thiện khi AI báo lỗi (error)
                            return Ok(new { chat = $"Ui, em không (no) thể chạy (run) câu lệnh (command) đó được ạ 😥 (Lỗi: {sqlQuery})" });
                        }

                        // 2. Lấy dữ liệu (data) thô (raw) từ CSDL (database) (Giữ nguyên)
                        object sqlData; // Dùng (Use) 'object' để linh hoạt
                        using (var connection = new SqlConnection(_connectionString))
                        {
                            sqlData = await connection.QueryAsync(sqlQuery);
                        }

                        // === 3. NÂNG CẤP (UPGRADE): GỌI GEMINI LẦN 2 ===
                        // Chuyển dữ liệu (data) thô (raw) (data) thành một (1) chuỗi JSON (Một định dạng dữ liệu)
                        string jsonData = JsonSerializer.Serialize(sqlData,
                            new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

                        // Tạo một (1) prompt (câu lệnh) "thân thiện" mới (new)
                        string friendlyPrompt = $@"
                        Một người dùng (user) đã hỏi em câu này: ""{query.Question}""
                        
                        Em đã truy vấn CSDL (database) và có kết quả (dưới dạng JSON (Một định dạng dữ liệu)) như sau:
                        {jsonData}

                        Nhiệm vụ của chị (AI):
                        - Đọc dữ liệu (data) JSON (Một định dạng dữ liệu) trên.
                        - Trả lời (answer) câu hỏi của người dùng (user) bằng tiếng Việt, một (1) cách thật TỰ NHIÊN, THÂN THIỆN (như người (person) chat bình thường).
                        - **Thêm (Add) các icon 'cute' (dễ thương) 💖📱💻 vào câu trả lời (answer).**
                        - **KHÔNG** dùng (use) các ký tự `{{`, `[`, `""`.
                        - Nếu JSON (Một định dạng dữ liệu) là `[]` (rỗng), hãy báo là 'Dạ, em không (no) tìm thấy gì hết 😥'.
                        - Chỉ trả về nội dung (content) câu trả lời (answer), không (no) thêm (add) gì khác.
                        ";

                        // 4. Gọi CallGeminiApi LẦN NỮA
                        string friendlyText = await CallGeminiApi(friendlyPrompt);

                        // 5. Trả về câu trả lời (answer) thân thiện
                        return Ok(new { chat = friendlyText });
                    }
                    else if (responseType == "chat")
                    {
                        string text = root.GetProperty("response").GetString();
                        return Ok(new { chat = text });
                    }
                }
                return BadRequest("Dữ liệu (data) AI trả về không (no) hợp lệ.");
            }
            catch (Exception ex)
            {
                // Trả về lỗi (error) chi tiết để bạn dễ debug (gỡ lỗi) hơn
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }

        private async Task<string> CallGeminiApi(string prompt)
        {
            var httpClient = _httpClientFactory.CreateClient();

            // Dùng (Use) đúng model (mô hình) `v1` và `gemini-2.5-flash`
            // Đã xóa (delete) API key (khóa API) khỏi URL (đường dẫn)
            string geminiUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent";

            // Gửi Key (Khóa) an toàn (safe) qua Header (Tiêu đề)
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-goog-api-key", _geminiApiKey);

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var jsonOpt = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            // Đã sửa (fix) lỗi gõ phím (sửa (fix) "url" thành "geminiUrl")
            var response = await httpClient.PostAsJsonAsync(geminiUrl, requestBody, jsonOpt);

            string raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Trả về lỗi (error) chi tiết từ Google
                throw new Exception($"Gemini API Error: {response.StatusCode} - {raw}");
            }

            // Parse (Phân tích) chuẩn output (đầu ra) v1
            JsonDocument doc = JsonDocument.Parse(raw);
            string aiText = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString()
                .Trim();

            // Nếu AI trả về dạng ```json ... ``` (cần dọn dẹp (clean) nó)
            if (aiText.StartsWith("```"))
            {
                aiText = aiText.Trim('`', '\n', '\r');
                aiText = aiText.Replace("json", "").Trim();
            }

            return aiText;
        }
    }
}