using UnityEngine;


public class HauteurGros : MonoBehaviour
{
    //Est appel�e � chaque frame
    void Update()
    {
        //Fait en sorte que les ennemis restent � une hauteur (sur l'axe des Y) constante
        transform.position = new Vector3(transform.position.x, 7.8f, transform.position.z);
    }
}
