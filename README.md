# Projet YARA Scanner avec Base de Données SQLite
Description: 

Ce projet est une application Windows Forms permettant d'analyser des fichiers à l'aide de l'outil YARA. Les résultats des analyses sont enregistrés dans une base de données SQLite pour une gestion et un suivi ultérieurs.

## Structure du Projet
### 1. DatabaseHelper

Cette classe gère la communication avec la base de données SQLite.

Principales responsabilités :

- Création de la base de données et des tables si elles n'existent pas.  
- Insertion des résultats des analyses.  
- Récupération des résultats par Scan ID ou critères spécifiques (ex : fichiers suspects).  
- Calcul des statistiques comme le nombre total de fichiers analysés et le nombre de fichiers suspects.  

Méthodes clés :

- InitializeDatabase() : Crée la base de données et la table.  
- InsertScanResult() : Enregistre un résultat dans la table.  
- GetResultsByScanId(string scanId) : Récupère les résultats pour un scan donné.  
- GetTotalFilesByScanId(string scanId) : Retourne le nombre de fichiers analysés dans un scan.  
- GetSuspiciousFilesByScanId(string scanId) : Retourne le nombre de fichiers suspects dans un scan.
- GetSuspiciousFilesNamesByScanId(string scanId) : Retourne le nom des fichiers suspects dans un scan.

### 2. YaraScanner

Cette classe encapsule l'exécution de l'outil YARA pour analyser des fichiers.

Principales responsabilités :

- Exécution des commandes YARA en ligne de commande.  
- Analyse des fichiers en utilisant un ensemble de règles YARA.   
- Gestion des résultats (succès, erreurs) et enregistrement des informations dans la base de données.  

Méthodes clés :

- ScanFile(string scanId, string rulesArguments, string targetFile) : Effectue l'analyse d'un fichier donné avec des règles YARA et enregistre les résultats dans la base de données.  

### 3. Program.cs

Ce fichier contient le point d'entrée principal du projet. Il initialise les composants, crée les instances des classes principales et lance les analyses.  

Principales étapes :  
- Sélection d'un répertoire cible pour l'analyse.  
- Création d'une instance de DatabaseHelper pour gérer la base de données.  
- Création d'une instance de YaraScanner pour exécuter les analyses.  
- Analyse des fichiers dans le répertoire choisi.  
- Affichage des résultats dans l'interface utilisateur.  
