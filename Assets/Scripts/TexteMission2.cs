using UnityEngine;


public class TexteMission2 : MonoBehaviour
{
    //Panneaux de la mission 2
    [SerializeField] private GameObject panneauMission2;

    //Colliders ayant le script "SiPasFiniMission2"
    [SerializeField] private GameObject siPasFini2Haut;
    [SerializeField] private GameObject siPasFini2Bas;
    [SerializeField] private GameObject siPasFini2Gauche;
    [SerializeField] private GameObject siPasFini2Droite;


    //Si le collider de la mission 2 entre en contact avec le tag "Player"
    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            //Rend le panneau de la mission 2 actif
            panneauMission2.SetActive(true);
        }
    }


    //Si le tag "Player" sort du collider de la mission 2
    void OnTriggerExit(Collider collision)
    {
        //Désactive le panneau de la mission 2
        panneauMission2.SetActive(false);

        //Active les colliders du village pour empêcher le perso de sortir du village
        siPasFini2Haut.SetActive(true);
        siPasFini2Bas.SetActive(true);
        siPasFini2Gauche.SetActive(true);
        siPasFini2Droite.SetActive(true);
    }
}
