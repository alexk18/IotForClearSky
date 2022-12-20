using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IotForClearSky
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        private async void button1_Click(object sender, EventArgs e)
        {
            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("http://192.168.0.173:5170/");

            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            const string login = "ad@gmail.com";

            const string password = "admin";

            HttpResponseMessage request = await httpClient.PostAsync("/api/auth/login",
                new StringContent(JsonSerializer.Serialize(new Login { email = login, password = password }),
                    Encoding.UTF8, "application/json"));

            string stringResult = await request.Content.ReadAsStringAsync();
            AuthorizeResponse testing = JsonSerializer.Deserialize<AuthorizeResponse>(stringResult, options);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + testing.accessToken);
            var updateOBJ = new MeasurementDTO()
            {
                Id = 7,
                Value = new Random().NextDouble(1,200),
                Type = MeasurementType.SALT,
                Date = DateTime.Now,
                DeviceId = 8
            };
            var rezalupa = await httpClient.PutAsync($"api/measurements",
                new StringContent(JsonSerializer.Serialize(updateOBJ), Encoding.UTF8, "application/json"));

        }
    }
    public class Login
    {
        public string email { set; get; } = string.Empty;
        public string password { set; get; } = string.Empty;
    }
    public class AuthorizeResponse
    {
        [CategoryAttribute("UserAttribute"), DescriptionAttribute("IsAuthSuccessful")]
        public string accessToken { get; set; }
    }

    public class MeasurementDTO
    {
        public int Id { get; set; }

        public double Value { get; set; }

        public MeasurementType Type { get; set; }

        public string TypeString => Type.ToString();

        public DateTime Date { get; set; }

        public int DeviceId { get; set; }

    }

    public enum MeasurementType
    {
        PH,
        ORP,
        EC,
        TDS,
        SALT,
        СO2
    }
    public static class RandomExtensions
    {
        public static double NextDouble(
            this Random random,
            double minValue,
            double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
}



