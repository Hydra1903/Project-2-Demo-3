using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public bool isRaining; // Biến công khai để các script khác có thể kiểm tra trạng thái thời tiết
    public float rainDuration; // Thời gian mưa kéo dài
    public float minRainDuration = 10f; // Thời gian mưa tối thiểu
    public float maxRainDuration = 30f; // Thời gian mưa tối đa
    public float rainProbability = 0.3f; // Xác suất xảy ra mưa (30%)

    private float nextWeatherCheck; // Thời gian kiểm tra thời tiết tiếp theo

    // Tham chiếu đến Particle System để bật/tắt hiệu ứng mưa
    public ParticleSystem rainEffect;

    void Start()
    {
        // Tắt hiệu ứng mưa khi bắt đầu game
        if (rainEffect != null)
        {
            rainEffect.Stop();
            rainEffect.Clear();
        }

        isRaining = false;
        ScheduleNextWeatherCheck();
    }

    void Update()
    {
        // Kiểm tra thời tiết định kỳ
        if (Time.time >= nextWeatherCheck)
        {
            CheckWeather();
            ScheduleNextWeatherCheck();
        }

        // Xử lý logic khi trời mưa
        if (isRaining)
        {
            rainDuration -= Time.deltaTime;

            if (rainDuration <= 0)
            {
                StopRain();
            }
        }
    }

    private void CheckWeather()
    {
        // Xác suất xảy ra mưa
        if (Random.value < rainProbability)
        {
            StartRain();
        }
    }

    private void StartRain()
    {
        isRaining = true;
        rainDuration = Random.Range(minRainDuration, maxRainDuration);

        if (rainEffect != null)
        {
            rainEffect.Play();
        }

        Debug.Log("Mưa bắt đầu, kéo dài trong: " + rainDuration + " giây.");
    }

    private void StopRain()
    {
        isRaining = false;

        if (rainEffect != null)
        {
            rainEffect.Stop();
            rainEffect.Clear();
        }

        Debug.Log("Mưa đã tạnh.");
    }

    private void ScheduleNextWeatherCheck()
    {
        // Đặt thời gian kiểm tra thời tiết tiếp theo trong khoảng 5 đến 15 giây
        nextWeatherCheck = Time.time + Random.Range(30f, 45f);
    }
}


