using UnityEngine;


public class SiPasFiniMission2 : MonoBehaviour
{
    //Murs invisibles pour emp�cher le perso de quitter le village tant qu'il n'a pas fini la mission 2
    [SerializeField] private GameObject murInvisibleHaut;
    [SerializeField] private GameObject murInvisibleBas;
    [SerializeField] private GameObject murInvisibleGauche;
    [SerializeField] private GameObject murInvisibleDroite;

    //Panneau si le perso a pas fini la mission 2
    [SerializeField] private GameObject panneauPasFini2;

    //SO "InfosPerso" du perso
    [SerializeField] private InfosPerso infosPerso;


    //Si le collider entre en contact avec le tag "Player" ET que le perso a moins que 5 pi�ces
    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player" && infosPerso.nbPieces < 5)
        {
            //Active les murs invisibles du village pour emp�cher le perso de sortir du village
            murInvisibleHaut.SetActive(true);
            murInvisibleBas.SetActive(true);
            murInvisibleGauche.SetActive(true);
            murInvisibleDroite.SetActive(true);

            //Active le panneau que le perso n'a pas fini la mission 2
            panneauPasFini2.SetActive(true);

            //Invoque la m�thode "EnleverPanneaux" dans 2 secondes
            Invoke("EnleverPanneaux", 2);
        }
    }


    //Permet de d�sactiver les panneaux actifs apr�s qu'ils aient �t� actifs
    void EnleverPanneaux()
    {
        panneauPasFini2.SetActive(false);
    }
}
