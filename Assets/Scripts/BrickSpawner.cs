using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    [SerializeField] int maxWidth = 5;
    [SerializeField] int maxHeight = 10;

    [SerializeField] float paddingX = 2f;
    [SerializeField] float paddingY = 2f;
    [SerializeField] GameObject brickPrefab;
    public bool[,] bricks;

    public List<GameObject> brickRefs;

    void Awake()
    {
        EventHandler.Instance.hub.Subscribe<Restart>(r => SpawnBricks());
        EventHandler.Instance.hub.Subscribe<GameOver>(r => DestroyBricks());
        SpawnBricks();
    }

    void SpawnBricks()
    {
        bricks = new bool[maxWidth, maxHeight];

        for (int x = 0; x < maxWidth; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                if(!(x % 3 == 0 && y % 2 ==0))
                {
                    bricks[x,y] = true;
                }
                if(bricks[x,y])
                {
                    var newBrick = Instantiate(brickPrefab, new Vector2((transform.position.x + x) + x * paddingX, (transform.position.y + y) + y * paddingY), brickPrefab.transform.rotation);
                    newBrick.GetComponent<Brick>().SetHealth(Random.Range(1, 4));
                    brickRefs.Add(newBrick);
                }
            }
        }
    }

    void DestroyBricks()
    {
        foreach (var brick in brickRefs)
        {
            if(brick) Destroy(brick);
        }
        brickRefs.Clear();
    }

}
