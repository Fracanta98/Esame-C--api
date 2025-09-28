# Relazione del lavoro svolto

Il progetto comprende una minimal API sviluppata in C# con database persistente (Sqlite).
Le REST API (Representational State Transfer) sono un’architettura per lo sviluppo di servizi web che permette la comunicazione tra client e server tramite il protocollo HTTP.
I metodi HTTP utilizzati in questo progetto sono:
- GET
- POST
- PUT
- DELETE

Ricordiamo che questi metodi HTTP sfruttano il protocollo TCP per la gestione degli errori.

Il progetto è stato sviluppato con interfaccia Swagger (openAPI) per tanto si consiglia l'utilizzo di questo strumento per il testing, la struttura è la seguente:

## Classi
In questa API sono state sviluppate 2 classi correlate ed un di "appoggio" per il login:
1) La classe Studente che come attributi ha:
    - Id, questa è la primary key per il DB
    - Nome
    - Età
    - Corsi

2) La classe Corsi che come atrributi ha:
    - Id, primary key per il DB
    - Nome
    - ore

Da notare come le due classi sono collegate in quanto uno studente nell'attributo Corsi avrà un array di oggetti "Corso"

3) La classe Credenziali che come attributi avrà:
    - Password 
    - Username

## Context
In questa cartella andiamo a strutturare il database(Sqlite), qui andiamo a definire le tabelle Studenti e Corsi

## Program.cs
Una volta creata la struttura qui andremo a sviluppare il cuore pusalnte dell'API, infatti qui andiamo a creare il database con le annesse tabelle:


### Build del database 
```csharp
 builder.Services.AddDbContext<StudentiContext>(
        options => options.UseSqlite("Data Source=Studenti.db") //crea il db
    );

builder.Services.AddDbContext<StudentiContext>(
        options => options.UseSqlite("Data Source=Corsi.db") //crea il db
    );


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StudentiContext>(); //se non esiste il db lo crea
    db.Database.EnsureCreated();
}
```

In queste righe di codice facciamo la build del database e delle cartelle Studenti e Corsi, come misura di sicurezza per il primo utilizzo specifichiamo di creare il DB in caso non ne venga trovato uno già esistente.

### Middleware di autenticazione
Qui sviluppiamo il middleware che ci permetterà di implementare la funzione di autenticazione tramite login o token di accesso: 

```csharp 
        app.Use(async (context, next) =>   //middleware per autenticazione
        {
            if (context.Request.Path == "/login")
                await next.Invoke();
            else
            {
                if (context.Request.Headers.TryGetValue("Access", out var valoreToken))
                {
                    if (valoreToken == "Test")
                    {
                        await next.Invoke();
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Accesso negato: token non valido.");
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Accesso negato: token non presente.");
                }
            }
        });
```
Qui l'API cercherà se è presente un token nell'header in questo caso contenente una stringa "Test" 
se non presente forzerà il path login in qui verranno richieste username e password.
Una volta effettutato il login verrà assegnato il token di accesso 

### Richieste GET
Le richieste GET sono richieste HTTP contenti solo l'header quindi prive di body per cui sono operazioni di sola lettura.
GET implementate nel progetto:
 - "/studenti" ci permette di visualizzare l'elenco di tutti gli studenti presenti nel db.
 - "/corsi" ci permette di visualizzare l'elenco di tutti i corsi presenti nel db.
 - "/studenti/{Id}" ci permette di cercare uno studente specifico nel db tramite l'id.
 - "/corsi/{nome}" ci permette di cercare un corso specifico nel db tramite il nome.
 - "/studentecorsi/{Id}" ci permette di cercare i corsi frequentati da un singolo studente tramite l'Id di quest'ultimo.

 Analizziamo l'ultima richiesta GET trattata nell' elenco:

 ```csharp
        app.MapGet("/studentecorsi/{Id}", (int Id, StudentiContext db) =>  //analogo alla ricerca studente ma restituisce solo i corsi
        {                                                            
            Studente? s = db.Studenti.Find(Id);
            if (s is null)
                return Results.NotFound();

            return Results.Ok(s.corsi);
        });
 ``` 
Una volta definita la route definiamo la variabile Id (id dello studente da cercare), poi tramite un'arrow function creiamo un oggetto nullable di tipo Studente "s" e gli imponiamo che deve essere uguale allo studente nel db con lo specifico Id precedentemente impostato.
Da qui se l'oggetto "s" è null l'API restituirà un rerrore 404 rislutato non trovato.
In caso di esistenza invece l'API riporterà uno status code 200 mostrando i corsi dello studente in formato JSON, da notare che se avessimo voluto esplicitare tutti i dati dello studente avrermmo dovuto scrivere nell'ultima riga " return Results.Ok(s.corsi); ".
Il ragionamento è analogo per tutte le altre richieste GET.

### Richieste POST/PUT
Questo tipo di richieste a differenza delle GET comprendono un body contenente dei dati in formato JSON da passare al db, richieste implementate in questo progetto sono:
 - "/login", il path che ci permette di effettuare l'autenticazione tramite credenziali.
 - "/aggiungistudente" ci permette di aggiungere uno studente al DB.
 - "/aggiungicorso" ci permette di aggiungere un corso al DB.
 
Analizziamo l'ultima richiesta POST trattata nell' elenco:

```csharp
        app.MapPost("/aggiungicorso", (Corso corso, StudentiContext db) =>  //aggiungi un corso
        {
            db.Corsi.Add(corso);
            db.SaveChanges();
            return Results.Created(); 

        });
 ``` 

 Praticamente tramite questo frammento di codice andiamo a passare un oggetto di tipo "Corso" in formato JSON e tramite la funzione ".add" lo andiamo ad aggiungere al DB.
 Una volta faccio ciò la funzuione andrà a salvare i cambiamenti nel DB ed a riportare uno status "Created" all'utente.

 ### Richieste Delete
 Normalmente le richieste DELETE come per le GET non hanno un body, in quanto mantengono le informazioni dell'oggetto da cancellare nell'URL.
 Le richieste DELETE implementate in questo progetto sono:
  - "/cancellastudente/{Id}", ci permette di cancellare un ogetto Studente nel DB tramite Id.
  - "/cancellacorso/{Id}", ci permette di cancellare un oggetto Corso nel DB tramite Id.

  Analizziamo l'ultima richiesta DELETE trattata nell' elenco:

  ```csharp
        app.MapDelete("/cancellacorso/{Id}", (int Id, StudentiContext db) =>
        {
            Corso? c = db.Corsi.Find(Id);
            if (c is null)
                return Results.NotFound();

            db.Corsi.Remove(c);
            db.SaveChanges();
            return Results.Ok();
        });
``` 
In questo frammento di codice vogliamo cancellare un corso presente nel db sfruttando sua primary key ovvero l'Id che verrà passato nell'URL, poi creiamo un oggetto nullable "c" di tipo "Corso" e gli assegneremo gli stessi valori dell'oggetto nel DB.
Se l'oggetto non viene trovato "c" sarà null quindi verrà restituito uno status code 404, invece nel caso in cui l'oggetto venga trovato viene rimosso da DB, una volta fatto ciò vengono slavate le modifiche apportate e viene restituito uno status code 200.






