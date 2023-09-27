using UnityEngine;


public class Limites : MonoBehaviour
{
    //Panneau avec le message que le perso peut pas aller au delà des limites
    [SerializeField] private GameObject panneauLimites;


    //Si son collider entre entre en contact avec le tag "Player"
    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            //Rend le pnneau actif
            panneauLimites.SetActive(true);
        }
    }


    //Si le tag "Player" quitte son collider
    void OnTriggerExit(Collider collision)
    {
        //Désactive le panneau
        panneauLimites.SetActive(false);
    }
}
