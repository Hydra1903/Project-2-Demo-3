using UnityEngine;

public class CropTile
{
    public int growthStage = 0; // 0 = hạt giống, 1 = nảy mầm, 2 = cây non, 3 = ra quả
    public bool isWatered = false;
    public float hoursGrowth = 0f; // Thời gian phát triển (hours) trong mỗi giai đoạn = mỗi 1h phải tưới ít nhất 1 lần
    //public float wateredTime; // Thời gian tính bằng giờ in-game
    public bool isHarvestable; // Cờ kiểm tra có thể thu hoạch được không
    public bool isPlanted;
    public string seedName;
    
    
    // Constructor nếu cần
    public CropTile()
    {
        isWatered = false;
        //wateredTime = 0;
        growthStage = 0;
        isHarvestable = false;
    }
}
