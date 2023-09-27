using UnityEngine;


public class Ennemis : MonoBehaviour
{
    //Prefabs des ennemis
    [SerializeField] private GameObject ennemiGros;
    [SerializeField] private GameObject ennemiPetit;

    //Liste des lieux o� les ennemis apparaissent
    [SerializeField] private Vector3[] listeEmplacementsAleatoiresGros;
    [SerializeField] private Vector3[] listeEmplacementsAleatoiresPetits;

    //Nombre de Gros ennemis et leur nombre max 
    private int nbGrosEnnemis = 0;
    private const int nbGrosEnnemisMax = 5;

    //Nombre de Petits ennemis et leur nombre max 
    private int nbPetitsEnnemis = 0;
    private const int nbPetitsEnnemisMax = 5;


    //Est appel�e au lancement de la sc�ne
    void Start()
    {
        CreerAleatoireGros();
        CreerAleatoirePetits();
    }


    //Fait en sorte que les Gros ennemis apparaissent al�atoirement sur la sc�ne selon leur liste
    void CreerAleatoireGros()
    {
        //Si le nombre de Gros ennemis est inf�rieur au nombre max, fait apparaitre cet ennemi al�atoirement
        if (nbGrosEnnemis < nbGrosEnnemisMax)
        {
            //Augmente le nombre total de cet ennemi de 1
            nbGrosEnnemis++;

            //Valeur al�atoire dans la liste des endroits d'apparition 
            int nbAleatoire = Random.Range(0, listeEmplacementsAleatoiresGros.Length);

            //Fait apparaitre l'ennemi de mani�re al�atoire
            GameObject unEnnemiGros = Instantiate(ennemiGros, new Vector3(0, 0, 0), Quaternion.identity);
            unEnnemiGros.transform.position = listeEmplacementsAleatoiresGros[nbAleatoire];

            //Met l'ennemi dans le parent attitr� et lui donne le bon nom
            unEnnemiGros.transform.parent = GameObject.Find("EnnemisGros").transform;
            unEnnemiGros.name = "EnnemiGros " + nbGrosEnnemis;

            //Invoque le d�lai entre les apparitions d'ennemis
            Invoke("CreerAleatoireGros", 7);
        }
    }


    //Fait en sorte que les Petits ennemis apparaissent al�atoirement sur la sc�ne selon leur liste
    void CreerAleatoirePetits()
    {
        //Si le nombre de Petits ennemis est inf�rieur au nombre max, fait apparaitre cet ennemi al�atoirement
        if (nbPetitsEnnemis < nbPetitsEnnemisMax)
        {
            //Augmente le nombre total de cet ennemi de 1
            nbPetitsEnnemis++;

            //Valeur al�atoire dans la liste des endroits d'apparition 
            int nbAleatoire = Random.Range(0, listeEmplacementsAleatoiresPetits.Length);

            //Fait apparaitre l'ennemi de mani�re al�atoire
            GameObject unEnnemiPetit = Instantiate(ennemiPetit, new Vector3(0, 0, 0), Quaternion.identity);
            unEnnemiPetit.transform.position = listeEmplacementsAleatoiresPetits[nbAleatoire];

            //Met l'ennemi dans le parent attitr� et lui donne le bon nom
            unEnnemiPetit.transform.parent = GameObject.Find("EnnemisPetits").transform;
            unEnnemiPetit.name = "EnnemiPetit " + nbPetitsEnnemis;

            //Invoque le d�lai entre les apparitions d'ennemis
            Invoke("CreerAleatoirePetits", 7);
        }
    }
}
