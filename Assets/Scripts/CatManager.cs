using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    [Header("Careful. Order of Cats matters a lot. It determines both their evolution order and their value")]
    [SerializeField]
    private List<Cat> cats;

    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private float startingDownForce = 10f;

    [SerializeField] private float delayBetweenDrops = 0.5f;

    [SerializeField] private Transform nextCatPose;
    [SerializeField] private Transform dropPawPose;

    [SerializeField] private float dropAreaWidth;

    private Cat _thisCat;
    private Cat _nextCat;


    private int _dropCatCount = 4;

    private int _highScore;

    private int HighScore
    {
        get => _highScore;
        set
        {
            _highScore = value;
            highScoreText.text = _highScore.ToString();
        }
    }

    private readonly List<MergeRequest> _mergeRequests = new List<MergeRequest>();

    private void Start()
    {
        if (cats == null || cats.Count == 0)
        {
            throw new Exception("There is a problem with the cats list. It is empty or null.");
        }

        if (highScoreText == null)
        {
            throw new Exception("The highScoreText is not set. Please set it in the inspector.");
        }

        CreateNextCat();
        MoveNextCatToDropPaw();
        CreateNextCat();
    }

    private bool _onCooldown = false;

    private void ResetCooldown()
    {
        _onCooldown = false;
    }

    private void Update()
    {
        SetDropPawPose();
        DropCat();
    }

    private void CreateNextCat()
    {
        _nextCat = Instantiate(GetRandomDropCat(), nextCatPose).GetComponent<Cat>();
        Rigidbody2D rigi = _nextCat.GetComponent<Rigidbody2D>();
        rigi.simulated = false;
    }

    private void MoveNextCatToDropPaw()
    {
        _nextCat.transform.position = dropPawPose.position;
        _nextCat.transform.SetParent(dropPawPose);
        _thisCat = _nextCat;
        _nextCat = null;
    }


    public void DropCat()
    {
        if (_onCooldown)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // if space is pressed, instantiate a cat at the mouse position
            _thisCat.transform.SetParent(this.transform, true);
            Rigidbody2D rigi = _thisCat.GetComponent<Rigidbody2D>();
            rigi.simulated = true;
            rigi.AddForce(Vector2.down * (startingDownForce * rigi.mass), ForceMode2D.Impulse);
            _onCooldown = true;
            Invoke(nameof(ResetCooldown), delayBetweenDrops);
            MoveNextCatToDropPaw();
            CreateNextCat();
        }
    }

    private void SetDropPawPose()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        mousePosition.y = this.transform.position.y;
        mousePosition.x = Math.Clamp(mousePosition.x, -dropAreaWidth, dropAreaWidth);
        // if (mousePosition.x < -dropAreaWidth)
        // {
        //     mousePosition.x = -dropAreaWidth;
        // }
        // else if (mousePosition.x > dropAreaWidth)
        // {
        //     mousePosition.x = dropAreaWidth;
        // }

        dropPawPose.position = mousePosition;
    }


    private GameObject GetRandomDropCat()
    {
        // return a random cat from the first _dropCatCount cats
        int randomIndex = UnityEngine.Random.Range(0, _dropCatCount);
        return cats[randomIndex].gameObject;
    }

    private void MergeCats(Cat cat1, Cat cat2)
    {
        Vector3 newPos = (cat1.transform.position + cat2.transform.position) / 2;
        Destroy(cat1.gameObject);
        Destroy(cat2.gameObject);
        GameObject nextCat = GetNextCatAndAddScore(cat1.CatType);
        Instantiate(nextCat, newPos, Quaternion.identity);
    }

    private struct MergeRequest
    {
        public int ID;
        public Cat Cat1;
        public Cat Cat2;
    }

    private void LateUpdate()
    {
        // Try clause to ensure that the merge requests are cleared even if an exception is thrown
        try
        {
            List<int> doneCatsIDs = new List<int>();
            _mergeRequests.DistinctBy(request => request.ID).ToList().ForEach(request =>
            {
                // prevent the same cat from being merged twice which would cause an exception
                if (!doneCatsIDs.Contains(request.Cat1.GetInstanceID()) &&
                    !doneCatsIDs.Contains(request.Cat2.GetInstanceID()))
                {
                    MergeCats(request.Cat1, request.Cat2);
                    doneCatsIDs.Add(request.Cat1.GetInstanceID());
                    doneCatsIDs.Add(request.Cat2.GetInstanceID());
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            _mergeRequests.Clear();
        }
    }

    public void QueueMergeRequest(Cat cat1, Cat cat2)
    {
        // add the two cats to the merge request queue as a tuple that is sorted by their instance id
        int id1 = cat1.GetInstanceID();
        int id2 = cat2.GetInstanceID();

        _mergeRequests.Add(new MergeRequest
        {
            // this should create a unique id for each pair of cats
            ID = id1.GetHashCode() ^ id2.GetHashCode(),
            Cat1 = cat1,
            Cat2 = cat2
        });
    }


    private GameObject GetNextCatAndAddScore(CatType catType)
    {
        int index = cats.FindIndex(cat => cat.CatType == catType);
        Debug.Log("index = " + index);
        if (index < 0)
        {
            throw new Exception("The catType" + catType + " is not in the list of cats.");
        }

        if (index >= cats.Count - 1)
        {
            Debug.Log("Collided two final cats. return first cat but with a value of Math.Pow(2^, cats.Index+1)");

            AddPointsToHighScoreByIndex(index + 1);
            return cats[0].gameObject;
        }

        AddPointsToHighScoreByIndex(index);
        return cats[index + 1].gameObject;
    }


    private void AddPointsToHighScoreByIndex(int index)
    {
        HighScore += (int) Math.Pow(2, index);
        // Add points to the HighScore
        if (HighScore > 99999)
        {
            HighScore = 99999;
        }

        Debug.Log("HighScore: " + HighScore);
    }

    public void GameOver()
    {
        // Game over logic
        Debug.Log("Game Over");
        highScoreText.text = "Game Over";
        Time.timeScale = 0;
        _onCooldown = true;
    }


    // Create Singleton instance of this

    public static CatManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }
}

public enum CatType
{
    Cat1,
    Cat2,
    Cat3,
    Cat4,
    Cat5,
    Cat6,
    Cat7,
    Cat8,
    Cat9,
    Cat10,
}