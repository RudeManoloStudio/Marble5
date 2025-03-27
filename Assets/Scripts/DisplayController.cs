using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayController : MonoBehaviour
{

    //[SerializeField] private GameObject background;
    [SerializeField] private Transform background;
    [SerializeField] private GameObject grid;
    [SerializeField] private Transform container;

    private GameObject bille;
    private GameObject plomb; // pour le handicap

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
        background.gameObject.SetActive(false);
        ClearBoard();
    }

    public void ShowBackground()
    {
        background.gameObject.SetActive(true);
    }

    public void PrepareBackgroundAndGrid(Vector2Int gridSize, Material backgroundMaterial)
    {

        ClearBoard();

        float h_offset = (gridSize.x % 2 == 0) ? h_offset = 0.5f : h_offset = 0f;
        float v_offset = (gridSize.y % 2 == 0) ? v_offset = 0.5f : v_offset = 0f;

        background.position = new Vector3(gridSize.x / 2 + h_offset, gridSize.y / 2 + v_offset, 0.5f);
        background.localScale = new Vector3(gridSize.x / 10 + 2, 1, gridSize.y / 10 + 2);
        background.GetComponent<Renderer>().material = backgroundMaterial;

        //background.transform.position = new Vector3(gridSize.x / 2 + h_offset, gridSize.y / 2 + v_offset, 0.5f);
        //background.transform.localScale = new Vector3(gridSize.x / 10 + 2, 1, gridSize.y / 10 + 2);
        //background.GetComponent<Renderer>().material = backgroundMaterial;

        grid.transform.position = new Vector3(gridSize.x / 2 + h_offset, gridSize.y / 2 + v_offset, 0.4f);
        grid.transform.localScale = new Vector3(gridSize.x, gridSize.y, 1);
        Material gridSGMaterial = grid.GetComponent<MeshRenderer>().material;
        gridSGMaterial.SetVector("_tiling", new Vector4(gridSize.x, gridSize.y));

    }

    public void PrepareMotif(Vector2Int gridSize, MotifData motif)
    {
        if (motif != null)
        {
            foreach (Vector2Int position in motif.BillesMotif)
            {
                GameObject newBille = Instantiate(bille, new Vector3Int(position.x + gridSize.x / 2, position.y + gridSize.y / 2), Quaternion.identity);
                newBille.transform.SetParent(container);
                newBille.tag = "Bille";

            }
        }
    }

}
