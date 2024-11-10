DROP DATABASE IF EXISTS T5_BBDDJuego;

CREATE DATABASE T5_BBDDJuego;
USE T5_BBDDJuego;

CREATE TABLE Users (
	Player_ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	Username VARCHAR(32) NOT NULL,
	Password VARCHAR(32) NOT NULL,
	Profile_Picture VARCHAR(255),
	Profile_Notes VARCHAR(255)
);

CREATE TABLE Teams (
	Group_ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	Group_Name VARCHAR(32) NOT NULL,
	Last_User VARCHAR(32),
	Last_Message VARCHAR(255),
	Group_Notes VARCHAR(255),
	Profile_Picture VARCHAR(255)
);

CREATE TABLE Group_Members (
	Group_ID INT NOT NULL,
	Player_ID INT NOT NULL,
    	FOREIGN KEY(Group_ID) REFERENCES Teams(Group_ID),
    	FOREIGN KEY(Player_ID) REFERENCES Users(Player_ID)
);

CREATE TABLE Game (
	Game_ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	Group_ID INT NOT NULL,
	Kind INT NOT NULL,
	Status INT NOT NULL,
	Time_Stamp VARCHAR(255) NOT NULL,
	FOREIGN KEY(Group_ID) REFERENCES Teams(Group_ID)
);


CREATE TABLE Game_Record (
	Game_ID INT(32) NOT NULL,
	Player_ID INT(32) NOT NULL,
	Points INT(8) NOT NULL,
	Variable_Field VARCHAR(255),
	FOREIGN KEY(Game_ID) REFERENCES Game(Game_ID),
	FOREIGN KEY(Player_ID) REFERENCES Users(Player_ID)

);

CREATE TABLE Chat (
	Message_ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	Group_ID INT NOT NULL,
	Player_ID INT NOT NULL,
	Message_Content VARCHAR(1024) NOT NULL,
	Time_Stampt VARCHAR(255) NOT NULL,
	FOREIGN KEY(Group_ID) REFERENCES Teams(Group_ID),
	FOREIGN KEY(Player_ID) REFERENCES Users(Player_ID)
);

CREATE TABLE Configuration (
	Version_ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	Mandatory INT NOT NULL,
	Time_Stamp VARCHAR(255) NOT NULL
);

-- Introduzco Datos en la BBDD
INSERT INTO Users (Username, Password) VALUES ('Alex','host2');
INSERT INTO Users (Username, Password) VALUES ('Judit','colino');
INSERT INTO Users (Username, Password) VALUES ('Ignasi','juntsxcat');
INSERT INTO Users (Username, Password) VALUES ('Joel','deutschland');

INSERT INTO Teams (Group_Name) VALUES ('Espanya');

INSERT INTO Game (Group_ID, Kind, Status, Time_Stamp) VALUES (1, 1, 1, 2200);

INSERT INTO Game_Record (Game_ID, Player_ID, Points) VALUES (1, 2, 900);

INSERT INTO Game_Record (Game_ID, Player_ID, Points) VALUES (1, 3, 900);
