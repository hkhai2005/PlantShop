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
                Đây là cấu trúc CSDL cho hệ thống Thuê xe Ô tô (CarRental):
	            - tb_Car(CarId PK, Title, CarCategoryId, Brand, Model, Seats, Price, PriceSale, Quantity, UnitInStock, IsActive, Star): Bảng (Table) chứa thông tin xe ô tô cho thuê.
	            - tb_CarCategory(CarCategoryId PK, Title, Description): Bảng (Table) chứa danh mục xe (ví dụ: 'Xe 4 chỗ', 'SUV', 'Xe điện').
	            - tb_CarImage(ImageId PK, CarId FK, ImageUrl, SortOrder): Bảng (Table) chứa hình ảnh chi tiết của xe.
	            - tb_CarReview(CarReviewId PK, CarId FK, Name, Detail, Star): Bảng (Table) chứa đánh giá của khách hàng về xe.
	            - tb_Booking(BookingId PK, Code, CustomerName, Phone, PickupLocation, ReturnLocation, StartDate, EndDate, TotalAmount, BookingStatusId FK): Bảng (Table) chứa thông tin đơn đặt xe (hợp đồng thuê).
	            - tb_BookingDetail(BookingDetailId PK, BookingId FK, CarId FK, Price, Quantity, Days, SubTotal): Bảng (Table) chi tiết đơn đặt xe (liên kết với xe được thuê).
	            - tb_BookingStatus(BookingStatusId PK, Name, Description): Bảng (Table) chứa trạng thái đơn đặt (ví dụ: 'Waiting', 'Confirmed', 'Completed').
	            - tb_Account(AccountId PK, Username, Password, FullName, Phone, Email, RoleId FK, IsActive): Bảng (Table) tài khoản quản trị/nhân viên.
	            - tb_Role(RoleId PK, RoleName, Description): Bảng (Table) quyền hạn (ví dụ: 'Admin', 'Staff').
	            - tb_Customer(CustomerId PK, Name, Phone, Email, CitizenId, Address, IsActive): Bảng (Table) khách hàng (có thể là khách vãng lai hoặc khách quen).
	            - tb_Category(CategoryId PK, Title, Description): Bảng (Table) danh mục chung (cho Tin tức và Blog).
	            - tb_News(NewsId PK, Title, Alias, CategoryId FK, Description, Detail): Bảng (Table) tin tức công ty.
	            - tb_Blog(BlogId PK, Title, Alias, CategoryId FK, Description, Detail, AccountId FK): Bảng (Table) bài viết Blog/Kinh nghiệm thuê xe.
	            - tb_BlogComment(CommentId PK, BlogId FK, Name, Detail): Bảng (Table) bình luận Blog.
	            - tb_Contact(ContactId PK, Name, Phone, Email, Message, IsRead): Bảng (Table) lưu trữ thông tin liên hệ của khách hàng.
	            - tb_Menu(MenuId PK, Title, ParentId FK, Levels, Position): Bảng (Table) cấu trúc Menu website.
	            - tb_Service(ServiceId PK, Title, Icon, Description): Bảng (Table) các dịch vụ cung cấp kèm theo.

                Các quan hệ (Relationship) chính:
                - tb_Car.CarCategoryId -> tb_CarCategory.CarCategoryId
                - tb_CarImage.CarId -> tb_Car.CarId
                - tb_CarReview.CarId -> tb_Car.CarId
                - tb_Booking.BookingStatusId -> tb_BookingStatus.BookingStatusId
                - tb_BookingDetail.BookingId -> tb_Booking.BookingId
                - tb_BookingDetail.CarId -> tb_Car.CarId
                - tb_Account.RoleId -> tb_Role.RoleId
                - tb_Blog.CategoryId -> tb_Category.CategoryId
                - tb_News.CategoryId -> tb_Category.CategoryId
                - tb_Blog.AccountId -> tb_Account.AccountId
                - tb_BlogComment.BlogId -> tb_Blog.BlogId
                - tb_Menu.ParentId -> tb_Menu.MenuId (Quan hệ tự tham chiếu cho Menu đa cấp)
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