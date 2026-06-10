class Gruppe {
    String name;
    List<String> teams;
    public void addTeam(String teamName) {
        teams.Add(teamName);
    }
}

class Spiel {
    String spielId;
    DateTime datum;
    DateTime uhrzeit;
    String ergebnis;
    String heimMannschaft;
    String auswaertsMannschaft;
    
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
    String name;
    double guthaben;
    
    public void updateGuthaben(double amount)
    {
        guthaben = amount;
    }
}

class Wette {
    String wettTyp;
    double quote;
    double einsatz;
    bool istAusgewertet;
    
    public double auswerten(String ergebnis)
    {
        return 0.0 * ergebnis.Length;
    }
}

class TurnierManager {
    List<Gruppe> gruppen;
    List<Spiel> spiele;
    List<Benutzer> benutzer;
    List<Wette> wetten;

    public void saveToJson(String filePath) {
        Console.WriteLine($"saveToJson: {filePath}");
    }

    public void loadFromJson(String filePath) {
        Console.WriteLine($"loadFromJson: {filePath}");
    }

    public void createNewTournament() {

    }

    public void printGames() {
        foreach (Spiel game in spiele)
        {
            Console.WriteLine(game.getErgebnis());
        }
    }

    public void setQuote(String spielId, String typ, double quote) {
        Wette wette = new();
    }

    public double getQuote(String spielId, String typ) {
        return 0.0;
    }

    public void placeBid(String playerName, String spielId, String typ, double amount) {
        Wette wette = new();
    }
    
    public void processResult(String spielId, String score) {
        Console.WriteLine($"{spielId}: ich glaube ich werde wahnsinnig {score}");
    }
}
