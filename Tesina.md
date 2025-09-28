# Tesina

Link repository: https://github.com/Fracanta98/Esame-C--api


## Introduzione
Le REST API (Representational State Transfer) rappresentano un’architettura per la comunicazione tra sistemi distribuiti, costruita su protocolli web standard come HTTP sfruttando il protocollo TCP per la gestione degli errori. Si basano su alcuni principi fondamentali che ne definiscono il funzionamento. 
Ogni richiesta deve essere stateless, ossia contenere tutte le informazioni necessarie senza che il server mantenga lo stato tra una chiamata e l’altra, le risorse sono esposte tramite un’interfaccia uniforme, identificate da URL e manipolate attraverso i metodi HTTP standard. 
L’architettura segue il modello client-server, garantendo la separazione tra chi utilizza i servizi e chi li fornisce, favorendo così la scalabilità. 
Inoltre, le risposte possono essere cacheable, cioè memorizzate per migliorare le prestazioni, e l’intero sistema può essere organizzato in più livelli, includendo componenti intermedi come proxy o gateway. 
Grazie a queste caratteristiche, le REST API risultano semplici da utilizzare, scalabili e facilmente integrabili con applicazioni web e mobile.

## Confronto tra REST API, SOAP e GraphQL
Le REST API rappresentano un modello architetturale per la creazione di servizi web basati su HTTP, che si focalizza sulla semplicità, l’uso delle risorse e l’adozione di metodi standard come GET, POST, PUT e DELETE. 
Al contrario, SOAP (Simple Object Access Protocol) è un protocollo più complesso e strutturato che utilizza messaggi in formato XML per lo scambio di dati, garantendo un alto livello di standardizzazione, affidabilità e sicurezza, particolarmente adatto ad ambienti enterprise dove sono richieste transazioni complesse e formalità nei messaggi.
SOAP si basa su un set rigoroso di regole definite da WSDL (Web Services Description Language) e supporta vari protocolli di trasporto, non solo HTTP.
GraphQL, invece, è un linguaggio di interrogazione e runtime API che permette ai client di specificare esattamente quali dati desiderano ricevere, riducendo il problema del sovraccarico di dati e minimizzando le richieste multiple. A differenza di REST, che espone molteplici endpoint, GraphQL offre un unico endpoint flessibile con uno schema definito che abilita query e mutazioni precise. Questo approccio è particolarmente utile in applicazioni complesse o con dati molto correlati, anche se comporta una maggiore complessità nella gestione dello schema e della sicurezza.
In conculusione, REST si distingue per la sua semplicità e scalabilità particolarmente adatte allo sviluppo web e mobile, SOAP per la sua robustezza e formalità nei contesti enterprise, mentre GraphQL fornisce flessibilità e controllo sui dati, diventando una scelta ideale per applicazioni moderne con esigenze di interazione dinamica e personalizzata.

## Metodi HTTP principali
Le REST API si basano sul protocollo HTTP, che a sua volta utilizza il protocollo TCP (Transmission Control Protocol) come livello di trasporto affidabile su cui inviare i dati tra client e server. TCP garantisce che i pacchetti di dati vengano consegnati in ordine, senza perdite, e senza duplicazioni, creando una connessione stabile prima che inizi la comunicazione HTTP. Questa connessione affidabile permette di eseguire con precisione le operazioni offerte dai metodi HTTP.

I principali metodi HTTP utilizzati nelle REST API sono:
- GET: Serve a recuperare dati dal server senza modificarli. È una richiesta "sicura" e idempotente, usata per leggere risorse, esempio:
    ```csharp
        app.MapGet("/studenti", (StudentiContext db) =>  
    {
        List<Studente> elenco = db.Studenti.ToList();
        return Results.Ok(elenco);
        });
    ```
    Il client invia una richiesta TCP al server, stabilisce la connessione e invia un messaggio HTTP GET per ottenere la lista di tutti gli studenti presenti nel database.

- POST: Utilizzato per creare una nuova risorsa sul server. A differenza di GET, POST può modificare lo stato sul server e invia dati nel corpo della richiesta detto body, esempio:
  ```csharp
        app.MapPost("/aggiungistudente", (Studente studente, StudentiContext db) => 
        {
            db.Studenti.Add(studente);
            db.SaveChanges();
            return Results.Created();
        });
    ```
    Il client apre la connessione TCP, invia dati JSON con una richiesta HTTP POST per aggiungere un nuovo studente al database.

- PUT: Serve ad aggiornare una risorsa esistente. Anche PUT è idempotente, cioè inviare più volte la stessa richiesta ha lo stesso effetto.  
    ```csharp
                app.MapPut("/aggiornastudente/{id}", (int id, Studente studenteAggiornato, StudentiContext db) =>
            {
                var studenteEsistente = db.Studenti.Find(id);
                if (studenteEsistente is null)
                {
                    return Results.NotFound();
                }

            
                studenteEsistente.Nome = studenteAggiornato.Nome;
                studenteEsistente.Cognome = studenteAggiornato.Cognome;
                studenteEsistente.Età = studenteAggiornato.Età;
            

                db.SaveChanges();
                return Results.Ok(studenteEsistente);
            });
    ```
    Il client apre la connessione TCP, invia dati JSON con una richiesta HTTP POST per aggiornare uno studente esistente nel database.
- DELETE: Elimina una risorsa specifica sul server ed è anch’esso idempotente, esempio:
    ```csharp
        app.MapDelete("/cancellastudente/{Id}", (int Id, StudentiContext db) =>
        {
            Studente? s = db.Studenti.Find(Id);
            if (s is null)
                return Results.NotFound();

            db.Studenti.Remove(s);
            db.SaveChanges();
            return Results.Ok();
        });
    ```
In conclusione, mentre il protocollo HTTP definisce i metodi e le modalità di interazione per le REST API, il protocollo TCP rappresenta la base tecnica che assicura la correttezza, l’ordine e la consegna dei messaggi che rendono possibile questa comunicazione fluida e sicura. Senza il protocollo TCP, le API REST non potrebbero garantire l’affidabilità necessaria nelle comunicazioni web moderne.

## Codici di stato HTTP più comuni
I codici di stato HTTP sono fondamentali per la comunicazione tra client e server nelle REST API, poiché indicano l’esito di ogni richiesta. Ogni codice è composto da tre cifre e appartiene a una delle cinque classi principali:
- 1xx (Informativi): indicano che la richiesta è stata ricevuta e il processo continua.
- 2xx (Successo): la richiesta è stata elaborata correttamente.
- 3xx (Redirezione): il client deve compiere ulteriori azioni per completare la richiesta.
- 4xx (Errore del client): la richiesta contiene errori da parte del client o non è autorizzata.
- 5xx (Errore del server): il server ha riscontrato un problema interno.


I codici più utilizzati nelle REST API includono:
- 200 OK: indica che la richiesta è andata a buon fine e il server ha restituito i dati richiesti. È il codice più comune per le operazioni GET.
- 201 Created: viene restituito dopo una richiesta POST quando una nuova risorsa è stata creata con successo. Spesso include l’URL della risorsa appena creata.
- 204 No Content: segnala che la richiesta è stata completata correttamente, ma il server non restituisce alcun contenuto. È tipico per operazioni DELETE o PUT.
- 400 Bad Request: il server non può elaborare la richiesta a causa di errori di sintassi o parametri non validi. È utile per segnalare input errati da parte del client.
- 401 Unauthorized: il client non ha fornito credenziali valide. È spesso usato in contesti con autenticazione tramite API Key o JWT.
- 403 Forbidden: il client è autenticato ma non ha i permessi necessari per accedere alla risorsa.
- 404 Not Found: la risorsa richiesta non esiste. È uno dei codici più comuni nelle API REST.
- 500 Internal Server Error: indica un errore generico lato server, spesso dovuto a eccezioni non gestite o problemi di configurazione.

## Autenticazione e sicurezza(API KEY E JWT)
Nel contesto delle REST API, la sicurezza e l’autenticazione sono elementi imprescindibili per proteggere le risorse e garantire che solo utenti o sistemi autorizzati possano accedervi. Un aspetto chiave per implementare questi meccanismi in applicazioni .NET, comprese le Minimal API, è l’uso dei middleware.
I middleware sono componenti software che si inseriscono nella pipeline di elaborazione delle richieste HTTP tra client e server. Agiscono come "filtri" o "intermediari" che possono intercettare, modificare, bloccare o autorizzare le richieste e le risposte durante il loro percorso. In pratica, i middleware consentono di centralizzare funzionalità trasversali come autenticazione, logging, gestione degli errori, compressione, e altro ancora, senza dover ripetere codice in ogni singolo endpoint.
Per proteggere le API, si utilizzano middleware che controllano la validità delle credenziali o dei token presenti nelle richieste prima che queste raggiungano il codice dell’applicazione. Nel mondo .NET, la configurazione dei middleware per l’autenticazione è semplice e modulare, favorendo la gestione di diverse strategie.
Le API Key sono chiavi univoche assegnate ai client, che devono essere inviate insieme a ogni richiesta, tipicamente come header HTTP. Un middleware dedicato intercetta la richiesta, verifica la presenza e la validità della chiave, e decide se permettere o bloccare l’accesso. Questo metodo è semplice, ma offre una protezione base e non gestisce informazioni sull’utente o scadenze.
I JWT, invece, sono token firmati digitalmente che includono informazioni codificate (claims) sull’utente, come ID, ruoli o scadenza. Dopo l’autenticazione iniziale, il server genera un JWT che il client conserva e invia in ogni richiesta. Il server può verificarne la validità senza dover consultare un database, questo sistema è stateless, perché non richiedendo la memorizzazione delle sessioni sul server, rende l’architettura più scalabile e performante.

## Documentazione delle API: OpenAPI/Swagger
La documentazione delle API è un elemento cruciale per facilitare la comprensione, l’utilizzo e l’integrazione delle REST API da parte di sviluppatori, team di sviluppo e consumatori esterni. Senza una documentazione chiara e dettagliata, anche un’API ben progettata rischia di risultare difficile da usare o di generare errori nell’implementazione client. Per questo motivo, OpenAPI e Swagger rappresentano gli standard de facto per la descrizione e la documentazione delle API REST, garantendo trasparenza, coerenza e automazione.
OpenAPI è una specifica formale che definisce un linguaggio standard in formato JSON o YAML per descrivere un’interfaccia API RESTful. Con OpenAPI, è possibile descrivere in modo completo tutti gli aspetti di un’API, tra cui gli endpoint, i metodi HTTP supportati, i parametri (sia di query, path, header o corpo), i tipi di dati delle richieste e delle risposte, i codici di stato HTTP previsti, e le eventuali misure di sicurezza applicate come autenticazione e autorizzazione. Questa definizione funge da vero e proprio contratto tra server e client, permettendo di standardizzare l’interazione e di automatizzare processi come la generazione di client SDK, test automatici, e validazione.
Swagger è invece un insieme di strumenti open source costruiti intorno alla specifica OpenAPI. Il più noto di questi è Swagger UI, che offre un’interfaccia web interattiva in cui gli sviluppatori possono esplorare tutti gli endpoint dell’API, visualizzare la struttura delle richieste e risposte, comprendere i parametri richiesti e provare direttamente le chiamate HTTP tramite il browser. Questo strumento è estremamente utile durante lo sviluppo e il testing, consentendo un feedback immediato e un rapido debug. Altri componenti di Swagger includono:

- Swagger Editor, che permette di scrivere e modificare manualmente i file OpenAPI con supporto per la validazione in tempo reale.

- Swagger Codegen, che automatizza la generazione di client SDK e server stub in vari linguaggi, facilitando l’integrazione con diverse piattaforme e riducendo i tempi di sviluppo.

Questa configurazione automatizza la scansione degli endpoint, genera in tempo reale la documentazione aggiornata e fornisce un’interfaccia intuitiva e accessibile a chiunque necessiti di interagire con l’API.
Inoltre, Swagger supporta l’integrazione con sistemi di autenticazione come JWT o API Key, permettendo di documentare anche le modalità di sicurezza e di testare l’accesso protetto direttamente dall’interfaccia. Questo rende più semplice per i team di sviluppo garantire conformità e sicurezza, e per gli sviluppatori client capire come effettuare richieste autorizzate.
Infine, la documentazione generata con OpenAPI/Swagger si rivela uno strumento fondamentale non solo in fase di sviluppo, ma anche in ambiti di DevOps e continuous integration, grazie alla possibilità di integrarla nei processi automatici di test, validazione e distribuzione.

## Fonti
- https://learn.microsoft.com
- https://spec.openapis.org
- https://swagger.io/docs
- https://auth0.com/learn/json-web-tokens
- https://blog.postman.com/rest-vs-soap-vs-graphql

