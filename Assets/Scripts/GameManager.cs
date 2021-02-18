using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int numTentativas;          //tentativas válidas da rodada  
    private int numMaxTentativas;       //máximo de tentativas para forca
    int score = 0;

    public GameObject letra;            //prefab da letra
    public GameObject centro;           //Objeto que indica o centro da tela

    private string palavraOculta = "";  //palavra a ser descoberta 
    // private string[] palavrasOcultas = new string[] { "GolTurbo", "Marea", "Santana" }; //array de palavras a serem descobertas  (usado em partes anteriores do lab)
    private int tamanhoPalavraOculta;   //tamanho da palavra oculta
    char[] letrasOcultas;               //vetor de letras da palavra oculta
    bool[] letrasDescobertas;           //indicador de letras descobertas

    // Start is called before the first frame update
    void Start()
    {
        centro = GameObject.Find("centrodatela"); // buscar o centro da tela
        InitGame();
        InitLetras();
        numTentativas = 0;
        numMaxTentativas = 10;
        UpadateNumTentativas();
        UpadateScore();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTeclado();
    }

    void InitLetras() // cria as letras da palavra
    {
        int numLetras = tamanhoPalavraOculta;
        for(int i=0; i<numLetras; i++)
        {
            Vector3 novaPosicao;
            novaPosicao = new Vector3(centro.transform.position.x + ((i-numLetras/2.0f)*80), centro.transform.position.y, centro.transform.position.z); //posição das letras
            GameObject l = (GameObject)Instantiate(letra, novaPosicao, Quaternion.identity);
            l.name = "letra" + (i + 1);                                 //nomeia-se na hierarquia a GameObject com letra iésima+1, i = l.numletras
            l.transform.SetParent(GameObject.Find("Canvas").transform); //posiciona-se como filho do GameObject Canvas
        }
    }
   
    void InitGame()
    {
        // int numAleatorio = Random.Range(0, palavrasOcultas.Length); //sorteia um número (usado na parte 2 do lab)
        // palavraOculta = palavrasOcultas[numAleatorio];              //seleciona a palavra com o número sorteado (usado na parte 2 do lab)

        palavraOculta = PegaUmaPalavraDoArquivo();
        tamanhoPalavraOculta = palavraOculta.Length;        //número de letras da palavra oculta
        palavraOculta = palavraOculta.ToUpper();            //transforma a palavra para maiúscula
        letrasOcultas = new char[tamanhoPalavraOculta];     //array de letras
        letrasDescobertas = new bool[tamanhoPalavraOculta]; //array com o idicador de letras certas
        letrasOcultas = palavraOculta.ToCharArray();        //copia da palavra no array de letras
    }

    void CheckTeclado()
    {
        if(Input.anyKeyDown)
        {
            char letraTeclada = Input.inputString.ToCharArray()[0];
            int letraTecladaInt = System.Convert.ToInt32(letraTeclada);

            if (letraTecladaInt >= 97 && letraTecladaInt <= 122)
            {
                numTentativas++;
                UpadateNumTentativas();
                if (numTentativas > numMaxTentativas)
                {
                    SceneManager.LoadScene("Lab1_Forca");
                }
                for(int i=0; i<=tamanhoPalavraOculta; i++)
                {
                    if (!letrasDescobertas[i])
                    {
                        letraTeclada = System.Char.ToUpper(letraTeclada);
                        if (letrasOcultas[i] == letraTeclada)
                        {
                            letrasDescobertas[i] = true;
                            GameObject.Find("letra" + (i + 1)).GetComponent<Text>().text = letraTeclada.ToString();
                            score = PlayerPrefs.GetInt("score");
                            score++;
                            PlayerPrefs.SetInt("score", score);
                            UpadateScore();
                            VerificaSeDescoberta();
                        }
                    }
                }
            }
        }
    }

    void UpadateNumTentativas()
    {
        GameObject.Find("numTentativas").GetComponent<Text>().text = numTentativas + " | " + numMaxTentativas;
    }

    void UpadateScore()
    {
        GameObject.Find("scoreUI").GetComponent<Text>().text = "Score" + score;
    }

    void VerificaSeDescoberta()
    {
        bool condicao = true;
        for (int i=0; i<tamanhoPalavraOculta; i++)
        {
            condicao = condicao && letrasDescobertas[i];
        }
        if (condicao)
        {
            PlayerPrefs.SetString("ultimaPalavraOculta", palavraOculta);
            SceneManager.LoadScene("Lab1_Salvo");
        }
    }

    string PegaUmaPalavraDoArquivo()
    {
        TextAsset t1 = (TextAsset)Resources.Load("Palavras", typeof(TextAsset));
        string s = t1.text;
        string[] palavras = s.Split(' ');
        int palavraAleatoria = Random.Range(0, palavras.Length);
        return palavras[palavraAleatoria];
    }
}
