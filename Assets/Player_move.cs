using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player_move : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public GameObject shadow;
    public Tilemap mainTilemap;
    public Vector3 desired_position;
    public Vector3 brandon_desired_position;
    public float moveSpeed = 2;
    public bool moving = false;
    public bool airborne = false;

    public bool inDialog = false;
    public float dialog_delay = 0f;

    public Transform sfx_player;
    public AudioClip[] sfx_list;
    public Transform[] visual_effects;
    float wall_collide_delay = 0;
    float pokemon_spawn_delay = 0;

    public enum directions { north, south, west, east }

    public directions facing;

    private GameController gameController;
    private PokeSpawner pokeSpawner;

    void Start()
    {
        desired_position = transform.position;
        gameController = FindObjectOfType<GameController>();
        pokeSpawner = FindObjectOfType<PokeSpawner>();
        brandon_desired_position = spriteRenderer.transform.localPosition;
    }

    void Interact(Vector2 position)
    {
        foreach (Sign obj in GameObject.FindObjectsOfType<Sign>())
        {
            if (obj.transform.position.x == position.x && obj.transform.position.y == position.y)
            {
                //Its a sign
                Debug.Log("Reading sign");
                gameController.DisplayMessage(obj.lines);
                return;
            }
        }
    }

    void Update()
    {
        if(dialog_delay > 0) 
        {
            dialog_delay -= Time.deltaTime;
        }

        if (spriteRenderer.transform.position != brandon_desired_position)
        {
            Vector3 brandon_pos = spriteRenderer.transform.localPosition;
            brandon_pos = Vector3.MoveTowards(brandon_pos, brandon_desired_position, moveSpeed * Time.deltaTime);
            spriteRenderer.transform.localPosition = brandon_pos;
        }

        if (airborne)
        {
            Vector3 pos = transform.position;
            pos = Vector3.MoveTowards(transform.position, desired_position, moveSpeed * 1.4f * Time.deltaTime);
            pos.z = (0.02f * pos.y) - 0.0175f;
            transform.position = pos;
            desired_position.z = transform.position.z;

            if (Vector2.Distance(transform.position, desired_position) <= 1f)
            {
                brandon_desired_position.y = 0;
            }

            if (transform.position == desired_position)
            {
                if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow))
                {
                    animator.SetBool("walk_down", false);
                    animator.SetBool("walk_side", false);
                    animator.SetBool("walk_up", false);
                }
                moving = false;
                airborne = false;
                shadow.SetActive(false);
            }
        }

        if (moving && !airborne)
        {
            Vector3 pos = transform.position;
            pos = Vector3.MoveTowards(transform.position, desired_position, moveSpeed * Time.deltaTime);
            pos.z = (0.02f * pos.y) - 0.0175f;
            transform.position = pos;
            desired_position.z = transform.position.z;

            TileBase tile = mainTilemap.GetTile(new Vector3Int((int)desired_position.x-1, (int)desired_position.y-1, 0));

            if (tile)
            {
                if (tile.name.StartsWith("tallgrass"))
                {
                    if (pokemon_spawn_delay <= 0.0f)
                    {
                        //Debug.Log("Grass!");
                        pokemon_spawn_delay = 0.25f;
                        pokeSpawner.TrySpawnPokemon();
                        Instantiate(visual_effects[0], desired_position, Quaternion.identity);
                    }
                    else
                    {
                        pokemon_spawn_delay -= Time.deltaTime;
                    }
                }
            }

            if (transform.position == desired_position)
            {
                if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow))
                {
                    animator.SetBool("walk_down", false);
                    animator.SetBool("walk_side", false);
                    animator.SetBool("walk_up", false);
                    wall_collide_delay = 0;
                }
                moving = false;
            }
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (!moving && !inDialog)
            {
                if (Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f), Vector2.up, 1f))
                {
                    //Collide
                    moving = true;
                    animator.speed = 0.5f;
                    spriteRenderer.flipX = false;
                    animator.SetBool("walk_down", false);
                    animator.SetBool("walk_side", false);
                    animator.SetBool("walk_up", true);
                    facing = directions.north;
                    if (wall_collide_delay <= 0.0f)
                    {
                        wall_collide_delay = 0.5f;
                        Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
                        t.GetComponent<AudioSource>().clip = sfx_list[0];
                        t.GetComponent<AudioSource>().Play();
                    }
                    else {
                        wall_collide_delay -= Time.deltaTime;
                    }
                }
                else {
                    wall_collide_delay = 0;
                    moving = true;
                    animator.speed = 1f;
                    spriteRenderer.flipX = false;
                    animator.SetBool("walk_down", false);
                    animator.SetBool("walk_side", false);
                    animator.SetBool("walk_up", true);
                    facing = directions.north;
                    desired_position.y++;
                    facing = directions.north;
                }
            }
            else
            {

            }
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (!moving && !inDialog)
            {
                if (Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f), Vector2.left, 1f))
                {
                    //Collide

                    TileBase tile = mainTilemap.GetTile(new Vector3Int((int)transform.position.x - 2, (int)transform.position.y - 1, 0));

                    if (tile)
                    {
                        if (tile.name.StartsWith("jumpl"))
                        {
                            shadow.SetActive(true);
                            brandon_desired_position.y = 0.4f;
                            airborne = true;
                            moving = true;
                            spriteRenderer.flipX = false;
                            animator.SetBool("walk_down", false);
                            animator.SetBool("walk_side", true);
                            animator.SetBool("walk_up", false);
                            facing = directions.west;
                            desired_position.x -= 2;
                            Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
                            t.GetComponent<AudioSource>().clip = sfx_list[1];
                            t.GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            moving = true;
                            animator.speed = 0.5f;
                            spriteRenderer.flipX = false;
                            animator.SetBool("walk_down", false);
                            animator.SetBool("walk_side", true);
                            animator.SetBool("walk_up", false);
                            facing = directions.west;
                            if (wall_collide_delay <= 0.0f)
                            {
                                wall_collide_delay = 0.5f;
                                Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
                                t.GetComponent<AudioSource>().clip = sfx_list[0];
                                t.GetComponent<AudioSource>().Play();
                            }
                            else
                            {
                                wall_collide_delay -= Time.deltaTime;
                            }
                        }
                    }
                    else
                    {

                        moving = true;
                        animator.speed = 0.5f;
                        spriteRenderer.flipX = false;
                        animator.SetBool("walk_down", false);
                        animator.SetBool("walk_side", true);
                        animator.SetBool("walk_up", false);
                        facing = directions.west;
                        if (wall_collide_delay <= 0.0f)
                        {
                            wall_collide_delay = 0.5f;
                            Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
                            t.GetComponent<AudioSource>().clip = sfx_list[0];
                            t.GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            wall_collide_delay -= Time.deltaTime;
                        }
                    }
                }
                else {
                    wall_collide_delay = 0;
                    moving = true;
                    animator.speed = 1f;
                    spriteRenderer.flipX = false;
                    animator.SetBool("walk_down", false);
                    animator.SetBool("walk_side", true);
                    animator.SetBool("walk_up", false);
                    desired_position.x--;
                    facing = directions.west;
                }
            }
            else
            {

            }
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (!moving && !inDialog)
            {
                if (Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f), Vector2.down, 1f))
                {
                    //Collide

                    TileBase tile = mainTilemap.GetTile(new Vector3Int((int)transform.position.x - 1, (int)transform.position.y - 2, 0));

                    if (tile)
                    {
                        if (tile.name.StartsWith("jumpd"))
                        {
                            shadow.SetActive(true);
                            brandon_desired_position.y = 0.4f;
                            airborne = true;
                            moving = true;
                            spriteRenderer.flipX = false;
                            animator.SetBool("walk_down", true);
                            animator.SetBool("walk_side", false);
                            animator.SetBool("walk_up", false);
                            desired_position.y -= 2;
                            facing = directions.south;
                            Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
                            t.GetComponent<AudioSource>().clip = sfx_list[1];
                            t.GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            moving = true;
                            animator.speed = 0.5f;
                            spriteRenderer.flipX = false;
                            animator.SetBool("walk_down", true);
                            animator.SetBool("walk_side", false);
                            animator.SetBool("walk_up", false);
                            facing = directions.south;
                            if (wall_collide_delay <= 0.0f)
                            {
                                wall_collide_delay = 0.5f;
                                Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
                                t.GetComponent<AudioSource>().clip = sfx_list[0];
                                t.GetComponent<AudioSource>().Play();
                            }
                            else
                            {
                                wall_collide_delay -= Time.deltaTime;
                            }
                        }
                    }
                    else {
                        wall_collide_delay = 0;
                        moving = true;
                        animator.speed = 0.5f;
                        spriteRenderer.flipX = false;
                        animator.SetBool("walk_down", true);
                        animator.SetBool("walk_side", false);
                        animator.SetBool("walk_up", false);
                        facing = directions.south;
                        if (wall_collide_delay <= 0.0f)
                        {
                            wall_collide_delay = 0.5f;
                            Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
                            t.GetComponent<AudioSource>().clip = sfx_list[0];
                            t.GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            wall_collide_delay -= Time.deltaTime;
                        }
                    }
                }
                else {
                    wall_collide_delay = 0;
                    moving = true;
                    animator.speed = 1f;
                    spriteRenderer.flipX = false;
                    animator.SetBool("walk_down", true);
                    animator.SetBool("walk_side", false);
                    animator.SetBool("walk_up", false);
                    desired_position.y--;
                    facing = directions.south;
                }
            }
            else
            {

            }
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (!moving && !inDialog)
            {
                if (Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f), Vector2.right, 1f))
                {
                    //Collide

                    TileBase tile = mainTilemap.GetTile(new Vector3Int((int)transform.position.x, (int)transform.position.y - 1, 0));

                    if (tile)
                    {
                        if (tile.name.StartsWith("jumpr"))
                        {
                            shadow.SetActive(true);
                            brandon_desired_position.y = 0.4f;
                            airborne = true;
                            moving = true;
                            spriteRenderer.flipX = true;
                            animator.SetBool("walk_down", false);
                            animator.SetBool("walk_side", true);
                            animator.SetBool("walk_up", false);
                            desired_position.x += 2;
                            facing = directions.east;
                            Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
                            t.GetComponent<AudioSource>().clip = sfx_list[1];
                            t.GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            moving = true;
                            animator.speed = 0.5f;
                            spriteRenderer.flipX = true;
                            animator.SetBool("walk_down", false);
                            animator.SetBool("walk_side", true);
                            animator.SetBool("walk_up", false);
                            facing = directions.east;
                            if (wall_collide_delay <= 0.0f)
                            {
                                wall_collide_delay = 0.5f;
                                Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
                                t.GetComponent<AudioSource>().clip = sfx_list[0];
                                t.GetComponent<AudioSource>().Play();
                            }
                            else
                            {
                                wall_collide_delay -= Time.deltaTime;
                            }
                        }
                    }
                    else{

                        moving = true;
                        animator.speed = 0.5f;
                        spriteRenderer.flipX = true;
                        animator.SetBool("walk_down", false);
                        animator.SetBool("walk_side", true);
                        animator.SetBool("walk_up", false);
                        facing = directions.east;
                        if (wall_collide_delay <= 0.0f)
                        {
                            wall_collide_delay = 0.5f;
                            Transform t = Instantiate(sfx_player, transform.position, Quaternion.identity);
                            t.GetComponent<AudioSource>().clip = sfx_list[0];
                            t.GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            wall_collide_delay -= Time.deltaTime;
                        }
                    }
                }
                else
                {
                    wall_collide_delay = 0;
                    moving = true;
                    animator.speed = 1f;
                    spriteRenderer.flipX = true;
                    animator.SetBool("walk_down", false);
                    animator.SetBool("walk_side", true);
                    animator.SetBool("walk_up", false);
                    desired_position.x++;
                    facing = directions.east;
                }
            }
            else
            {

            }
        }

        if (Input.GetKeyDown(KeyCode.Z) && inDialog == false && dialog_delay <= 0)
        {
            Debug.Log("Z Pressed!");
            Debug.Log("Dialog: " + inDialog.ToString());

            if (!moving || wall_collide_delay > 0)
            {
                Debug.Log("Not moving, neither in collide delay");

                if (!airborne)
                {
                    Debug.Log("Not airborne!");

                    if (facing == directions.north)
                    {
                        //Up
                        Interact(new Vector2(transform.position.x, transform.position.y + 1));
                    }
                    if (facing == directions.south)
                    {
                        //Down
                        Interact(new Vector2(transform.position.x, transform.position.y - 1));
                    }
                    if (facing == directions.west)
                    {
                        //Left
                        Interact(new Vector2(transform.position.x - 1, transform.position.y));
                    }
                    if (facing == directions.east)
                    {
                        //Right
                        Interact(new Vector2(transform.position.x + 1, transform.position.y));
                    }
                }
            }
        }



    }
}
