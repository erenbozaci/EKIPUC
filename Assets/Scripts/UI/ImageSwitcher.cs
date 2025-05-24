using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    [Range(0, 100)]
    public float percentage; // Inspector'dan veya koddan kontrol edilebilir
    public Slider slider;

    public GameObject image1;
    public GameObject image2;
    public GameObject image3;

    void Update()
    {
        percentage = slider.value;
        if (percentage <= 33f)
        {
            SetActiveImage(image1);
        }
        else if (percentage <= 66f)
        {
            SetActiveImage(image2);
        }
        else
        {
            SetActiveImage(image3);
        }
    }

    void SetActiveImage(GameObject activeImage)
    {
        image1.SetActive(activeImage == image1);
        image2.SetActive(activeImage == image2);
        image3.SetActive(activeImage == image3);
    }
}