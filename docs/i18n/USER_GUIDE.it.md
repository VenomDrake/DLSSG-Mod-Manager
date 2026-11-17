# DLSSG Mod Manager — tutorial completo

**Lingua:** [English](../USER_GUIDE.md) · **Italiano** · [简体中文](USER_GUIDE.zh-Hans.md) · [हिन्दी](USER_GUIDE.hi.md) · [Español](USER_GUIDE.es.md) · [Français](USER_GUIDE.fr.md) · [العربية](USER_GUIDE.ar.md) · [বাংলা](USER_GUIDE.bn.md) · [Português](USER_GUIDE.pt.md) · [Русский](USER_GUIDE.ru.md) · [اردو](USER_GUIDE.ur.md)

Per il **gestore 0.5.1** su **Windows 10/11 x64**. I nomi riportati qui corrispondono all'interfaccia italiana: puoi scegliere la lingua in alto a destra. Questo gestore affianca i progetti originali ed è indipendente: non è un'applicazione ufficiale di NVIDIA, AMD, Intel, ANR o OptiScaler.

**Indice:** [Primo avvio](#start) · [Sicurezza](#safety) · [Download](#install) · [Scegliere la tecnologia](#choose-provider) · [Schermata e pulsanti](#screen) · [Prima installazione](#first-install) · [Controlli](#checks) · [Impostazioni SM86](#sm86) · [Impostazioni ANR](#anr) · [Impostazioni OptiScaler](#optiscaler) · [Impostazioni nel gioco](#in-game) · [Verifica](#verify) · [Aggiornamenti](#updates) · [Disattivazione e ripristino](#backup) · [Problemi comuni](#troubleshooting) · [File e assistenza](#files) · [Glossario](#glossary) · [Fonti](#sources)

<a id="start"></a>
## 1. Primo avvio: la tua prima prova

1. Chiudi il gioco. Scegli un **gioco per giocatore singolo offline**.
2. Apri il gestore e attendi il completamento del controllo del pacchetto della tecnologia selezionata.
3. Scegli la tecnologia, poi seleziona il gioco da Steam oppure indica la sua cartella o il suo EXE. Attendi l'analisi.
4. Leggi **2 · Esamina il risultato**. Apri **Diagnostica** per l'elenco completo. Risolvi tutti i controlli **Bloccati** e leggi gli avvisi prima di procedere.
5. In **Impostazioni della mod**, mantieni i valori consigliati per la prima prova: **SM86: DLSS, 2×**; **ANR: Auto, 2×**; **OptiScaler: upscaling automatico, ingresso e uscita della generazione dei fotogrammi entrambi Disattivati**.
6. Torna in **Gestione giochi**. Premi **Installa mod**, controlla destinazione e impostazioni nella conferma, poi conferma. Attendi la fine dell'operazione.
7. Avvia il gioco e segui le [istruzioni specifiche per la tecnologia](#in-game). Gioca brevemente, osservando movimento, interfaccia e risposta ai comandi.
8. Leggi il log in **Diagnostica**. Con OptiScaler, premi anche **Ins/Insert** nel gioco per vedere quale uscita è attiva. Il solo caricamento di una DLL non prova che la generazione dei fotogrammi funzioni.

Dopo una prima prova funzionante, cambia **una sola impostazione alla volta**. Chiudi il gioco, applica la modifica nel gestore e riavvia il gioco prima di confrontare.

<a id="safety"></a>
## 2. Da leggere prima di modificare un gioco

**Usa queste mod soltanto nei giochi per giocatore singolo offline. I file modificati possono attivare l'anti-cheat e comportare ban.** Il gestore non aggira gli anti-cheat e non protegge dai ban. La modalità offline non garantisce che l'anti-cheat sia assente: se viene rilevato, l'installazione rimane bloccata.

Il gestore controlla file e requisiti. **«Requisiti rilevati» non certifica compatibilità durante il gioco, stabilità o un aumento degli FPS.** Conserva normali copie di sicurezza dei salvataggi importanti: i backup del gestore riguardano le sue modifiche ai file del gioco, non tutti i salvataggi o l'intero gioco.

Non sovrapporre manualmente diversi loader, non copiare DLL casuali sopra i file del gioco e non eliminare un conflitto soltanto per superare un controllo. Prima identifica la mod esistente. I binari vengono scaricati come dati dai repository dei progetti originali: il gestore non esegue i loro script di installazione.

<a id="install"></a>
## 3. Scaricare e aprire il gestore

Usa le [release del progetto del gestore](https://github.com/VenomDrake/DLSSG-Mod-Manager/releases/latest).

| Download | Cosa fare |
|---|---|
| **ZIP portatile** | Estrai **tutto lo ZIP** in una cartella scrivibile, poi apri `DLSSGModManager.exe`. Non eseguirlo dall'interno dello ZIP. Mantieni insieme anche i file di licenza e documentazione. |
| **Installer Windows (.msi)** | Chiudi il gestore ed esegui il file MSI. L'installazione riguarda il tuo utente Windows e crea i collegamenti. |

I pacchetti pubblicati includono il runtime .NET del gestore. Non occorre installare Python, CUDA Toolkit o un programma separato per estrarre gli archivi. Una tecnologia può comunque richiedere driver grafici, Visual C++ o componenti Vulkan: vengono controllati separatamente.

Il gestore e l'MSI non sono firmati. Usa soltanto le release del progetto che intendevi scaricare; non disattivare globalmente le protezioni di Windows. Al primo avvio serve internet per ottenere il pacchetto della tecnologia. In seguito si può utilizzare una copia in cache già verificata, ma **la cache offline non conferma che sia l'ultima release**.

<a id="choose-provider"></a>
## 4. Quale tecnologia scegliere?

Scegli nel menu **Upscaling e generazione dei fotogrammi**. In questa guida la tecnologia può essere chiamata anche **provider**.

| Scelta | Scopo e requisiti per iniziare |
|---|---|
| **DLSSG for SM86 · NVIDIA / XeSS** | Percorso verificato dal gestore per NVIDIA **RTX 20/30**, driver **R580 o successivo**, pianificazione GPU con accelerazione hardware (HAGS) configurata e un runtime DLSS Frame Generation nativo del gioco supportato. Il gioco usa DirectX 12. Inizia con DLSS e 2×. |
| **Apex Neural Rendering · Beta** | Tecnologia FG alternativa per le famiglie dichiarate dal gestore: **GTX 16 / RTX 20–50** e **Radeon RX 5000/6000/7000/9000**. Richiede DLSS Frame Generation nativo x64 nel gioco tramite DX12/Vulkan, Visual C++ x64 e il loader Vulkan. GTX 10/Pascal e Intel Arc non rientrano nel supporto ANR di questo gestore. Inizia con Auto e 2×. ANR rimane una beta. |
| **OptiScaler · Upscaling / FG** | Sostituisce un upscaler temporale DLSS/FSR/XeSS compatibile del gioco e può gestire percorsi FG supportati. DLSS Frame Generation nativo **non è necessario per il solo upscaling**. NVIDIA, AMD e Intel sono possibili candidati, ma ogni uscita ha ulteriori limiti. Inizia con upscaling Auto e FG **Disattivato/Disattivato**. |

HAGS e il driver NVIDIA R580 **non sono requisiti universali** ereditati da ANR o OptiScaler. L'**uscita DLSS** nativa di OptiScaler richiede comunque NVIDIA RTX e il runtime DLSS del gioco. OptiScaler non rende ogni motore utilizzabile con qualsiasi GPU.

Le procedure verificate dal gestore 0.5.1 sono **SM86 0.4.1/0.4.2**, **ANR FG-V.B15/B17** e **OptiScaler 0.9.5**. Un nuovo pacchetto scaricato dal progetto originale può restare bloccato finché i suoi requisiti modificati non vengono verificati.

Il gestore ricorda la tecnologia selezionata: **non sceglie automaticamente quella migliore per la tua GPU**. Seleziona una tecnologia adatta prima di interpretare un messaggio di GPU non supportata.

<a id="screen"></a>
## 5. Capire la schermata e tutti i pulsanti principali

![Schermata Gestione giochi](../screenshot.png)

| Area o controllo | A cosa serve |
|---|---|
| **Versione del gestore** | Versione dell'applicazione. È diversa dalla versione del pacchetto della mod selezionata. |
| **Lingua** | Cambia la lingua dell'interfaccia. Arabo e urdu usano la disposizione da destra a sinistra. I valori di configurazione, come `dxgi.dll`, conservano la loro grafia originale. |
| **Selettore della tecnologia** | Sceglie SM86, ANR oppure OptiScaler. Il cambio ricontrolla il pacchetto e i requisiti di quella tecnologia; non modifica immediatamente il gioco. |
| **Gestione giochi** | Scegli il gioco, esamina il risultato e applica la mod. |
| **Impostazioni della mod** | Modifica le opzioni del provider selezionato. Cambiare un campo non scrive da solo nei file del gioco. |
| **Diagnostica** | Tutti i controlli, dettagli del pacchetto e del PC, log, collegamenti ai driver, esportazione del report, disattivazione e ripristino. |
| **Giochi Steam installati / ↻** | Seleziona un gioco Steam rilevato; ↻ aggiorna le librerie. La presenza nell'elenco non implica che la mod sia supportata. |
| **Scegli cartella / Scegli EXE** | Seleziona manualmente un gioco, anche fuori da Steam. Scegli l'EXE che esegue il rendering, non un launcher, un programma per segnalare crash o un installer. |
| **Casella del percorso / Analizza gioco** | Puoi incollare il percorso di una cartella o di un EXE e avviare l'analisi. Cambiare percorso cancella il risultato precedente per evitare di riutilizzarlo per errore. |
| **Eseguibile di rendering** | Risolve la presenza di più candidati. Per molti giochi Unreal, usa `NomeGioco\Binaries\Win64\...-Win64-Shipping.exe`, non il piccolo launcher nella cartella principale. |
| **Visualizza tutti i controlli / Modifica le impostazioni della mod** | Apre la relativa scheda. |
| **Installa / Aggiorna / Applica impostazioni / Cambia tecnologia** | È la stessa area d'azione, con un'etichetta che dipende dalla situazione. Applica modifiche soltanto dopo i controlli e la conferma. |
| **Avvia gioco** | Avvia il gioco selezionato, tramite Steam quando è stato associato. Non installa la mod. |
| **Annulla operazione** | Richiede un arresto sicuro. Attendi che il gestore completi un'applicazione o un recupero critico. |

In **Diagnostica**, il simbolo **↻** del pacchetto controlla il progetto originale, mentre quello del PC rilegge l'hardware. **Apri impostazioni grafiche di Windows** apre le impostazioni: non attiva HAGS al tuo posto. I pulsanti dei driver aprono le pagine NVIDIA, AMD o Intel in base a tecnologia e schede rilevate. **Progetto originale** apre il repository del provider.

Il piccolo avviso **Aggiorna e riavvia** riguarda **il gestore stesso**. La sua **×** nasconde l'avviso di quella versione nella finestra corrente; non disinstalla nulla.

<a id="first-install"></a>
## 6. Installare, cambiare impostazioni o cambiare tecnologia

1. Attendi download e verifica del pacchetto, poi seleziona l'EXE di rendering corretto.
2. Leggi il riepilogo e tutti i controlli. Chiudi il gioco e gli eventuali processi rimasti attivi.
3. Imposta i valori consigliati per la prima prova o le opzioni desiderate. Il risultato può cambiare se un'opzione introduce un nuovo requisito.
4. Premi il pulsante d'azione in basso. Controlla tecnologia, cartella di rendering, versione e impostazioni nella conferma.
5. Conferma. Il gestore prepara i file verificati, crea un backup, disattiva i vecchi loader, modifica i dati e attiva per ultimo il loader selezionato. Attendi la fine.
6. Riavvia il gioco. Un gioco già aperto non può utilizzare in modo affidabile le impostazioni appena scritte.

**«Hai già l'ultima versione» si riferisce all'identità del pacchetto installato.** Non significa che un'impostazione appena cambiata sia già stata applicata. Per salvarla usa **Applica impostazioni**, con il gioco chiuso.

Per un'installazione gestita, il cambio di provider usa lo stesso sistema di backup e cronologia. OptiScaler autonomo 0.9.5 e il loader OptiScaler personalizzato di ANR sono diversi: il gestore non li mescola. Un'installazione ANR Addon esterna può richiedere prima il ripristino dell'installazione esterna. Le installazioni manuali sconosciute vengono protette, non sovrascritte automaticamente.

<a id="checks"></a>
## 7. Leggere i controlli senza andare a tentativi

| Stato | Significato e azione |
|---|---|
| **Superato** | Quel particolare controllo è riuscito. Non prova che siano soddisfatti tutti gli altri requisiti o che il gioco funzioni correttamente. |
| **Informazione** | Un dettaglio di contesto, per esempio un'impostazione o un componente facoltativo. Leggilo quando è rilevante. |
| **Avviso / Da verificare** | Qualcosa richiede attenzione o una conferma nel gioco. Non blocca automaticamente, ma non garantisce neppure che procedere sia sicuro o compatibile. |
| **Sconosciuto** | Il gestore non ha potuto stabilire un fatto. Leggi il dettaglio invece di considerarlo superato. |
| **Bloccato** | L'installazione non può continuare finché non risolvi la causa. |

I controlli normalmente riguardano: Windows e x64, GPU e driver, HAGS quando richiesto, componenti Visual C++/Vulkan, identità e struttura supportata del pacchetto, completezza e permessi della scansione, EXE corretto, ingresso o runtime nativo, indizi sull'API grafica, indicatori anti-cheat, loader esistenti o modificati, gioco aperto, destinazioni di sola lettura e spazio libero. Backup e preparazione richiedono più spazio della dimensione finale della mod.

**Non supportato** indica un'incompatibilità tecnica rilevata. **Da verificare** significa che le informazioni sono incomplete, l'EXE è ambiguo o un ingresso potrebbe essere incorporato nel gioco. **Installazione bloccata** può indicare un conflitto o un problema del pacchetto, non necessariamente un gioco non supportato. Una scansione statica dei file non può dimostrare l'assenza di anti-cheat o garantire che il gioco funzioni.

<a id="sm86"></a>
## 8. SM86: tutte le impostazioni del gestore

![Impostazioni SM86](../settings-sm86.png)

**Prima prova consigliata:** DLSS, 2×, ottimizzazione 1, preset A, sostituzione SR disattivata, log 2, registrazione tempi disattivata, `version.dll`.

| Impostazione | Spiegazione semplice e valori disponibili |
|---|---|
| **Motore di generazione** | **NVIDIA DLSS-G** oppure **Intel XeSS-FG** sceglie l'implementazione FG della mod. Qui XeSS è l'implementazione della mod SM86: non promette il supporto generale delle GPU Intel. Inizia con DLSS. |
| **Moltiplicatore dei fotogrammi** | DLSS offre **2×–6×**, XeSS **2×–4×**. Sono richieste di un moltiplicatore: 2× significa un fotogramma generato per ogni fotogramma reale, 3× ne significa due e così via. Il gioco può imporre un limite inferiore e una richiesta non supportata può impedire la FG. Inizia a 2×. Più fotogrammi generati non significano una riduzione proporzionale della latenza o una fluidità garantita. |
| **Ottimizzazione dei kernel** | **0:** kernel originali, utili per un confronto. **1:** accelerazioni con risultato esatto, consigliate. **2:** elaborazione approssimata più veloce. **3:** approssimazione massima. I livelli superiori possono modificare il risultato: confronta bordi in movimento e interfaccia invece di presumere che il numero maggiore sia migliore. Vale per DLSS-G. |
| **Preimpostazione DLSS-G** | **A:** scelta normale per iniziare. **B:** ricomposizione dell'interfaccia quando il gioco fornisce immagini senza HUD e buffer UI adatti; non crea questi buffer. **C:** FG più leggera, con un possibile compromesso sulla qualità, soprattutto vicino a HUD e bordi delle occlusioni. Vale per DLSS-G. |
| **Sostituzione Super Resolution** | **Disattivato** mantiene l'upscaler del gioco. **M-INT8 / L-INT8** selezionano i kernel INT8 M/L del progetto. **M-FP16 / L-FP16** selezionano l'elaborazione FP16 corrispondente. M/L sono scelte del modello, non una classifica universale delle prestazioni. Le sostituzioni possono peggiorare le prestazioni in Qualità, Bilanciata o DLAA: lascia Disattivato per la prima prova. |
| **Flusso ottico XeSS** | **Coarse**, **Middle**, **Fine** cambiano la risoluzione dell'elaborazione del movimento. Una risoluzione più fine può richiedere più tempo GPU. **CoarseAt4K** usa Coarse con un numero di pixel in uscita almeno pari a 3840×2160, altrimenti Middle. Inizia con Middle. Vale per XeSS-FG. |
| **Gestione HUD XeSS** | **Automatico**, **Richiedi un'immagine senza HUD (1)** oppure **Non usare un'immagine senza HUD (0)**. In SM86 **0.4.2 verificato, Auto si comporta come 0**: viene interpolata l'intera immagine, compresa l'interfaccia. La modalità 1 richiede che il gioco fornisca un'immagine senza HUD corrispondente e può fallire se manca. |
| **Dettaglio del log** | **0 Disattivato**, **1 Errori**, **2 Diagnostica (consigliato)**, **3 Tracce dettagliate**. Più dettagli aiutano a capire i problemi ma aumentano la registrazione. |
| **Registra i tempi dei fotogrammi** | Scrive ulteriori dati sui tempi per le indagini. Non imposta un limite FPS e non attiva la FG. Lascialo disattivato per la prima prova. |
| **DLL di caricamento** | `version.dll` (consigliata), `winmm.dll`, `dbghelp.dll`, `dinput8.dll`, `dxgi.dll`, `d3d12.dll`. Il nome controlla come il gioco carica la mod. Mantieni un solo loader attivo; prova le alternative tramite il gestore se la mod non si carica. Non sovrapporre manualmente più copie. |

DLSS-G e XeSS-FG hanno impostazioni differenti. Un'opzione specifica di un motore non diventa attiva soltanto perché è visibile quando è selezionato l'altro.

**HAGS per SM86:** da Diagnostica apri le impostazioni grafiche di Windows, abilita la pianificazione GPU con accelerazione hardware se Windows la offre e poi riavvia Windows. Lo stato configurato nel registro e lo stato attivo durante l'esecuzione sono controlli diversi. La conferma durante l'esecuzione viene dal log del gioco. Il gestore non modifica automaticamente driver o HAGS.

<a id="anr"></a>
## 9. ANR: tutte le impostazioni del gestore

![Impostazioni ANR](../settings-anr.png)

**Prima prova consigliata:** percorso di calcolo Auto, 2×, 80 nit per unità, gestione dei cambi di scena attiva, `dxgi.dll`. Il motore è ANR: ottimizzazione, preset e sostituzione SR di SM86 non si applicano.

| Impostazione o modalità | Significato |
|---|---|
| **Full / Addon** | La scelta è automatica. **Full** comprende la configurazione verificata di loader e provider. **Addon** aggiunge ANR a una configurazione OptiScaler esterna compatibile, preservandone il loader. Il cambio da OptiScaler autonomo gestito ad ANR usa Full. Non scegliere o copiare manualmente un addon qualsiasi. |
| **Moltiplicatore dei fotogrammi** | Richiede **2×–6×**, nei limiti dell'integrazione del gioco. Inizia a 2×. Il gestore scrive il numero corrispondente di fotogrammi aggiuntivi: la scelta 6× non significa sei fotogrammi aggiuntivi. |
| **Percorso di calcolo** | **auto**, **coopmat**, **packed**, **dp4a**, **portable**, **portable-linalg**, **portable-xmx** sono modi alternativi di eseguire i calcoli di ANR. Non sono livelli di qualità dell'immagine. Auto usa la selezione del provider; i percorsi espliciti richiedono capacità GPU adatte e non è garantito che funzionino tutti o siano più veloci. Lascia Auto, salvo un'indagine specifica. |
| **Luminanza HDR** | Intervallo **40–1000**, valore iniziale **80**: descrive la luminanza associata al valore lineare 1,0 nell'elaborazione HDR/PQ del provider. Non è il cursore della luminosità del monitor. Mantieni 80 salvo indagini specifiche sull'HDR. |
| **Gestione dei cambi di scena** | Attiva inizialmente: evita di interpolare tra tagli bruschi. Mantienila attiva nelle prove normali. |
| **DLL di caricamento** | `dxgi.dll`, `winmm.dll`, `version.dll`: inizia con `dxgi.dll`. In modalità Addon il loader esterno viene mantenuto e il selettore è bloccato. |

L'etichetta **Beta** di ANR riguarda il provider originale, anche quando il gestore è una release stabile. ANR richiede i componenti Visual C++ x64 verificati e il loader Vulkan anche se l'API grafica del gioco è DX12. Non impone il requisito HAGS di SM86.

<a id="optiscaler"></a>
## 10. OptiScaler: tutte le impostazioni del gestore

![Impostazioni OptiScaler](../settings-optiscaler.png)

**Prima prova consigliata:** tutti gli upscaler **Automatici**, ingresso FG **Disattivato**, uscita FG **Disattivata**, identità GPU simulata in DXGI **Automatica**, log **2 · Informazioni**, loader `dxgi.dll`.

### Scelte dell'upscaler

Un **ingresso** è la tecnologia selezionata nel menu del gioco. Un'**uscita** è ciò che OptiScaler esegue realmente. Per esempio, un gioco può fornire un ingresso DLSS mentre OptiScaler produce un'uscita XeSS. Scegliere DLSS come ingresso nel gioco non prova da solo che DLSS sia anche l'uscita attiva.

| Campo del gestore | Valori disponibili |
|---|---|
| **Upscaler DirectX 11** | Auto; FSR 2.2 (`fsr22`); FSR 3.1 (`fsr31`); XeSS nativo (`xess`); XeSS tramite D3D11on12 (`xess_12`); FSR 2.1/2.2/3.1 tramite interoperabilità DX12 (`fsr21_12`, `fsr22_12`, `fsr31_12`); DLSS. |
| **Upscaler DirectX 12** | Auto; XeSS; FSR 2.1; FSR 2.2; FSR 3.1/4; DLSS. |
| **Upscaler Vulkan** | Auto; FSR 2.1/2.2/3.1/4 nativo nei limiti del motore; XeSS nativo; FSR 2.1 oppure FSR 3.1/4 tramite interoperabilità DX12 (`fsr21_12`, `fsr31_12`); DLSS. |

Soltanto l'API attiva del gioco usa la relativa impostazione. **Auto** segue i valori predefiniti di OptiScaler: è il punto di partenza più prudente, non la promessa dell'uscita più veloce. `_12` indica un percorso di interoperabilità con DX12, che può aggiungere un costo di elaborazione.

Lo **XeSS DX11 nativo** richiede Intel Arc; `xess_12` è una scelta di interoperabilità diversa. L'**uscita DLSS** richiede NVIDIA RTX e il runtime DLSS x64 del gioco. **FSR 3.1/4** non promette FSR 4 su qualsiasi GPU: motore e hardware determinano il modello attivo o quello alternativo usato se necessario. Controlla l'uscita effettiva nell'overlay: il gestore non forza FSR 4 su hardware non supportato.

Su DX11 il percorso nativo `fsr31` usa FSR 3.1; il percorso FSR 4 passa dall'interoperabilità DX12. Un'etichetta generale «3.1 / 4» non garantisce FSR 4 nativo su DX11.

### Ingresso e uscita della generazione dei fotogrammi

Per la prima prova con il solo upscaling, lascia **entrambi i campi Disattivati**. Per attivare la FG in seguito, scegli un ingresso **e** un'uscita compatibili; l'attivazione di uno solo dei due viene bloccata.

| Ingresso FG | Cosa deve fornire il gioco |
|---|---|
| **Disattivato** (`nofg`) | Non viene richiesta FG da OptiScaler. Disattiva anche l'uscita. |
| **DLSS Frame Generation** (`dlssg`) | DLSS FG nativo con un'integrazione Streamline 2 adatta; il gestore controlla gli indizi disponibili e avvisa se non può confermarla. |
| **DLSS-G · Nukem** (`nukems`) | Un runtime DLSS-G nativo. Associa questo ingresso all'uscita **Nukem · FSR 3**. |
| **FSR Frame Generation** (`fsrfg`) | Una sorgente FSR FG nativa compatibile nel gioco. Il solo nome di una DLL non dimostra che l'integrazione sia presente. |
| **Ingresso upscaler / OptiFG** (`upscaler`) | Un ingresso upscaler temporale funzionante, attraverso il percorso OptiFG DX12 verificato. Può servire la correzione HUD nell'overlay originale. |
| **FSR 3.0 Frame Generation** (`fsrfg30`) | Una sorgente FSR 3.0 FG nativa compatibile. |

| Uscita FG | Significato |
|---|---|
| **Disattivata** | Nessuna uscita FG. Disattiva anche l'ingresso. |
| **AMD FSR Frame Generation** (`fsrfg`) | Usa il motore FidelityFX FG compatibile. Non garantisce la stessa versione o le stesse funzioni FSR su tutte le GPU. |
| **Intel XeSS Frame Generation** (`xefg`) | Richiede le capacità GPU previste dal motore, compreso un supporto DP4a adatto, e condizioni compatibili del gioco e della visualizzazione. Non è abilitata su ogni dispositivo Intel, AMD o NVIDIA soltanto perché ne è stata riconosciuta la marca. Il supporto multi-frame ha ulteriori limiti nel progetto originale. |
| **Nukem · FSR 3** (`nukems`) | Usa il percorso DLSSG-to-FSR3 incluso e verificato. È obbligatoria quando l'ingresso selezionato è Nukem. |

Il gestore 0.5.1 blocca la FG selezionata quando rileva un percorso **DX11**. Per un percorso **Vulkan** rilevato, la coppia FG ammessa dalla procedura verificata è **Nukem → Nukem**. Gli altri percorsi FG proposti richiedono DX12; se l'API è sconosciuta serve comunque una conferma nel gioco. Questi controlli non rendono compatibile un'integrazione del gioco non supportata.

### Altri controlli e impostazioni avanzate

| Impostazione | Significato |
|---|---|
| **Identità GPU simulata in DXGI: Automatico / Attivato / Disattivato** | Cambia l'identità GPU comunicata tramite DXGI per compatibilità. Non migliora l'hardware reale e non garantisce prestazioni. Inizia con Automatico. Non protegge dagli anti-cheat: usa le mod soltanto offline. |
| **Dettaglio del log** | **0 Tracce**, **1 Debug**, **2 Informazioni (consigliato)**, **3 Avvisi**, **4 Errori**. Diversamente da SM86, **0 non disattiva il log**. Il gestore abilita la registrazione su file per la diagnostica. |
| **DLL di caricamento** | `dxgi.dll`, `winmm.dll`, `version.dll`, `d3d12.dll`, `dbghelp.dll`, `wininet.dll`, `winhttp.dll`. Inizia con `dxgi.dll`; se serve cambiala dal gestore, con il gioco chiuso. |

Premi **Ins/Insert** nel gioco per aprire l'overlay completo di OptiScaler, le impostazioni avanzate e le informazioni sul motore attivo. I numerosi controlli avanzati appartengono a OptiScaler; il gestore espone le scelte comuni verificate descritte qui sopra. Conserva una configurazione funzionante, modifica una cosa alla volta e usa il [manuale ufficiale](https://github.com/optiscaler/OptiScaler/wiki) per i controlli esterni a questo gestore. Le istruzioni per la stabile 0.9.5 e per la nightly v10 non sono intercambiabili.

Quando applichi le impostazioni dal gestore, i valori selezionati vengono scritti di nuovo. Se hai modificato le stesse opzioni nell'overlay, rendi coerenti anche quelle del gestore prima di applicarle; le altre impostazioni INI vengono preservate dove supportato.

<a id="in-game"></a>
## 11. Cosa selezionare dentro il gioco

**SM86 oppure ANR:** dopo l'installazione abilita **NVIDIA DLSS Frame Generation** nel gioco. Inizia a 2× dove viene offerto. Riavvia il gioco se il suo menu lo richiede. La qualità dell'upscaling, per esempio Qualità o Bilanciata, è una scelta separata dalla FG.

**Prima prova OptiScaler:** abilita nel gioco un upscaler temporale compatibile, lascia ingresso e uscita FG del gestore disattivati e controlla con Ins/Insert l'uscita attiva. Non devi attivare DLSS FG soltanto per provare l'upscaling. Se in seguito scegli un ingresso FG nativo, abilita la funzione corrispondente del gioco. Per l'ingresso Upscaler/OptiFG usa il percorso DX12 verificato e le indicazioni dell'overlay originale.

Un selettore **2×** grigio può semplicemente indicare che quello è l'unico moltiplicatore disponibile. **Non** dimostra che la FG sia attiva. Il gestore non può sbloccare qualsiasi moltiplicatore nel menu di ogni gioco. Allo stesso modo, un'etichetta del menu può descrivere l'ingresso mentre OptiScaler usa un'uscita diversa.

Per confrontare, usa la stessa scena, risoluzione e impostazioni. Osserva testi in movimento, recinzioni, bordi dei personaggi e HUD, oltre agli FPS e alla risposta ai comandi. Gli FPS generati o visualizzati sono diversi da quelli realmente renderizzati. Non presumere che più fotogrammi visualizzati raddoppino la reattività.

<a id="verify"></a>
## 12. Diagnostica: capire cosa è successo

In **Diagnostica**, premi **Leggi log del gioco** dopo una breve prova. L'avvio dal gestore e la lettura del log attivano letture periodiche mentre il gestore resta aperto. Se il risultato rimane in attesa, chiudi il gioco e rileggi il log completato.

| Provider | Cosa può stabilire il gestore |
|---|---|
| **SM86** | Cerca nella sessione registrata corrispondente la creazione riuscita della funzione FG e la sua prima elaborazione. Non bastano il caricamento della mod o una funzione SR. Leggi gli errori e le informazioni su HAGS/GPU durante l'esecuzione. |
| **ANR** | Distingue caricamento e inizializzazione dalle elaborazioni con fotogrammi realmente generati. La conferma FG richiede una sessione completa con conteggi positivi di elaborazioni generate e nessun errore di generazione. Conteggi e moltiplicatore registrato aiutano la diagnosi. |
| **OptiScaler** | Segnala il caricamento e gli errori del log della sessione corrente. Una DLL caricata o una swapchain creata **non provano il successo di SR, FG o un aumento delle prestazioni**. Verifica motore attivo, statistiche dei fotogrammi e risultato visivo nell'overlay del gioco. |

Un log parziale, incompleto o troppo grande non permette di stabilire che l'intera sessione sia riuscita. **Nessun errore** significa che non sono stati trovati errori nella parte compresa dal lettore, non che prestazioni o FG siano certificate. Controlla l'ora della sessione per non confondere una prova precedente con quella attuale. Se il PC ha più GPU, verifica quale utilizza realmente il gioco.

<a id="updates"></a>
## 13. Ci sono tre aggiornamenti diversi

| Aggiornamento | Come funziona |
|---|---|
| **Applicazione del gestore** | Controlla le release stabili di GitHub all'avvio e periodicamente mentre resta aperta, circa ogni sei ore. Un piccolo avviso appare soltanto per una versione stabile più recente. Premi **Aggiorna e riavvia**: scarica e verifica il pacchetto portatile o MSI corretto, poi aggiorna e riavvia con il tuo consenso. Riapri il gestore per controllare subito. Il controllo stabile normale ignora le prerelease. |
| **Pacchetto del provider scaricato** | Viene controllato per la tecnologia selezionata all'avvio o quando la selezioni; il ↻ del pacchetto ripete il controllo. Un nuovo download verificato aggiorna la cache. **Non** si applica da solo ai giochi. |
| **Mod in un singolo gioco** | Seleziona quel gioco, analizzalo e premi **Aggiorna mod** dopo i controlli e la conferma. Gli altri giochi mantengono la propria installazione finché non li aggiorni singolarmente. |

Il gestore confronta ID della release, ID del file e SHA-256: può quindi rilevare anche un pacchetto sostituito con lo stesso numero di versione visibile. **Scaricato**, **ultima versione**, **cache offline** e **non ancora supportato da questo gestore** sono risultati diversi. Un pacchetto può superare la verifica di integrità del download ma avere ancora una procedura d'installazione sconosciuta. Attendi una versione del gestore che la supporti esplicitamente: non forzarla copiando manualmente le DLL.

Il gestore **0.4.3 e successivi** usa il repository rinominato. Le versioni molto vecchie, **0.4.2 o precedenti**, possono richiedere un aggiornamento manuale una tantum con l'ultimo pacchetto MSI o portatile completo. Anche il problema della finestra di aiuto/errore 1639 nell'aggiornamento MSI di 0.2.0/0.2.1 richiede l'installazione manuale, una volta, di un MSI successivo corretto. Usa lo stesso tipo di installazione e prima chiudi il gestore. Conserva intatti giochi e cartelle di backup.

<a id="backup"></a>
## 14. Disattivare, riattivare, ripristinare e cambiare provider

**Disattiva mod** impedisce temporaneamente l'attivazione della mod gestita: non è una disinstallazione completa. Normalmente il gestore disabilita il suo loader e conserva file del runtime e backup. Per un ANR Addon esterno ripristina la selezione dell'OptiScaler esterno, mantenendo quel loader.

Per **riattivare**, tieni il gioco chiuso e usa di nuovo l'azione principale Installa/Aggiorna per il provider gestito selezionato.

**Ripristina stato precedente** usa il backup associato all'attuale installazione, aggiornamento o cambio di provider gestito. Non è un comando generale «annulla l'ultimo pulsante» e non è un annullamento dedicato di Disattiva. In base alla cronologia può riportare una versione precedente della mod, un altro provider, un'installazione disattivata oppure lo stato originale senza modifiche. Ripristinare il backup originale della prima installazione può rimuovere la mod gestita attiva. Leggi la conferma.

I record di installazione precedenti possono rimandare a backup precedenti: ulteriori ripristini possono quindi tornare indietro nella cronologia finché rimane un record. Non c'è un sistema generale per sfogliare e rifare tutte le operazioni. I backup vengono conservati; file modificati o sconosciuti possono bloccare il ripristino invece di essere sovrascritti.

**Non eliminare mai manualmente** `.dlssg-manager.json`, `.dlssg-manager.transaction.json` o `.dlssg-manager-backups` per togliere un errore. Contengono informazioni sulla proprietà dei file e sul recupero. Dopo un'operazione interrotta tieni il gioco chiuso, conserva questi file e riapri il gestore. Il recupero usa il registro della transazione durante le operazioni sui file. Se resta bloccato, esporta un report e chiedi aiuto invece di riattivare o cancellare manualmente i loader.

<a id="troubleshooting"></a>
## 15. Problemi comuni e prossima azione sensata

| Cosa vedi | Cosa fare |
|---|---|
| **Installa è disabilitato** | Attendi la fine dell'operazione corrente, scegli l'EXE reale e leggi i controlli Bloccati in Diagnostica. Una versione di sola lettura/anteprima UI non può installare. Risolvi le cause invece di copiare file manualmente. |
| **GPU non supportata** | Controlla prima la tecnologia selezionata. Il gestore ne ricorda o usa una iniziale: non sceglie automaticamente quella migliore per la GPU. ANR e OptiScaler hanno requisiti diversi da SM86. |
| **Gioco non supportato / Da verificare** | Leggi il motivo preciso. L'assenza di FG nativa conta per SM86/ANR; OptiScaler può invece richiedere indizi di un ingresso SR temporale. Gli ingressi FSR incorporati possono rendere il controllo inconcludente. Verifica API ed EXE. |
| **Ultimo pacchetto, ma installazione bloccata** | Integrità e ultima versione sono separate da procedura verificata, hardware e compatibilità del gioco. Una procedura nuova sconosciuta richiede un aggiornamento o una verifica del gestore. |
| **Elenco Steam vuoto o gioco assente** | Aggiorna le librerie Steam; altrimenti scegli manualmente cartella del gioco o EXE di rendering reale. |
| **Più EXE** | Preferisci il renderer effettivo. Per Unreal controlla `Binaries\Win64` e il candidato `Shipping.exe`. Non scegliere un launcher o un programma per i crash soltanto per aggirare l'ambiguità. |
| **Gioco ancora aperto** | Chiudilo completamente e attendi la fine del suo processo prima di installare, cambiare impostazioni, cambiare provider o ripristinare. |
| **HAGS configurato, ma non confermato durante il gioco** | Per SM86 riavvia Windows dopo avere abilitato HAGS, prova il gioco e leggi l'ultimo log. La sola impostazione nel registro non dimostra che HAGS sia attivo in quella sessione. |
| **Visual C++ / Vulkan mancanti** | Usa gli installer ufficiali Microsoft o del produttore GPU, non siti che distribuiscono singole DLL. ANR richiede sia i componenti Visual C++ x64 verificati sia il loader Vulkan. Poi ricontrolla l'hardware. |
| **Loader sconosciuto, mod manuale o file modificato** | Conservalo. Identifica e ripristina/rimuovi la mod originale con la sua procedura supportata, oppure chiedi consiglio con un report. Il gestore protegge intenzionalmente i file estranei: non adotta automaticamente un OptiScaler manuale. |
| **Log assente / solo caricamento / 2× grigio** | Segui le istruzioni corrette nel gioco, prova brevemente e rileggi il log completo. Se non si carica nulla può servire un altro loader. Il 2× grigio e il solo caricamento nel log non provano la FG. |
| **Scie, sfarfallii, scatti o FPS peggiori** | Torna ai valori iniziali consigliati. Confronta una modifica alla volta, riduci la richiesta FG dove opportuno e leggi gli errori. Prova disattivazione o ripristino tramite il gestore. Non presumere che il massimo moltiplicatore, il flusso più fine o il maggiore livello di approssimazione siano migliori. |
| **Le impostazioni cambiate non hanno effetto** | Chiudi il gioco, premi Applica impostazioni e poi riavvialo. Controlla motore selezionato e API attiva: le opzioni di un altro motore o API possono non essere pertinenti. |
| **Operazione interrotta o ripristino bloccato** | Tieni il gioco chiuso e conserva record e backup. Leggi l'errore, esporta un report e chiedi aiuto. Non cancellare il registro di recupero e non forzare file sconosciuti sopra il backup. |
| **Aggiornamento del gestore non mostrato** | Riapri il gestore, controlla internet e la versione installata. Il controllo normale ignora le prerelease. Le installazioni molto vecchie possono richiedere l'aggiornamento manuale una tantum descritto sopra. |

<a id="files"></a>
## 16. File, privacy e richiesta di assistenza

Preferenze, cache e `application.log` dell'applicazione sono normalmente in `%LOCALAPPDATA%\DLSSGModManager`. Una directory dati personalizzata o un avvio dedicato all'anteprima UI possono usare un altro percorso. Il record dei file gestiti, il registro della transazione e i backup del gioco selezionato si trovano accanto al suo **EXE di rendering**, non necessariamente accanto al launcher.

| Provider | Posizione normale del log nel gioco, relativa alla cartella di rendering |
|---|---|
| SM86 | `dlssg_sm86\logs\dlssg_sm86.log` |
| ANR | `OptiScaler\custom\Apex Neural Rendering FG\dlssg\nr_fg-*.log` |
| OptiScaler | `OptiScaler.log` |

In Diagnostica usa **Esporta report diagnostico** e scegli dove salvare `DLSSG-diagnostic-report.json`. Comprende versioni del gestore, provider e pacchetto, hash e stato dell'aggiornamento, dati GPU/driver, controlli e informazioni disponibili dai log. Sostituisce il percorso della cartella principale del gioco e il prefisso del profilo utente con segnaposto come `<GAME>` e `<USER>`: rileggi il file prima di condividerlo, perché questo non garantisce la rimozione di qualsiasi possibile stringa identificativa.

I report vengono salvati localmente e non inviati automaticamente. Non ci sono account o telemetria. Quando chiedi aiuto indica provider, gioco, versione del gestore, messaggio di blocco esatto e cosa hai modificato. Uno screenshot del risultato e il report esportato aiutano più di «non funziona». Non condividere token, password o file personali non pertinenti.

<a id="glossary"></a>
## 17. Piccolo glossario

| Termine | Significato semplice |
|---|---|
| **FPS renderizzati / di base** | Fotogrammi reali calcolati dal gioco. |
| **Generazione dei fotogrammi / FG** | Fotogrammi aggiuntivi stimati, inseriti tra quelli reali. |
| **Upscaling / SR** | Ricostruzione di un'immagine a risoluzione maggiore partendo da un rendering a risoluzione inferiore. È separato dalla FG. |
| **Ingresso / uscita** | La funzione del gioco intercettata da OptiScaler / il motore che OptiScaler esegue al suo posto. |
| **API: DX11, DX12, Vulkan** | L'interfaccia grafica usata dal gioco. API diverse supportano percorsi diversi. |
| **HUD / HUD-less** | L'interfaccia sovrapposta sullo schermo / un'immagine del gioco priva di quell'interfaccia. |
| **DLL / loader / proxy** | Una libreria di codice e il file con cui il gioco carica una mod. Il nome del file conta. |
| **HAGS** | La pianificazione GPU con accelerazione hardware di Windows. |
| **Runtime** | Un componente software richiesto da una funzione, per esempio la DLL DLSS del gioco o Visual C++. |
| **Cache / SHA-256** | Un pacchetto scaricato conservato / un'impronta usata per verificare l'integrità dei file. La corrispondenza dell'impronta non certifica il funzionamento nel gioco e non rende supportata una procedura sconosciuta. |
| **Backup / registro della transazione** | Lo stato precedente dei file salvato / informazioni per recuperare una modifica interrotta. |

<a id="sources"></a>
## 18. Progetti originali e approfondimenti

- [Download e progetto del gestore](https://github.com/VenomDrake/DLSSG-Mod-Manager).
- [DLSSG for SM86](https://github.com/sdli1995/dlssg_for_sm86), di sdli1995 e collaboratori; [adattamento SM75](https://github.com/Coldwood1026), di Coldwood1026.
- [Apex Neural Rendering](https://github.com/apex-compute/ANR), di Apex Compute e collaboratori.
- [OptiScaler](https://github.com/optiscaler/OptiScaler) e la sua [wiki ufficiale](https://github.com/optiscaler/OptiScaler/wiki), dei relativi collaboratori.
- [Panoramica del gestore](README.it.md) e [note dei componenti di terze parti](../../THIRD_PARTY_NOTICES.md).

Questa guida descrive i **controlli esposti dal gestore 0.5.1** e le procedure dei provider che ha verificato. Non garantisce la compatibilità di ogni gioco, non è un benchmark e non è un manuale completo di tutte le opzioni degli overlay originali. Per le funzioni avanzate disponibili soltanto nei progetti originali usa la documentazione della versione corrispondente.
