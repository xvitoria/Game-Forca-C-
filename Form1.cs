//Yngrid Vitória Sa Baeta 21694
//Karen Rayssa Pereira Ruas 21242

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace apVetorObjeto
{
    public partial class FrmForca : Form
    {
        //variaveis
        VetorDicionario asPalavras;
        Dicionario asLetras;
        int posicaoDeInclusao;
        int segundos = 60000;
        int numDeErros = 0;
        int quantasLetras;
        int pontos = 1;

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

            asPalavras = new VetorDicionario(100); // instancia com vetor dados com 100 posições

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
                asPalavras.ExibirDados(dgvDicionario);
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
            foreach (var elemento in pLetras.Controls)
            {
                var botao = elemento as Button;
                botao.BackColor = Color.Black;
            }
            (sender as Button).Enabled = false; //o botão não podera ser clicado novamente quando o jogo inciciar
            Button oBotao = (Button)sender;
            Random SorteioDaPalavra = new Random(); //sortear Palavras
            int sorteio = SorteioDaPalavra.Next(asPalavras.Tamanho);
            asLetras = new Dicionario(asPalavras[sorteio].Palavra.ToUpper().Trim(), asPalavras[sorteio].Dica);
            segundos = 60; //quantos segundos serão contados
            quantasLetras = asLetras.Palavra.Length; // quantidade de letrar que a palavra possui
            dgvRecebeLetra.ColumnCount = quantasLetras; //quantidade de letras da palavra no dgv
            dgvRecebeLetra.RowCount = 1;
            lbPontos.Text = " "; //exibir pontuação

            for (int i = 0; i < quantasLetras; i++) //condição
            {
                dgvRecebeLetra.Rows[0].Cells[i].Value = null;
            }

            if (cbDica.Checked) //se o checkbox estiver "ativado"
            {
                lbDica.Text = " " + asPalavras[sorteio].Dica;
                tmrSegundos.Enabled = true; //contagem de tempo
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
                ImagensPerdeu();
                tmrSegundos.Stop(); //quando o tempo acabar
                if (MessageBox.Show("Você perdeu! Deseja iniciar um novo jogo?", "Tempo esgotado!", MessageBoxButtons.YesNo) == DialogResult.Yes) //mensagem que aparece caso o tempo seja esgotado
                {
                    btnIniciar.Enabled = true;
                    reiniciarJogo();
                }
                else Close(); //caso tecle não, o form é fechado
            }
        }

        void VencerJogo() // exibir imagens caso o jogador vença o jogo
        {
            pbCabecaFeliz.Visible = true;
            pbPescoco.Visible = true;
            pbBarriga.Visible = true;
            pbBracoDireito.Visible = true;
            pbBracoEsq.Visible = true;
            pbPernaDireita.Visible = true;
            pbPernaEsquerda.Visible = true;
            pbQuadril.Visible = true;
            pbBandeira.Visible = true;
            pbMtBandeira.Visible = true;
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;
        }

        private void ImagensPerdeu() //metodo que exibe as imagens caso a pessoa perca o jogo
        {
            pbCabeca.Visible = true; pbCabeca.BringToFront();
            pbPescoco.Visible = true; pbPescoco.BringToFront();
            pbBarriga.Visible = true;
            pbBracoDireito.Visible = true;
            pbBracoEsquerdo.Visible = true;
            pbQuadril.Visible = true;
            pbPernaDireita.Visible = true;
            pbPernaEsquerda.Visible = true;
            pbCabecaEnforcada.Visible = true; pbCabecaEnforcada.BringToFront(); //com este metodo a imagem fica a frente de todas as outras que estão na sua mesma posição
            pbAlma.Visible = true;
        }


        private void btnA_Click(object sender, EventArgs e)
        {
            bool ganhou = true;
            Button botao = (Button)sender;
            char letra = (sender as Button).Text[0]; //método para todos os botões de letras
            (sender as Button).Enabled = false;
            if (asLetras.ProcurarLetra(letra))
            {
                asLetras.ExibirLetra(dgvRecebeLetra); //exibe as letras clicadas do DataGridView
                lbPontos.Text = "" + pontos++; //caso a pessoa ganhe a quantidade de pontos começa a ser contada
                botao.BackColor = Color.Green;
                for (int i = 0; i < dgvRecebeLetra.ColumnCount; i++)
                {
                    if (dgvRecebeLetra.Rows[0].Cells[i].Value == null) //linhas do dgv/condição
                    {
                        ganhou = false;
                    }
                }
                if (ganhou)
                {
                    VencerJogo();
                    if (MessageBox.Show("Deseja jogar novamente?", "Você Ganhou!", MessageBoxButtons.YesNo) == DialogResult.Yes) //mensagem que aparece caso o jogador ganhe
                    {
                        btnIniciar.Enabled = true; //metodos do botao
                        reiniciarJogo();
                    }
                    else Close();
                }
            }

            else
            {
                lbErros.Text = "" + ++numDeErros;
                botao.BackColor = Color.Red;
                PiscaLed(numDeErros);
                switch (numDeErros) //condição de erros, mostrar imagens dos erros
                {
                    case 1:
                        pbCabeca.Visible = true; pbCabeca.BringToFront();
                        break;
                    case 2:
                        pbPescoco.Visible = true; pbPescoco.BringToFront();
                        break;
                    case 3:
                        pbBarriga.Visible = true;
                        break;
                    case 4:
                        pbBracoDireito.Visible = true;
                        break;
                    case 5:
                        pbBracoEsquerdo.Visible = true;
                        break;
                    case 6:
                        pbQuadril.Visible = true;
                        break;
                    case 7:
                        pbPernaDireita.Visible = true;
                        break;
                    case 8:
                        pbPernaEsquerda.Visible = true;
                        pbCabecaEnforcada.Visible = true; pbCabecaEnforcada.BringToFront(); //com este metodo a imagem fica a frente de todas as outras que estão na sua mesma posição
                        pbAlma.Visible = true;
                        if (MessageBox.Show("Quantidade de erros esgotadas! Gostaria jogar novamente?", "Você perdeu!", MessageBoxButtons.YesNo) == DialogResult.Yes) //mensagem que aparece caso o jogador perca
                        {
                            btnIniciar.Enabled = true;
                            reiniciarJogo();
                        }
                        else Close(); //fecha o form
                        break;
                }
            }
        }

        private void reiniciarJogo()
        {
            Random SorteioDaPalavra = new Random(); //sortear Palavras
            int sorteio = SorteioDaPalavra.Next(asPalavras.Tamanho - 1);
            asLetras = new Dicionario(asPalavras[sorteio].Palavra.ToUpper().Trim(), asPalavras[sorteio].Dica);
            segundos = 60; //quantos segundos serão contados
            quantasLetras = asLetras.Palavra.Length; // quantidade de letrar que a palavra possui
            dgvRecebeLetra.ColumnCount = quantasLetras; //quantidade de letras da palavra no dgv
            dgvRecebeLetra.RowCount = 1;
            lbPontos.Text = " "; //exibir pontuação
            pbAlma.Visible = false;
            pbCabecaFeliz.Visible = false;
            pbPescoco.Visible = false;
            pbCabeca.Visible = false;
            pbBarriga.Visible = false;
            pbBracoEsquerdo.Visible = false;
            pbBracoDireito.Visible = false;
            pbBracoEsq.Visible = false;
            pbCabecaEnforcada.Visible = false;
            pbPernaDireita.Visible = false;
            pbPernaEsquerda.Visible = false;
            pbQuadril.Visible = false;
            pbBandeira.Visible = false;
            pbMtBandeira.Visible = false;
            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = true;
            pictureBox5.Visible = true;
            pictureBox6.Visible = true;
            pictureBox7.Visible = true;
            btnA.Enabled = true;
            btnB.Enabled = true;
            btnC.Enabled = true;
            btnD.Enabled = true;
            btnE.Enabled = true;
            btnF.Enabled = true;
            btnG.Enabled = true;
            btnH.Enabled = true;
            btnI.Enabled = true;
            btnJ.Enabled = true;
            btnK.Enabled = true;
            btnL.Enabled = true;
            btnM.Enabled = true;
            btnN.Enabled = true;
            btnO.Enabled = true;
            btnP.Enabled = true;
            btnQ.Enabled = true;
            btnR.Enabled = true;
            btnS.Enabled = true;
            btnT.Enabled = true;
            btnU.Enabled = true;
            btnV.Enabled = true;
            btnW.Enabled = true;
            btnX.Enabled = true;
            btnY.Enabled = true;
            btnZ.Enabled = true;
            button1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
            button12.Enabled = true;
            button13.Enabled = true;
            dgvRecebeLetra.Enabled = true;

            foreach (var elemento in pLetras.Controls)
            {
                var botao = elemento as Button;
                botao.BackColor = Color.Black;
            }

            for (int i = 0; i < quantasLetras; i++) //condição
            {
                dgvRecebeLetra.Rows[0].Cells[i].Value = null;
            }

            if (cbDica.Checked) //se o checkbox estiver "ativado"
            {

                lbDica.Text = " " + asPalavras[sorteio].Dica;
                tmrSegundos.Enabled = true; //contagem de tempo
            }
            numDeErros = 0;
            lbErros.Text = "___";
            pontos = 0;
            lbPontos.Text = "___";
        }

        private void cbSerial_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSerial.Checked)
            {
                /* Muda o nome porta e  Abre a porta serial */
                sp.PortName = txtCom.Text;
                try
                {
                    sp.Open();
                }
                catch (Exception)
                {
                    MessageBox.Show("Erro ao abrir porta serial ...");
                    cbSerial.Checked = false;
                    return;
                }
            }
            else
            {
                sp.Close();  //fechar porta serial
            }
            HabilitaControles(cbSerial.Checked);
        }
        private void HabilitaControles(bool estado)
        {
            btnSend.Enabled = estado;
            txtCom.Enabled = !estado;
        }

        private void PiscaLed(int numDeErros)
        {
            if (cbSerial.Checked) //condição para o arduíno (caso ele esteja marcado, o método execultara sua função, se não estiver, não execultará)
            {
                string str = "";
                if (numDeErros == 1)
                    str = "A";
                if (numDeErros == 2)
                    str = "B";
                if (numDeErros == 3)
                    str = "C";
                if (numDeErros == 4)
                    str = "D";
                if (numDeErros == 5)
                    str = "E";
                if (numDeErros == 6)
                    str = "F";
                if (numDeErros == 7)
                    str = "G";
                if (numDeErros == 8)
                    str = "H";

                sp.Write(str);
            }
        }
    }
}



