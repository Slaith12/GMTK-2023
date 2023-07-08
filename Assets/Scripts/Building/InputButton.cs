using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputButtons : MonoBehaviour
{
    public putObject[] siegeComps;
    public GameObject siege;
    public PhysicsMaterial2D sieges;
    public GameObject king;
    public GameObject canvas;
    public void cancel()
    {
        for (int i = 0; i < siegeComps.Length; i++)
        {
            siegeComps[i].objInComponent = null;
            GetComponentInParent<siegeBuilder>().TheresKing = false;
            siegeComps[i].TheresKing = false;
            siegeComps[i].isUsed = false;
        }
    }
    public void createSiege()
    {

        GameObject s = Instantiate(siege, Vector3.zero, Quaternion.identity);
        for (int i = 0; i < siegeComps.Length; i++)
        {
            if (siegeComps[i].GetComponent<putObject>().objInComponent != null)
            {
                GameObject z = Instantiate(siegeComps[i].GetComponent<putObject>().objInComponent, s.GetComponent<Siege>().comps[i].transform.position, Quaternion.identity);
                z.transform.parent = s.transform;
            }
            if (siegeComps[i].GetComponent<putObject>().TheresKing)
            {
                GameObject z = Instantiate(king, s.GetComponent<Siege>().comps[i].transform.position, Quaternion.identity);
                Camera.main.transform.parent = z.transform;
                Camera.main.transform.localPosition = new Vector3(0, 0, -10);

            }
        }

        canvas.SetActive(false);
    }
}