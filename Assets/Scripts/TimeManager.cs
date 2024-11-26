using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    public  int hours = 6;
    public  int minutes = 0;
    public  int days = 1;
    public  int season = 1; // 0 = Spring, 1 = Summer, 2 = Fall, 3 = Winter
    public  int year = 1;

    public SoitileManager cropManager;


    private float timeMultiplier = 60f;
    private float timer = 0f;

    //public Text timeText;

    public Light2D globalLight; // Ánh sáng toàn cảnh
    public Light2D playerLight; // Ánh sáng theo nhân vật

    // Mảng tên các mùa
    private string[] seasonNames = { "Spring", "Summer", "Fall", "Winter" };

    public static TimeManager instance;

    private void Awake()
    {
        // Thiết lập Singleton cho TimeManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ TimeManager khi chuyển Scene
        }
        else
        {
            Destroy(gameObject); // Xóa đối tượng mới nếu instance đã tồn tại
        }
    }

    private void Update()
    {
        UpdateTime();
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateTimeDisplay(GetCurrentTime());
        }
    }

    void UpdateTime()
    {
        timer += Time.deltaTime * timeMultiplier;

        if (timer >= 60f)
        {
            timer = 0f;
            minutes++;
            if (minutes >= 60)
            {
                minutes = 0;
                hours++;

               // cropManager.UpdateCrops(1.0f); // Gọi trong TimeManager khi tăng 1 giờ in-game

                if (hours >= 24)
                {
                    hours = 0;
                    days++;
                    if (ShopPanel.instance != null)
                    {
                        ShopPanel.instance.ResetDailyStock();
                    }
                    else
                    {
                        Debug.LogError("ShopPanel.instance là null. Đảm bảo ShopPanel đã được khởi tạo.");
                    }
                    if (days > 30)
                    {
                        days = 1;
                        season++;

                        if (season >= 4)
                        {
                            season = 0;
                            year++;
                        }
                    }
                }
            }
        }

        // Cập nhật Text UI
        //if (timeText != null)
        //{
        //timeText.text = $"Time: {hours:00}:{minutes:00} | Day: {days} | Season: {seasonNames[season]} | Year: {year}";
        //}
        // Cập nhật ánh sáng dựa trên giờ
        if (hours >= 18 || hours < 6)  // Thời gian tối
        {
            globalLight.intensity = 0.01f;  // Độ sáng rất thấp
            globalLight.color = Color.cyan;
            playerLight.enabled = true;     // Bật ánh sáng quanh nhân vật
        }
        else if (hours >= 6 && hours < 12)  // Buổi sáng (7h - 12h)
        {
            globalLight.intensity = 1.2f;  // Độ sáng trung bình
            globalLight.color = new Color(1f, 1f, 0.9f); // Màu sáng trắng nhạt
            playerLight.enabled = false;   // Tắt ánh sáng quanh nhân vật
        }
        else if (hours >= 12 && hours < 16) // Buổi trưa (12h - 16h)
        {
            globalLight.intensity = 1.8f;    // Độ sáng cao
            globalLight.color = new Color(1f, 0.95f, 0.6f); // Màu vàng nắng gắt
            playerLight.enabled = false;   // Tắt ánh sáng quanh nhân vật
        }
        else if (hours >= 16 && hours < 18) // Xế chiều (16h - 18h)
        {
            globalLight.intensity = 1.5f;  // Độ sáng giảm
            globalLight.color = new Color32(217, 95, 140, 255);
            playerLight.enabled = false;   // Tắt ánh sáng quanh nhân vật
        }
    }
    private string GetCurrentTime()
    {
        return string.Format($"Time: {hours:00}:{minutes:00} | Day: {days} | Season: {seasonNames[season]} | Year: {year}");
    }
}


