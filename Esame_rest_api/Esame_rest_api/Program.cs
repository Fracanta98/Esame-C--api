using Esame_rest_api.Context;
using Microsoft.EntityFrameworkCore;
using System;
using Esame_rest_api.Classi;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StudentiContext>(
        options => options.UseSqlite("Data Source=Studenti.db") //crea il db
    );

builder.Services.AddDbContext<StudentiContext>(
        options => options.UseSqlite("Data Source=Corsi.db") //crea il db
    );

builder.Services.AddCors();

var app = builder.Build();






using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StudentiContext>(); //se non esiste il db lo crea
    db.Database.EnsureCreated();
}


app.UseHttpsRedirection();


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




app.Run();

