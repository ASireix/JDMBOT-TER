# 🤖 Bot Discord de gestion de connaissances

# JDMBOT-TER

JDMBOT-TER est un bot Discord de gestion de connaissances, développé en **C# avec .NET 9**, capable d’explorer le graphe lexical **JeuxDeMots (JDM)**. Il permet d’interagir avec les utilisateurs via des commandes Discord et d’extraire, filtrer et sauvegarder des relations sémantiques dans une base de données locale.

---

## ⚙️ Prérequis techniques

### Environnement logiciel

- **.NET 9 SDK**  
  Téléchargeable sur le [site officiel Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

- **C#**  
  Inclus dans le SDK .NET.

- **SQLite**  
  Base de données légère utilisée pour la persistance. Outils conseillés : [DB Browser for SQLite](https://sqlitebrowser.org/)

### Comptes et accès

- **Compte Discord**  
  Pour créer et gérer une application Discord (le bot).

- **Application Discord (Bot Token)**  
  À créer sur le [Discord Developer Portal](https://discord.com/developers/applications)

### Dépendances NuGet

- `DSharpPlus` — interaction avec Discord  
- `Entity Framework Core` — ORM pour la base de données  
- `Newtonsoft.Json`, `System.Text.Json` — sérialisation JSON  
- Autres packages pour requêtes HTTP, logs, etc.

### Configuration réseau

- Accès Internet requis pour :
  - L’API JeuxDeMots (JDM)
  - Le fonctionnement du bot Discord

---

## 🛠️ Installation et configuration

Voici les étapes pour installer et exécuter le bot :

### 1. Cloner le projet


`git clone https://github.com/ton-utilisateur/JDMBOT-TER.git`
cd JDMBOT-TER


### 2. Restaurer les dépendances
exécuter la commande `dotnet restore`dans le répertoire racine du projet

### 3. Créer l'application Discord et récupérer le token
- aller sur Discord  Developper Portal
- cliquer sur "new Application"
- Aller dans l'onglet Bot -> Add Bot
- copier le token du bot et coller dans 'Config/config.json' 

### 4. Lancer le bot
- exécuter la commande `dotnet run`

## Liens utiles  
- [RezoJDM : Description générale](http://www.jeuxdemots.org/jdm-about.php)  
- [API](https://jdm-api.demo.lirmm.fr/)  

