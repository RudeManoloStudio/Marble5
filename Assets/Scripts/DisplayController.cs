using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayController : MonoBehaviour
{

    [SerializeField] private Transform background;
    [SerializeField] private GameObject grid;
    [SerializeField] private Transform container;

    private GameObject bille;
    private GameObject plomb; // pour le handicap

    private Sprite backgroundImage;

    private void Start()
    {
        backgroundImage = GameManager.Instance.InitialBackground;
        background.GetComponent<Image>().sprite = backgroundImage;
    }

    public void DropBilles()
    {

        EventManager.TriggerEvent("DropBilles");
    }

    public void SetBilleAndPlomb(GameObject bille, GameObject plomb)
    {
        this.bille = bille;
        this.plomb = plomb;
    }

    public void ClearBoard()
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ResetBoard()
    {
        background.GetComponent<Image>().sprite = backgroundImage;
        grid.gameObject.SetActive(false);
        ClearBoard();
    }

    public void PrepareBackgroundAndGrid(Vector2Int gridSize, Sprite backgroundTexture)
    {

        ClearBoard();

        float h_offset = (gridSize.x % 2 == 0) ? h_offset = 0.5f : h_offset = 0f;
        float v_offset = (gridSize.y % 2 == 0) ? v_offset = 0.5f : v_offset = 0f;

        background.GetComponent<Image>().sprite = backgroundTexture;

        grid.gameObject.SetActive(true);
        grid.transform.position = new Vector3(gridSize.x / 2 + h_offset, gridSize.y / 2 + v_offset, 0.4f);
        grid.transform.localScale = new Vector3(gridSize.x, gridSize.y, 1);
        Material gridSGMaterial = grid.GetComponent<MeshRenderer>().material;
        gridSGMaterial.SetVector("_tiling", new Vector4(gridSize.x, gridSize.y));

    }

    public void PrepareMotif(Vector2Int gridSize, MotifData motif, int handicap)
    {
        if (motif != null)
        {

            handicap = handicap > motif.BillesMotif.Length ? motif.BillesMotif.Length : handicap;

            List<Vector2Int> billesConservees = new List<Vector2Int>(motif.BillesMotif);
            List<Vector2Int> billesTransformees = new List<Vector2Int>();

            for (int i = 0; i < handicap; i++)
            {
                int randomBille = Random.Range(0, billesConservees.Count);
                billesTransformees.Add(billesConservees[randomBille]);
                billesConservees.RemoveAt(randomBille);
            }

            foreach (Vector2Int position in billesConservees)
            {
                GameObject newBille = Instantiate(bille, new Vector3Int(position.x + gridSize.x / 2, position.y + gridSize.y / 2), Quaternion.identity);
                newBille.transform.SetParent(container);
                newBille.tag = "Bille";
            }

            foreach (Vector2Int position in billesTransformees)
            {
                GameObject newPlomb = Instantiate(plomb, new Vector3Int(position.x + gridSize.x / 2, position.y + gridSize.y / 2), Quaternion.identity);
                newPlomb.transform.SetParent(container);
                newPlomb.tag = "Plomb";
                newPlomb.GetComponent<BilleController>().DoRotate(false);
            }
        }
    }

}
