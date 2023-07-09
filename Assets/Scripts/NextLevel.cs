using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] LevelEnd levelEnd;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        levelEnd.ShowWin();
    }
}