using UnityEngine;


public class HauteurPetits : MonoBehaviour
{
    //Est appelée à chaque frame
    void Update()
    {
        //Fait en sorte que les ennemis restent à une hauteur (sur l'axe des Y) constante
        transform.position = new Vector3(transform.position.x, 4f, transform.position.z);
    }
}
