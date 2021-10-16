//Karen Rayssa Pereira Ruas 21242
//Yngrid Vitória Sá Baeta 21694
using System;
using System.IO;
using System.Windows.Forms;


namespace apVetorObjeto
{
    public partial class FrmForca : Form
    {
        VetorDicionario asPalavras;
        string palavras;
        int posicaoDeInclusao;
        int segundos = 60000;
        int quantasLetras;
        int numDePalavras;
        int pontuacao = 0;
        int erros = 0;

        public FrmForca()
        {
            InitializeComponent();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmFunc_Load(object sender, EventArgs e)
        {
            int indice = 0;
            tsBotoes.ImageList = imlBotoes;
            foreach (ToolStripItem item in tsBotoes.Items)
                if (item is ToolStripButton) // se não é separador:
                    (item as ToolStripButton).ImageIndex = indice++;

            asPalavras = new VetorDicionario(20); // instancia com vetor dados com 20 posições

            if (dlgAbrir.ShowDialog() == DialogResult.OK)
            {
                var arquivo = new StreamReader(dlgAbrir.FileName);
                while (!arquivo.EndOfStream)
                {

                    Dicionario dadoLido = new Dicionario();
                    dadoLido.LerDados(arquivo); // método da classe Funcionario
                    asPalavras.Incluir(dadoLido);   // método de VetorFuncionario – inclui ao final
                    asPalavras.ExibirDados(dgvDicionario);
                }


                arquivo.Close();
                asPalavras.PosicionarNoPrimeiro(); // posiciona no 1o registro a visitação nos dados
                AtualizarTela();               // mostra na tela as informações do registro visitado agora 
            }
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            asPalavras.PosicionarNoPrimeiro();
            AtualizarTela();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            asPalavras.RetrocederPosicao();
            AtualizarTela();
        }

        private void AtualizarTela()
        {
            if (!asPalavras.EstaVazio)
            {
                int indice = asPalavras.PosicaoAtual;
                edPalavra.Text = asPalavras[indice].Palavra + "";
                edDica.Text = asPalavras[indice].Dica;
                TestarBotoes();
                stlbMensagem.Text = "Registro " + (asPalavras.PosicaoAtual + 1) +
                "/" + asPalavras.Tamanho;
            }
        }

        private void TestarBotoes()
        {
            btnInicio.Enabled = true;
            btnAnterior.Enabled = true;
            btnProximo.Enabled = true;
            btnUltimo.Enabled = true;
            if (asPalavras.EstaNoInicio)
            {
                btnInicio.Enabled = false;
                btnAnterior.Enabled = false;
            }

            if (asPalavras.EstaNoFim)
            {
                btnProximo.Enabled = false;
                btnUltimo.Enabled = false;
            }
        }
        private void LimparTela()
        {
            edPalavra.Clear();
            edDica.Clear();
        }

        private void btnProximo_Click(object sender, EventArgs e)
        {
            asPalavras.AvancarPosicao();
            AtualizarTela();
        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            asPalavras.PosicionarNoUltimo();
            AtualizarTela();
        }

        private void FrmFunc_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dlgAbrir.FileName != "")  // foi selecionado um arquivo com dados
                asPalavras.GravarDados(dlgAbrir.FileName);
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            // saímos do modo de navegação e entramos no modo de inclusão:
            asPalavras.SituacaoAtual = Situacao.incluindo;

            // preparamos a tela para que seja possível digitar dados do novo funcionário
            LimparTela();

            edPalavra.ReadOnly = false;
            // colocamos o cursor no campo chave
            edPalavra.Focus();

            // Exibimos mensagem no statusStrip para instruir o usuário a digitar dados
            stlbMensagem.Text = "Digite a nova palavra do jogo.";
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (asPalavras.SituacaoAtual == Situacao.incluindo)  // só guarda novo funcionário no vetor se estiver incluindo
            {
                // criamos objeto com o registro do novo funcionário digitado no formulário
                var novaPalavra = new Dicionario(edPalavra.Text, edDica.Text);

                asPalavras.Incluir(novaPalavra, posicaoDeInclusao);

                asPalavras.SituacaoAtual = Situacao.navegando;  // voltamos ao mode de navegação

                asPalavras.PosicaoAtual = posicaoDeInclusao;

                AtualizarTela();
            }
            else
              if (asPalavras.SituacaoAtual == Situacao.editando)
            {
                asPalavras[asPalavras.PosicaoAtual].Palavra = edPalavra.Text;
                asPalavras[asPalavras.PosicaoAtual].Dica = (edDica.Text);
                asPalavras.SituacaoAtual = Situacao.navegando;
            }
            btnSalvar.Enabled = false;    // desabilita pois a inclusão terminou
            edPalavra.ReadOnly = true;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja realmente excluir?", "Exclusão",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                asPalavras.Excluir(asPalavras.PosicaoAtual);
                if (asPalavras.PosicaoAtual >= asPalavras.Tamanho)
                    asPalavras.PosicionarNoUltimo();
                AtualizarTela();
            }
        }

        private void btnProcurar_Click(object sender, EventArgs e)
        {

            asPalavras.SituacaoAtual = Situacao.pesquisando;  // entramos no modo de busca
            LimparTela();
            edPalavra.ReadOnly = false;
            edPalavra.Focus();
            stlbMensagem.Text = "Digite a palavra procurada";

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            asPalavras.SituacaoAtual = Situacao.navegando;
            AtualizarTela();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            // permitimos ao usuario editar o registro atualmente
            // exibido na tela
            asPalavras.SituacaoAtual = Situacao.editando;
            edPalavra.Focus();
            stlbMensagem.Text = "Digite novo nome e/ou novo salário e pressione [Salvar].";
            btnSalvar.Enabled = true;
            edPalavra.ReadOnly = true;
        }

        private void tpLista_Enter(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void txtPalavra_Leave(object sender, EventArgs e)
        {
            if (asPalavras.SituacaoAtual == Situacao.incluindo ||
          asPalavras.SituacaoAtual == Situacao.pesquisando)
            if (edPalavra.Text == "")
            {
                MessageBox.Show("Digite uma palavra válida!");
                edPalavra.Focus();
            }
            else  // temos um valor digitado no txtPalavra
            {
                string palavraProcurada = (edPalavra.Text);
                int posicao;
                bool achouRegistro = asPalavras.ExisteSemOrdem(palavraProcurada, out posicao);
                switch (asPalavras.SituacaoAtual)
                {
                    case Situacao.incluindo:
                        if (achouRegistro)
                        {
                            MessageBox.Show("Palavra repetida! Inclusão cancelada.");
                            asPalavras.SituacaoAtual = Situacao.navegando;
                            AtualizarTela(); // exibe novamente o registro que estava na tela antes de esta ser limpa
                        }
                        else  // a matrícula não existe e podemos incluí-la no índice ondeIncluir
                        {     // incluí-la no índice ondeIncluir do vetor interno dados de osFunc
                            edDica.Focus();
                            stlbMensagem.Text = "Digite os demais dados e pressione [Salvar].";
                            btnSalvar.Enabled = true;  // habilita quando é possível incluir
                            posicaoDeInclusao = posicao;  // guarda índice de inclusão em variável global
                        }
                        break;

                    case Situacao.pesquisando:
                        if (achouRegistro)
                        {
                            // a variável posicao contém o índice do funcionário que se buscou
                            asPalavras.PosicaoAtual = posicao;   // reposiciona o índice da posição visitada
                            AtualizarTela();
                        }
                        else
                        {
                            MessageBox.Show("Palavra digitada não foi encontrada.");
                            AtualizarTela();  // reexibir o registro que aparecia antes de limparmos a tela
                        }

                        asPalavras.SituacaoAtual = Situacao.navegando;
                        edPalavra.ReadOnly = true;
                        break;
                }
            }
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {

            Random SorteioDaPalavra = new Random(); //sortearPalavras

            Dicionario novaPalavra = new Dicionario();
            palavras = asPalavras[numDePalavras].Palavra.ToUpper();
            segundos = 60; //quantos segundos serão contados
            quantasLetras = palavras.Length; // quantidade de letrar que a palavra possui
            tmrSegundos.Enabled = true; //mostrar segundos
            dgvRecebeLetra.ColumnCount = quantasLetras; //quantidade de letras da palavra no dgv
            dgvRecebeLetra.RowCount = 1;
            lbPontos.Text = " "; //exibir pontuação

            for (int i = 0; i < quantasLetras; i++)
            {
                dgvRecebeLetra.Rows[0].Cells[1].Value = null;
            }

            if (cbDica.Checked) 
            {
                lbDica.Text = " " + asPalavras[numDePalavras].Dica;
                tmrSegundos.Enabled = true; //contar 
            }

        }

        private void tmrSegundos_Tick(object sender, EventArgs e)
        {
            if (segundos > 0) //contagem dos segundos do jogo
            {
                segundos -= 1;
                lbSegundos.Text = segundos + "s"; //mostrar o tempo de jogo na tela
            }
            else
            {
                tmrSegundos.Stop(); //quando o tempo parar
                MessageBox.Show("Você perdeu! Tecle Iniciar para começar um novo jogo", "O tempo acabou", MessageBoxButtons.OK);
            }
        }
    }
}
