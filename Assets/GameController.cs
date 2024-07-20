using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Tilemap grass_toppings;
    public GameObject message_obj;
    private Text message_text;
    private GameObject message_pointer_arrow;
    private string[] message_buffer = new string[] { "" };
    private int message_index = 0;

    private Player_move player;
    private string current_city = "Littleroot town";

    public Transform sfx_player;
    public AudioClip[] sfx_list;

    public float letter_delay = 0.025f;
    private float letter_delay_now;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player_move>();
        message_text = message_obj.GetComponentInChildren<Text>();
        message_pointer_arrow = message_obj.GetComponentInChildren<UIImageAnimator>().gameObject;
        foreach (var position in grass_toppings.cellBounds.allPositionsWithin)
        {
            if (!grass_toppings.HasTile(position))
            {
                //Debug.Log("Tile empty: " + position.x + "-" + position.y);
                continue;
            }
            //Debug.Log("Tile CHANGED: " + position.x + "-" + position.y);
            grass_toppings.SetTransformMatrix(position, Matrix4x4.TRS(new Vector3(0, 0, (0.02f * position.y)), Quaternion.Euler(0, 0, 0), Vector3.one));
        }

        //grass_toppings.RefreshAllTiles();
    }

    public void SetCurrentCity(string newCity) 
    {
        current_city = newCity;
    }

    public string GetCurrentCity() 
    {
        return current_city;
    }

    public void DisplayMessage(string[] lines)
    {
        message_text.text = "";
        message_index = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Replace("\\r\\n", "\n");
            lines[i] = lines[i].Replace("/\n/\r", "\n");
            lines[i] = lines[i].Replace("\\n", "\n");
            lines[i] = lines[i].Replace("/\n", "\n");
        }
        message_buffer = lines;
        player.inDialog = true;
        Invoke("StartMessage", 0.1f);
        Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
        t.GetComponent<AudioSource>().clip = sfx_list[0];
        t.GetComponent<AudioSource>().Play();
    }

    private void StartMessage()
    {
        message_obj.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (message_obj.activeSelf)
        {
            if (message_index < message_buffer.Length-1)
            {
                message_pointer_arrow.SetActive(true);
            }
            else
            {
                message_pointer_arrow.SetActive(false);
            }

            if (message_text.text != message_buffer[message_index])
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    message_text.text = message_buffer[message_index];
                }else
                {
                    if (letter_delay_now <= 0)
                    {
                        letter_delay_now = letter_delay;
                        message_text.text += message_buffer[message_index].Substring(message_text.text.Length, 1);
                    }else
                    {
                        letter_delay_now -= Time.deltaTime;
                    }
                }
            }else {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (message_index < message_buffer.Length-1)
                    {
                        message_index++;
                        message_text.text = "";
                        Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
                        t.GetComponent<AudioSource>().clip = sfx_list[0];
                        t.GetComponent<AudioSource>().Play();
                    }
                    else {
                        message_obj.SetActive(false);
                        player.inDialog = false;
                        player.dialog_delay = 0.25f;
                    }
                }
            }
        }
    }
}
