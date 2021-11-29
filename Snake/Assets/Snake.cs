using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    public int xSize, ySize;
    public GameObject block;

    GameObject head;
    public Material headMaterial, tailMaterial;
    List<GameObject> tail;

    Vector2 dir;

    private bool down = true;
    private bool top = true;
    private bool right = true;
    private bool left = true;

    public Text points;
    // Start is called before the first frame update
    void Start()
    {
        timeBetweenMovements = 0.5f;
        dir = Vector2.right;
        createGrid();
        createPlayer();
        spawnFood();
        block.SetActive(false);
        isAlive = true;
    }
    private Vector2 getRandomPos(){
        return new Vector2(Random.Range(-xSize/2+1, (xSize/2)+1), Random.Range(-ySize/2+1, (ySize/2))+1);
    }

    private bool containedInSnake(Vector2 spawnPos){
        bool isInHead = spawnPos.x == head.transform.position.x && spawnPos.y == head.transform.position.y;
        bool isInTail = false;
        foreach(var item in tail)
        {
            if(item.transform.position.x == spawnPos.x && item.transform.position.y == spawnPos.y){
                isInTail =true;
            }
        }
        return isInHead || isInTail;
    }
    GameObject food;

    bool isAlive;

    private void spawnFood(){
        Vector2 spawnPos = getRandomPos();
        while(containedInSnake(spawnPos)){ 
            spawnPos = getRandomPos();
        }
        food = Instantiate(block);
        food.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);
        food.SetActive(true);
    }
    private void createPlayer(){
        head = Instantiate(block) as GameObject;
        head.GetComponent<MeshRenderer>().material = headMaterial;
        tail = new List<GameObject>();
    }
    private void createGrid()
    {
        for(int x = 0; x <= xSize+1; x++)
        {
            GameObject borderBottom = Instantiate(block) as GameObject;
            borderBottom.GetComponent<Transform>().position = new Vector3(x - xSize / 2, -ySize / 2, 0);

            GameObject borderTop = Instantiate(block) as GameObject;
            borderTop.GetComponent<Transform>().position = new Vector3(x - xSize / 2, (ySize-ySize / 2)+1, 0);
        }

        for(int y = 0;y <= ySize+1; y++)
        {
            GameObject borderRight = Instantiate(block) as GameObject;
            borderRight.GetComponent<Transform>().position = new Vector3(-xSize/2, y-(ySize/2), 0);

            GameObject borderLeft = Instantiate(block) as GameObject;
            borderLeft.GetComponent<Transform>().position = new Vector3(xSize-(xSize/2)+1, y-(ySize/2), 0);
        }
    }

    float passedTime, timeBetweenMovements;

    public GameObject gameOverUI;

    private void gameOver(){
        isAlive = false;
        gameOverUI.SetActive(true);
    }

    public void restart(){
        SceneManager.LoadScene(0);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.DownArrow) && down == true){
            dir = Vector2.down;
            down = true; top = false; right = true; left = true;
        }else if(Input.GetKey(KeyCode.UpArrow) && top == true){
            dir = Vector2.up;
            down = false; top = true; right = true; left = true;
        }else if(Input.GetKey(KeyCode.RightArrow) && right == true){
            dir = Vector2.right;
            down = true; top = true; right = true; left = false;
        }else if(Input.GetKey(KeyCode.LeftArrow) && left == true){
            dir = Vector2.left;
            down = true; top = true; right = false; left = true;
        }

        passedTime += Time.deltaTime;
        if(timeBetweenMovements < passedTime && isAlive){
            passedTime = 0;
            //ruch
            Vector3 newPosition = head.GetComponent<Transform>().position + new Vector3(dir.x, dir.y, 0);

            //Check for collisions
            //border collision
            if(newPosition.x >= (xSize/2)+1
            || newPosition.x <= -xSize/2
            || newPosition.y >= (ySize/2)+1
            || newPosition.y <= -ySize/2){
                gameOver();
            }

            //tail collision
            foreach(var item in tail){
                if(item.transform.position == newPosition){
                    gameOver();
                }
            }
            if(newPosition.x == food.transform.position.x && newPosition.y == food.transform.position.y){
                GameObject newTile = Instantiate(block);
                newTile.SetActive(true);
                newTile.transform.position = food.transform.position;
                DestroyImmediate(food);
                head.GetComponent<MeshRenderer>().material = tailMaterial;
                tail.Add(head);
                head = newTile;
                head.GetComponent<MeshRenderer>().material = headMaterial;
                spawnFood();
                points.text = "Points: " + (tail.Count);
            }else
            {
                if(tail.Count == 0){
                    head.transform.position = newPosition;
                } else{
                    head.GetComponent<MeshRenderer>().material = tailMaterial;
                    tail.Add(head);
                    head = tail[0];
                    head.GetComponent<MeshRenderer>().material = headMaterial;
                    tail.RemoveAt(0);
                    head.transform.position = newPosition;
                }
            }

        }
    }
}
