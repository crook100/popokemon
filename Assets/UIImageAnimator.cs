using UnityEngine;
using UnityEngine.UI;

public class UIImageAnimator : MonoBehaviour
{
    public Sprite[] sprites;
    public float delay;
    private int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("changeSprite", delay, delay);
    }

    void changeSprite()
    {
        if (index < sprites.Length-1)
        {
            index++;
        }
        else {
            index = 0;
        }

        GetComponent<Image>().sprite = sprites[index];
    }
}
