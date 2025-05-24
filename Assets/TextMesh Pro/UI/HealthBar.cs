using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;


    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }
    public void SetHealth(int health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}

//Karakter koduna referanslamak için

// public HealthBar healthBar;

//oluþturulmalý.

//Max caný girmek için Start fonksiyonunda

// healthBar.SetHealth(maxHealth);

//oluþturulmalý.

//Karakter hasar aldýðýnda kalan caný göstermek için

// healthBar.SetHealth(currentHealth);

//kodu hasar aldýðý fonksiyona eklenmeli.

//Player Scriptine eklendikten sonra Unity üzerinden healthBar adlý gameObject "Health Bar" adlý yere eklenmeli.