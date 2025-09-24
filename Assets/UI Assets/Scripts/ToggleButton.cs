using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    public Image buttonImage;
    public GameObject panel;
    public Sprite image1;
    public Sprite image2;
    
    private bool isToggled = false;
    
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Toggle);
    }
    
    void Toggle()
    {
        isToggled = !isToggled;
        buttonImage.sprite = isToggled ? image2 : image1;
        panel.SetActive(isToggled);
    }
}
