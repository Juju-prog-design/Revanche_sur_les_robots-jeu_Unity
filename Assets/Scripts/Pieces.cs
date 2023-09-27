using UnityEngine;

public class Pieces : MonoBehaviour
{
    //Prefabs des pi�ces
    [SerializeField] private GameObject pieces;

    //Liste des lieux o� les pi�ces apparaissent
    [SerializeField] private Vector3[] listeEmplacementsAleatoiresPieces;

    //Nombre de pi�ces et leur nombre max 
    private int nbPieces = 0;
    private const int nbPiecesMax = 5;


    //Est appel�e au lancement de la sc�ne
    void Start()
    {
        CreerAleatoirePieces();
    }


    //Fait en sorte que les pi�ces apparaissent al�atoirement sur la sc�ne selon leur liste
    void CreerAleatoirePieces()
    {
        //Si le nombre de pi�ces est inf�rieur au nombre max, fait apparaitre une pi�ce al�atoirement
        if (nbPieces < nbPiecesMax)
        {
            //Augmente le nombre total de pi�ces de 1
            nbPieces++;

            //Valeur al�atoire dans la liste des endroits d'apparition 
            int nbAleatoire = Random.Range(0, listeEmplacementsAleatoiresPieces.Length);

            //Fait apparaitre la pi�ce de mani�re al�atoire
            GameObject unePiece = Instantiate(pieces, new Vector3(0, 0, 0), Quaternion.identity);
            unePiece.transform.position = listeEmplacementsAleatoiresPieces[nbAleatoire];

            //Met la pi�ce dans le parent attitr� et lui donne le bon nom
            unePiece.transform.parent = GameObject.Find("Pieces").transform;
            unePiece.name = "Piece " + nbPieces;

            //Invoque le d�lai entre les apparitions des pi�ces
            Invoke("CreerAleatoirePieces", 7);
        }
    }
}
