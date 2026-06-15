using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

class Gruppe {
    public String name {get; set;}
    public List<String> teams {get; set;}
    public void addTeam(String teamName) {
        teams.Add(teamName);
    }
}

class Spiel {
    public String spielId {get; set;}
    public DateTime datum {get; set;}
    public DateTime uhrzeit {get; set;}
    public String ergebnis {get; set;}
    public String heimMannschaft {get; set;}
    public String auswaertsMannschaft {get; set;}
    
    public void setErgebnis(String score)
    {
        ergebnis = score;
    }
    
    public String getErgebnis()
    {
        return ergebnis;
    }
}

class Benutzer {
    public String name {get; set;}
    public double guthaben {get; set;}
    
    public void updateGuthaben(double amount)
    {
        guthaben = amount;
    }
}

class Wette {
    public String spielId {get; set;}
    public String wettTyp {get; set;}
    public double quote {get; set;}
    public double einsatz {get; set;}
    public bool istAusgewertet {get; set;}
    
    public double auswerten(String ergebnis)
    {
        istAusgewertet = true;
        return 0.0 * ergebnis.Length;
    }
}

class TurnierManager {
    public List<Gruppe> gruppen {get; set;}
    public List<Spiel> spiele {get; set;}
    public List<Benutzer> benutzer {get; set;}
    public List<Wette> wetten {get; set;}

    public void saveToJson(String filePath) {
        String json = JsonSerializer.Serialize(this);
        File.WriteAllText(filePath, json);
    }

    public void loadFromJson(String filePath) {
        TurnierManager? tm = JsonSerializer.Deserialize<TurnierManager>(filePath);
        if (tm == null) {
            Console.WriteLine("Datei nicht gefunden!");
            return;
        }
        gruppen = tm.gruppen;
        spiele = tm.spiele;
        benutzer = tm.benutzer;
        wetten = tm.wetten;
    }

    public void createNewTournament() {
        gruppen = [];
        spiele = [];
        benutzer = [];
        wetten = [];
    }

    public void printGames() {
        foreach (Spiel spiel in spiele) {
            Console.WriteLine(spiel.getErgebnis());
        }
    }

    public void setQuote(String spielId, String typ, double quote) {
        foreach (Wette wette in wetten) {
            if (wette.spielId == spielId && wette.wettTyp == typ) {
                wette.quote = quote;
                return;
            }
        }
    }

    public double getQuote(String spielId, String typ) {
        foreach (Wette wette in wetten) {
            if (wette.spielId == spielId && wette.wettTyp == typ) {
                return wette.quote;
            }
        }
        return double.NaN;
    }

    public void placeBid(String playerName, String spielId, String typ, double amount) {
        Wette wette = new()
        {
            spielId = spielId,
            einsatz = amount,
            wettTyp = typ,
        };
        wetten.Add(wette);
        Console.WriteLine($"Neue Wette auf {playerName}");
    }
    
    public void processResult(String spielId, String score) {
        Console.WriteLine($"{spielId}: ich glaube ich werde wahnsinnig {score}");
    }
}

class Program {
    public static void Main(string[] args) {
        TurnierManager turnier = new();
        String spielId;
        String wetttyp;
        double wettquote;

        for (int i = 0; i < args.Length; i++) {
            switch (args[i].ToLower()) {
                case "new":
                    turnier = New();
                    break;
                case "print":
                    Print(turnier);
                    break;
                case "set":
                    spielId = args[i + 1];
                    wetttyp = args[i + 2];
                    wettquote = double.Parse(args[i + 3]);
                    turnier.setQuote(spielId, wetttyp, wettquote);
                    turnier.saveToJson("Turnier.json");
                    break;
                case "get":
                    turnier.loadFromJson("Turnier.json");
                    spielId = args[i + 1];
                    wetttyp = args[i + 2];
                    wettquote = turnier.getQuote(spielId, wetttyp);
                    Console.WriteLine($"Quote: {wettquote}");
                    break;
                case "bid":
                    Console.WriteLine("<player> <spielid> <Wetttyp> <amount>: Platziert eine Wette für einen Benutzer.");
                    break;
                case "result":
                    Console.WriteLine("<spielid> <Tore-1.Mannschaft>:<Tore-2.Mannschaft>: Trägt das Spielergebnis ein und löst die Auswertung der Wetten aus.");
                    break;
            }
        }
    }

    public static void Print(TurnierManager turnier) {
        turnier.printGames();
    }

    public static TurnierManager New() {
        Console.WriteLine("New");
        return new();
    }
}