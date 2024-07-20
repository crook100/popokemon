using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGM_Player : MonoBehaviour
{
    public AudioSource audioSource;

    private bool down;
    private bool area_name_visible;
    private float desiredVolume;
    private AudioClip desiredClip;

    public Transform new_area_display;
    private RectTransform new_area_rect;
    private string desired_area_name = "";
    private float desired_area_name_width = 150;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.GetComponent<BGM_Music>())
        {
            if (desiredClip != col.transform.GetComponent<BGM_Music>().music) { down = true; }
            desired_area_name = col.transform.GetComponent<BGM_Music>().display_name;
            FindObjectOfType<GameController>().SetCurrentCity(desired_area_name);

            if (area_name_visible)
            {
                area_name_visible = false;
                Invoke("ShowAreaName", 0.25f);
                Invoke("HideAreaName", 3.25f);
            }
            else {
                area_name_visible = true;
                new_area_display.transform.GetComponentInChildren<Text>().text = desired_area_name;
                desired_area_name_width = new_area_display.transform.GetComponentInChildren<Text>().preferredWidth + 40;
                Invoke("HideAreaName", 3f);
            }
            desiredClip = col.transform.GetComponent<BGM_Music>().music;
            desiredVolume = col.transform.GetComponent<BGM_Music>().volume;
        }
    }

    private void Start()
    {
        new_area_rect = new_area_display.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (new_area_rect.sizeDelta.x != desired_area_name_width)
        {
            new_area_rect.sizeDelta = new Vector2(desired_area_name_width, new_area_rect.sizeDelta.y);
            new_area_rect.anchoredPosition = new Vector2(desired_area_name_width/2, new_area_rect.anchoredPosition.y);
            RectTransform child_rect = new_area_display.transform.GetComponentInChildren<Text>().transform.GetComponent<RectTransform>();
            child_rect.sizeDelta = new_area_rect.sizeDelta;
            //child_rect.anchoredPosition = new_area_rect.anchoredPosition;
        }

        if (area_name_visible)
        {
            if (new_area_rect.anchoredPosition.y != -25)
            {
                new_area_rect.anchoredPosition = Vector2.MoveTowards(new_area_rect.anchoredPosition, new Vector2(new_area_rect.position.x, -25), 100 * Time.deltaTime);
            }
        }
        else{
            if (new_area_rect.anchoredPosition.y != 25)
            {
                new_area_rect.anchoredPosition = Vector2.MoveTowards(new_area_rect.anchoredPosition, new Vector2(new_area_rect.position.x, 25), 100 * Time.deltaTime);
            }
        }

        if (down)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0, 1 * Time.deltaTime);
            if (audioSource.volume <= 0) { down = false; audioSource.clip = desiredClip; audioSource.Play(); }
        }
        else {
            if (audioSource.volume != desiredVolume)
            {
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, desiredVolume, 1 * Time.deltaTime);
            }
        }
    }

    private void ShowAreaName()
    {
        new_area_display.transform.GetComponentInChildren<Text>().text = desired_area_name;
        desired_area_name_width = new_area_display.transform.GetComponentInChildren<Text>().preferredWidth + 30;
        area_name_visible = true;
    }

    private void HideAreaName()
    {
        area_name_visible = false;
    }
}
