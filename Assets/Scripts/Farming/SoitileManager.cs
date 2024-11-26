using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using TMPro;
using System.Linq;

public class SoitileManager : MonoBehaviour
{
    [Header("TileBase")]
    //public TileBase dirtTile;       // Tile đất bình thường
    public TileBase[] dirtTile = new TileBase[5]; // Khởi tạo mảng với các phần tử cụ thể

    public TileBase dugTile;        // Tile đất đã đào
    public TileBase seededTile;     // Tile đất đã gieo hạt (để đánh dấu trên SoilLayer)
    //public TileBase plantSeedTile;  // Tile hạt giống trên PlantLayer
    public TileBase wateredSoilTile; // Tham chiếu đến Tile đậm hơn cho ô đất đã tưới

    [Header("TileMap")]
    private Tilemap soilTilemap;    // Tilemap cho đất (SoilLayer)
    public Tilemap plantTilemap;   // Tilemap cho cây trồng (PlantLayer)

    [Header("FruitDrop")]
    //public GameObject fruitPrefab; // Gán prefab của quả vào trong Inspector
    //public Transform fruitDropPosition;

    [Header("Fruit Prefabs")]
    public GameObject tomatoFruitPrefab; // Prefab cho quả Tomato
    public GameObject carrotFruitPrefab; // Prefab cho quả Carrot
    public GameObject potatoFruitPrefab; // Prefab cho quả Potato

    [Header("TileBase - Carrot")]
    public TileBase carrotSeedTile;
    public TileBase carrotSproutTile;
    public TileBase carrotYoungPlantTile;
    public TileBase carrotFruitTile;

    [Header("TileBase - Tomato")]
    public TileBase tomatoSeedTile;
    public TileBase tomatoSproutTile;
    public TileBase tomatoYoungPlantTile;
    public TileBase tomatoFruitTile;

    [Header("TileBase - Potato")]
    public TileBase potatoSeedTile;
    public TileBase potatoSproutTile;
    public TileBase potatoYoungPlantTile;
    public TileBase potatoFruitTile;

    [Header("FruitData")]
    //public TileBase sproutTile, youngPlantTile, fruitTile; // Các Tile cho từng giai đoạn
    private Dictionary<Vector3Int, CropTile> crops = new Dictionary<Vector3Int, CropTile>(); // Lưu trữ cây trồng trên Tilemap

    // Seed Selection Variables
    private string selectedSeed = null; // Currently selected seed type
    private Dictionary<string, SeedData> seedData; // Dictionary to store seed properties

    //public InventoryManager playerInventory;
    public GameObject highlightTile;
    public Transform player;            // Tham chiếu đối tượng Player
    public Player toolCheck;
    //public Animator anim;

    // Kích thước của một tile trong TileMap (giả sử kích thước tile là 1x1)
    public float tileSize = 1f;
    // Chỉnh sửa offset (có thể là 1 ô về một hướng nào đó)
    // Cập nhật offset khi Player di chuyển
    private Vector3Int offset = Vector3Int.zero;
    [Header("Weather/ WateredSoil")]
    public WeatherManager weatherManager;
    //private float timeSinceRainStopped;
    private float dryOutTime = 30f; // Thời gian để đất khô (đơn vị: giây)
    private TileBase[,] originalTileStates;
    // Biến thời gian để theo dõi thời gian sau khi mưa dừng
    private float timeSinceRainStopped = 0f;
    private bool isRaining = false;  // Theo dõi trạng thái mưa
    private float timeSinceWatered = 0f;
    private bool isWatered = false;

    
    void Start()
    {
        soilTilemap = GameObject.Find("SoilLayer").GetComponent<Tilemap>();
        plantTilemap = GameObject.Find("PlantLayer").GetComponent<Tilemap>();

        if (soilTilemap == null || plantTilemap == null)
        {
            Debug.LogError("Không tìm thấy Tilemap cho SoilLayer hoặc PlantLayer!");
        }

        // Khởi tạo dữ liệu hạt giống
        seedData = new Dictionary<string, SeedData>
        {
            { "Carrot Seed", new SeedData(carrotSeedTile,carrotSproutTile, carrotYoungPlantTile, carrotFruitTile,1) },
            { "Tomato Seed", new SeedData(tomatoSeedTile,tomatoSproutTile, tomatoYoungPlantTile, tomatoFruitTile,2) },
            { "Potato Seed", new SeedData(potatoSeedTile,potatoSproutTile, potatoYoungPlantTile, potatoFruitTile,1) },
        };
    }
  

    void Update()
    {
        //WateredTile duration
        WateredDuration();

        UpdateCrops(Time.deltaTime);

        if (toolCheck.isUsingTool)
        {
            Vector3 playerPosition = player.position;  // Lấy vị trí Player

            // Tính toán vị trí ô TileMap gần nhất với vị trí Player
            Vector3Int gridPos = soilTilemap.WorldToCell(playerPosition);

            // Xử lý input để thay đổi offset (di chuyển highlight theo các hướng)
            HandleInput();

            // Cộng thêm offset để highlight cách Player một ô
            gridPos += offset;

            // Gọi hàm highlight tile
            HighlightTile(gridPos);

            if (Input.GetKeyDown(KeyCode.F) && Player.instance != null)
            {
                // Kiểm tra xem người chơi có thể đào đất
                if (Player.instance.CanDig())
                {
                    TileBase currentTile = soilTilemap.GetTile(gridPos);
                    if (System.Array.Exists(dirtTile, tile => tile == currentTile))//currentTile == dirtTile)
                    {
                        PlayerController.instance.IsDigging();
                        Dig(gridPos);
                    }
                }

                // Kiểm tra xem người chơi có thể gieo hạt
                else if (Player.instance.CanPlantSeeds())
                {
                    if (selectedSeed == null)
                    {
                        Debug.Log("Please select a seed before planting.");
                        return;
                    }

                    TileBase currentTile = soilTilemap.GetTile(gridPos);
                    if (currentTile == dugTile)
                    {
                        PlantSeed(gridPos);
                    }
                    else
                    {
                        Debug.Log("Ô đất chưa được đào");
                    }
                }

                // Kiểm tra xem người chơi có thể tưới nước
                else if (Player.instance.CanWatering())
                {
                    TileBase currentTile = soilTilemap.GetTile(gridPos);
                    if (currentTile == seededTile)
                    {
                        //Player.instance.IsWatering();
                        PlayerController.instance.IsWatering();
                        WaterCrop(gridPos);
                    }
                    else
                    {
                        Debug.Log("Ô đất chưa có cây");
                    }
                }

                // Kiểm tra xem người chơi có thể thu hoạch
                else if (Player.instance.CanHarvest())
                {
                    TileBase currentTile = plantTilemap.GetTile(gridPos);
                    if (currentTile != null && crops.ContainsKey(gridPos) && crops[gridPos].isHarvestable)
                    {
                        Harvest(gridPos);
                    }
                    else
                    {
                        Debug.Log("Cây chưa thể thu hoạch hoặc không có cây để thu hoạch.");
                    }
                }
            }
        }
        else
        {
            // Nếu không cầm công cụ, ẩn highlightTile
            highlightTile.SetActive(false);
        }

        // Weather Effected
        if (weatherManager.isRaining)
        {
            isRaining = true;
            timeSinceRainStopped = 0f;


            // Lặp qua tất cả các vị trí trong tilemap (trong phạm vi giới hạn của tilemap)
            foreach (Vector3Int cellPosition in soilTilemap.cellBounds.allPositionsWithin)
            {
                // Kiểm tra nếu ô đất hiện tại có tồn tại một Tile
                TileBase currentTile = soilTilemap.GetTile(cellPosition);
                if (currentTile == null) continue; // Bỏ qua nếu không có Tile tại vị trí này

                // Kiểm tra nếu ô đất là dugTile hoặc seededTile
                if (currentTile == dugTile || currentTile == seededTile)
                {
                    // Cập nhật trạng thái watered cho cây trồng (nếu có) tại vị trí này
                    if (crops.ContainsKey(cellPosition))
                    {
                        CropTile crop = crops[cellPosition];
                        crop.isWatered = true;
                    }

                    // Đặt ô đất thành wateredSoilTile
                    soilTilemap.SetTile(cellPosition, wateredSoilTile);
                }
            }
        }
        else
        {
            if (isRaining)
            {
                // Trời đã ngừng mưa, bắt đầu đếm thời gian
                timeSinceRainStopped += Time.deltaTime;

                // Sau 30 giây (30 phút trong game), trả các ô về trạng thái ban đầu
                if (timeSinceRainStopped >= 30f)
                {
                    isRaining = false; // Đặt lại trạng thái mưa

                    foreach (Vector3Int cellPosition in soilTilemap.cellBounds.allPositionsWithin)
                    {
                        TileBase currentTile = soilTilemap.GetTile(cellPosition);
                        if (currentTile == null) continue;

                        // Trả các ô về trạng thái dugTile hoặc seededTile
                        if (currentTile == wateredSoilTile)
                        {
                            if (crops.ContainsKey(cellPosition) && crops[cellPosition].isPlanted)
                            {
                                soilTilemap.SetTile(cellPosition, seededTile); // Trả về seededTile nếu có cây trồng
                            }
                            else
                            {
                                soilTilemap.SetTile(cellPosition, dugTile); // Trả về dugTile nếu không có cây trồng
                            }
                        }
                    }
                }
            }
        }
    }


    // Hàm xử lý input của người chơi để thay đổi hướng offset
    private void HandleInput()
    {
        // Dùng các phím mũi tên hoặc WASD để thay đổi offset
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            offset = new Vector3Int(0, 1, 0);  // Di chuyển lên
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            offset = new Vector3Int(0, -1, 0); // Di chuyển xuống
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            offset = new Vector3Int(-1, 0, 0); // Di chuyển sang trái
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            offset = new Vector3Int(1, 0, 0);  // Di chuyển sang phải
        }
    }
    // Hàm highlight tile tại vị trí đã chọn
    private void HighlightTile(Vector3Int gridPos)
    {
        if (highlightTile != null)
        {
            // Chuyển Grid Position sang World Position
            Vector3 worldPos = soilTilemap.CellToWorld(gridPos);

            // Điều chỉnh vị trí để highlightTile khớp với ô
            worldPos.x += tileSize / 2;  // Điều chỉnh sao cho highlight khớp với tâm ô
            worldPos.y += tileSize / 2;  // Điều chỉnh sao cho highlight khớp với tâm ô

            // Đặt vị trí của highlightTile
            highlightTile.transform.position = worldPos;

            // Hiển thị highlightTile
            highlightTile.SetActive(true);
        }
    }

    private Vector3Int GetGridPosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = soilTilemap.WorldToCell(mouseWorldPos);
        return gridPosition;
    }

    void Dig(Vector3Int gridPos)
    {
        soilTilemap.SetTile(gridPos, dugTile);
        Debug.Log("Đã đào đất tại: " + gridPos);
    }

    public void SelectSeed(string seedName)
    {

        if (seedData.ContainsKey(seedName))
        {
            selectedSeed = seedName;
            Debug.Log("Đã chọn hạt giống: " + seedName);
        }
        else
        {
            Debug.Log("Hạt giống không hợp lệ!");
        }
    }

    // Thêm cây vào Tilemap khi gieo hạt
    public void PlantSeed(Vector3Int position)
    {
        if (selectedSeed == null || !seedData.ContainsKey(selectedSeed)) return;

        // Kiểm tra trong kho xem có đủ số lượng hạt giống không
        if (!InventoryManager.instance.HasItem(selectedSeed))
        {
            Debug.Log("Không có đủ hạt giống trong kho để gieo!");
            return;
        }
        // Kiểm tra nếu vị trí đã có cây để không bị thay thế
        if (crops.ContainsKey(position))
        {
            Debug.Log("Đã có cây trồng ở vị trí này. Không thể gieo hạt mới.");
            return;
        }
        SeedData seed = seedData[selectedSeed];

        // Giảm số lượng hạt giống trong kho
        InventoryManager.instance.RemoveItem(selectedSeed, 1);

        CropTile newCrop = new CropTile()
        {
            growthStage = 0,
            //hoursGrowth = 0f,

            isPlanted = true,
            isWatered = false,
            isHarvestable = false,
            seedName = selectedSeed // Lưu tên hạt giống để sử dụng khi cập nhật
        };
        crops[position] = newCrop;
        plantTilemap.SetTile(position, seed.seedTile); // Bắt đầu với Tile hạt giống

    }


    // Phương thức tưới nước cho cây
    public void WaterCrop(Vector3Int position)
    {
        if (crops.ContainsKey(position))
        {
            crops[position].isWatered = true;
            CropTile crop = crops[position];
            crop.hoursGrowth = 0f;
            isWatered = true;
            //crops[position].wateredTime = 0.5f; // Đặt thời gian tưới 
            timeSinceWatered = 0f;
            soilTilemap.SetTile(position, wateredSoilTile);
            Debug.Log($"Watered time set to: {timeSinceWatered}");
        }

    }
    public void WateredDuration()
    {
        if (isWatered)
        {
            // Trời đã ngừng mưa, bắt đầu đếm thời gian
            timeSinceWatered += Time.deltaTime;

            // Sau 30 giây (30 phút trong game), trả các ô về trạng thái ban đầu
            if (timeSinceWatered >= 30f)
            {
                isWatered = false; // Đặt lại trạng thái mưa

                foreach (Vector3Int cellPosition in soilTilemap.cellBounds.allPositionsWithin)
                {
                    TileBase currentTile = soilTilemap.GetTile(cellPosition);
                    if (currentTile == null) continue;

                    // Trả các ô về trạng thái dugTile hoặc seededTile
                    if (currentTile == wateredSoilTile)
                    {
                        if (crops.ContainsKey(cellPosition))
                        {
                            soilTilemap.SetTile(cellPosition, seededTile); // Trả về seededTile nếu có cây trồng
                        }
                    }
                }
            }
        }
    }

    // Hàm Harvest
    public void Harvest(Vector3Int gridPos)
    {
        if (!crops.ContainsKey(gridPos) || !crops[gridPos].isHarvestable)
        {
            Debug.Log("Cây chưa thể thu hoạch hoặc không có cây để thu hoạch.");
            return;
        }

        CropTile crop = crops[gridPos];
        GameObject fruitPrefab = null;

        // Chọn đúng Prefab của trái cây dựa trên loại hạt giống
        if (crop.seedName == "Tomato Seed")
        {
            fruitPrefab = tomatoFruitPrefab;
        }
        else if (crop.seedName == "Carrot Seed")
        {
            fruitPrefab = carrotFruitPrefab;
        }
        else if (crop.seedName == "Potato Seed")
        {
            fruitPrefab = potatoFruitPrefab;
        }

        if (fruitPrefab != null)
        {
            // Instantiate trái cây tại vị trí của ô đất
            Vector3 dropPosition = soilTilemap.CellToWorld(gridPos) + new Vector3(0.7f, 0.3f, 0);
            Instantiate(fruitPrefab, dropPosition, Quaternion.identity);
            Debug.Log($"+1 {crop.seedName} Fruit");
        }

        // Đặt lại tile trên plantTilemap về None để xóa cây
        plantTilemap.SetTile(gridPos, null);

        // Đặt lại tile trên soilTilemap thành dugTile để biểu thị đất đã đào
        soilTilemap.SetTile(gridPos, dugTile);

        // Xóa cây trồng khỏi danh sách crops
        crops.Remove(gridPos);

        Debug.Log("Thu hoạch thành công! Đất đã được trả về trạng thái đào.");
    }
    public void UpdateCrops(float deltaTime)
    {
        List<Vector3Int> positions = new List<Vector3Int>(crops.Keys);
        foreach (var position in positions)
        {
            CropTile crop = crops[position];
            SeedData seed = seedData[crop.seedName];

            // Nếu cây được tưới, cho phép tăng thời gian phát triển
            if (crop.isWatered)
            {
                crop.hoursGrowth += deltaTime;
               
                // Kiểm tra điều kiện chuyển giai đoạn phát triển
                if (crop.growthStage == 0 && crop.hoursGrowth >= 20f)
                {
                    crop.growthStage = 1;
                    crop.hoursGrowth = 0;
                    plantTilemap.SetTile(position, seed.sproutTile);
                    crop.isWatered = false; // Cần tưới lại cho giai đoạn mới
                }
                else if (crop.growthStage == 1 && crop.hoursGrowth >= 30f)
                {
                    crop.growthStage = 2;
                    crop.hoursGrowth = 0;
                    plantTilemap.SetTile(position, seed.youngPlantTile);
                    crop.isWatered = false; // Cần tưới lại cho giai đoạn mới
                }
                else if (crop.growthStage == 2 && crop.hoursGrowth >= 40f)
                {
                    crop.growthStage = 3;
                    crop.hoursGrowth = 0;
                    plantTilemap.SetTile(position, seed.fruitTile);
                    crop.isHarvestable = true;
                    crop.isWatered = false; // Đặt lại trạng thái tưới
                }
            }
        }
    }
   
       


}
    public class SeedData
    {
        public TileBase seedTile;
        public TileBase sproutTile;
        public TileBase youngPlantTile;
        public TileBase fruitTile;
        public int timeGrowth;

        public SeedData(TileBase seedTile, TileBase sproutTile, TileBase youngPlantTile, TileBase fruitTile, int timeGrowth)
        {
            this.seedTile = seedTile;
            this.sproutTile = sproutTile;
            this.youngPlantTile = youngPlantTile;
            this.fruitTile = fruitTile;
            this.timeGrowth = timeGrowth;
            this.timeGrowth = timeGrowth;
        }
    }









