using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
   public void OnClick()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
