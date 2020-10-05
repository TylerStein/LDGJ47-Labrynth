using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public List<GameObject> views;

    public void SwitchToView(int index) {
        if (index >= views.Count) throw new UnityException($"Index {index} is not a valid view");
        for (int i = 0; i < views.Count; i++) {
            if (i == index) views[i].SetActive(true);
            else views[i].SetActive(false);
        }
    }

    public void EnableView(int index) {
        if (index >= views.Count) throw new UnityException($"Index {index} is not a valid view");
        views[index].SetActive(true);
    }

    public void DisableView(int index) {
        if (index >= views.Count) throw new UnityException($"Index {index} is not a valid view");
        views[index].SetActive(false);
    }

    public void LoadScene(int index) {
        SceneManager.LoadScene(index);
    }

    public void CloseAll() {
        foreach (GameObject view in views) {
            view.SetActive(false);
        }
    }

    public void Quit() {
        Application.Quit();
    }
}
