using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text text1, text2, text3;
    public Image img1, img2, img3;
    private Item itemImg1, itemImg2, itemImg3, itemText1, itemText2, itemText3;
    private DataManager dataManager;
    // Start is called before the first frame update
    private void Awake()
    {
        itemImg1 = img1.GetComponent<Item>();
        itemImg2 = img2.GetComponent<Item>();
        itemImg3 = img3.GetComponent<Item>();
        itemText1 = text1.GetComponent<Item>();
        itemText2 = text2.GetComponent<Item>();
        itemText3 = text3.GetComponent<Item>();
    }
    private void Start()
    {
        dataManager = GameObject.Find("_DataManager").GetComponent<DataManager>();
        DisplayImagesTexts();
    }
    public void DisplayImagesTexts()
    {
        var animals = dataManager.GetRandomAnimals();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/sprites");
        img1.sprite = sprites[short.Parse(animals[0].imgInResources)];
        img2.sprite = sprites[short.Parse(animals[1].imgInResources)];
        img3.sprite = sprites[short.Parse(animals[2].imgInResources)];

        itemImg1.id = short.Parse(animals[0].imgInResources);
        itemImg2.id = short.Parse(animals[1].imgInResources);
        itemImg3.id = short.Parse(animals[2].imgInResources);

        int random = Random.Range(0, animals.Count);
        text1.text = animals[random].animalName;
        itemText1.id = short.Parse(animals[random].imgInResources);
        animals.RemoveAt(random);

        int random2 = Random.Range(0, animals.Count);
        text2.text = animals[random2].animalName;
        itemText2.id = short.Parse(animals[random2].imgInResources);
        animals.RemoveAt(random2);

        int random3 = Random.Range(0, animals.Count);
        text3.text = animals[random3].animalName;
        itemText3.id = short.Parse(animals[random3].imgInResources);
        animals.RemoveAt(random3);
    }
    public void ResetRoundTags()
    {
        text1.tag = "choice";
        text2.tag = "choice";
        text3.tag = "choice";
        img1.tag = "img";
        img2.tag = "img";
        img3.tag = "img";
    }

    public void StarUI(int starsNumber, GameObject popupGo)
    {
        if (starsNumber == 1)
        {
            popupGo.transform.GetChild(3).gameObject.SetActive(true);
        }
        if (starsNumber == 2)
        {
            popupGo.transform.GetChild(3).gameObject.SetActive(true);
            popupGo.transform.GetChild(4).gameObject.SetActive(true);
        }
        if (starsNumber == 3)
        {
            popupGo.transform.GetChild(3).gameObject.SetActive(true);
            popupGo.transform.GetChild(4).gameObject.SetActive(true);
            popupGo.transform.GetChild(5).gameObject.SetActive(true);
        }
    }
}
