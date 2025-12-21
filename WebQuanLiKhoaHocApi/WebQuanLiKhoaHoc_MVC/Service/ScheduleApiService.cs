using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebQuanLiKhoaHoc_MVC.Interface;
using WebQuanLiKhoaHoc_MVC.Models;

namespace WebQuanLiKhoaHoc_MVC.Service
{
    // 1. Converter xử lý giờ (TimeOnly)
    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return TimeOnly.Parse(value!);
        }
        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("HH:mm:ss"));
        }
    }

    // 2. Service Chính
    public class ScheduleApiService : IScheduleApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ScheduleApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiSchedule");

            // Cấu hình để đọc/ghi JSON (bao gồm xử lý TimeOnly)
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _jsonOptions.Converters.Add(new TimeOnlyConverter());
        }

        // --- 1. LẤY DANH SÁCH (GET) ---
        public async Task<List<StudentScheduleViewModel>> GetStudentScheduleAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("AdminApi/schedules");

                if (!response.IsSuccessStatusCode)
                    return new List<StudentScheduleViewModel>();

                var json = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(json))
                    return new List<StudentScheduleViewModel>();

                return JsonSerializer.Deserialize<List<StudentScheduleViewModel>>(json, _jsonOptions)
                       ?? new List<StudentScheduleViewModel>();
            }
            catch
            {
                return new List<StudentScheduleViewModel>();
            }
        }

        // --- 2. THÊM MỚI (POST) ---
        public async Task<bool> CreateScheduleAsync(StudentScheduleViewModel model)
        {
            try
            {
                // Serialize model thành JSON (dùng _jsonOptions để format TimeOnly chuẩn)
                var jsonContent = JsonSerializer.Serialize(model, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("AdminApi/schedules", content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // --- 3. CẬP NHẬT (PUT) ---
        public async Task<bool> UpdateScheduleAsync(int id, StudentScheduleViewModel model)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(model, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"AdminApi/schedules/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // --- 4. XÓA (DELETE) ---
        public async Task<bool> DeleteScheduleAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"AdminApi/schedules/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}