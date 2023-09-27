using UnityEngine;


public class HauteurGros : MonoBehaviour
{
    //Est appelée à chaque frame
    void Update()
    {
        //Fait en sorte que les ennemis restent à une hauteur (sur l'axe des Y) constante
        transform.position = new Vector3(transform.position.x, 7.8f, transform.position.z);
    }
}
