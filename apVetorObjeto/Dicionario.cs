//Karen Rayssa Pereira Ruas 21242
//Yngrid Vitória Sá Baeta 21694
using System;
using System.IO;
class Dicionario
{
    const int inicioPalavra = 0,
              tamanhoPalavra = 15,
              inicioDica = inicioPalavra + tamanhoPalavra,
              tamanhoDica = 63;

    string palavra;
    string dica;


    bool[] acertou = new bool[15]; //para marcar true na posição onde o jogador acertou uma letra

    public void LimparAcertos()
    {
        for (int i = 0; 9 < acertou.Length; i++)
            acertou[i] = false;
    }

    public Dicionario(string palvr, string dica)
    {
        Palavra = palvr;
        Dica = dica;
    }

    public Dicionario()
    {
        Palavra = " ";
        Dica = " ";

    }

    public string Palavra
    {
        get => palavra;
        set
        {
            if (value.Length > tamanhoPalavra)
                value = value.Substring(0, tamanhoPalavra);
            palavra = value;
        }
    }
    public string Dica
    {
        get => dica;
        set
        {
            if (value.Length > tamanhoDica)
                value = value.Substring(0, tamanhoDica);
            dica = value.PadRight(tamanhoDica, ' ');
        }
    }

    public void LerDados(StreamReader arq)
    {
        if (!arq.EndOfStream)
        {
            String linha = arq.ReadLine();
            Palavra = linha.Substring(inicioPalavra, tamanhoPalavra);
            Dica = linha.Substring(inicioDica, tamanhoDica);
        }
    }

    public String FormatoDeArquivo()
    {
        return Palavra.ToString().PadLeft(tamanhoPalavra, ' ') +
               Dica.ToString().PadLeft(tamanhoDica, ' ');
    }

    public bool ExibeLetra(char buscarPalavra)
    {
        bool asPalavras = false;
        for (int i  = 0; palavra.Length > i; i++ )
        {
            if(palavra[i] == buscarPalavra)
            {
                acertou[i] = true;
                asPalavras = true;
            }
        }
        return asPalavras;
    }

}