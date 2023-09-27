using UnityEngine;


//Permet de cr�er un SO dans le dossier Scripts
[CreateAssetMenu(fileName = "InfosPerso", menuName = "SO/NouveauPerso")]


public class InfosPerso : ScriptableObject
{
    //Nb de pi�ces
    public int nbPieces = 0;

    //Nb d'ennemis tu�s
    public int nbEnnemis = 0;
}