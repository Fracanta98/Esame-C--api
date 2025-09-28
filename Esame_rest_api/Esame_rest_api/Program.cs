using Esame_rest_api.Context;
using Microsoft.EntityFrameworkCore;
using System;
using Esame_rest_api.Classi;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<StudentiContext>(
        options => options.UseSqlite("Data Source=Studenti.db") //crea il db
    );

builder.Services.AddDbContext<StudentiContext>(
        options => options.UseSqlite("Data Source=Corsi.db") //crea il db
    );

builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}






using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StudentiContext>(); //se non esiste il db lo crea
    db.Database.EnsureCreated();
}


app.UseHttpsRedirection();


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



app.MapPost("/login", (Credenziali cred) =>  //validazione username e password 
{
    if (string.IsNullOrWhiteSpace(cred.Password) || string.IsNullOrWhiteSpace(cred.Username))
        return Results.BadRequest();

    if (cred.Username == "Test" && cred.Password == "Test")
        return Results.Ok(new { token = "Test" });

    return Results.BadRequest();
});


app.MapPost("/aggiungistudente", (Studente studente, StudentiContext db) =>  //aggiungi studente
{
    db.Studenti.Add(studente);
    db.SaveChanges();
    return Results.Created();
});

app.MapPost("/aggiungicorso", (Corso corso, StudentiContext db) =>  //aggiungi un corso
{
    db.Corsi.Add(corso);
    db.SaveChanges();
    return Results.Created(); 

});

app.MapGet("/studenti", (StudentiContext db) =>  //visualizza la lista degli studenti  USA DTO PER NON FAR VEDERE L'ID
{
    List<Studente> elenco = db.Studenti.ToList();
    return Results.Ok(elenco);
});

app.MapGet("/corsi", (StudentiContext db) =>  //visualizza la lista dei corsi
{
    List<Corso> elenco = db.Corsi.ToList();
    return Results.Ok(elenco);
});


app.MapGet("/studenti/{Id}", (int Id, StudentiContext db) => //ricerca studenti per id 
{                                                            //da aggiornare a ricerca per matricola
    Studente? s = db.Studenti.Find(Id);
    if (s is null)
        return Results.NotFound();

    return Results.Ok(s);
});



app.MapGet("/corsi/{nome}", (String nome, StudentiContext db) => //ricerca corso per nome
{                                                           
    Corso? c = db.Corsi.Find(nome);
    if (c is null)
        return Results.NotFound();

    return Results.Ok(c);
});


app.MapDelete("/cancellastudente/{Id}", (int Id, StudentiContext db) =>
{
    Studente? s = db.Studenti.Find(Id);
    if (s is null)
        return Results.NotFound();

    db.Studenti.Remove(s);
    db.SaveChanges();
    return Results.Ok();
});



app.MapDelete("/cancellacorso/{Id}", (int Id, StudentiContext db) =>
{
    Corso? c = db.Corsi.Find(Id);
    if (c is null)
        return Results.NotFound();

    db.Corsi.Remove(c);
    db.SaveChanges();
    return Results.Ok();
});



app.MapGet("/studentecorsi/{Id}", (int Id, StudentiContext db) =>  //analogo alla ricerca studente ma restituisce solo i corsi
{                                                            
    Studente? s = db.Studenti.Find(Id);
    if (s is null)
        return Results.NotFound();

    return Results.Ok(s.corsi);
});




app.Run();

