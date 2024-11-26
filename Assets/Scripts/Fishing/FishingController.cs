using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class FishingController : MonoBehaviour
{
    //public GameObject fishingRod;
    public Transform fishingSpot;
    public float fishingTime = 5f;
    public GameObject fishingResultPanel;
    public Image fishImage;
    public TextMeshProUGUI fishInfoText;
    public Slider fishingSlider;
    //public Image exclamationMark;
    public RectTransform fishIcon;
    public RectTransform greenBar;
    public Slider progressBar;
    public float fishSpeed;

    private bool isFishing = false;
    private bool isFishCaught = false;
    //private Animator animator;

    private InventoryManager inventory;
    public List<GameObject> fishPrefabs; // List chứa các Prefab của các loại cá
    void Start()
    {
        //fishingRod.SetActive(false);
        fishingResultPanel.SetActive(false);
        fishingSlider.gameObject.SetActive(false);
        //exclamationMark.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
        //animator = GetComponent<Animator>();

        if (fishingResultPanel == null) Debug.LogError("Fishing Result Panel is not assigned.");
        if (fishImage == null) Debug.LogError("Fish Image is not assigned.");
        if (fishInfoText == null) Debug.LogError("Fish Info Text is not assigned.");
        if (fishingSlider == null) Debug.LogError("Fishing Slider is not assigned.");
        if (fishIcon == null) Debug.LogError("Fish Icon is not assigned.");
        if (greenBar == null) Debug.LogError("Green Bar is not assigned.");
        if (progressBar == null) Debug.LogError("Progress Bar is not assigned.");
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F) && !isFishing && IsNearWater() && Player.instance.CanFishing())
        {
            if (InventoryManager.instance.HasItem("Spilua Bait"))
            {
                StartCoroutine(Fish());
                InventoryManager.instance.RemoveItem("Spilua Bait", 1);
            }
            else Debug.Log("Không đủ mồi câu");
        }
    }

    private bool IsNearWater()
    {
        return Vector2.Distance(transform.position, fishingSpot.position) < 2f;
    }
    //
    private FishData SelectFish()
    {
        float totalCatchChance = 0f;
        foreach (GameObject fishPrefab in fishPrefabs)
        {
            FishInstance fishInstance = fishPrefab.GetComponent<FishInstance>();
            if (fishInstance != null && fishInstance.fishData != null)
            {
                totalCatchChance += fishInstance.fishData.catchChance;
            }
        }

        float roll = Random.Range(0f, totalCatchChance);
        float cumulative = 0f;

        foreach (GameObject fishPrefab in fishPrefabs)
        {
            FishInstance fishInstance = fishPrefab.GetComponent<FishInstance>();
            if (fishInstance != null && fishInstance.fishData != null)
            {
                cumulative += fishInstance.fishData.catchChance;
                if (roll <= cumulative)
                {
                    return fishInstance.fishData;
                }
            }
        }
        return null; // Không chọn được cá
    }

    private IEnumerator Fish()
    {
        isFishing = true;
        isFishCaught = false;
        //fishingRod.SetActive(true);
        fishingResultPanel.SetActive(false);
        //animator.SetBool("isFishing", true);
        //animator.SetTrigger("Cast");

        // yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        //animator.SetTrigger("Hold");

        //exclamationMark.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        ////exclamationMark.gameObject.SetActive(false);

        fishingSlider.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(true);

        //yield return StartCoroutine(FishingMinigame());
        // Chọn cá trước khi bắt đầu minigame
        FishData selectedFish = SelectFish();

        // Kiểm tra nếu chọn được cá
        if (selectedFish != null)
        {
            // Truyền thông tin cá vào minigame
            yield return StartCoroutine(FishingMinigame(selectedFish));
        }

        //animator.SetTrigger("Reel");
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        //fishingRod.SetActive(false);
        isFishing = false;
        //animator.SetBool("isFishing", false);
        fishingSlider.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);

        if (isFishCaught)
        {
            CatchFish();
        }
    }
    //
    private float GetFishSpeed(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 0.0025f;    // Tốc độ chậm
            case Rarity.Uncommon: return 0.0037f; // Tốc độ trung bình
            case Rarity.Rare: return 0.005f;     // Tốc độ nhanh
            case Rarity.Legendary: return 0.006f; // Tốc độ rất nhanh
            default: return 0f; //0.003f;              // Mặc định
        }
    }

    //
    public IEnumerator FishingMinigame(FishData currentFishData)
    {
        // Điều chỉnh tốc độ cá dựa trên độ hiếm
        fishSpeed = GetFishSpeed(currentFishData.rarity);
        float fishPosition = 0.5f;
        bool directionUp = true;
      

        float greenBarPosition = 0.5f;
        float greenBarSpeed = 0.4f;

        float catchProgress = 0f;
        float requiredCatchProgress = 3f;

        float lowProgressTimer = 0f;
        float lowProgressThreshold = 3f; // Thời gian tối đa cho phép khi thanh quá trình ở mức thấp nhất

        while (catchProgress < requiredCatchProgress)
        {

            // Di chuyển hình cá lên và xuống ngẫu nhiên
            if (directionUp)
            {
                fishPosition += fishSpeed;
                if (fishPosition >= 1) directionUp = false;
            }
            else
            {
                fishPosition -= fishSpeed;
                if (fishPosition <= 0) directionUp = true;
            }

            // Tính toán vị trí mới của fishIcon
            float newFishPositionY = Mathf.Lerp(fishingSlider.GetComponent<RectTransform>().rect.min.y, fishingSlider.GetComponent<RectTransform>().rect.max.y, fishPosition);

            fishIcon.anchoredPosition = new Vector2(fishIcon.anchoredPosition.x, newFishPositionY);

            // Điều chỉnh vị trí của hình vuông xanh khi người chơi nhấn phím Space
            if (Input.GetKey(KeyCode.E))
            {
                greenBarPosition += greenBarSpeed * Time.deltaTime;
            }
            else
            {
                greenBarPosition -= greenBarSpeed * Time.deltaTime;
            }

            greenBarPosition = Mathf.Clamp(greenBarPosition, 0f, 1f);

            // Tính toán vị trí mới của greenBar
            float newGreenBarPositionY = Mathf.Lerp(fishingSlider.GetComponent<RectTransform>().rect.min.y, fishingSlider.GetComponent<RectTransform>().rect.max.y, greenBarPosition);

            greenBar.anchoredPosition = new Vector2(greenBar.anchoredPosition.x, newGreenBarPositionY);

            // Kiểm tra nếu hình vuông xanh trùng với hình cá
            float fishMin = fishPosition - 0.1f;
            float fishMax = fishPosition + 0.1f;

            if (greenBarPosition >= fishMin && greenBarPosition <= fishMax)
            {
                catchProgress += Time.deltaTime;
            }
            else
            {
                catchProgress -= Time.deltaTime;
                if (catchProgress < 0) catchProgress = 0;
            }

            // Kiểm tra nếu thanh quá trình ở mức thấp nhất
            if (catchProgress <= 0.1f)
            {
                lowProgressTimer += Time.deltaTime;
                if (lowProgressTimer >= lowProgressThreshold)
                {
                    // Thất bại khi thanh quá trình ở mức thấp nhất quá 2-3 giây
                    break;
                }
            }
            else
            {
                lowProgressTimer = 0f; // Đặt lại timer nếu thanh quá trình không ở mức thấp nhất
            }

            progressBar.value = catchProgress / requiredCatchProgress;

            yield return null;
        }

        if (catchProgress >= requiredCatchProgress)
        {
            isFishCaught = true;
        }
        else
        {
            isFishCaught = false; // Đặt lại trạng thái câu cá thất bại
        }
    }

    private void CatchFish()
    {
        // Tính tổng tỷ lệ bắt cho toàn bộ cá
        float totalCatchChance = 0f;
        foreach (GameObject fishPrefab in fishPrefabs)
        {
            FishInstance fishInstance = fishPrefab.GetComponent<FishInstance>();
            if (fishInstance != null && fishInstance.fishData != null)
            {
                totalCatchChance += fishInstance.fishData.catchChance;
            }
        }

        // Tạo ngẫu nhiên một số trong khoảng từ 0 đến tổng catchChance
        float roll = Random.Range(0f, totalCatchChance);
        float cumulative = 0f;

        FishData caughtFishData = null;
        GameObject caughtFishPrefab = null;

        // Duyệt qua các loài cá để chọn loại có xác suất phù hợp
        foreach (GameObject fishPrefab in fishPrefabs)
        {
            FishInstance fishInstance = fishPrefab.GetComponent<FishInstance>();
            if (fishInstance != null && fishInstance.fishData != null)
            {
                cumulative += fishInstance.fishData.catchChance;

                if (roll <= cumulative)
                {
                    caughtFishData = fishInstance.fishData;
                    caughtFishPrefab = fishPrefab;
                    break;
                }
            }
        }

        if (caughtFishData != null && caughtFishPrefab != null)
        {
            // Tạo Item từ FishData
            Item caughtFishItem = new Item(
                caughtFishData.fishName,
                caughtFishData.icon,
                caughtFishData.quantity,
                caughtFishData.itemType,
                caughtFishData.toolType,
                caughtFishData.price,
                caughtFishData.energy,
                caughtFishData.description,
                caughtFishPrefab
            );

            // Thêm Item vào Inventory
            InventoryManager.instance.AddItem(caughtFishItem);

                // Hiển thị UI cho cá đã câu được
                if (fishImage != null && fishInfoText != null && fishingResultPanel != null)
                {
                    fishImage.sprite = caughtFishData.icon;
                    //fishInfoText.text = "Name: " + caughtFishData.fishName + "\n" + caughtFishData.description;
                    fishInfoText.text = $"Name: {caughtFishData.fishName}\nRarity: {caughtFishData.rarity}\n{caughtFishData.description}";

                    fishingResultPanel.SetActive(true);

                    StartCoroutine(HideFishingResult());
                }
                else
                {
                    Debug.LogError("Fish UI components are not properly assigned.");
                }
            }
            else
            {
                // Trường hợp không câu được cá
                if (fishInfoText != null && fishingResultPanel != null)
                {
                    fishImage.sprite = null; // Không hiển thị hình ảnh cá
                    fishInfoText.text = "Chúc bạn may mắn lần sau !";
                    fishingResultPanel.SetActive(true);

                    StartCoroutine(HideFishingResult());
                }
                Debug.Log("Không câu được cá lần này.");
            }
        }
         private IEnumerator HideFishingResult()
        {
            yield return new WaitForSeconds(3f);
            fishingResultPanel.SetActive(false);
        }
    }



