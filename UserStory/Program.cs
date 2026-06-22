using System.Text.Json;

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
        guthaben += amount;
    }
}

class Wette {
    public String spielId {get; set;}
    public String wettTyp {get; set;}
    public String benutzer {get; set;}
    public double quote {get; set;}
    public double einsatz {get; set;}
    public bool istAusgewertet {get; set;}
    
    public double auswerten(String ergebnis)
    {
        if (!istAusgewertet && wettTyp != ergebnis) {
            return 0.0;
        }
        istAusgewertet = true;
        return einsatz * quote;
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
        String json = File.ReadAllText(filePath);
        TurnierManager? tm = JsonSerializer.Deserialize<TurnierManager>(json);
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
        Wette neu_wette = new()
        {
            spielId = spielId,
            wettTyp = typ,
            quote = quote
        };
        wetten.Add(neu_wette);
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
            benutzer = playerName,
        };
        wetten.Add(wette);
        bool userExists = false;
        foreach (Benutzer nutzer in benutzer) {
            if (nutzer.name == playerName) {
                userExists = true;
                break;
            }
        }
        if (!userExists) {
            benutzer.Add(new() {name = playerName, guthaben = 0,});
        }
    }
    
    public void processResult(String spielId, String score) {
        int? tore1 = 0, tore2 = 0;
        string[] parts = score.Split(':');
        tore1 = int.Parse(parts[0]);
        tore2 = int.Parse(parts[1]);
        tore1 ??= 0;
        tore2 ??= 0;
        string ergebnis = "Unentschiedenwette";
        if (tore1 > tore2) {
            ergebnis = "Siegwette";
        } else if (tore1 < tore2) {
            ergebnis = "Niederlagenwette";
        }

        foreach (Wette bid in wetten) {
            if (bid.spielId == spielId) {
                if (bid.wettTyp == ergebnis) {
                    foreach (Benutzer user in benutzer) {
                        if (user.name == bid.benutzer && !bid.istAusgewertet) {
                            user.updateGuthaben(bid.auswerten(ergebnis));
                        }
                    }
                }
            }
        }
    }
}

class Program {
    public static void Main(string[] args) {
        TurnierManager turnier = New();
        String player, spielId, wetttyp;
        double wettquote, amount;
        int tore1, tore2;

        if (args.Length <= 1) {
            if (File.Exists("Turnier.json")) {
                turnier.loadFromJson("Turnier.json");
            }
            Bid(turnier, "test-spieler", "test-spiel", "Siegwette", 500.0);
            Result(turnier, "test-spiel", 1, 0);
            turnier.printGames();
            foreach (Wette bid in turnier.wetten) {
                string done = bid.istAusgewertet ? "X" : " ";
                Console.WriteLine($"{bid.spielId} ({bid.wettTyp}): {bid.einsatz}€, {bid.quote * 100.0}% [{done}]");
            }
            foreach (Benutzer user in turnier.benutzer) {
                Console.WriteLine($"{user.name}: {user.guthaben}€");
            }
        }

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
                    Set(turnier, spielId, wetttyp, wettquote);
                    break;
                case "get":
                    spielId = args[i + 1];
                    wetttyp = args[i + 2];
                    Get(turnier, spielId, wetttyp);
                    break;
                case "bid":
                    player = args[i + 1];
                    spielId = args[i + 2];
                    wetttyp = args[i + 3];
                    amount = double.Parse(args[i + 4]);
                    Bid(turnier, player, spielId, wetttyp, amount);
                    break;
                case "result":
                    spielId = args[i + 1];
                    tore1 = int.Parse(args[i + 2]);
                    tore2 = int.Parse(args[i + 3]);
                    Result(turnier, spielId, tore1, tore2);
                    break;
            }
        }
    }

    public static void Print(TurnierManager turnier) {
        turnier.printGames();
    }

    public static TurnierManager New() {
        TurnierManager tm = new();
        tm.createNewTournament();
        return tm;
    }

    public static void Set(TurnierManager turnier, String spielId, String wetttyp, double wettquote) {
        turnier.setQuote(spielId, wetttyp, wettquote);
        turnier.saveToJson("Turnier.json");
    }

    public static void Get(TurnierManager turnier, String spielId, String wetttyp) {
        turnier.loadFromJson("Turnier.json");
        double wettquote = turnier.getQuote(spielId, wetttyp);
        Console.WriteLine($"Quote: {wettquote}");
    }

    public static void Bid(TurnierManager turnier, String player, String spielid, String wetttyp, double amount) {
        turnier.placeBid(player, spielid, wetttyp, amount);
        turnier.saveToJson("Turnier.json");
    }

    public static void Result(TurnierManager turnier, String spielId, int tore1, int tore2) {
        bool gameExists = false;
        foreach (Spiel game in turnier.spiele) {
            if (game.spielId == spielId) {
                gameExists = true;
                break;
            }
        }
        if (!gameExists) {
            turnier.spiele.Add(new() {spielId = spielId, ergebnis = "0:0"});
        }

        foreach (Spiel game in turnier.spiele) {
            if (game.spielId == spielId) {
                var score = $"{tore1}:{tore2}";
                game.setErgebnis(score);
                turnier.processResult(spielId, score);
                break;
            }
        }
        turnier.saveToJson("Turnier.json");
    }
}