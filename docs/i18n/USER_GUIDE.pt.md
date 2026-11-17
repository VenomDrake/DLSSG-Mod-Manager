# DLSSG Mod Manager — tutorial completo de uso

**Idioma:** [English](../USER_GUIDE.md) · [Italiano](USER_GUIDE.it.md) · [简体中文](USER_GUIDE.zh-Hans.md) · [हिन्दी](USER_GUIDE.hi.md) · [Español](USER_GUIDE.es.md) · [Français](USER_GUIDE.fr.md) · [العربية](USER_GUIDE.ar.md) · [বাংলা](USER_GUIDE.bn.md) · **Português** · [Русский](USER_GUIDE.ru.md) · [اردو](USER_GUIDE.ur.md)

Para o **gerenciador 0.5.2** no **Windows 10/11 x64**. Os nomes abaixo se baseiam na interface em inglês e são apresentados aqui traduzidos; escolha seu idioma no canto superior direito do aplicativo. Este é um gerenciador independente para os projetos originais, e não um aplicativo oficial da NVIDIA, AMD, Intel, ANR ou OptiScaler.

**Conteúdo:** [Início rápido](#start) · [Segurança](#safety) · [Download](#install) · [Escolher uma tecnologia](#choose-provider) · [Tela e botões](#screen) · [Primeira instalação](#first-install) · [Verificações](#checks) · [Configurações SM86](#sm86) · [Configurações ANR](#anr) · [Configurações OptiScaler](#optiscaler) · [Configurações no jogo](#in-game) · [Confirmação do resultado](#verify) · [Atualizações](#updates) · [Desativar e restaurar](#backup) · [Solução de problemas](#troubleshooting) · [Arquivos e ajuda](#files) · [Glossário](#glossary) · [Fontes](#sources)

<a id="start"></a>
## 1. Início rápido: seu primeiro teste

No aplicativo, abra **Tutorial** e escolha **Iniciar tour guiado** para seguir oito etapas práticas. Pesquise os tópicos, use **?** ao lado de uma configuração ou pressione **F1** enquanto seu controle está em foco. **Mostrar no gerenciador** abre e destaca os controles correspondentes sem aplicar alterações.

1. Feche o jogo. Escolha um **jogo offline para um jogador**.
2. Abra o gerenciador e deixe a verificação do pacote da tecnologia selecionada terminar.
3. Escolha a tecnologia e selecione o jogo no Steam ou escolha sua pasta/EXE. Deixe o gerenciador analisá-lo.
4. Leia **2 · Confira o resultado**. Abra **Diagnóstico** para ver a lista completa. Resolva todas as verificações **Bloqueadas**; leia os avisos antes de continuar.
5. Em **Configurações do mod**, mantenha os padrões do primeiro teste: **SM86: DLSS, 2×**; **ANR: Auto, 2×**; **OptiScaler: ampliação automática, entrada FG desativada e saída FG desativada**.
6. Volte para **Gerenciamento de jogos**. Escolha **Instalar mod**, confira o destino e as configurações na confirmação e confirme. Aguarde a conclusão.
7. Inicie o jogo e siga as [etapas no jogo específicas de cada provedor](#in-game). Jogue um pouco, observando o movimento, o HUD e a resposta aos comandos.
8. Leia o registro em **Diagnóstico**. Para OptiScaler, pressione também **Insert** no jogo para inspecionar a saída ativa. Apenas carregar uma DLL não comprova que a geração de quadros funcionou.

Altere **uma configuração por vez** depois de um primeiro teste que funcione. Feche o jogo, aplique a alteração no gerenciador e reinicie o jogo antes de comparar.

<a id="safety"></a>
## 2. Leia antes de modificar um jogo

**Use estes mods somente em jogos offline para um jogador. Arquivos de jogo modificados podem acionar o antitrapaça ou causar banimentos.** O gerenciador não oferece formas de contornar o antitrapaça nem proteção contra banimentos. Um modo offline não garante a ausência de antitrapaça; um antitrapaça detectado continua bloqueando a instalação.

O gerenciador verifica arquivos e pré-requisitos. **“Requisitos detectados” não certifica a compatibilidade durante o jogo, a estabilidade nem um ganho de FPS.** Mantenha suas cópias de segurança habituais das partidas salvas importantes; os backups do gerenciador cobrem as alterações que ele faz nos arquivos do jogo, não todas as partidas salvas nem o jogo inteiro.

Não acumule carregadores manualmente, não copie DLLs de origem desconhecida sobre os arquivos do jogo e não apague um conflito apenas para passar em uma verificação. Examine primeiro o mod existente. Os binários dos projetos originais são baixados dos repositórios originais como dados; o gerenciador não executa seus scripts de instalação.

<a id="install"></a>
## 3. Baixar e abrir o gerenciador

Use as [versões oficiais do gerenciador](https://github.com/VenomDrake/DLSSG-Mod-Manager/releases/latest).

| Download | O que fazer |
|---|---|
| **ZIP portátil** | Extraia o **ZIP inteiro** para uma pasta com permissão de escrita e abra `DLSSGModManager.exe`. Não o execute de dentro do ZIP. Mantenha juntos os arquivos de licença e documentação. |
| **Instalador do Windows (.msi)** | Feche o gerenciador e execute o MSI. Ele instala para seu usuário do Windows e cria atalhos. |

Os pacotes publicados incluem o ambiente de execução .NET do gerenciador. Você não precisa instalar Python, CUDA Toolkit ou um extrator de arquivos separado. Um provedor ainda pode exigir drivers gráficos, Visual C++ ou componentes Vulkan; essas são verificações separadas.

O gerenciador e o MSI não são assinados. Use apenas as versões do projeto que você pretendia baixar; não desative globalmente a segurança do Windows. O primeiro início precisa de internet para obter o pacote de um provedor. Depois, pode ser possível usar offline um pacote verificado em cache, mas **o cache offline não confirma que seja a versão mais recente**.

<a id="choose-provider"></a>
## 4. Qual tecnologia devo escolher?

Escolha em **Ampliação e geração de quadros**. Neste guia, uma tecnologia também é chamada de **provedor**.

| Opção | Finalidade e requisitos para o primeiro teste |
|---|---|
| **DLSSG for SM86 · NVIDIA / XeSS** | O procedimento revisado pelo gerenciador para **NVIDIA RTX 20/30**, driver **R580 ou mais recente**, agendamento de GPU acelerado por hardware (HAGS) configurado e uma biblioteca de execução nativa de DLSS Frame Generation compatível no jogo. O jogo usa DirectX 12. Comece com DLSS e 2×. |
| **Apex Neural Rendering · Beta** | Um provedor FG alternativo para as famílias declaradas compatíveis pelo gerenciador: **GTX 16 / RTX 20–50** e **Radeon RX 5000/6000/7000/9000**. Precisa de DLSS Frame Generation nativo x64 no jogo por DX12/Vulkan, Visual C++ x64 e um carregador Vulkan. GTX 10/Pascal e Intel Arc estão fora do suporte ANR deste gerenciador. Comece com Auto e 2×. O próprio ANR continua em beta. |
| **OptiScaler · Ampliação / FG** | Substitui o ampliador temporal DLSS/FSR/XeSS de um jogo compatível e pode gerenciar os caminhos FG suportados. DLSS Frame Generation nativo **não é necessário para usar apenas ampliação**. NVIDIA, AMD e Intel são possibilidades, mas cada saída tem limites adicionais. Comece com ampliação Auto e FG **desativada/desativada (Off/Off)**. |

HAGS e NVIDIA R580 **não são requisitos universais** herdados por ANR ou OptiScaler. A **saída** DLSS nativa no OptiScaler ainda exige NVIDIA RTX e a biblioteca de execução DLSS do jogo. O OptiScaler não faz todos os mecanismos funcionarem em todas as GPUs.

Os procedimentos revisados pelo gerenciador 0.5.2 são **SM86 0.4.1/0.4.2**, **ANR FG-V.B15/B17** e **OptiScaler 0.9.5**. Um download mais recente do projeto original pode continuar bloqueado até que seus requisitos alterados sejam revisados.

O gerenciador lembra o provedor selecionado; ele **não escolhe automaticamente** o melhor provedor para sua GPU. Selecione uma tecnologia adequada antes de interpretar uma falha de GPU.

<a id="screen"></a>
## 5. Entenda a tela e cada botão principal

A aba **Tutorial** contém este guia completo offline, com navegação por tópicos e pesquisa. O tour guiado permanece ao lado do gerenciador: **Voltar** e **Avançar** percorrem as oito etapas, **Ler a explicação completa** abre o tópico correspondente e **Fechar tour guiado** encerra o tour. Avançar não marca as verificações de compatibilidade como aprovadas; a instalação continua exigindo a revisão e a confirmação normais.

![Tela de gerenciamento de jogos](../screenshot.png)

| Área ou controle | O que faz |
|---|---|
| **Versão do gerenciador** | A versão do aplicativo. Ela é separada da versão do pacote do mod selecionado. |
| **Idioma** | Muda o idioma da interface. Árabe e urdu usam uma disposição da direita para a esquerda. Valores de configuração como `dxgi.dll` mantêm a grafia original. |
| **Seletor de tecnologia** | Escolhe SM86, ANR ou OptiScaler. Alterá-lo verifica novamente o pacote e os pré-requisitos do provedor; não modifica imediatamente o jogo. |
| **Gerenciamento de jogos** | Escolha o jogo, confira o resultado e aplique o mod. |
| **Configurações do mod** | Edite as configurações do provedor selecionado. Apenas editar um campo não grava nada no jogo. |
| **Diagnóstico** | Verificações completas, detalhes do pacote/PC, registros, links de drivers, exportação de relatórios, desativação e restauração. |
| **Jogos Steam instalados / ↻** | Seleciona um jogo Steam detectado; ↻ atualiza as bibliotecas. Um jogo aparecer no Steam não significa que seja compatível com o mod. |
| **Escolher pasta / Escolher EXE** | Seleciona um jogo manualmente, inclusive fora do Steam. Escolha o EXE real de renderização, não um inicializador, um programa de relatórios de falha ou um instalador. |
| **Campo do caminho / Analisar jogo** | Você pode colar o caminho de uma pasta/EXE e analisá-lo. Alterar o caminho limpa o resultado anterior para que ele não seja reutilizado por engano. |
| **Executável de renderização** | Permite escolher entre vários candidatos. Para muitos jogos Unreal, use `GameName\Binaries\Win64\...-Win64-Shipping.exe`, não o pequeno inicializador da pasta principal. |
| **Ver todas as verificações / Ajustar as configurações do mod** | Abre a aba correspondente. |
| **Instalar / Atualizar / Aplicar configurações / Trocar tecnologia** | Uma área de ação cujo texto corresponde à situação atual. Aplica alterações somente após a confirmação e o cumprimento dos pré-requisitos. |
| **Iniciar jogo** | Inicia o jogo selecionado, usando o Steam quando encontra uma correspondência. Não instala o mod. |
| **Cancelar operação** | Solicita uma parada segura. Aguarde o gerenciador terminar uma etapa crítica de aplicação ou reversão das alterações. |

Em **Diagnóstico**, o **↻** do pacote verifica o pacote do projeto original, enquanto o **↻** do PC lê o hardware novamente. **Abrir configurações gráficas do Windows** abre as configurações do Windows; não ativa HAGS para você. Os botões de drivers abrem páginas NVIDIA, AMD ou Intel conforme o provedor/os adaptadores selecionados. **Projeto original** abre o repositório do provedor.

O pequeno aviso **Atualizar e reiniciar** se refere ao **próprio gerenciador**. Seu **×** dispensa o aviso daquela versão na janela atual; não desinstala nada.

<a id="first-install"></a>
## 6. Instalar, alterar configurações ou trocar de tecnologia

1. Termine o download/a verificação do pacote e selecione o EXE de renderização correto.
2. Confira o resumo e todas as verificações. Feche o jogo e qualquer processo do jogo que continue aberto.
3. Defina os padrões do primeiro teste ou as opções que pretende usar. O resultado pode mudar quando uma opção introduz um novo requisito.
4. Pressione o botão de ação inferior. Leia a tecnologia, a pasta de renderização, a versão e as configurações na confirmação.
5. Confirme. O gerenciador prepara os arquivos verificados, cria um backup, desativa os carregadores antigos, altera os dados e ativa o carregador selecionado por último. Deixe-o terminar.
6. Reinicie o jogo. Um jogo que já estava aberto não pode usar com confiabilidade as configurações recém-gravadas.

**“Você já tem a versão mais recente” se refere à identidade do pacote instalado.** Isso não significa que uma configuração que você acabou de mudar já tenha sido aplicada. Use **Aplicar configurações**, com o jogo fechado, para salvar essas alterações.

Em uma instalação gerenciada, trocar de provedor usa o mesmo sistema de backups e histórico. O OptiScaler independente 0.9.5 e o carregador OptiScaler personalizado do ANR são diferentes; o gerenciador não os combina. Um Addon ANR externo pode exigir a restauração da instalação externa antes da troca. Instalações manuais desconhecidas são protegidas em vez de sobrescritas silenciosamente.

<a id="checks"></a>
## 7. Leia as verificações sem adivinhar

| Estado | Significado e ação |
|---|---|
| **Aprovado** | Esta verificação específica foi bem-sucedida. Isso não comprova todos os outros requisitos nem o funcionamento durante o jogo. |
| **Informações** | Contexto, como uma configuração ou um componente opcional. Leia se for relevante. |
| **Aviso / Revisar** | Algo precisa de atenção ou confirmação no jogo. Não bloqueia automaticamente; também não promete que continuar seja seguro ou compatível. |
| **Não confirmado** | O gerenciador não conseguiu estabelecer um fato. Leia o detalhe em vez de presumir que a verificação passou. |
| **Bloqueado** | A instalação não pode continuar até a causa ser resolvida. |

As verificações habituais cobrem Windows/x64, GPU e driver, HAGS quando necessário, componentes Visual C++/Vulkan, identidade e estrutura suportada do pacote, análise completa/permissões, EXE correto, entrada/biblioteca de execução nativa, indícios de API gráfica, indicadores de antitrapaça, carregadores existentes ou modificados, jogo em execução, destinos somente leitura e espaço livre em disco. Backups e preparação precisam de espaço além do tamanho final do mod.

**Não compatível** identifica uma incompatibilidade técnica detectada. **Precisa de revisão** significa que os indícios estão incompletos, que um EXE é ambíguo ou que uma entrada pode estar integrada no jogo. **Instalação bloqueada** pode significar um conflito ou problema de pacote, em vez de um jogo incompatível. Análises estáticas de arquivos não podem comprovar a ausência de antitrapaça nem garantir que o jogo funcione.

<a id="sm86"></a>
## 8. SM86: todas as configurações do gerenciador

![Configurações SM86](../settings-sm86.png)

**Primeiro teste recomendado:** DLSS, 2×, otimização 1, predefinição A, substituição SR desativada, registro 2, registro de tempos desativado, `version.dll`.

| Configuração | Explicação simples e opções |
|---|---|
| **Motor de geração** | **NVIDIA DLSS-G** ou **Intel XeSS-FG** seleciona a implementação FG do mod. Aqui XeSS é a implementação do mod SM86, não uma promessa de suporte geral a GPUs Intel. Comece com DLSS. |
| **Multiplicador de quadros** | DLSS oferece **2×–6×**; XeSS **2×–4×**. Esses valores solicitam um multiplicador: 2× significa um quadro gerado por quadro real; 3× significa dois, e assim por diante. O jogo pode impor um limite menor, e uma solicitação não suportada pode impedir a FG. Comece em 2×. Mais quadros gerados não significam latência proporcionalmente menor nem maior fluidez garantida. |
| **Otimização dos núcleos** | **0:** núcleos originais, úteis para comparação. **1:** acelerações com resultado exato, recomendado. **2:** processamento aproximado mais rápido. **3:** aproximação máxima. Níveis mais altos podem alterar o resultado; compare bordas em movimento e a interface em vez de presumir que o maior número é o melhor. Aplica-se a DLSS-G. |
| **Predefinição DLSS-G** | **A:** opção normal para o primeiro teste. **B:** recomposição da interface quando o jogo fornece buffers adequados sem HUD/da interface; não pode criar esses buffers. **C:** FG mais leve com uma possível perda de qualidade de imagem, especialmente nas bordas do HUD/de oclusão. Aplica-se a DLSS-G. |
| **Substituição de Super Resolution** | **Desativado (Off)** mantém o ampliador do jogo. **M-INT8 / L-INT8** selecionam os núcleos M/L INT8 do projeto. **M-FP16 / L-FP16** selecionam o processamento FP16 correspondente. M/L são opções de modelo, não uma classificação universal de desempenho. As substituições podem reduzir o desempenho nos modos Qualidade, Equilibrado ou DLAA; mantenha desativado no primeiro teste. |
| **Fluxo óptico XeSS** | **Coarse**, **Middle**, **Fine** mudam a resolução do processamento de fluxo. Um processamento mais fino pode consumir mais tempo de GPU. **CoarseAt4K** usa Coarse quando a saída tem pelo menos 3840×2160 pixels; caso contrário, usa Middle. Comece com Middle. Aplica-se a XeSS-FG. |
| **Tratamento do HUD XeSS** | **Auto**, **Exigir imagem sem HUD (1)** ou **Não usar imagem sem HUD (0)**. No SM86 **0.4.2 revisado, Auto se comporta como 0**: a imagem inteira, incluindo a interface, é interpolada. O modo 1 exige que o jogo forneça uma imagem correspondente sem HUD e pode falhar sem ela. |
| **Detalhamento do registro** | **0 Desativado**, **1 Erros**, **2 Diagnóstico (recomendado)**, **3 Rastreamento detalhado**. Mais detalhes ajudam no diagnóstico, mas aumentam o registro. |
| **Registrar tempos dos quadros** | Grava dados adicionais de tempo para investigação. Não define um limite de FPS nem ativa FG. Deixe desativado no primeiro teste. |
| **DLL de carregamento** | `version.dll` (recomendada), `winmm.dll`, `dbghelp.dll`, `dinput8.dll`, `dxgi.dll`, `d3d12.dll`. O nome do arquivo determina como o jogo carrega o mod. Mantenha apenas um carregador ativo; tente alternativas pelo gerenciador se o mod não carregar. Não acumule cópias manualmente. |

DLSS-G e XeSS-FG têm configurações diferentes. Uma opção específica de um motor estar visível não a torna ativa quando o outro motor está selecionado.

**HAGS para SM86:** abra as configurações gráficas do Windows pelo Diagnóstico, ative o agendamento de GPU acelerado por hardware onde o Windows o oferecer e reinicie o Windows. O estado do Registro/da configuração e o estado ativo durante a execução são verificações diferentes. O registro do jogo fornece a confirmação durante a execução. O gerenciador não altera automaticamente o driver nem HAGS.

<a id="anr"></a>
## 9. ANR: todas as configurações do gerenciador

![Configurações ANR](../settings-anr.png)

**Primeiro teste recomendado:** caminho de cálculo Auto, 2×, 80 nits por unidade, tratamento de mudanças de cena ativado, `dxgi.dll`. O motor é ANR; os controles de otimização/predefinição/substituição SR do SM86 não se aplicam.

| Configuração ou modo | Significado |
|---|---|
| **Full / Addon** | Selecionado automaticamente. **Full** inclui a configuração de carregador/provedor revisada. **Addon** adiciona ANR a uma instalação externa compatível do OptiScaler e preserva esse carregador. Uma troca gerenciada de OptiScaler independente → ANR usa Full. Não escolha nem copie manualmente um addon qualquer. |
| **Multiplicador de quadros** | Solicita **2×–6×**, dependendo da integração do jogo. Comece em 2×. O gerenciador grava o número correspondente de quadros extras, não seis quadros extras para a opção 6×. |
| **Caminho de cálculo** | **auto**, **coopmat**, **packed**, **dp4a**, **portable**, **portable-linalg**, **portable-xmx** são formas alternativas de realizar os cálculos do ANR. Não são níveis de qualidade de imagem. Auto usa a lógica de seleção do provedor; caminhos explícitos exigem recursos adequados da GPU, e não há garantia de que todos funcionem ou sejam mais rápidos. Mantenha Auto, a menos que esteja diagnosticando um problema específico. |
| **Luminância HDR** | **40–1000**, padrão **80**, descreve a luminância associada a um valor linear de 1.0 no processamento HDR/PQ do provedor. Não é o controle de brilho do monitor. Mantenha 80, a menos que uma investigação específica de HDR exija uma alteração. |
| **Tratamento de mudanças de cena** | Ativado por padrão; evita a interpolação entre cortes bruscos. Mantenha ativado nos testes normais. |
| **DLL de carregamento** | `dxgi.dll`, `winmm.dll`, `version.dll`; comece com `dxgi.dll`. No modo Addon, o carregador externo é mantido e o seletor fica bloqueado. |

O rótulo **Beta** do ANR descreve o provedor original, mesmo quando o gerenciador é uma versão estável. O ANR precisa dos componentes Visual C++ x64 revisados e do carregador Vulkan mesmo quando a API gráfica do jogo é DX12. O requisito de HAGS do SM86 não é imposto.

<a id="optiscaler"></a>
## 10. OptiScaler: todas as configurações do gerenciador

![Configurações OptiScaler](../settings-optiscaler.png)

**Primeiro teste recomendado:** todos os ampliadores em **Auto**, entrada FG **desativada (Off)**, saída FG **desativada (Off)**, identidade simulada DXGI **Auto**, registro **2 · Informações**, carregador `dxgi.dll`.

### Opções de ampliador

Uma **entrada** é a tecnologia selecionada no próprio menu do jogo. Uma **saída** é o que o OptiScaler realmente executa. Por exemplo, um jogo pode fornecer uma entrada DLSS enquanto o OptiScaler produz uma saída XeSS. Selecionar DLSS como entrada do jogo não comprova por si só que DLSS é a saída ativa.

| Campo do gerenciador | Valores disponíveis |
|---|---|
| **Ampliador DirectX 11** | Auto; FSR 2.2 (`fsr22`); FSR 3.1 (`fsr31`); XeSS nativo (`xess`); XeSS por D3D11on12 (`xess_12`); FSR 2.1/2.2/3.1 por interoperabilidade DX12 (`fsr21_12`, `fsr22_12`, `fsr31_12`); DLSS. |
| **Ampliador DirectX 12** | Auto; XeSS; FSR 2.1; FSR 2.2; FSR 3.1/4; DLSS. |
| **Ampliador Vulkan** | Auto; FSR 2.1/2.2/3.1/4 nativo conforme o suporte do mecanismo; XeSS nativo; FSR 2.1 ou FSR 3.1/4 por interoperabilidade DX12 (`fsr21_12`, `fsr31_12`); DLSS. |

Somente a API ativa do jogo usa a configuração correspondente. **Auto** segue os padrões do OptiScaler; é o ponto de partida mais seguro, não uma promessa da saída mais rápida. `_12` significa usar um caminho de interoperabilidade DX12, o que pode ter um custo adicional.

**XeSS nativo em DX11** exige Intel Arc; `xess_12` é uma opção diferente de interoperabilidade. A **saída DLSS** exige NVIDIA RTX e a biblioteca de execução DLSS x64 do jogo. **FSR 3.1/4** não promete FSR 4 em todas as GPUs: o suporte do mecanismo e do hardware determina o modelo ativo ou a alternativa usada. Confira a saída real na interface sobreposta; o gerenciador não força FSR 4 sem suporte.

No DX11, o caminho nativo `fsr31` é FSR 3.1; o caminho FSR 4 usa interoperabilidade DX12. Um rótulo geral “3.1 / 4” não garante FSR 4 nativo em DX11.

### Entrada e saída de geração de quadros

Para um primeiro teste somente de ampliação, mantenha **ambos os campos desativados (Off)**. Para ativar FG depois, escolha uma entrada **e** uma saída compatíveis; alterar apenas um dos campos para um modo ativo é bloqueado.

| Entrada FG | O que o jogo precisa fornecer |
|---|---|
| **Desativado (Off)** (`nofg`) | Nenhuma FG OptiScaler solicitada. Desative também a saída. |
| **DLSS Frame Generation** (`dlssg`) | DLSS FG nativo com uma integração Streamline 2 adequada; o gerenciador verifica os indícios disponíveis e avisa quando não consegue confirmá-la. |
| **DLSS-G · Nukem** (`nukems`) | Uma biblioteca de execução DLSS-G nativa. Combine esta entrada com a saída **Nukem · FSR 3**. |
| **FSR Frame Generation** (`fsrfg`) | Uma fonte FSR FG nativa compatível no jogo. Apenas o nome de uma DLL não permite estabelecer essa integração. |
| **Entrada do ampliador / OptiFG** (`upscaler`) | Uma entrada de ampliação temporal que funcione, usando o caminho OptiFG DX12 revisado. Pode ser necessário corrigir o HUD na interface sobreposta do projeto original. |
| **FSR 3.0 Frame Generation** (`fsrfg30`) | Uma fonte FSR 3.0 FG nativa compatível. |

| Saída FG | Significado |
|---|---|
| **Desativado (Off)** | Nenhuma saída FG. Desative também a entrada. |
| **AMD FSR Frame Generation** (`fsrfg`) | Usa o mecanismo FidelityFX FG compatível. Não garante a mesma versão/os mesmos recursos FSR em todas as GPUs. |
| **Intel XeSS Frame Generation** (`xefg`) | Exige os recursos de GPU do mecanismo, incluindo suporte DP4a adequado e condições apropriadas do jogo/da tela. Não é ativado para todos os dispositivos Intel, AMD ou NVIDIA apenas porque seu fabricante foi detectado. O suporte a múltiplos quadros tem outros limites no projeto original. |
| **Nukem · FSR 3** (`nukems`) | Usa o caminho DLSSG-to-FSR3 incluído e revisado. Obrigatório quando a entrada selecionada é Nukem. |

O gerenciador 0.5.2 bloqueia a FG selecionada em um caminho **DX11** detectado. Para um caminho **Vulkan** detectado, o par FG revisado e permitido é **Nukem → Nukem**. Os outros caminhos FG oferecidos exigem DX12; indícios de API desconhecidos ainda precisam de confirmação no jogo. Esses controles não tornam compatível uma integração de jogo sem suporte.

### Outros controles e configurações avançadas

| Configuração | Significado |
|---|---|
| **Identidade de GPU simulada no DXGI: Auto / Ativado / Desativado** | Altera a identidade da GPU informada por DXGI para compatibilidade. Não melhora a GPU real nem garante desempenho. Mantenha Auto inicialmente. Não é proteção antitrapaça; use mods somente offline. |
| **Detalhamento do registro** | **0 Rastreamento**, **1 Depuração**, **2 Informações (recomendado)**, **3 Avisos**, **4 Erros**. Ao contrário do SM86, **0 não significa registro desativado**. O gerenciador ativa o registro em arquivo para diagnóstico. |
| **DLL de carregamento** | `dxgi.dll`, `winmm.dll`, `version.dll`, `d3d12.dll`, `dbghelp.dll`, `wininet.dll`, `winhttp.dll`. Comece com `dxgi.dll`; altere pelo gerenciador se necessário, com o jogo fechado. |

Pressione **Insert** dentro do jogo para abrir a interface sobreposta completa do projeto OptiScaler, as configurações avançadas e as informações do mecanismo ativo. Seus muitos controles avançados pertencem ao OptiScaler; o gerenciador expõe as opções comuns revisadas acima. Mantenha uma configuração de referência que funcione, altere uma coisa por vez e use o [manual oficial](https://github.com/optiscaler/OptiScaler/wiki) para os controles fora deste gerenciador. As instruções da versão estável 0.9.5 e da v10 nightly não são intercambiáveis.

Aplicar configurações pelo gerenciador grava novamente os valores selecionados. Se você alterou as mesmas opções na interface sobreposta, deixe as escolhas do gerenciador consistentes antes de aplicar; as configurações INI não relacionadas são preservadas onde houver suporte.

<a id="in-game"></a>
## 11. O que selecionar dentro do jogo

**SM86 ou ANR:** ative a opção **NVIDIA DLSS Frame Generation** do jogo depois de instalar. Comece em 2× onde o jogo oferecer essa opção. Reinicie o jogo se o menu exigir. A qualidade da ampliação, como Qualidade ou Equilibrado, é uma escolha separada de FG.

**Primeiro teste OptiScaler:** ative um ampliador temporal compatível no jogo, mantenha a entrada/saída FG do gerenciador desativadas e inspecione a saída ativa com Insert. Você não precisa ativar DLSS FG apenas para testar a ampliação. Se depois escolher uma entrada FG nativa, ative o recurso correspondente do jogo. Para a entrada Ampliador/OptiFG, use o caminho DX12 revisado e as orientações da interface sobreposta do projeto original.

Uma configuração **2×** cinza pode simplesmente significar que somente aquele multiplicador está disponível. Isso **não comprova** que FG esteja funcionando. O gerenciador não consegue desbloquear todos os multiplicadores no menu de todos os jogos. Da mesma forma, um rótulo do menu do jogo pode descrever a entrada enquanto o OptiScaler usa uma saída diferente.

Para comparar, use a mesma cena, resolução e configurações. Observe texto em movimento, cercas, bordas dos personagens e elementos do HUD, além dos FPS e da resposta aos comandos. FPS gerados/exibidos e FPS realmente renderizados são diferentes. Não se pode presumir que mais quadros exibidos dobrem a resposta aos comandos.

<a id="verify"></a>
## 12. Diagnóstico: como saber o que aconteceu

Em **Diagnóstico**, escolha **Ler registro do jogo** após um teste curto. Iniciar pelo gerenciador e ler o registro inicia a leitura periódica enquanto o gerenciador permanece aberto. Se o resultado estiver pendente, feche o jogo e leia o registro concluído novamente.

| Provedor | O que o gerenciador pode estabelecer |
|---|---|
| **SM86** | Examina a sessão registrada correspondente para verificar a criação bem-sucedida do recurso FG e sua primeira avaliação. Apenas carregar o mod ou ver um recurso SR é insuficiente. Leia os erros e as informações HAGS/GPU durante a execução. |
| **ANR** | Distingue o carregamento e a inicialização das avaliações que realmente geram quadros. É necessária uma sessão completa com contagens positivas de avaliações de geração e sem erros de geração para confirmar FG. As contagens e o multiplicador registrado ajudam no diagnóstico. |
| **OptiScaler** | Informa o carregamento e os erros do registro da sessão atual. Uma DLL carregada ou uma cadeia de troca criada **não comprova SR ou FG bem-sucedidas nem aumento de desempenho**. Confira o mecanismo ativo, as estatísticas de quadros e o resultado visual na interface sobreposta do jogo. |

Um registro parcial, inacabado ou grande demais não permite estabelecer uma sessão completa bem-sucedida. **Sem erros** significa que não foram encontrados erros na parte compreendida pelo leitor do registro, não que o desempenho ou a FG tenham sido certificados. Confira o horário de sessão exibido para não confundir um teste antigo com o atual. Em um PC com várias GPUs, confirme qual adaptador o jogo realmente usa.

<a id="updates"></a>
## 13. Três coisas diferentes podem ser atualizadas

| Atualização | Como funciona |
|---|---|
| **Aplicativo do gerenciador** | Verifica versões estáveis do GitHub ao iniciar e periodicamente enquanto está aberto (cerca de seis em seis horas). Um pequeno aviso aparece apenas para uma versão estável mais recente. Escolha **Atualizar e reiniciar**; ele baixa e verifica o pacote portátil/MSI correto e depois atualiza e reinicia com seu consentimento. Reabra o gerenciador para verificar imediatamente. Pré-lançamentos são ignorados pela verificação estável normal. |
| **Pacote baixado do provedor** | Verificado para o provedor selecionado ao iniciar ou após selecioná-lo; o ↻ do pacote verifica novamente. Um novo download verificado atualiza o cache. Ele **não se aplica sozinho** aos seus jogos. |
| **Mod em um jogo específico** | Selecione esse jogo, analise-o e escolha **Atualizar mod** após as verificações e a confirmação. Os outros jogos mantêm sua instalação existente até serem atualizados individualmente. |

O gerenciador compara o ID da versão, o ID do arquivo e SHA-256, portanto pode detectar arquivos substituídos mesmo com a mesma versão visível. **Baixado**, **atualizado**, **cache offline** e **ainda não suportado por este gerenciador** são resultados diferentes. Um pacote pode passar nas verificações de integridade do download e ainda ter um procedimento de instalação desconhecido. Aguarde uma versão do gerenciador que ofereça suporte explícito; não force copiando suas DLLs manualmente.

O gerenciador **0.4.3 e posteriores** usa o repositório renomeado. Versões muito antigas **0.4.2 ou anteriores** podem precisar de uma primeira atualização manual usando o pacote MSI/portátil completo mais recente. O problema da janela de ajuda/erro 1639 do atualizador MSI 0.2.0/0.2.1 também exige uma única instalação manual de um MSI mais recente corrigido. Use o mesmo tipo de instalação e feche o gerenciador primeiro. Mantenha intactas as pastas dos jogos e dos backups.

<a id="backup"></a>
## 14. Desativar, reativar, restaurar e trocar de provedor

**Desativar mod** impede temporariamente a ativação do mod gerenciado; não é uma desinstalação completa. Normalmente, o gerenciador desativa seu carregador e mantém os arquivos de execução e os backups. Para um Addon ANR externo, ele restaura a seleção externa do OptiScaler e mantém esse carregador externo.

Para **reativar**, mantenha o jogo fechado e use novamente a ação principal Instalar/Atualizar para o provedor gerenciado selecionado.

**Restaurar estado anterior** restaura o backup associado à instalação/atualização/troca de provedor gerenciada atual. Não é um comando geral de “desfazer o último botão” nem uma função específica para desfazer Desativar. Dependendo do histórico, pode retornar uma versão anterior do mod, outro provedor, uma instalação desativada ou o estado original sem modificações. Restaurar o backup original da primeira instalação pode remover o mod gerenciado ativo. Leia a confirmação.

Registros de instalações anteriores podem levar a backups anteriores; assim, restaurações adicionais podem voltar mais no histórico de instalação enquanto houver um registro. Não há um navegador geral do histórico para refazer. Os backups são mantidos; arquivos modificados ou desconhecidos podem bloquear a restauração em vez de serem sobrescritos.

**Nunca apague manualmente** `.dlssg-manager.json`, `.dlssg-manager.transaction.json` ou `.dlssg-manager-backups` para eliminar um erro. Eles preservam informações de propriedade e recuperação. Após uma operação interrompida, mantenha o jogo fechado, preserve esses arquivos e reabra o gerenciador. A recuperação usa o diário de transações durante as operações com arquivos. Se continuar bloqueada, exporte um relatório e peça ajuda em vez de reativar ou apagar carregadores manualmente.

<a id="troubleshooting"></a>
## 15. Problemas comuns e a próxima ação sensata

| O que você vê | O que fazer |
|---|---|
| **Instalar está desativado** | Aguarde a operação atual, selecione o EXE real e inspecione as verificações Bloqueadas em Diagnóstico. Uma versão somente leitura/prévia da interface não pode instalar. Resolva as causas em vez de copiar arquivos manualmente. |
| **GPU não suportada** | Confira primeiro o provedor selecionado. O gerenciador lembra/usa um provedor padrão; não escolhe automaticamente o melhor para sua GPU. ANR e OptiScaler têm requisitos diferentes do SM86. |
| **Jogo não compatível / Precisa de revisão** | Leia o motivo exato. A ausência de FG nativa importa para SM86/ANR; o OptiScaler pode precisar, em vez disso, de indícios de uma entrada SR temporal. Entradas FSR integradas podem ser inconclusivas. Confira a API e o EXE do jogo. |
| **Pacote mais recente, mas instalação bloqueada** | Integridade/estado de versão mais recente é separado de um procedimento revisado, do hardware ou da compatibilidade do jogo. Um procedimento mais recente desconhecido exige atualização/revisão do gerenciador. |
| **Lista Steam vazia ou jogo ausente** | Atualize as bibliotecas Steam; caso contrário, escolha manualmente a pasta do jogo ou seu EXE real de renderização. |
| **Vários EXEs** | Prefira o renderizador real. Para Unreal, inspecione `Binaries\Win64` e o candidato `Shipping.exe`. Não escolha um inicializador ou programa de relatórios de falha apenas para contornar a ambiguidade. |
| **Jogo ainda em execução** | Feche-o completamente e aguarde seu processo terminar antes de instalar, alterar configurações, trocar de tecnologia ou restaurar. |
| **HAGS configurado, mas execução ainda não confirmada** | Para SM86, reinicie o Windows após ativar HAGS, teste o jogo e leia o registro mais recente. Uma configuração do Registro sozinha não comprova que HAGS esteja ativo naquela sessão. |
| **Visual C++ / Vulkan ausente** | Use instaladores oficiais da Microsoft ou do fabricante da GPU, não sites de download de DLLs. ANR exige tanto os componentes Visual C++ x64 revisados quanto o carregador Vulkan. Verifique o hardware novamente depois. |
| **Carregador desconhecido, mod manual ou arquivo modificado** | Preserve-o. Identifique e restaure/remova o mod original pelo próprio procedimento suportado dele, ou peça orientação com um relatório. O gerenciador protege deliberadamente arquivos de terceiros; OptiScaler manual não é adotado silenciosamente. |
| **Registro ausente / apenas carregado / 2× cinza** | Siga as etapas corretas no jogo, teste brevemente e releia o registro concluído. Outro carregador pode ser necessário se nada carregar. Um 2× cinza e registros que mostram apenas o carregamento não comprovam FG. |
| **Rastros, cintilação, travamentos breves ou FPS piores** | Volte à configuração de referência recomendada. Compare uma alteração por vez, reduza a FG solicitada quando apropriado e inspecione os erros. Tente desativar/restaurar pelo gerenciador. Não presuma que o maior multiplicador, o fluxo mais fino ou o maior nível de aproximação seja o melhor. |
| **Configurações alteradas não têm efeito** | Feche o jogo, use Aplicar configurações e reinicie-o. Confira o motor selecionado e a API ativa: configurações de outro motor/API podem ser irrelevantes. |
| **Operação interrompida ou restauração bloqueada** | Mantenha o jogo fechado e preserve os registros de instalação/backups. Leia o erro, exporte um relatório e peça ajuda. Não apague o diário de recuperação nem force arquivos desconhecidos sobre o backup. |
| **Atualização do gerenciador não aparece** | Reabra o gerenciador, confirme o acesso à internet e confira sua versão instalada. As verificações normais ignoram pré-lançamentos. Instalações muito antigas podem precisar da atualização manual única descrita acima. |

<a id="files"></a>
## 16. Arquivos, privacidade e como pedir ajuda

As preferências normais do aplicativo, os caches e `application.log` ficam em `%LOCALAPPDATA%\DLSSGModManager`. Um diretório de dados personalizado ou um início dedicado à prévia da interface pode usar outro local. O registro de propriedade, o diário de transações e os backups do jogo selecionado ficam ao lado de seu **EXE de renderização**, não necessariamente ao lado do inicializador.

| Provedor | Local normal do registro no jogo, relativo à pasta de renderização |
|---|---|
| SM86 | `dlssg_sm86\logs\dlssg_sm86.log` |
| ANR | `OptiScaler\custom\Apex Neural Rendering FG\dlssg\nr_fg-*.log` |
| OptiScaler | `OptiScaler.log` |

Use **Exportar relatório de diagnóstico** em Diagnóstico e escolha onde salvar `DLSSG-diagnostic-report.json`. Ele inclui versões do gerenciador/provedor/pacote, hash do pacote/estado de atualização, detalhes de GPU/driver, verificações e indícios disponíveis nos registros. Substitui o caminho raiz do jogo selecionado e o prefixo do perfil de usuário por marcadores como `<GAME>` e `<USER>`; revise o arquivo antes de compartilhar, pois isso não garante a remoção de toda possível cadeia de texto que possa identificar você.

Os relatórios são salvos localmente, não enviados automaticamente. Não há conta nem telemetria. Ao pedir ajuda, inclua o provedor, o jogo, a versão do gerenciador, a mensagem exata de bloqueio e o que você alterou. Uma captura do resultado e o relatório exportado são mais úteis do que “não funciona”. Não compartilhe tokens, senhas nem arquivos pessoais sem relação com o problema.

<a id="glossary"></a>
## 17. Pequeno glossário

| Termo | Significado simples |
|---|---|
| **FPS renderizados/base** | Quadros reais calculados pelo jogo. |
| **Geração de quadros / FG** | Quadros extras estimados inseridos entre os reais. |
| **Ampliação / SR** | Reconstrução de uma imagem de maior resolução a partir de uma renderização de menor resolução. É separada de FG. |
| **Entrada / saída** | O recurso do jogo que o OptiScaler intercepta / o mecanismo que o OptiScaler executa no lugar dele. |
| **API: DX11, DX12, Vulkan** | A interface gráfica usada pelo jogo. APIs diferentes têm caminhos suportados diferentes. |
| **HUD / sem HUD** | A interface na tela / uma imagem do jogo sem essa interface. |
| **DLL / carregador / proxy** | Uma biblioteca de código e o arquivo com nome específico por meio do qual o jogo carrega um mod. O nome do arquivo importa. |
| **HAGS** | Agendamento de GPU acelerado por hardware do Windows. |
| **Runtime / ambiente de execução** | Um componente de software necessário para um recurso, como a DLL DLSS do jogo ou Visual C++. |
| **Cache / SHA-256** | Um pacote baixado e salvo / uma impressão digital usada para verificar a integridade dos arquivos. Uma impressão correspondente não certifica o funcionamento durante o jogo nem torna suportado um procedimento desconhecido. |
| **Backup / diário de transações** | Estado anterior salvo dos arquivos / informações para recuperar uma alteração interrompida. |

<a id="sources"></a>
## 18. Projetos originais e leitura adicional

- [Downloads e projeto do gerenciador](https://github.com/VenomDrake/DLSSG-Mod-Manager).
- [DLSSG for SM86](https://github.com/sdli1995/dlssg_for_sm86), de sdli1995 e colaboradores; [adaptação SM75](https://github.com/Coldwood1026), de Coldwood1026.
- [Apex Neural Rendering](https://github.com/apex-compute/ANR), de Apex Compute e colaboradores.
- [OptiScaler](https://github.com/optiscaler/OptiScaler) e sua [wiki oficial](https://github.com/optiscaler/OptiScaler/wiki), de seus colaboradores.
- [Visão geral do gerenciador](README.pt.md) e [avisos de terceiros](../../THIRD_PARTY_NOTICES.md).

Este guia descreve os **controles expostos pelo gerenciador 0.5.2** e seus procedimentos de provedor revisados. Não é uma garantia de compatibilidade de todos os jogos, um teste de desempenho nem um manual completo de todas as opções das interfaces sobrepostas dos projetos originais. Para recursos avançados exclusivos desses projetos, use a documentação original da versão correspondente.
