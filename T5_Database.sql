DROP DATABASE IF EXISTS T5_BBDDJuego;

CREATE DATABASE T5_BBDDJuego;
USE T5_BBDDJuego;

CREATE TABLE Users (
    Player_ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(50) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Profile_Picture VARCHAR(255),
    Profile_Notes VARCHAR(500)
);

CREATE TABLE Game (
    Game_ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    GameStatus INT NOT NULL,
    PosicionFichas VARCHAR(255) NOT NULL,
	GameAdmin INT NOT NULL
);

CREATE TABLE Game_Members (
	Game_ID INT NOT NULL,
	Player_ID INT NOT NULL,
	PlayerNumber INT NOT NULL,
    FOREIGN KEY(Game_ID) REFERENCES Game(Game_ID),
    FOREIGN KEY(Player_ID) REFERENCES Users(Player_ID)
);

CREATE TABLE Configuration (
	Version_ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	Mandatory INT NOT NULL
);

-- Introduzco Datos en la BBDD
INSERT INTO Users (Username, Password) VALUES ('Alex','host2');
INSERT INTO Users (Username, Password) VALUES ('Judit','colino');
INSERT INTO Users (Username, Password) VALUES ('Ignasi','juntsxcat');
INSERT INTO Users (Username, Password) VALUES ('Joel','deutschland');