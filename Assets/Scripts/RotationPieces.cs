using UnityEngine;


public class RotationPieces : MonoBehaviour
{
    //Est appel�e � chaque frame
    void Update()
    {
        //Rotate sur son axe des Y
        transform.Rotate(0, 0.4f, 0);
    }
}
